using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using LXProtocols.Acn.IO;
using System.IO;
using LXProtocols.Acn.Packets.sAcn;
using System.Threading;
using System.Collections.ObjectModel;
using LXProtocols.Acn.Helpers;

namespace LXProtocols.Acn.Sockets
{
    [ComVisible(true)]
    [Guid("C1E567CA-D546-45DD-A17B-D65339FF69EE")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComSourceInterfaces(typeof(IStreamingAcnSocket))]
    public class StreamingAcnSocket:AcnSocket,IStreamingAcnSocket, IProtocolFilter
    {
        /// <summary>
        /// Occurs when a new Streaming ACN DMX packet is recieved.
        /// </summary>
        public event EventHandler<NewPacketEventArgs<StreamingAcnDmxPacket>> NewPacket;

        /// <summary>
        /// Occurs when a new synchronization message is recieved from a source.
        /// </summary>
        public event EventHandler<NewPacketEventArgs<StreamingAcnSynchronizationPacket>> NewSynchronize;

        /// <summary>
        /// Occurs when a new discovery message is recieved from a source.
        /// </summary>
        /// <remarks>
        /// The discovery message contains information about what universes a source is transmitting.
        /// </remarks>
        public event EventHandler<NewPacketEventArgs<StreamingAcnDiscoveryPacket>> NewDiscovery;

        #region Setup and Initialisation

        public StreamingAcnSocket()
            : base(Guid.Empty)
        {
        }

        public StreamingAcnSocket(Guid sourceId, string sourceName)
            : base(sourceId)
        {
            if (sourceName.Length > 64)
                throw new ArgumentException("The source name must be no longer than 64 characters.");

            SourceName = sourceName;
            RegisterProtocolFilter(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Net.Sockets.Socket" />, and optionally disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            StopDiscovery();
            foreach (int universe in DmxUniverses.ToList())
                DropDmxUniverse(universe);

            base.Dispose(disposing);
        }

        #endregion

        #region Information

        /// <summary>
        /// The discovery universe used to send discovery messages.
        /// </summary>
        public const int DiscoveryUniverse = 64214;

        /// <summary>
        /// Gets or sets the name of the source name used when sending sACN packets.
        /// </summary>
        /// <remarks>
        /// This can be used by a recieving device to determine where the sACN data originated from.
        /// </remarks>
        public string SourceName { get; protected set; }

        private Dictionary<int,int> sequenceNumber = new Dictionary<int,int>();

        public int GetSequenceNumber(int universe)
        {
            int value = 0;
            sequenceNumber.TryGetValue(universe,out value);
            return value; 
        }

        public void IncrementSequenceNumber(int universe)
        {
            int value = 0;
            sequenceNumber.TryGetValue(universe, out value);

            value = (value >=255 ? 0 : value + 1);
                
            sequenceNumber[universe] = value;
        }



        private HashSet<int> dmxUniverses = new HashSet<int>();

        /// <summary>
        /// Gets a list of dmx universes this socket has joined to.
        /// </summary>
        public IEnumerable<int> DmxUniverses
        {
            get { return dmxUniverses; }
        }

        private int synchronizationAddress = 0;

        /// <summary>
        /// Gets or sets the synchronization address used for transmitting sync messages.
        /// </summary>
        /// <remarks>
        /// The synchronization address can be any universe and does not have to be one you are transmitting on.
        /// Setting the address to zero indicates that synchronization is not in use.
        /// </remarks>
        public int SynchronizationAddress
        {
            get { return synchronizationAddress; }
            set { synchronizationAddress = value; }
        }

        /// <summary>
        /// Gets the base address used before universe offset is calculated.
        /// </summary>
        /// <returns>The starting multi-cast address to use.</returns>
        protected virtual byte[] GetBaseAddress()
        {
            return new byte[] { 239, 255, 0, 0 };
        }

        public IPAddress GetUniverseAddress(int universe)
        {
            if ((universe < 0 || universe > 63999) && universe != DiscoveryUniverse)
                throw new InvalidOperationException("Unable to determine multicast group because the universe must be between 1 and 64000. Universes outside this range are not allowed.");

            byte[] group = GetBaseAddress();

            group[2] = (byte)((universe >> 8) & 0xff);     //Universe Hi Byte
            group[3] = (byte)(universe & 0xff);           //Universe Lo Byte

            return new IPAddress(group);
        }

        public IPEndPoint GetUniverseEndPoint(int universe)
        {
            return new IPEndPoint(GetUniverseAddress(universe), Port);
        }

        #endregion

        #region Traffic

        public void JoinDmxUniverse(int universe)
        {
            if (dmxUniverses.Contains(universe))
                throw new InvalidOperationException(string.Format("You have already joined the Dmx Universe {0}.",universe));

            //Join Group
            SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(GetUniverseAddress(universe),((IPEndPoint) LocalEndPoint).Address));

            //Add to the list of universes we have joined.
            dmxUniverses.Add(universe);
        }

        public void DropDmxUniverse(int universe)
        {
            if (!dmxUniverses.Contains(universe))
                throw new InvalidOperationException(string.Format("You are trying to drop the DMX Universe {0} but you are not a member. Please ensure you join the group before leaving!", universe));

            //Drop Group
            SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(GetUniverseAddress(universe)));

            //Add to the list of universes we have joined.
            dmxUniverses.Remove(universe);
        }

        public void SendDmx(int universe, byte[] dmxData)
        {
            SendDmx(universe, dmxData, 100);
        }

        /// <summary>
        /// Sends a DMX frame over streaming ACN.
        /// </summary>
        /// <remarks>
        /// The dmxData must include the start code. Please use the overload with startCode specified
        /// if the data does not include the start code.
        /// </remarks>
        /// <param name="universe">The streaming ACN universe between 1 and 3000.</param>
        /// <param name="dmxData">The DMX data including the start code.</param>
        /// <param name="priority">The sACN priority for the DMX data.</param>
        public void SendDmx(int universe, byte[] dmxData, byte priority)
        {
            //Start code of 0xFF indicates the start code is in the data.
            SendDmx(universe, 0xFF, dmxData, priority);
        }

        /// <summary>
        /// Sends a DMX frame over streaming ACN.
        /// </summary>
        /// <param name="universe">The streaming ACN universe between 1 and 3000.</param>
        /// <param name="startCode"></param>
        /// <param name="dmxData">The DMX data including the start code.</param>
        /// <param name="priority">The sACN priority for the DMX data.</param>
        public void SendDmx(int universe, byte startCode, byte[] dmxData, byte priority)
        {
            IncrementSequenceNumber(universe);

            StreamingAcnDmxPacket packet = new StreamingAcnDmxPacket();
            if (OverrideRootLayer) packet.Root = GetRootLayer();
            packet.Framing.SourceName = SourceName;
            packet.Framing.SyncPacketAddress = (short) SynchronizationAddress;
            packet.Framing.Universe = (short)universe;
            packet.Framing.Priority = priority;
            packet.Framing.SequenceNumber = (byte)GetSequenceNumber(universe);
            packet.Dmx.StartCode = startCode;
            packet.Dmx.Data = dmxData;

            SendPacket(packet, GetUniverseEndPoint(universe));
        }

        #region Synchronization

        /// <summary>
        /// Sends a DMX synchronization message to all interested parties.
        /// </summary>
        /// <remarks>
        /// This should be sent after every DMX frame to indicate to listeners that the DMX data is ready
        /// to be sent.
        /// </remarks>
        public void SendSynchronize()
        {
            if (SynchronizationAddress == 0)
                throw new InvalidOperationException("An attempt was made to send a synchronize message on an sACN socket not configured for synchronization. Please set SynchronizationAddress on the socket to a non zero universe.");

            StreamingAcnSynchronizationPacket packet = new StreamingAcnSynchronizationPacket();
            if (OverrideRootLayer) packet.Root = GetRootLayer();
            packet.Framing.SequenceNumber = (byte)GetSequenceNumber(SynchronizationAddress);
            packet.Framing.SynchronizationAddress = (short)SynchronizationAddress;

            SendPacket(packet, GetUniverseEndPoint(SynchronizationAddress));
        }

        /// <summary>
        /// Raises the synchronization message recieved event.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="packet">The packet.</param>
        protected virtual void RaiseNewSynchronize(IPEndPoint source, StreamingAcnSynchronizationPacket packet)
        {
            if (NewSynchronize != null)
                NewSynchronize(this, new NewPacketEventArgs<StreamingAcnSynchronizationPacket>(source, packet));
        }

        #endregion

        #region Discovery

        /// <summary>
        /// Starts listening to discovery messages on the discovery universe.
        /// </summary>
        public void StartDiscovery()
        {
            JoinDmxUniverse(DiscoveryUniverse);
        }

        /// <summary>
        /// Stops listening to discovery messages on the discovery universe.
        /// </summary>
        public void StopDiscovery()
        {
            if (dmxUniverses.Contains(DiscoveryUniverse))
                DropDmxUniverse(DiscoveryUniverse);
        }

        /// <summary>
        /// Sends a discovery message to all interested parties containing the universes we are transmitting.
        /// </summary>
        /// <remarks>
        /// The discovery message allows interested parties to know what universes we are transmitting.
        /// </remarks>
        /// <param name="universes">The universes we are currently sending.</param>
        public void SendDiscovery(IEnumerable<int> universes)
        {
            var chunkedUniverses = universes.ChunkBy(512);
            byte index = 1;
            foreach (List<int> chunk in chunkedUniverses)
            {
                StreamingAcnDiscoveryPacket packet = new StreamingAcnDiscoveryPacket();
                if (OverrideRootLayer) packet.Root = GetRootLayer();
                packet.Framing.SourceName = SourceName;
                packet.UniverseDiscovery.Page = index;
                packet.UniverseDiscovery.TotalPages = (byte) chunkedUniverses.Count();
                packet.UniverseDiscovery.Universes.AddRange(chunk);

                SendPacket(packet, GetUniverseEndPoint(DiscoveryUniverse));

                index++;
            }
        }


        /// <summary>
        /// Raises the discovery packet recieved event.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="packet">The packet.</param>
        protected virtual void RaiseNewDiscovery(IPEndPoint source, StreamingAcnDiscoveryPacket packet)
        {
            if (NewDiscovery != null)
                NewDiscovery(this, new NewPacketEventArgs<StreamingAcnDiscoveryPacket>(source, packet));
        }

        #endregion


        /// <summary>
        /// Raises the DMX data packet recieved event.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="newPacket">The new packet.</param>
        protected virtual void RaiseNewPacket(IPEndPoint source, StreamingAcnDmxPacket newPacket)
        {
            if(NewPacket != null)
                NewPacket(this, new NewPacketEventArgs<StreamingAcnDmxPacket>(source, newPacket));
        }

        #endregion

        #region IProtocolFilter Members

        /// <summary>
        /// Gets a list of protocol ID's that this filter supports.
        /// </summary>
        public IEnumerable<int> ProtocolId
        {
            get { return new[] { (int)ProtocolIds.sACN, (int)ProtocolIds.sACNExtended }; }
        }

        /// <summary>
        /// Processes the packet that have been recieved and allocated to this filter.
        /// </summary>
        /// <param name="source">The source IP address of the packet.</param>
        /// <param name="header">The header information for the ACN packet.</param>
        /// <param name="data">The data reader for the remaining packet data.</param>
        /// <remarks>
        /// Only packets that have supported protocol ID's will be sent to this function.
        /// </remarks>
        public void ProcessPacket(IPEndPoint source, AcnRootLayer header, AcnBinaryReader data)
        {
            AcnPacket packet = AcnPacket.ReadPacket(header, data);
            
            if (packet is StreamingAcnDmxPacket)
                RaiseNewPacket(source, (StreamingAcnDmxPacket) packet);
            if (packet is StreamingAcnSynchronizationPacket)
                RaiseNewSynchronize(source, (StreamingAcnSynchronizationPacket)packet);
            if (packet is StreamingAcnDiscoveryPacket)
                RaiseNewDiscovery(source, (StreamingAcnDiscoveryPacket)packet);
        }

        #endregion
    }
}

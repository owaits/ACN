using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Acn.IO;
using System.IO;
using Acn.Packets.sAcn;
using System.Threading;
using System.Collections.ObjectModel;

namespace Acn.Sockets
{
    public class StreamingAcnSocket:AcnSocket, IProtocolFilter
    {
        public event EventHandler<NewPacketEventArgs<DmxPacket>> NewPacket;

        #region Setup and Initialisation

        public StreamingAcnSocket(Guid sourceId, string sourceName)
            : base(sourceId)
        {
            if (sourceName.Length > 64)
                throw new ArgumentException("The source name must be no longer than 64 characters.");

            SourceName = sourceName;
            RegisterProtocolFilter(this);
        }

        #endregion

        #region Information

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



        private List<int> dmxUniverses = new List<int>();

        /// <summary>
        /// Gets a list of dmx universes this socket has joined to.
        /// </summary>
        public ReadOnlyCollection<int> DmxUniverses
        {
            get { return dmxUniverses.AsReadOnly(); }
        }

        public static IPAddress GetUniverseAddress(int universe)
        {
            if (universe < 0 || universe > 63999)
                throw new InvalidOperationException("Unable to determine multicast group because the universe must be between 1 and 64000. Universes outside this range are not allowed.");

            byte[] group = new byte[] { 239, 255, 0, 0 };

            group[2] = (byte)((universe >> 8) & 0xff);     //Universe Hi Byte
            group[3] = (byte)(universe & 0xff);           //Universe Lo Byte

            return new IPAddress(group);
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

        public void SendDmx(int universe, byte[] dmxData, byte priority)
        {
            IncrementSequenceNumber(universe);

            DmxPacket packet = new DmxPacket();
            packet.Framing.SourceName = SourceName;
            packet.Framing.Universe = (short) universe;
            packet.Framing.Priority = priority;
            packet.Framing.SequenceNumber = (byte) GetSequenceNumber(universe);
            packet.Dmx.Data = dmxData;            

            SendPacket(packet, GetUniverseAddress(universe));
        }

        protected virtual void RaiseNewPacket(IPEndPoint source, DmxPacket newPacket)
        {
            if(NewPacket != null)
                NewPacket(this, new NewPacketEventArgs<DmxPacket>(source, newPacket));
        }

        #endregion

        #region IProtocolFilter Members

        public int ProtocolId
        {
            get { return (int) ProtocolIds.sACN; }
        }

        public void ProcessPacket(IPEndPoint source, AcnRootLayer header, AcnBinaryReader data)
        {
            DmxPacket newPacket = AcnPacket.ReadPacket(header, data) as DmxPacket;
            if (newPacket != null)
            {
                RaiseNewPacket(source, newPacket);
            }
        }

        #endregion
    }
}

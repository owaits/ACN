using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Acn.Rdm;
using System.IO;
using Acn.Packets.sAcn;
using Acn.Sockets;
using Acn.ArtNet.Packets;
using Acn.ArtNet.IO;

namespace Acn.ArtNet.Sockets
{
    public class ArtNetSocket : Socket, IRdmSocket
    {
        public const int Port = 6454;

        public event UnhandledExceptionEventHandler UnhandledException;
        public event EventHandler<NewPacketEventArgs<ArtNetPacket>> NewPacket;
        public event EventHandler<NewPacketEventArgs<RdmPacket>> NewRdmPacket;
        public event EventHandler<NewPacketEventArgs<RdmPacket>> RdmPacketSent;

        public ArtNetSocket(UId rdmId)
            : base(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp)
        {
            RdmId = rdmId;
        }

        #region Information

        /// <summary>
        /// Gets or sets the RDM Id to use when sending packets.
        /// </summary>
        public UId RdmId { get; protected set; }

        private bool portOpen= false;

        public bool PortOpen
        {
            get { return portOpen; }
            set { portOpen = value; }
        }

        public IPAddress LocalIP { get; protected set; }

        public IPAddress LocalSubnetMask { get; protected set; }

        private static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        public IPAddress BroadcastAddress
        {
            get
            {
                if (LocalSubnetMask == null)
                    return IPAddress.Broadcast;
                return GetBroadcastAddress(LocalIP, LocalSubnetMask);
            }
        }

        private DateTime? lastPacket = null;

        public DateTime? LastPacket
        {
            get { return lastPacket; }
            protected set { lastPacket = value; }
        }

        #endregion
        
	
	

        public void Open(IPAddress localIp, IPAddress localSubnetMask)
        {
            LocalIP = localIp;
            LocalSubnetMask = localSubnetMask;

            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Bind(new IPEndPoint(LocalIP, Port));
            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            PortOpen = true;

            StartRecieve();
        }

        public void StartRecieve()
        {
            try
            {
                EndPoint localPort = new IPEndPoint(IPAddress.Any, Port);
                ArtNetRecieveData recieveState = new ArtNetRecieveData();
                BeginReceiveFrom(recieveState.buffer,0,recieveState.bufferSize,SocketFlags.None,ref localPort,new AsyncCallback(OnRecieve),recieveState);
            }
            catch (Exception ex)
            {
                OnUnhandledException(new ApplicationException("An error ocurred while trying to start recieving ArtNet.",ex));
            }
        }

        private void OnRecieve(IAsyncResult state)
        {
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any,0);

            if (PortOpen)
            {
                try
                {
                    ArtNetRecieveData recieveState = (ArtNetRecieveData)(state.AsyncState);

                    if (recieveState != null)
                    {
                        recieveState.DataLength = EndReceiveFrom(state, ref remoteEndPoint);

                        //Protect against UDP loopback where we recieve our own packets.
                        if (LocalEndPoint != remoteEndPoint && recieveState.Valid)
                        {
                            LastPacket = DateTime.Now;

                            ProcessPacket((IPEndPoint) remoteEndPoint, ArtNetPacket.Create(recieveState));
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnUnhandledException(ex);
                }
                finally
                {
                    //Attempt to recieve another packet.
                    StartRecieve();
                }
            }
        }

        private void ProcessPacket(IPEndPoint source, ArtNetPacket packet)
        {
            if(packet != null)
            {
                if(NewPacket != null)
                    NewPacket(this, new NewPacketEventArgs<ArtNetPacket>(source,packet));
                
                ArtRdmPacket rdmPacket = packet as ArtRdmPacket;
                if(rdmPacket != null && NewRdmPacket != null)
                {
                    RdmPacket rdm = RdmPacket.ReadPacket(new RdmBinaryReader(new MemoryStream(rdmPacket.RdmData)));
                    NewRdmPacket(this,new NewPacketEventArgs<RdmPacket>(source,rdm)); 
                }
            }
        }

        protected void OnUnhandledException(Exception ex)
        {
            if (UnhandledException != null) UnhandledException(this, new UnhandledExceptionEventArgs((object)ex, false));
        }

        #region Sending

        public void Send(ArtNetPacket packet)
        {
            SendTo(packet.ToArray(), new IPEndPoint(BroadcastAddress,Port));
        }

        public void Send(ArtNetPacket packet, RdmEndPoint address)
        {
            SendTo(packet.ToArray(), new IPEndPoint(address.IpAddress, Port));
        }

        public void SendRdm(RdmPacket packet, RdmEndPoint targetAddress, UId targetId)
        {
            SendRdm(packet, targetAddress, targetId, RdmId);
        }

        public void SendRdm(RdmPacket packet, RdmEndPoint targetAddress, UId targetId, UId sourceId)
        {
            //Fill in addition details
            packet.Header.SourceId = sourceId;
            packet.Header.DestinationId = targetId;

            //Sub Devices
            if (targetId is SubDeviceUId)
                packet.Header.SubDevice = ((SubDeviceUId)targetId).SubDeviceId;

            //Create Rdm Packet
            MemoryStream rdmData = new MemoryStream();
            RdmBinaryWriter rdmWriter = new RdmBinaryWriter(rdmData);

            //Write the RDM packet
            RdmPacket.WritePacket(packet, rdmWriter);

            //Write the checksum
            rdmWriter.WriteNetwork((short)(RdmPacket.CalculateChecksum(rdmData.GetBuffer()) + (int) RdmVersions.SubMessage + (int) DmxStartCodes.RDM));

            //Create sACN Packet
            ArtRdmPacket rdmPacket = new ArtRdmPacket();
            rdmPacket.Address = (byte)targetAddress.Universe;
            rdmPacket.SubStartCode = (byte)RdmVersions.SubMessage;
            rdmPacket.RdmData = rdmData.GetBuffer();

            Send(rdmPacket, targetAddress);

            if(RdmPacketSent != null)
                RdmPacketSent(this, new NewPacketEventArgs<RdmPacket>(new IPEndPoint(targetAddress.IpAddress, Port), packet));
        }

        public void SendRdm(List<RdmPacket> packets, RdmEndPoint targetAddress, UId targetId)
        {
            if(packets.Count <1)
                throw new ArgumentException("Rdm packets list is empty.");

            RdmPacket primaryPacket = packets[0];

            //Create sACN Packet
            ArtRdmSubPacket rdmPacket = new ArtRdmSubPacket();
            rdmPacket.DeviceId = targetId;
            rdmPacket.RdmVersion = (byte)RdmVersions.SubMessage;
            rdmPacket.Command = primaryPacket.Header.Command;
            rdmPacket.ParameterId = primaryPacket.Header.ParameterId;
            rdmPacket.SubDevice = (short) primaryPacket.Header.SubDevice;
            rdmPacket.SubCount = (short) packets.Count;

            MemoryStream rdmData = new MemoryStream();
            RdmBinaryWriter dataWriter = new RdmBinaryWriter(rdmData);

            foreach (RdmPacket item in packets)
                RdmPacket.WritePacket(item, dataWriter, true);

            rdmPacket.RdmData = rdmData.ToArray();

            Send(rdmPacket,targetAddress);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            PortOpen = false;

            base.Dispose(disposing);
        }
    }
}

#region Copyright © 2011 Oliver Waits
//______________________________________________________________________________________________________________
// Remote Device Management
// Copyright © 2011 Oliver Waits
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//______________________________________________________________________________________________________________
#endregion
   

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Rdm;
using Acn.Packets.sAcn;
using Acn.IO;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Acn.Sockets;

namespace Acn.RdmNet.Sockets
{
    public class RdmNetSocket : AcnSocket, IProtocolFilter, IRdmSocket
    {
        public static int RdmNetPort = 5569;

        public event EventHandler<NewPacketEventArgs<RdmPacket>> NewRdmPacket;
        public event EventHandler<NewPacketEventArgs<RdmPacket>> RdmPacketSent;

        public RdmNetSocket(UId rdmId, Guid sourceId, string sourceName)
            : base(sourceId)
        {
            RdmSourceId = rdmId;
            SourceName = sourceName;

            RegisterProtocolFilter(this);
        }        

        public override int Port
        {
            get { return RdmNetPort; }
        }


        #region Information

        /// <summary>
        /// Gets or sets the RDM Id to use when sending packets.
        /// </summary>
        public UId RdmSourceId { get; set; }

        public string SourceName { get; set; }

        /// <summary>
        /// Gets or sets whether RDM packets are blocked by this socket.
        /// </summary>
        public bool BlockRDM { get; set; }

        #endregion

        protected void RaiseNewRdmPacket(RdmEndPoint source, RdmPacket packet)
        {
            if (NewRdmPacket != null)
                NewRdmPacket(this, new NewPacketEventArgs<RdmPacket>(source, packet));
        }

        public void SendRdm(RdmPacket packet, RdmEndPoint targetAddress, UId targetId)
        {
            SendRdm(packet, targetAddress, targetId, RdmSourceId);
        }

        public void SendRdm(RdmPacket packet, RdmEndPoint targetAddress, UId targetId, UId sourceId)
        {
            if (BlockRDM)
                return; 

            //Fill in addition details
            packet.Header.SourceId = sourceId;
            packet.Header.DestinationId = targetId;

            SubDeviceUId id = targetId as SubDeviceUId;
            if (id != null)
                packet.Header.SubDevice = id.SubDeviceId;

            //Create Rdm Packet
            MemoryStream rdmData = new MemoryStream();
            RdmBinaryWriter rdmWriter = new RdmBinaryWriter(rdmData);

            //Write the RDM sub-start code.
            rdmWriter.Write((byte)RdmVersions.SubMessage);

            //Write the RDM packet
            RdmPacket.WritePacket(packet,rdmWriter);

            //Write the checksum
            rdmWriter.WriteNetwork((short)RdmPacket.CalculateChecksum(rdmData.GetBuffer()) + (byte)DmxStartCodes.RDM);

            //Create sACN Packet
            RdmNetPacket dmxPacket = new RdmNetPacket();
            dmxPacket.Framing.SourceName = SourceName;
            dmxPacket.Framing.EndpointID = (short) targetAddress.Universe;
            dmxPacket.RdmNet.RdmData = rdmData.GetBuffer();

            SendPacket(dmxPacket, targetAddress);

            RaiseRdmPacketSent(new NewPacketEventArgs<RdmPacket>(targetAddress, packet));
        }

        protected virtual void RaiseRdmPacketSent(NewPacketEventArgs<RdmPacket> args)
        {
            if (RdmPacketSent != null)
                RdmPacketSent(this, args);
        }

        #region IProtocolFilter Members

        /// <summary>
        /// Gets a list of protocol ID's that this filter supports.
        /// </summary>
        IEnumerable<int> IProtocolFilter.ProtocolId
        {
            get { return new []{(int) ProtocolIds.RdmNet}; }
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
        void IProtocolFilter.ProcessPacket(IPEndPoint source, AcnRootLayer header, AcnBinaryReader data)
        {
            RdmNetPacket newPacket = AcnPacket.ReadPacket(header,data) as RdmNetPacket;
            if (newPacket != null)
            {
                RdmBinaryReader dmxReader = new RdmBinaryReader(new MemoryStream(newPacket.RdmNet.RdmData));

                //Skip Start Code and sub-start code
                dmxReader.BaseStream.Seek(1, SeekOrigin.Begin);

                RdmPacket rdmPacket = RdmPacket.ReadPacket(dmxReader);
                RaiseNewRdmPacket(new RdmEndPoint(source,newPacket.Framing.EndpointID), rdmPacket);
            }
        }

        #endregion
    }
}

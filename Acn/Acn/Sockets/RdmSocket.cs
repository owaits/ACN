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

namespace Acn.Sockets
{
    public class RdmSocket : StreamingAcnSocket, IProtocolFilter, IRdmSocket
    {
        public event EventHandler<NewPacketEventArgs<RdmPacket>> NewRdmPacket;
        public event EventHandler<NewPacketEventArgs<RdmPacket>> RdmPacketSent;

        public RdmSocket(UId rdmId, Guid sourceId, string sourceName)
            : base(sourceId, sourceName)
        {
            RdmSourceId = rdmId;
        }

        #region Information

        /// <summary>
        /// Gets or sets the RDM Id to use when sending packets.
        /// </summary>
        public UId RdmSourceId { get; set; }

        #endregion

        public IPAddress SquawkGroup
        {
            get { return new IPAddress(new byte[] { 239, 255, 250, 0 }); }
        }

        protected void RaiseNewRdmPacket(IPEndPoint source, RdmPacket packet)
        {
            if (NewRdmPacket != null)
                NewRdmPacket(this, new NewPacketEventArgs<RdmPacket>(source, packet));
        }

        public void SendRdm(RdmPacket packet, RdmAddress targetAddress, UId targetId)
        {
            SendRdm(packet, targetAddress, targetId, RdmSourceId);
        }

        public void SendRdm(RdmPacket packet, RdmAddress targetAddress, UId targetId, UId sourceId)
        {
            //Fill in addition details
            packet.Header.SourceId = sourceId;
            packet.Header.DestinationId = targetId;

            SubDeviceUId id = targetId as SubDeviceUId;
            if (id != null)
                packet.Header.SubDevice = id.SubDeviceId;

            //Create Rdm Packet
            MemoryStream rdmData = new MemoryStream();
            RdmBinaryWriter rdmWriter = new RdmBinaryWriter(rdmData);

            //Write the RDM start code.
            rdmWriter.Write((byte)DmxStartCodes.RDM);

            //Write the RDM sub-start code.
            rdmWriter.Write((byte)RdmVersions.SubMessage);

            //Write the RDM packet
            RdmPacket.WritePacket(packet,rdmWriter);

            //Write the checksum
            rdmWriter.WriteNetwork((short) RdmPacket.CalculateChecksum(rdmData.GetBuffer()));

            //Create sACN Packet
            RdmNetPacket dmxPacket = new RdmNetPacket();
            dmxPacket.Framing.SourceName = SourceName;
            dmxPacket.Dmx.Data = rdmData.GetBuffer();

            SendPacket(dmxPacket, targetAddress.IpAddress);

            if (RdmPacketSent != null)
                RdmPacketSent(this, new NewPacketEventArgs<RdmPacket>(new IPEndPoint(targetAddress.IpAddress, Port), packet));
        }



        #region IProtocolFilter Members

        int IProtocolFilter.ProtocolId
        {
            get { return (int) ProtocolIds.RdmNet; }
        }

        void IProtocolFilter.ProcessPacket(IPEndPoint source, AcnRootLayer header, AcnBinaryReader data)
        {
            RdmNetPacket newPacket = AcnPacket.ReadPacket(header,data) as RdmNetPacket;
            if (newPacket != null)
            {
                RdmBinaryReader dmxReader = new RdmBinaryReader(new MemoryStream(newPacket.Dmx.Data));

                //Skip Start Code and sub-start code
                dmxReader.BaseStream.Seek(2, SeekOrigin.Begin);

                RdmPacket rdmPacket = RdmPacket.ReadPacket(dmxReader);
                RaiseNewRdmPacket(source, rdmPacket);
            }
        }

        #endregion
    }
}

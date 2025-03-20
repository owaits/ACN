using LXProtocols.Acn.IO;
using LXProtocols.Acn.Packets.RdmNet.RPT;
using LXProtocols.Acn.Rdm;
using LXProtocols.Acn.RdmNet;
using LXProtocols.Acn.RdmNet.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace LXProtocols.Acn.Packets.RdmNet.LLRP
{
    public class LLRPCommandPdu : AcnPdu
    {
        public LLRPCommandPdu()
           : base((int)1, 1)
        {
            Flags = PduFlags.Extended;
        }

        #region PDU Contents

        public RdmPacket RDM { get; set; }

        #endregion

        #region Read and Write

        protected override void ReadData(AcnBinaryReader data)
        {
            RdmBinaryReader dmxReader = new RdmBinaryReader(new MemoryStream(data.ReadBytes(Length)));

            //Skip Start Code and sub-start code
            dmxReader.BaseStream.Seek(1, SeekOrigin.Begin);

            RDM = RdmPacket.ReadPacket(dmxReader);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            //Create Rdm Packet
            MemoryStream rdmData = new MemoryStream();
            RdmBinaryWriter rdmWriter = new RdmBinaryWriter(rdmData);

            //Write the RDM sub-start code.
            rdmWriter.Write((byte)RdmVersions.SubMessage);

            //Write the RDM packet
            RdmPacket.WritePacket(RDM, rdmWriter);

            //Write the checksum
            ushort checksum = (ushort)(RdmPacket.CalculateChecksum(rdmData.ToArray()) + (int)DmxStartCodes.RDM);
            rdmWriter.WriteNetwork(checksum);

            //Flush the writer to ensure the buffer is up to date.
            rdmWriter.Flush();

            data.Write(rdmData.ToArray());
        }

        #endregion
    }
}

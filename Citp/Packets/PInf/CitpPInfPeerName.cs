using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.PInf
{
    public class CitpPInfPeerName:CitpPInfHeader
    {
        public const string PacketType = "PNam";

        #region Constructors

        public CitpPInfPeerName()
        {
            LayerContentType = PacketType;
        }

        public CitpPInfPeerName(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content

        public string Name { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
            Name = data.ReadUcs1();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.WriteUcs1(Name);
        }

        #endregion
    }
}

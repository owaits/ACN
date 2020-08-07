using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.PInf
{
    public class CitpPInfHeader : CitpHeader
    {
        public const string PacketType = "PINF";

        public CitpPInfHeader():base(PacketType)
        {
        }

        #region Packet Content

        public string LayerContentType { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
            LayerContentType = data.ReadCookie();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.WriteCookie(LayerContentType);
        }

        #endregion
    }
}

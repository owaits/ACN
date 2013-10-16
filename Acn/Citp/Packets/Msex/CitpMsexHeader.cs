using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Citp.IO;

namespace Citp.Packets
{
    public class CitpMsexHeader : CitpHeader
    {
        public const string PacketType = "MSEX";

        public CitpMsexHeader():base(PacketType)
        {
            MsexVersion = 1.0f;
        }

        #region Packet Content

        public float MsexVersion { get; set; }

        public string LayerContentType { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            MsexVersion = data.ReadByte();
            MsexVersion += (float) data.ReadByte() / 10;
            LayerContentType = data.ReadCookie();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            byte majorVersion = (byte)MsexVersion;
            byte minorVersion = (byte) ((MsexVersion - (float) majorVersion) * 10);
            data.Write(majorVersion);
            data.Write(minorVersion);
            data.WriteCookie(LayerContentType);
        }

        #endregion
    }
}

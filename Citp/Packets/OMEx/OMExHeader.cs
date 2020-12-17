using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.OMEx
{
    public class OMExHeader:CitpHeader
    {
        public const string PacketType = "OMEX";

        #region Setup and Initialisation

        public OMExHeader(string contentType)
            : base(PacketType)
        {
            OMExContentType = contentType;
        }

        #endregion

        #region Packet Content

        public float OMExVersion { get; set; }

        public string OMExContentType { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            OMExVersion = data.ReadByte();
            OMExVersion += (float)data.ReadByte() / 10;
            OMExContentType = data.ReadCookie();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            byte majorVersion = (byte)OMExVersion;
            byte minorVersion = (byte)((OMExVersion - (float)majorVersion) * 10);
            data.Write(majorVersion);
            data.Write(minorVersion);
            data.WriteCookie(OMExContentType);

        }

        #endregion
    }
}

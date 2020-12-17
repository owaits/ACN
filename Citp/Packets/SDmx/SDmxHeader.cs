using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.SDmx
{
    public class SDmxHeader:CitpHeader
    {
        public const string PacketType = "SDMX";

        #region Setup and Initialisation

        public SDmxHeader(string contentType)
            : base(PacketType)
        {
            SDmxContentType = contentType;
        }

        #endregion

        #region Packet Content

        public string SDmxContentType { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            SDmxContentType = data.ReadCookie();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.WriteCookie(SDmxContentType);

        }

        #endregion
    }
}

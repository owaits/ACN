using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.FInf
{
    public class FInfHeader:CitpHeader
    {
        public const string PacketType = "FINF";

        #region Setup and Initialisation
        public FInfHeader()
            : base(PacketType)
        {

        }

        public FInfHeader(string contentType)
            : base(PacketType)
        {
            ContentType = contentType;
        }

        #endregion

        #region Packet Content

        public string ContentType { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            ContentType = data.ReadCookie();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.WriteCookie(ContentType);

        }

        #endregion
    }
}

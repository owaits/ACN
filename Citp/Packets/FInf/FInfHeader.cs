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
            FInfContentType = contentType;
        }

        #endregion

        #region Packet Content

        public string FInfContentType { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            FInfContentType = data.ReadCookie();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.WriteCookie(FInfContentType);

        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.FPtc
{
    public class FPtcHeader:CitpHeader
    {
        public const string PacketType = "FPTC";

        #region Setup and Initialisation

        public FPtcHeader(string contentType)
            : base(PacketType)
        {
            FPtcContentType = contentType;
        }

        #endregion

        #region Packet Content

        public string FPtcContentType { get; set; }

        public uint FPtcContentHint { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            FPtcContentType = data.ReadCookie();
            FPtcContentHint = data.ReadUInt32();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.WriteCookie(FPtcContentType);
            data.Write(FPtcContentHint);
        }

        #endregion
    }
}

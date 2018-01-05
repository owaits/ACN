using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citp.IO;

namespace Citp.Packets.FPtc
{
    public class FPtcHeader:CitpHeader
    {
        public const string PacketType = "FPTC";

        #region Setup and Initialisation

        public FPtcHeader(string contentType)
            : base(PacketType)
        {
            ContentType = contentType;
        }

        #endregion

        #region Packet Content

        public string ContentType { get; set; }

        public uint ContentHint { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            ContentType = data.ReadCookie();
            ContentHint = data.ReadUInt32();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.WriteCookie(ContentType);
            data.Write(ContentHint);
        }

        #endregion
    }
}

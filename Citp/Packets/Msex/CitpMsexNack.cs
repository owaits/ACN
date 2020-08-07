using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.Msex
{
    public class CitpMsexNack:CitpMsexHeader
    {
        public new const string PacketType = "Nack";

        #region Constructors

        public CitpMsexNack()
            : base()
        {
            MsexVersion = CitpMsexVersions.Msex10Version;
            LayerContentType = PacketType;
        }

        public CitpMsexNack(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content
        public string ReceivedContentType { get; set; }
        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
            ReceivedContentType = data.ReadCookie();
            
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            if (ReceivedContentType != null)
            {
                data.WriteCookie(ReceivedContentType);
            }
        }

        #endregion
    }
}

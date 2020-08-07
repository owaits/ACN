using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Citp.IO;
using LXProtocols.Citp.Packets.Msex;

namespace LXProtocols.Citp.Packets
{
    public class CitpMsexHeader : CitpHeader
    {
        public const string PacketType = "MSEX";        

        public CitpMsexHeader():base(PacketType)
        {
            MsexVersion = CitpMsexVersions.Msex10Version;
        }        

        #region Packet Content

        /// <summary>
        /// Gets or sets the msex version.
        /// </summary>
        /// <value>
        /// The msex version.
        /// </value>
        public Version MsexVersion { get; set; }        

        public string LayerContentType { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
            byte majorVersion = data.ReadByte();
            byte minorVersion = data.ReadByte();
            MsexVersion = new Version(majorVersion, minorVersion);
            LayerContentType = data.ReadCookie();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            //Write out the Msex version, if it isn't set send as 1.0.
            if (MsexVersion != null)
            {
                data.Write(((byte)MsexVersion.Major));
                data.Write(((byte)MsexVersion.Minor));
            }
            else
            {
                byte majorVersion = 1;
                byte minorVersion = 0;
                data.Write(majorVersion);
                data.Write(minorVersion);
            }
            data.WriteCookie(LayerContentType);
        }

        #endregion
    }
}

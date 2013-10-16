using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Citp.IO;

namespace Citp.Packets.Msex
{
    public class CitpMsexClientInformation:CitpMsexHeader
    {
        public const string PacketType = "CInf";

        #region Constructors

        public CitpMsexClientInformation()
            : base()
        {
            LayerContentType = PacketType;
        }

        public CitpMsexClientInformation(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content

        private List<UInt16> supportedMSEXVersions = new List<ushort>();

        public List<UInt16> SupportedMSEXVersions
        {
            get { return supportedMSEXVersions; }
        }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            int count = data.ReadByte();

            for (int n = 0; n < count; n++)
            {
                SupportedMSEXVersions.Add(data.ReadUInt16());
            }
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write(SupportedMSEXVersions.Count);

            foreach (UInt32 version in SupportedMSEXVersions)
                data.Write(version);
        }

        #endregion
    }
}

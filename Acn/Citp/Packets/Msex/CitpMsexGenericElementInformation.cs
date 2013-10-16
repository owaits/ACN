using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Citp.IO;

namespace Citp.Packets.Msex
{
    public class CitpMsexGenericElementInformation:CitpMsexHeader
    {
        public const string PacketType = "GLEI";

        #region Constructors

        public CitpMsexGenericElementInformation()
        {
            LayerContentType = PacketType;
        }

        public CitpMsexGenericElementInformation(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
        }

        #endregion
    }
}

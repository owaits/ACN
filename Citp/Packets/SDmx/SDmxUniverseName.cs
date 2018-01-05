using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citp.IO;

namespace Citp.Packets.SDmx
{
    public class SDmxUniverseName:SDmxHeader
    {
        public const string PacketType = "UNam";

        #region Setup and Initialisation

        public SDmxUniverseName()
            : base(PacketType)
        {    
        }

        #endregion

        #region Packet Content

        public byte UniverseIndex { get; set; }

        public string UniverseName { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
            UniverseIndex = data.ReadByte();
            UniverseName = data.ReadUcs1();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.Write(UniverseIndex);
            data.WriteUcs1(UniverseName);
        }

        #endregion
    }
}

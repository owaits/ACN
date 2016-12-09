using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citp.IO;

namespace Citp.Packets.SDmx
{
    public class SDmxSetExternalUniverseSource:SDmxHeader
    {
        public const string PacketType = "SXUS";

        #region Setup and Initialisation

        public SDmxSetExternalUniverseSource()
            : base(PacketType)
        {    
        }

        #endregion

        #region Packet Content

        public byte UniverseIndex { get; set; }

        public string ConnectionString { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
            UniverseIndex = data.ReadByte();
            ConnectionString = data.ReadUcs1();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.Write(UniverseIndex);
            data.WriteUcs1(ConnectionString);
        }

        #endregion
    }
}

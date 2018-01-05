using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citp.IO;

namespace Citp.Packets.SDmx
{
    public class SDmxEncryptionIdentifier:SDmxHeader
    {
        public const string PacketType = "EnId";

        #region Setup and Initialisation

        public SDmxEncryptionIdentifier()
            : base(PacketType)
        {    
        }

        #endregion

        #region Packet Content

        public string Identifier { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
            Identifier = data.ReadUcs1();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.WriteUcs1(Identifier);
        }

        #endregion
    }
}

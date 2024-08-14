using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.SDmx
{
    public class SDmxChannelBlock:SDmxHeader
    {
        public new const string PacketType = "ChBk";

        #region Setup and Initialisation

        public SDmxChannelBlock()
            : base(PacketType)
        {    
        }

        #endregion

        #region Packet Content

        public byte Blind { get; set; }

        public byte UniverseIndex { get; set; }

        public ushort FirstChannel { get; set; }

        public byte[] ChannelLevels { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            Blind = data.ReadByte();
            UniverseIndex = data.ReadByte();
            FirstChannel = data.ReadUInt16();
            int channelCount = data.ReadUInt16();
            ChannelLevels = data.ReadData(channelCount);
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.Write(Blind);
            data.Write(UniverseIndex);
            data.Write(FirstChannel);
            data.Write((ushort) ChannelLevels.Length);
            data.Write(ChannelLevels);
        }

        #endregion
    }
}

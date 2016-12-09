using Citp.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citp.Packets.SDmx
{
    public class SDmxChannelList:SDmxHeader
    {
        public const string PacketType = "ChLs";

        #region Setup and Initialisation

        public SDmxChannelList()
            : base(PacketType)
        {    
        }

        #endregion

        #region Packet Content

        public struct ChannelLevel
        {
            public byte UniverseIndex;
            public ushort Channel;
            public byte Level;
        }

        private List<ChannelLevel> channelLevels = new List<ChannelLevel>();

        public List<ChannelLevel> ChannelLevels
        {
            get { return channelLevels; }
            set { channelLevels = value; }
        }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            int count = data.ReadUInt16();
            for (int n = 0; n < count; n++)
            {
                ChannelLevel level = new ChannelLevel()
                {
                    UniverseIndex = data.ReadByte(),
                    Channel = data.ReadUInt16(),
                    Level = data.ReadByte()
                };
                ChannelLevels.Add(level);
            }
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((ushort)ChannelLevels.Count);
            foreach (ChannelLevel capability in ChannelLevels)
            {
                data.Write(capability.UniverseIndex);
                data.Write(capability.Channel);
                data.Write(capability.Level);
            }
        }

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.FPtc
{
    public class FPtcPatch : FPtcHeader
    {
        public const string PacketType = "Ptch";

        #region Setup and Initialisation

        public FPtcPatch()
            : base(PacketType)
        {
        }

        #endregion

        #region Packet Content

        public ushort FixtureIdentifier { get; set; }

        public byte Universe { get; set; }

        public ushort Channel { get; set; }

        public ushort ChannelCount { get; set; }

        public string FixtureMake { get; set; }

        public string FixtureName { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            FixtureIdentifier = data.ReadUInt16();
            Universe = data.ReadByte();
            data.ReadByte();
            Channel = data.ReadUInt16();
            ChannelCount = data.ReadUInt16();
            FixtureMake = data.ReadUcs1();
            FixtureName = data.ReadUcs1();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.Write(FixtureIdentifier);
            data.Write(Universe);
            data.Write((byte)0);
            data.Write(Channel);
            data.Write(ChannelCount);
            data.WriteUcs1(FixtureMake);
            data.WriteUcs1(FixtureName);
        }

        #endregion
    }
}

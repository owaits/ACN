using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citp.IO;

namespace Citp.Packets.FInf
{
    public class FInfFrames:FInfHeader
    {
        public const string PacketType = "Fram";

        #region Setup and Initialisation

        public FInfFrames()
            : base(PacketType)
        {
        }

        public FInfFrames(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content

        public ushort FixtureIdentifier { get; set; }

        private List<string> frameFilters = new List<string>();

        public List<string> FrameFilters
        {
            get { return frameFilters; }
            set { frameFilters = value; }
        }

        private List<string> frameGobos = new List<string>();

        public List<string> FrameGobos
        {
            get { return frameGobos; }
            set { frameGobos = value; }
        }


        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            FixtureIdentifier = data.ReadUInt16();
            int filterCount = data.ReadByte();
            int goboCount = data.ReadByte();

            for (int n = 0; n < filterCount; n++)
                FrameFilters.Add(data.ReadUcs1());
            for (int n = 0; n < goboCount; n++)
                FrameGobos.Add(data.ReadUcs1());
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write(FixtureIdentifier);
            data.Write((byte) FrameFilters.Count);
            data.Write((byte)FrameGobos.Count);

            foreach (string name in FrameFilters)
                data.WriteUcs1(name);
            foreach (string name in FrameGobos)
                data.WriteUcs1(name);
        }

        #endregion
    }
}

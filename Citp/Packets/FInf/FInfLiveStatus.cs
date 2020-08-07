using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.FInf
{
    public struct LiveStatus
    {
        public LiveStatus(byte flagSize)
        {
            this.fixtureIdentifier = 0;
            this.flagMask = new byte[flagSize];
            this.flags = new byte[flagSize];
        }

        private ushort fixtureIdentifier;

        public ushort FixtureIdentifier
        {
            get { return fixtureIdentifier; }
            set { fixtureIdentifier = value; }
        }

        private byte[] flagMask;

        public byte[] FlagMask
        {
            get { return flagMask; }
            set { flagMask = value; }
        }

        private byte[] flags;

        public byte[] Flags
        {
            get { return flags; }
            set { flags = value; }
        }
    }

    public class FInfLiveStatus : FInfHeader
    {
        public const string PacketType = "LSta";

        #region Setup and Initialisation

        public FInfLiveStatus()
            : base(PacketType)
        {
        }

        #endregion

        #region Packet Content

        public byte FlagSize { get; set; }

        private List<LiveStatus> liveStatus = new List<LiveStatus>();

        public List<LiveStatus> LiveStatus
        {
            get { return liveStatus; }
            set { liveStatus = value; }
        }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            int liveStatusCount = data.ReadUInt16();
            FlagSize = data.ReadByte();

            LiveStatus.Clear();
            for (int n = 0; n < liveStatusCount; n++)
            {
                LiveStatus status = new LiveStatus()
                {
                    FixtureIdentifier = data.ReadUInt16(),
                    FlagMask = data.ReadBytes(FlagSize),
                    Flags = data.ReadBytes(FlagSize)
                };

                LiveStatus.Add(status);
            }
                
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.Write((ushort)LiveStatus.Count);
            data.Write(FlagSize);
            foreach (LiveStatus status in LiveStatus)
            {
                data.Write(status.FixtureIdentifier);
                data.Write(status.FlagMask);
                data.Write(status.Flags);
            }                
        }

        #endregion
    }
}

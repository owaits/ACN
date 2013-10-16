using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citp.IO;

namespace Citp.Packets.FInf
{
    public struct LiveStatus
    {
        public LiveStatus(byte flagSize)
        {
            FlagMask = new byte[flagSize];
            Flags = new byte[flagSize];
        }

        public ushort FixtureIdentifier { get; set; }

        public byte[] FlagMask { get; set; }

        public byte[] Flags { get; set; }
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

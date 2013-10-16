using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citp;
using Citp.IO;

namespace Citp.Packets.CaEx
{
    public struct CueRecordingOption
    {
        public string Name;
        public List<string> Choices;
        public string Help;
    }

    public class CaExSetCueRecordingCapabilities:CaExHeader
    {
        #region Setup and Initialisation

        public CaExSetCueRecordingCapabilities()
            : base(CaExContentCodes.SetCueRecordingCapabilities)
        {    
        }

        #endregion

        #region Packet Content

        public bool Availability { get; set; }

        private List<CueRecordingOption> options = new List<CueRecordingOption>();

        public List<CueRecordingOption> Options
        {
            get { return options; }
            set { options = value; }
        }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
            Availability = data.ReadBoolean();
            int optionCount = data.ReadByte();
            Options = new List<CueRecordingOption>(optionCount);
            for (int n = 0; n < optionCount; n++)
            {
                CueRecordingOption option = new CueRecordingOption()
                {
                    Name = data.ReadUcs2(),
                    Choices = new List<string>(data.ReadUcs2().Split("\t"[0])),
                    Help = data.ReadUcs2()
                };
                Options.Add(option);
            }

        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.Write(Availability);
            data.Write((byte) Options.Count);
            foreach (CueRecordingOption option in Options)
            {
                data.WriteUcs2(option.Name);
                data.WriteUcs2(string.Join("\t",option.Choices));
                data.WriteUcs2(option.Help);
            }
        }

        #endregion
    }
}

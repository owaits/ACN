using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citp.IO;
using Citp;

namespace Citp.Packets.CaEx
{
    public struct RecordCueOption
    {
        public string Name;
        public string Value;
    }

    public class CaExRecordCue:CaExHeader
    {
        #region Setup and Initialisation

        public CaExRecordCue()
            : base(CaExContentCodes.RecordCue)
        {    
        }

        #endregion

        #region Packet Content

        public List<RecordCueOption> Options { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
            int optionCount = data.ReadByte();
            Options = new List<RecordCueOption>(optionCount);
            for (int n = 0; n < optionCount; n++)
            {
                RecordCueOption option = new RecordCueOption() 
                { 
                    Name = data.ReadUcs2(),
                    Value = data.ReadUcs2()
                };
                Options.Add(option);
            }
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.Write((byte) Options.Count);
            foreach(RecordCueOption option in Options)
            {
                data.WriteUcs2(option.Name);
                data.WriteUcs2(option.Value);
            }
        }

        #endregion
    }
}

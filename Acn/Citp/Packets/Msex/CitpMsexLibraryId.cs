using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Citp.Packets.Msex
{
    public class CitpMsexLibraryId
    {
        public CitpMsexLibraryId(byte number)
        {
            ParseNumber(number);
        }

        public CitpMsexLibraryId(byte level, byte level1, byte level2, byte level3)
        {
            Level = level;
            Level1 = level1;
            Level2 = level2;
            Level3 = level3;
        }

        public CitpMsexLibraryId()
        {
            Level = 0;
            Level1 = 0;
            Level2 = 0;
            Level3 = 0;
        }

        public byte Level { get; set; }

        public byte Level1 { get; set; }

        public byte Level2 { get; set; }

        public byte Level3 { get; set; }

        public byte ToNumber()
        {
            if (Level != 1)
                throw new InvalidOperationException("Only Ids at level 1 can be converted to a number.");

            return Level1;
        }

        public void ParseNumber(byte number)
        {
            Level = 1;
            Level1 = number;
            Level2 = 0;
            Level3 = 0;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}/{2}/{3}",Level,Level1,Level2,Level3);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Citp.IO;

namespace Citp.Packets
{
    public abstract class CitpPacket
    {
        public abstract void ReadData(CitpBinaryReader data);

        public abstract void WriteData(CitpBinaryWriter data);   

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.Acn.Packets.Sdt.Addresses
{
    public class NullAddress:SdtAddress
    {
        public NullAddress():base(SdtAddressTypes.Null)
        {
        }

        public override void WriteData(IO.AcnBinaryWriter data)
        {
        }
    }
}

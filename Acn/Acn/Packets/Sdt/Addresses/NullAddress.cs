using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acn.Packets.Sdt.Addresses
{
    public class NullAddress:SdtAddress
    {
        public NullAddress():base(SdtAddressTypes.Null)
        {
        }
    }
}

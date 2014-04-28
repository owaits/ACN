using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acn.Packets.Sdt.Addresses
{
    public class IPv4Address:SdtAddress
    {
        public IPv4Address()
            : base(SdtAddressTypes.IPv4)
        {
        }
    }
}

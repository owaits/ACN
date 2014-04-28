using Acn.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acn.Packets.Sdt.Addresses
{
    public enum SdtAddressTypes
    {
        Null = 0,
        IPv4 = 1,
        IPv6 = 2
    }

    public abstract class SdtAddress
    {
        public SdtAddress(SdtAddressTypes type)
        {
            Type = type;
        }

        public SdtAddressTypes Type { get; protected set; }

        public abstract void WriteData(AcnBinaryWriter data);

        public static SdtAddress ReadData(AcnBinaryReader data)
        {
            switch((SdtAddressTypes) data.ReadByte())
            {
                case SdtAddressTypes.Null:
                    return new NullAddress();
                case SdtAddressTypes.IPv4:
                    return new IPv4Address();
                case SdtAddressTypes.IPv6:
                    return new IPv6Address();
                default:
                    throw new InvalidOperationException("Unknown address STD type.");
            }
            
        }
    }
}

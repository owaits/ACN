using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acn.Packets.Sdt
{
    public enum ReasonCodes
    {
        Nonspecific = 1,
        IllegalParameters = 2,
        LowResources = 3,
        AlreadyMember = 4,
        BadAddressType = 5,
        NoReciprocalChannel = 6,
        ChannelExpired = 7,
        LostSequence = 8,
        Saturated = 9,
        TransportAddressChanging = 10,
        AskedToLeave = 11,
        NoRecipient = 12,
        OnlyUnicastSupported = 13
    }
}

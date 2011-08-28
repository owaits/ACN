using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Packets.Dmp
{
    public enum DmpMessages
    {
        GetProperty = 1,
        SetProperty = 2,
        GetPropertyReply = 3,
        Event = 4,
        Subscribe = 7,
        UnSubscribe = 8 
    }
}

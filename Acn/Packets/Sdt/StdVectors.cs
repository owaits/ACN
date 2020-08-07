using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.Acn.Packets.Sdt
{
    public enum StdVectors
    {
        ReliableWrapper = 1,
        UnreliableWrapper = 2,
        ChannelParameters = 3,
        Join = 4,
        JoinRefuse=5,
        JoinAccept = 6,
        Leave = 7,
        Leaving = 8,
        Connect = 9,
        ConnectAccept = 10,
        ConnectRefuse = 11,
        Disconnect = 12,
        Disconnecting = 13,
        Ack = 14,
        Nak = 14,
        GetSessions = 16,
        Sessions = 17
    }
}

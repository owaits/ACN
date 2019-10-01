using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.TCNet
{
    /// <summary>
    /// The TCNet packet types.
    /// </summary>
    public enum MessageTypes:byte
    {
        None = 0,
        OptIn = 2,
        OptOut = 3,
        TimeSync = 10,
        Error = 13,
        DataRequest = 20,
        ApplicationSpecificData = 30,
        ControlMessages = 101,
        TextData = 128,
        KeyboardData = 132,
        Data = 200,
        LowResArtwork = 204,
        DataFile = 204,
        Time = 254
    }
}

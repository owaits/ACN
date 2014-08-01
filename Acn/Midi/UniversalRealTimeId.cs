using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midi
{
    /// <summary>
    /// Used in the Universal Real Time messages to determine the protocol type.
    /// </summary>
    public enum UniversalRealTimeId
    {
        None = 0,
        Timecode = 1,
        MidiShowControl = 2
    }
}

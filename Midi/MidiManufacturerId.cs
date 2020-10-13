using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.Midi
{
    /// <summary>
    /// Used in the System Exclusive messages to allow manufacturer specific messages.
    /// </summary>
    /// <remarks>
    /// The second byte of a System exclusive message defines the manufacturer ID. Universal Real Time
    /// messages use a manufacturer ID of 0x7F.
    /// </remarks>
    public enum MidiManufacturerId
    {
        None = 0x0,
        UniversalRealTime = 0x7F
    }
}

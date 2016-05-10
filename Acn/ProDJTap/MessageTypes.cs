using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProDJTap
{
    /// <summary>
    /// The DJ Tap packet types.
    /// </summary>
    public enum MessageTypes
    {
        /// <summary>
        /// Not Set
        /// </summary>
        None = 0,
        /// <summary>
        /// The sent by devices whhiching to advertise opn the DJ Tap network.
        /// </summary>
        GWOffer = 0x200,
        /// <summary>
        /// The timecode data for a deck.
        /// </summary>
        Timecode = 0xFF00
    }
}

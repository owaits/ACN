using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.TCNet
{
    /// <summary>
    /// The track play state of the deck.
    /// </summary>
    public enum DeckState:byte
    {
        Idle = 0,
        Load = 2,
        Playing = 3,
        Looping = 4,
        Paused = 5,
        Stopped = 6,
        CueButtonDown = 7,
        PlatterDown = 8,
        FastForward = 9,
        FastRewind = 10,
        Hold = 11,
        EmergencyLoop = 18
    }
}

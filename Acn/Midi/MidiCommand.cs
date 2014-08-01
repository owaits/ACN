using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midi
{
    /// <summary>
    /// Defines all the MIDI message types.
    /// </summary>
    /// <remarks>
    /// The message command is the first byte in a message stream. For none System Exclusive
    /// commands the second niddle of the command is the channel number.
    /// </remarks>
    public enum MidiCommand
    {
        None = 0,
        NoteOff = 0x80,
        NoteOn = 0x90,
        Aftertouch = 0xA0,
        Continuous = 0xB0,
        PatchChange = 0xC0,
        ChannelPressure = 0xD0,
        PitchBend = 0xE0,
        SystemExclusiveStart = 0xF0,
        TimeCodeQuarterFrame = 0xF1,
        SongPositionPointer = 0xF2,
        TuneRequest = 0xF6,
        SystemExclusiveEnd = 0xF7,
        TimingClock = 0xF8,
        Start = 0xFA,
        Continue = 0xFB,
        Stop = 0xFC,
        ActiveSensing = 0xFE,
        SystemReset = 0xFF

    }
}

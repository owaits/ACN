using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midi.MidiShowControl
{
    /// <summary>
    /// MIDI Show Control command formats.
    /// </summary>
    public enum MscFormat
    {
        None = 0,
        Lighting = 0x01,
        MovingLights = 0x02,
        ColourChangers = 0x03,
        Strobes = 0x04,
        Lasers = 0x05,
        Chasers = 0x06,

        Sound = 0x10,
        Music = 0x11,
        CDPlayers = 0x12,
        EPROMPlayback = 0x13,
        AudioTapeMachines = 0x14,
        Intercoms = 0x15,
        Amplifiers = 0x16,
        AudioEffectsDevices = 0x17,
        Equalisers = 0x18,

        Machinery = 0x20,
        Rigging = 0x21,
        Flys = 0x22,
        Lifts = 0x23,
        Turntables = 0x24,
        Trusses = 0x25,
        Robots = 0x26,
        Animation = 0x27,
        Floats = 0x28,
        Breakaways = 0x29,
        Barges = 0x2A,

        Video = 0x30,
        VideoTapeMachines = 0x32,
        VideoCassetteMachines = 0x32,
        VideoDiscPlayers = 0x33,
        VideoSwitchers = 0x34,
        VideoEffects = 0x35,
        VideoCharacterGenerators = 0x36,
        VideoStillStores = 0x37,
        VideoMonitors = 0x38,

        Projection = 0x40,
        FilmProjectors = 0x41,
        SlideProjectors = 0x42,
        VideoProjectors = 0x43,
        Dissolvers = 0x44,
        ShutterControls = 0x45,

        AllTypes = 0x7f
    }
}

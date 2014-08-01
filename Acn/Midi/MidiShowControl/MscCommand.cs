using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midi.MidiShowControl
{
    /// <summary>
    /// MIDI Show Control command types.
    /// </summary>
    public enum MscCommand
    {
        None,
        Go,
        Stop,
        Resume,
        TimedGo,
        Load,
        Set,
        Fire,
        AllOff,
        Restore,
        Reset,
        GoOff,
        JamClock,
        StandbyPlus,
        StandbyMinus,
        StartClock,
        StopClock,
        ZeroClock,
        SetClock,
        MTCChaseOn,
        MTCChaseOff,
        OpenCueList,
        CloseCueList,
        OpenCuePath,
        CloseCuePatch
    }

    /// <summary>
    /// Provides extension classes for additional information relating to MscCommand.
    /// </summary>
    public static class MscCommandExtensions
    {
        /// <summary>
        /// Returns whether the command supports CueNumber, CueList and CuePath parameters.
        /// </summary>
        /// <param name="command">The command to test.</param>
        /// <returns>Whether the parameters are supported.</returns>
        public static bool HasCueParameter(this MscCommand command)
        {
            switch(command)
            {
                case MscCommand.Go:
                case MscCommand.Stop:
                case MscCommand.Resume:
                case  MscCommand.TimedGo:
                case MscCommand.Load:
                case MscCommand.GoOff:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns whether the command supports a single byte number parameter.
        /// </summary>
        /// <param name="command">The command to test.</param>
        /// <returns>Whether the parameters are supported.</returns>
        public static bool HasNumberParameter(this MscCommand command)
        {
            switch (command)
            {
                case MscCommand.Go:
                case MscCommand.Stop:
                case MscCommand.Resume:
                case MscCommand.TimedGo:
                case MscCommand.Load:
                case MscCommand.GoOff:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns whether the command supports a byte array parameter format.
        /// </summary>
        /// <param name="command">The command to test.</param>
        /// <returns>Whether the parameters are supported.</returns>
        public static bool HasDataParameter(this MscCommand command)
        {
            switch (command)
            {
                case MscCommand.Set:
                    return true;
            }

            return false;
        }
    }
}

using Midi.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midi
{
    /// <summary>
    /// Creates a message from a MIDI data stream.
    /// </summary>
    public class MidiMessageBuilder
    {
        /// <summary>
        /// Attempts to create a MIDI message from the MIDI data available.
        /// </summary>
        /// <param name="data">The raw MIDI data.</param>
        /// <param name="message">The generated MIDI message which you can cast to the required type.</param>
        /// <returns>True if the message type was recognised, otherwise false.</returns>
        public static bool TryBuild(byte[] data, out MidiMessage message)
        {
            message = null;

            switch((MidiCommand) data[0])
            {
                case MidiCommand.SystemExclusiveStart:
                    return TryBuildSystemExclusive(data,1,out message);
            }

            return message != null;
        }

        /// <summary>
        /// Attempts to create a MIDI message from the MIDI data available.
        /// </summary>
        /// <param name="data">The raw MIDI data.</param>
        /// <param name="dataOffset">The data offset.</param>
        /// <param name="message">The generated MIDI message which you can cast to the required type.</param>
        /// <returns>
        /// True if the message type was recognised, otherwise false.
        /// </returns>
        public static bool TryBuildSystemExclusive(byte[] data,int dataOffset, out MidiMessage message)
        {
            message = null;

            switch ((MidiManufacturerId) data[dataOffset])
            {
                case MidiManufacturerId.UniversalRealTime:
                    message = new MidiShowControl.MscMessage();
                    break;
            }

            if(message != null)
                message.ReadData(new MidiBinaryReader(new MemoryStream(data)));

            return message != null;
        }
    }
}

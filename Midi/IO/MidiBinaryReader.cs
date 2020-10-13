using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace LXProtocols.Midi.IO
{
    /// <summary>
    /// This class handles reading data from a MIDI stream.
    /// </summary>
    /// <remarks>
    /// Extends the BinaryReader class with MIDI specific functionality.
    /// </remarks>
    public class MidiBinaryReader:BinaryReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MidiBinaryReader"/> class.
        /// </summary>
        /// <param name="input">The input stream.</param>
        public MidiBinaryReader(Stream input)
            : base(input)
        { }

        private bool endOfMessage = false;

        /// <summary>
        /// Gets whether the end of a MIDI message has been reached.
        /// </summary>
        public bool EndOfMessage
        {
            get 
            {
                return endOfMessage || PeekChar() == (int) MidiCommand.SystemExclusiveEnd; 
            }
            protected set { endOfMessage = value; }
        }

        /// <summary>
        /// Reads a string from MIDI data, supports 00 and F7 terminators.
        /// </summary>
        /// <returns>The string which has been read from the stream.</returns>
        public string ReadMidiString()
        {
            string readString = string.Empty;
            char newCharacter = (char)ReadByte();

            while (newCharacter != 0 && newCharacter != (char) MidiCommand.SystemExclusiveEnd)
            {
                readString += newCharacter;
                newCharacter = (char)ReadByte();
            }

            EndOfMessage = (newCharacter == (char)MidiCommand.SystemExclusiveEnd);

            return readString;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace LXProtocols.Midi.IO
{
    /// <summary>
    /// This class handles writting data from a MIDI stream.
    /// </summary>
    /// <remarks>
    /// Extends the BinaryWriter class with MIDI specific functionality.
    /// </remarks>
    public class MidiBinaryWriter:BinaryWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MidiBinaryWriter"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public MidiBinaryWriter(Stream input)
            : base(input)
        { }

        /// <summary>
        /// Writes a MIDI string to the stream.
        /// </summary>
        /// <param name="text">The text to write to the stream.</param>
        /// <param name="nullTerminate">Whether to add the Null terminator at the end of the ASCII string.</param>
        public void WriteMidiString(string text, bool nullTerminate)
        {
            Write(System.Text.ASCIIEncoding.ASCII.GetBytes(text));
            if(nullTerminate) Write((byte)0);
        }
    }
}

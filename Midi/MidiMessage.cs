using Midi.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midi
{
    /// <summary>
    /// The base class for all MIDI messages.
    /// </summary>
    public abstract class MidiMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MidiMessage"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        protected MidiMessage(MidiCommand command)
        {
            Command = command;
        }

        private MidiCommand command = MidiCommand.None;

        /// <summary>
        /// Gets the MIDI command for this message.
        /// </summary>
        public MidiCommand Command
        {
            get { return command; }
            private set { command = value; }
        }

        /// <summary>
        /// Reads the message from a MIDI stream.
        /// </summary>
        /// <param name="data">The MIDI stream containing the message data.</param>
        public virtual void ReadData(MidiBinaryReader data)
        {
            Command = (MidiCommand)data.ReadByte();
        }

        /// <summary>
        /// Writes this message to the MIDI stream.
        /// </summary>
        /// <param name="data">The MIDI stream to write the messafe to.</param>
        public virtual void WriteData(MidiBinaryWriter data)
        {
            data.Write((byte) Command);
        }
    }
}

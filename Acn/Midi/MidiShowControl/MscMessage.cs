using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midi.MidiShowControl
{
    /// <summary>
    /// MIDI Show Control Message
    /// </summary>
    public class MscMessage : MidiSystemExclusiveMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MscMessage"/> class.
        /// </summary>
        public MscMessage():base(MidiManufacturerId.UniversalRealTime, UniversalRealTimeId.MidiShowControl)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MscMessage"/> class.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="format">The format.</param>
        /// <param name="command">The command.</param>
        public MscMessage(byte deviceId, MscFormat format, MscCommand command)
            : this()
        {
            DeviceId = deviceId;
            CommandFormat = format;
            ShowControlCommand = command;
        }

        private MscFormat commandFormat = MscFormat.None;

        /// <summary>
        /// Gets or sets the show control format such as Lighting or Sound.
        /// </summary>
        public MscFormat CommandFormat
        {
            get { return commandFormat; }
            set { commandFormat = value; }
        }

        private MscCommand showControlCommand = MscCommand.None;

        /// <summary>
        /// Gets or sets the show control command.
        /// </summary>
        public MscCommand ShowControlCommand
        {
            get { return showControlCommand; }
            set { showControlCommand = value; }
        }

        private int number = 0;

        /// <summary>
        /// Optional command parameter for a single byte number.
        /// </summary>
        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        private byte[] data = null;

        /// <summary>
        /// Optional command parameter for data.
        /// </summary>
        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }

        private string cueNumber = string.Empty;

        /// <summary>
        /// Optional command parameter for Cue Number as string.
        /// </summary>
        /// <remarks>
        /// An empty string will not be written to the message.
        /// </remarks>
        public string CueNumber
        {
            get { return cueNumber; }
            set { cueNumber = value; }
        }

        private string cueList = string.Empty;

        /// <summary>
        /// Optional command parameter for Cue List as string.
        /// </summary>
        /// <remarks>
        /// An empty string will not be written to the message.
        /// </remarks>
        public string CueList
        {
            get { return cueList; }
            set { cueList = value; }
        }

        private string cuePath = string.Empty;

        /// <summary>
        /// Optional command parameter for Cue Path as string.
        /// </summary>
        /// <remarks>
        /// An empty string will not be written to the message.
        /// </remarks>
        public string CuePath
        {
            get { return cuePath; }
            set { cuePath = value; }
        }

        /// <summary>
        /// Reads the message from a MIDI stream.
        /// </summary>
        /// <param name="data">The MIDI stream containing the message data.</param>
        public override void ReadData(IO.MidiBinaryReader data)
        {
            base.ReadData(data);

            CommandFormat = (MscFormat)data.ReadByte();
            ShowControlCommand = (MscCommand)data.ReadByte();

            if(ShowControlCommand.HasCueParameter())
            {
                if (!data.EndOfMessage)
                    CueNumber = data.ReadMidiString();

                if (!data.EndOfMessage)
                    CueList = data.ReadMidiString();

                if (!data.EndOfMessage)
                    CuePath = data.ReadMidiString();
            }

            if(ShowControlCommand.HasNumberParameter())
            {
                Number = data.ReadByte();
            }
        }

        /// <summary>
        /// Writes this message to the MIDI stream.
        /// </summary>
        /// <param name="data">The MIDI stream to write the messafe to.</param>
        public override void WriteData(IO.MidiBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((byte) CommandFormat);
            data.Write((byte) ShowControlCommand);

            if (ShowControlCommand.HasCueParameter())
            {
                if (!string.IsNullOrEmpty(CueNumber))
                {
                    data.WriteMidiString(CueNumber, !string.IsNullOrEmpty(CueList));

                    if (!string.IsNullOrEmpty(CueList))
                    {
                        data.WriteMidiString(CueList, !string.IsNullOrEmpty(CuePath));
                        
                        if (!string.IsNullOrEmpty(CuePath))
                            data.WriteMidiString(CuePath, false);
                    }                        
                    else
                    {
                        if (!string.IsNullOrEmpty(CuePath))
                            throw new FormatException("The Cue List parameter has not been set the MIDI Show Control message. This will result in an invalid MIDI message.");
                    }                    
                }
                else
                {
                    if (!string.IsNullOrEmpty(CueList) || !string.IsNullOrEmpty(CuePath))
                        throw new FormatException("The Cue Number parameter has not been set the MIDI Show Control message. This will result in an invalid MIDI message.");
                }

            }

            if (ShowControlCommand.HasNumberParameter())
            {
                data.Write((byte)Number);
            }

            data.Write((byte) MidiCommand.SystemExclusiveEnd);
        }
    }
}

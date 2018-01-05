using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midi
{
    /// <summary>
    /// System Exclusive MIDI Message
    /// </summary>
    public class MidiSystemExclusiveMessage:MidiMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MidiSystemExclusiveMessage"/> class.
        /// </summary>
        /// <param name="manufacturerId">The manufacturer identifier.</param>
        /// <param name="universalRealTimeId">The universal real time identifier.</param>
        protected MidiSystemExclusiveMessage(MidiManufacturerId manufacturerId, UniversalRealTimeId universalRealTimeId)
            : base(MidiCommand.SystemExclusiveStart)
        {
            ManufacturerId = manufacturerId;
            UniversalRealTimeId = universalRealTimeId;
        }

        private MidiManufacturerId manufacturerId = MidiManufacturerId.None;

        /// <summary>
        /// Gets the System Exclusive manufacturer ID.
        /// </summary>
        public MidiManufacturerId ManufacturerId
        {
            get { return manufacturerId; }
            private set { manufacturerId = value; }
        }

        private byte deviceId = 0;

        /// <summary>
        /// Gets or sets the System Exclusive device ID.
        /// </summary>
        public byte DeviceId
        {
            get { return deviceId; }
            set { deviceId = value; }
        }

        private UniversalRealTimeId universalRealTimeId = UniversalRealTimeId.None;

        /// <summary>
        /// The Universal Real Time message type ID.
        /// </summary>
        public UniversalRealTimeId UniversalRealTimeId
        {
            get { return universalRealTimeId; }
            set { universalRealTimeId = value; }
        }

        /// <summary>
        /// Reads the message from a MIDI stream.
        /// </summary>
        /// <param name="data">The MIDI stream containing the message data.</param>
        public override void ReadData(IO.MidiBinaryReader data)
        {
            base.ReadData(data);

            ManufacturerId = (MidiManufacturerId)data.ReadByte();
            DeviceId = data.ReadByte();
            UniversalRealTimeId = (UniversalRealTimeId)data.ReadByte();
        }

        /// <summary>
        /// Writes this message to the MIDI stream.
        /// </summary>
        /// <param name="data">The MIDI stream to write the messafe to.</param>
        public override void WriteData(IO.MidiBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((byte)ManufacturerId);
            data.Write(DeviceId);
            data.Write((byte)UniversalRealTimeId);
        }
    }
}

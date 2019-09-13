using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LXProtocols.TCNet.IO;
using LXProtocols.TCNet.Packets;
using System.Net.Sockets;

namespace LXProtocols.TCNet
{
    /// <summary>
    /// Recieve data for DJ Tap packets used as a container while receiving data.
    /// </summary>
    /// <seealso cref="System.IO.MemoryStream" />
    public class TCNetRecieveData:MemoryStream
    {
        private TCNetBinaryReader reader = null;

        public int ReadNibble = 1500;
        public int ReadPosition = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetRecieveData"/> class.
        /// </summary>
        public TCNetRecieveData()
        {
            this.Capacity = ReadNibble;
        }

        /// <summary>
        /// Gets or sets the socket being used to recieve the data.
        /// </summary>
        public Socket Socket { get; set; }

        /// <summary>
        /// Gets or sets the port, this data was recieved on.
        /// </summary>
        /// <remarks>
        /// This is used to ensure the next recieve is started on the correct port.
        /// </remarks>
        public int Port { get; set; }

        /// <summary>
        /// Determines if we have reached the end of the recieved data.
        /// </summary>
        /// <returns>True if we are at the end of the data.</returns>
        public bool EndOfData()
        {
            return ReadPosition >= Length;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="TCNetRecieveData"/> is valid.
        /// </summary>
        public bool Valid
        {
            get { return Length - ReadPosition > TCNetHeader.PacketSize; }
        }

        /// <summary>
        /// Gets a data reader for the data inside this buffer.
        /// </summary>
        /// <returns>A reader for the recieved data.</returns>
        public TCNetBinaryReader GetReader()
        {
            if (reader == null)
            {
                reader = new TCNetBinaryReader(this);
            }

            Seek(ReadPosition, SeekOrigin.Begin);
            return reader;
        }

        /// <summary>
        /// Resets this data buffer, ready for the next packet.
        /// </summary>
        public void Reset()
        {
            SetLength(0);
            ReadPosition = 0;
        }
    }
}

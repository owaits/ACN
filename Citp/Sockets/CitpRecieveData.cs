using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Citp.Packets;
using Citp.IO;

namespace Citp.Sockets
{
    public class CitpRecieveData:MemoryStream
    {
        private CitpBinaryReader reader = null;

        public int ReadNibble = 64000;

        /// <summary>
        /// Gets or sets the position in the buffer where we are reading packets.
        /// </summary>
        public long ReadPosition { get; set; }

        /// <summary>
        /// Gets or sets the position in the buffer at which we are filling data from.
        /// </summary>
        public long WritePosition { get; set; }

        public CitpRecieveData()
        {
            this.Capacity = ReadNibble;
        }

        public bool EndOfData()
        {
            return ReadPosition >= Length;
        }

        public bool Valid
        {
            get { return Length - ReadPosition > CitpHeader.PacketSize; }
        }

        public CitpBinaryReader GetReader()
        {
            if (reader == null)
            {
                reader = new CitpBinaryReader(this);
            }

            Seek(ReadPosition, SeekOrigin.Begin);
            return reader;
        }

        public void Reset()
        {
            SetLength(0);
            ReadPosition = 0;
            WritePosition = 0;
        }
    }
}

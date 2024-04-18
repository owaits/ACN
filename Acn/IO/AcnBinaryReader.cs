﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using LXProtocols.Acn.Rdm;

namespace LXProtocols.Acn.IO
{
    public class AcnBinaryReader : BinaryReader
    {
        public AcnBinaryReader(Stream input)
            : base(input)
        { }

        public short ReadOctet2()
        {
            return IPAddress.NetworkToHostOrder(ReadInt16());
        }

        public int ReadOctet4()
        {
            return IPAddress.NetworkToHostOrder(ReadInt32());
        }

        public string ReadUtf8String(int size)
        {
            byte[] data = ReadBytes(size);
            return UTF8Encoding.UTF8.GetString(data).TrimEnd((char)0);
        }

        /// <summary>
        /// Reads an RDM UID from the stream.
        /// </summary>
        /// <returns>The UID read from the stream.</returns>
        public UId ReadUId()
        {
            return new UId((ushort)(int)ReadOctet2(), (uint)ReadOctet4());
        }
    }
}

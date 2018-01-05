#region Copyright © 2011 Oliver Waits
//______________________________________________________________________________________________________________
// Service Location Protocol
// Copyright © 2011 Oliver Waits
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//______________________________________________________________________________________________________________
#endregion
   
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Acn.Slp.Packets;

namespace Acn.Slp.IO
{
    public class SlpBinaryReader : BinaryReader
    {
        public SlpBinaryReader(Stream input)
            : base(input)
        { }

        public short ReadNetwork16()
        {
            return IPAddress.NetworkToHostOrder(ReadInt16());
        }

        public int ReadNetwork32()
        {
            return IPAddress.NetworkToHostOrder(ReadInt32());
        }

        public int ReadNetwork24()
        {
            return (int)ReadByte() + ((int)ReadByte() << 8) + ((int)ReadByte() << 16);
        }

        public string ReadNetworkString()
        {
            int length = ReadNetwork16();
            return Encoding.UTF8.GetString(ReadBytes(length));
        }

        public UrlEntry ReadUrlEntry()
        {
            UrlEntry entry = new UrlEntry();

            entry.Reserved = ReadByte();
            entry.Lifetime = ReadNetwork16();
            entry.Url = ReadNetworkString();

            int authCount = ReadByte();
            for(int n=0;n<authCount;n++)
                entry.Authorities.Add(ReadAuthenticationBlock());

            return entry;
        }

        public AuthenticationBlock ReadAuthenticationBlock()
        {
            AuthenticationBlock block = new AuthenticationBlock();
            return block;
        }

        /// <summary>
        /// Reads a comma seperated string list from the network.
        /// </summary>
        /// <returns>An Enumerable of strings (commas are stripped out)</returns>
        internal IEnumerable<string> ReadNetworkStringList()
        {
            string data = ReadNetworkString();
            return data.Split(',');
        }
    }
}

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
    public class SlpBinaryWriter : BinaryWriter
    {
        public SlpBinaryWriter(Stream input)
            : base(input)
        { }

        public void WriteNetwork(short value)
        {
            Write(IPAddress.HostToNetworkOrder(value));
        }

        public void WriteNetwork(int value)
        {
            Write(IPAddress.HostToNetworkOrder(value));
        }

        public void WriteNetwork24(int value)
        {
            Write((byte)(value>>16));
            Write((byte)(value>>8));
            Write((byte)value);           
        }

        public void WriteNetworkString(string value)
        {
            WriteNetwork((short)value.Length);
            Write(Encoding.UTF8.GetBytes(value));
        }

        public void Write(UrlEntry value)
        {
            Write(value.Reserved);
            WriteNetwork(value.Lifetime);
            WriteNetworkString(value.Url);

            Write((byte)value.Authorities.Count);
            foreach(AuthenticationBlock block in value.Authorities)
                Write(block);
        }

        public void Write(AuthenticationBlock value)
        {
        }
    }
}

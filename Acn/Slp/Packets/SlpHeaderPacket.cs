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
using LXProtocols.Acn.Slp.IO;

namespace LXProtocols.Acn.Slp.Packets
{
    [Flags]
    public enum SlpHeaderFlags
    {
        Overflow = 0x8000,
        Fresh = 0x4000,
        RequestMulticast = 0x2000
    }

    public class SlpHeaderPacket
    {
        public SlpHeaderPacket(SlpFunctionId functionId)
        {
            Version = 2;
            FunctionId = functionId;
        }

        #region Packet Contents

        public byte Version { get; set; }

        public SlpFunctionId FunctionId { get; set; }

        public int Length { get; set; }

        public SlpHeaderFlags Flags { get; set; }

        public int NextExtensionOffset { get; set; }

        public short XId { get; set; }

        public string LanguageTag { get; set; }

        #endregion

        #region Read and Write

        public void ReadData(SlpBinaryReader data)
        {
            Version = data.ReadByte();
            FunctionId = (SlpFunctionId) data.ReadByte();
            Length = data.ReadNetwork24();
            Flags = (SlpHeaderFlags)data.ReadNetwork16();
            NextExtensionOffset = data.ReadNetwork24();
            XId = data.ReadNetwork16();
            LanguageTag = data.ReadNetworkString();

        }

        public void WriteData(SlpBinaryWriter data)
        {
            data.Write(Version);
            data.Write((byte) FunctionId);
            data.WriteNetwork24(Length);
            data.WriteNetwork((short) Flags);
            data.WriteNetwork24(NextExtensionOffset);
            data.WriteNetwork(XId);
            data.WriteNetworkString(LanguageTag);
        }

        #endregion
    }
}

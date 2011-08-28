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

using Acn.IO;
using System.IO;
using Acn.Slp.IO;

namespace Acn.Slp.Packets
{
    public abstract class SlpPacket
    {
        public const int MaxSize = 1000;

        public SlpPacket(SlpFunctionId functionId)
        {
            Header = new SlpHeaderPacket(functionId);
        }

        public SlpHeaderPacket Header
        {
            get;
            protected set;
        }

        protected void WriteLength(SlpBinaryWriter data)
        {
            data.BaseStream.Seek(2, SeekOrigin.Begin);
            data.WriteNetwork24((int) data.BaseStream.Length);
        }

        protected void ReadHeader(SlpBinaryReader data)
        {
            Header.ReadData(data);
        }

        protected void WriteHeader(SlpBinaryWriter data)
        {
            Header.WriteData(data);
        }

        protected abstract void ReadData(SlpBinaryReader data);

        protected abstract void WriteData(SlpBinaryWriter data);

        public static SlpPacket ReadPacket(SlpBinaryReader data)
        {
            SlpPacket newPacket = null;

            SlpHeaderPacket header = new SlpHeaderPacket(SlpFunctionId.None);
            header.ReadData(data);

            switch (header.FunctionId)
            {
                case SlpFunctionId.ServiceRequest:
                    newPacket = new ServiceRequestPacket();
                    break;
                case SlpFunctionId.ServiceReply:
                    newPacket = new ServiceReplyPacket();
                    break;
                case SlpFunctionId.ServiceRegistration:
                    newPacket = new ServiceRegistrationPacket();
                    break;
                case SlpFunctionId.ServiceDeRegister:
                    newPacket = new ServiceDeregistrationPacket();
                    break;
                case SlpFunctionId.ServiceAcknowledge:
                    newPacket = new ServiceAcknowledgePacket();
                    break;
                case SlpFunctionId.DirectoryAgentAdvert:
                    newPacket = new DirectoryAgentAdvertPacket();
                    break;
            }

            if (newPacket != null)
            {
                newPacket.Header = header;
                newPacket.ReadData(data);
            }

            return newPacket;
        }

        public static void WritePacket(SlpPacket packet, SlpBinaryWriter data)
        {
            packet.WriteHeader(data);
            packet.WriteData(data);
            packet.WriteLength(data);
        }
    }
}

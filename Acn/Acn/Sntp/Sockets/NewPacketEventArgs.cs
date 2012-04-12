#region Copyright © 2012 Mark Daniel
//______________________________________________________________________________________________________________
// NewPacketEventArgs
// Copyright © 2012 Mark Daniel
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
using System.Net;

namespace Acn.Sntp.Sockets
{
    /// <summary>
    /// Data from a new incomming NTP request
    /// </summary>
    public class NewPacketEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewPacketEventArgs"/> class.
        /// </summary>
        /// <param name="packet">The packet.</param>
        public NewPacketEventArgs(NtpData packet)
        {
            Packet = packet;
        }

        /// <summary>
        /// Gets or sets the source end point.
        /// </summary>
        /// <value>
        /// The source end point.
        /// </value>
        public IPEndPoint SourceEndPoint { get; set; }

        private NtpData packet;

        /// <summary>
        /// Gets the packet.
        /// </summary>
        public NtpData Packet
        {
            get { return packet; }
            private set { packet = value; }
        }

        /// <summary>
        /// Gets or sets the recieved time.
        /// </summary>
        /// <value>
        /// The recieved time.
        /// </value>
        public DateTime RecievedTime { get; set; }

    }
}

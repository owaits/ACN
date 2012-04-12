#region Copyright © 2012 Mark Daniel
//______________________________________________________________________________________________________________
// SntpSocket
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
using System.Net.Sockets;
using System.Net;
using System.IO;


namespace Acn.Sntp.Sockets
{
    /// <summary>
    /// Specialised socket to support sned and recive of Sntp Packets
    /// </summary>
    public class SntpSocket:Socket
    {
        public event UnhandledExceptionEventHandler UnhandledException;
        internal event EventHandler<NewPacketEventArgs> NewPacket;

        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="SntpSocket"/> class.
        /// </summary>
        public SntpSocket()
            : base(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp)
        {
        }

        #endregion

        #region Information

        /// <summary>
        /// Then SNTP/NTP port
        /// </summary>
        public const int Port = 123;

        /// <summary>
        /// Gets the multicast group for NTP/SNTP.
        /// </summary>
        public static IPAddress MulticastGroup
        {
            get
            {
                return new IPAddress(new byte[] { 244,0,1,1});
            }
        }

        private bool portOpen = false;

        /// <summary>
        /// Gets or sets a value indicating whether the port is open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if port open; otherwise, <c>false</c>.
        /// </value>
        public bool PortOpen
        {
            get { return portOpen; }
            set { portOpen = value; }
        }

        private DateTime? lastPacket = null;

        /// <summary>
        /// Gets or sets the last packet recieve time.
        /// (This is a generall socket time not NTP related)
        /// </summary>
        /// <value>
        /// The last packet.
        /// </value>
        public DateTime? LastPacket
        {
            get { return lastPacket; }
            protected set { lastPacket = value; }
        }

        #endregion

        #region Traffic

        /// <summary>
        /// Opens the specified ip address.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        public void Open(IPAddress ipAddress)
        {
            Open(new IPEndPoint(ipAddress, Port));
        }

        /// <summary>
        /// Opens the specified local end point.
        /// </summary>
        /// <param name="localEndPoint">The local end point.</param>
        public void Open(IPEndPoint localEndPoint)
        {
            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Bind(localEndPoint);
            PortOpen = true;

            StartRecieve();
        }

        /// <summary>
        /// Starts the recieve.
        /// </summary>
        public void StartRecieve()
        {
            try
            {
                EndPoint remotePort = new IPEndPoint(IPAddress.Any, Port);
                MemoryStream recieveState = new MemoryStream(NtpData.NTPDataLength);
                recieveState.SetLength(NtpData.NTPDataLength);
                BeginReceiveFrom(recieveState.GetBuffer(), 0, (int)recieveState.Length, SocketFlags.None, ref remotePort, new AsyncCallback(OnRecieve), recieveState);
            }
            catch (Exception ex)
            {
                OnUnhandledException(new ApplicationException("An error ocurred while trying to start recieving CITP.", ex));
            }
        }

        /// <summary>
        /// Called when a packet is recieved.
        /// </summary>
        /// <param name="state">The state.</param>
        private void OnRecieve(IAsyncResult state)
        {
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            if (PortOpen)
            {
                try
                {
                    MemoryStream recieveState = (MemoryStream)(state.AsyncState);

                    if (recieveState != null)
                    {
                        EndReceiveFrom(state, ref remoteEndPoint);

                        LastPacket = DateTime.UtcNow;

                        if (NewPacket != null)
                        {

                            //Read the Header
                            NtpData packet = NtpData.ReadPacket(recieveState);

                            NewPacketEventArgs args = new NewPacketEventArgs(packet) { RecievedTime = (DateTime)LastPacket };
                            args.SourceEndPoint = (IPEndPoint) remoteEndPoint;

                            NewPacket(this, args);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnUnhandledException(ex);
                }
                finally
                {
                    //Attempt to recieve another packet.
                    StartRecieve();
                }
            }
        }



        /// <summary>
        /// Called when an unhandled exception occurs.
        /// </summary>
        /// <param name="ex">The ex.</param>
        protected void OnUnhandledException(Exception ex)
        {
            if (UnhandledException != null) UnhandledException(this, new UnhandledExceptionEventArgs((object)ex, false));
        }

        /// <summary>
        /// Sends the specified packet.
        /// </summary>
        /// <param name="packet">The packet.</param>
        public void Send(NtpData packet)
        {
            Send(new IPEndPoint(MulticastGroup,Port), packet);
        }

        /// <summary>
        /// Sends the specified packet to a target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="packet">The packet.</param>
        public void Send(IPEndPoint target, NtpData packet)
        {
            byte[] data = packet.ToArray();
            BeginSendTo(data, 0, (int)data.Length, SocketFlags.None, target, null, null);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Net.Sockets.Socket"/>, and optionally disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            PortOpen = false;
            base.Dispose(disposing);
        }

        #endregion
    }
}

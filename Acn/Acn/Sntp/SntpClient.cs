#region Copyright

#region Copyright © 2012 Mark Daniel
//______________________________________________________________________________________________________________
// Simple Network Time Protocol Client
//
// Code taken largely from Code Project
// http://www.codeproject.com/Articles/1005/SNTP-Client-in-C
// SNTP Client in C# by Valer Bocan
// Used with persmission of CPOL.
// See original copyright notice below.
//
// In this derivative version
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

#region Original Copyright

/*
 * NTPClient
 * Copyright (C)2001 Valer BOCAN <vbocan@dataman.ro>
 * Last modified: June 29, 2001
 * All Rights Reserved
 * 
 * This code is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 * To fully understand the concepts used herein, I strongly
 * recommend that you read the RFC 2030.
 * 
 * NOTE: This example is intended to be compiled with Visual Studio .NET Beta 2
 */

#endregion

#endregion
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Acn.Sntp.Sockets;
using System.Linq;

namespace Acn.Sntp
{

    /// <summary>
    /// SntpClient is a class that allows easy connection to a remote host to get the current
    /// time.
    /// The implementation of the protocol is based on the RFC 2030.
    /// </summary>
    /// <remarks>
    /// Public class members:
    /// LeapIndicator - Warns of an impending leap second to be inserted/deleted in the last
    /// minute of the current day. (See the LeapIndicator enum)
    /// VersionNumber - Version number of the protocol (3 or 4).
    /// Mode - Returns mode. (See the Mode enum)
    /// Stratum - Stratum of the clock. (See the _Stratum enum)
    /// PollInterval - Maximum interval between successive messages.
    /// Precision - Precision of the clock.
    /// RootDelay - Round trip time to the primary reference source.
    /// RootDispersion - Nominal error relative to the primary reference source.
    /// ReferenceID - Reference identifier (either a 4 character string or an IP address).
    /// ReferenceTimestamp - The time at which the clock was last set or corrected.
    /// OriginateTimestamp - The time at which the request departed the client for the server.
    /// ReceiveTimestamp - The time at which the request arrived at the server.
    /// Transmit Timestamp - The time at which the reply departed the server for client.
    /// RoundTripDelay - The time between the departure of request and arrival of reply.
    /// LocalClockOffset - The offset of the local clock relative to the primary reference
    /// source.
    /// Initialize - Sets up data structure and prepares for connection.
    /// Connect - Connects to the time server and populates the data structure.
    /// It can also set the system time.
    /// IsResponseValid - Returns true if received data is valid and if comes from
    /// a NTP-compliant time server.
    /// ToString - Returns a string representation of the object.
    /// -----------------------------------------------------------------------------
    /// Structure of the standard NTP header (as described in RFC 2030)
    /// 1                   2                   3
    ///  0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |LI | VN  |Mode |    Stratum    |     Poll      |   Precision   |
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |                          Root Delay                           |
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |                       Root Dispersion                         |
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |                     Reference Identifier                      |
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |                                                               |
    /// |                   Reference Timestamp (64)                    |
    /// |                                                               |
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |                                                               |
    /// |                   Originate Timestamp (64)                    |
    /// |                                                               |
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |                                                               |
    /// |                    Receive Timestamp (64)                     |
    /// |                                                               |
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |                                                               |
    /// |                    Transmit Timestamp (64)                    |
    /// |                                                               |
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |                 Key Identifier (optional) (32)                |
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |                                                               |
    /// |                                                               |
    /// |                 Message Digest (optional) (128)               |
    /// |                                                               |
    /// |                                                               |
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// -----------------------------------------------------------------------------
    /// NTP Timestamp Format (as described in RFC 2030)
    /// 1                   2                   3
    /// 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |                           Seconds                             |
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |                  Seconds Fraction (0-padded)                  |
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// </remarks>
    public class SntpClient
    {

        // The URL of the time server we're connecting to
        private string TimeServer;


        /// <summary>
        /// Initializes a new instance of the <see cref="NTPClient"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        public SntpClient(string host)
        {
            TimeServer = host;
        }

        /// <summary>
        /// Connect to the time server and update system time
        /// </summary>
        /// <param name="UpdateSystemTime">if set to <c>true</c> update the system time.</param>
        public NtpData GetTime(bool UpdateSystemTime)
        {
            try
            {
                // Resolve server address
                IPHostEntry hostadd = Dns.GetHostEntry(TimeServer);
                IPEndPoint EPhost = new IPEndPoint(hostadd.AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork), SntpSocket.Port);

                //Connect the time server
                UdpClient timeSocket = new UdpClient();
                // Don't block for ages
                timeSocket.Client.SendTimeout = 1000;
                timeSocket.Client.ReceiveTimeout = 1000;
                // Allow conenction back to local
                //timeSocket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                timeSocket.Connect(EPhost);

                // Initialize data structure
                NtpData sendData = new NtpData();
                sendData.Initialize(4, NtpMode.Client, DateTime.Now);
                byte[] sendBytes = sendData.ToArray();
                timeSocket.Send(sendBytes, sendBytes.Length);

                NtpData recieveData = new NtpData(timeSocket.Receive(ref EPhost));
                recieveData.ReceptionTimestamp = DateTime.Now;
                if (!recieveData.IsResponseValid())
                {
                    throw new Exception("Invalid response from " + TimeServer);
                }


                // Update system time
                if (UpdateSystemTime)
                {
                    SetTime(recieveData.NewTime);
                }

                return recieveData;
            }
            catch (SocketException e)
            {
                throw new Exception(e.Message);
            }

        }



        // SYSTEMTIME structure used by SetSystemTime
        [StructLayoutAttribute(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public short year;
            public short month;
            public short dayOfWeek;
            public short day;
            public short hour;
            public short minute;
            public short second;
            public short milliseconds;
        }

        [DllImport("kernel32.dll")]
        static extern bool SetLocalTime(ref SYSTEMTIME time);

        /// <summary>
        /// Set system time according to transmit timestamp
        /// </summary>
        /// <param name="timeStamp">The time stamp.</param>
        private void SetTime(DateTime timeStamp)
        {
            SYSTEMTIME st;

            st.year = (short)timeStamp.Year;
            st.month = (short)timeStamp.Month;
            st.dayOfWeek = (short)timeStamp.DayOfWeek;
            st.day = (short)timeStamp.Day;
            st.hour = (short)timeStamp.Hour;
            st.minute = (short)timeStamp.Minute;
            st.second = (short)timeStamp.Second;
            st.milliseconds = (short)timeStamp.Millisecond;

            SetLocalTime(ref st);
        }


    }
}

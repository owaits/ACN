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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Acn.Sntp
{
    /// <summary>
    /// Leap indicator field values
    /// </summary>
    public enum LeapIndicator
    {
        /// <summary>
        /// 0 - No warning
        /// </summary>
        NoWarning,
        /// <summary>
        /// 1 - Last minute has 61 seconds
        /// </summary>
        LastMinute61,
        /// <summary>
        ///  2 - Last minute has 59 seconds
        /// </summary>
        LastMinute59,
        /// <summary>
        /// 3 - Alarm condition (clock not synchronized)
        /// </summary>
        Alarm
    }

    /// <summary>
    /// Mode field values
    /// </summary>
    public enum NtpMode
    {
        /// <summary>
        ///  1 - Symmetric active
        /// </summary>
        SymmetricActive = 1,
        /// <summary>
        ///  2 - Symmetric pasive
        /// </summary>
        SymmetricPassive = 2,
        /// <summary>
        ///  3 - Client
        /// </summary>
        Client = 3,
        /// <summary>
        /// 4 - Server
        /// </summary>
        Server = 4,
        /// <summary>
        /// 5 - Broadcast
        /// </summary>
        Broadcast = 5,
        /// <summary>
        /// 0, 6, 7 - Reserved
        /// </summary>
        Unknown
    }

    /// <summary>
    ///  Stratum field values
    /// </summary>
    public enum NtpStratum
    {
        /// <summary>
        /// 0 - unspecified or unavailable
        /// </summary>
        Unspecified,
        /// <summary>
        /// 1 - primary reference (e.g. radio-clock)
        /// </summary>
        PrimaryReference,
        /// <summary>
        /// 2-15 - secondary reference (via NTP or SNTP)
        /// </summary>
        SecondaryReference,
        /// <summary>
        /// 16-255 - reserved
        /// </summary>
        Reserved
    }

    public class NtpData
    {
        // NTP Data Structure Length
        public const byte NTPDataLength = 48;
        // NTP Data Structure (as described in RFC 2030)
        byte[] NTPData;

        public byte[] ToArray()
        {
            return NTPData;
        }

        public NtpData()
            : this(new byte[NTPDataLength])
        {

        }

        public NtpData(byte[] byteArray)
        {
            NTPData = byteArray;
        }

        public static NtpData ReadPacket(MemoryStream stream)
        {
            return new NtpData(stream.ToArray());
        }

        // Offset constants for timestamps in the data structure
        private const byte offReferenceID = 12;
        private const byte offReferenceTimestamp = 16;
        private const byte offOriginateTimestamp = 24;
        private const byte offReceiveTimestamp = 32;
        private const byte offTransmitTimestamp = 40;
        /// <summary>
        /// Gets the leap indicator.
        /// </summary>
        public LeapIndicator LeapIndicator
        {
            get
            {
                // Isolate the two most significant bits
                byte val = (byte)(NTPData[0] >> 6);
                switch (val)
                {
                    case 0: return LeapIndicator.NoWarning;
                    case 1: return LeapIndicator.LastMinute61;
                    case 2: return LeapIndicator.LastMinute59;
                    case 3: goto default;
                    default:
                        return LeapIndicator.Alarm;
                }
            }
            set
            {
                NTPData[0] = (byte)((NTPData[0] & 0x3F) | ((byte)value << 6));
            }
        }

        /// <summary>
        /// Gets the version number.
        /// </summary>
        public byte VersionNumber
        {
            get
            {
                // Isolate bits 3 - 5
                byte val = (byte)((NTPData[0] & 0x38) >> 3);
                return val;
            }
            set
            {
                NTPData[0] = (byte)((NTPData[0] & 0xc7) | ((value & 0x7) << 3));
            }
        }

        /// <summary>
        /// Gets the mode.
        /// </summary>
        public NtpMode Mode
        {
            get
            {
                // Isolate bits 0 - 3
                byte val = (byte)(NTPData[0] & 0x7);
                switch (val)
                {
                    case 0: goto default;
                    case 6: goto default;
                    case 7: goto default;
                    default:
                        return NtpMode.Unknown;
                    case 1:
                        return NtpMode.SymmetricActive;
                    case 2:
                        return NtpMode.SymmetricPassive;
                    case 3:
                        return NtpMode.Client;
                    case 4:
                        return NtpMode.Server;
                    case 5:
                        return NtpMode.Broadcast;
                }
            }
            set
            {
                NTPData[0] = (byte)((NTPData[0] & 0xF8) | ((byte)value));
            }
        }

        /// <summary>
        /// Gets the stratum.
        /// </summary>
        public NtpStratum Stratum
        {
            get
            {
                byte val = (byte)NTPData[1];
                if (val == 0) return NtpStratum.Unspecified;
                else
                    if (val == 1) return NtpStratum.PrimaryReference;
                    else
                        if (val <= 15) return NtpStratum.SecondaryReference;
                        else
                            return NtpStratum.Reserved;
            }
            set
            {
                NTPData[1] = (byte)value;
            }
        }

        /// <summary>
        /// Gets the poll interval.
        /// </summary>
        public uint PollInterval
        {
            get
            {
                return (uint)Math.Round(Math.Pow(2, NTPData[2]));
            }
        }

        /// <summary>
        /// Gets the precision.
        /// </summary>
        public double Precision
        {
            get
            {
                return (1000 * Math.Pow(2, NTPData[3]));
            }
        }

        /// <summary>
        /// Gets the root delay.
        /// </summary>
        public double RootDelay
        {
            get
            {
                int temp = 0;
                temp = 256 * (256 * (256 * NTPData[4] + NTPData[5]) + NTPData[6]) + NTPData[7];
                return 1000 * (((double)temp) / 0x10000);
            }
            set
            {
                int temp = (int)(value * 0x10000 / 1000);
                byte[] bytes = BitConverter.GetBytes(temp);
                for (int i = 0; i < 4; i++)
                {
                    NTPData[7 - i] = bytes[i];
                }
            }
        }

        /// <summary>
        /// Gets the root dispersion.
        /// </summary>
        public double RootDispersion
        {
            get
            {
                int temp = 0;
                temp = 256 * (256 * (256 * NTPData[8] + NTPData[9]) + NTPData[10]) + NTPData[11];
                return (((double)temp) * 1000 / 0x10000);
            }
            set
            {
                int temp = (int)(value * 0x10000 / 1000);
                byte[] bytes = BitConverter.GetBytes(temp);
                for (int i = 0; i < 4; i++)
                {
                    NTPData[11 - i] = bytes[i];
                }
            }
        }

        /// <summary>
        /// Gets the reference Identifier.
        /// </summary>
        public string ReferenceID
        {
            get
            {
                string val = "";
                switch (Stratum)
                {
                    case NtpStratum.Unspecified:
                        goto case NtpStratum.PrimaryReference;
                    case NtpStratum.PrimaryReference:
                        val += (char)NTPData[offReferenceID + 0];
                        val += (char)NTPData[offReferenceID + 1];
                        val += (char)NTPData[offReferenceID + 2];
                        val += (char)NTPData[offReferenceID + 3];
                        break;
                    case NtpStratum.SecondaryReference:
                        switch (VersionNumber)
                        {
                            case 3:	// Version 3, Reference ID is an IPv4 address
                                string Address = NTPData[offReferenceID + 0].ToString() + "." +
                                                 NTPData[offReferenceID + 1].ToString() + "." +
                                                 NTPData[offReferenceID + 2].ToString() + "." +
                                                 NTPData[offReferenceID + 3].ToString();
                                try
                                {
                                    IPHostEntry Host = Dns.GetHostByAddress(Address);
                                    val = Host.HostName + " (" + Address + ")";
                                }
                                catch (Exception)
                                {
                                    val = "N/A";
                                }
                                break;
                            case 4: // Version 4, Reference ID is the timestamp of last update
                                DateTime time = ComputeDate(GetMilliSeconds(offReferenceID));
                                // Take care of the time zone
                                TimeSpan offspan = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                                val = (time + offspan).ToString();
                                break;
                            default:
                                val = "N/A";
                                break;
                        }
                        break;
                }

                return val;
            }
            set
            {
                // Only support setting the primary reference at the moment
                if (Stratum != NtpStratum.PrimaryReference)
                    throw new NotImplementedException("Setting reference ID not implemented for Startum other than the Primary Reference");

                for (int i = 0; i < 4; i++)
                {
                    byte chr;
                    if (i < value.Length)
                        chr = (byte)value[i];
                    else
                        chr = 0;
                    NTPData[offReferenceID + i] = chr;
                }

            }
        }

        /// <summary>
        /// Gets the reference timestamp.
        /// </summary>
        public DateTime ReferenceTimestamp
        {
            get
            {
                DateTime time = ComputeDate(GetMilliSeconds(offReferenceTimestamp));
                // Take care of the time zone
                TimeSpan offspan = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                return time + offspan;
            }
        }

        /// <summary>
        /// Gets the originate timestamp.
        /// </summary>
        public DateTime OriginateTimestamp
        {
            get
            {
                return ComputeDate(GetMilliSeconds(offOriginateTimestamp));
            }
            set
            {
                SetDate(offOriginateTimestamp, value);
            }
        }

        /// <summary>
        /// Gets the receive timestamp.
        /// </summary>
        public DateTime ReceiveTimestamp
        {
            get
            {
                DateTime time = ComputeDate(GetMilliSeconds(offReceiveTimestamp));
                // Take care of the time zone
                TimeSpan offspan = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                return time + offspan;
            }
            set
            {
                SetDate(offReceiveTimestamp, value);
            }
        }

        /// <summary>
        /// Gets or sets the transmit timestamp.
        /// </summary>
        /// <value>
        /// The transmit timestamp.
        /// </value>
        public DateTime TransmitTimestamp
        {
            get
            {
                DateTime time = ComputeDate(GetMilliSeconds(offTransmitTimestamp));
                // Take care of the time zone
                TimeSpan offspan = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                return time + offspan;
            }
            set
            {
                SetDate(offTransmitTimestamp, value);
            }
        }

        // Reception Timestamp
        public DateTime ReceptionTimestamp;

        /// <summary>
        /// Gets the round trip delay.
        /// </summary>
        public TimeSpan RoundTripDelay
        {
            get
            {
                return (ReceiveTimestamp - OriginateTimestamp) + (ReceptionTimestamp - TransmitTimestamp);
            }
        }

        /// <summary>
        /// Gets the local clock offset.
        /// </summary>
        public TimeSpan LocalClockOffset
        {
            get
            {
                return TimeSpan.FromMilliseconds(((ReceiveTimestamp - OriginateTimestamp) - (ReceptionTimestamp - TransmitTimestamp)).TotalMilliseconds / 2);
            }
        }

        /// <summary>
        /// Gets the new time.
        /// </summary>
        public DateTime NewTime
        {
            get
            {
                return DateTime.Now + LocalClockOffset;
            }
        }

        /// <summary>
        /// Compute date, given the number of milliseconds since January 1, 1900
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <returns></returns>
        private DateTime ComputeDate(ulong milliseconds)
        {
            TimeSpan span = TimeSpan.FromMilliseconds((double)milliseconds);
            DateTime time = new DateTime(1900, 1, 1);
            time += span;
            return time;
        }

        /// <summary>
        /// Compute the number of milliseconds, given the offset of a 8-byte array
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        private ulong GetMilliSeconds(byte offset)
        {
            ulong intpart = 0, fractpart = 0;

            for (int i = 0; i <= 3; i++)
            {
                intpart = 256 * intpart + NTPData[offset + i];
            }
            for (int i = 4; i <= 7; i++)
            {
                fractpart = 256 * fractpart + NTPData[offset + i];
            }
            ulong milliseconds = intpart * 1000 + (fractpart * 1000) / 0x100000000L;
            return milliseconds;
        }

        /// <summary>
        /// Compute the 8-byte array, given the date
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="date">The date.</param>
        private void SetDate(byte offset, DateTime date)
        {
            ulong intpart = 0, fractpart = 0;
            DateTime StartOfCentury = new DateTime(1900, 1, 1, 0, 0, 0);	// January 1, 1900 12:00 AM

            ulong milliseconds = (ulong)(date - StartOfCentury).TotalMilliseconds;
            intpart = milliseconds / 1000;
            fractpart = ((milliseconds % 1000) * 0x100000000L) / 1000;

            ulong temp = intpart;
            for (int i = 3; i >= 0; i--)
            {
                NTPData[offset + i] = (byte)(temp % 256);
                temp = temp / 256;
            }

            temp = fractpart;
            for (int i = 7; i >= 4; i--)
            {
                NTPData[offset + i] = (byte)(temp % 256);
                temp = temp / 256;
            }
        }

        /// <summary>
        /// Check if the response from server is valid
        /// </summary>
        /// <returns>
        ///   <c>true</c> if is response valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsResponseValid()
        {
            if (NTPData.Length < NTPDataLength || Mode != NtpMode.Server)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string str;

            str = "Leap Indicator: ";
            switch (LeapIndicator)
            {
                case LeapIndicator.NoWarning:
                    str += "No warning";
                    break;
                case LeapIndicator.LastMinute61:
                    str += "Last minute has 61 seconds";
                    break;
                case LeapIndicator.LastMinute59:
                    str += "Last minute has 59 seconds";
                    break;
                case LeapIndicator.Alarm:
                    str += "Alarm Condition (clock not synchronized)";
                    break;
            }
            str += "\r\nVersion number: " + VersionNumber.ToString() + "\r\n";
            str += "Mode: ";
            switch (Mode)
            {
                case NtpMode.Unknown:
                    str += "Unknown";
                    break;
                case NtpMode.SymmetricActive:
                    str += "Symmetric Active";
                    break;
                case NtpMode.SymmetricPassive:
                    str += "Symmetric Pasive";
                    break;
                case NtpMode.Client:
                    str += "Client";
                    break;
                case NtpMode.Server:
                    str += "Server";
                    break;
                case NtpMode.Broadcast:
                    str += "Broadcast";
                    break;
            }
            str += "\r\nStratum: ";
            switch (Stratum)
            {
                case NtpStratum.Unspecified:
                case NtpStratum.Reserved:
                    str += "Unspecified";
                    break;
                case NtpStratum.PrimaryReference:
                    str += "Primary Reference";
                    break;
                case NtpStratum.SecondaryReference:
                    str += "Secondary Reference";
                    break;
            }
            str += "\r\nOriginate (Send request): " + OriginateTimestamp.ToString("dd/MM/yyyy HH:mm:ss.fff");
            str += "\r\nRecieve (Server recieved): " + ReceiveTimestamp.ToString("dd/MM/yyyy HH:mm:ss.fff");
            str += "\r\nTransmit (Server reply): " + TransmitTimestamp.ToString("dd/MM/yyyy HH:mm:ss.fff");
            str += "\r\nReception (Request recieved): " + ReceptionTimestamp.ToString("dd/MM/yyyy HH:mm:ss.fff");
            str += "\r\nServer transmit time (Local zone): " + TransmitTimestamp.ToString("dd/MM/yyyy HH:mm:ss.fff");
            str += "\r\nPrecision: " + Precision.ToString() + " ms";
            str += "\r\nPoll Interval: " + PollInterval.ToString() + " s";
            str += "\r\nReference ID: " + ReferenceID.ToString();
            str += "\r\nRoot Dispersion: " + RootDispersion.ToString() + " ms";
            str += "\r\nRound Trip Delay: " + RoundTripDelay.TotalMilliseconds.ToString() + " ms";
            str += "\r\nLocal Clock Offset: " + LocalClockOffset.TotalMilliseconds.ToString() + " ms";
            str += "\r\n";

            return str;
        }

        /// <summary>
        /// Initializes packet for a request
        /// </summary>
        /// <param name="versionNumber">The version number.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="transmitTimestamp">The transmit timestamp.</param>
        public void Initialize(byte versionNumber, NtpMode mode, DateTime transmitTimestamp)
        {
            // Set version number to 3 and Mode to 3 (client)
            VersionNumber = versionNumber;
            Mode = mode;

            //NTPData[0] = 0x1B;

            // Initialize all other fields with 0
            for (int i = 1; i < 48; i++)
            {
                NTPData[i] = 0;
            }
            // Initialize the transmit timestamp
            TransmitTimestamp = transmitTimestamp;
        }


        /// <summary>
        /// Copies the transmit timestamp to the originate timestamp.
        /// </summary>
        internal void CopyTransmitToOriginate()
        {
            // Copy 64 bit value
            for (int i = 0; i < 8; i++)
            {
                NTPData[offOriginateTimestamp + i] = NTPData[offTransmitTimestamp + i];
            }
        }
    }
}

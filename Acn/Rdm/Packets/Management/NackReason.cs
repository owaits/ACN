using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LXProtocols.Acn.Rdm.Packets.Management
{
    public enum NackReason
    {
        UnknownPid = 0x0,
        FormatError = 0x1,
        HardwareFault = 0x2,
        ProxyReject = 0x3,
        WriteProtect = 0x4,
        UnsupportedCommandClass = 0x5,
        DataOutOfRange = 0x6,
        BufferFull = 0x7,
        PacketSizeUnsupported = 0x8,
        SubDeviceOutOfRange = 0x9,
        ProxyBufferFull = 0xA,
        /// <summary>
        /// The specified action is not supported.
        /// </summary>
        ActionNotSupported = 0x000B,
        /// <summary>
        /// The Component is not participating in the given Scope.
        /// </summary>
        UnknownScope = 0x000F,
        /// <summary>
        /// The Static Config Type provided is invalid.
        /// </summary>
        InvalidStaticConfigType = 0x0010,
        /// <summary>
        /// The IPv4 Address provided is invalid.
        /// </summary>
        InvalidIPV4Address = 0x0011,
        /// <summary>
        /// The IPv6 Address provided is invalid.
        /// </summary>
        InvalidIPV6Address = 0x0012,
        /// <summary>
        /// The transport layer port provided is invalid.
        /// </summary>
        InvalidPort = 0x0013
    }
}

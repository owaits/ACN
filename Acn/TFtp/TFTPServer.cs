#region Copyright © 2005 EnerLinx.com, Inc. - All rights reserved.
/*  
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights 
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included 
 * in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 * 
 * For more information see:
 * http://www.enerlinx.com
*/
#endregion

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.IO;

namespace LXProtocols.Acn.TFtp
{
    #region TFTP Formats
    //	TFTP Formats
    //	Type   Op #     Format without header
    //         2 bytes    string   1 byte     string   1 byte
    //         -----------------------------------------------
    //	RRQ/  | 01/02 |  Filename  |   0  |    Mode    |   0  |
    //	WRQ    -----------------------------------------------
    //         2 bytes    2 bytes       n bytes
    //		   ---------------------------------
    //	DATA  | 03    |   Block #  |    Data    |
    //	       ---------------------------------
    //	       2 bytes    2 bytes
    //	       -------------------
    //	ACK   | 04    |   Block #  |
    //	       --------------------
    //	       2 bytes  2 bytes        string    1 byte
    //	       ----------------------------------------
    //	ERROR | 05    |  ErrorCode |   ErrMsg   |   0  |
    //	       ----------------------------------------
    #endregion

    /// <summary>
    /// Simple read-only TFTP Server	
    /// </summary>		
    public class TFTPServer
    {
        #region delegates
        public delegate Stream GetFileDelegate(string filename, string mode);
        #endregion

        #region events
        public event EventHandler ReadRequestDone;
        public event EventHandler ReadBlockSent;
        #endregion

        #region fields
        //public static readonly ExTraceSwitch TFTPServerTraceSwitch = new ExTraceSwitch("TFTPServerTraceSwitch", "TFTPServer Traces.");

        private int m_ListenPort = 69;
        private UdpClient m_Server = null;
        private bool m_Done = false;
        private Thread m_ListenThread = null;
        private GetFileDelegate m_GFD = null;
        private System.IO.Stream m_CurrentReadStream = null;
        private BinaryReader m_ReadStreamReader = null;
        private int m_CurrentBlock = 1;
        private int m_LastBlockLength = 0;
        #endregion

        #region Opcode
        private enum Opcodes
        {
            TFTP_RRQ = 01,		// TFTP read request packet.
            TFTP_WRQ = 02,		// TFTP write request packet. 
            TFTP_DATA = 03,		// TFTP data packet. 
            TFTP_ACK = 04,		// TFTP acknowledgement packet.
            TFTP_ERROR = 05,		// TFTP error packet. 
            TFTP_OACK = 06		// TFTP option acknowledgement packet. 
        }
        #endregion

        #region ErrorCodes
        public enum ErrorCodes
        {
            TFTP_ERROR_UNDEFINED = 0,	// Not defined, see error message (if any).
            TFTP_ERROR_FILE_NOT_FOUND = 1,	// File not found.
            TFTP_ERROR_ACCESS_VIOLATION = 2,    // Access violation.
            TFTP_ERROR_ALLOC_ERROR = 3,	// Disk full or allocation exceeded.
            TFTP_ERROR_ILLEGAL_OP = 4,    // Illegal TFTP operation.
            TFTP_ERROR_UNKNOWN_TID = 5,	// Unknown transfer ID.
            TFTP_ERROR_FILE_EXISTS = 6,    // File already exists.
            TFTP_ERROR_INVALID_USER = 7     //   No such user.
        }
        #endregion

        #region Request Modes
        public const string REQUEST_MODE_NETASCII = "netascii";
        public const string REQUEST_MODE_OCTET = "octet";
        public const string REQUEST_MODE_MAIL = "mail";
        #endregion

        #region ctor
        public TFTPServer()
        {
            //ExTrace.WriteMethodIf(TFTPServerTraceSwitch.TraceVerbose);
        }
        #endregion

        #region GetFileHandler property
        /// <summary>
        /// Delegate to handle creation of file stream
        /// </summary>
        public GetFileDelegate GetFileHandler
        {
            get
            {
                return m_GFD;
            }
            set
            {
                m_GFD = value;
            }
        }
        #endregion

        #region Start / Stop
        /// <summary>
        /// Start server
        /// </summary>
        public void Start()
        {
            //ExTrace.WriteMethodIf(TFTPServerTraceSwitch.TraceVerbose);

            if (m_Server == null)
            {
                m_Server = new UdpClient(m_ListenPort);

                m_ListenThread = new Thread(new ThreadStart(Listener));
                m_ListenThread.Start();
            }
        }

        /// <summary>
        /// Stop server
        /// </summary>
        public void Stop()
        {
            //ExTrace.WriteMethodIf(TFTPServerTraceSwitch.TraceVerbose);
            m_Done = true;
            try
            {
                if (m_Server != null)
                    m_Server.Close();
                if (m_CurrentReadStream != null)
                    m_CurrentReadStream.Close();
            }
            catch (Exception)
            {
                //ExTrace.ShowException(err);
            }
        }
        #endregion

        #region Listner thread section
        #region Listener thread loop
        /// <summary>
        /// Main listener thread loop
        /// </summary>
        public void Listener()
        {
            //ExTrace.WriteMethodIf(TFTPServerTraceSwitch.TraceVerbose);

            while (!m_Done)
            {
                try
                {
                    IPEndPoint endpoint = null;
                    Byte[] data = m_Server.Receive(ref endpoint);

                    Opcodes opcode = (Opcodes)(short)((((short)data[0]) * 256) + (short)data[1]);

                    //ExTrace.WriteLineIf(TFTPServerTraceSwitch.TraceVerbose, opcode.ToString() + " from " + endpoint.ToString());
                    switch (opcode)
                    {
                        case Opcodes.TFTP_RRQ:
                            DoReadRequest(data, endpoint);
                            break;

                        case Opcodes.TFTP_ERROR:
                            DoError(data, endpoint);
                            break;

                        case Opcodes.TFTP_ACK:
                            DoAck(data, endpoint);
                            break;

                        case Opcodes.TFTP_WRQ:
                        case Opcodes.TFTP_DATA:
                        case Opcodes.TFTP_OACK:
                        default:
                            //ExTrace.WriteLineIf(TFTPServerTraceSwitch.TraceVerbose, "Unsupported Opcode");
                            break;

                    }
                }
                catch (Exception)
                {
                    //ExTrace.ShowException(err);
                }
            }
            return;
        }
        #endregion

        #region DoAck
        /// <summary>
        /// Handle ACK response and send next block.
        /// </summary>
        /// <param name="data">data from packet</param>
        /// <param name="endpoint">client</param>
        private void DoAck(Byte[] data, IPEndPoint endpoint)
        {
            short blocknum = (short)((((short)data[2]) * 256) + (short)data[3]);
            if (m_CurrentReadStream != null)
            {
                if (m_LastBlockLength != 4) // still more to send
                {
                    if ((blocknum + 1) != this.m_CurrentBlock)
                    {
                        // not sure how we'd ever get here, but just in case
                        m_CurrentBlock = blocknum + 1;
                    }
                    SendStream(endpoint, m_CurrentBlock);
                    m_CurrentBlock++;
                }
            }
        }
        #endregion

        #region DoError
        /// <summary>
        /// Parse an error response
        /// </summary>
        /// <param name="data">data from packet</param>
        /// <param name="endpoint">client</param>
        private void DoError(Byte[] data, IPEndPoint endpoint)
        {
            //ExTrace.WriteMethodIf(TFTPServerTraceSwitch.TraceVerbose);

            Encoding ASCII = Encoding.ASCII;

            string delimStr = "\0";
            char[] delimiter = delimStr.ToCharArray();

            short errorcode = (short)((((short)data[2]) * 256) + (short)data[3]);

            string[] strData = ASCII.GetString(data, 2, data.Length - 2).Split(delimiter, 3);

            string message = strData[0];

            //ExTrace.WriteLineIf(TFTPServerTraceSwitch.TraceVerbose, "Error-" + errorcode.ToString() + ":" + message);
        }
        #endregion

        #region DoReadRequest
        /// <summary>
        /// Handle Read request
        /// </summary>
        /// <param name="data">data from packet</param>
        /// <param name="endpoint">client</param>
        private void DoReadRequest(Byte[] data, IPEndPoint endpoint)
        {
            //ExTrace.WriteMethodIf(TFTPServerTraceSwitch.TraceVerbose);

            Encoding ASCII = Encoding.ASCII;

            string delimStr = "\0";
            char[] delimiter = delimStr.ToCharArray();

            string[] strData = ASCII.GetString(data, 2, data.Length - 2).Split(delimiter, 3);

            string filename = strData[0];
            string mode = strData[1].ToLower();

            //ExTrace.WriteLineIf(TFTPServerTraceSwitch.TraceVerbose, "filename:" + filename + " mode:" + mode);

            if (m_GFD != null)
            {
                //ExTrace.WriteLineIf(TFTPServerTraceSwitch.TraceVerbose, "Calling GetFileDelegate...");
                System.IO.Stream filestream = m_GFD(filename, mode);
                //ExTrace.WriteLineIf(TFTPServerTraceSwitch.TraceVerbose, "OK (filestream " + ((filestream == null) ? "INVALID" : "VALID") + ")");
                if (filestream != null)
                {
                    // TODO: Start a timer to close stream in connection timeout					
                    if (m_CurrentReadStream != null)
                        m_CurrentReadStream.Close();
                    m_CurrentReadStream = filestream;
                    m_ReadStreamReader = new BinaryReader(m_CurrentReadStream);
                    m_CurrentBlock = 1;
                    SendStream(endpoint, m_CurrentBlock);
                }
            }
        }
        #endregion

        #region SendStream
        /// <summary>
        /// Send part of a stream
        /// </summary>
        /// <param name="endpoint">location to send stream to</param>
        /// <param name="BlockNumber">512 byte block to send</param>
        private void SendStream(IPEndPoint endpoint, int BlockNumber)
        {
            //ExTrace.WriteMethodIf(TFTPServerTraceSwitch.TraceSuperVerbose);

            int fileoffset = (BlockNumber - 1) * 512;
            m_CurrentReadStream.Seek(fileoffset, SeekOrigin.Begin);

            Byte[] buffer = new Byte[516];
            buffer[0] = 0;
            buffer[1] = (byte)Opcodes.TFTP_DATA;
            buffer[2] = (byte)((BlockNumber & 0xFF00) / 256);
            buffer[3] = (byte)(BlockNumber & 0x00FF);

            m_LastBlockLength = m_ReadStreamReader.Read(buffer, 4, 512);
            m_LastBlockLength += 4;
            //ExTrace.WriteLineIf(TFTPServerTraceSwitch.TraceVerbose, "Sending " + m_LastBlockLength.ToString() + " bytes.");
            int ecode = m_Server.Send(buffer, m_LastBlockLength, endpoint);

            //if (ecode <= 0)
                //ExTrace.WriteLineIf(TFTPServerTraceSwitch.TraceVerbose, "Error in send : " + ecode);

            if (m_LastBlockLength == 4)  // end of stream, some clients won't ACK this last packet
            {
                //ExTrace.WriteLineIf(TFTPServerTraceSwitch.TraceVerbose, "All done, closing stream...");
                m_ReadStreamReader.Close();
                m_CurrentReadStream = null;
                //ExTrace.WriteLineIf(TFTPServerTraceSwitch.TraceVerbose, "OK.");
                try
                {
                    if (ReadRequestDone != null)
                        ReadRequestDone(this, null);
                }
                catch (Exception)
                {
                    //ExTrace.ShowException(err);
                }
            }
            else
            {
                try
                {
                    if (ReadBlockSent != null)
                        ReadBlockSent(m_LastBlockLength - 4, null);
                }
                catch (Exception)
                {
                    //ExTrace.ShowException(err);
                }
            }

            // TODO: Start a timer to resend in no ACK.
        }
        #endregion
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Acn.ArtNet.IO;
using Acn.ArtNet.Sockets;

namespace Acn.ArtNet.Packets
{
    [Flags]
    public enum PollReplyStatus
    {
        None = 0,
        UBEA = 1,
        RdmCapable = 2,
        ROMBoot = 4
    }

    internal class ArtPollReplyPacket:ArtNetPacket
    {
        public ArtPollReplyPacket()
            : base(ArtNetOpCodes.PollReply)
        {
        }

        public ArtPollReplyPacket(ArtNetRecieveData data)
            : base(data)
        {
            
        }

        #region Packet Properties

        private byte[] ipAddress=new byte[4];

        public byte[] IpAddress
        {
            get { return ipAddress; }
            set 
            {
                if (value.Length != 4)
                    throw new ArgumentException("The IP address must be an array of 4 bytes.");

                ipAddress = value; 
            }
        }

        private short port= ArtNetSocket.Port;

        public short Port
        {
            get { return port; }
            set { port = value; }
        }

        private short firmwareVersion = 0;

        public short FirmwareVersion
        {
            get { return firmwareVersion; }
            set { firmwareVersion = value; }
        }



        private short subSwitch = 0;

        public short SubSwitch
        {
            get { return subSwitch; }
            set { subSwitch = value; }
        }

        private short oem = 0xff;

        public short Oem
        {
            get { return oem; }
            set { oem = value; }
        }

        private byte ubeaVersion= 0;

        public byte UbeaVersion
        {
            get { return ubeaVersion; }
            set { ubeaVersion = value; }
        }

        private PollReplyStatus status = 0;

        public PollReplyStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        private string estaCode= string.Empty;

        public string EstaCode
        {
            get { return estaCode; }
            set { estaCode = value; }
        }

        private string shortName = string.Empty;

        public string ShortName
        {
            get { return shortName; }
            set { shortName = value; }
        }

        private string longName = string.Empty;

        public string LongName
        {
            get { return longName; }
            set { longName = value; }
        }

        private string nodeReport= string.Empty;

        public string NodeReport
        {
            get { return nodeReport; }
            set { nodeReport = value; }
        }

        private short portCount=0;

        public short PortCount
        {
            get { return portCount; }
            set { portCount = value; }
        }

        private byte[] portTypes= new byte[4];

        public byte[] PortTypes
        {
            get { return portTypes; }
            set 
            {
                if (value.Length != 4)
                    throw new ArgumentException("The port types must be an array of 4 bytes.");

                portTypes = value; 
            }
        }

        private byte[] goodInput = new byte[4];

        public byte[] GoodInput
        {
            get { return goodInput; }
            set 
            {
                if (value.Length != 4)
                    throw new ArgumentException("The good input must be an array of 4 bytes.");

                goodInput = value; 
            }
        }

        private byte[] goodOutput = new byte[4];

        public byte[] GoodOutput
        {
            get { return goodOutput; }
            set {
                if (value.Length != 4)
                    throw new ArgumentException("The good output must be an array of 4 bytes.");

                goodOutput = value; 
            }
        }

        private byte[] swIn = new byte[4];

        public byte[] SwIn
        {
            get { return swIn; }
            set { swIn = value; }
        }

        private byte[] swOut = new byte[4];

        public byte[] SwOut
        {
            get { return swOut; }
            set { swOut = value; }
        }

        private byte swVideo=0;

        public byte SwVideo
        {
            get { return swVideo; }
            set { swVideo = value; }
        }

        private byte swMacro=0;

        public byte SwMacro
        {
            get { return swMacro; }
            set { swMacro = value; }
        }

        private byte swRemote=0;

        public byte SwRemote
        {
            get { return swRemote; }
            set { swRemote = value; }
        }

        private byte style=0;

        public byte Style
        {
            get { return style; }
            set { style = value; }
        }

        private byte[] macAddress= new byte[6];

        public byte[] MacAddress
        {
            get { return macAddress; }
            set 
            {
                if (value.Length != 6) 
                    throw new ArgumentException("The mac address must be an array of 6 bytes.");
                
                macAddress = value; 
            }
        }

        private byte[] bindIpAddress = new byte[4];

        public byte[] BindIpAddress
        {
            get { return bindIpAddress; }
            set {
                if (value.Length != 4)
                    throw new ArgumentException("The bind IP address must be an array of 4 bytes.");

                bindIpAddress = value; }
        }

        private byte bindIndex= 0;

	    public byte BindIndex
	    {
		    get { return bindIndex;}
		    set { bindIndex = value;}
	    }

        private byte status2 = 0;

        public byte Status2
        {
            get { return status2; }
            set { status2 = value; }
        }
	
	
        #endregion

        public override void ReadData(System.IO.BinaryReader data)
        {
            Protocol = System.Text.ASCIIEncoding.UTF8.GetString(data.ReadBytes(8));
            OpCode = data.ReadInt16();
            IpAddress = data.ReadBytes(4);
            Port = data.ReadInt16();
            Version = data.ReadInt16();
        }

        public override void WriteData(System.IO.BinaryWriter data)
        {
            data.Write(System.Text.ASCIIEncoding.UTF8.GetBytes(Protocol.PadRight(8, (char) 0x0)));
            data.Write(OpCode);
            data.Write(IpAddress);
            data.Write(Port);
            data.Write(IPAddress.HostToNetworkOrder(FirmwareVersion));
            data.Write(IPAddress.HostToNetworkOrder(SubSwitch));
            data.Write(IPAddress.HostToNetworkOrder(Oem));
            data.Write(UbeaVersion);
            data.Write((byte) Status);
            data.Write(System.Text.ASCIIEncoding.UTF8.GetBytes(EstaCode.PadRight(2)));
            data.Write(System.Text.ASCIIEncoding.UTF8.GetBytes(ShortName.PadRight(18, (char) 0x0)));
            data.Write(System.Text.ASCIIEncoding.UTF8.GetBytes(LongName.PadRight(64, (char) 0x0)));
            data.Write(System.Text.ASCIIEncoding.UTF8.GetBytes(NodeReport.PadRight(64, (char) 0x0)));
            data.Write(IPAddress.HostToNetworkOrder(PortCount));
            data.Write(PortTypes);
            data.Write(GoodInput);
            data.Write(GoodOutput);
            data.Write(SwIn);
            data.Write(SwOut);
            data.Write(SwVideo);
            data.Write(SwMacro);
            data.Write(SwRemote);
            data.Write(GoodInput);
            data.Write(new byte[3]);
            data.Write(Style);
            data.Write(MacAddress);
            data.Write(BindIpAddress);
            data.Write(BindIndex);
            data.Write(Status2);
            data.Write(new byte[208]);
        }
	

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Acn.Slp;
using System.Net;
using Acn.Sockets;
using Acn.Rdm;

namespace SandboxAcnDevice
{
    public partial class AcnDevice : Form
    {
        private SlpServiceAgent slpService = new SlpServiceAgent();
        private List<RdmDevice> devices = new List<RdmDevice>();
        private RdmSocket socket = new RdmSocket(UId.NewUId(0xFF),Guid.NewGuid(), "Sandbox Acn Device");

        public AcnDevice()
        {
            InitializeComponent();

            slpService.NewDirectoryAgentFound += new EventHandler(slpAgent_NewDirectoryAgentFound);

            devices.Add(new RdmDevice(socket));

            socket.NewRdmPacket += new EventHandler<NewPacketEventArgs<RdmPacket>>(socket_NewRdmPacket);
        }

        void socket_NewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<NewPacketEventArgs<RdmPacket>>(socket_NewRdmPacket), sender, e);
                return;
            }

            rdmTrace.Items.Add(string.Format("{0}:{1} from {2}:{3}", e.Packet.Header.Command.ToString(), e.Packet.Header.ParameterId.ToString(), e.Source.ToString(), ""));
        }


        void slpAgent_NewDirectoryAgentFound(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler(slpAgent_NewDirectoryAgentFound), sender, e);
                return;
            }

            daList.Items.Clear();

            foreach (DirectoryAgentInformation item in new List<DirectoryAgentInformation>(slpService.DirectoryAgents.Values))
            {
                daList.Items.Add(item.Url);
            }
        }

        private void StartDevice()
        {
            IPAddress localAddress = new IPAddress(new byte[] { 10, 0, 0, 2 });
            
            socket.Open(localAddress);    

            if (!slpService.Active)
            {
                slpService.NetworkAdapter = localAddress;
                slpService.Scope = "ACN-DEFAULT";
                slpService.ServiceType = serviceTypeText.Text;
                slpService.ServiceUrl = string.Format("{0}//192.168.2.30:5568/{1}", serviceTypeText.Text,uidText.Text);

                slpService.Open();
            }       
        }

        private void RefreshRunningState()
        {
            stopButton.Enabled = slpService.Active;

            startButton.Enabled = !slpService.Active;
            deviceSettingsGroup.Enabled = !slpService.Active;
        }

        private void StopDevice()
        {
            slpService.Close();
            socket.Close();  
        }


        private void startButton_Click(object sender, EventArgs e)
        {
            StartDevice();
            RefreshRunningState();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            StopDevice();
            RefreshRunningState();
        }

        private void AcnDevice_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopDevice();
        }

        private void serviceTypeText_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

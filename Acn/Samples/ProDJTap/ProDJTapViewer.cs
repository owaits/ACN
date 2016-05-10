using ProDJTap.Packets;
using ProDJTap.Sockets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProDJTapViewer
{
    public partial class ProDJTapViewer : Form
    {
        private ProDJTap.Sockets.DJTapSocket socket= new ProDJTap.Sockets.DJTapSocket();
        public ProDJTapViewer()
        {
            InitializeComponent();

            CardInfo firstCard = null;
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.SupportsMulticast)
                {
                    IPInterfaceProperties ipProperties = adapter.GetIPProperties();

                    for (int n = 0; n < ipProperties.UnicastAddresses.Count; n++)
                    {
                        CardInfo card = new CardInfo(adapter, n);
                        if (card.IpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            networkCardSelect.Items.Add(card);
                        }

                        firstCard = card;
                    }
                }
            }

            if (firstCard != null)
                Start(firstCard);
        }

        private void Start(CardInfo networkCard)
        {
            socket = new DJTapSocket();
            socket.Brand = "GW-AVOLP";
            socket.Model = "GW-TCSSN";
            socket.NewPacket += socket_NewPacket;
            socket.Open(networkCard.IpAddress,networkCard.SubnetMask);
        }

        private void Stop()
        {
            if (socket != null)
            {
                socket.Close();
                socket = null;
            }
        }

        private Timecode timecodePacket = null;

        public Timecode TimecodePacket
        {
            get { return timecodePacket;  }
            set
            {
                if(timecodePacket != value)
                {
                    timecodePacket = value;

                    if (InvokeRequired)
                        BeginInvoke(new Action(UpdateTCInfo));
                }
            }
        }

        private void UpdateTCInfo()
        {
             tcInfo.SelectedObject = TimecodePacket;
        }

        void socket_NewPacket(object sender, Acn.Sockets.NewPacketEventArgs<DJTapPacket> e)
        {
            if(e.Packet is Timecode)
                TimecodePacket = (Timecode) e.Packet;
        }

        private void networkCardSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            CardInfo networkCard = networkCardSelect.SelectedItem as CardInfo;

            if (networkCard != null)
            {
                try
                {
                    Stop();
                    Start(networkCard);
                }
                catch (SocketException)
                {
                    MessageBox.Show("Unable to use selected Network interface.");
                }

            }
        }
    }
}

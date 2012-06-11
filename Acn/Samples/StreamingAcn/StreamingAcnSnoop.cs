using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Acn.Sockets;
using System.Net;
using Acn.Packets.sAcn;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;
using Acn;
using Acn.Helpers;

namespace StreamingAcn
{
    public partial class StreamingAcnSnoop : Form
    {
        private StreamingAcnSocket socket = new StreamingAcnSocket(Guid.NewGuid(), "Streaming ACN Snoop");
        private DmxStreamer dmxOutput;
        private DmxUniverseData recieveData = new DmxUniverseData();

        #region Setup and Initialisation

        private void Start(CardInfo networkCard, IEnumerable<int> universes)
        {
            socket = new StreamingAcnSocket(Guid.NewGuid(), "Streaming ACN Snoop");
            socket.NewPacket += new EventHandler<NewPacketEventArgs<Acn.Packets.sAcn.DmxPacket>>(socket_NewPacket);
            socket.Open(networkCard.IpAddress);

            foreach (int universe in universes)
                socket.JoinDmxUniverse(universe);

            dmxOutput = new DmxStreamer(socket);
            dmxOutput.AddUniverse(sendData.Universe);
            dmxOutput.Start();
        }

        private void Stop()
        {
            if (dmxOutput != null)
            {
                dmxOutput.Dispose();
                dmxOutput = null;
            }

            if (socket != null)
            {
                socket.Close();
                socket = null;
            }
        }

        #endregion

        public StreamingAcnSnoop()
        {
            InitializeComponent();

            for (int n = 1; n <= 512; n++)
            {
                ChannelCell recieveCell = new ChannelCell(n, recieveData);
                recieveCell.Width = 60;
                recieveCell.Margin = new Padding(3);

                channelArea.Controls.Add(recieveCell);

                ChannelCell cell = new ChannelCell(n, sendData);
                cell.Width = 60;
                cell.Margin = new Padding(3);

                cell.Click += new EventHandler(cell_Click);

                sendChannelArea.Controls.Add(cell);
            }

            CardInfo firstCard = null;
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.SupportsMulticast)
                {
                    IPInterfaceProperties ipProperties = adapter.GetIPProperties();

                    for (int n = 0; n < ipProperties.UnicastAddresses.Count; n++)
                    {
                        CardInfo card = new CardInfo(adapter, n);
                        networkCardSelect.Items.Add(card);

                        firstCard = card;
                    }
                }
            }

            if(firstCard != null)
             Start(firstCard,((IEnumerable<int>) new int[] {int.Parse(toolStripTextBox1.Text)}));
        }




        void socket_NewPacket(object sender, NewPacketEventArgs<Acn.Packets.sAcn.DmxPacket> e)
        {
            DmxPacket dmxPacket = e.Packet as DmxPacket;
            if (dmxPacket != null)
            {
                recieveData.DmxData = dmxPacket.Dmx.Data;
            }
        }

        private void StreamingAcnSnoop_FormClosing(object sender, FormClosingEventArgs e)
        {
            dmxOutput.Stop();
            socket.Close();
        }

        private int selectedUniverse = 1;

        public int SelectedUniverse
        {
            get { return selectedUniverse; }
            set 
            {
                if (selectedUniverse != value)
                {
                    selectedUniverse = value;

                    foreach (int universe in new List<int>(socket.DmxUniverses))
                    {
                        socket.DropDmxUniverse(universe);
                        dmxOutput.RemoveUniverse(universe);
                    }

                    socket.JoinDmxUniverse(selectedUniverse);

                    sendData.Universe = new DmxUniverse(selectedUniverse);
                    dmxOutput.AddUniverse(sendData.Universe);
                }
            }
        }


        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    SelectedUniverse = int.Parse(toolStripTextBox1.Text);
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Set Universe");
                }
                
            }
        }

        private void networkCardSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            CardInfo networkCard = networkCardSelect.SelectedItem as CardInfo;

            if (networkCard != null)
            {
                ReadOnlyCollection<int> universes = socket.DmxUniverses;

                Stop();
                Start(networkCard, universes);
            }
        }

        #region Send Dmx
        private DmxUniverseData sendData = new DmxUniverseData();

        private ChannelCell selectedChannel = null;

        public ChannelCell SelectedChannel 
        {
            get { return selectedChannel; }
            set
            {
                if (selectedChannel != value)
                {
                    if (selectedChannel != null)
                    {
                        selectedChannel.Selected = false;
                    }

                    selectedChannel = value;
                    UpdateLevelControls();

                    if (selectedChannel != null)
                    {
                        selectedChannel.Selected = true;
                    }
                }
            }
        }

        private void UpdateLevelControls()
        {
            if (selectedChannel == null)
            {
                levelGroup.Enabled = false;
                levelBar.Value = 0;
            }
            else
            {
                levelGroup.Enabled = true;
                levelBar.Value = sendData.DmxData[SelectedChannel.Channel];
            }
        }

        private void levelFull_Click(object sender, EventArgs e)
        {
            sendData.SetLevel(SelectedChannel.Channel,255);
            levelBar.Value = sendData.DmxData[SelectedChannel.Channel];
        }

        private void levelZero_Click(object sender, EventArgs e)
        {
            sendData.SetLevel(SelectedChannel.Channel, 0);
            levelBar.Value = sendData.DmxData[SelectedChannel.Channel];
        }

        private void levelBar_Scroll(object sender, EventArgs e)
        {
            sendData.SetLevel(SelectedChannel.Channel, (byte) levelBar.Value);
        }

        void cell_Click(object sender, EventArgs e)
        {
            ChannelCell cell = sender as ChannelCell;
            if (cell != null)
            {
                SelectedChannel = cell;
            }
        }

        #endregion       

    }
}

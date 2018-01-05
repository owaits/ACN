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
using StreamingAcn.RdmNet;
using Acn.Rdm.Packets.Net;
using System.Net.Sockets;

namespace StreamingAcn
{
    public partial class StreamingAcnSnoop : Form
    {
        private StreamingAcnSocket socket = new StreamingAcnSocket(Guid.NewGuid(), "Streaming ACN Snoop");
        private DmxStreamer dmxOutput;
        private DmxUniverseData recieveData = new DmxUniverseData();

        private RdmNetEndPointExplorer acnPortExplorer;

        #region Setup and Initialisation

        private void Start(CardInfo networkCard, IEnumerable<int> universes)
        {
            socket = new StreamingAcnSocket(Guid.NewGuid(), "Streaming ACN Snoop");
            socket.SynchronizationAddress = SynchronizationUniverse;
            socket.NewPacket += socket_NewPacket;
            socket.NewSynchronize += socket_NewSynchronize;
            socket.NewDiscovery += socket_NewDiscovery;
            socket.Open(networkCard.IpAddress);
            socket.StartDiscovery();

            foreach (int universe in universes)
                socket.JoinDmxUniverse(universe);

            dmxOutput = new DmxStreamer(socket);
            dmxOutput.AddUniverse(sendData.Universe);

            acnPortExplorer = new RdmNetEndPointExplorer();
            acnPortExplorer.LocalAdapter = networkCard.IpAddress;
            acnPortExplorer.NewEndpointFound += acnPortExplorer_NewEndpointFound;
            acnPortExplorer.Start();

        }

        private int lockAddress = 0;

        void socket_NewSynchronize(object sender, NewPacketEventArgs<StreamingAcnSynchronizationPacket> e)
        {
            lockAddress = e.Packet.Framing.SynchronizationAddress;
        }

        private void Stop()
        {
            if (acnPortExplorer != null)
            {
                acnPortExplorer.Stop();
                acnPortExplorer = null;
            }

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
            SetupGrid();

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
                        if(card.IpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            networkCardSelect.Items.Add(card);
                        }

                        firstCard = card;
                    }
                }
            }

            if(firstCard != null)
             Start(firstCard,((IEnumerable<int>) new int[] {int.Parse(toolStripTextBox1.Text)}));
        }

        private void SetupGrid()
        {
            DataGridViewTextBoxColumn column;
            portGrid.AutoGenerateColumns = false;
            portGrid.CellClick += portGrid_CellClick;
            portGrid.CellFormatting += portGrid_CellFormatting;

            portGrid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Device",
                DataPropertyName = "ManufacturerLabel",
                HeaderText = "Device"
            });

            portGrid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Label",
                DataPropertyName = "DeviceLabel",
                HeaderText = "Label"
            });

            portGrid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "Endpoint",
                DataPropertyName = "Universe",
                HeaderText = "Endpoint",
                Width = 80
            });

            column = new DataGridViewTextBoxColumn();
            column.Name = "PortLabel";
            column.DataPropertyName = "PortLabel";
            column.HeaderText = "Port Label";
            portGrid.Columns.Add(column);


            portGrid.Columns.Add(new DataGridViewComboBoxColumn()
            {
                Name = "Direction",
                DataPropertyName = "Direction",
                HeaderText = "Direction",
                ValueType = typeof(EndpointMode.EndpointModes),
                DataSource = Enum.GetValues(typeof(EndpointMode.EndpointModes))
            });

            column = new DataGridViewTextBoxColumn();
            column.Name = "AcnUniverse";
            column.DataPropertyName = "AcnUniverse";
            column.HeaderText = "Universe";
            portGrid.Columns.Add(column);

            portGrid.Columns.Add(new DataGridViewButtonColumn()
            {
                Name = "Patched",
                HeaderText = "Patched",
                DataPropertyName = "Patched",
                Width = 60
            });

            portGrid.Columns.Add(new DataGridViewButtonColumn()
            {
                Name = "Identify",
                HeaderText = "Identify",
                DataPropertyName = "Identify",
                Width = 60
            });
        }

        void portGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            
            RdmNetEndPoint endpoint = portGrid.Rows[e.RowIndex].DataBoundItem as RdmNetEndPoint;
            if (endpoint != null)
            {
                if (e.ColumnIndex == portGrid.Columns["Identify"].Index)
                    e.Value = endpoint.Identify ? "On" : "Off";
            }
        }

        void portGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {            
            if(e.RowIndex >=0)
            {
                RdmNetEndPoint endpoint = portGrid.Rows[e.RowIndex].DataBoundItem as RdmNetEndPoint;
                if (endpoint != null)
                {
                    if (e.ColumnIndex == portGrid.Columns["Patched"].Index)
                    {
                        if (endpoint.AcnUniverse == 0)
                            endpoint.AcnUniverse = endpoint.Universe;
                        else
                            endpoint.AcnUniverse = 0;
                    }                    

                    if (e.ColumnIndex == portGrid.Columns["Identify"].Index)
                        endpoint.Identify = !endpoint.Identify;
                }
            }
        }


        void socket_NewDiscovery(object sender, NewPacketEventArgs<StreamingAcnDiscoveryPacket> e)
        {
            if (e.Packet != null)
            {
                AvailableUniverses = new HashSet<int>(e.Packet.UniverseDiscovery.Universes);
            }
        }

        private bool locked = false;

        public bool Locked 
        { 
            get { return locked;}
            set 
            { 
                if(locked != value)
                {
                    locked = value; 
                    BeginInvoke(new Action(UpdateLock));   
                }                
            }
        }

        void socket_NewPacket(object sender, NewPacketEventArgs<StreamingAcnDmxPacket> e)
        {
            bool isLocked = false;

            StreamingAcnDmxPacket dmxPacket = e.Packet as StreamingAcnDmxPacket;
            if (dmxPacket != null)
            {
                recieveData.DmxData = dmxPacket.Dmx.Data;
                isLocked = (lockAddress != 0 && lockAddress == dmxPacket.Framing.SyncPacketAddress);
            }

            Locked = isLocked;
        }

        private void UpdateLock()
        {
            lockIndicator.Visible= Locked;
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

        private int synchronizationUniverse = 0;

        public int SynchronizationUniverse
        {
            get { return synchronizationUniverse; }
            set 
            { 
                if(synchronizationUniverse != value)
                {
                    synchronizationUniverse = value;
                    if (socket != null)
                        socket.SynchronizationAddress = value;
                }
                
            }
        }

        private HashSet<int> availableUniverses = new HashSet<int>();

        public HashSet<int> AvailableUniverses
        {
            get { return availableUniverses; }
            set { availableUniverses = value; }
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
                try
                {
                    Stop();
                    Start(networkCard, new List<int>(){ selectedUniverse});
                }
                catch (SocketException)
                {
                    MessageBox.Show("Unable to use selected Network interface.");
                }

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

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            sendSelect.Checked = true;
            recieveSelect.Checked = false;

            dataTabs.SelectedTab = sendTab;
            dmxOutput.Start();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            sendSelect.Checked = false;
            recieveSelect.Checked = true;

            dataTabs.SelectedTab = recieveTab;
            dmxOutput.Stop();
        }

        #region RDMNet Ports

        private BindingList<RdmNetEndPoint> ports = new BindingList<RdmNetEndPoint>();

        void acnPortExplorer_NewEndpointFound(object sender, EventArgs e)
        {
            BeginInvoke(new UpdatePortsHandler(UpdatePorts));
        }

        private delegate void UpdatePortsHandler();

        private void UpdatePorts()
        {
            ports.Clear();

            lock (acnPortExplorer.DiscoveredEndpoints)
            {
                foreach (RdmNetEndPoint port in acnPortExplorer.DiscoveredEndpoints)
                    ports.Add(port);
            }

            portGrid.DataSource = ports;
            portGrid.ResetBindings();
        }

        #endregion

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            acnPortExplorer.ReadOnly = !acnPortExplorer.ReadOnly;

            toolStripButton1.Text = acnPortExplorer.ReadOnly ? "Read Only" : "Edit";
        }

        private void portGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (acnPortExplorer.ReadOnly)
            {
                MessageBox.Show("Editting has been disabled, Read Only is selected.");
                e.Cancel = true;
            }
        }

        private void toolStripTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    SynchronizationUniverse = int.Parse(toolStripTextBox2.Text);
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Set Universe");
                }

            }
        }

    }
}

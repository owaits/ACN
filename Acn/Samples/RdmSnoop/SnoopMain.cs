using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using Acn.Rdm;
using RdmSnoop.Models;
using RdmSnoop.Transports;
using Acn.Slp;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;
using Acn.Sockets;
using RdmNetworkMonitor;
using RdmSnoop.Tools;
using Acn.Rdm.Routing;
using Acn.Rdm.Packets;

namespace RdmSnoop
{
    public partial class SnoopMain : Form
    {
        private ListViewColumnSorter columnSorter = new ListViewColumnSorter();

        public SnoopMain()
        {
            InitializeComponent();


            packetView.ListViewItemSorter = columnSorter;
            packetView.Columns.Add("Time", 100);            
            packetView.Columns.Add("Parameter", 200);
            packetView.Columns.Add("Command",120);
            packetView.Columns.Add("Type", 100);
            packetView.Columns.Add("Source Id",150);
            packetView.Columns.Add("Target Id",150);
            packetView.Columns.Add("Sub Device", 50); 
            packetView.Columns.Add("IP Address",150);
            packetView.Columns.Add("Message Count", 90);
            packetView.Columns.Add("Transaction", 90);


            IPAddress selectedIp = IPAddress.None;
            IPAddress.TryParse(Properties.Settings.Default.NetworkAdapter, out selectedIp);
            CardInfo selectedCard = null;

            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.SupportsMulticast && adapter.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties ipProperties = adapter.GetIPProperties();

                    for (int n = 0; n < ipProperties.UnicastAddresses.Count; n++)
                    {
                        if(ipProperties.UnicastAddresses[n].Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            CardInfo card = new CardInfo(adapter, n);
                            networkCardSelect.Items.Add(card);

                            if (selectedIp != null && card.IpAddress.ToString() == selectedIp.ToString())
                                selectedCard = card;
                        }
                    }
                }
            }

            if (selectedCard != null)
                networkCardSelect.SelectedItem = selectedCard;
            else
                networkCardSelect.SelectedIndex = 1;

        }

        private CardInfo selectedNetworkAdapter = null;

        public CardInfo SelectedNetworkAdapter
        {
            get { return selectedNetworkAdapter; }
            set 
            {
                if (selectedNetworkAdapter != value)
                {
                    selectedNetworkAdapter = value;

                    if (selectedNetworkAdapter != null && Transport != null)
                    {
                        StopTransport();
                        StartTransport();
                    }

                    Properties.Settings.Default.NetworkAdapter = selectedNetworkAdapter.IpAddress.ToString();
                    Properties.Settings.Default.Save();
                }
            }
        }

        private RdmDeviceBroker selectedDevice = null;

        public RdmDeviceBroker SelectedDevice
        {
            get { return selectedDevice; }
            set 
            {
                if (selectedDevice != value)
                {
                    selectedDevice = value;
                    LoadDevice();
                }
 
            }
        }

        private void LoadDevice()
        {
            deviceToolbox.Enabled = SelectedDevice != null;
            deviceInformation.SelectedObject = SelectedDevice;

            modeTool.DropDownItems.Clear();

            if (SelectedDevice.DeviceInformation != null)
            {
                for (int n = 1; n <= SelectedDevice.DeviceInformation.DmxPersonalityCount; n++)
                {
                    ToolStripMenuItem newItem = new ToolStripMenuItem(string.Format("Mode {0}", n));
                    if (n == SelectedDevice.DeviceInformation.DmxPersonality)
                        newItem.Checked = true;

                    newItem.Tag = n;
                    modeTool.DropDownItems.Add(newItem);

                    newItem.Click += new EventHandler(modeTool_Click);
                }
            }
        }

        void modeTool_Click(object sender, EventArgs e)
        {
            ToolStripDropDownItem item = sender as ToolStripDropDownItem;
            if(item != null)
            {
                SelectedDevice.SetMode((int)item.Tag);
            }
        }



        private IRdmTransport transport = null;

        public IRdmTransport Transport
        {
            get { return transport; }
            set 
            {
                if (transport != value)
                {
                    transport = value;

                    if (transport != null)
                    {
                        transport.NewDeviceFound += new EventHandler<DeviceFoundEventArgs>(transport_NewDeviceFound);
                    }

                    Properties.Settings.Default.Transport = transport.GetType().Name;
                    Properties.Settings.Default.Save();
                }
                
            }
        }


        void transport_NewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            if (!pause)
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new EventHandler<NewPacketEventArgs<RdmPacket>>(transport_NewRdmPacket), sender, e);
                    return;
                }

                DateTime timeStamp = DateTime.Now;

                ListViewItem newItem = new ListViewItem(string.Format("{0}{1}", timeStamp.ToLongTimeString(), timeStamp.Millisecond.ToString()));
                newItem.SubItems.Add(e.Packet.Header.ParameterId.ToString());
                newItem.SubItems.Add(e.Packet.Header.Command.ToString());
                newItem.SubItems.Add(((RdmResponseTypes)e.Packet.Header.PortOrResponseType).ToString());
                newItem.SubItems.Add(e.Packet.Header.SourceId.ToString());
                newItem.SubItems.Add(e.Packet.Header.DestinationId.ToString());
                newItem.SubItems.Add(e.Packet.Header.SubDevice.ToString());
                newItem.SubItems.Add(e.Source.Address.ToString());
                newItem.SubItems.Add(e.Packet.Header.MessageCount.ToString());
                newItem.SubItems.Add(e.Packet.Header.TransactionNumber.ToString());

                packetView.Items.Add(newItem);
            }
        }

        private void StopTransport()
        {
            Transport.Stop();
            rdmDevices.Nodes.Clear();
            devices.Clear();
            packetView.Items.Clear();
        }

        private void StartTransport()
        {
            ISnoopTransport snoopTransport = Transport as ISnoopTransport;
            if (snoopTransport != null)
            {
                snoopTransport.LocalAdapter = SelectedNetworkAdapter.IpAddress;
                snoopTransport.SubnetMask = selectedNetworkAdapter.SubnetMask;
            }

            Transport.Start();

            IRdmSocket socket = Transport.Socket;
            socket.NewRdmPacket += transport_NewRdmPacket;
            socket.RdmPacketSent += transport_NewRdmPacket;

            RdmReliableSocket reliableSocket = socket as RdmReliableSocket;
            if (reliableSocket != null)
            {
                reliableSocket.PropertyChanged += new PropertyChangedEventHandler(reliableSocket_PropertyChanged);
                UpdatePacketCount(reliableSocket);
            }
        }

        void reliableSocket_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new EventHandler<PropertyChangedEventArgs>(reliableSocket_PropertyChanged),sender,e);
                return;
            }

            RdmReliableSocket reliableSocket = sender as RdmReliableSocket;
            if (reliableSocket != null)
            {
                switch (e.PropertyName)
                {
                    case "PacketsSent":
                        packetsSentLabel.Text = "Sent: " + reliableSocket.PacketsSent;
                        break;
                    case "PacketsRecieved":
                        packetsRecievedLabel.Text = "Recieved: " + reliableSocket.PacketsRecieved;
                        break;
                    case "PacketsDropped":
                        droppedLabel.Text = "Dropped: " + reliableSocket.PacketsDropped;
                        break;
                    case "TransactionsStarted":
                        transactionsLabel.Text = "Started: " + reliableSocket.TransactionsStarted;
                        break;
                    case "TransactionsFailed":
                        failedLabel.Text = "Failed: " + reliableSocket.TransactionsFailed;
                        break;
                }
            }
        }

        private void UpdatePacketCount(RdmReliableSocket reliableSocket)
        {
            packetsSentLabel.Text = "Sent: " + reliableSocket.PacketsSent;
            packetsRecievedLabel.Text = "Recieved: " + reliableSocket.PacketsRecieved;
            droppedLabel.Text = "Dropped: " + reliableSocket.PacketsDropped;
            transactionsLabel.Text = "Started: " + reliableSocket.TransactionsStarted;
            failedLabel.Text = "Failed: " + reliableSocket.TransactionsFailed;
        }

        void transport_NewDeviceFound(object sender, DeviceFoundEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<DeviceFoundEventArgs>(transport_NewDeviceFound), sender, e);
                return;
            }

            AddDevice(e.Id, e.IpAddress);
        }


        private Dictionary<UId, RdmDeviceModel> devices = new Dictionary<UId, RdmDeviceModel>();

        private void AddDevice(UId id, RdmEndPoint address)
        {
            if (!devices.ContainsKey(id))
            {
                RdmDeviceModel device = new RdmDeviceModel(new TreeNode(id.ToString()), Transport.Socket, id, address);
                devices[id] = device;
                rdmDevices.Nodes.Add(device.Node);

                device.Interogate();
            }
        }

        private void rdmNetSelect_Click(object sender, EventArgs e)
        {
            if (!(Transport is RdmNet))
            {
                StopTransport();
                Transport = new RdmNet();
                StartTransport();
                
            }

            rdmNetSelect.Checked = true;
            artNetSelect.Checked = false;
            routerSelect.Checked = false;
        }

        private void artNetSelect_Click(object sender, EventArgs e)
        {
            if (!(Transport is ArtNet))
            {
                StopTransport();
                Transport = new ArtNet();
                StartTransport();
            }

            rdmNetSelect.Checked = false;
            artNetSelect.Checked = true;
            routerSelect.Checked = false;
        }

        private void networkCardSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedNetworkAdapter = networkCardSelect.SelectedItem as CardInfo;

            
        }

        private void discoverSelect_Click(object sender, EventArgs e)
        {
            Transport.Discover(DiscoveryType.GatewayDiscovery);
        }

        private void SnoopMain_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.Transport == typeof(ArtNet).Name)
            {
                Transport = new ArtNet();

                rdmNetSelect.Checked = false;
                artNetSelect.Checked = true;
            }
            else if (Properties.Settings.Default.Transport == typeof(RdmRouter).Name)
            {
                Transport = CreateRouter();
            }
            else
            {
                Transport = new RdmNet();
            }

            StartTransport();
        }

        private void rdmDevices_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RdmDeviceModel model = e.Node.Tag as RdmDeviceModel;
            if (model != null)
            {
                SelectedDevice = model.Broker;
                
            }
        }

        private void identifyOn_Click(object sender, EventArgs e)
        {
            SelectedDevice.Identify(true);
        }

        private void identifyOff_Click(object sender, EventArgs e)
        {
            SelectedDevice.Identify(false);
        }

        private void addressTool_Click(object sender, EventArgs e)
        {
            DmxAddressDialog addressDialog = new DmxAddressDialog();
            addressDialog.DmxAddress = SelectedDevice.DmxAddress;

            if (addressDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedDevice.SetDmxAddress(addressDialog.DmxAddress);
            }
        }

        private void modeTool_DropDownOpened(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in modeTool.DropDownItems)
            {
                item.Checked = ((int) (item.Tag) == SelectedDevice.DeviceInformation.DmxPersonality);
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedDevice.Reset();
        }

        private void selfTestTool_Click(object sender, EventArgs e)
        {
            selectedDevice.SelfTest();
        }

        private void powerOffTool_Click(object sender, EventArgs e)
        {
            selectedDevice.Power(Acn.Rdm.Packets.Control.PowerState.States.Off);
        }

        private void shutdownTool_Click(object sender, EventArgs e)
        {
            selectedDevice.Power(Acn.Rdm.Packets.Control.PowerState.States.Shutdown);
        }

        private void powerStandbyTool_Click(object sender, EventArgs e)
        {
            selectedDevice.Power(Acn.Rdm.Packets.Control.PowerState.States.Standby);
        }

        private void powerOnTool_Click(object sender, EventArgs e)
        {
            selectedDevice.Power(Acn.Rdm.Packets.Control.PowerState.States.Normal);
        }

        private bool pause = false;

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            pause = !pause;
        }

        private void routerSelect_Click(object sender, EventArgs e)
        {
            RdmRouter newRouter = CreateRouter();

            //Change Transpaort
            if (!(Transport is RdmRouter))
            {
                StopTransport();
                Transport = newRouter;
                StartTransport();
            }
            
            rdmNetSelect.Checked = false;
            artNetSelect.Checked = false;
            routerSelect.Checked = true;


        }

        private RdmRouter CreateRouter()
        {
            RdmRouter newRouter = new RdmRouter();

            int priority = 0;
                        foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.SupportsMulticast)
                {
                    IPInterfaceProperties ipProperties = adapter.GetIPProperties();

                    for (int n = 0; n < ipProperties.UnicastAddresses.Count; n++)
                    {
                        if (ipProperties.UnicastAddresses[n].Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            CardInfo card = new CardInfo(adapter, n);
                            if (card.SubnetMask != null)
                            {
                                RdmNet rdmNetTransport = new RdmNet();
                                rdmNetTransport.LocalAdapter = card.IpAddress;
                                rdmNetTransport.SubnetMask = card.SubnetMask;
                                newRouter.RegisterTransport(rdmNetTransport, "RdmNet: " + card.ToString(), string.Empty, priority);
                                priority++; 

                                ArtNet artNettransport = new ArtNet();
                                artNettransport.LocalAdapter = card.IpAddress;
                                artNettransport.SubnetMask = card.SubnetMask;
                                newRouter.RegisterTransport(artNettransport, "ArtNet: " + card.ToString(), string.Empty, priority);
                                priority++;                                                                   
                            }
                        }
                    }
                }
            }

            return newRouter;
        }

        private void packetView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == columnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (columnSorter.Order == SortOrder.Ascending)
                {
                    columnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    columnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                columnSorter.SortColumn = e.Column;
                columnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            packetView.Sort();
        }

        private void rdmDiscoverSelect_Click(object sender, EventArgs e)
        {
            Transport.Discover(DiscoveryType.DeviceDiscovery);
        }

        private RdmRawPacket customMessage = null;

        private void sendTool_Click(object sender, EventArgs e)
        {
            UserMessage message = new UserMessage();
            if (customMessage != null)
            {
                message.Command = customMessage.Header.Command;
                message.ParameterID = customMessage.Header.ParameterId;
                message.Data = customMessage.Data;
            }

            if (message.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                customMessage = message.Message;
                if (customMessage == null)
                    MessageBox.Show("Invalid message format!");
                else
                    Transport.Socket.SendRdm(message.Message, SelectedDevice.Address, SelectedDevice.Id);
            }
        }


    }
}

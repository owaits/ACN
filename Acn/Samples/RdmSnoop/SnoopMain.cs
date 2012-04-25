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

namespace RdmSnoop
{
    public partial class SnoopMain : Form
    {
        public SnoopMain()
        {
            InitializeComponent();

            packetView.Columns.Add("Time", 100);            
            packetView.Columns.Add("Parameter", 200);
            packetView.Columns.Add("Command",120);
            packetView.Columns.Add("Type", 100);
            packetView.Columns.Add("Source Id",150);
            packetView.Columns.Add("Target Id",150);
            packetView.Columns.Add("Sub Device", 50); 
            packetView.Columns.Add("IP Address",150);

            
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.SupportsMulticast)
                {
                    IPInterfaceProperties ipProperties = adapter.GetIPProperties();

                    for (int n = 0; n < ipProperties.UnicastAddresses.Count; n++)
                    {
                        if(ipProperties.UnicastAddresses[n].Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            CardInfo card = new CardInfo(adapter, n);
                            networkCardSelect.Items.Add(card);
                        }
                    }
                }
            }

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
                }
                
            }
        }


        void transport_NewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<NewPacketEventArgs<RdmPacket>>(transport_NewRdmPacket),sender,e);
                return;
            }

            DateTime timeStamp = DateTime.Now;

            ListViewItem newItem = new ListViewItem(string.Format("{0}{1}",timeStamp.ToLongTimeString(),timeStamp.Millisecond.ToString()));
            newItem.SubItems.Add(e.Packet.Header.ParameterId.ToString());
            newItem.SubItems.Add(e.Packet.Header.Command.ToString());
            newItem.SubItems.Add(((RdmResponseTypes) e.Packet.Header.PortOrResponseType).ToString());
            newItem.SubItems.Add(e.Packet.Header.SourceId.ToString());
            newItem.SubItems.Add(e.Packet.Header.DestinationId.ToString());
            newItem.SubItems.Add(e.Packet.Header.SubDevice.ToString());   
            newItem.SubItems.Add(e.Source.Address.ToString());
            
            packetView.Items.Add(newItem);
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
            Transport.Start(SelectedNetworkAdapter.IpAddress, selectedNetworkAdapter.SubnetMask);

            foreach (IRdmSocket socket in Transport.Sockets)
            {
                socket.NewRdmPacket += transport_NewRdmPacket;
                socket.RdmPacketSent += transport_NewRdmPacket;

                RdmReliableSocket reliableSocket = socket as RdmReliableSocket;
                if (reliableSocket != null)
                {
                    reliableSocket.PropertyChanged += new PropertyChangedEventHandler(reliableSocket_PropertyChanged);
                }

                UpdatePacketCount((RdmReliableSocket) socket);
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

        private void AddDevice(UId id, RdmAddress address)
        {
            if (!devices.ContainsKey(id))
            {
                RdmDeviceModel device = new RdmDeviceModel(new TreeNode(id.ToString()), Transport.GetDeviceSocket(id), id, address);
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
        }

        private void networkCardSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedNetworkAdapter = networkCardSelect.SelectedItem as CardInfo;

            
        }

        private void discoverSelect_Click(object sender, EventArgs e)
        {
            Transport.Discover();
        }

        private void SnoopMain_Load(object sender, EventArgs e)
        {
            Transport = new RdmNet();
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


    }
}

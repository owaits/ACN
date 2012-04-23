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

namespace RdmSnoop
{
    public partial class SnoopMain : Form
    {
        public SnoopMain()
        {
            InitializeComponent();
            
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
                        Transport.Stop();
                        Transport.Start(selectedNetworkAdapter.IpAddress,selectedNetworkAdapter.SubnetMask);
                    }
                }
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

        private void StopTransport()
        {
            Transport.Stop();
            rdmDevices.Nodes.Clear();
            devices.Clear();
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

        private void AddDevice(UId id, IPAddress address)
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
                Transport.Start(SelectedNetworkAdapter.IpAddress, selectedNetworkAdapter.SubnetMask);
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
                Transport.Start(SelectedNetworkAdapter.IpAddress,selectedNetworkAdapter.SubnetMask);
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
                deviceInformation.SelectedObject = model.Broker;
            }
        }

    }
}

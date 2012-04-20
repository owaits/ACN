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

namespace RdmSnoop
{
    public partial class SnoopMain : Form
    {
        public SnoopMain()
        {
            InitializeComponent();

            Transport = new RdmNet();
            Transport.Start(IPAddress.Any);
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
            }
        }
    }
}

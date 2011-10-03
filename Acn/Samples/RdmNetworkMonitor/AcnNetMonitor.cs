#region Copyright © 2011 Oliver Waits
//______________________________________________________________________________________________________________
// ACN
// Copyright © 2011 Oliver Waits
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//______________________________________________________________________________________________________________
#endregion
   

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Acn.Slp;
using Acn.Rdm;
using System.Net;
using Acn.Sockets;
using Acn.Slp.Packets;

namespace RdmNetworkMonitor
{
    public partial class AcnNetMonitor : Form
    {
        private SlpUserAgent slpUser = new SlpUserAgent("ACN-DEFAULT");
        private RdmSocket acnSocket = new RdmSocket(UId.NewUId(0xFF),Guid.NewGuid(), "Acn Net Monitor");
        private IPAddress localAddress = new IPAddress(new byte[] { 10, 0, 0, 1 });

        public AcnNetMonitor()
        {
            InitializeComponent();

            slpUser.NetworkAdapter = localAddress;
            slpUser.ServiceFound += new EventHandler<ServiceFoundEventArgs>(slpUser_ServiceFound);
        }


        void slpUser_ServiceFound(object sender, ServiceFoundEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<ServiceFoundEventArgs>(slpUser_ServiceFound), sender, e);
                return;
            }

            foreach (UrlEntry url in e.Urls)
                AddDevice(UId.ParseUrl(url.Url), e.Address.Address);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if(!acnSocket.PortOpen)
                acnSocket.Open(localAddress);

            slpUser.Open();
            slpUser.Find("service:e133.esta");
        }


        private Dictionary<UId, RdmDeviceModel> devices = new Dictionary<UId, RdmDeviceModel>();

        private void AddDevice(UId id,IPAddress address)
        {
            if (!devices.ContainsKey(id))
            {
                RdmDeviceModel device = new RdmDeviceModel(new TreeNode(id.ToString()),acnSocket,id,address);
                devices[id] = device;                
                rdmDevices.Nodes.Add(device.Node);

                device.PortsChanged += new EventHandler(device_PortsChanged);
                device.Identify();                
            }
        }

        void device_PortsChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(device_PortsChanged), sender, e);
                return;
            }
            
            RdmDeviceModel model = sender as RdmDeviceModel;

            foreach (short port in model.Ports)
            {
                TreeNode portNode = new TreeNode("Port " + port.ToString());
                model.Node.Nodes.Add(portNode);
            }
        }

        private void RequestPorts()
        {

        }
    }
}

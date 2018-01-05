using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using Acn.Slp;
using Acn.Slp.Packets;
using Acn.Helpers;

namespace SlpDiscovery
{
    public partial class SlpDiscovery : Form
    {
        private SlpUserAgent slpUser = new SlpUserAgent();

        public SlpDiscovery()
        {
            InitializeComponent();

            slpUser.ServiceFound += new EventHandler<ServiceFoundEventArgs>(slpUser_ServiceFound);
            manager.DeviceUpdated += new EventHandler(manager_DevicesUpdated);
        }



        private void find_Click(object sender, EventArgs e)
        {
            deviceList.Items.Clear();

            //slpUser.NetworkAdapter = new IPAddress(new byte[] { 10, 0, 0, 1 });
            slpUser.Scope = scopeSelect.Text;
            slpUser.Open();
            slpUser.Find(urlText.Text);
        }

        void slpUser_ServiceFound(object sender, ServiceFoundEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<ServiceFoundEventArgs>(slpUser_ServiceFound), sender, e);
                return;
            }

            foreach (UrlEntry url in e.Urls)
                deviceList.Items.Add(url.Url);
        }

        private void SlpDiscovery_FormClosing(object sender, FormClosingEventArgs e)
        {
            slpUser.Dispose();
        }

        private void scopeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private SlpDeviceManager manager = new SlpDeviceManager();

        private void button1_Click(object sender, EventArgs e)
        {
            if (manager.Running)
            {
                manager.Stop();
            }
            else
            {
                manager.Scope = scopeSelect.Text;
                manager.ServiceType = urlText.Text;
                manager.Start();
            }
        }

        private void update_Click(object sender, EventArgs e)
        {
            manager.Update();
        }

        void manager_DevicesUpdated(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler(manager_DevicesUpdated));
                return;
            }
            devicesGrid.Rows.Clear();
            foreach (var device in manager.GetDevices())
            {
                devicesGrid.Rows.Add(
                    device.Url,
                    SlpServiceAgent.JoinAttributeString(device.Attributes),
                    device.LastContact,
                    device.State.ToString()
                    );
                Color backColour = Color.White;

                switch (device.State)
                {
                    case SlpDeviceState.New:
                        backColour = Color.LightGreen;
                        break;

                    case SlpDeviceState.MissedPoll:
                        backColour = Color.Yellow;
                        break;

                    case SlpDeviceState.Disappeared:
                        backColour = Color.Red;
                        break;

                    case SlpDeviceState.ReAppeared:
                        backColour = Color.Green;
                        break;
                }

                devicesGrid.Rows[devicesGrid.Rows.Count - 1].DefaultCellStyle.BackColor = backColour;

            }
        }

    }
}

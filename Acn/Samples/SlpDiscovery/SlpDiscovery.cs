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

namespace SlpDiscovery
{
    public partial class SlpDiscovery : Form
    {
        private SlpUserAgent slpUser = new SlpUserAgent();

        public SlpDiscovery()
        {
            InitializeComponent();

            slpUser.ServiceFound +=new EventHandler<ServiceFoundEventArgs>(slpUser_ServiceFound);
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

            foreach(UrlEntry url in e.Urls)
                deviceList.Items.Add(url.Url);
        }

        private void SlpDiscovery_FormClosing(object sender, FormClosingEventArgs e)
        {
            slpUser.Dispose();
        }

        private void scopeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}

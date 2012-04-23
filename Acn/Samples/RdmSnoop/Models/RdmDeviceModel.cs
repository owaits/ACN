using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RdmNetworkMonitor;
using System.Windows.Forms;
using Acn.Sockets;
using Acn.Rdm;
using System.Net;

namespace RdmSnoop.Models
{
    public class RdmDeviceModel:UserControl
    {


        public RdmDeviceModel(TreeNode node, IRdmSocket socket, UId id, IPAddress ipAddress)
        {
            CreateHandle();

            broker = new RdmDeviceBroker(socket, id, ipAddress);

            Node = node;
            Node.Tag = this;

            broker.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(RdmDeviceModel_PropertyChanged);
        }

        private RdmDeviceBroker broker = null;

        public RdmDeviceBroker Broker
        {
            get { return broker; }
            protected set { broker = value; }
        }


        void RdmDeviceModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Label" || e.PropertyName == "Model")
                BeginInvoke(new VoidHandler(UpdateNodeName));
        }

        private delegate void VoidHandler();

        private void UpdateNodeName()
        {
            if (string.IsNullOrEmpty(broker.Label))
                Node.Text = broker.Id.ToString();
            else
                Node.Text = string.Format("{0}:{1}",broker.Model,broker.Label);
        }

        public void Interogate()
        {
            broker.Interogate();
        }

        public TreeNode Node { get; set; }

    }
}

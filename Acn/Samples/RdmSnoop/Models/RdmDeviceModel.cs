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
        public RdmDeviceModel(TreeNode node, RdmDeviceBroker broker)
        {
            CreateHandle();

            if (broker == null)
                throw new ArgumentNullException("The broker parameter may not be null.");

            this.broker = broker;

            Node = node;
            Node.Tag = this;

            broker.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(RdmDeviceModel_PropertyChanged);

            UpdateNodeName();
        }

        public RdmDeviceModel(TreeNode node, IEnumerable<IRdmSocket> socket, UId id, RdmEndPoint address)
        {
            CreateHandle();

            broker = new RdmDeviceBroker(socket, id, address);

            Node = node;
            Node.Tag = this;

            broker.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(RdmDeviceModel_PropertyChanged);

            UpdateNodeName();
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

            if (e.PropertyName == "SubDevices")
                BeginInvoke(new VoidHandler(UpdateSubDevices));
        }

        private delegate void VoidHandler();

        private void UpdateNodeName()
        {
            if (string.IsNullOrEmpty(broker.Model))
                Node.Text = broker.Id.ToString();
            else
            {
                if (string.IsNullOrEmpty(broker.Label))
                    Node.Text = broker.Model;
                else
                    Node.Text = string.Format("{0}:{1}", broker.Model, broker.Label);
            }
        }

        private void UpdateSubDevices()
        {
            Node.Nodes.Clear();

            foreach (RdmDeviceBroker device in broker.SubDevices)
            {
                TreeNode newTreeNode = new TreeNode();
                RdmDeviceModel newModel = new RdmDeviceModel(newTreeNode, device);
                Node.Nodes.Add(newTreeNode);
            }
        }

        public void Interogate()
        {
            broker.Interogate();
        }

        public TreeNode Node { get; set; }

    }
}

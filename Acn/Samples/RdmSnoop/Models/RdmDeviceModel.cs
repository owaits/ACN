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
    public class RdmDeviceModel : RdmDeviceBroker
    {
        public RdmDeviceModel(TreeNode node, IRdmSocket socket, UId id, IPAddress ipAddress)
            : base(socket, id, ipAddress)
        {
            Node = node;
            Node.Tag = this;
        }

        public TreeNode Node { get; set; }

    }
}

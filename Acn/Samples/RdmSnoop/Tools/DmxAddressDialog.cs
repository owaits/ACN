using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RdmSnoop.Tools
{
    public partial class DmxAddressDialog : Form
    {
        public DmxAddressDialog()
        {
            InitializeComponent();
        }

        public int DmxAddress
        {
            get { return (int) addressNumber.Value; }
            set { addressNumber.Value = value; }
        }
    }
}

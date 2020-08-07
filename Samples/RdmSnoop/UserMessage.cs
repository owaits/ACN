using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LXProtocols.Acn.Rdm;
using LXProtocols.Acn.Rdm.Packets;

namespace RdmSnoop
{
    public partial class UserMessage : Form
    {
        public UserMessage()
        {
            InitializeComponent();
            commandSelect.SelectedIndex = 0;
        }

        public RdmCommands Command
        {
            get { return commandSelect.SelectedIndex == 0 ? RdmCommands.Get : RdmCommands.Set; }
            set { commandSelect.SelectedIndex = value == RdmCommands.Get ? 0 : 1; }
        }

        public RdmParameters ParameterID
        {
            get
            {
                int parameterId;
                if (int.TryParse(txtParameterId.Text, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out parameterId))
                    return (RdmParameters) parameterId;
                return 0;
            }
            set
            {
                txtParameterId.Text = ((int)value).ToString("X2");
            }
        }

        public byte[] Data
        {
            get { return StrToByteArray(txtData.Text); }
            set { txtData.Text = BitConverter.ToString(value); }
        }


        public RdmRawPacket Message
        {
            get
            {
                int parameterId;
                if (!int.TryParse(txtParameterId.Text,System.Globalization.NumberStyles.HexNumber,CultureInfo.InvariantCulture,out parameterId))
                    return null;                   

                RdmRawPacket packet = new RdmRawPacket(Command, ParameterID);
                packet.Data = Data;

                return packet;
            }
        }

        public static byte[] StrToByteArray(string str)
        {
            Dictionary<string, byte> hexindex = new Dictionary<string, byte>();
            for (int i = 0; i <= 255; i++)
                hexindex.Add(((byte)i).ToString("X2"), (byte)i);

            List<byte> hexres = new List<byte>();
            for (int i = 0; i < str.Length; i += 2)
                hexres.Add(hexindex[str.Substring(i, 2)]);

            return hexres.ToArray();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }


    }
}

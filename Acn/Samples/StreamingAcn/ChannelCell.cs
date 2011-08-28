using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace StreamingAcn
{
    public class ChannelCell:UserControl
    {
        private TextBox valueText;
        private Label channelLabel;
    
        public ChannelCell(int channel, DmxUniverseData data):base()
        {
            InitializeComponent();

            Channel = channel;
            UniverseData = data;

            UniverseData.DmxDataChanged += new EventHandler(UniverseData_DmxDataChanged);

            channelLabel.Text = channel.ToString("000");

            Level = UniverseData.DmxData[Channel];
        }

        void UniverseData_DmxDataChanged(object sender, EventArgs e)
        {
            if (UniverseData.DmxData != null)
            {
                Level = UniverseData.DmxData[Channel];
            }
        }

        public DmxUniverseData UniverseData { get; set; }

        public int Channel { get; set; }

        private int level = 0;
        public int Level 
        {
            get { return level; }
            set
            {
                if (level != value)
                {
                    level = value;
                    UpdateChannel();
                }
            }
        }

        private bool selected = false;

        public bool Selected
        {
            get { return selected; }
            set 
            { 
                selected = value;

                if (selected)
                {
                    valueText.BackColor = Color.OrangeRed;
                }
                else
                {
                    valueText.BackColor = Color.White;
                }
            }
        }


        
        private delegate void UpdateChannelHandler();

        private void UpdateChannel()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new UpdateChannelHandler(UpdateChannel));
                return;
            }

            valueText.Text = level.ToString();
        }

        private void InitializeComponent()
        {
            this.channelLabel = new System.Windows.Forms.Label();
            this.valueText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // channelLabel
            // 
            this.channelLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.channelLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.channelLabel.Location = new System.Drawing.Point(0, 0);
            this.channelLabel.Name = "channelLabel";
            this.channelLabel.Size = new System.Drawing.Size(25, 20);
            this.channelLabel.TabIndex = 0;
            this.channelLabel.Text = "001:";
            this.channelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // valueText
            // 
            this.valueText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.valueText.Location = new System.Drawing.Point(25, 0);
            this.valueText.Name = "valueText";
            this.valueText.ReadOnly = true;
            this.valueText.Size = new System.Drawing.Size(124, 20);
            this.valueText.TabIndex = 1;
            this.valueText.Text = "0";
            this.valueText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.valueText.TextChanged += new System.EventHandler(this.valueText_TextChanged);
            this.valueText.Click += new System.EventHandler(this.valueText_Click);
            // 
            // ChannelCell
            // 
            this.Controls.Add(this.valueText);
            this.Controls.Add(this.channelLabel);
            this.Name = "ChannelCell";
            this.Size = new System.Drawing.Size(149, 20);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void valueText_TextChanged(object sender, EventArgs e)
        {

        }

        private void valueText_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

    }
}

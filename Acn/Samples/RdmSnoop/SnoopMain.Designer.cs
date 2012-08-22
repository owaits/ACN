namespace RdmSnoop
{
    partial class SnoopMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SnoopMain));
            this.tools = new System.Windows.Forms.ToolStrip();
            this.rdmNetSelect = new System.Windows.Forms.ToolStripButton();
            this.artNetSelect = new System.Windows.Forms.ToolStripButton();
            this.routerSelect = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.networkCardSelect = new System.Windows.Forms.ToolStripComboBox();
            this.discoverSelect = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.rdmDevices = new System.Windows.Forms.TreeView();
            this.deviceInformation = new System.Windows.Forms.PropertyGrid();
            this.deviceToolbox = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.identifyOn = new System.Windows.Forms.ToolStripMenuItem();
            this.identifyOff = new System.Windows.Forms.ToolStripMenuItem();
            this.addressTool = new System.Windows.Forms.ToolStripButton();
            this.modeTool = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.resetTool = new System.Windows.Forms.ToolStripMenuItem();
            this.selfTestTool = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.powerOffTool = new System.Windows.Forms.ToolStripMenuItem();
            this.shutdownTool = new System.Windows.Forms.ToolStripMenuItem();
            this.powerStandbyTool = new System.Windows.Forms.ToolStripMenuItem();
            this.powerOnTool = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.packetView = new System.Windows.Forms.ListView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.packetsSentLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.packetsRecievedLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.droppedLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.transactionsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.failedLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tools.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.deviceToolbox.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tools
            // 
            this.tools.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rdmNetSelect,
            this.artNetSelect,
            this.routerSelect,
            this.toolStripSeparator1,
            this.networkCardSelect,
            this.discoverSelect,
            this.toolStripButton1});
            this.tools.Location = new System.Drawing.Point(0, 0);
            this.tools.Name = "tools";
            this.tools.Size = new System.Drawing.Size(980, 38);
            this.tools.TabIndex = 0;
            this.tools.Text = "tools";
            // 
            // rdmNetSelect
            // 
            this.rdmNetSelect.Checked = true;
            this.rdmNetSelect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rdmNetSelect.Image = global::RdmSnoop.Properties.Resources.OrgChartHS;
            this.rdmNetSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rdmNetSelect.Name = "rdmNetSelect";
            this.rdmNetSelect.Size = new System.Drawing.Size(56, 35);
            this.rdmNetSelect.Text = "RDMNet";
            this.rdmNetSelect.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.rdmNetSelect.Click += new System.EventHandler(this.rdmNetSelect_Click);
            // 
            // artNetSelect
            // 
            this.artNetSelect.Image = global::RdmSnoop.Properties.Resources.ArtNet;
            this.artNetSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.artNetSelect.Name = "artNetSelect";
            this.artNetSelect.Size = new System.Drawing.Size(46, 35);
            this.artNetSelect.Text = "ArtNet";
            this.artNetSelect.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.artNetSelect.Click += new System.EventHandler(this.artNetSelect_Click);
            // 
            // routerSelect
            // 
            this.routerSelect.Image = global::RdmSnoop.Properties.Resources.ArtNet;
            this.routerSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.routerSelect.Name = "routerSelect";
            this.routerSelect.Size = new System.Drawing.Size(46, 35);
            this.routerSelect.Text = "Router";
            this.routerSelect.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.routerSelect.Click += new System.EventHandler(this.routerSelect_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // networkCardSelect
            // 
            this.networkCardSelect.DropDownWidth = 600;
            this.networkCardSelect.Name = "networkCardSelect";
            this.networkCardSelect.Size = new System.Drawing.Size(250, 38);
            this.networkCardSelect.SelectedIndexChanged += new System.EventHandler(this.networkCardSelect_SelectedIndexChanged);
            // 
            // discoverSelect
            // 
            this.discoverSelect.Image = global::RdmSnoop.Properties.Resources.RepeatHS;
            this.discoverSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.discoverSelect.Name = "discoverSelect";
            this.discoverSelect.Size = new System.Drawing.Size(50, 35);
            this.discoverSelect.Text = "Refresh";
            this.discoverSelect.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.discoverSelect.Click += new System.EventHandler(this.discoverSelect_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 35);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.rdmDevices);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.deviceInformation);
            this.splitContainer1.Panel2.Controls.Add(this.deviceToolbox);
            this.splitContainer1.Size = new System.Drawing.Size(980, 320);
            this.splitContainer1.SplitterDistance = 325;
            this.splitContainer1.TabIndex = 1;
            // 
            // rdmDevices
            // 
            this.rdmDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdmDevices.Location = new System.Drawing.Point(0, 0);
            this.rdmDevices.Name = "rdmDevices";
            this.rdmDevices.Size = new System.Drawing.Size(325, 320);
            this.rdmDevices.TabIndex = 0;
            this.rdmDevices.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.rdmDevices_AfterSelect);
            // 
            // deviceInformation
            // 
            this.deviceInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceInformation.Location = new System.Drawing.Point(0, 38);
            this.deviceInformation.Name = "deviceInformation";
            this.deviceInformation.Size = new System.Drawing.Size(651, 282);
            this.deviceInformation.TabIndex = 0;
            // 
            // deviceToolbox
            // 
            this.deviceToolbox.Enabled = false;
            this.deviceToolbox.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.deviceToolbox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.addressTool,
            this.modeTool,
            this.toolStripDropDownButton2,
            this.toolStripDropDownButton3});
            this.deviceToolbox.Location = new System.Drawing.Point(0, 0);
            this.deviceToolbox.Name = "deviceToolbox";
            this.deviceToolbox.Size = new System.Drawing.Size(651, 38);
            this.deviceToolbox.TabIndex = 1;
            this.deviceToolbox.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.identifyOn,
            this.identifyOff});
            this.toolStripDropDownButton1.Image = global::RdmSnoop.Properties.Resources._008_Reminder_32x42_72;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(60, 35);
            this.toolStripDropDownButton1.Text = "Identify";
            this.toolStripDropDownButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // identifyOn
            // 
            this.identifyOn.Name = "identifyOn";
            this.identifyOn.Size = new System.Drawing.Size(91, 22);
            this.identifyOn.Text = "On";
            this.identifyOn.Click += new System.EventHandler(this.identifyOn_Click);
            // 
            // identifyOff
            // 
            this.identifyOff.Name = "identifyOff";
            this.identifyOff.Size = new System.Drawing.Size(91, 22);
            this.identifyOff.Text = "Off";
            this.identifyOff.Click += new System.EventHandler(this.identifyOff_Click);
            // 
            // addressTool
            // 
            this.addressTool.Image = global::RdmSnoop.Properties.Resources._1446_envelope_stamp_clsd_48;
            this.addressTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addressTool.Name = "addressTool";
            this.addressTool.Size = new System.Drawing.Size(53, 35);
            this.addressTool.Text = "Address";
            this.addressTool.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.addressTool.Click += new System.EventHandler(this.addressTool_Click);
            // 
            // modeTool
            // 
            this.modeTool.Image = global::RdmSnoop.Properties.Resources.settings_16;
            this.modeTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.modeTool.Name = "modeTool";
            this.modeTool.Size = new System.Drawing.Size(51, 35);
            this.modeTool.Text = "Mode";
            this.modeTool.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.modeTool.DropDownOpened += new System.EventHandler(this.modeTool_DropDownOpened);
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetTool,
            this.selfTestTool});
            this.toolStripDropDownButton2.Image = global::RdmSnoop.Properties.Resources.Gear;
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(58, 35);
            this.toolStripDropDownButton2.Text = "System";
            this.toolStripDropDownButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // resetTool
            // 
            this.resetTool.Name = "resetTool";
            this.resetTool.Size = new System.Drawing.Size(118, 22);
            this.resetTool.Text = "Reset";
            this.resetTool.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // selfTestTool
            // 
            this.selfTestTool.Name = "selfTestTool";
            this.selfTestTool.Size = new System.Drawing.Size(118, 22);
            this.selfTestTool.Text = "Self Test";
            this.selfTestTool.Click += new System.EventHandler(this.selfTestTool_Click);
            // 
            // toolStripDropDownButton3
            // 
            this.toolStripDropDownButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.powerOffTool,
            this.shutdownTool,
            this.powerStandbyTool,
            this.powerOnTool});
            this.toolStripDropDownButton3.Image = global::RdmSnoop.Properties.Resources.Red_Power_Button_clip_art_small;
            this.toolStripDropDownButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton3.Name = "toolStripDropDownButton3";
            this.toolStripDropDownButton3.Size = new System.Drawing.Size(53, 35);
            this.toolStripDropDownButton3.Text = "Power";
            this.toolStripDropDownButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // powerOffTool
            // 
            this.powerOffTool.Name = "powerOffTool";
            this.powerOffTool.Size = new System.Drawing.Size(128, 22);
            this.powerOffTool.Text = "Off";
            this.powerOffTool.Click += new System.EventHandler(this.powerOffTool_Click);
            // 
            // shutdownTool
            // 
            this.shutdownTool.Name = "shutdownTool";
            this.shutdownTool.Size = new System.Drawing.Size(128, 22);
            this.shutdownTool.Text = "Shutdown";
            this.shutdownTool.Click += new System.EventHandler(this.shutdownTool_Click);
            // 
            // powerStandbyTool
            // 
            this.powerStandbyTool.Name = "powerStandbyTool";
            this.powerStandbyTool.Size = new System.Drawing.Size(128, 22);
            this.powerStandbyTool.Text = "Standby";
            this.powerStandbyTool.Click += new System.EventHandler(this.powerStandbyTool_Click);
            // 
            // powerOnTool
            // 
            this.powerOnTool.Name = "powerOnTool";
            this.powerOnTool.Size = new System.Drawing.Size(128, 22);
            this.powerOnTool.Text = "On";
            this.powerOnTool.Click += new System.EventHandler(this.powerOnTool_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 38);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.packetView);
            this.splitContainer2.Size = new System.Drawing.Size(980, 480);
            this.splitContainer2.SplitterDistance = 320;
            this.splitContainer2.TabIndex = 2;
            // 
            // packetView
            // 
            this.packetView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.packetView.Location = new System.Drawing.Point(0, 0);
            this.packetView.MultiSelect = false;
            this.packetView.Name = "packetView";
            this.packetView.Size = new System.Drawing.Size(980, 156);
            this.packetView.TabIndex = 0;
            this.packetView.UseCompatibleStateImageBehavior = false;
            this.packetView.View = System.Windows.Forms.View.Details;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel2,
            this.packetsSentLabel,
            this.packetsRecievedLabel,
            this.toolStripStatusLabel1,
            this.droppedLabel,
            this.toolStripStatusLabel3,
            this.transactionsLabel,
            this.failedLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 518);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(980, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(53, 17);
            this.toolStripStatusLabel2.Text = "Packets:";
            // 
            // packetsSentLabel
            // 
            this.packetsSentLabel.Name = "packetsSentLabel";
            this.packetsSentLabel.Size = new System.Drawing.Size(42, 17);
            this.packetsSentLabel.Text = "Sent: 0";
            // 
            // packetsRecievedLabel
            // 
            this.packetsRecievedLabel.Name = "packetsRecievedLabel";
            this.packetsRecievedLabel.Size = new System.Drawing.Size(66, 17);
            this.packetsRecievedLabel.Text = "Recieved: 0";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // droppedLabel
            // 
            this.droppedLabel.Name = "droppedLabel";
            this.droppedLabel.Size = new System.Drawing.Size(65, 17);
            this.droppedLabel.Text = "Dropped: 0";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(79, 17);
            this.toolStripStatusLabel3.Text = "Transactions:";
            // 
            // transactionsLabel
            // 
            this.transactionsLabel.Name = "transactionsLabel";
            this.transactionsLabel.Size = new System.Drawing.Size(56, 17);
            this.transactionsLabel.Text = "Started: 0";
            // 
            // failedLabel
            // 
            this.failedLabel.Name = "failedLabel";
            this.failedLabel.Size = new System.Drawing.Size(50, 17);
            this.failedLabel.Text = "Failed: 0";
            // 
            // SnoopMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 540);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.tools);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SnoopMain";
            this.Text = "RDM Snoop";
            this.Load += new System.EventHandler(this.SnoopMain_Load);
            this.tools.ResumeLayout(false);
            this.tools.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.deviceToolbox.ResumeLayout(false);
            this.deviceToolbox.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tools;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView rdmDevices;
        private System.Windows.Forms.PropertyGrid deviceInformation;
        private System.Windows.Forms.ToolStripButton rdmNetSelect;
        private System.Windows.Forms.ToolStripButton artNetSelect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripComboBox networkCardSelect;
        private System.Windows.Forms.ToolStripButton discoverSelect;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListView packetView;
        private System.Windows.Forms.ToolStrip deviceToolbox;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem identifyOn;
        private System.Windows.Forms.ToolStripMenuItem identifyOff;
        private System.Windows.Forms.ToolStripButton addressTool;
        private System.Windows.Forms.ToolStripDropDownButton modeTool;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem resetTool;
        private System.Windows.Forms.ToolStripMenuItem selfTestTool;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton3;
        private System.Windows.Forms.ToolStripMenuItem powerOffTool;
        private System.Windows.Forms.ToolStripMenuItem shutdownTool;
        private System.Windows.Forms.ToolStripMenuItem powerStandbyTool;
        private System.Windows.Forms.ToolStripMenuItem powerOnTool;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel packetsSentLabel;
        private System.Windows.Forms.ToolStripStatusLabel packetsRecievedLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel droppedLabel;
        private System.Windows.Forms.ToolStripStatusLabel failedLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel transactionsLabel;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton routerSelect;

    }
}


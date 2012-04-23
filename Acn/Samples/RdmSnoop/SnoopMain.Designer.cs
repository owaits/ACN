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
            this.tools = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.networkCardSelect = new System.Windows.Forms.ToolStripComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.rdmDevices = new System.Windows.Forms.TreeView();
            this.deviceInformation = new System.Windows.Forms.PropertyGrid();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.packetView = new System.Windows.Forms.ListView();
            this.rdmNetSelect = new System.Windows.Forms.ToolStripButton();
            this.artNetSelect = new System.Windows.Forms.ToolStripButton();
            this.discoverSelect = new System.Windows.Forms.ToolStripButton();
            this.tools.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tools
            // 
            this.tools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rdmNetSelect,
            this.artNetSelect,
            this.toolStripSeparator1,
            this.networkCardSelect,
            this.discoverSelect});
            this.tools.Location = new System.Drawing.Point(0, 0);
            this.tools.Name = "tools";
            this.tools.Size = new System.Drawing.Size(980, 38);
            this.tools.TabIndex = 0;
            this.tools.Text = "tools";
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
            this.splitContainer1.Size = new System.Drawing.Size(980, 335);
            this.splitContainer1.SplitterDistance = 325;
            this.splitContainer1.TabIndex = 1;
            // 
            // rdmDevices
            // 
            this.rdmDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdmDevices.Location = new System.Drawing.Point(0, 0);
            this.rdmDevices.Name = "rdmDevices";
            this.rdmDevices.Size = new System.Drawing.Size(325, 335);
            this.rdmDevices.TabIndex = 0;
            this.rdmDevices.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.rdmDevices_AfterSelect);
            // 
            // deviceInformation
            // 
            this.deviceInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceInformation.Location = new System.Drawing.Point(0, 0);
            this.deviceInformation.Name = "deviceInformation";
            this.deviceInformation.Size = new System.Drawing.Size(651, 335);
            this.deviceInformation.TabIndex = 0;
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
            this.splitContainer2.Size = new System.Drawing.Size(980, 502);
            this.splitContainer2.SplitterDistance = 335;
            this.splitContainer2.TabIndex = 2;
            // 
            // packetView
            // 
            this.packetView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.packetView.Location = new System.Drawing.Point(0, 0);
            this.packetView.Name = "packetView";
            this.packetView.Size = new System.Drawing.Size(980, 163);
            this.packetView.TabIndex = 0;
            this.packetView.UseCompatibleStateImageBehavior = false;
            this.packetView.View = System.Windows.Forms.View.Details;
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
            // SnoopMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 540);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.tools);
            this.Name = "SnoopMain";
            this.Text = "RDM Snoop";
            this.Load += new System.EventHandler(this.SnoopMain_Load);
            this.tools.ResumeLayout(false);
            this.tools.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
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

    }
}


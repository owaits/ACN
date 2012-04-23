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
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.networkCardSelect = new System.Windows.Forms.ToolStripComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.rdmDevices = new System.Windows.Forms.TreeView();
            this.deviceInformation = new System.Windows.Forms.PropertyGrid();
            this.discoverSelect = new System.Windows.Forms.ToolStripButton();
            this.tools.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
            this.tools.Size = new System.Drawing.Size(734, 38);
            this.tools.TabIndex = 0;
            this.tools.Text = "tools";
            // 
            // rdmNetSelect
            // 
            this.rdmNetSelect.Checked = true;
            this.rdmNetSelect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rdmNetSelect.Image = ((System.Drawing.Image)(resources.GetObject("rdmNetSelect.Image")));
            this.rdmNetSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rdmNetSelect.Name = "rdmNetSelect";
            this.rdmNetSelect.Size = new System.Drawing.Size(56, 35);
            this.rdmNetSelect.Text = "RDMNet";
            this.rdmNetSelect.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.rdmNetSelect.Click += new System.EventHandler(this.rdmNetSelect_Click);
            // 
            // artNetSelect
            // 
            this.artNetSelect.Image = ((System.Drawing.Image)(resources.GetObject("artNetSelect.Image")));
            this.artNetSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.artNetSelect.Name = "artNetSelect";
            this.artNetSelect.Size = new System.Drawing.Size(46, 35);
            this.artNetSelect.Text = "ArtNet";
            this.artNetSelect.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.artNetSelect.Click += new System.EventHandler(this.artNetSelect_Click);
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
            this.splitContainer1.Location = new System.Drawing.Point(0, 38);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.rdmDevices);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.deviceInformation);
            this.splitContainer1.Size = new System.Drawing.Size(734, 388);
            this.splitContainer1.SplitterDistance = 244;
            this.splitContainer1.TabIndex = 1;
            // 
            // rdmDevices
            // 
            this.rdmDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdmDevices.Location = new System.Drawing.Point(0, 0);
            this.rdmDevices.Name = "rdmDevices";
            this.rdmDevices.Size = new System.Drawing.Size(244, 388);
            this.rdmDevices.TabIndex = 0;
            this.rdmDevices.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.rdmDevices_AfterSelect);
            // 
            // deviceInformation
            // 
            this.deviceInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceInformation.Location = new System.Drawing.Point(0, 0);
            this.deviceInformation.Name = "deviceInformation";
            this.deviceInformation.Size = new System.Drawing.Size(486, 388);
            this.deviceInformation.TabIndex = 0;
            // 
            // discoverSelect
            // 
            this.discoverSelect.Image = ((System.Drawing.Image)(resources.GetObject("discoverSelect.Image")));
            this.discoverSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.discoverSelect.Name = "discoverSelect";
            this.discoverSelect.Size = new System.Drawing.Size(56, 35);
            this.discoverSelect.Text = "Discover";
            this.discoverSelect.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.discoverSelect.Click += new System.EventHandler(this.discoverSelect_Click);
            // 
            // SnoopMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 426);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.tools);
            this.Name = "SnoopMain";
            this.Text = "RDM Snoop";
            this.Load += new System.EventHandler(this.SnoopMain_Load);
            this.tools.ResumeLayout(false);
            this.tools.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
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

    }
}


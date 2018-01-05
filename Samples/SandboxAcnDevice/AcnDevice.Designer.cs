namespace SandboxAcnDevice
{
    partial class AcnDevice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AcnDevice));
            this.deviceSettingsGroup = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.serviceTypeText = new System.Windows.Forms.TextBox();
            this.uidText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.startButton = new System.Windows.Forms.ToolStripButton();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.daList = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.discoveryTab = new System.Windows.Forms.TabPage();
            this.rdmTab = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdmTrace = new System.Windows.Forms.ListBox();
            this.deviceSettingsGroup.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.discoveryTab.SuspendLayout();
            this.rdmTab.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // deviceSettingsGroup
            // 
            this.deviceSettingsGroup.Controls.Add(this.tableLayoutPanel1);
            this.deviceSettingsGroup.Dock = System.Windows.Forms.DockStyle.Top;
            this.deviceSettingsGroup.Location = new System.Drawing.Point(3, 3);
            this.deviceSettingsGroup.Name = "deviceSettingsGroup";
            this.deviceSettingsGroup.Size = new System.Drawing.Size(610, 97);
            this.deviceSettingsGroup.TabIndex = 1;
            this.deviceSettingsGroup.TabStop = false;
            this.deviceSettingsGroup.Text = "Device Settings";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.35294F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67.64706F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.serviceTypeText, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.uidText, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(604, 75);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(189, 27);
            this.label2.TabIndex = 3;
            this.label2.Text = "UID:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // serviceTypeText
            // 
            this.serviceTypeText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serviceTypeText.Location = new System.Drawing.Point(198, 3);
            this.serviceTypeText.Name = "serviceTypeText";
            this.serviceTypeText.Size = new System.Drawing.Size(403, 20);
            this.serviceTypeText.TabIndex = 0;
            this.serviceTypeText.Text = "service:rdmnet-device";
            this.serviceTypeText.TextChanged += new System.EventHandler(this.serviceTypeText_TextChanged);
            // 
            // uidText
            // 
            this.uidText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uidText.Location = new System.Drawing.Point(198, 30);
            this.uidText.Name = "uidText";
            this.uidText.Size = new System.Drawing.Size(403, 20);
            this.uidText.TabIndex = 1;
            this.uidText.Text = "0xaabb11223344";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(189, 27);
            this.label1.TabIndex = 2;
            this.label1.Text = "Service Type:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startButton,
            this.stopButton});
            this.toolStrip1.Location = new System.Drawing.Point(5, 5);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(624, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // startButton
            // 
            this.startButton.Image = ((System.Drawing.Image)(resources.GetObject("startButton.Image")));
            this.startButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(51, 22);
            this.startButton.Text = "Start";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Image = ((System.Drawing.Image)(resources.GetObject("stopButton.Image")));
            this.stopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(51, 22);
            this.stopButton.Text = "Stop";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.daList);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(3, 100);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(610, 112);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Connected Discovery Agents";
            // 
            // daList
            // 
            this.daList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.daList.FormattingEnabled = true;
            this.daList.Location = new System.Drawing.Point(3, 16);
            this.daList.Name = "daList";
            this.daList.Size = new System.Drawing.Size(604, 93);
            this.daList.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.discoveryTab);
            this.tabControl1.Controls.Add(this.rdmTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(5, 30);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(624, 351);
            this.tabControl1.TabIndex = 4;
            // 
            // discoveryTab
            // 
            this.discoveryTab.Controls.Add(this.groupBox2);
            this.discoveryTab.Controls.Add(this.deviceSettingsGroup);
            this.discoveryTab.Location = new System.Drawing.Point(4, 22);
            this.discoveryTab.Name = "discoveryTab";
            this.discoveryTab.Padding = new System.Windows.Forms.Padding(3);
            this.discoveryTab.Size = new System.Drawing.Size(616, 325);
            this.discoveryTab.TabIndex = 0;
            this.discoveryTab.Text = "Discovery";
            this.discoveryTab.UseVisualStyleBackColor = true;
            // 
            // rdmTab
            // 
            this.rdmTab.Controls.Add(this.groupBox1);
            this.rdmTab.Location = new System.Drawing.Point(4, 22);
            this.rdmTab.Name = "rdmTab";
            this.rdmTab.Padding = new System.Windows.Forms.Padding(3);
            this.rdmTab.Size = new System.Drawing.Size(616, 325);
            this.rdmTab.TabIndex = 1;
            this.rdmTab.Text = "RDM";
            this.rdmTab.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdmTrace);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(3, 200);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(610, 122);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Trace";
            // 
            // rdmTrace
            // 
            this.rdmTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdmTrace.FormattingEnabled = true;
            this.rdmTrace.Location = new System.Drawing.Point(3, 16);
            this.rdmTrace.Name = "rdmTrace";
            this.rdmTrace.Size = new System.Drawing.Size(604, 103);
            this.rdmTrace.TabIndex = 0;
            // 
            // AcnDevice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 386);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AcnDevice";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "Acn Sandbox Device";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AcnDevice_FormClosing);
            this.deviceSettingsGroup.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.discoveryTab.ResumeLayout(false);
            this.rdmTab.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox deviceSettingsGroup;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton startButton;
        private System.Windows.Forms.ToolStripButton stopButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox daList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox serviceTypeText;
        private System.Windows.Forms.TextBox uidText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage discoveryTab;
        private System.Windows.Forms.TabPage rdmTab;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox rdmTrace;
    }
}


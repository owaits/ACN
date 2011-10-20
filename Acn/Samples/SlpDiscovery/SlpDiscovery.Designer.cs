namespace SlpDiscovery
{
    partial class SlpDiscovery
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.deviceList = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.urlText = new System.Windows.Forms.TextBox();
            this.scopeSelect = new System.Windows.Forms.ComboBox();
            this.find = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.devicesGrid = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.update = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.urlColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.attributeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastSeenColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.devicesGrid)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(5, 5);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.deviceList);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.devicesGrid);
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Size = new System.Drawing.Size(629, 417);
            this.splitContainer1.SplitterDistance = 184;
            this.splitContainer1.TabIndex = 0;
            // 
            // deviceList
            // 
            this.deviceList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceList.FormattingEnabled = true;
            this.deviceList.Location = new System.Drawing.Point(0, 26);
            this.deviceList.Name = "deviceList";
            this.deviceList.Size = new System.Drawing.Size(629, 158);
            this.deviceList.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.urlText);
            this.panel1.Controls.Add(this.scopeSelect);
            this.panel1.Controls.Add(this.find);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.panel1.Size = new System.Drawing.Size(629, 26);
            this.panel1.TabIndex = 5;
            // 
            // urlText
            // 
            this.urlText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.urlText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.urlText.Location = new System.Drawing.Point(0, 0);
            this.urlText.Name = "urlText";
            this.urlText.Size = new System.Drawing.Size(438, 21);
            this.urlText.TabIndex = 0;
            this.urlText.Text = "service:e133.esta";
            // 
            // scopeSelect
            // 
            this.scopeSelect.Dock = System.Windows.Forms.DockStyle.Right;
            this.scopeSelect.FormattingEnabled = true;
            this.scopeSelect.Items.AddRange(new object[] {
            "ACN-DEFAULT",
            "DEFAULT"});
            this.scopeSelect.Location = new System.Drawing.Point(438, 0);
            this.scopeSelect.Name = "scopeSelect";
            this.scopeSelect.Size = new System.Drawing.Size(138, 21);
            this.scopeSelect.TabIndex = 2;
            this.scopeSelect.Text = "ACN-DEFAULT";
            // 
            // find
            // 
            this.find.Dock = System.Windows.Forms.DockStyle.Right;
            this.find.Location = new System.Drawing.Point(576, 0);
            this.find.Margin = new System.Windows.Forms.Padding(0);
            this.find.Name = "find";
            this.find.Size = new System.Drawing.Size(53, 21);
            this.find.TabIndex = 1;
            this.find.Text = "Find";
            this.find.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer2.Size = new System.Drawing.Size(629, 184);
            this.splitContainer2.SplitterDistance = 64;
            this.splitContainer2.TabIndex = 7;
            // 
            // devicesGrid
            // 
            this.devicesGrid.AllowUserToAddRows = false;
            this.devicesGrid.AllowUserToDeleteRows = false;
            this.devicesGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.devicesGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.urlColumn,
            this.attributeColumn,
            this.lastSeenColumn,
            this.StateColumn});
            this.devicesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.devicesGrid.Location = new System.Drawing.Point(0, 26);
            this.devicesGrid.MultiSelect = false;
            this.devicesGrid.Name = "devicesGrid";
            this.devicesGrid.ReadOnly = true;
            this.devicesGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.devicesGrid.RowHeadersVisible = false;
            this.devicesGrid.Size = new System.Drawing.Size(629, 203);
            this.devicesGrid.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.update);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(629, 26);
            this.panel2.TabIndex = 0;
            // 
            // update
            // 
            this.update.Location = new System.Drawing.Point(81, 0);
            this.update.Name = "update";
            this.update.Size = new System.Drawing.Size(75, 27);
            this.update.TabIndex = 1;
            this.update.Text = "Update Now";
            this.update.UseVisualStyleBackColor = true;
            this.update.Click += new System.EventHandler(this.update_Click);
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Left;
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(74, 26);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start / Stop";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // urlColumn
            // 
            this.urlColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.urlColumn.HeaderText = "URL";
            this.urlColumn.Name = "urlColumn";
            this.urlColumn.ReadOnly = true;
            this.urlColumn.Width = 54;
            // 
            // attributeColumn
            // 
            this.attributeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.attributeColumn.HeaderText = "Attribtues";
            this.attributeColumn.Name = "attributeColumn";
            this.attributeColumn.ReadOnly = true;
            // 
            // lastSeenColumn
            // 
            this.lastSeenColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.lastSeenColumn.HeaderText = "Last Seen";
            this.lastSeenColumn.Name = "lastSeenColumn";
            this.lastSeenColumn.ReadOnly = true;
            this.lastSeenColumn.Width = 80;
            // 
            // StateColumn
            // 
            this.StateColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.StateColumn.HeaderText = "State";
            this.StateColumn.Name = "StateColumn";
            this.StateColumn.ReadOnly = true;
            this.StateColumn.Width = 57;
            // 
            // SlpDiscovery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(639, 427);
            this.Controls.Add(this.splitContainer1);
            this.Name = "SlpDiscovery";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "SLP Discovery";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SlpDiscovery_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.devicesGrid)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox deviceList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox urlText;
        private System.Windows.Forms.ComboBox scopeSelect;
        private System.Windows.Forms.Button find;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView devicesGrid;
        private System.Windows.Forms.Button update;
        private System.Windows.Forms.DataGridViewTextBoxColumn urlColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn attributeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastSeenColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn StateColumn;

    }
}


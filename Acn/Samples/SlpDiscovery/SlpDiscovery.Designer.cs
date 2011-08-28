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
            this.urlText = new System.Windows.Forms.TextBox();
            this.find = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.scopeSelect = new System.Windows.Forms.ComboBox();
            this.deviceList = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // urlText
            // 
            this.urlText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.urlText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.urlText.Location = new System.Drawing.Point(0, 0);
            this.urlText.Name = "urlText";
            this.urlText.Size = new System.Drawing.Size(221, 21);
            this.urlText.TabIndex = 0;
            this.urlText.Text = "service:acn.esta";
            // 
            // find
            // 
            this.find.Dock = System.Windows.Forms.DockStyle.Right;
            this.find.Location = new System.Drawing.Point(359, 0);
            this.find.Margin = new System.Windows.Forms.Padding(0);
            this.find.Name = "find";
            this.find.Size = new System.Drawing.Size(53, 21);
            this.find.TabIndex = 1;
            this.find.Text = "Find";
            this.find.UseVisualStyleBackColor = true;
            this.find.Click += new System.EventHandler(this.find_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.urlText);
            this.panel1.Controls.Add(this.scopeSelect);
            this.panel1.Controls.Add(this.find);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.panel1.Size = new System.Drawing.Size(412, 26);
            this.panel1.TabIndex = 2;
            // 
            // scopeSelect
            // 
            this.scopeSelect.Dock = System.Windows.Forms.DockStyle.Right;
            this.scopeSelect.FormattingEnabled = true;
            this.scopeSelect.Items.AddRange(new object[] {
            "ACN-DEFAULT",
            "DEFAULT"});
            this.scopeSelect.Location = new System.Drawing.Point(221, 0);
            this.scopeSelect.Name = "scopeSelect";
            this.scopeSelect.Size = new System.Drawing.Size(138, 21);
            this.scopeSelect.TabIndex = 2;
            this.scopeSelect.Text = "ACN-DEFAULT";
            // 
            // deviceList
            // 
            this.deviceList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceList.FormattingEnabled = true;
            this.deviceList.Location = new System.Drawing.Point(5, 31);
            this.deviceList.Name = "deviceList";
            this.deviceList.Size = new System.Drawing.Size(412, 277);
            this.deviceList.TabIndex = 3;
            // 
            // SlpDiscovery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 318);
            this.Controls.Add(this.deviceList);
            this.Controls.Add(this.panel1);
            this.Name = "SlpDiscovery";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "SLP Discovery";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SlpDiscovery_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox urlText;
        private System.Windows.Forms.Button find;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox deviceList;
        private System.Windows.Forms.ComboBox scopeSelect;
    }
}


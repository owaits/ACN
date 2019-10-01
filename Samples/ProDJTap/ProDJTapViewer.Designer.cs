namespace LXProtocols.TCNetViewer
{
    partial class ProDJTapViewer
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.networkCardSelect = new System.Windows.Forms.ToolStripComboBox();
            this.deviceSelect = new System.Windows.Forms.ToolStripComboBox();
            this.tcInfo = new System.Windows.Forms.PropertyGrid();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.layersPage = new System.Windows.Forms.TabPage();
            this.controlPage = new System.Windows.Forms.TabPage();
            this.stopButton = new System.Windows.Forms.Button();
            this.layerSelect = new System.Windows.Forms.ComboBox();
            this.pauseButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.downloadPage = new System.Windows.Forms.TabPage();
            this.downloadBigWaveform = new System.Windows.Forms.Button();
            this.downloadLayerSelect = new System.Windows.Forms.ComboBox();
            this.downloadSmallWaveform = new System.Windows.Forms.Button();
            this.downloadMetaData = new System.Windows.Forms.Button();
            this.metricsDownload = new System.Windows.Forms.Button();
            this.beatGrid = new System.Windows.Forms.Button();
            this.cueData = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.layersPage.SuspendLayout();
            this.controlPage.SuspendLayout();
            this.downloadPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.networkCardSelect,
            this.deviceSelect});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(651, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // networkCardSelect
            // 
            this.networkCardSelect.DropDownWidth = 600;
            this.networkCardSelect.Name = "networkCardSelect";
            this.networkCardSelect.Size = new System.Drawing.Size(250, 25);
            this.networkCardSelect.SelectedIndexChanged += new System.EventHandler(this.networkCardSelect_SelectedIndexChanged);
            // 
            // deviceSelect
            // 
            this.deviceSelect.Name = "deviceSelect";
            this.deviceSelect.Size = new System.Drawing.Size(200, 25);
            // 
            // tcInfo
            // 
            this.tcInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcInfo.HelpVisible = false;
            this.tcInfo.Location = new System.Drawing.Point(3, 3);
            this.tcInfo.Name = "tcInfo";
            this.tcInfo.Size = new System.Drawing.Size(637, 259);
            this.tcInfo.TabIndex = 3;
            this.tcInfo.ToolbarVisible = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.layersPage);
            this.tabControl1.Controls.Add(this.controlPage);
            this.tabControl1.Controls.Add(this.downloadPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(651, 291);
            this.tabControl1.TabIndex = 4;
            // 
            // layersPage
            // 
            this.layersPage.Controls.Add(this.tcInfo);
            this.layersPage.Location = new System.Drawing.Point(4, 22);
            this.layersPage.Name = "layersPage";
            this.layersPage.Padding = new System.Windows.Forms.Padding(3);
            this.layersPage.Size = new System.Drawing.Size(643, 265);
            this.layersPage.TabIndex = 0;
            this.layersPage.Text = "Layers";
            this.layersPage.UseVisualStyleBackColor = true;
            // 
            // controlPage
            // 
            this.controlPage.Controls.Add(this.stopButton);
            this.controlPage.Controls.Add(this.layerSelect);
            this.controlPage.Controls.Add(this.pauseButton);
            this.controlPage.Controls.Add(this.playButton);
            this.controlPage.Location = new System.Drawing.Point(4, 22);
            this.controlPage.Name = "controlPage";
            this.controlPage.Padding = new System.Windows.Forms.Padding(3);
            this.controlPage.Size = new System.Drawing.Size(643, 265);
            this.controlPage.TabIndex = 1;
            this.controlPage.Text = "Controls";
            this.controlPage.UseVisualStyleBackColor = true;
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(170, 33);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 11;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // layerSelect
            // 
            this.layerSelect.FormattingEnabled = true;
            this.layerSelect.Items.AddRange(new object[] {
            "All",
            "Layer 1",
            "Layer 2",
            "Layer 3",
            "Layer 4",
            "Layer A",
            "Layer B",
            "Layer M",
            "Layer C"});
            this.layerSelect.Location = new System.Drawing.Point(8, 6);
            this.layerSelect.Name = "layerSelect";
            this.layerSelect.Size = new System.Drawing.Size(311, 21);
            this.layerSelect.TabIndex = 10;
            // 
            // pauseButton
            // 
            this.pauseButton.Location = new System.Drawing.Point(89, 33);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(75, 23);
            this.pauseButton.TabIndex = 9;
            this.pauseButton.Text = "Pause";
            this.pauseButton.UseVisualStyleBackColor = true;
            this.pauseButton.Click += new System.EventHandler(this.PauseButton_Click);
            // 
            // playButton
            // 
            this.playButton.Location = new System.Drawing.Point(8, 33);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(75, 23);
            this.playButton.TabIndex = 8;
            this.playButton.Text = "Play";
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.PlayButton_Click);
            // 
            // downloadPage
            // 
            this.downloadPage.Controls.Add(this.cueData);
            this.downloadPage.Controls.Add(this.beatGrid);
            this.downloadPage.Controls.Add(this.downloadBigWaveform);
            this.downloadPage.Controls.Add(this.downloadLayerSelect);
            this.downloadPage.Controls.Add(this.downloadSmallWaveform);
            this.downloadPage.Controls.Add(this.downloadMetaData);
            this.downloadPage.Controls.Add(this.metricsDownload);
            this.downloadPage.Location = new System.Drawing.Point(4, 22);
            this.downloadPage.Name = "downloadPage";
            this.downloadPage.Size = new System.Drawing.Size(643, 265);
            this.downloadPage.TabIndex = 2;
            this.downloadPage.Text = "Download";
            this.downloadPage.UseVisualStyleBackColor = true;
            // 
            // downloadBigWaveform
            // 
            this.downloadBigWaveform.Location = new System.Drawing.Point(362, 29);
            this.downloadBigWaveform.Name = "downloadBigWaveform";
            this.downloadBigWaveform.Size = new System.Drawing.Size(106, 23);
            this.downloadBigWaveform.TabIndex = 16;
            this.downloadBigWaveform.Text = "Big Waveform";
            this.downloadBigWaveform.UseVisualStyleBackColor = true;
            this.downloadBigWaveform.Click += new System.EventHandler(this.DownloadBigWaveform_Click);
            // 
            // downloadLayerSelect
            // 
            this.downloadLayerSelect.Dock = System.Windows.Forms.DockStyle.Top;
            this.downloadLayerSelect.FormattingEnabled = true;
            this.downloadLayerSelect.Items.AddRange(new object[] {
            "All",
            "Layer 1",
            "Layer 2",
            "Layer 3",
            "Layer 4",
            "Layer A",
            "Layer B",
            "Layer M",
            "Layer C"});
            this.downloadLayerSelect.Location = new System.Drawing.Point(0, 0);
            this.downloadLayerSelect.Name = "downloadLayerSelect";
            this.downloadLayerSelect.Size = new System.Drawing.Size(643, 21);
            this.downloadLayerSelect.TabIndex = 15;
            // 
            // downloadSmallWaveform
            // 
            this.downloadSmallWaveform.Location = new System.Drawing.Point(250, 29);
            this.downloadSmallWaveform.Name = "downloadSmallWaveform";
            this.downloadSmallWaveform.Size = new System.Drawing.Size(106, 23);
            this.downloadSmallWaveform.TabIndex = 14;
            this.downloadSmallWaveform.Text = "Small Waveform";
            this.downloadSmallWaveform.UseVisualStyleBackColor = true;
            this.downloadSmallWaveform.Click += new System.EventHandler(this.DownloadSmallWaveform_Click);
            // 
            // downloadMetaData
            // 
            this.downloadMetaData.Location = new System.Drawing.Point(88, 29);
            this.downloadMetaData.Name = "downloadMetaData";
            this.downloadMetaData.Size = new System.Drawing.Size(75, 23);
            this.downloadMetaData.TabIndex = 13;
            this.downloadMetaData.Text = "Meta Data";
            this.downloadMetaData.UseVisualStyleBackColor = true;
            this.downloadMetaData.Click += new System.EventHandler(this.DownloadMetaData_Click);
            // 
            // metricsDownload
            // 
            this.metricsDownload.Location = new System.Drawing.Point(7, 29);
            this.metricsDownload.Name = "metricsDownload";
            this.metricsDownload.Size = new System.Drawing.Size(75, 23);
            this.metricsDownload.TabIndex = 12;
            this.metricsDownload.Text = "Metrics";
            this.metricsDownload.UseVisualStyleBackColor = true;
            this.metricsDownload.Click += new System.EventHandler(this.MetricsDownload_Click);
            // 
            // beatGrid
            // 
            this.beatGrid.Location = new System.Drawing.Point(169, 29);
            this.beatGrid.Name = "beatGrid";
            this.beatGrid.Size = new System.Drawing.Size(75, 23);
            this.beatGrid.TabIndex = 17;
            this.beatGrid.Text = "Beat Grid";
            this.beatGrid.UseVisualStyleBackColor = true;
            this.beatGrid.Click += new System.EventHandler(this.BeatGrid_Click);
            // 
            // cueData
            // 
            this.cueData.Location = new System.Drawing.Point(474, 29);
            this.cueData.Name = "cueData";
            this.cueData.Size = new System.Drawing.Size(106, 23);
            this.cueData.TabIndex = 18;
            this.cueData.Text = "Cue Data";
            this.cueData.UseVisualStyleBackColor = true;
            this.cueData.Click += new System.EventHandler(this.CueData_Click);
            // 
            // ProDJTapViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(651, 316);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ProDJTapViewer";
            this.Text = "Pro DJ Tap";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.layersPage.ResumeLayout(false);
            this.controlPage.ResumeLayout(false);
            this.downloadPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripComboBox networkCardSelect;
        private System.Windows.Forms.PropertyGrid tcInfo;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage layersPage;
        private System.Windows.Forms.TabPage controlPage;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.ComboBox layerSelect;
        private System.Windows.Forms.Button pauseButton;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.TabPage downloadPage;
        private System.Windows.Forms.Button downloadSmallWaveform;
        private System.Windows.Forms.Button downloadMetaData;
        private System.Windows.Forms.Button metricsDownload;
        private System.Windows.Forms.ComboBox downloadLayerSelect;
        private System.Windows.Forms.Button downloadBigWaveform;
        private System.Windows.Forms.ToolStripComboBox deviceSelect;
        private System.Windows.Forms.Button beatGrid;
        private System.Windows.Forms.Button cueData;
    }
}


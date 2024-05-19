namespace Biological_Signal_Processing_Using_AI.DetailsModify.FiltersControls
{
    partial class SegmentDistributionUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.autoApplyCheckBox = new System.Windows.Forms.CheckBox();
            this.highlightedSegmentDistributionLabel = new System.Windows.Forms.Label();
            this.applyButton = new System.Windows.Forms.Button();
            this.distributionSignalChart = new ScottPlot.FormsPlot();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sendSignalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analyseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.endingIndexTextBox = new System.Windows.Forms.TextBox();
            this.endingIndexLabel = new System.Windows.Forms.Label();
            this.startingIndexTextBox = new System.Windows.Forms.TextBox();
            this.startingIndexLabel = new System.Windows.Forms.Label();
            this.resolutionTextBox = new System.Windows.Forms.TextBox();
            this.resolutionLabel = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // autoApplyCheckBox
            // 
            this.autoApplyCheckBox.AutoSize = true;
            this.autoApplyCheckBox.Checked = true;
            this.autoApplyCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoApplyCheckBox.Location = new System.Drawing.Point(2, 21);
            this.autoApplyCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.autoApplyCheckBox.Name = "autoApplyCheckBox";
            this.autoApplyCheckBox.Size = new System.Drawing.Size(84, 19);
            this.autoApplyCheckBox.TabIndex = 24;
            this.autoApplyCheckBox.Text = "Auto apply";
            this.autoApplyCheckBox.UseVisualStyleBackColor = true;
            // 
            // highlightedSegmentDistributionLabel
            // 
            this.highlightedSegmentDistributionLabel.AutoSize = true;
            this.highlightedSegmentDistributionLabel.Location = new System.Drawing.Point(2, 2);
            this.highlightedSegmentDistributionLabel.Margin = new System.Windows.Forms.Padding(2);
            this.highlightedSegmentDistributionLabel.Name = "highlightedSegmentDistributionLabel";
            this.highlightedSegmentDistributionLabel.Size = new System.Drawing.Size(186, 15);
            this.highlightedSegmentDistributionLabel.TabIndex = 23;
            this.highlightedSegmentDistributionLabel.Text = "Highllighted segment distribution";
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.applyButton.Location = new System.Drawing.Point(264, 303);
            this.applyButton.Margin = new System.Windows.Forms.Padding(2);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(65, 23);
            this.applyButton.TabIndex = 25;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // distributionSignalChart
            // 
            this.distributionSignalChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.distributionSignalChart.ContextMenuStrip = this.contextMenuStrip2;
            this.distributionSignalChart.Location = new System.Drawing.Point(0, 44);
            this.distributionSignalChart.Margin = new System.Windows.Forms.Padding(2);
            this.distributionSignalChart.Name = "distributionSignalChart";
            this.distributionSignalChart.Size = new System.Drawing.Size(331, 211);
            this.distributionSignalChart.TabIndex = 27;
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(108, 26);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendSignalToolStripMenuItem,
            this.analyseToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip1";
            this.contextMenuStrip2.Size = new System.Drawing.Size(135, 48);
            // 
            // sendSignalToolStripMenuItem
            // 
            this.sendSignalToolStripMenuItem.Name = "sendSignalToolStripMenuItem";
            this.sendSignalToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.sendSignalToolStripMenuItem.Text = "Send signal";
            this.sendSignalToolStripMenuItem.Click += new System.EventHandler(this.sendSignalToolStripMenuItem_Click);
            // 
            // analyseToolStripMenuItem
            // 
            this.analyseToolStripMenuItem.Name = "analyseToolStripMenuItem";
            this.analyseToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.analyseToolStripMenuItem.Text = "Analyse";
            this.analyseToolStripMenuItem.Click += new System.EventHandler(this.analyseToolStripMenuItem_Click);
            // 
            // endingIndexTextBox
            // 
            this.endingIndexTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.endingIndexTextBox.Location = new System.Drawing.Point(256, 277);
            this.endingIndexTextBox.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.endingIndexTextBox.Name = "endingIndexTextBox";
            this.endingIndexTextBox.Size = new System.Drawing.Size(73, 23);
            this.endingIndexTextBox.TabIndex = 32;
            this.endingIndexTextBox.Text = "0";
            this.endingIndexTextBox.TextChanged += new System.EventHandler(this.endingIndexTextBox_TextChanged);
            this.endingIndexTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.startingIndexTextBox_KeyPress);
            // 
            // endingIndexLabel
            // 
            this.endingIndexLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.endingIndexLabel.AutoSize = true;
            this.endingIndexLabel.Location = new System.Drawing.Point(253, 259);
            this.endingIndexLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.endingIndexLabel.Name = "endingIndexLabel";
            this.endingIndexLabel.Size = new System.Drawing.Size(76, 15);
            this.endingIndexLabel.TabIndex = 31;
            this.endingIndexLabel.Text = "Ending index";
            // 
            // startingIndexTextBox
            // 
            this.startingIndexTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.startingIndexTextBox.Location = new System.Drawing.Point(2, 277);
            this.startingIndexTextBox.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.startingIndexTextBox.Name = "startingIndexTextBox";
            this.startingIndexTextBox.Size = new System.Drawing.Size(73, 23);
            this.startingIndexTextBox.TabIndex = 34;
            this.startingIndexTextBox.Text = "0";
            this.startingIndexTextBox.TextChanged += new System.EventHandler(this.startingIndexTextBox_TextChanged);
            this.startingIndexTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.startingIndexTextBox_KeyPress);
            // 
            // startingIndexLabel
            // 
            this.startingIndexLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.startingIndexLabel.AutoSize = true;
            this.startingIndexLabel.Location = new System.Drawing.Point(-1, 259);
            this.startingIndexLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.startingIndexLabel.Name = "startingIndexLabel";
            this.startingIndexLabel.Size = new System.Drawing.Size(80, 15);
            this.startingIndexLabel.TabIndex = 33;
            this.startingIndexLabel.Text = "Starting index";
            // 
            // resolutionTextBox
            // 
            this.resolutionTextBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.resolutionTextBox.Location = new System.Drawing.Point(133, 277);
            this.resolutionTextBox.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.resolutionTextBox.Name = "resolutionTextBox";
            this.resolutionTextBox.Size = new System.Drawing.Size(73, 23);
            this.resolutionTextBox.TabIndex = 36;
            this.resolutionTextBox.Text = "100";
            this.resolutionTextBox.TextChanged += new System.EventHandler(this.resolutionTextBox_TextChanged);
            this.resolutionTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.startingIndexTextBox_KeyPress);
            // 
            // resolutionLabel
            // 
            this.resolutionLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.resolutionLabel.AutoSize = true;
            this.resolutionLabel.Location = new System.Drawing.Point(130, 259);
            this.resolutionLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.resolutionLabel.Name = "resolutionLabel";
            this.resolutionLabel.Size = new System.Drawing.Size(63, 15);
            this.resolutionLabel.TabIndex = 35;
            this.resolutionLabel.Text = "Resolution";
            // 
            // SegmentDistributionUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.resolutionTextBox);
            this.Controls.Add(this.resolutionLabel);
            this.Controls.Add(this.startingIndexTextBox);
            this.Controls.Add(this.startingIndexLabel);
            this.Controls.Add(this.endingIndexTextBox);
            this.Controls.Add(this.endingIndexLabel);
            this.Controls.Add(this.distributionSignalChart);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.autoApplyCheckBox);
            this.Controls.Add(this.highlightedSegmentDistributionLabel);
            this.Name = "SegmentDistributionUserControl";
            this.Size = new System.Drawing.Size(331, 328);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox autoApplyCheckBox;
        public System.Windows.Forms.Label highlightedSegmentDistributionLabel;
        public System.Windows.Forms.Button applyButton;
        public ScottPlot.FormsPlot distributionSignalChart;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem sendSignalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem analyseToolStripMenuItem;
        public System.Windows.Forms.TextBox endingIndexTextBox;
        public System.Windows.Forms.Label endingIndexLabel;
        public System.Windows.Forms.TextBox startingIndexTextBox;
        public System.Windows.Forms.Label startingIndexLabel;
        public System.Windows.Forms.TextBox resolutionTextBox;
        public System.Windows.Forms.Label resolutionLabel;
    }
}


namespace BSP_Using_AI.DetailsModify.Filters
{
    partial class SignalStatesViewerUserControl
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
            this.hThresholdScrollBar = new System.Windows.Forms.HScrollBar();
            this.thresholdRatioLabel = new System.Windows.Forms.Label();
            this.signalStatesViewerLabel = new System.Windows.Forms.Label();
            this.autoApplyCheckBox = new System.Windows.Forms.CheckBox();
            this.showStatesCheckBox = new System.Windows.Forms.CheckBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.amplitudeThresholdScrollBar = new System.Windows.Forms.VScrollBar();
            this.hThresholdLabel = new System.Windows.Forms.Label();
            this.tdtThresholdLabel = new System.Windows.Forms.Label();
            this.tdtThresholdScrollBar = new System.Windows.Forms.HScrollBar();
            this.showDeviationCheckBox = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // hThresholdScrollBar
            // 
            this.hThresholdScrollBar.Location = new System.Drawing.Point(3, 161);
            this.hThresholdScrollBar.Margin = new System.Windows.Forms.Padding(2);
            this.hThresholdScrollBar.Maximum = 1009;
            this.hThresholdScrollBar.Name = "hThresholdScrollBar";
            this.hThresholdScrollBar.Size = new System.Drawing.Size(260, 21);
            this.hThresholdScrollBar.TabIndex = 20;
            this.hThresholdScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hThresholdScrollBar_Scroll);
            // 
            // thresholdRatioLabel
            // 
            this.thresholdRatioLabel.AutoSize = true;
            this.thresholdRatioLabel.Location = new System.Drawing.Point(151, 22);
            this.thresholdRatioLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.thresholdRatioLabel.Name = "thresholdRatioLabel";
            this.thresholdRatioLabel.Size = new System.Drawing.Size(80, 13);
            this.thresholdRatioLabel.TabIndex = 19;
            this.thresholdRatioLabel.Text = "Threshold ratio:";
            // 
            // signalStatesViewerLabel
            // 
            this.signalStatesViewerLabel.AutoSize = true;
            this.signalStatesViewerLabel.Location = new System.Drawing.Point(2, 2);
            this.signalStatesViewerLabel.Margin = new System.Windows.Forms.Padding(2);
            this.signalStatesViewerLabel.Name = "signalStatesViewerLabel";
            this.signalStatesViewerLabel.Size = new System.Drawing.Size(102, 13);
            this.signalStatesViewerLabel.TabIndex = 21;
            this.signalStatesViewerLabel.Text = "SIgnal states viewer";
            // 
            // autoApplyCheckBox
            // 
            this.autoApplyCheckBox.AutoSize = true;
            this.autoApplyCheckBox.Checked = true;
            this.autoApplyCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoApplyCheckBox.Location = new System.Drawing.Point(4, 43);
            this.autoApplyCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.autoApplyCheckBox.Name = "autoApplyCheckBox";
            this.autoApplyCheckBox.Size = new System.Drawing.Size(76, 17);
            this.autoApplyCheckBox.TabIndex = 22;
            this.autoApplyCheckBox.Text = "Auto apply";
            this.autoApplyCheckBox.UseVisualStyleBackColor = true;
            // 
            // showStatesCheckBox
            // 
            this.showStatesCheckBox.AutoSize = true;
            this.showStatesCheckBox.Checked = true;
            this.showStatesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showStatesCheckBox.Location = new System.Drawing.Point(4, 21);
            this.showStatesCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.showStatesCheckBox.Name = "showStatesCheckBox";
            this.showStatesCheckBox.Size = new System.Drawing.Size(84, 17);
            this.showStatesCheckBox.TabIndex = 23;
            this.showStatesCheckBox.Text = "Show states";
            this.showStatesCheckBox.UseVisualStyleBackColor = true;
            this.showStatesCheckBox.CheckedChanged += new System.EventHandler(this.showCheckBox_CheckedChanged);
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.applyButton.Location = new System.Drawing.Point(225, 183);
            this.applyButton.Margin = new System.Windows.Forms.Padding(2);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(56, 20);
            this.applyButton.TabIndex = 24;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(108, 26);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // amplitudeThresholdScrollBar
            // 
            this.amplitudeThresholdScrollBar.Location = new System.Drawing.Point(266, 0);
            this.amplitudeThresholdScrollBar.Maximum = 1009;
            this.amplitudeThresholdScrollBar.Name = "amplitudeThresholdScrollBar";
            this.amplitudeThresholdScrollBar.Size = new System.Drawing.Size(21, 180);
            this.amplitudeThresholdScrollBar.TabIndex = 25;
            this.amplitudeThresholdScrollBar.Value = 1000;
            this.amplitudeThresholdScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.amplitudeThresholdScrollBar_Scroll);
            // 
            // hThresholdLabel
            // 
            this.hThresholdLabel.AutoSize = true;
            this.hThresholdLabel.Location = new System.Drawing.Point(54, 183);
            this.hThresholdLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.hThresholdLabel.Name = "hThresholdLabel";
            this.hThresholdLabel.Size = new System.Drawing.Size(103, 13);
            this.hThresholdLabel.TabIndex = 26;
            this.hThresholdLabel.Text = "Hor Threshold: - sec";
            // 
            // tdtThresholdLabel
            // 
            this.tdtThresholdLabel.AutoSize = true;
            this.tdtThresholdLabel.Location = new System.Drawing.Point(80, 141);
            this.tdtThresholdLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.tdtThresholdLabel.Name = "tdtThresholdLabel";
            this.tdtThresholdLabel.Size = new System.Drawing.Size(105, 13);
            this.tdtThresholdLabel.TabIndex = 28;
            this.tdtThresholdLabel.Text = "TDT Threshold: 10%";
            // 
            // tdtThresholdScrollBar
            // 
            this.tdtThresholdScrollBar.Location = new System.Drawing.Point(3, 119);
            this.tdtThresholdScrollBar.Margin = new System.Windows.Forms.Padding(2);
            this.tdtThresholdScrollBar.Maximum = 100009;
            this.tdtThresholdScrollBar.Name = "tdtThresholdScrollBar";
            this.tdtThresholdScrollBar.Size = new System.Drawing.Size(260, 21);
            this.tdtThresholdScrollBar.TabIndex = 27;
            this.tdtThresholdScrollBar.Value = 10000;
            this.tdtThresholdScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.accelerationThresholdScrollBar_Scroll);
            // 
            // showDeviationCheckBox
            // 
            this.showDeviationCheckBox.AutoSize = true;
            this.showDeviationCheckBox.Location = new System.Drawing.Point(4, 65);
            this.showDeviationCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.showDeviationCheckBox.Name = "showDeviationCheckBox";
            this.showDeviationCheckBox.Size = new System.Drawing.Size(128, 17);
            this.showDeviationCheckBox.TabIndex = 29;
            this.showDeviationCheckBox.Text = "Show deviation angle";
            this.showDeviationCheckBox.UseVisualStyleBackColor = true;
            this.showDeviationCheckBox.CheckedChanged += new System.EventHandler(this.showAccelerationCheckBox_CheckedChanged);
            // 
            // SignalStatesViewerUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.showDeviationCheckBox);
            this.Controls.Add(this.tdtThresholdLabel);
            this.Controls.Add(this.tdtThresholdScrollBar);
            this.Controls.Add(this.hThresholdLabel);
            this.Controls.Add(this.amplitudeThresholdScrollBar);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.showStatesCheckBox);
            this.Controls.Add(this.autoApplyCheckBox);
            this.Controls.Add(this.signalStatesViewerLabel);
            this.Controls.Add(this.hThresholdScrollBar);
            this.Controls.Add(this.thresholdRatioLabel);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SignalStatesViewerUserControl";
            this.Size = new System.Drawing.Size(284, 205);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.HScrollBar hThresholdScrollBar;
        public System.Windows.Forms.Label thresholdRatioLabel;
        public System.Windows.Forms.Label signalStatesViewerLabel;
        public System.Windows.Forms.CheckBox autoApplyCheckBox;
        public System.Windows.Forms.CheckBox showStatesCheckBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        public System.Windows.Forms.Label hThresholdLabel;
        public System.Windows.Forms.Label tdtThresholdLabel;
        public System.Windows.Forms.HScrollBar tdtThresholdScrollBar;
        public System.Windows.Forms.CheckBox showDeviationCheckBox;
        public System.Windows.Forms.VScrollBar amplitudeThresholdScrollBar;
        public System.Windows.Forms.Button applyButton;
    }
}

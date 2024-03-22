
using Biological_Signal_Processing_Using_AI;

namespace BSP_Using_AI.DetailsModify.FiltersControls
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
            this.thresholdRatioLabel = new System.Windows.Forms.Label();
            this.signalPeaksAnalyzerLabel = new System.Windows.Forms.Label();
            this.autoApplyCheckBox = new System.Windows.Forms.CheckBox();
            this.showStatesCheckBox = new System.Windows.Forms.CheckBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hThresholdLabel = new System.Windows.Forms.Label();
            this.tdtThresholdLabel = new System.Windows.Forms.Label();
            this.showDeviationCheckBox = new System.Windows.Forms.CheckBox();
            this.artValueTextBox = new System.Windows.Forms.TextBox();
            this.tdtPercentLabel = new System.Windows.Forms.Label();
            this.tdtValueTextBox = new System.Windows.Forms.TextBox();
            this.htTimeUnitLabel = new System.Windows.Forms.Label();
            this.htValueTextBox = new System.Windows.Forms.TextBox();
            this.hThresholdScrollBar = new Biological_Signal_Processing_Using_AI.CustomHScrollBar();
            this.tdtThresholdScrollBar = new Biological_Signal_Processing_Using_AI.CustomHScrollBar();
            this.amplitudeThresholdScrollBar = new Biological_Signal_Processing_Using_AI.CustomVScrollBar();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // thresholdRatioLabel
            // 
            this.thresholdRatioLabel.AutoSize = true;
            this.thresholdRatioLabel.Location = new System.Drawing.Point(201, 38);
            this.thresholdRatioLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.thresholdRatioLabel.Name = "thresholdRatioLabel";
            this.thresholdRatioLabel.Size = new System.Drawing.Size(29, 13);
            this.thresholdRatioLabel.TabIndex = 19;
            this.thresholdRatioLabel.Text = "ART";
            // 
            // signalPeaksAnalyzerLabel
            // 
            this.signalPeaksAnalyzerLabel.AutoSize = true;
            this.signalPeaksAnalyzerLabel.Location = new System.Drawing.Point(2, 2);
            this.signalPeaksAnalyzerLabel.Margin = new System.Windows.Forms.Padding(2);
            this.signalPeaksAnalyzerLabel.Name = "signalPeaksAnalyzerLabel";
            this.signalPeaksAnalyzerLabel.Size = new System.Drawing.Size(111, 13);
            this.signalPeaksAnalyzerLabel.TabIndex = 21;
            this.signalPeaksAnalyzerLabel.Text = "SIgnal peaks analyzer";
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
            this.autoApplyCheckBox.CheckedChanged += new System.EventHandler(this.autoApplyCheckBox_CheckedChanged);
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
            // hThresholdLabel
            // 
            this.hThresholdLabel.AutoSize = true;
            this.hThresholdLabel.Location = new System.Drawing.Point(92, 181);
            this.hThresholdLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.hThresholdLabel.Name = "hThresholdLabel";
            this.hThresholdLabel.Size = new System.Drawing.Size(25, 13);
            this.hThresholdLabel.TabIndex = 26;
            this.hThresholdLabel.Text = "HT:";
            // 
            // tdtThresholdLabel
            // 
            this.tdtThresholdLabel.AutoSize = true;
            this.tdtThresholdLabel.Location = new System.Drawing.Point(111, 135);
            this.tdtThresholdLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.tdtThresholdLabel.Name = "tdtThresholdLabel";
            this.tdtThresholdLabel.Size = new System.Drawing.Size(32, 13);
            this.tdtThresholdLabel.TabIndex = 28;
            this.tdtThresholdLabel.Text = "TDT:";
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
            // artValueTextBox
            // 
            this.artValueTextBox.Location = new System.Drawing.Point(203, 54);
            this.artValueTextBox.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.artValueTextBox.Name = "artValueTextBox";
            this.artValueTextBox.Size = new System.Drawing.Size(43, 20);
            this.artValueTextBox.TabIndex = 30;
            this.artValueTextBox.Text = "0.2";
            this.artValueTextBox.TextChanged += new System.EventHandler(this.artValueTextBox_TextChanged);
            this.artValueTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.artValueTextBox_KeyPress);
            // 
            // tdtPercentLabel
            // 
            this.tdtPercentLabel.AutoSize = true;
            this.tdtPercentLabel.Location = new System.Drawing.Point(186, 135);
            this.tdtPercentLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.tdtPercentLabel.Name = "tdtPercentLabel";
            this.tdtPercentLabel.Size = new System.Drawing.Size(15, 13);
            this.tdtPercentLabel.TabIndex = 33;
            this.tdtPercentLabel.Text = "%";
            // 
            // tdtValueTextBox
            // 
            this.tdtValueTextBox.Location = new System.Drawing.Point(142, 132);
            this.tdtValueTextBox.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.tdtValueTextBox.Name = "tdtValueTextBox";
            this.tdtValueTextBox.Size = new System.Drawing.Size(43, 20);
            this.tdtValueTextBox.TabIndex = 32;
            this.tdtValueTextBox.Text = "10";
            this.tdtValueTextBox.TextChanged += new System.EventHandler(this.tdtValueTextBox_TextChanged);
            this.tdtValueTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.artValueTextBox_KeyPress);
            // 
            // htTimeUnitLabel
            // 
            this.htTimeUnitLabel.AutoSize = true;
            this.htTimeUnitLabel.Location = new System.Drawing.Point(160, 181);
            this.htTimeUnitLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.htTimeUnitLabel.Name = "htTimeUnitLabel";
            this.htTimeUnitLabel.Size = new System.Drawing.Size(24, 13);
            this.htTimeUnitLabel.TabIndex = 35;
            this.htTimeUnitLabel.Text = "sec";
            // 
            // htValueTextBox
            // 
            this.htValueTextBox.Location = new System.Drawing.Point(116, 179);
            this.htValueTextBox.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.htValueTextBox.Name = "htValueTextBox";
            this.htValueTextBox.Size = new System.Drawing.Size(43, 20);
            this.htValueTextBox.TabIndex = 34;
            this.htValueTextBox.Text = "0";
            this.htValueTextBox.TextChanged += new System.EventHandler(this.htValueTextBox_TextChanged);
            this.htValueTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.artValueTextBox_KeyPress);
            // 
            // hThresholdScrollBar
            // 
            this.hThresholdScrollBar.Location = new System.Drawing.Point(3, 155);
            this.hThresholdScrollBar.Maximum = 1009;
            this.hThresholdScrollBar.Name = "hThresholdScrollBar";
            this.hThresholdScrollBar.Size = new System.Drawing.Size(260, 21);
            this.hThresholdScrollBar.TabIndex = 20;
            this.hThresholdScrollBar.Value = 1;
            this.hThresholdScrollBar.ValueChanged += new System.EventHandler(this.hThresholdScrollBar_ValueChanged);
            // 
            // tdtThresholdScrollBar
            // 
            this.tdtThresholdScrollBar.Location = new System.Drawing.Point(3, 109);
            this.tdtThresholdScrollBar.Maximum = 100009;
            this.tdtThresholdScrollBar.Name = "tdtThresholdScrollBar";
            this.tdtThresholdScrollBar.Size = new System.Drawing.Size(260, 21);
            this.tdtThresholdScrollBar.TabIndex = 27;
            this.tdtThresholdScrollBar.Value = 10000;
            this.tdtThresholdScrollBar.ValueChanged += new System.EventHandler(this.tdtThresholdScrollBar_ValueChanged);
            // 
            // amplitudeThresholdScrollBar
            // 
            this.amplitudeThresholdScrollBar.Location = new System.Drawing.Point(266, 0);
            this.amplitudeThresholdScrollBar.Maximum = 1009;
            this.amplitudeThresholdScrollBar.Name = "amplitudeThresholdScrollBar";
            this.amplitudeThresholdScrollBar.Size = new System.Drawing.Size(21, 180);
            this.amplitudeThresholdScrollBar.TabIndex = 25;
            this.amplitudeThresholdScrollBar.Value = 800;
            this.amplitudeThresholdScrollBar.ValueChanged += new System.EventHandler(this.amplitudeThresholdScrollBar_ValueChanged);
            // 
            // SignalStatesViewerUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.amplitudeThresholdScrollBar);
            this.Controls.Add(this.tdtThresholdScrollBar);
            this.Controls.Add(this.hThresholdScrollBar);
            this.Controls.Add(this.htTimeUnitLabel);
            this.Controls.Add(this.htValueTextBox);
            this.Controls.Add(this.tdtPercentLabel);
            this.Controls.Add(this.tdtValueTextBox);
            this.Controls.Add(this.artValueTextBox);
            this.Controls.Add(this.showDeviationCheckBox);
            this.Controls.Add(this.tdtThresholdLabel);
            this.Controls.Add(this.hThresholdLabel);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.showStatesCheckBox);
            this.Controls.Add(this.autoApplyCheckBox);
            this.Controls.Add(this.signalPeaksAnalyzerLabel);
            this.Controls.Add(this.thresholdRatioLabel);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SignalStatesViewerUserControl";
            this.Size = new System.Drawing.Size(284, 205);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label thresholdRatioLabel;
        public System.Windows.Forms.Label signalPeaksAnalyzerLabel;
        public System.Windows.Forms.CheckBox autoApplyCheckBox;
        public System.Windows.Forms.CheckBox showStatesCheckBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        public System.Windows.Forms.Label hThresholdLabel;
        public System.Windows.Forms.Label tdtThresholdLabel;
        public System.Windows.Forms.CheckBox showDeviationCheckBox;
        public System.Windows.Forms.Button applyButton;
        public System.Windows.Forms.Label tdtPercentLabel;
        public System.Windows.Forms.Label htTimeUnitLabel;
        public CustomHScrollBar hThresholdScrollBar;
        public CustomHScrollBar tdtThresholdScrollBar;
        public System.Windows.Forms.TextBox artValueTextBox;
        public System.Windows.Forms.TextBox tdtValueTextBox;
        public System.Windows.Forms.TextBox htValueTextBox;
        public CustomVScrollBar amplitudeThresholdScrollBar;
    }
}

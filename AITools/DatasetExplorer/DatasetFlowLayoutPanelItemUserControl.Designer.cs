
namespace BSP_Using_AI.AITools.DatasetExplorer
{
    partial class DatasetFlowLayoutPanelItemUserControl
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
            this.signalNameLabel = new System.Windows.Forms.Label();
            this.startingIndexLabel = new System.Windows.Forms.Label();
            this.samplingRateLabel = new System.Windows.Forms.Label();
            this.featuresDetailsButton = new System.Windows.Forms.Button();
            this.selectionCheckBox = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quantizationStepLabel = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // signalNameLabel
            // 
            this.signalNameLabel.AutoSize = true;
            this.signalNameLabel.Location = new System.Drawing.Point(4, 4);
            this.signalNameLabel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.signalNameLabel.MaximumSize = new System.Drawing.Size(217, 0);
            this.signalNameLabel.MinimumSize = new System.Drawing.Size(217, 28);
            this.signalNameLabel.Name = "signalNameLabel";
            this.signalNameLabel.Size = new System.Drawing.Size(217, 28);
            this.signalNameLabel.TabIndex = 3;
            this.signalNameLabel.Text = "Signal name";
            this.signalNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // startingIndexLabel
            // 
            this.startingIndexLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.startingIndexLabel.Location = new System.Drawing.Point(234, 4);
            this.startingIndexLabel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.startingIndexLabel.Name = "startingIndexLabel";
            this.startingIndexLabel.Size = new System.Drawing.Size(144, 28);
            this.startingIndexLabel.TabIndex = 4;
            this.startingIndexLabel.Text = "Starting index (sec)";
            this.startingIndexLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // samplingRateLabel
            // 
            this.samplingRateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.samplingRateLabel.Location = new System.Drawing.Point(386, 4);
            this.samplingRateLabel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.samplingRateLabel.Name = "samplingRateLabel";
            this.samplingRateLabel.Size = new System.Drawing.Size(116, 28);
            this.samplingRateLabel.TabIndex = 5;
            this.samplingRateLabel.Text = "Sampling rate";
            this.samplingRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // featuresDetailsButton
            // 
            this.featuresDetailsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.featuresDetailsButton.Location = new System.Drawing.Point(653, 4);
            this.featuresDetailsButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.featuresDetailsButton.Name = "featuresDetailsButton";
            this.featuresDetailsButton.Size = new System.Drawing.Size(152, 28);
            this.featuresDetailsButton.TabIndex = 20;
            this.featuresDetailsButton.Text = "Features details";
            this.featuresDetailsButton.UseVisualStyleBackColor = true;
            this.featuresDetailsButton.Click += new System.EventHandler(this.featuresDetailsButton_Click);
            // 
            // selectionCheckBox
            // 
            this.selectionCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectionCheckBox.AutoSize = true;
            this.selectionCheckBox.Location = new System.Drawing.Point(874, 10);
            this.selectionCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.selectionCheckBox.Name = "selectionCheckBox";
            this.selectionCheckBox.Size = new System.Drawing.Size(15, 14);
            this.selectionCheckBox.TabIndex = 21;
            this.selectionCheckBox.UseVisualStyleBackColor = true;
            this.selectionCheckBox.Click += new System.EventHandler(this.selectionCheckBox_Click);
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
            // quantizationStepLabel
            // 
            this.quantizationStepLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.quantizationStepLabel.Location = new System.Drawing.Point(510, 4);
            this.quantizationStepLabel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.quantizationStepLabel.Name = "quantizationStepLabel";
            this.quantizationStepLabel.Size = new System.Drawing.Size(136, 28);
            this.quantizationStepLabel.TabIndex = 22;
            this.quantizationStepLabel.Text = "Quantization step";
            this.quantizationStepLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DatasetFlowLayoutPanelItemUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.quantizationStepLabel);
            this.Controls.Add(this.selectionCheckBox);
            this.Controls.Add(this.featuresDetailsButton);
            this.Controls.Add(this.samplingRateLabel);
            this.Controls.Add(this.startingIndexLabel);
            this.Controls.Add(this.signalNameLabel);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "DatasetFlowLayoutPanelItemUserControl";
            this.Size = new System.Drawing.Size(902, 36);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Button featuresDetailsButton;
        public System.Windows.Forms.Label signalNameLabel;
        public System.Windows.Forms.Label startingIndexLabel;
        public System.Windows.Forms.Label samplingRateLabel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        public System.Windows.Forms.CheckBox selectionCheckBox;
        public System.Windows.Forms.Label quantizationStepLabel;
    }
}

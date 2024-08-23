namespace Biological_Signal_Processing_Using_AI.AITools.Details.ValidationDataSelection
{
    partial class ValSeleDataItemUC
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
            this.samplingRateLabel = new System.Windows.Forms.Label();
            this.startingIndexLabel = new System.Windows.Forms.Label();
            this.signalNameLabel = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quantizationStepLabel = new System.Windows.Forms.Label();
            this.categoryLabel = new System.Windows.Forms.Label();
            this.orderTextBox = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // samplingRateLabel
            // 
            this.samplingRateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.samplingRateLabel.Location = new System.Drawing.Point(293, 3);
            this.samplingRateLabel.Margin = new System.Windows.Forms.Padding(3);
            this.samplingRateLabel.Name = "samplingRateLabel";
            this.samplingRateLabel.Size = new System.Drawing.Size(87, 23);
            this.samplingRateLabel.TabIndex = 25;
            this.samplingRateLabel.Text = "Sampling rate";
            this.samplingRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // startingIndexLabel
            // 
            this.startingIndexLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.startingIndexLabel.Location = new System.Drawing.Point(179, 3);
            this.startingIndexLabel.Margin = new System.Windows.Forms.Padding(3);
            this.startingIndexLabel.Name = "startingIndexLabel";
            this.startingIndexLabel.Size = new System.Drawing.Size(108, 23);
            this.startingIndexLabel.TabIndex = 24;
            this.startingIndexLabel.Text = "Starting index (sec)";
            this.startingIndexLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // signalNameLabel
            // 
            this.signalNameLabel.AutoSize = true;
            this.signalNameLabel.Location = new System.Drawing.Point(6, 3);
            this.signalNameLabel.Margin = new System.Windows.Forms.Padding(3);
            this.signalNameLabel.MaximumSize = new System.Drawing.Size(163, 0);
            this.signalNameLabel.MinimumSize = new System.Drawing.Size(163, 23);
            this.signalNameLabel.Name = "signalNameLabel";
            this.signalNameLabel.Size = new System.Drawing.Size(163, 23);
            this.signalNameLabel.TabIndex = 23;
            this.signalNameLabel.Text = "Signal name";
            this.signalNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            // 
            // quantizationStepLabel
            // 
            this.quantizationStepLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.quantizationStepLabel.Location = new System.Drawing.Point(385, 3);
            this.quantizationStepLabel.Margin = new System.Windows.Forms.Padding(3);
            this.quantizationStepLabel.Name = "quantizationStepLabel";
            this.quantizationStepLabel.Size = new System.Drawing.Size(102, 23);
            this.quantizationStepLabel.TabIndex = 28;
            this.quantizationStepLabel.Text = "Quantization step";
            this.quantizationStepLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // categoryLabel
            // 
            this.categoryLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.categoryLabel.Location = new System.Drawing.Point(493, 3);
            this.categoryLabel.Margin = new System.Windows.Forms.Padding(3);
            this.categoryLabel.Name = "categoryLabel";
            this.categoryLabel.Size = new System.Drawing.Size(90, 23);
            this.categoryLabel.TabIndex = 29;
            this.categoryLabel.Text = "Training";
            this.categoryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // orderTextBox
            // 
            this.orderTextBox.Location = new System.Drawing.Point(589, 5);
            this.orderTextBox.Name = "orderTextBox";
            this.orderTextBox.Size = new System.Drawing.Size(84, 20);
            this.orderTextBox.TabIndex = 30;
            this.orderTextBox.TextChanged += new System.EventHandler(this.orderTextBox_TextChanged);
            this.orderTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.orderTextBox_KeyPress);
            // 
            // ValidationDataItemUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.orderTextBox);
            this.Controls.Add(this.categoryLabel);
            this.Controls.Add(this.samplingRateLabel);
            this.Controls.Add(this.startingIndexLabel);
            this.Controls.Add(this.signalNameLabel);
            this.Controls.Add(this.quantizationStepLabel);
            this.Name = "ValidationDataItemUserControl";
            this.Size = new System.Drawing.Size(676, 29);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label samplingRateLabel;
        public System.Windows.Forms.Label startingIndexLabel;
        public System.Windows.Forms.Label signalNameLabel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        public System.Windows.Forms.Label quantizationStepLabel;
        public System.Windows.Forms.Label categoryLabel;
        public System.Windows.Forms.TextBox orderTextBox;
    }
}

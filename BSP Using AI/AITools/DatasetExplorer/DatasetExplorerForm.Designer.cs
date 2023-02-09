
namespace BSP_Using_AI.AITools.DatasetExplorer
{
    partial class DatasetExplorerForm
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
            this.signalsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.unselectAllButton = new System.Windows.Forms.Button();
            this.deleteSelectionButton = new System.Windows.Forms.Button();
            this.samplingRateLabel = new System.Windows.Forms.Label();
            this.startingIndexLabel = new System.Windows.Forms.Label();
            this.signalNameLabel = new System.Windows.Forms.Label();
            this.fitSelectionButton = new System.Windows.Forms.Button();
            this.instrucitonLabel = new System.Windows.Forms.Label();
            this.quantizationStepLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // signalsFlowLayoutPanel
            // 
            this.signalsFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.signalsFlowLayoutPanel.AutoScroll = true;
            this.signalsFlowLayoutPanel.Location = new System.Drawing.Point(3, 122);
            this.signalsFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.signalsFlowLayoutPanel.Name = "signalsFlowLayoutPanel";
            this.signalsFlowLayoutPanel.Size = new System.Drawing.Size(910, 554);
            this.signalsFlowLayoutPanel.TabIndex = 1;
            this.signalsFlowLayoutPanel.SizeChanged += new System.EventHandler(this.signalsFlowLayoutPanel_SizeChanged);
            // 
            // unselectAllButton
            // 
            this.unselectAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.unselectAllButton.Location = new System.Drawing.Point(758, 14);
            this.unselectAllButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.unselectAllButton.Name = "unselectAllButton";
            this.unselectAllButton.Size = new System.Drawing.Size(143, 27);
            this.unselectAllButton.TabIndex = 27;
            this.unselectAllButton.Text = "Unselect all";
            this.unselectAllButton.UseVisualStyleBackColor = true;
            this.unselectAllButton.Click += new System.EventHandler(this.uselectAllButton_Click);
            // 
            // deleteSelectionButton
            // 
            this.deleteSelectionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteSelectionButton.Location = new System.Drawing.Point(613, 14);
            this.deleteSelectionButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.deleteSelectionButton.Name = "deleteSelectionButton";
            this.deleteSelectionButton.Size = new System.Drawing.Size(140, 27);
            this.deleteSelectionButton.TabIndex = 28;
            this.deleteSelectionButton.Text = "Delete selection";
            this.deleteSelectionButton.UseVisualStyleBackColor = true;
            this.deleteSelectionButton.Click += new System.EventHandler(this.removeSelectionButton_Click);
            // 
            // samplingRateLabel
            // 
            this.samplingRateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.samplingRateLabel.Location = new System.Drawing.Point(396, 86);
            this.samplingRateLabel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.samplingRateLabel.Name = "samplingRateLabel";
            this.samplingRateLabel.Size = new System.Drawing.Size(116, 28);
            this.samplingRateLabel.TabIndex = 31;
            this.samplingRateLabel.Text = "Sampling rate";
            this.samplingRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // startingIndexLabel
            // 
            this.startingIndexLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.startingIndexLabel.Location = new System.Drawing.Point(244, 86);
            this.startingIndexLabel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.startingIndexLabel.Name = "startingIndexLabel";
            this.startingIndexLabel.Size = new System.Drawing.Size(144, 28);
            this.startingIndexLabel.TabIndex = 30;
            this.startingIndexLabel.Text = "Starting index (sec)";
            this.startingIndexLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // signalNameLabel
            // 
            this.signalNameLabel.AutoSize = true;
            this.signalNameLabel.Location = new System.Drawing.Point(8, 86);
            this.signalNameLabel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.signalNameLabel.MaximumSize = new System.Drawing.Size(217, 0);
            this.signalNameLabel.MinimumSize = new System.Drawing.Size(217, 28);
            this.signalNameLabel.Name = "signalNameLabel";
            this.signalNameLabel.Size = new System.Drawing.Size(217, 28);
            this.signalNameLabel.TabIndex = 29;
            this.signalNameLabel.Text = "Signal name";
            this.signalNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fitSelectionButton
            // 
            this.fitSelectionButton.Location = new System.Drawing.Point(19, 14);
            this.fitSelectionButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.fitSelectionButton.Name = "fitSelectionButton";
            this.fitSelectionButton.Size = new System.Drawing.Size(140, 27);
            this.fitSelectionButton.TabIndex = 32;
            this.fitSelectionButton.Text = "Fit selection";
            this.fitSelectionButton.UseVisualStyleBackColor = true;
            this.fitSelectionButton.Visible = false;
            this.fitSelectionButton.Click += new System.EventHandler(this.fitSelectionButton_Click);
            // 
            // instrucitonLabel
            // 
            this.instrucitonLabel.AutoSize = true;
            this.instrucitonLabel.Location = new System.Drawing.Point(16, 43);
            this.instrucitonLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.instrucitonLabel.Name = "instrucitonLabel";
            this.instrucitonLabel.Size = new System.Drawing.Size(196, 16);
            this.instrucitonLabel.TabIndex = 33;
            this.instrucitonLabel.Text = "(Click Shift for multiple selection)";
            this.instrucitonLabel.Visible = false;
            // 
            // quantizationStepLabel
            // 
            this.quantizationStepLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.quantizationStepLabel.Location = new System.Drawing.Point(520, 86);
            this.quantizationStepLabel.Margin = new System.Windows.Forms.Padding(4);
            this.quantizationStepLabel.Name = "quantizationStepLabel";
            this.quantizationStepLabel.Size = new System.Drawing.Size(136, 28);
            this.quantizationStepLabel.TabIndex = 34;
            this.quantizationStepLabel.Text = "Quantization step";
            this.quantizationStepLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DatasetExplorerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(916, 678);
            this.Controls.Add(this.quantizationStepLabel);
            this.Controls.Add(this.instrucitonLabel);
            this.Controls.Add(this.fitSelectionButton);
            this.Controls.Add(this.samplingRateLabel);
            this.Controls.Add(this.startingIndexLabel);
            this.Controls.Add(this.signalNameLabel);
            this.Controls.Add(this.deleteSelectionButton);
            this.Controls.Add(this.unselectAllButton);
            this.Controls.Add(this.signalsFlowLayoutPanel);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "DatasetExplorerForm";
            this.Text = "DatasetExplorerForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DatasetExplorerForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DatasetExplorerForm_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel signalsFlowLayoutPanel;
        private System.Windows.Forms.Label samplingRateLabel;
        private System.Windows.Forms.Label startingIndexLabel;
        private System.Windows.Forms.Label signalNameLabel;
        public System.Windows.Forms.Label instrucitonLabel;
        public System.Windows.Forms.Button unselectAllButton;
        public System.Windows.Forms.Button deleteSelectionButton;
        public System.Windows.Forms.Button fitSelectionButton;
        public System.Windows.Forms.Label quantizationStepLabel;
    }
}
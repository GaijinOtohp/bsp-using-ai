using BSP_Using_AI;

namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation.ValDataVis.ROC_thresholds
{
    partial class ROCDataSelForm
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
            this.quantizationStepLabel = new System.Windows.Forms.Label();
            this.startOptimizationButton = new System.Windows.Forms.Button();
            this.samplingRateLabel = new System.Windows.Forms.Label();
            this.startingIndexLabel = new System.Windows.Forms.Label();
            this.signalNameLabel = new System.Windows.Forms.Label();
            this.categoryLabel = new System.Windows.Forms.Label();
            this.separation1Label = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.roleLabel = new System.Windows.Forms.Label();
            this.instrucitonLabel = new System.Windows.Forms.Label();
            this.ValDataFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // quantizationStepLabel
            // 
            this.quantizationStepLabel.Location = new System.Drawing.Point(446, 77);
            this.quantizationStepLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.quantizationStepLabel.Name = "quantizationStepLabel";
            this.quantizationStepLabel.Size = new System.Drawing.Size(119, 27);
            this.quantizationStepLabel.TabIndex = 41;
            this.quantizationStepLabel.Text = "Quantization step";
            this.quantizationStepLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // startOptimizationButton
            // 
            this.startOptimizationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.startOptimizationButton.Location = new System.Drawing.Point(708, 693);
            this.startOptimizationButton.Margin = new System.Windows.Forms.Padding(2);
            this.startOptimizationButton.Name = "startOptimizationButton";
            this.startOptimizationButton.Size = new System.Drawing.Size(122, 25);
            this.startOptimizationButton.TabIndex = 39;
            this.startOptimizationButton.Text = "Sart optimization";
            this.startOptimizationButton.UseVisualStyleBackColor = true;
            this.startOptimizationButton.Click += new System.EventHandler(this.startOptimizationButton_Click);
            // 
            // samplingRateLabel
            // 
            this.samplingRateLabel.Location = new System.Drawing.Point(338, 77);
            this.samplingRateLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.samplingRateLabel.Name = "samplingRateLabel";
            this.samplingRateLabel.Size = new System.Drawing.Size(102, 27);
            this.samplingRateLabel.TabIndex = 38;
            this.samplingRateLabel.Text = "Sampling rate";
            this.samplingRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // startingIndexLabel
            // 
            this.startingIndexLabel.Location = new System.Drawing.Point(205, 77);
            this.startingIndexLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.startingIndexLabel.Name = "startingIndexLabel";
            this.startingIndexLabel.Size = new System.Drawing.Size(126, 27);
            this.startingIndexLabel.TabIndex = 37;
            this.startingIndexLabel.Text = "Starting index (sec)";
            this.startingIndexLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // signalNameLabel
            // 
            this.signalNameLabel.AutoSize = true;
            this.signalNameLabel.Location = new System.Drawing.Point(8, 77);
            this.signalNameLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.signalNameLabel.MaximumSize = new System.Drawing.Size(190, 0);
            this.signalNameLabel.MinimumSize = new System.Drawing.Size(190, 27);
            this.signalNameLabel.Name = "signalNameLabel";
            this.signalNameLabel.Size = new System.Drawing.Size(190, 27);
            this.signalNameLabel.TabIndex = 36;
            this.signalNameLabel.Text = "Signal name";
            this.signalNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // categoryLabel
            // 
            this.categoryLabel.Location = new System.Drawing.Point(572, 77);
            this.categoryLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.categoryLabel.Name = "categoryLabel";
            this.categoryLabel.Size = new System.Drawing.Size(105, 27);
            this.categoryLabel.TabIndex = 48;
            this.categoryLabel.Text = "Category";
            this.categoryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // separation1Label
            // 
            this.separation1Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separation1Label.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.separation1Label.Location = new System.Drawing.Point(0, 63);
            this.separation1Label.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.separation1Label.Name = "separation1Label";
            this.separation1Label.Size = new System.Drawing.Size(845, 1);
            this.separation1Label.TabIndex = 49;
            this.separation1Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(0, 685);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(845, 1);
            this.label1.TabIndex = 50;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // roleLabel
            // 
            this.roleLabel.Location = new System.Drawing.Point(684, 77);
            this.roleLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.roleLabel.Name = "roleLabel";
            this.roleLabel.Size = new System.Drawing.Size(105, 27);
            this.roleLabel.TabIndex = 51;
            this.roleLabel.Text = "Role";
            this.roleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // instrucitonLabel
            // 
            this.instrucitonLabel.AutoSize = true;
            this.instrucitonLabel.Location = new System.Drawing.Point(8, 10);
            this.instrucitonLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.instrucitonLabel.Name = "instrucitonLabel";
            this.instrucitonLabel.Size = new System.Drawing.Size(183, 15);
            this.instrucitonLabel.TabIndex = 56;
            this.instrucitonLabel.Text = "(Click Shift for multiple selection)";
            this.instrucitonLabel.Visible = false;
            // 
            // ValDataFlowLayoutPanel
            // 
            this.ValDataFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ValDataFlowLayoutPanel.AutoScroll = true;
            this.ValDataFlowLayoutPanel.Location = new System.Drawing.Point(4, 110);
            this.ValDataFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ValDataFlowLayoutPanel.Name = "ValDataFlowLayoutPanel";
            this.ValDataFlowLayoutPanel.Size = new System.Drawing.Size(838, 568);
            this.ValDataFlowLayoutPanel.TabIndex = 35;
            // 
            // ROCDataSelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 732);
            this.Controls.Add(this.ValDataFlowLayoutPanel);
            this.Controls.Add(this.instrucitonLabel);
            this.Controls.Add(this.roleLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.separation1Label);
            this.Controls.Add(this.categoryLabel);
            this.Controls.Add(this.quantizationStepLabel);
            this.Controls.Add(this.startOptimizationButton);
            this.Controls.Add(this.samplingRateLabel);
            this.Controls.Add(this.startingIndexLabel);
            this.Controls.Add(this.signalNameLabel);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ROCDataSelForm";
            this.Text = "ValidationDataSelectionForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ValDataSelectionForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ValDataSelectionForm_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label quantizationStepLabel;
        public System.Windows.Forms.Button startOptimizationButton;
        private System.Windows.Forms.Label samplingRateLabel;
        private System.Windows.Forms.Label startingIndexLabel;
        private System.Windows.Forms.Label signalNameLabel;
        public System.Windows.Forms.Label categoryLabel;
        private System.Windows.Forms.Label separation1Label;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label roleLabel;
        public System.Windows.Forms.Label instrucitonLabel;
        private System.Windows.Forms.FlowLayoutPanel ValDataFlowLayoutPanel;
    }
}
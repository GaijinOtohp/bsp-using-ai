using BSP_Using_AI;

namespace Biological_Signal_Processing_Using_AI.AITools.Details.ValidationDataSelection
{
    partial class ValDataSelectionForm
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
            this.trainingSetRatioLabel = new System.Windows.Forms.Label();
            this.startValidationButton = new System.Windows.Forms.Button();
            this.samplingRateLabel = new System.Windows.Forms.Label();
            this.startingIndexLabel = new System.Windows.Forms.Label();
            this.signalNameLabel = new System.Windows.Forms.Label();
            this.crossValidationRadioButton = new System.Windows.Forms.RadioButton();
            this.holdoutValidationRadioButton = new System.Windows.Forms.RadioButton();
            this.shuffleButton = new System.Windows.Forms.Button();
            this.numberOfFoldsTextBox = new System.Windows.Forms.TextBox();
            this.trainingSetRatioTextBox = new System.Windows.Forms.TextBox();
            this.numberOfFoldsLabel = new System.Windows.Forms.Label();
            this.categoryLabel = new System.Windows.Forms.Label();
            this.separation1Label = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.orderLabel = new System.Windows.Forms.Label();
            this.applyChangesButton = new System.Windows.Forms.Button();
            this.fastValidadioButton = new System.Windows.Forms.RadioButton();
            this.trainingAndValidRadioButton = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.instrucitonLabel = new System.Windows.Forms.Label();
            this.ValDataFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // quantizationStepLabel
            // 
            this.quantizationStepLabel.Location = new System.Drawing.Point(446, 126);
            this.quantizationStepLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.quantizationStepLabel.Name = "quantizationStepLabel";
            this.quantizationStepLabel.Size = new System.Drawing.Size(119, 27);
            this.quantizationStepLabel.TabIndex = 41;
            this.quantizationStepLabel.Text = "Quantization step";
            this.quantizationStepLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // trainingSetRatioLabel
            // 
            this.trainingSetRatioLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trainingSetRatioLabel.AutoSize = true;
            this.trainingSetRatioLabel.Location = new System.Drawing.Point(558, 32);
            this.trainingSetRatioLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.trainingSetRatioLabel.Name = "trainingSetRatioLabel";
            this.trainingSetRatioLabel.Size = new System.Drawing.Size(94, 15);
            this.trainingSetRatioLabel.TabIndex = 40;
            this.trainingSetRatioLabel.Text = "Training set ratio";
            this.trainingSetRatioLabel.Visible = false;
            // 
            // startValidationButton
            // 
            this.startValidationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.startValidationButton.Location = new System.Drawing.Point(687, 693);
            this.startValidationButton.Margin = new System.Windows.Forms.Padding(2);
            this.startValidationButton.Name = "startValidationButton";
            this.startValidationButton.Size = new System.Drawing.Size(122, 25);
            this.startValidationButton.TabIndex = 39;
            this.startValidationButton.Text = "Sart validation";
            this.startValidationButton.UseVisualStyleBackColor = true;
            this.startValidationButton.Click += new System.EventHandler(this.startValidationButton_Click);
            // 
            // samplingRateLabel
            // 
            this.samplingRateLabel.Location = new System.Drawing.Point(338, 126);
            this.samplingRateLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.samplingRateLabel.Name = "samplingRateLabel";
            this.samplingRateLabel.Size = new System.Drawing.Size(102, 27);
            this.samplingRateLabel.TabIndex = 38;
            this.samplingRateLabel.Text = "Sampling rate";
            this.samplingRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // startingIndexLabel
            // 
            this.startingIndexLabel.Location = new System.Drawing.Point(205, 126);
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
            this.signalNameLabel.Location = new System.Drawing.Point(8, 126);
            this.signalNameLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.signalNameLabel.MaximumSize = new System.Drawing.Size(190, 0);
            this.signalNameLabel.MinimumSize = new System.Drawing.Size(190, 27);
            this.signalNameLabel.Name = "signalNameLabel";
            this.signalNameLabel.Size = new System.Drawing.Size(190, 27);
            this.signalNameLabel.TabIndex = 36;
            this.signalNameLabel.Text = "Signal name";
            this.signalNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // crossValidationRadioButton
            // 
            this.crossValidationRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.crossValidationRadioButton.AutoSize = true;
            this.crossValidationRadioButton.Location = new System.Drawing.Point(694, 10);
            this.crossValidationRadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.crossValidationRadioButton.Name = "crossValidationRadioButton";
            this.crossValidationRadioButton.Size = new System.Drawing.Size(109, 19);
            this.crossValidationRadioButton.TabIndex = 43;
            this.crossValidationRadioButton.Text = "Cross validation";
            this.crossValidationRadioButton.UseVisualStyleBackColor = true;
            this.crossValidationRadioButton.CheckedChanged += new System.EventHandler(this.crossValidationRadioButton_CheckedChanged);
            // 
            // holdoutValidationRadioButton
            // 
            this.holdoutValidationRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.holdoutValidationRadioButton.AutoSize = true;
            this.holdoutValidationRadioButton.Checked = true;
            this.holdoutValidationRadioButton.Location = new System.Drawing.Point(561, 11);
            this.holdoutValidationRadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.holdoutValidationRadioButton.Name = "holdoutValidationRadioButton";
            this.holdoutValidationRadioButton.Size = new System.Drawing.Size(124, 19);
            this.holdoutValidationRadioButton.TabIndex = 42;
            this.holdoutValidationRadioButton.TabStop = true;
            this.holdoutValidationRadioButton.Text = "Holdout validation";
            this.holdoutValidationRadioButton.UseVisualStyleBackColor = true;
            this.holdoutValidationRadioButton.CheckedChanged += new System.EventHandler(this.holdoutValidationRadioButton_CheckedChanged);
            // 
            // shuffleButton
            // 
            this.shuffleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.shuffleButton.Location = new System.Drawing.Point(471, 80);
            this.shuffleButton.Margin = new System.Windows.Forms.Padding(2);
            this.shuffleButton.Name = "shuffleButton";
            this.shuffleButton.Size = new System.Drawing.Size(86, 25);
            this.shuffleButton.TabIndex = 44;
            this.shuffleButton.Text = "Shuffle";
            this.shuffleButton.UseVisualStyleBackColor = true;
            this.shuffleButton.Click += new System.EventHandler(this.shuffleButton_Click);
            // 
            // numberOfFoldsTextBox
            // 
            this.numberOfFoldsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numberOfFoldsTextBox.Enabled = false;
            this.numberOfFoldsTextBox.Location = new System.Drawing.Point(694, 51);
            this.numberOfFoldsTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numberOfFoldsTextBox.Name = "numberOfFoldsTextBox";
            this.numberOfFoldsTextBox.Size = new System.Drawing.Size(114, 23);
            this.numberOfFoldsTextBox.TabIndex = 45;
            this.numberOfFoldsTextBox.Text = "10";
            this.numberOfFoldsTextBox.TextChanged += new System.EventHandler(this.numberOfFoldsTextBox_TextChanged);
            this.numberOfFoldsTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.trainingSetRatioTextBox_KeyPress);
            // 
            // trainingSetRatioTextBox
            // 
            this.trainingSetRatioTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trainingSetRatioTextBox.Location = new System.Drawing.Point(561, 51);
            this.trainingSetRatioTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.trainingSetRatioTextBox.Name = "trainingSetRatioTextBox";
            this.trainingSetRatioTextBox.Size = new System.Drawing.Size(114, 23);
            this.trainingSetRatioTextBox.TabIndex = 46;
            this.trainingSetRatioTextBox.Text = "0.75";
            this.trainingSetRatioTextBox.TextChanged += new System.EventHandler(this.trainingSetRatioTextBox_TextChanged);
            this.trainingSetRatioTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.trainingSetRatioTextBox_KeyPress);
            // 
            // numberOfFoldsLabel
            // 
            this.numberOfFoldsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numberOfFoldsLabel.AutoSize = true;
            this.numberOfFoldsLabel.Location = new System.Drawing.Point(691, 32);
            this.numberOfFoldsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.numberOfFoldsLabel.Name = "numberOfFoldsLabel";
            this.numberOfFoldsLabel.Size = new System.Drawing.Size(94, 15);
            this.numberOfFoldsLabel.TabIndex = 47;
            this.numberOfFoldsLabel.Text = "Number of folds";
            this.numberOfFoldsLabel.Visible = false;
            // 
            // categoryLabel
            // 
            this.categoryLabel.Location = new System.Drawing.Point(572, 126);
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
            this.separation1Label.Location = new System.Drawing.Point(0, 112);
            this.separation1Label.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.separation1Label.Name = "separation1Label";
            this.separation1Label.Size = new System.Drawing.Size(824, 1);
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
            this.label1.Size = new System.Drawing.Size(824, 1);
            this.label1.TabIndex = 50;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // orderLabel
            // 
            this.orderLabel.Location = new System.Drawing.Point(684, 126);
            this.orderLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.orderLabel.Name = "orderLabel";
            this.orderLabel.Size = new System.Drawing.Size(105, 27);
            this.orderLabel.TabIndex = 51;
            this.orderLabel.Text = "Order";
            this.orderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // applyChangesButton
            // 
            this.applyChangesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.applyChangesButton.Location = new System.Drawing.Point(561, 80);
            this.applyChangesButton.Margin = new System.Windows.Forms.Padding(2);
            this.applyChangesButton.Name = "applyChangesButton";
            this.applyChangesButton.Size = new System.Drawing.Size(248, 25);
            this.applyChangesButton.TabIndex = 52;
            this.applyChangesButton.Text = "Apply changes";
            this.applyChangesButton.UseVisualStyleBackColor = true;
            this.applyChangesButton.Click += new System.EventHandler(this.applyChangesButton_Click);
            // 
            // fastValidadioButton
            // 
            this.fastValidadioButton.AutoSize = true;
            this.fastValidadioButton.Location = new System.Drawing.Point(2, 23);
            this.fastValidadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.fastValidadioButton.Name = "fastValidadioButton";
            this.fastValidadioButton.Size = new System.Drawing.Size(101, 19);
            this.fastValidadioButton.TabIndex = 54;
            this.fastValidadioButton.Text = "Fast validation";
            this.fastValidadioButton.UseVisualStyleBackColor = true;
            this.fastValidadioButton.CheckedChanged += new System.EventHandler(this.fastValidadioButton_CheckedChanged);
            // 
            // trainingAndValidRadioButton
            // 
            this.trainingAndValidRadioButton.AutoSize = true;
            this.trainingAndValidRadioButton.Checked = true;
            this.trainingAndValidRadioButton.Location = new System.Drawing.Point(2, 2);
            this.trainingAndValidRadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.trainingAndValidRadioButton.Name = "trainingAndValidRadioButton";
            this.trainingAndValidRadioButton.Size = new System.Drawing.Size(145, 19);
            this.trainingAndValidRadioButton.TabIndex = 53;
            this.trainingAndValidRadioButton.TabStop = true;
            this.trainingAndValidRadioButton.Text = "Training and validation";
            this.trainingAndValidRadioButton.UseVisualStyleBackColor = true;
            this.trainingAndValidRadioButton.CheckedChanged += new System.EventHandler(this.trainingAndValidRadioButton_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.trainingAndValidRadioButton);
            this.panel1.Controls.Add(this.fastValidadioButton);
            this.panel1.Location = new System.Drawing.Point(9, 11);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(178, 47);
            this.panel1.TabIndex = 55;
            // 
            // instrucitonLabel
            // 
            this.instrucitonLabel.AutoSize = true;
            this.instrucitonLabel.Location = new System.Drawing.Point(8, 59);
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
            this.ValDataFlowLayoutPanel.Location = new System.Drawing.Point(4, 159);
            this.ValDataFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ValDataFlowLayoutPanel.Name = "ValDataFlowLayoutPanel";
            this.ValDataFlowLayoutPanel.Size = new System.Drawing.Size(817, 519);
            this.ValDataFlowLayoutPanel.TabIndex = 35;
            // 
            // ValDataSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 732);
            this.Controls.Add(this.ValDataFlowLayoutPanel);
            this.Controls.Add(this.instrucitonLabel);
            this.Controls.Add(this.crossValidationRadioButton);
            this.Controls.Add(this.holdoutValidationRadioButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.applyChangesButton);
            this.Controls.Add(this.orderLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.separation1Label);
            this.Controls.Add(this.categoryLabel);
            this.Controls.Add(this.numberOfFoldsLabel);
            this.Controls.Add(this.trainingSetRatioTextBox);
            this.Controls.Add(this.numberOfFoldsTextBox);
            this.Controls.Add(this.shuffleButton);
            this.Controls.Add(this.quantizationStepLabel);
            this.Controls.Add(this.trainingSetRatioLabel);
            this.Controls.Add(this.startValidationButton);
            this.Controls.Add(this.samplingRateLabel);
            this.Controls.Add(this.startingIndexLabel);
            this.Controls.Add(this.signalNameLabel);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ValDataSelectionForm";
            this.Text = "ValidationDataSelectionForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ValDataSelectionForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ValDataSelectionForm_KeyUp);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label quantizationStepLabel;
        public System.Windows.Forms.Label trainingSetRatioLabel;
        public System.Windows.Forms.Button startValidationButton;
        private System.Windows.Forms.Label samplingRateLabel;
        private System.Windows.Forms.Label startingIndexLabel;
        private System.Windows.Forms.Label signalNameLabel;
        private System.Windows.Forms.RadioButton crossValidationRadioButton;
        private System.Windows.Forms.RadioButton holdoutValidationRadioButton;
        public System.Windows.Forms.Button shuffleButton;
        private System.Windows.Forms.TextBox numberOfFoldsTextBox;
        private System.Windows.Forms.TextBox trainingSetRatioTextBox;
        public System.Windows.Forms.Label numberOfFoldsLabel;
        public System.Windows.Forms.Label categoryLabel;
        private System.Windows.Forms.Label separation1Label;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label orderLabel;
        public System.Windows.Forms.Button applyChangesButton;
        private System.Windows.Forms.RadioButton fastValidadioButton;
        private System.Windows.Forms.RadioButton trainingAndValidRadioButton;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Label instrucitonLabel;
        private System.Windows.Forms.FlowLayoutPanel ValDataFlowLayoutPanel;
    }
}
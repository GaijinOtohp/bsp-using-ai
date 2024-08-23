
namespace BSP_Using_AI.AITools.Details
{
    partial class DetailsForm
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
            this.trainingsDetailsLabel = new System.Windows.Forms.Label();
            this.trainingsDetailsListBox = new System.Windows.Forms.ListBox();
            this.separatorLabel = new System.Windows.Forms.Label();
            this.validationDetailsLabel = new System.Windows.Forms.Label();
            this.selectValidationDataButton = new System.Windows.Forms.Button();
            this.validationProgressBar = new System.Windows.Forms.ProgressBar();
            this.timeToFinishLabel = new System.Windows.Forms.Label();
            this.resultsLabel = new System.Windows.Forms.Label();
            this.signalsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.samplingRateLabel = new System.Windows.Forms.Label();
            this.startingIndexLabel = new System.Windows.Forms.Label();
            this.signalNameLabel = new System.Windows.Forms.Label();
            this.overallTPLabel = new System.Windows.Forms.Label();
            this.overallTNLabel = new System.Windows.Forms.Label();
            this.overallFPLabel = new System.Windows.Forms.Label();
            this.overallFNLabel = new System.Windows.Forms.Label();
            this.validationFlowLayoutPanelUserControlTitles1 = new BSP_Using_AI.AITools.Details.ValidationFlowLayoutPanelUserControlTitles();
            this.validationFlowLayoutPanel = new BSP_Using_AI.CustomFlowLayoutPanel();
            this.overallMASELabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // trainingsDetailsLabel
            // 
            this.trainingsDetailsLabel.AutoSize = true;
            this.trainingsDetailsLabel.Location = new System.Drawing.Point(14, 10);
            this.trainingsDetailsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.trainingsDetailsLabel.Name = "trainingsDetailsLabel";
            this.trainingsDetailsLabel.Size = new System.Drawing.Size(91, 15);
            this.trainingsDetailsLabel.TabIndex = 0;
            this.trainingsDetailsLabel.Text = "Trainings details";
            // 
            // trainingsDetailsListBox
            // 
            this.trainingsDetailsListBox.FormattingEnabled = true;
            this.trainingsDetailsListBox.ItemHeight = 15;
            this.trainingsDetailsListBox.Location = new System.Drawing.Point(14, 29);
            this.trainingsDetailsListBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.trainingsDetailsListBox.Name = "trainingsDetailsListBox";
            this.trainingsDetailsListBox.Size = new System.Drawing.Size(249, 79);
            this.trainingsDetailsListBox.TabIndex = 1;
            // 
            // separatorLabel
            // 
            this.separatorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separatorLabel.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.separatorLabel.Location = new System.Drawing.Point(6, 263);
            this.separatorLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.separatorLabel.Name = "separatorLabel";
            this.separatorLabel.Size = new System.Drawing.Size(1224, 1);
            this.separatorLabel.TabIndex = 22;
            // 
            // validationDetailsLabel
            // 
            this.validationDetailsLabel.AutoSize = true;
            this.validationDetailsLabel.Location = new System.Drawing.Point(14, 268);
            this.validationDetailsLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.validationDetailsLabel.Name = "validationDetailsLabel";
            this.validationDetailsLabel.Size = new System.Drawing.Size(96, 15);
            this.validationDetailsLabel.TabIndex = 23;
            this.validationDetailsLabel.Text = "Validation details";
            // 
            // selectValidationDataButton
            // 
            this.selectValidationDataButton.Location = new System.Drawing.Point(590, 288);
            this.selectValidationDataButton.Margin = new System.Windows.Forms.Padding(2);
            this.selectValidationDataButton.Name = "selectValidationDataButton";
            this.selectValidationDataButton.Size = new System.Drawing.Size(136, 28);
            this.selectValidationDataButton.TabIndex = 25;
            this.selectValidationDataButton.Text = "Select validation data";
            this.selectValidationDataButton.UseVisualStyleBackColor = true;
            this.selectValidationDataButton.Click += new System.EventHandler(this.validateButton_Click);
            // 
            // validationProgressBar
            // 
            this.validationProgressBar.BackColor = System.Drawing.SystemColors.Control;
            this.validationProgressBar.Location = new System.Drawing.Point(9, 290);
            this.validationProgressBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.validationProgressBar.Name = "validationProgressBar";
            this.validationProgressBar.Size = new System.Drawing.Size(533, 27);
            this.validationProgressBar.TabIndex = 24;
            // 
            // timeToFinishLabel
            // 
            this.timeToFinishLabel.AutoSize = true;
            this.timeToFinishLabel.Location = new System.Drawing.Point(14, 323);
            this.timeToFinishLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.timeToFinishLabel.Name = "timeToFinishLabel";
            this.timeToFinishLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.timeToFinishLabel.Size = new System.Drawing.Size(82, 15);
            this.timeToFinishLabel.TabIndex = 26;
            this.timeToFinishLabel.Text = "Time to finish:";
            // 
            // resultsLabel
            // 
            this.resultsLabel.AutoSize = true;
            this.resultsLabel.Location = new System.Drawing.Point(14, 358);
            this.resultsLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.resultsLabel.Name = "resultsLabel";
            this.resultsLabel.Size = new System.Drawing.Size(44, 15);
            this.resultsLabel.TabIndex = 27;
            this.resultsLabel.Text = "Results";
            // 
            // signalsFlowLayoutPanel
            // 
            this.signalsFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.signalsFlowLayoutPanel.AutoScroll = true;
            this.signalsFlowLayoutPanel.Location = new System.Drawing.Point(285, 55);
            this.signalsFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.signalsFlowLayoutPanel.Name = "signalsFlowLayoutPanel";
            this.signalsFlowLayoutPanel.Size = new System.Drawing.Size(859, 204);
            this.signalsFlowLayoutPanel.TabIndex = 31;
            // 
            // samplingRateLabel
            // 
            this.samplingRateLabel.Location = new System.Drawing.Point(666, 29);
            this.samplingRateLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.samplingRateLabel.Name = "samplingRateLabel";
            this.samplingRateLabel.Size = new System.Drawing.Size(102, 27);
            this.samplingRateLabel.TabIndex = 34;
            this.samplingRateLabel.Text = "Sampling rate";
            this.samplingRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // startingIndexLabel
            // 
            this.startingIndexLabel.Location = new System.Drawing.Point(533, 29);
            this.startingIndexLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.startingIndexLabel.Name = "startingIndexLabel";
            this.startingIndexLabel.Size = new System.Drawing.Size(126, 27);
            this.startingIndexLabel.TabIndex = 33;
            this.startingIndexLabel.Text = "Starting index (sec)";
            this.startingIndexLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // signalNameLabel
            // 
            this.signalNameLabel.AutoSize = true;
            this.signalNameLabel.Location = new System.Drawing.Point(285, 29);
            this.signalNameLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.signalNameLabel.MaximumSize = new System.Drawing.Size(190, 0);
            this.signalNameLabel.MinimumSize = new System.Drawing.Size(190, 27);
            this.signalNameLabel.Name = "signalNameLabel";
            this.signalNameLabel.Size = new System.Drawing.Size(190, 27);
            this.signalNameLabel.TabIndex = 32;
            this.signalNameLabel.Text = "Signal name";
            this.signalNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // overallTPLabel
            // 
            this.overallTPLabel.AutoSize = true;
            this.overallTPLabel.Location = new System.Drawing.Point(6, 855);
            this.overallTPLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.overallTPLabel.MinimumSize = new System.Drawing.Size(160, 0);
            this.overallTPLabel.Name = "overallTPLabel";
            this.overallTPLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.overallTPLabel.Size = new System.Drawing.Size(160, 15);
            this.overallTPLabel.TabIndex = 39;
            this.overallTPLabel.Text = "Overall true positives:";
            // 
            // overallTNLabel
            // 
            this.overallTNLabel.AutoSize = true;
            this.overallTNLabel.Location = new System.Drawing.Point(174, 855);
            this.overallTNLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.overallTNLabel.MinimumSize = new System.Drawing.Size(160, 0);
            this.overallTNLabel.Name = "overallTNLabel";
            this.overallTNLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.overallTNLabel.Size = new System.Drawing.Size(160, 15);
            this.overallTNLabel.TabIndex = 40;
            this.overallTNLabel.Text = "Overall true negatives:";
            // 
            // overallFPLabel
            // 
            this.overallFPLabel.AutoSize = true;
            this.overallFPLabel.Location = new System.Drawing.Point(342, 855);
            this.overallFPLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.overallFPLabel.MinimumSize = new System.Drawing.Size(160, 0);
            this.overallFPLabel.Name = "overallFPLabel";
            this.overallFPLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.overallFPLabel.Size = new System.Drawing.Size(160, 15);
            this.overallFPLabel.TabIndex = 41;
            this.overallFPLabel.Text = "Overall false positives:";
            // 
            // overallFNLabel
            // 
            this.overallFNLabel.AutoSize = true;
            this.overallFNLabel.Location = new System.Drawing.Point(510, 855);
            this.overallFNLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.overallFNLabel.MinimumSize = new System.Drawing.Size(160, 0);
            this.overallFNLabel.Name = "overallFNLabel";
            this.overallFNLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.overallFNLabel.Size = new System.Drawing.Size(160, 15);
            this.overallFNLabel.TabIndex = 42;
            this.overallFNLabel.Text = "Overall false negatives:";
            // 
            // validationFlowLayoutPanelUserControlTitles1
            // 
            this.validationFlowLayoutPanelUserControlTitles1.Location = new System.Drawing.Point(9, 380);
            this.validationFlowLayoutPanelUserControlTitles1.Margin = new System.Windows.Forms.Padding(5);
            this.validationFlowLayoutPanelUserControlTitles1.Name = "validationFlowLayoutPanelUserControlTitles1";
            this.validationFlowLayoutPanelUserControlTitles1.Size = new System.Drawing.Size(1190, 35);
            this.validationFlowLayoutPanelUserControlTitles1.TabIndex = 38;
            // 
            // validationFlowLayoutPanel
            // 
            this.validationFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.validationFlowLayoutPanel.AutoScroll = true;
            this.validationFlowLayoutPanel.Location = new System.Drawing.Point(9, 421);
            this.validationFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.validationFlowLayoutPanel.Name = "validationFlowLayoutPanel";
            this.validationFlowLayoutPanel.Size = new System.Drawing.Size(1220, 427);
            this.validationFlowLayoutPanel.TabIndex = 30;
            // 
            // overallMASELabel
            // 
            this.overallMASELabel.AutoSize = true;
            this.overallMASELabel.Location = new System.Drawing.Point(678, 855);
            this.overallMASELabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.overallMASELabel.MinimumSize = new System.Drawing.Size(160, 0);
            this.overallMASELabel.Name = "overallMASELabel";
            this.overallMASELabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.overallMASELabel.Size = new System.Drawing.Size(160, 15);
            this.overallMASELabel.TabIndex = 43;
            this.overallMASELabel.Text = "Overall MASE:";
            // 
            // DetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1237, 930);
            this.Controls.Add(this.overallMASELabel);
            this.Controls.Add(this.overallFNLabel);
            this.Controls.Add(this.overallFPLabel);
            this.Controls.Add(this.overallTNLabel);
            this.Controls.Add(this.overallTPLabel);
            this.Controls.Add(this.validationFlowLayoutPanelUserControlTitles1);
            this.Controls.Add(this.samplingRateLabel);
            this.Controls.Add(this.startingIndexLabel);
            this.Controls.Add(this.signalNameLabel);
            this.Controls.Add(this.signalsFlowLayoutPanel);
            this.Controls.Add(this.validationFlowLayoutPanel);
            this.Controls.Add(this.resultsLabel);
            this.Controls.Add(this.timeToFinishLabel);
            this.Controls.Add(this.selectValidationDataButton);
            this.Controls.Add(this.validationProgressBar);
            this.Controls.Add(this.validationDetailsLabel);
            this.Controls.Add(this.separatorLabel);
            this.Controls.Add(this.trainingsDetailsListBox);
            this.Controls.Add(this.trainingsDetailsLabel);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "DetailsForm";
            this.Text = "DetailsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label trainingsDetailsLabel;
        private System.Windows.Forms.Label separatorLabel;
        private System.Windows.Forms.Label validationDetailsLabel;
        private System.Windows.Forms.Button selectValidationDataButton;
        public System.Windows.Forms.ProgressBar validationProgressBar;
        private System.Windows.Forms.Label timeToFinishLabel;
        private System.Windows.Forms.Label resultsLabel;
        private CustomFlowLayoutPanel validationFlowLayoutPanel;
        public System.Windows.Forms.ListBox trainingsDetailsListBox;
        private System.Windows.Forms.FlowLayoutPanel signalsFlowLayoutPanel;
        private System.Windows.Forms.Label samplingRateLabel;
        private System.Windows.Forms.Label startingIndexLabel;
        private System.Windows.Forms.Label signalNameLabel;
        private ValidationFlowLayoutPanelUserControlTitles validationFlowLayoutPanelUserControlTitles1;
        private System.Windows.Forms.Label overallTPLabel;
        private System.Windows.Forms.Label overallTNLabel;
        private System.Windows.Forms.Label overallFPLabel;
        private System.Windows.Forms.Label overallFNLabel;
        private System.Windows.Forms.Label overallMASELabel;
    }
}
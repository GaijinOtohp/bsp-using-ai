namespace Biological_Signal_Processing_Using_AI.DetailsModify.ExportToWFDB
{
    partial class ExportToWFDBForm
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
            this.annotationFormatLabel = new System.Windows.Forms.Label();
            this.annotationFormatsComboBox = new System.Windows.Forms.ComboBox();
            this.signalFormatLabel = new System.Windows.Forms.Label();
            this.signalFormatsComboBox = new System.Windows.Forms.ComboBox();
            this.exportButton = new System.Windows.Forms.Button();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.quantizationStepTextBox = new System.Windows.Forms.TextBox();
            this.quantizationStepLabel = new System.Windows.Forms.Label();
            this.samplingRateTextBox = new System.Windows.Forms.TextBox();
            this.samplingRteLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // annotationFormatLabel
            // 
            this.annotationFormatLabel.AutoSize = true;
            this.annotationFormatLabel.Location = new System.Drawing.Point(13, 48);
            this.annotationFormatLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.annotationFormatLabel.MinimumSize = new System.Drawing.Size(100, 0);
            this.annotationFormatLabel.Name = "annotationFormatLabel";
            this.annotationFormatLabel.Size = new System.Drawing.Size(108, 15);
            this.annotationFormatLabel.TabIndex = 43;
            this.annotationFormatLabel.Text = "Annotation Format";
            // 
            // annotationFormatsComboBox
            // 
            this.annotationFormatsComboBox.DisplayMember = "1";
            this.annotationFormatsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.annotationFormatsComboBox.DropDownWidth = 140;
            this.annotationFormatsComboBox.FormattingEnabled = true;
            this.annotationFormatsComboBox.Items.AddRange(new object[] {
            "MIT"});
            this.annotationFormatsComboBox.Location = new System.Drawing.Point(122, 45);
            this.annotationFormatsComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.annotationFormatsComboBox.Name = "annotationFormatsComboBox";
            this.annotationFormatsComboBox.Size = new System.Drawing.Size(150, 23);
            this.annotationFormatsComboBox.TabIndex = 42;
            this.annotationFormatsComboBox.Tag = "";
            // 
            // signalFormatLabel
            // 
            this.signalFormatLabel.AutoSize = true;
            this.signalFormatLabel.Location = new System.Drawing.Point(13, 21);
            this.signalFormatLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.signalFormatLabel.MinimumSize = new System.Drawing.Size(100, 0);
            this.signalFormatLabel.Name = "signalFormatLabel";
            this.signalFormatLabel.Size = new System.Drawing.Size(100, 15);
            this.signalFormatLabel.TabIndex = 41;
            this.signalFormatLabel.Text = "Signal Format";
            // 
            // signalFormatsComboBox
            // 
            this.signalFormatsComboBox.DisplayMember = "1";
            this.signalFormatsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.signalFormatsComboBox.DropDownWidth = 140;
            this.signalFormatsComboBox.FormattingEnabled = true;
            this.signalFormatsComboBox.Items.AddRange(new object[] {
            "212"});
            this.signalFormatsComboBox.Location = new System.Drawing.Point(122, 18);
            this.signalFormatsComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.signalFormatsComboBox.Name = "signalFormatsComboBox";
            this.signalFormatsComboBox.Size = new System.Drawing.Size(150, 23);
            this.signalFormatsComboBox.TabIndex = 40;
            this.signalFormatsComboBox.Tag = "";
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(193, 185);
            this.exportButton.Margin = new System.Windows.Forms.Padding(2);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(79, 30);
            this.exportButton.TabIndex = 44;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.Location = new System.Drawing.Point(121, 138);
            this.descriptionTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.Size = new System.Drawing.Size(151, 23);
            this.descriptionTextBox.TabIndex = 50;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Location = new System.Drawing.Point(13, 142);
            this.descriptionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.descriptionLabel.MinimumSize = new System.Drawing.Size(100, 0);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(100, 15);
            this.descriptionLabel.TabIndex = 49;
            this.descriptionLabel.Text = "Description";
            // 
            // quantizationStepTextBox
            // 
            this.quantizationStepTextBox.Location = new System.Drawing.Point(183, 109);
            this.quantizationStepTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.quantizationStepTextBox.Name = "quantizationStepTextBox";
            this.quantizationStepTextBox.ReadOnly = true;
            this.quantizationStepTextBox.Size = new System.Drawing.Size(89, 23);
            this.quantizationStepTextBox.TabIndex = 48;
            // 
            // quantizationStepLabel
            // 
            this.quantizationStepLabel.AutoSize = true;
            this.quantizationStepLabel.Location = new System.Drawing.Point(13, 113);
            this.quantizationStepLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.quantizationStepLabel.MinimumSize = new System.Drawing.Size(163, 0);
            this.quantizationStepLabel.Name = "quantizationStepLabel";
            this.quantizationStepLabel.Size = new System.Drawing.Size(163, 15);
            this.quantizationStepLabel.TabIndex = 47;
            this.quantizationStepLabel.Text = "Quantization step (adu/mV)";
            // 
            // samplingRateTextBox
            // 
            this.samplingRateTextBox.Location = new System.Drawing.Point(183, 80);
            this.samplingRateTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.samplingRateTextBox.Name = "samplingRateTextBox";
            this.samplingRateTextBox.ReadOnly = true;
            this.samplingRateTextBox.Size = new System.Drawing.Size(89, 23);
            this.samplingRateTextBox.TabIndex = 46;
            // 
            // samplingRteLabel
            // 
            this.samplingRteLabel.AutoSize = true;
            this.samplingRteLabel.Location = new System.Drawing.Point(13, 83);
            this.samplingRteLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.samplingRteLabel.MinimumSize = new System.Drawing.Size(163, 0);
            this.samplingRteLabel.Name = "samplingRteLabel";
            this.samplingRteLabel.Size = new System.Drawing.Size(163, 15);
            this.samplingRteLabel.TabIndex = 45;
            this.samplingRteLabel.Text = "Sampling rate (Hz)";
            // 
            // ExportToWFDBForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(285, 237);
            this.Controls.Add(this.descriptionTextBox);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.quantizationStepTextBox);
            this.Controls.Add(this.quantizationStepLabel);
            this.Controls.Add(this.samplingRateTextBox);
            this.Controls.Add(this.samplingRteLabel);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.annotationFormatLabel);
            this.Controls.Add(this.annotationFormatsComboBox);
            this.Controls.Add(this.signalFormatLabel);
            this.Controls.Add(this.signalFormatsComboBox);
            this.Name = "ExportToWFDBForm";
            this.Text = "ExportToWFDBForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label annotationFormatLabel;
        public System.Windows.Forms.ComboBox annotationFormatsComboBox;
        private System.Windows.Forms.Label signalFormatLabel;
        public System.Windows.Forms.ComboBox signalFormatsComboBox;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.TextBox quantizationStepTextBox;
        private System.Windows.Forms.Label quantizationStepLabel;
        private System.Windows.Forms.TextBox samplingRateTextBox;
        private System.Windows.Forms.Label samplingRteLabel;
    }
}
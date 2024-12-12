namespace Biological_Signal_Processing_Using_AI.AITools.DatasetExplorer.WFDBExplorer
{
    partial class WFDBExplorerForm
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
            this.quantizationStepTextBox = new System.Windows.Forms.TextBox();
            this.quantizationStepLabel = new System.Windows.Forms.Label();
            this.samplingRateTextBox = new System.Windows.Forms.TextBox();
            this.samplingRteLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.signalsComboBox = new System.Windows.Forms.ComboBox();
            this.signalLabel = new System.Windows.Forms.Label();
            this.annotationLabel = new System.Windows.Forms.Label();
            this.annotationsComboBox = new System.Windows.Forms.ComboBox();
            this.informationLabel = new System.Windows.Forms.Label();
            this.chooseFileButton = new System.Windows.Forms.Button();
            this.signalStartLabel = new System.Windows.Forms.Label();
            this.signalStartTextBox = new System.Windows.Forms.TextBox();
            this.signalEndLabel = new System.Windows.Forms.Label();
            this.signalEndTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // quantizationStepTextBox
            // 
            this.quantizationStepTextBox.Location = new System.Drawing.Point(183, 181);
            this.quantizationStepTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.quantizationStepTextBox.Name = "quantizationStepTextBox";
            this.quantizationStepTextBox.ReadOnly = true;
            this.quantizationStepTextBox.Size = new System.Drawing.Size(89, 23);
            this.quantizationStepTextBox.TabIndex = 9;
            // 
            // quantizationStepLabel
            // 
            this.quantizationStepLabel.AutoSize = true;
            this.quantizationStepLabel.Location = new System.Drawing.Point(13, 185);
            this.quantizationStepLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.quantizationStepLabel.MinimumSize = new System.Drawing.Size(163, 0);
            this.quantizationStepLabel.Name = "quantizationStepLabel";
            this.quantizationStepLabel.Size = new System.Drawing.Size(163, 15);
            this.quantizationStepLabel.TabIndex = 8;
            this.quantizationStepLabel.Text = "Quantization step (adu/mV)";
            // 
            // samplingRateTextBox
            // 
            this.samplingRateTextBox.Location = new System.Drawing.Point(183, 152);
            this.samplingRateTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.samplingRateTextBox.Name = "samplingRateTextBox";
            this.samplingRateTextBox.ReadOnly = true;
            this.samplingRateTextBox.Size = new System.Drawing.Size(89, 23);
            this.samplingRateTextBox.TabIndex = 7;
            // 
            // samplingRteLabel
            // 
            this.samplingRteLabel.AutoSize = true;
            this.samplingRteLabel.Location = new System.Drawing.Point(13, 155);
            this.samplingRteLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.samplingRteLabel.MinimumSize = new System.Drawing.Size(163, 0);
            this.samplingRteLabel.Name = "samplingRteLabel";
            this.samplingRteLabel.Size = new System.Drawing.Size(163, 15);
            this.samplingRteLabel.TabIndex = 6;
            this.samplingRteLabel.Text = "Sampling rate (Hz)";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(193, 322);
            this.okButton.Margin = new System.Windows.Forms.Padding(2);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(79, 30);
            this.okButton.TabIndex = 10;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.Location = new System.Drawing.Point(121, 210);
            this.descriptionTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.ReadOnly = true;
            this.descriptionTextBox.Size = new System.Drawing.Size(151, 23);
            this.descriptionTextBox.TabIndex = 12;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Location = new System.Drawing.Point(13, 214);
            this.descriptionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.descriptionLabel.MinimumSize = new System.Drawing.Size(100, 0);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(100, 15);
            this.descriptionLabel.TabIndex = 11;
            this.descriptionLabel.Text = "Description";
            // 
            // signalsComboBox
            // 
            this.signalsComboBox.DisplayMember = "1";
            this.signalsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.signalsComboBox.DropDownWidth = 140;
            this.signalsComboBox.FormattingEnabled = true;
            this.signalsComboBox.Location = new System.Drawing.Point(122, 75);
            this.signalsComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.signalsComboBox.Name = "signalsComboBox";
            this.signalsComboBox.Size = new System.Drawing.Size(150, 23);
            this.signalsComboBox.TabIndex = 36;
            this.signalsComboBox.Tag = "";
            this.signalsComboBox.SelectedIndexChanged += new System.EventHandler(this.signalsComboBox_SelectedIndexChanged);
            // 
            // signalLabel
            // 
            this.signalLabel.AutoSize = true;
            this.signalLabel.Location = new System.Drawing.Point(13, 78);
            this.signalLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.signalLabel.MinimumSize = new System.Drawing.Size(100, 0);
            this.signalLabel.Name = "signalLabel";
            this.signalLabel.Size = new System.Drawing.Size(100, 15);
            this.signalLabel.TabIndex = 37;
            this.signalLabel.Text = "Signal";
            // 
            // annotationLabel
            // 
            this.annotationLabel.AutoSize = true;
            this.annotationLabel.Location = new System.Drawing.Point(13, 105);
            this.annotationLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.annotationLabel.MinimumSize = new System.Drawing.Size(100, 0);
            this.annotationLabel.Name = "annotationLabel";
            this.annotationLabel.Size = new System.Drawing.Size(100, 15);
            this.annotationLabel.TabIndex = 39;
            this.annotationLabel.Text = "Annotation";
            // 
            // annotationsComboBox
            // 
            this.annotationsComboBox.DisplayMember = "1";
            this.annotationsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.annotationsComboBox.DropDownWidth = 140;
            this.annotationsComboBox.FormattingEnabled = true;
            this.annotationsComboBox.Location = new System.Drawing.Point(122, 102);
            this.annotationsComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.annotationsComboBox.Name = "annotationsComboBox";
            this.annotationsComboBox.Size = new System.Drawing.Size(150, 23);
            this.annotationsComboBox.TabIndex = 38;
            this.annotationsComboBox.Tag = "";
            // 
            // informationLabel
            // 
            this.informationLabel.AutoSize = true;
            this.informationLabel.Location = new System.Drawing.Point(13, 9);
            this.informationLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.informationLabel.MaximumSize = new System.Drawing.Size(100, 0);
            this.informationLabel.MinimumSize = new System.Drawing.Size(163, 0);
            this.informationLabel.Name = "informationLabel";
            this.informationLabel.Size = new System.Drawing.Size(163, 30);
            this.informationLabel.TabIndex = 40;
            this.informationLabel.Text = "Choose any file from the identical signal files.";
            // 
            // chooseFileButton
            // 
            this.chooseFileButton.Location = new System.Drawing.Point(182, 11);
            this.chooseFileButton.Margin = new System.Windows.Forms.Padding(2);
            this.chooseFileButton.Name = "chooseFileButton";
            this.chooseFileButton.Size = new System.Drawing.Size(90, 30);
            this.chooseFileButton.TabIndex = 41;
            this.chooseFileButton.Text = "Choose File";
            this.chooseFileButton.UseVisualStyleBackColor = true;
            this.chooseFileButton.Click += new System.EventHandler(this.chooseFileButton_Click);
            // 
            // signalStartLabel
            // 
            this.signalStartLabel.AutoSize = true;
            this.signalStartLabel.Location = new System.Drawing.Point(13, 258);
            this.signalStartLabel.Name = "signalStartLabel";
            this.signalStartLabel.Size = new System.Drawing.Size(94, 15);
            this.signalStartLabel.TabIndex = 43;
            this.signalStartLabel.Text = "Signal start (Sec)";
            // 
            // signalStartTextBox
            // 
            this.signalStartTextBox.Location = new System.Drawing.Point(13, 276);
            this.signalStartTextBox.Name = "signalStartTextBox";
            this.signalStartTextBox.PlaceholderText = "0";
            this.signalStartTextBox.Size = new System.Drawing.Size(96, 23);
            this.signalStartTextBox.TabIndex = 42;
            this.signalStartTextBox.Text = "0";
            this.signalStartTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.signalStartTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.signalStartTextBox_KeyPress);
            // 
            // signalEndLabel
            // 
            this.signalEndLabel.AutoSize = true;
            this.signalEndLabel.Location = new System.Drawing.Point(177, 258);
            this.signalEndLabel.Name = "signalEndLabel";
            this.signalEndLabel.Size = new System.Drawing.Size(91, 15);
            this.signalEndLabel.TabIndex = 45;
            this.signalEndLabel.Text = "Signal end (Sec)";
            // 
            // signalEndTextBox
            // 
            this.signalEndTextBox.Location = new System.Drawing.Point(177, 276);
            this.signalEndTextBox.Name = "signalEndTextBox";
            this.signalEndTextBox.PlaceholderText = "0";
            this.signalEndTextBox.Size = new System.Drawing.Size(96, 23);
            this.signalEndTextBox.TabIndex = 44;
            this.signalEndTextBox.Text = "30";
            this.signalEndTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.signalEndTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.signalStartTextBox_KeyPress);
            // 
            // WFDBExplorerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 363);
            this.Controls.Add(this.signalEndLabel);
            this.Controls.Add(this.signalEndTextBox);
            this.Controls.Add(this.signalStartLabel);
            this.Controls.Add(this.signalStartTextBox);
            this.Controls.Add(this.chooseFileButton);
            this.Controls.Add(this.informationLabel);
            this.Controls.Add(this.annotationLabel);
            this.Controls.Add(this.annotationsComboBox);
            this.Controls.Add(this.signalLabel);
            this.Controls.Add(this.signalsComboBox);
            this.Controls.Add(this.descriptionTextBox);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.quantizationStepTextBox);
            this.Controls.Add(this.quantizationStepLabel);
            this.Controls.Add(this.samplingRateTextBox);
            this.Controls.Add(this.samplingRteLabel);
            this.Controls.Add(this.okButton);
            this.Name = "WFDBExplorerForm";
            this.Text = "WFDBExplorerForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox quantizationStepTextBox;
        private System.Windows.Forms.Label quantizationStepLabel;
        private System.Windows.Forms.TextBox samplingRateTextBox;
        private System.Windows.Forms.Label samplingRteLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.Label descriptionLabel;
        public System.Windows.Forms.ComboBox signalsComboBox;
        private System.Windows.Forms.Label signalLabel;
        private System.Windows.Forms.Label annotationLabel;
        public System.Windows.Forms.ComboBox annotationsComboBox;
        private System.Windows.Forms.Label informationLabel;
        private System.Windows.Forms.Button chooseFileButton;
        private System.Windows.Forms.Label signalStartLabel;
        private System.Windows.Forms.TextBox signalStartTextBox;
        private System.Windows.Forms.Label signalEndLabel;
        private System.Windows.Forms.TextBox signalEndTextBox;
    }
}
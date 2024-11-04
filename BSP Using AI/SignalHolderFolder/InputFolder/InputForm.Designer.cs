
namespace BSP_Using_AI.SignalHolderFolder.Input
{
    partial class InputForm
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
            this.titleLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.samplingRteLabel = new System.Windows.Forms.Label();
            this.samplingRateTextBox = new System.Windows.Forms.TextBox();
            this.quantizationStepTextBox = new System.Windows.Forms.TextBox();
            this.quantizationStepLabel = new System.Windows.Forms.Label();
            this.signalLabel = new System.Windows.Forms.Label();
            this.signalsComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.Location = new System.Drawing.Point(10, 8);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(270, 45);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Insert the sampling rate and the quantization of the chosen singal";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(197, 158);
            this.okButton.Margin = new System.Windows.Forms.Padding(2);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(79, 30);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // samplingRteLabel
            // 
            this.samplingRteLabel.AutoSize = true;
            this.samplingRteLabel.Location = new System.Drawing.Point(10, 97);
            this.samplingRteLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.samplingRteLabel.MinimumSize = new System.Drawing.Size(163, 0);
            this.samplingRteLabel.Name = "samplingRteLabel";
            this.samplingRteLabel.Size = new System.Drawing.Size(163, 15);
            this.samplingRteLabel.TabIndex = 1;
            this.samplingRteLabel.Text = "Sampling rate (Hz)";
            // 
            // samplingRateTextBox
            // 
            this.samplingRateTextBox.Location = new System.Drawing.Point(180, 94);
            this.samplingRateTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.samplingRateTextBox.Name = "samplingRateTextBox";
            this.samplingRateTextBox.Size = new System.Drawing.Size(89, 23);
            this.samplingRateTextBox.TabIndex = 2;
            this.samplingRateTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.samplingRateTextBox_KeyPress);
            // 
            // quantizationStepTextBox
            // 
            this.quantizationStepTextBox.Location = new System.Drawing.Point(180, 121);
            this.quantizationStepTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.quantizationStepTextBox.Name = "quantizationStepTextBox";
            this.quantizationStepTextBox.Size = new System.Drawing.Size(89, 23);
            this.quantizationStepTextBox.TabIndex = 4;
            this.quantizationStepTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.samplingRateTextBox_KeyPress);
            // 
            // quantizationStepLabel
            // 
            this.quantizationStepLabel.AutoSize = true;
            this.quantizationStepLabel.Location = new System.Drawing.Point(10, 125);
            this.quantizationStepLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.quantizationStepLabel.MinimumSize = new System.Drawing.Size(163, 0);
            this.quantizationStepLabel.Name = "quantizationStepLabel";
            this.quantizationStepLabel.Size = new System.Drawing.Size(163, 15);
            this.quantizationStepLabel.TabIndex = 3;
            this.quantizationStepLabel.Text = "Quantization step (adu/mV)";
            // 
            // signalLabel
            // 
            this.signalLabel.AutoSize = true;
            this.signalLabel.Location = new System.Drawing.Point(10, 53);
            this.signalLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.signalLabel.MinimumSize = new System.Drawing.Size(100, 0);
            this.signalLabel.Name = "signalLabel";
            this.signalLabel.Size = new System.Drawing.Size(100, 15);
            this.signalLabel.TabIndex = 39;
            this.signalLabel.Text = "Signal";
            // 
            // signalsComboBox
            // 
            this.signalsComboBox.DisplayMember = "1";
            this.signalsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.signalsComboBox.DropDownWidth = 140;
            this.signalsComboBox.FormattingEnabled = true;
            this.signalsComboBox.Location = new System.Drawing.Point(119, 50);
            this.signalsComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.signalsComboBox.Name = "signalsComboBox";
            this.signalsComboBox.Size = new System.Drawing.Size(150, 23);
            this.signalsComboBox.TabIndex = 38;
            this.signalsComboBox.Tag = "";
            this.signalsComboBox.SelectedIndexChanged += new System.EventHandler(this.signalsComboBox_SelectedIndexChanged);
            // 
            // InputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(285, 201);
            this.Controls.Add(this.signalLabel);
            this.Controls.Add(this.signalsComboBox);
            this.Controls.Add(this.quantizationStepTextBox);
            this.Controls.Add(this.quantizationStepLabel);
            this.Controls.Add(this.samplingRateTextBox);
            this.Controls.Add(this.samplingRteLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.titleLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputForm";
            this.Text = "Input values";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label samplingRteLabel;
        private System.Windows.Forms.TextBox samplingRateTextBox;
        private System.Windows.Forms.TextBox quantizationStepTextBox;
        private System.Windows.Forms.Label quantizationStepLabel;
        private System.Windows.Forms.Label signalLabel;
        public System.Windows.Forms.ComboBox signalsComboBox;
    }
}
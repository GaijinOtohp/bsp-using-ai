
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
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.Location = new System.Drawing.Point(9, 7);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(231, 39);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Insert the sampling rate and the quantization of the chosen singal";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(172, 105);
            this.okButton.Margin = new System.Windows.Forms.Padding(2);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(68, 26);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // samplingRteLabel
            // 
            this.samplingRteLabel.AutoSize = true;
            this.samplingRteLabel.Location = new System.Drawing.Point(12, 52);
            this.samplingRteLabel.MinimumSize = new System.Drawing.Size(140, 0);
            this.samplingRteLabel.Name = "samplingRteLabel";
            this.samplingRteLabel.Size = new System.Drawing.Size(140, 13);
            this.samplingRteLabel.TabIndex = 1;
            this.samplingRteLabel.Text = "Sampling rate (Hz)";
            // 
            // samplingRateTextBox
            // 
            this.samplingRateTextBox.Location = new System.Drawing.Point(158, 49);
            this.samplingRateTextBox.Name = "samplingRateTextBox";
            this.samplingRateTextBox.Size = new System.Drawing.Size(77, 20);
            this.samplingRateTextBox.TabIndex = 2;
            this.samplingRateTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.samplingRateTextBox_KeyPress);
            // 
            // quantizationStepTextBox
            // 
            this.quantizationStepTextBox.Location = new System.Drawing.Point(158, 73);
            this.quantizationStepTextBox.Name = "quantizationStepTextBox";
            this.quantizationStepTextBox.Size = new System.Drawing.Size(77, 20);
            this.quantizationStepTextBox.TabIndex = 4;
            this.quantizationStepTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.samplingRateTextBox_KeyPress);
            // 
            // quantizationStepLabel
            // 
            this.quantizationStepLabel.AutoSize = true;
            this.quantizationStepLabel.Location = new System.Drawing.Point(12, 76);
            this.quantizationStepLabel.MinimumSize = new System.Drawing.Size(140, 0);
            this.quantizationStepLabel.Name = "quantizationStepLabel";
            this.quantizationStepLabel.Size = new System.Drawing.Size(140, 13);
            this.quantizationStepLabel.TabIndex = 3;
            this.quantizationStepLabel.Text = "Quantization step (adu/mV)";
            // 
            // InputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 142);
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
    }
}
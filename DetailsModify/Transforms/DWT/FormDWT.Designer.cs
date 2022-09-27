
namespace BSP_Using_AI.DetailsModify.Transforms.DWT
{
    partial class FormDWT
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
            this.waveletTypeLabel = new System.Windows.Forms.Label();
            this.waveletTypeComboBox = new System.Windows.Forms.ComboBox();
            this.numOfVanMoComboBox = new System.Windows.Forms.ComboBox();
            this.numOfVanishingLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // waveletTypeLabel
            // 
            this.waveletTypeLabel.AutoSize = true;
            this.waveletTypeLabel.Location = new System.Drawing.Point(12, 9);
            this.waveletTypeLabel.Name = "waveletTypeLabel";
            this.waveletTypeLabel.Size = new System.Drawing.Size(90, 17);
            this.waveletTypeLabel.TabIndex = 13;
            this.waveletTypeLabel.Text = "Wavelet type";
            // 
            // waveletTypeComboBox
            // 
            this.waveletTypeComboBox.DisplayMember = "1";
            this.waveletTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.waveletTypeComboBox.FormattingEnabled = true;
            this.waveletTypeComboBox.Items.AddRange(new object[] {
            "Haar",
            "Daubechies",
            "Symlet",
            "Coiflet"});
            this.waveletTypeComboBox.Location = new System.Drawing.Point(217, 2);
            this.waveletTypeComboBox.Name = "waveletTypeComboBox";
            this.waveletTypeComboBox.Size = new System.Drawing.Size(171, 24);
            this.waveletTypeComboBox.TabIndex = 14;
            this.waveletTypeComboBox.Tag = "";
            this.waveletTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.waveletTypeComboBox_SelectedIndexChanged);
            // 
            // numOfVanMoComboBox
            // 
            this.numOfVanMoComboBox.DisplayMember = "1";
            this.numOfVanMoComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.numOfVanMoComboBox.FormattingEnabled = true;
            this.numOfVanMoComboBox.Items.AddRange(new object[] {
            "Haar",
            "Daubechies",
            "Symlet",
            "Coiflet"});
            this.numOfVanMoComboBox.Location = new System.Drawing.Point(217, 32);
            this.numOfVanMoComboBox.Name = "numOfVanMoComboBox";
            this.numOfVanMoComboBox.Size = new System.Drawing.Size(171, 24);
            this.numOfVanMoComboBox.TabIndex = 16;
            this.numOfVanMoComboBox.Tag = "";
            this.numOfVanMoComboBox.SelectedIndexChanged += new System.EventHandler(this.numOfVanMoComboBox_SelectedIndexChanged);
            // 
            // numOfVanishingLabel
            // 
            this.numOfVanishingLabel.AutoSize = true;
            this.numOfVanishingLabel.Location = new System.Drawing.Point(12, 39);
            this.numOfVanishingLabel.Name = "numOfVanishingLabel";
            this.numOfVanishingLabel.Size = new System.Drawing.Size(199, 17);
            this.numOfVanishingLabel.TabIndex = 15;
            this.numOfVanishingLabel.Text = "Number of vanishing moments";
            // 
            // FormDWT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 156);
            this.Controls.Add(this.numOfVanMoComboBox);
            this.Controls.Add(this.numOfVanishingLabel);
            this.Controls.Add(this.waveletTypeComboBox);
            this.Controls.Add(this.waveletTypeLabel);
            this.Name = "FormDWT";
            this.Text = "DWT edit";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormDWT_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label waveletTypeLabel;
        private System.Windows.Forms.ComboBox waveletTypeComboBox;
        private System.Windows.Forms.ComboBox numOfVanMoComboBox;
        private System.Windows.Forms.Label numOfVanishingLabel;
    }
}
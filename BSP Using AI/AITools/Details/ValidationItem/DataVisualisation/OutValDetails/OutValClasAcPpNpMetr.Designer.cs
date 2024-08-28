
namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation.OutValDetails
{
    partial class OutValClasAcPpNpMetr
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
            this.outputLabel = new System.Windows.Forms.Label();
            this.npvLabel = new System.Windows.Forms.Label();
            this.ppvLabel = new System.Windows.Forms.Label();
            this.accuracyLabel = new System.Windows.Forms.Label();
            this.classificationThresholdTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // outputLabel
            // 
            this.outputLabel.AutoSize = true;
            this.outputLabel.Location = new System.Drawing.Point(4, 3);
            this.outputLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.outputLabel.MaximumSize = new System.Drawing.Size(190, 0);
            this.outputLabel.MinimumSize = new System.Drawing.Size(190, 27);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(190, 27);
            this.outputLabel.TabIndex = 3;
            this.outputLabel.Text = "Output label";
            this.outputLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // npvLabel
            // 
            this.npvLabel.AutoSize = true;
            this.npvLabel.Location = new System.Drawing.Point(406, 3);
            this.npvLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.npvLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.npvLabel.MinimumSize = new System.Drawing.Size(94, 27);
            this.npvLabel.Name = "npvLabel";
            this.npvLabel.Size = new System.Drawing.Size(94, 27);
            this.npvLabel.TabIndex = 16;
            this.npvLabel.Text = "NPV";
            this.npvLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ppvLabel
            // 
            this.ppvLabel.AutoSize = true;
            this.ppvLabel.Location = new System.Drawing.Point(304, 3);
            this.ppvLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ppvLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.ppvLabel.MinimumSize = new System.Drawing.Size(94, 27);
            this.ppvLabel.Name = "ppvLabel";
            this.ppvLabel.Size = new System.Drawing.Size(94, 27);
            this.ppvLabel.TabIndex = 15;
            this.ppvLabel.Text = "PPV (Precision)";
            this.ppvLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // accuracyLabel
            // 
            this.accuracyLabel.AutoSize = true;
            this.accuracyLabel.Location = new System.Drawing.Point(202, 3);
            this.accuracyLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.accuracyLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.accuracyLabel.MinimumSize = new System.Drawing.Size(94, 27);
            this.accuracyLabel.Name = "accuracyLabel";
            this.accuracyLabel.Size = new System.Drawing.Size(94, 27);
            this.accuracyLabel.TabIndex = 14;
            this.accuracyLabel.Text = "Accuracy";
            this.accuracyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // classificationThresholdTextBox
            // 
            this.classificationThresholdTextBox.Location = new System.Drawing.Point(517, 6);
            this.classificationThresholdTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.classificationThresholdTextBox.Name = "classificationThresholdTextBox";
            this.classificationThresholdTextBox.Size = new System.Drawing.Size(160, 23);
            this.classificationThresholdTextBox.TabIndex = 17;
            this.classificationThresholdTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.classificationThresholdTextBox_KeyPress);
            // 
            // OutValClasAcPpNpMetr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.classificationThresholdTextBox);
            this.Controls.Add(this.npvLabel);
            this.Controls.Add(this.ppvLabel);
            this.Controls.Add(this.accuracyLabel);
            this.Controls.Add(this.outputLabel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "OutValClasAcPpNpMetr";
            this.Size = new System.Drawing.Size(683, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label outputLabel;
        public System.Windows.Forms.Label npvLabel;
        public System.Windows.Forms.Label ppvLabel;
        public System.Windows.Forms.Label accuracyLabel;
        public System.Windows.Forms.TextBox classificationThresholdTextBox;
    }
}


namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation.OutValDetails
{
    partial class OutValRegrRawMetrics
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
            this.maseLabel = new System.Windows.Forms.Label();
            this.outputLabel = new System.Windows.Forms.Label();
            this.classificationThresholdTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // maseLabel
            // 
            this.maseLabel.AutoSize = true;
            this.maseLabel.Location = new System.Drawing.Point(202, 3);
            this.maseLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.maseLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.maseLabel.MinimumSize = new System.Drawing.Size(60, 27);
            this.maseLabel.Name = "maseLabel";
            this.maseLabel.Size = new System.Drawing.Size(60, 27);
            this.maseLabel.TabIndex = 15;
            this.maseLabel.Text = "MASE";
            this.maseLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.outputLabel.TabIndex = 14;
            this.outputLabel.Text = "Output label";
            this.outputLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // classificationThresholdTextBox
            // 
            this.classificationThresholdTextBox.Location = new System.Drawing.Point(517, 6);
            this.classificationThresholdTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.classificationThresholdTextBox.Name = "classificationThresholdTextBox";
            this.classificationThresholdTextBox.Size = new System.Drawing.Size(160, 23);
            this.classificationThresholdTextBox.TabIndex = 16;
            this.classificationThresholdTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.classificationThresholdTextBox_KeyPress);
            // 
            // OutValRegrRawMetrics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.classificationThresholdTextBox);
            this.Controls.Add(this.maseLabel);
            this.Controls.Add(this.outputLabel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "OutValRegrRawMetrics";
            this.Size = new System.Drawing.Size(683, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label maseLabel;
        public System.Windows.Forms.Label outputLabel;
        public System.Windows.Forms.TextBox classificationThresholdTextBox;
    }
}

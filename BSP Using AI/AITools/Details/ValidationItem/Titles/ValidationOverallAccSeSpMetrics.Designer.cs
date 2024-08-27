
namespace BSP_Using_AI.AITools.Details
{
    partial class ValidationOverallAccSeSpMetrics
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
            this.overallAccuracyLabel = new System.Windows.Forms.Label();
            this.overallMASELabel = new System.Windows.Forms.Label();
            this.overallSensitivityLabel = new System.Windows.Forms.Label();
            this.overallSpecificityLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // overallAccuracyLabel
            // 
            this.overallAccuracyLabel.AutoSize = true;
            this.overallAccuracyLabel.Location = new System.Drawing.Point(4, 3);
            this.overallAccuracyLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.overallAccuracyLabel.MaximumSize = new System.Drawing.Size(190, 0);
            this.overallAccuracyLabel.MinimumSize = new System.Drawing.Size(216, 27);
            this.overallAccuracyLabel.Name = "overallAccuracyLabel";
            this.overallAccuracyLabel.Size = new System.Drawing.Size(216, 27);
            this.overallAccuracyLabel.TabIndex = 3;
            this.overallAccuracyLabel.Text = "Overall accuracy:";
            this.overallAccuracyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // overallMASELabel
            // 
            this.overallMASELabel.AutoSize = true;
            this.overallMASELabel.Location = new System.Drawing.Point(705, 3);
            this.overallMASELabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.overallMASELabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.overallMASELabel.MinimumSize = new System.Drawing.Size(160, 27);
            this.overallMASELabel.Name = "overallMASELabel";
            this.overallMASELabel.Size = new System.Drawing.Size(160, 27);
            this.overallMASELabel.TabIndex = 6;
            this.overallMASELabel.Text = "Overall MASE:";
            this.overallMASELabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // overallSensitivityLabel
            // 
            this.overallSensitivityLabel.AutoSize = true;
            this.overallSensitivityLabel.Location = new System.Drawing.Point(228, 3);
            this.overallSensitivityLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.overallSensitivityLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.overallSensitivityLabel.MinimumSize = new System.Drawing.Size(216, 27);
            this.overallSensitivityLabel.Name = "overallSensitivityLabel";
            this.overallSensitivityLabel.Size = new System.Drawing.Size(216, 27);
            this.overallSensitivityLabel.TabIndex = 4;
            this.overallSensitivityLabel.Text = "Overall sensitivity:";
            this.overallSensitivityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // overallSpecificityLabel
            // 
            this.overallSpecificityLabel.AutoSize = true;
            this.overallSpecificityLabel.Location = new System.Drawing.Point(452, 3);
            this.overallSpecificityLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.overallSpecificityLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.overallSpecificityLabel.MinimumSize = new System.Drawing.Size(216, 27);
            this.overallSpecificityLabel.Name = "overallSpecificityLabel";
            this.overallSpecificityLabel.Size = new System.Drawing.Size(216, 27);
            this.overallSpecificityLabel.TabIndex = 11;
            this.overallSpecificityLabel.Text = "Overall specificity (recall):";
            this.overallSpecificityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ValidationOverallAccSeSpMetrics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.overallSpecificityLabel);
            this.Controls.Add(this.overallSensitivityLabel);
            this.Controls.Add(this.overallMASELabel);
            this.Controls.Add(this.overallAccuracyLabel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ValidationOverallAccSeSpMetrics";
            this.Size = new System.Drawing.Size(882, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label overallAccuracyLabel;
        public System.Windows.Forms.Label overallMASELabel;
        public System.Windows.Forms.Label overallSensitivityLabel;
        public System.Windows.Forms.Label overallSpecificityLabel;
    }
}

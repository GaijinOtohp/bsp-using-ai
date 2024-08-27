
namespace BSP_Using_AI.AITools.Details
{
    partial class ValidationOverallAccPpvNpvMetrics
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
            this.overallPPVLabel = new System.Windows.Forms.Label();
            this.overallNPVLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // overallAccuracyLabel
            // 
            this.overallAccuracyLabel.AutoSize = true;
            this.overallAccuracyLabel.Location = new System.Drawing.Point(4, 3);
            this.overallAccuracyLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.overallAccuracyLabel.MaximumSize = new System.Drawing.Size(190, 0);
            this.overallAccuracyLabel.MinimumSize = new System.Drawing.Size(190, 27);
            this.overallAccuracyLabel.Name = "overallAccuracyLabel";
            this.overallAccuracyLabel.Size = new System.Drawing.Size(190, 27);
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
            // overallPPVLabel
            // 
            this.overallPPVLabel.AutoSize = true;
            this.overallPPVLabel.Location = new System.Drawing.Point(202, 3);
            this.overallPPVLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.overallPPVLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.overallPPVLabel.MinimumSize = new System.Drawing.Size(271, 27);
            this.overallPPVLabel.Name = "overallPPVLabel";
            this.overallPPVLabel.Size = new System.Drawing.Size(271, 27);
            this.overallPPVLabel.TabIndex = 4;
            this.overallPPVLabel.Text = "Overall positive predictive value (precision):";
            this.overallPPVLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // overallNPVLabel
            // 
            this.overallNPVLabel.AutoSize = true;
            this.overallNPVLabel.Location = new System.Drawing.Point(481, 3);
            this.overallNPVLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.overallNPVLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.overallNPVLabel.MinimumSize = new System.Drawing.Size(216, 27);
            this.overallNPVLabel.Name = "overallNPVLabel";
            this.overallNPVLabel.Size = new System.Drawing.Size(216, 27);
            this.overallNPVLabel.TabIndex = 11;
            this.overallNPVLabel.Text = "Overall negative predictive value:";
            this.overallNPVLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ValidationOverallAccPpvNpvMetrics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.overallNPVLabel);
            this.Controls.Add(this.overallPPVLabel);
            this.Controls.Add(this.overallMASELabel);
            this.Controls.Add(this.overallAccuracyLabel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ValidationOverallAccPpvNpvMetrics";
            this.Size = new System.Drawing.Size(882, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label overallAccuracyLabel;
        public System.Windows.Forms.Label overallMASELabel;
        public System.Windows.Forms.Label overallPPVLabel;
        public System.Windows.Forms.Label overallNPVLabel;
    }
}

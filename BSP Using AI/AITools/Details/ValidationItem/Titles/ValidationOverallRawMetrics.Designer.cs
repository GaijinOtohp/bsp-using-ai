
namespace BSP_Using_AI.AITools.Details
{
    partial class ValidationOverallRawMetrics
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
            this.overallTPLabel = new System.Windows.Forms.Label();
            this.overallFNLabel = new System.Windows.Forms.Label();
            this.overallMASELabel = new System.Windows.Forms.Label();
            this.overallTNLabel = new System.Windows.Forms.Label();
            this.overallFPLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // overallTPLabel
            // 
            this.overallTPLabel.AutoSize = true;
            this.overallTPLabel.Location = new System.Drawing.Point(4, 3);
            this.overallTPLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.overallTPLabel.MaximumSize = new System.Drawing.Size(190, 0);
            this.overallTPLabel.MinimumSize = new System.Drawing.Size(160, 27);
            this.overallTPLabel.Name = "overallTPLabel";
            this.overallTPLabel.Size = new System.Drawing.Size(160, 27);
            this.overallTPLabel.TabIndex = 3;
            this.overallTPLabel.Text = "Overall true positives:";
            this.overallTPLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // overallFNLabel
            // 
            this.overallFNLabel.AutoSize = true;
            this.overallFNLabel.Location = new System.Drawing.Point(537, 3);
            this.overallFNLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.overallFNLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.overallFNLabel.MinimumSize = new System.Drawing.Size(160, 27);
            this.overallFNLabel.Name = "overallFNLabel";
            this.overallFNLabel.Size = new System.Drawing.Size(160, 27);
            this.overallFNLabel.TabIndex = 5;
            this.overallFNLabel.Text = "Overall false negatives:";
            this.overallFNLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            // overallTNLabel
            // 
            this.overallTNLabel.AutoSize = true;
            this.overallTNLabel.Location = new System.Drawing.Point(201, 3);
            this.overallTNLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.overallTNLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.overallTNLabel.MinimumSize = new System.Drawing.Size(160, 27);
            this.overallTNLabel.Name = "overallTNLabel";
            this.overallTNLabel.Size = new System.Drawing.Size(160, 27);
            this.overallTNLabel.TabIndex = 4;
            this.overallTNLabel.Text = "Overall true negatives:";
            this.overallTNLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // overallFPLabel
            // 
            this.overallFPLabel.AutoSize = true;
            this.overallFPLabel.Location = new System.Drawing.Point(369, 3);
            this.overallFPLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.overallFPLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.overallFPLabel.MinimumSize = new System.Drawing.Size(160, 27);
            this.overallFPLabel.Name = "overallFPLabel";
            this.overallFPLabel.Size = new System.Drawing.Size(160, 27);
            this.overallFPLabel.TabIndex = 11;
            this.overallFPLabel.Text = "Overall false positives:";
            this.overallFPLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ValidationOverallRawMetrics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.overallFPLabel);
            this.Controls.Add(this.overallTNLabel);
            this.Controls.Add(this.overallMASELabel);
            this.Controls.Add(this.overallFNLabel);
            this.Controls.Add(this.overallTPLabel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ValidationOverallRawMetrics";
            this.Size = new System.Drawing.Size(882, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label overallTPLabel;
        public System.Windows.Forms.Label overallFNLabel;
        public System.Windows.Forms.Label overallMASELabel;
        public System.Windows.Forms.Label overallTNLabel;
        public System.Windows.Forms.Label overallFPLabel;
    }
}

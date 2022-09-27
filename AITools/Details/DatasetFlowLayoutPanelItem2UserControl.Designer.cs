
namespace BSP_Using_AI.AITools.Details
{
    partial class DatasetFlowLayoutPanelItem2UserControl
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
            this.trainingUpdateLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // trainingUpdateLabel
            // 
            this.trainingUpdateLabel.AutoSize = true;
            this.trainingUpdateLabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.trainingUpdateLabel.Location = new System.Drawing.Point(3, 3);
            this.trainingUpdateLabel.Margin = new System.Windows.Forms.Padding(3);
            this.trainingUpdateLabel.MaximumSize = new System.Drawing.Size(163, 0);
            this.trainingUpdateLabel.MinimumSize = new System.Drawing.Size(163, 23);
            this.trainingUpdateLabel.Name = "trainingUpdateLabel";
            this.trainingUpdateLabel.Size = new System.Drawing.Size(163, 23);
            this.trainingUpdateLabel.TabIndex = 3;
            this.trainingUpdateLabel.Text = "Training update";
            this.trainingUpdateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DatasetFlowLayoutPanelItem2UserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.Controls.Add(this.trainingUpdateLabel);
            this.Name = "DatasetFlowLayoutPanelItem2UserControl";
            this.Size = new System.Drawing.Size(605, 29);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label trainingUpdateLabel;
    }
}

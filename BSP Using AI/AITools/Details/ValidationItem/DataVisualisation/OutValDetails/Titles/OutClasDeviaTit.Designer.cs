
namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation.OutValDetails.Titles
{
    partial class OutClasDeviaTit
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
            this.toleranceThresholdLabel = new System.Windows.Forms.Label();
            this.deviationStdLabel = new System.Windows.Forms.Label();
            this.deviationMeanLabel = new System.Windows.Forms.Label();
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
            // toleranceThresholdLabel
            // 
            this.toleranceThresholdLabel.AutoSize = true;
            this.toleranceThresholdLabel.Location = new System.Drawing.Point(528, 3);
            this.toleranceThresholdLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.toleranceThresholdLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.toleranceThresholdLabel.MinimumSize = new System.Drawing.Size(140, 27);
            this.toleranceThresholdLabel.Name = "toleranceThresholdLabel";
            this.toleranceThresholdLabel.Size = new System.Drawing.Size(140, 27);
            this.toleranceThresholdLabel.TabIndex = 13;
            this.toleranceThresholdLabel.Text = "Tolerance threshold (ms)";
            this.toleranceThresholdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // deviationStdLabel
            // 
            this.deviationStdLabel.AutoSize = true;
            this.deviationStdLabel.Location = new System.Drawing.Point(355, 3);
            this.deviationStdLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.deviationStdLabel.MaximumSize = new System.Drawing.Size(145, 0);
            this.deviationStdLabel.MinimumSize = new System.Drawing.Size(165, 27);
            this.deviationStdLabel.Name = "deviationStdLabel";
            this.deviationStdLabel.Size = new System.Drawing.Size(165, 27);
            this.deviationStdLabel.TabIndex = 15;
            this.deviationStdLabel.Text = "Deviation Standard dev (ms)";
            this.deviationStdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // deviationMeanLabel
            // 
            this.deviationMeanLabel.AutoSize = true;
            this.deviationMeanLabel.Location = new System.Drawing.Point(202, 3);
            this.deviationMeanLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.deviationMeanLabel.MaximumSize = new System.Drawing.Size(145, 0);
            this.deviationMeanLabel.MinimumSize = new System.Drawing.Size(145, 27);
            this.deviationMeanLabel.Name = "deviationMeanLabel";
            this.deviationMeanLabel.Size = new System.Drawing.Size(145, 27);
            this.deviationMeanLabel.TabIndex = 14;
            this.deviationMeanLabel.Text = "Deviation mean (ms)";
            this.deviationMeanLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OutClasDeviaTit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.deviationStdLabel);
            this.Controls.Add(this.deviationMeanLabel);
            this.Controls.Add(this.toleranceThresholdLabel);
            this.Controls.Add(this.outputLabel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "OutClasDeviaTit";
            this.Size = new System.Drawing.Size(683, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label outputLabel;
        public System.Windows.Forms.Label toleranceThresholdLabel;
        public System.Windows.Forms.Label deviationStdLabel;
        public System.Windows.Forms.Label deviationMeanLabel;
    }
}

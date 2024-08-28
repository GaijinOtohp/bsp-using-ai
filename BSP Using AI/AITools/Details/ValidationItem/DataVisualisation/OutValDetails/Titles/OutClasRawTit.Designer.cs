
namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation.OutValDetails.Titles
{
    partial class OutClasRawTit
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
            this.truePositiveLabel = new System.Windows.Forms.Label();
            this.trueNegativeLabel = new System.Windows.Forms.Label();
            this.falsePositiveLabel = new System.Windows.Forms.Label();
            this.classificationThresholdLabel = new System.Windows.Forms.Label();
            this.falseNegativeLabel = new System.Windows.Forms.Label();
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
            // truePositiveLabel
            // 
            this.truePositiveLabel.AutoSize = true;
            this.truePositiveLabel.Location = new System.Drawing.Point(202, 3);
            this.truePositiveLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.truePositiveLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.truePositiveLabel.MinimumSize = new System.Drawing.Size(60, 27);
            this.truePositiveLabel.Name = "truePositiveLabel";
            this.truePositiveLabel.Size = new System.Drawing.Size(60, 27);
            this.truePositiveLabel.TabIndex = 8;
            this.truePositiveLabel.Text = "TP";
            this.truePositiveLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // trueNegativeLabel
            // 
            this.trueNegativeLabel.AutoSize = true;
            this.trueNegativeLabel.Location = new System.Drawing.Point(270, 3);
            this.trueNegativeLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.trueNegativeLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.trueNegativeLabel.MinimumSize = new System.Drawing.Size(60, 27);
            this.trueNegativeLabel.Name = "trueNegativeLabel";
            this.trueNegativeLabel.Size = new System.Drawing.Size(60, 27);
            this.trueNegativeLabel.TabIndex = 9;
            this.trueNegativeLabel.Text = "TN";
            this.trueNegativeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // falsePositiveLabel
            // 
            this.falsePositiveLabel.AutoSize = true;
            this.falsePositiveLabel.Location = new System.Drawing.Point(338, 3);
            this.falsePositiveLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.falsePositiveLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.falsePositiveLabel.MinimumSize = new System.Drawing.Size(60, 27);
            this.falsePositiveLabel.Name = "falsePositiveLabel";
            this.falsePositiveLabel.Size = new System.Drawing.Size(60, 27);
            this.falsePositiveLabel.TabIndex = 10;
            this.falsePositiveLabel.Text = "FP";
            this.falsePositiveLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // classificationThresholdLabel
            // 
            this.classificationThresholdLabel.AutoSize = true;
            this.classificationThresholdLabel.Location = new System.Drawing.Point(517, 3);
            this.classificationThresholdLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.classificationThresholdLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.classificationThresholdLabel.MinimumSize = new System.Drawing.Size(160, 27);
            this.classificationThresholdLabel.Name = "classificationThresholdLabel";
            this.classificationThresholdLabel.Size = new System.Drawing.Size(160, 27);
            this.classificationThresholdLabel.TabIndex = 13;
            this.classificationThresholdLabel.Text = "Classification threshold";
            this.classificationThresholdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // falseNegativeLabel
            // 
            this.falseNegativeLabel.AutoSize = true;
            this.falseNegativeLabel.Location = new System.Drawing.Point(406, 3);
            this.falseNegativeLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.falseNegativeLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.falseNegativeLabel.MinimumSize = new System.Drawing.Size(60, 27);
            this.falseNegativeLabel.Name = "falseNegativeLabel";
            this.falseNegativeLabel.Size = new System.Drawing.Size(60, 27);
            this.falseNegativeLabel.TabIndex = 12;
            this.falseNegativeLabel.Text = "FN";
            this.falseNegativeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OutValClassRawMetricsTitles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.classificationThresholdLabel);
            this.Controls.Add(this.falseNegativeLabel);
            this.Controls.Add(this.falsePositiveLabel);
            this.Controls.Add(this.trueNegativeLabel);
            this.Controls.Add(this.truePositiveLabel);
            this.Controls.Add(this.outputLabel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "OutValClassRawMetricsTitles";
            this.Size = new System.Drawing.Size(683, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label outputLabel;
        public System.Windows.Forms.Label truePositiveLabel;
        public System.Windows.Forms.Label trueNegativeLabel;
        public System.Windows.Forms.Label falsePositiveLabel;
        public System.Windows.Forms.Label classificationThresholdLabel;
        public System.Windows.Forms.Label falseNegativeLabel;
    }
}

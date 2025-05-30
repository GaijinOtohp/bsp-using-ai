﻿
namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation.OutValDetails.Titles
{
    partial class OutClasAcF1STit
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
            this.classificationThresholdLabel = new System.Windows.Forms.Label();
            this.f1ScoreLabel = new System.Windows.Forms.Label();
            this.accuracyLabel = new System.Windows.Forms.Label();
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
            // f1ScoreLabel
            // 
            this.f1ScoreLabel.AutoSize = true;
            this.f1ScoreLabel.Location = new System.Drawing.Point(355, 3);
            this.f1ScoreLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.f1ScoreLabel.MaximumSize = new System.Drawing.Size(145, 0);
            this.f1ScoreLabel.MinimumSize = new System.Drawing.Size(145, 27);
            this.f1ScoreLabel.Name = "f1ScoreLabel";
            this.f1ScoreLabel.Size = new System.Drawing.Size(145, 27);
            this.f1ScoreLabel.TabIndex = 15;
            this.f1ScoreLabel.Text = "F1-Score";
            this.f1ScoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // accuracyLabel
            // 
            this.accuracyLabel.AutoSize = true;
            this.accuracyLabel.Location = new System.Drawing.Point(202, 3);
            this.accuracyLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.accuracyLabel.MaximumSize = new System.Drawing.Size(145, 0);
            this.accuracyLabel.MinimumSize = new System.Drawing.Size(145, 27);
            this.accuracyLabel.Name = "accuracyLabel";
            this.accuracyLabel.Size = new System.Drawing.Size(145, 27);
            this.accuracyLabel.TabIndex = 14;
            this.accuracyLabel.Text = "Accuracy";
            this.accuracyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OutValClassAccF1ScoreMetricsTitles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.f1ScoreLabel);
            this.Controls.Add(this.accuracyLabel);
            this.Controls.Add(this.classificationThresholdLabel);
            this.Controls.Add(this.outputLabel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "OutValClassAccF1ScoreMetricsTitles";
            this.Size = new System.Drawing.Size(683, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label outputLabel;
        public System.Windows.Forms.Label classificationThresholdLabel;
        public System.Windows.Forms.Label f1ScoreLabel;
        public System.Windows.Forms.Label accuracyLabel;
    }
}

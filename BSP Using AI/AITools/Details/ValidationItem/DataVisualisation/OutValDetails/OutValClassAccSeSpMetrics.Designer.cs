
namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation.OutValDetails
{
    partial class OutValClassAccSeSpMetrics
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
            this.specificityLabel = new System.Windows.Forms.Label();
            this.sensitivityLabel = new System.Windows.Forms.Label();
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
            // specificityLabel
            // 
            this.specificityLabel.AutoSize = true;
            this.specificityLabel.Location = new System.Drawing.Point(406, 3);
            this.specificityLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.specificityLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.specificityLabel.MinimumSize = new System.Drawing.Size(94, 27);
            this.specificityLabel.Name = "specificityLabel";
            this.specificityLabel.Size = new System.Drawing.Size(103, 27);
            this.specificityLabel.TabIndex = 16;
            this.specificityLabel.Text = "Specificity (Recall)";
            this.specificityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // sensitivityLabel
            // 
            this.sensitivityLabel.AutoSize = true;
            this.sensitivityLabel.Location = new System.Drawing.Point(304, 3);
            this.sensitivityLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.sensitivityLabel.MaximumSize = new System.Drawing.Size(117, 0);
            this.sensitivityLabel.MinimumSize = new System.Drawing.Size(94, 27);
            this.sensitivityLabel.Name = "sensitivityLabel";
            this.sensitivityLabel.Size = new System.Drawing.Size(94, 27);
            this.sensitivityLabel.TabIndex = 15;
            this.sensitivityLabel.Text = "Sensitivity";
            this.sensitivityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            // 
            // OutValClassAccSeSpMetrics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.classificationThresholdTextBox);
            this.Controls.Add(this.specificityLabel);
            this.Controls.Add(this.sensitivityLabel);
            this.Controls.Add(this.accuracyLabel);
            this.Controls.Add(this.outputLabel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "OutValClassAccSeSpMetrics";
            this.Size = new System.Drawing.Size(683, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label outputLabel;
        public System.Windows.Forms.Label specificityLabel;
        public System.Windows.Forms.Label sensitivityLabel;
        public System.Windows.Forms.Label accuracyLabel;
        public System.Windows.Forms.TextBox classificationThresholdTextBox;
    }
}

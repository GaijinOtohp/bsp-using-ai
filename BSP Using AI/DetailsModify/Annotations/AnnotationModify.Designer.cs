namespace Biological_Signal_Processing_Using_AI.DetailsModify.Annotations
{
    partial class AnnotationModify
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.nameLabel = new System.Windows.Forms.Label();
            this.removeButton = new System.Windows.Forms.Button();
            this.startingIndexTextBox = new System.Windows.Forms.TextBox();
            this.startingIndexLabel = new System.Windows.Forms.Label();
            this.endingIndexTextBox = new System.Windows.Forms.TextBox();
            this.endingIndexLabel = new System.Windows.Forms.Label();
            this.AnnotationTypeLabel = new System.Windows.Forms.Label();
            this.confirmButton = new System.Windows.Forms.Button();
            this.nameComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(12, 40);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(39, 15);
            this.nameLabel.TabIndex = 1;
            this.nameLabel.Text = "Name";
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(12, 178);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 7;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // startingIndexTextBox
            // 
            this.startingIndexTextBox.Location = new System.Drawing.Point(12, 119);
            this.startingIndexTextBox.Name = "startingIndexTextBox";
            this.startingIndexTextBox.Size = new System.Drawing.Size(100, 23);
            this.startingIndexTextBox.TabIndex = 4;
            this.startingIndexTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.startingIndexTextBox_KeyPress);
            // 
            // startingIndexLabel
            // 
            this.startingIndexLabel.AutoSize = true;
            this.startingIndexLabel.Location = new System.Drawing.Point(12, 101);
            this.startingIndexLabel.Name = "startingIndexLabel";
            this.startingIndexLabel.Size = new System.Drawing.Size(80, 15);
            this.startingIndexLabel.TabIndex = 3;
            this.startingIndexLabel.Text = "Starting index";
            // 
            // endingIndexTextBox
            // 
            this.endingIndexTextBox.Location = new System.Drawing.Point(145, 119);
            this.endingIndexTextBox.Name = "endingIndexTextBox";
            this.endingIndexTextBox.Size = new System.Drawing.Size(100, 23);
            this.endingIndexTextBox.TabIndex = 6;
            this.endingIndexTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.startingIndexTextBox_KeyPress);
            // 
            // endingIndexLabel
            // 
            this.endingIndexLabel.AutoSize = true;
            this.endingIndexLabel.Location = new System.Drawing.Point(145, 101);
            this.endingIndexLabel.Name = "endingIndexLabel";
            this.endingIndexLabel.Size = new System.Drawing.Size(76, 15);
            this.endingIndexLabel.TabIndex = 5;
            this.endingIndexLabel.Text = "Ending index";
            // 
            // AnnotationTypeLabel
            // 
            this.AnnotationTypeLabel.AutoSize = true;
            this.AnnotationTypeLabel.Location = new System.Drawing.Point(12, 9);
            this.AnnotationTypeLabel.Name = "AnnotationTypeLabel";
            this.AnnotationTypeLabel.Size = new System.Drawing.Size(107, 15);
            this.AnnotationTypeLabel.TabIndex = 0;
            this.AnnotationTypeLabel.Text = "Interval annotation";
            // 
            // confirmButton
            // 
            this.confirmButton.Location = new System.Drawing.Point(170, 178);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(75, 23);
            this.confirmButton.TabIndex = 8;
            this.confirmButton.Text = "Confirm";
            this.confirmButton.UseVisualStyleBackColor = true;
            this.confirmButton.Click += new System.EventHandler(this.confirmButton_Click);
            // 
            // nameComboBox
            // 
            this.nameComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.nameComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.nameComboBox.FormattingEnabled = true;
            this.nameComboBox.Location = new System.Drawing.Point(12, 58);
            this.nameComboBox.Name = "nameComboBox";
            this.nameComboBox.Size = new System.Drawing.Size(121, 23);
            this.nameComboBox.TabIndex = 2;
            // 
            // AnnotationModify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(257, 208);
            this.Controls.Add(this.nameComboBox);
            this.Controls.Add(this.confirmButton);
            this.Controls.Add(this.AnnotationTypeLabel);
            this.Controls.Add(this.endingIndexTextBox);
            this.Controls.Add(this.endingIndexLabel);
            this.Controls.Add(this.startingIndexTextBox);
            this.Controls.Add(this.startingIndexLabel);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.nameLabel);
            this.Name = "AnnotationModify";
            this.Text = "AnnotationModify";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.TextBox startingIndexTextBox;
        private System.Windows.Forms.Label startingIndexLabel;
        private System.Windows.Forms.TextBox endingIndexTextBox;
        private System.Windows.Forms.Label endingIndexLabel;
        private System.Windows.Forms.Label AnnotationTypeLabel;
        private System.Windows.Forms.Button confirmButton;
        private System.Windows.Forms.ComboBox nameComboBox;
    }
}
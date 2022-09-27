
namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation
{
    partial class RawVisItemUserControl
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
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.primaryColorButton = new System.Windows.Forms.Button();
            this.secondaryColorButton = new System.Windows.Forms.Button();
            this.outputCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // colorDialog
            // 
            this.colorDialog.Color = System.Drawing.Color.Fuchsia;
            // 
            // primaryColorButton
            // 
            this.primaryColorButton.BackColor = System.Drawing.Color.Fuchsia;
            this.primaryColorButton.Location = new System.Drawing.Point(102, 3);
            this.primaryColorButton.Name = "primaryColorButton";
            this.primaryColorButton.Size = new System.Drawing.Size(32, 23);
            this.primaryColorButton.TabIndex = 1;
            this.primaryColorButton.UseVisualStyleBackColor = false;
            this.primaryColorButton.Click += new System.EventHandler(this.primaryColorButton_Click);
            // 
            // secondaryColorButton
            // 
            this.secondaryColorButton.BackColor = System.Drawing.Color.Blue;
            this.secondaryColorButton.Location = new System.Drawing.Point(140, 3);
            this.secondaryColorButton.Name = "secondaryColorButton";
            this.secondaryColorButton.Size = new System.Drawing.Size(32, 23);
            this.secondaryColorButton.TabIndex = 2;
            this.secondaryColorButton.UseVisualStyleBackColor = false;
            this.secondaryColorButton.Click += new System.EventHandler(this.primaryColorButton_Click);
            // 
            // outputCheckBox
            // 
            this.outputCheckBox.AutoSize = true;
            this.outputCheckBox.Checked = true;
            this.outputCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.outputCheckBox.Location = new System.Drawing.Point(3, 7);
            this.outputCheckBox.Name = "outputCheckBox";
            this.outputCheckBox.Size = new System.Drawing.Size(56, 17);
            this.outputCheckBox.TabIndex = 3;
            this.outputCheckBox.Text = "output";
            this.outputCheckBox.UseVisualStyleBackColor = true;
            this.outputCheckBox.CheckedChanged += new System.EventHandler(this.outputCheckBox_CheckedChanged);
            // 
            // RawVisItemUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.outputCheckBox);
            this.Controls.Add(this.secondaryColorButton);
            this.Controls.Add(this.primaryColorButton);
            this.Name = "RawVisItemUserControl";
            this.Size = new System.Drawing.Size(175, 30);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog;
        public System.Windows.Forms.Button primaryColorButton;
        public System.Windows.Forms.Button secondaryColorButton;
        public System.Windows.Forms.CheckBox outputCheckBox;
    }
}

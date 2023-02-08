
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
            this.primaryColorButton.Location = new System.Drawing.Point(136, 5);
            this.primaryColorButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.primaryColorButton.Name = "primaryColorButton";
            this.primaryColorButton.Size = new System.Drawing.Size(43, 28);
            this.primaryColorButton.TabIndex = 1;
            this.primaryColorButton.UseVisualStyleBackColor = false;
            this.primaryColorButton.Click += new System.EventHandler(this.primaryColorButton_Click);
            // 
            // secondaryColorButton
            // 
            this.secondaryColorButton.BackColor = System.Drawing.Color.Blue;
            this.secondaryColorButton.Location = new System.Drawing.Point(187, 5);
            this.secondaryColorButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.secondaryColorButton.Name = "secondaryColorButton";
            this.secondaryColorButton.Size = new System.Drawing.Size(43, 28);
            this.secondaryColorButton.TabIndex = 2;
            this.secondaryColorButton.UseVisualStyleBackColor = false;
            this.secondaryColorButton.Click += new System.EventHandler(this.primaryColorButton_Click);
            // 
            // outputCheckBox
            // 
            this.outputCheckBox.Checked = true;
            this.outputCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.outputCheckBox.Location = new System.Drawing.Point(0, 0);
            this.outputCheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.outputCheckBox.MinimumSize = new System.Drawing.Size(125, 0);
            this.outputCheckBox.Name = "outputCheckBox";
            this.outputCheckBox.Size = new System.Drawing.Size(125, 40);
            this.outputCheckBox.TabIndex = 3;
            this.outputCheckBox.Text = "output";
            this.outputCheckBox.UseVisualStyleBackColor = true;
            this.outputCheckBox.CheckedChanged += new System.EventHandler(this.outputCheckBox_CheckedChanged);
            // 
            // RawVisItemUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.outputCheckBox);
            this.Controls.Add(this.secondaryColorButton);
            this.Controls.Add(this.primaryColorButton);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(233, 37);
            this.Name = "RawVisItemUserControl";
            this.Size = new System.Drawing.Size(234, 40);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog;
        public System.Windows.Forms.Button primaryColorButton;
        public System.Windows.Forms.Button secondaryColorButton;
        public System.Windows.Forms.CheckBox outputCheckBox;
    }
}

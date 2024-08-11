
namespace BSP_Using_AI.DetailsModify.FiltersControls
{
    partial class CheckExistenceUserControl
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
            this.existenceOfCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // existenceOfCheckBox
            // 
            this.existenceOfCheckBox.AutoSize = true;
            this.existenceOfCheckBox.Location = new System.Drawing.Point(26, 9);
            this.existenceOfCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.existenceOfCheckBox.Name = "existenceOfCheckBox";
            this.existenceOfCheckBox.Size = new System.Drawing.Size(89, 19);
            this.existenceOfCheckBox.TabIndex = 21;
            this.existenceOfCheckBox.Text = "Existence of";
            this.existenceOfCheckBox.UseVisualStyleBackColor = true;
            this.existenceOfCheckBox.CheckedChanged += new System.EventHandler(this.existenceOfCheckBox_CheckedChanged);
            // 
            // CheckExistenceUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.existenceOfCheckBox);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "CheckExistenceUserControl";
            this.Size = new System.Drawing.Size(331, 38);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox existenceOfCheckBox;
    }
}

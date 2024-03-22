
namespace BSP_Using_AI.DetailsModify.FiltersControls
{
    partial class CheckExistanceUserControl
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
            this.existanceOfCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // existanceOfCheckBox
            // 
            this.existanceOfCheckBox.AutoSize = true;
            this.existanceOfCheckBox.Location = new System.Drawing.Point(22, 8);
            this.existanceOfCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.existanceOfCheckBox.Name = "existanceOfCheckBox";
            this.existanceOfCheckBox.Size = new System.Drawing.Size(84, 17);
            this.existanceOfCheckBox.TabIndex = 21;
            this.existanceOfCheckBox.Text = "Existance of";
            this.existanceOfCheckBox.UseVisualStyleBackColor = true;
            this.existanceOfCheckBox.CheckedChanged += new System.EventHandler(this.existanceOfCheckBox_CheckedChanged);
            // 
            // CheckExistanceUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.existanceOfCheckBox);
            this.Name = "CheckExistanceUserControl";
            this.Size = new System.Drawing.Size(284, 33);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox existanceOfCheckBox;
    }
}

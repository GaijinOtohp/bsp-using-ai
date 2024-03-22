
namespace BSP_Using_AI.DetailsModify.FiltersControls
{
    partial class AbsoluteSignalUserControl
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
            this.components = new System.ComponentModel.Container();
            this.absoluteSignalCheckBox = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // absoluteSignalCheckBox
            // 
            this.absoluteSignalCheckBox.AutoSize = true;
            this.absoluteSignalCheckBox.Checked = true;
            this.absoluteSignalCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.absoluteSignalCheckBox.Location = new System.Drawing.Point(22, 8);
            this.absoluteSignalCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.absoluteSignalCheckBox.Name = "absoluteSignalCheckBox";
            this.absoluteSignalCheckBox.Size = new System.Drawing.Size(97, 17);
            this.absoluteSignalCheckBox.TabIndex = 22;
            this.absoluteSignalCheckBox.Text = "Absolute signal";
            this.absoluteSignalCheckBox.UseVisualStyleBackColor = true;
            this.absoluteSignalCheckBox.CheckStateChanged += new System.EventHandler(this.dcValueRemoveCheckBox_CheckStateChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(108, 26);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // AbsoluteSignalUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.absoluteSignalCheckBox);
            this.Name = "AbsoluteSignalUserControl";
            this.Size = new System.Drawing.Size(284, 33);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.CheckBox absoluteSignalCheckBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    }
}

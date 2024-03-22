
namespace BSP_Using_AI.DetailsModify.FiltersControls
{
    partial class NormalizedSignalUserControl
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
            this.normalizeSignalCheckBox = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // normalizeSignalCheckBox
            // 
            this.normalizeSignalCheckBox.AutoSize = true;
            this.normalizeSignalCheckBox.Checked = true;
            this.normalizeSignalCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.normalizeSignalCheckBox.Location = new System.Drawing.Point(22, 8);
            this.normalizeSignalCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.normalizeSignalCheckBox.Name = "normalizeSignalCheckBox";
            this.normalizeSignalCheckBox.Size = new System.Drawing.Size(102, 17);
            this.normalizeSignalCheckBox.TabIndex = 20;
            this.normalizeSignalCheckBox.Text = "Normalize signal";
            this.normalizeSignalCheckBox.UseVisualStyleBackColor = true;
            this.normalizeSignalCheckBox.CheckStateChanged += new System.EventHandler(this.dcValueRemoveCheckBox_CheckStateChanged);
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
            // NormalizedSignalUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.normalizeSignalCheckBox);
            this.Name = "NormalizedSignalUserControl";
            this.Size = new System.Drawing.Size(284, 33);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.CheckBox normalizeSignalCheckBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    }
}

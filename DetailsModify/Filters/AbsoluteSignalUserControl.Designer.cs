
namespace BSP_Using_AI.DetailsModify.Filters
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
            this.applyAfterTransformCheckBox = new System.Windows.Forms.CheckBox();
            this.absoluteSignalCheckBox = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // applyAfterTransformCheckBox
            // 
            this.applyAfterTransformCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.applyAfterTransformCheckBox.AutoSize = true;
            this.applyAfterTransformCheckBox.Location = new System.Drawing.Point(22, 37);
            this.applyAfterTransformCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.applyAfterTransformCheckBox.Name = "applyAfterTransformCheckBox";
            this.applyAfterTransformCheckBox.Size = new System.Drawing.Size(122, 17);
            this.applyAfterTransformCheckBox.TabIndex = 23;
            this.applyAfterTransformCheckBox.Text = "Apply after transform";
            this.applyAfterTransformCheckBox.UseVisualStyleBackColor = true;
            this.applyAfterTransformCheckBox.CheckStateChanged += new System.EventHandler(this.applyAfterTransformCheckBox_CheckStateChanged);
            // 
            // absoluteSignalCheckBox
            // 
            this.absoluteSignalCheckBox.AutoSize = true;
            this.absoluteSignalCheckBox.Location = new System.Drawing.Point(22, 16);
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
            this.Controls.Add(this.applyAfterTransformCheckBox);
            this.Controls.Add(this.absoluteSignalCheckBox);
            this.Name = "AbsoluteSignalUserControl";
            this.Size = new System.Drawing.Size(284, 67);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox applyAfterTransformCheckBox;
        public System.Windows.Forms.CheckBox absoluteSignalCheckBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    }
}

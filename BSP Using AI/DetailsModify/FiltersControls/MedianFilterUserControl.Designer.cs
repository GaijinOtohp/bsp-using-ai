namespace Biological_Signal_Processing_Using_AI.DetailsModify.FiltersControls
{
    partial class MedianFilterUserControl
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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowSizeLabel = new System.Windows.Forms.Label();
            this.windowSizeTextBox = new System.Windows.Forms.TextBox();
            this.medianFilterLabel = new System.Windows.Forms.Label();
            this.strideSizeabel = new System.Windows.Forms.Label();
            this.strideSizeTextBox = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
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
            // windowSizeLabel
            // 
            this.windowSizeLabel.AutoSize = true;
            this.windowSizeLabel.Location = new System.Drawing.Point(2, 34);
            this.windowSizeLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.windowSizeLabel.Name = "windowSizeLabel";
            this.windowSizeLabel.Size = new System.Drawing.Size(73, 15);
            this.windowSizeLabel.TabIndex = 23;
            this.windowSizeLabel.Text = "Window size";
            // 
            // windowSizeTextBox
            // 
            this.windowSizeTextBox.ForeColor = System.Drawing.Color.Black;
            this.windowSizeTextBox.Location = new System.Drawing.Point(2, 51);
            this.windowSizeTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.windowSizeTextBox.Name = "windowSizeTextBox";
            this.windowSizeTextBox.Size = new System.Drawing.Size(73, 23);
            this.windowSizeTextBox.TabIndex = 22;
            this.windowSizeTextBox.Text = "3";
            this.windowSizeTextBox.TextChanged += new System.EventHandler(this.windowSizeTextBox_TextChanged);
            this.windowSizeTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.windowSizeTextBox_KeyPress);
            // 
            // medianFilterLabel
            // 
            this.medianFilterLabel.AutoSize = true;
            this.medianFilterLabel.Location = new System.Drawing.Point(2, 2);
            this.medianFilterLabel.Margin = new System.Windows.Forms.Padding(2);
            this.medianFilterLabel.Name = "medianFilterLabel";
            this.medianFilterLabel.Size = new System.Drawing.Size(74, 15);
            this.medianFilterLabel.TabIndex = 21;
            this.medianFilterLabel.Text = "Median filter";
            // 
            // strideSizeabel
            // 
            this.strideSizeabel.AutoSize = true;
            this.strideSizeabel.Location = new System.Drawing.Point(254, 34);
            this.strideSizeabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.strideSizeabel.Name = "strideSizeabel";
            this.strideSizeabel.Size = new System.Drawing.Size(59, 15);
            this.strideSizeabel.TabIndex = 25;
            this.strideSizeabel.Text = "Stride size";
            // 
            // strideSizeTextBox
            // 
            this.strideSizeTextBox.ForeColor = System.Drawing.Color.Black;
            this.strideSizeTextBox.Location = new System.Drawing.Point(254, 51);
            this.strideSizeTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.strideSizeTextBox.Name = "strideSizeTextBox";
            this.strideSizeTextBox.Size = new System.Drawing.Size(73, 23);
            this.strideSizeTextBox.TabIndex = 24;
            this.strideSizeTextBox.Text = "1";
            this.strideSizeTextBox.TextChanged += new System.EventHandler(this.strideSizeTextBox_TextChanged);
            this.strideSizeTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.windowSizeTextBox_KeyPress);
            // 
            // MedianFilterUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.strideSizeabel);
            this.Controls.Add(this.strideSizeTextBox);
            this.Controls.Add(this.windowSizeLabel);
            this.Controls.Add(this.windowSizeTextBox);
            this.Controls.Add(this.medianFilterLabel);
            this.Name = "MedianFilterUserControl";
            this.Size = new System.Drawing.Size(331, 78);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Label windowSizeLabel;
        public System.Windows.Forms.TextBox windowSizeTextBox;
        public System.Windows.Forms.Label medianFilterLabel;
        private System.Windows.Forms.Label strideSizeabel;
        public System.Windows.Forms.TextBox strideSizeTextBox;
    }
}

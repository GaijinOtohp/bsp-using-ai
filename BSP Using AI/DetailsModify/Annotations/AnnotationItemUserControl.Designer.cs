namespace Biological_Signal_Processing_Using_AI.DetailsModify.Annotations
{
    partial class AnnotationItemUserControl
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
            this.nameCheckBox = new System.Windows.Forms.CheckBox();
            this.startingIndexLabel = new System.Windows.Forms.Label();
            this.startingIndexTextBox = new System.Windows.Forms.TextBox();
            this.endingIndexTextBox = new System.Windows.Forms.TextBox();
            this.endingIndexLabel = new System.Windows.Forms.Label();
            this.deleteContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteCurrentAnnotationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAllSelectedAnnotationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // nameCheckBox
            // 
            this.nameCheckBox.AutoSize = true;
            this.nameCheckBox.Location = new System.Drawing.Point(3, 3);
            this.nameCheckBox.Name = "nameCheckBox";
            this.nameCheckBox.Size = new System.Drawing.Size(83, 19);
            this.nameCheckBox.TabIndex = 0;
            this.nameCheckBox.Text = "checkBox1";
            this.nameCheckBox.UseVisualStyleBackColor = true;
            this.nameCheckBox.CheckedChanged += new System.EventHandler(this.nameCheckBox_CheckedChanged);
            this.nameCheckBox.MouseEnter += new System.EventHandler(this.AnnotationItemUserControl_MouseEnter);
            this.nameCheckBox.MouseLeave += new System.EventHandler(this.AnnotationItemUserControl_MouseLeave);
            // 
            // startingIndexLabel
            // 
            this.startingIndexLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.startingIndexLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.startingIndexLabel.Location = new System.Drawing.Point(3, 30);
            this.startingIndexLabel.Name = "startingIndexLabel";
            this.startingIndexLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.startingIndexLabel.Size = new System.Drawing.Size(83, 23);
            this.startingIndexLabel.TabIndex = 1;
            this.startingIndexLabel.Text = "Starting index";
            this.startingIndexLabel.Click += new System.EventHandler(this.AnnotationItemUserControl_Click);
            this.startingIndexLabel.MouseEnter += new System.EventHandler(this.AnnotationItemUserControl_MouseEnter);
            this.startingIndexLabel.MouseLeave += new System.EventHandler(this.AnnotationItemUserControl_MouseLeave);
            // 
            // startingIndexTextBox
            // 
            this.startingIndexTextBox.Location = new System.Drawing.Point(87, 27);
            this.startingIndexTextBox.Name = "startingIndexTextBox";
            this.startingIndexTextBox.ReadOnly = true;
            this.startingIndexTextBox.Size = new System.Drawing.Size(74, 23);
            this.startingIndexTextBox.TabIndex = 2;
            this.startingIndexTextBox.Click += new System.EventHandler(this.AnnotationItemUserControl_Click);
            this.startingIndexTextBox.MouseEnter += new System.EventHandler(this.AnnotationItemUserControl_MouseEnter);
            this.startingIndexTextBox.MouseLeave += new System.EventHandler(this.AnnotationItemUserControl_MouseLeave);
            // 
            // endingIndexTextBox
            // 
            this.endingIndexTextBox.Location = new System.Drawing.Point(256, 27);
            this.endingIndexTextBox.Name = "endingIndexTextBox";
            this.endingIndexTextBox.ReadOnly = true;
            this.endingIndexTextBox.Size = new System.Drawing.Size(74, 23);
            this.endingIndexTextBox.TabIndex = 4;
            this.endingIndexTextBox.Click += new System.EventHandler(this.AnnotationItemUserControl_Click);
            this.endingIndexTextBox.MouseEnter += new System.EventHandler(this.AnnotationItemUserControl_MouseEnter);
            this.endingIndexTextBox.MouseLeave += new System.EventHandler(this.AnnotationItemUserControl_MouseLeave);
            // 
            // endingIndexLabel
            // 
            this.endingIndexLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.endingIndexLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.endingIndexLabel.Location = new System.Drawing.Point(172, 30);
            this.endingIndexLabel.Name = "endingIndexLabel";
            this.endingIndexLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.endingIndexLabel.Size = new System.Drawing.Size(83, 23);
            this.endingIndexLabel.TabIndex = 3;
            this.endingIndexLabel.Text = "Ending index";
            this.endingIndexLabel.Click += new System.EventHandler(this.AnnotationItemUserControl_Click);
            this.endingIndexLabel.MouseEnter += new System.EventHandler(this.AnnotationItemUserControl_MouseEnter);
            this.endingIndexLabel.MouseLeave += new System.EventHandler(this.AnnotationItemUserControl_MouseLeave);
            // 
            // deleteContextMenuStrip
            // 
            this.deleteContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteCurrentAnnotationToolStripMenuItem,
            this.deleteAllSelectedAnnotationsToolStripMenuItem});
            this.deleteContextMenuStrip.Name = "deleteContextMenuStrip";
            this.deleteContextMenuStrip.Size = new System.Drawing.Size(235, 48);
            // 
            // deleteCurrentAnnotationToolStripMenuItem
            // 
            this.deleteCurrentAnnotationToolStripMenuItem.Name = "deleteCurrentAnnotationToolStripMenuItem";
            this.deleteCurrentAnnotationToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.deleteCurrentAnnotationToolStripMenuItem.Text = "Delete current annotation";
            this.deleteCurrentAnnotationToolStripMenuItem.Click += new System.EventHandler(this.deleteCurrentAnnotationToolStripMenuItem_Click);
            // 
            // deleteAllSelectedAnnotationsToolStripMenuItem
            // 
            this.deleteAllSelectedAnnotationsToolStripMenuItem.Name = "deleteAllSelectedAnnotationsToolStripMenuItem";
            this.deleteAllSelectedAnnotationsToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.deleteAllSelectedAnnotationsToolStripMenuItem.Text = "Delete all selected annotations";
            this.deleteAllSelectedAnnotationsToolStripMenuItem.Click += new System.EventHandler(this.deleteAllSelectedAnnotationsToolStripMenuItem_Click);
            // 
            // AnnotationItemUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.deleteContextMenuStrip;
            this.Controls.Add(this.endingIndexTextBox);
            this.Controls.Add(this.endingIndexLabel);
            this.Controls.Add(this.startingIndexTextBox);
            this.Controls.Add(this.startingIndexLabel);
            this.Controls.Add(this.nameCheckBox);
            this.Name = "AnnotationItemUserControl";
            this.Size = new System.Drawing.Size(333, 53);
            this.Click += new System.EventHandler(this.AnnotationItemUserControl_Click);
            this.MouseEnter += new System.EventHandler(this.AnnotationItemUserControl_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.AnnotationItemUserControl_MouseLeave);
            this.deleteContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox nameCheckBox;
        private System.Windows.Forms.Label startingIndexLabel;
        private System.Windows.Forms.TextBox startingIndexTextBox;
        private System.Windows.Forms.TextBox endingIndexTextBox;
        private System.Windows.Forms.Label endingIndexLabel;
        private System.Windows.Forms.ContextMenuStrip deleteContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem deleteCurrentAnnotationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteAllSelectedAnnotationsToolStripMenuItem;
    }
}

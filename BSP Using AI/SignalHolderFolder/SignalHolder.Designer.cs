﻿namespace BSP_Using_AI.SignalHolderFolder
{
    partial class SignalHolder
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
            this.signalExhibitor = new ScottPlot.FormsPlot();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sendSignalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chooseFileButton = new System.Windows.Forms.Button();
            this.detailsModifyButton = new System.Windows.Forms.Button();
            this.backwardButton = new System.Windows.Forms.Button();
            this.forwardButton = new System.Windows.Forms.Button();
            this.pathLabel = new System.Windows.Forms.Label();
            this.signalSpanTextBox = new System.Windows.Forms.TextBox();
            this.signalSpanLabel = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // signalExhibitor
            // 
            this.signalExhibitor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.signalExhibitor.ContextMenuStrip = this.contextMenuStrip1;
            this.signalExhibitor.Location = new System.Drawing.Point(7, 31);
            this.signalExhibitor.Margin = new System.Windows.Forms.Padding(7, 2, 2, 6);
            this.signalExhibitor.Name = "signalExhibitor";
            this.signalExhibitor.Size = new System.Drawing.Size(870, 194);
            this.signalExhibitor.TabIndex = 0;
            this.signalExhibitor.MouseEnter += new System.EventHandler(this.signalExhibitor_MouseEnter);
            this.signalExhibitor.MouseLeave += new System.EventHandler(this.signalExhibitor_MouseLeave);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendSignalToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(135, 26);
            // 
            // sendSignalToolStripMenuItem
            // 
            this.sendSignalToolStripMenuItem.Name = "sendSignalToolStripMenuItem";
            this.sendSignalToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.sendSignalToolStripMenuItem.Text = "Send signal";
            this.sendSignalToolStripMenuItem.Click += new System.EventHandler(this.sendSignalToolStripMenuItem_Click);
            // 
            // chooseFileButton
            // 
            this.chooseFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseFileButton.Location = new System.Drawing.Point(883, 60);
            this.chooseFileButton.Margin = new System.Windows.Forms.Padding(2);
            this.chooseFileButton.Name = "chooseFileButton";
            this.chooseFileButton.Size = new System.Drawing.Size(149, 30);
            this.chooseFileButton.TabIndex = 1;
            this.chooseFileButton.Text = "Choose File";
            this.chooseFileButton.UseVisualStyleBackColor = true;
            this.chooseFileButton.Click += new System.EventHandler(this.chooseFileButton_Click);
            // 
            // detailsModifyButton
            // 
            this.detailsModifyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.detailsModifyButton.Location = new System.Drawing.Point(883, 105);
            this.detailsModifyButton.Margin = new System.Windows.Forms.Padding(2);
            this.detailsModifyButton.Name = "detailsModifyButton";
            this.detailsModifyButton.Size = new System.Drawing.Size(149, 30);
            this.detailsModifyButton.TabIndex = 2;
            this.detailsModifyButton.Text = "Details/Modify";
            this.detailsModifyButton.UseVisualStyleBackColor = true;
            this.detailsModifyButton.Click += new System.EventHandler(this.detailsModifyButton_Click);
            // 
            // backwardButton
            // 
            this.backwardButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.backwardButton.Location = new System.Drawing.Point(883, 195);
            this.backwardButton.Margin = new System.Windows.Forms.Padding(2);
            this.backwardButton.Name = "backwardButton";
            this.backwardButton.Size = new System.Drawing.Size(75, 30);
            this.backwardButton.TabIndex = 5;
            this.backwardButton.Text = "Backward";
            this.backwardButton.UseVisualStyleBackColor = true;
            this.backwardButton.Click += new System.EventHandler(this.backwardButton_Click);
            // 
            // forwardButton
            // 
            this.forwardButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.forwardButton.Location = new System.Drawing.Point(964, 195);
            this.forwardButton.Margin = new System.Windows.Forms.Padding(2);
            this.forwardButton.Name = "forwardButton";
            this.forwardButton.Size = new System.Drawing.Size(68, 30);
            this.forwardButton.TabIndex = 6;
            this.forwardButton.Text = "Forward";
            this.forwardButton.UseVisualStyleBackColor = true;
            this.forwardButton.Click += new System.EventHandler(this.forwardButton_Click);
            // 
            // pathLabel
            // 
            this.pathLabel.AutoSize = true;
            this.pathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.pathLabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pathLabel.Location = new System.Drawing.Point(4, 9);
            this.pathLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(37, 17);
            this.pathLabel.TabIndex = 7;
            this.pathLabel.Text = "Path";
            // 
            // signalSpanTextBox
            // 
            this.signalSpanTextBox.Location = new System.Drawing.Point(883, 156);
            this.signalSpanTextBox.Name = "signalSpanTextBox";
            this.signalSpanTextBox.PlaceholderText = "0";
            this.signalSpanTextBox.Size = new System.Drawing.Size(149, 23);
            this.signalSpanTextBox.TabIndex = 8;
            this.signalSpanTextBox.Text = "10";
            this.signalSpanTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.signalSpanTextBox.TextChanged += new System.EventHandler(this.signalSpanTextBox_TextChanged);
            this.signalSpanTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.signalSpanTextBox_KeyPress);
            // 
            // signalSpanLabel
            // 
            this.signalSpanLabel.AutoSize = true;
            this.signalSpanLabel.Location = new System.Drawing.Point(911, 138);
            this.signalSpanLabel.Name = "signalSpanLabel";
            this.signalSpanLabel.Size = new System.Drawing.Size(96, 15);
            this.signalSpanLabel.TabIndex = 9;
            this.signalSpanLabel.Text = "Signal span (Sec)";
            // 
            // SignalHolder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Controls.Add(this.signalSpanLabel);
            this.Controls.Add(this.signalSpanTextBox);
            this.Controls.Add(this.pathLabel);
            this.Controls.Add(this.forwardButton);
            this.Controls.Add(this.backwardButton);
            this.Controls.Add(this.detailsModifyButton);
            this.Controls.Add(this.chooseFileButton);
            this.Controls.Add(this.signalExhibitor);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SignalHolder";
            this.Size = new System.Drawing.Size(1045, 233);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ScottPlot.FormsPlot signalExhibitor;
        private System.Windows.Forms.Button chooseFileButton;
        private System.Windows.Forms.Button detailsModifyButton;
        private System.Windows.Forms.Button backwardButton;
        private System.Windows.Forms.Button forwardButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem sendSignalToolStripMenuItem;
        public System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.TextBox signalSpanTextBox;
        private System.Windows.Forms.Label signalSpanLabel;
    }
}

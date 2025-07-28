namespace Biological_Signal_Processing_Using_AI.DetailsModify.FiltersControls
{
    partial class AddGWNUserControl
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
            this.AdditiveGWNLabel = new System.Windows.Forms.Label();
            this.snrDBLabel = new System.Windows.Forms.Label();
            this.snrDBTextBox = new System.Windows.Forms.TextBox();
            this.activateCheckBox = new System.Windows.Forms.CheckBox();
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
            // AdditiveGWNLabel
            // 
            this.AdditiveGWNLabel.AutoSize = true;
            this.AdditiveGWNLabel.Location = new System.Drawing.Point(2, 2);
            this.AdditiveGWNLabel.Margin = new System.Windows.Forms.Padding(2);
            this.AdditiveGWNLabel.Name = "AdditiveGWNLabel";
            this.AdditiveGWNLabel.Size = new System.Drawing.Size(163, 15);
            this.AdditiveGWNLabel.TabIndex = 21;
            this.AdditiveGWNLabel.Text = "Additive white gaussian noise";
            // 
            // snrDBLabel
            // 
            this.snrDBLabel.AutoSize = true;
            this.snrDBLabel.Location = new System.Drawing.Point(143, 36);
            this.snrDBLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.snrDBLabel.Name = "snrDBLabel";
            this.snrDBLabel.Size = new System.Drawing.Size(54, 15);
            this.snrDBLabel.TabIndex = 25;
            this.snrDBLabel.Text = "SNR (dB)";
            // 
            // snrDBTextBox
            // 
            this.snrDBTextBox.ForeColor = System.Drawing.Color.Black;
            this.snrDBTextBox.Location = new System.Drawing.Point(143, 53);
            this.snrDBTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.snrDBTextBox.Name = "snrDBTextBox";
            this.snrDBTextBox.Size = new System.Drawing.Size(73, 23);
            this.snrDBTextBox.TabIndex = 24;
            this.snrDBTextBox.Text = "10";
            this.snrDBTextBox.TextChanged += new System.EventHandler(this.snrDBTextBox_TextChanged);
            this.snrDBTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.windowSizeTextBox_KeyPress);
            // 
            // activateCheckBox
            // 
            this.activateCheckBox.AutoSize = true;
            this.activateCheckBox.Checked = true;
            this.activateCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.activateCheckBox.Location = new System.Drawing.Point(2, 21);
            this.activateCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.activateCheckBox.Name = "activateCheckBox";
            this.activateCheckBox.Size = new System.Drawing.Size(69, 19);
            this.activateCheckBox.TabIndex = 26;
            this.activateCheckBox.Text = "Activate";
            this.activateCheckBox.UseVisualStyleBackColor = true;
            this.activateCheckBox.CheckedChanged += new System.EventHandler(this.activateCheckBox_CheckedChanged);
            // 
            // AddGWNUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.activateCheckBox);
            this.Controls.Add(this.snrDBLabel);
            this.Controls.Add(this.snrDBTextBox);
            this.Controls.Add(this.AdditiveGWNLabel);
            this.Name = "AddGWNUserControl";
            this.Size = new System.Drawing.Size(331, 78);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        public System.Windows.Forms.Label AdditiveGWNLabel;
        private System.Windows.Forms.Label snrDBLabel;
        public System.Windows.Forms.TextBox snrDBTextBox;
        public System.Windows.Forms.CheckBox activateCheckBox;
    }
}

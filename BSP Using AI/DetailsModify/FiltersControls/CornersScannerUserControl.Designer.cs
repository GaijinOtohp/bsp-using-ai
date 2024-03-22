
using Biological_Signal_Processing_Using_AI;

namespace BSP_Using_AI.DetailsModify.FiltersControls
{
    partial class CornersScannerUserControl
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
            this.artLabel = new System.Windows.Forms.Label();
            this.cornersScannerLabel = new System.Windows.Forms.Label();
            this.autoApplyCheckBox = new System.Windows.Forms.CheckBox();
            this.showCornersCheckBox = new System.Windows.Forms.CheckBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.angleThresholdLabel = new System.Windows.Forms.Label();
            this.showDeviationCheckBox = new System.Windows.Forms.CheckBox();
            this.artValueTextBox = new System.Windows.Forms.TextBox();
            this.atDegreeLabel = new System.Windows.Forms.Label();
            this.atValueTextBox = new System.Windows.Forms.TextBox();
            this.angleThresholdScrollBar = new Biological_Signal_Processing_Using_AI.CustomHScrollBar();
            this.artScrollBar = new Biological_Signal_Processing_Using_AI.CustomVScrollBar();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // artLabel
            // 
            this.artLabel.AutoSize = true;
            this.artLabel.Location = new System.Drawing.Point(234, 44);
            this.artLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.artLabel.Name = "artLabel";
            this.artLabel.Size = new System.Drawing.Size(27, 15);
            this.artLabel.TabIndex = 19;
            this.artLabel.Text = "ART";
            // 
            // cornersScannerLabel
            // 
            this.cornersScannerLabel.AutoSize = true;
            this.cornersScannerLabel.Location = new System.Drawing.Point(2, 2);
            this.cornersScannerLabel.Margin = new System.Windows.Forms.Padding(2);
            this.cornersScannerLabel.Name = "cornersScannerLabel";
            this.cornersScannerLabel.Size = new System.Drawing.Size(92, 15);
            this.cornersScannerLabel.TabIndex = 21;
            this.cornersScannerLabel.Text = "Corners scanner";
            // 
            // autoApplyCheckBox
            // 
            this.autoApplyCheckBox.AutoSize = true;
            this.autoApplyCheckBox.Checked = true;
            this.autoApplyCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoApplyCheckBox.Location = new System.Drawing.Point(5, 50);
            this.autoApplyCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.autoApplyCheckBox.Name = "autoApplyCheckBox";
            this.autoApplyCheckBox.Size = new System.Drawing.Size(84, 19);
            this.autoApplyCheckBox.TabIndex = 22;
            this.autoApplyCheckBox.Text = "Auto apply";
            this.autoApplyCheckBox.UseVisualStyleBackColor = true;
            this.autoApplyCheckBox.CheckedChanged += new System.EventHandler(this.autoApplyCheckBox_CheckedChanged);
            // 
            // showCornersCheckBox
            // 
            this.showCornersCheckBox.AutoSize = true;
            this.showCornersCheckBox.Checked = true;
            this.showCornersCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showCornersCheckBox.Location = new System.Drawing.Point(5, 24);
            this.showCornersCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.showCornersCheckBox.Name = "showCornersCheckBox";
            this.showCornersCheckBox.Size = new System.Drawing.Size(97, 19);
            this.showCornersCheckBox.TabIndex = 23;
            this.showCornersCheckBox.Text = "Show corners";
            this.showCornersCheckBox.UseVisualStyleBackColor = true;
            this.showCornersCheckBox.CheckedChanged += new System.EventHandler(this.showCheckBox_CheckedChanged);
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.applyButton.Location = new System.Drawing.Point(264, 152);
            this.applyButton.Margin = new System.Windows.Forms.Padding(2);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(65, 23);
            this.applyButton.TabIndex = 24;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
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
            // angleThresholdLabel
            // 
            this.angleThresholdLabel.AutoSize = true;
            this.angleThresholdLabel.Location = new System.Drawing.Point(141, 156);
            this.angleThresholdLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.angleThresholdLabel.Name = "angleThresholdLabel";
            this.angleThresholdLabel.Size = new System.Drawing.Size(23, 15);
            this.angleThresholdLabel.TabIndex = 28;
            this.angleThresholdLabel.Text = "AT:";
            // 
            // showDeviationCheckBox
            // 
            this.showDeviationCheckBox.AutoSize = true;
            this.showDeviationCheckBox.Location = new System.Drawing.Point(5, 75);
            this.showDeviationCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.showDeviationCheckBox.Name = "showDeviationCheckBox";
            this.showDeviationCheckBox.Size = new System.Drawing.Size(139, 19);
            this.showDeviationCheckBox.TabIndex = 29;
            this.showDeviationCheckBox.Text = "Show deviation angle";
            this.showDeviationCheckBox.UseVisualStyleBackColor = true;
            this.showDeviationCheckBox.CheckedChanged += new System.EventHandler(this.showDeviationAnglesCheckBox_CheckedChanged);
            // 
            // artValueTextBox
            // 
            this.artValueTextBox.Location = new System.Drawing.Point(237, 62);
            this.artValueTextBox.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.artValueTextBox.Name = "artValueTextBox";
            this.artValueTextBox.Size = new System.Drawing.Size(50, 23);
            this.artValueTextBox.TabIndex = 30;
            this.artValueTextBox.Text = "0.2";
            this.artValueTextBox.TextChanged += new System.EventHandler(this.artValueTextBox_TextChanged);
            this.artValueTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.artValueTextBox_KeyPress);
            // 
            // atDegreeLabel
            // 
            this.atDegreeLabel.AutoSize = true;
            this.atDegreeLabel.Location = new System.Drawing.Point(200, 155);
            this.atDegreeLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.atDegreeLabel.Name = "atDegreeLabel";
            this.atDegreeLabel.Size = new System.Drawing.Size(12, 15);
            this.atDegreeLabel.TabIndex = 33;
            this.atDegreeLabel.Text = "°";
            // 
            // atValueTextBox
            // 
            this.atValueTextBox.Location = new System.Drawing.Point(166, 152);
            this.atValueTextBox.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.atValueTextBox.Name = "atValueTextBox";
            this.atValueTextBox.Size = new System.Drawing.Size(32, 23);
            this.atValueTextBox.TabIndex = 32;
            this.atValueTextBox.Text = "20";
            this.atValueTextBox.TextChanged += new System.EventHandler(this.atValueTextBox_TextChanged);
            this.atValueTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.artValueTextBox_KeyPress);
            // 
            // angleThresholdScrollBar
            // 
            this.angleThresholdScrollBar.Location = new System.Drawing.Point(4, 126);
            this.angleThresholdScrollBar.Maximum = 3609;
            this.angleThresholdScrollBar.Name = "angleThresholdScrollBar";
            this.angleThresholdScrollBar.Size = new System.Drawing.Size(303, 21);
            this.angleThresholdScrollBar.TabIndex = 27;
            this.angleThresholdScrollBar.Value = 200;
            this.angleThresholdScrollBar.ValueChanged += new System.EventHandler(this.angleThresholdScrollBar_ValueChanged);
            // 
            // artScrollBar
            // 
            this.artScrollBar.Location = new System.Drawing.Point(310, 0);
            this.artScrollBar.Maximum = 1009;
            this.artScrollBar.Name = "artScrollBar";
            this.artScrollBar.Size = new System.Drawing.Size(21, 147);
            this.artScrollBar.TabIndex = 25;
            this.artScrollBar.Value = 800;
            this.artScrollBar.ValueChanged += new System.EventHandler(this.artScrollBar_ValueChanged);
            // 
            // CornersScannerUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.artScrollBar);
            this.Controls.Add(this.angleThresholdScrollBar);
            this.Controls.Add(this.atDegreeLabel);
            this.Controls.Add(this.atValueTextBox);
            this.Controls.Add(this.artValueTextBox);
            this.Controls.Add(this.showDeviationCheckBox);
            this.Controls.Add(this.angleThresholdLabel);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.showCornersCheckBox);
            this.Controls.Add(this.autoApplyCheckBox);
            this.Controls.Add(this.cornersScannerLabel);
            this.Controls.Add(this.artLabel);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "CornersScannerUserControl";
            this.Size = new System.Drawing.Size(331, 180);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label artLabel;
        public System.Windows.Forms.Label cornersScannerLabel;
        public System.Windows.Forms.CheckBox autoApplyCheckBox;
        public System.Windows.Forms.CheckBox showCornersCheckBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        public System.Windows.Forms.Label angleThresholdLabel;
        public System.Windows.Forms.CheckBox showDeviationCheckBox;
        public System.Windows.Forms.Button applyButton;
        public System.Windows.Forms.Label atDegreeLabel;
        public CustomHScrollBar angleThresholdScrollBar;
        public System.Windows.Forms.TextBox artValueTextBox;
        public System.Windows.Forms.TextBox atValueTextBox;
        public CustomVScrollBar artScrollBar;
    }
}

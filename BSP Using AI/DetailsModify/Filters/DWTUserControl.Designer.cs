namespace Biological_Signal_Processing_Using_AI.DetailsModify.Filters
{
    partial class DWTUserControl
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
            this.numOfVanMoComboBox = new System.Windows.Forms.ComboBox();
            this.numOfVanishingLabel = new System.Windows.Forms.Label();
            this.waveletTypeComboBox = new System.Windows.Forms.ComboBox();
            this.waveletTypeLabel = new System.Windows.Forms.Label();
            this.signalStatesViewerLabel = new System.Windows.Forms.Label();
            this.levelComboBox = new System.Windows.Forms.ComboBox();
            this.selectedLevelLabel = new System.Windows.Forms.Label();
            this.separatorLabel = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // numOfVanMoComboBox
            // 
            this.numOfVanMoComboBox.DisplayMember = "1";
            this.numOfVanMoComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.numOfVanMoComboBox.FormattingEnabled = true;
            this.numOfVanMoComboBox.Location = new System.Drawing.Point(154, 53);
            this.numOfVanMoComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.numOfVanMoComboBox.Name = "numOfVanMoComboBox";
            this.numOfVanMoComboBox.Size = new System.Drawing.Size(129, 21);
            this.numOfVanMoComboBox.TabIndex = 20;
            this.numOfVanMoComboBox.Tag = "";
            this.numOfVanMoComboBox.SelectedIndexChanged += new System.EventHandler(this.numOfVanMoComboBox_SelectedIndexChanged);
            // 
            // numOfVanishingLabel
            // 
            this.numOfVanishingLabel.AutoSize = true;
            this.numOfVanishingLabel.Location = new System.Drawing.Point(1, 58);
            this.numOfVanishingLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.numOfVanishingLabel.Name = "numOfVanishingLabel";
            this.numOfVanishingLabel.Size = new System.Drawing.Size(149, 13);
            this.numOfVanishingLabel.TabIndex = 19;
            this.numOfVanishingLabel.Text = "Number of vanishing moments";
            // 
            // waveletTypeComboBox
            // 
            this.waveletTypeComboBox.DisplayMember = "1";
            this.waveletTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.waveletTypeComboBox.FormattingEnabled = true;
            this.waveletTypeComboBox.Items.AddRange(new object[] {
            "Haar",
            "Daubechies",
            "Symlet",
            "Coiflet"});
            this.waveletTypeComboBox.Location = new System.Drawing.Point(154, 28);
            this.waveletTypeComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.waveletTypeComboBox.Name = "waveletTypeComboBox";
            this.waveletTypeComboBox.Size = new System.Drawing.Size(129, 21);
            this.waveletTypeComboBox.TabIndex = 18;
            this.waveletTypeComboBox.Tag = "";
            this.waveletTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.waveletTypeComboBox_SelectedIndexChanged);
            // 
            // waveletTypeLabel
            // 
            this.waveletTypeLabel.AutoSize = true;
            this.waveletTypeLabel.Location = new System.Drawing.Point(1, 34);
            this.waveletTypeLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.waveletTypeLabel.Name = "waveletTypeLabel";
            this.waveletTypeLabel.Size = new System.Drawing.Size(70, 13);
            this.waveletTypeLabel.TabIndex = 17;
            this.waveletTypeLabel.Text = "Wavelet type";
            // 
            // signalStatesViewerLabel
            // 
            this.signalStatesViewerLabel.AutoSize = true;
            this.signalStatesViewerLabel.Location = new System.Drawing.Point(2, 2);
            this.signalStatesViewerLabel.Margin = new System.Windows.Forms.Padding(2);
            this.signalStatesViewerLabel.Name = "signalStatesViewerLabel";
            this.signalStatesViewerLabel.Size = new System.Drawing.Size(174, 13);
            this.signalStatesViewerLabel.TabIndex = 22;
            this.signalStatesViewerLabel.Text = "Discrete Wavelet Transform (DWT)";
            // 
            // levelComboBox
            // 
            this.levelComboBox.DisplayMember = "1";
            this.levelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.levelComboBox.FormattingEnabled = true;
            this.levelComboBox.Location = new System.Drawing.Point(154, 93);
            this.levelComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.levelComboBox.Name = "levelComboBox";
            this.levelComboBox.Size = new System.Drawing.Size(129, 21);
            this.levelComboBox.TabIndex = 24;
            this.levelComboBox.Tag = "";
            this.levelComboBox.SelectedIndexChanged += new System.EventHandler(this.levelComboBox_SelectedIndexChanged);
            // 
            // selectedLevelLabel
            // 
            this.selectedLevelLabel.AutoSize = true;
            this.selectedLevelLabel.Location = new System.Drawing.Point(1, 98);
            this.selectedLevelLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.selectedLevelLabel.Name = "selectedLevelLabel";
            this.selectedLevelLabel.Size = new System.Drawing.Size(74, 13);
            this.selectedLevelLabel.TabIndex = 23;
            this.selectedLevelLabel.Text = "Selected level";
            // 
            // separatorLabel
            // 
            this.separatorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separatorLabel.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.separatorLabel.Location = new System.Drawing.Point(0, 89);
            this.separatorLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.separatorLabel.Name = "separatorLabel";
            this.separatorLabel.Size = new System.Drawing.Size(284, 1);
            this.separatorLabel.TabIndex = 25;
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
            // DWTUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.separatorLabel);
            this.Controls.Add(this.levelComboBox);
            this.Controls.Add(this.selectedLevelLabel);
            this.Controls.Add(this.signalStatesViewerLabel);
            this.Controls.Add(this.numOfVanMoComboBox);
            this.Controls.Add(this.numOfVanishingLabel);
            this.Controls.Add(this.waveletTypeComboBox);
            this.Controls.Add(this.waveletTypeLabel);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "DWTUserControl";
            this.Size = new System.Drawing.Size(284, 126);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label numOfVanishingLabel;
        private System.Windows.Forms.Label waveletTypeLabel;
        public System.Windows.Forms.Label signalStatesViewerLabel;
        private System.Windows.Forms.Label selectedLevelLabel;
        private System.Windows.Forms.Label separatorLabel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        public System.Windows.Forms.ComboBox numOfVanMoComboBox;
        public System.Windows.Forms.ComboBox waveletTypeComboBox;
        public System.Windows.Forms.ComboBox levelComboBox;
    }
}

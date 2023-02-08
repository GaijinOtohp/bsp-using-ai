
namespace BSP_Using_AI.DetailsModify.Filters.IIRFilters
{
    partial class IIRFilterUserControl
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
            this.cutoffFreqLabel = new System.Windows.Forms.Label();
            this.minFreqTextBox = new System.Windows.Forms.TextBox();
            this.maxFreqTextBox = new System.Windows.Forms.TextBox();
            this.filterBandComboBox = new System.Windows.Forms.ComboBox();
            this.nameFilterLabel = new System.Windows.Forms.Label();
            this.orderTextBox = new System.Windows.Forms.TextBox();
            this.orderLabel = new System.Windows.Forms.Label();
            this.minFreqLabel = new System.Windows.Forms.Label();
            this.maxFreqLabel = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.frequencyScrollBar = new Biological_Signal_Processing_Using_AI.CustomHScrollBar();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cutoffFreqLabel
            // 
            this.cutoffFreqLabel.AutoSize = true;
            this.cutoffFreqLabel.Location = new System.Drawing.Point(118, 128);
            this.cutoffFreqLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.cutoffFreqLabel.Name = "cutoffFreqLabel";
            this.cutoffFreqLabel.Size = new System.Drawing.Size(59, 13);
            this.cutoffFreqLabel.TabIndex = 17;
            this.cutoffFreqLabel.Text = "Cutoff freq:";
            // 
            // minFreqTextBox
            // 
            this.minFreqTextBox.ForeColor = System.Drawing.Color.Black;
            this.minFreqTextBox.Location = new System.Drawing.Point(2, 84);
            this.minFreqTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.minFreqTextBox.Name = "minFreqTextBox";
            this.minFreqTextBox.Size = new System.Drawing.Size(51, 20);
            this.minFreqTextBox.TabIndex = 16;
            this.minFreqTextBox.Text = "0";
            this.minFreqTextBox.TextChanged += new System.EventHandler(this.minFreqTextBox_TextChanged);
            this.minFreqTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.minFreqTextBox_KeyPress);
            // 
            // maxFreqTextBox
            // 
            this.maxFreqTextBox.ForeColor = System.Drawing.Color.Black;
            this.maxFreqTextBox.Location = new System.Drawing.Point(231, 84);
            this.maxFreqTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.maxFreqTextBox.Name = "maxFreqTextBox";
            this.maxFreqTextBox.Size = new System.Drawing.Size(51, 20);
            this.maxFreqTextBox.TabIndex = 15;
            this.maxFreqTextBox.TextChanged += new System.EventHandler(this.maxFreqTextBox_TextChanged);
            this.maxFreqTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.maxFreqTextBox_KeyPress);
            // 
            // filterBandComboBox
            // 
            this.filterBandComboBox.DisplayMember = "1";
            this.filterBandComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filterBandComboBox.FormattingEnabled = true;
            this.filterBandComboBox.Items.AddRange(new object[] {
            "Lowpass",
            "Highpass"});
            this.filterBandComboBox.Location = new System.Drawing.Point(192, 28);
            this.filterBandComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.filterBandComboBox.Name = "filterBandComboBox";
            this.filterBandComboBox.Size = new System.Drawing.Size(90, 21);
            this.filterBandComboBox.TabIndex = 14;
            this.filterBandComboBox.Tag = "";
            this.filterBandComboBox.SelectedIndexChanged += new System.EventHandler(this.filterTypeComboBox_SelectedIndexChanged);
            // 
            // nameFilterLabel
            // 
            this.nameFilterLabel.AutoSize = true;
            this.nameFilterLabel.Location = new System.Drawing.Point(2, 2);
            this.nameFilterLabel.Margin = new System.Windows.Forms.Padding(2);
            this.nameFilterLabel.Name = "nameFilterLabel";
            this.nameFilterLabel.Size = new System.Drawing.Size(57, 13);
            this.nameFilterLabel.TabIndex = 13;
            this.nameFilterLabel.Text = "Name filter";
            // 
            // orderTextBox
            // 
            this.orderTextBox.ForeColor = System.Drawing.Color.Black;
            this.orderTextBox.Location = new System.Drawing.Point(44, 52);
            this.orderTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.orderTextBox.Name = "orderTextBox";
            this.orderTextBox.Size = new System.Drawing.Size(51, 20);
            this.orderTextBox.TabIndex = 19;
            this.orderTextBox.Text = "4";
            this.orderTextBox.TextChanged += new System.EventHandler(this.orderTextBox_TextChanged);
            this.orderTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.orderTextBox_KeyPress);
            // 
            // orderLabel
            // 
            this.orderLabel.AutoSize = true;
            this.orderLabel.Location = new System.Drawing.Point(2, 54);
            this.orderLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.orderLabel.Name = "orderLabel";
            this.orderLabel.Size = new System.Drawing.Size(36, 13);
            this.orderLabel.TabIndex = 20;
            this.orderLabel.Text = "Order:";
            // 
            // minFreqLabel
            // 
            this.minFreqLabel.AutoSize = true;
            this.minFreqLabel.Location = new System.Drawing.Point(57, 86);
            this.minFreqLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.minFreqLabel.Name = "minFreqLabel";
            this.minFreqLabel.Size = new System.Drawing.Size(45, 13);
            this.minFreqLabel.TabIndex = 21;
            this.minFreqLabel.Text = "Min freq";
            // 
            // maxFreqLabel
            // 
            this.maxFreqLabel.AutoSize = true;
            this.maxFreqLabel.Location = new System.Drawing.Point(182, 86);
            this.maxFreqLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.maxFreqLabel.Name = "maxFreqLabel";
            this.maxFreqLabel.Size = new System.Drawing.Size(48, 13);
            this.maxFreqLabel.TabIndex = 22;
            this.maxFreqLabel.Text = "Max freq";
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
            // frequencyScrollBar
            // 
            this.frequencyScrollBar.Location = new System.Drawing.Point(2, 106);
            this.frequencyScrollBar.Name = "frequencyScrollBar";
            this.frequencyScrollBar.Size = new System.Drawing.Size(279, 21);
            this.frequencyScrollBar.TabIndex = 18;
            this.frequencyScrollBar.Value = 100;
            this.frequencyScrollBar.ValueChanged += new System.EventHandler(this.frequencyScrollBar_ValueChanged);
            // 
            // IIRFilterUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.frequencyScrollBar);
            this.Controls.Add(this.maxFreqLabel);
            this.Controls.Add(this.minFreqLabel);
            this.Controls.Add(this.orderLabel);
            this.Controls.Add(this.orderTextBox);
            this.Controls.Add(this.cutoffFreqLabel);
            this.Controls.Add(this.minFreqTextBox);
            this.Controls.Add(this.maxFreqTextBox);
            this.Controls.Add(this.filterBandComboBox);
            this.Controls.Add(this.nameFilterLabel);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "IIRFilterUserControl";
            this.Size = new System.Drawing.Size(284, 148);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label cutoffFreqLabel;
        public System.Windows.Forms.TextBox minFreqTextBox;
        public System.Windows.Forms.TextBox maxFreqTextBox;
        public System.Windows.Forms.ComboBox filterBandComboBox;
        public System.Windows.Forms.Label nameFilterLabel;
        public System.Windows.Forms.TextBox orderTextBox;
        private System.Windows.Forms.Label orderLabel;
        private System.Windows.Forms.Label minFreqLabel;
        private System.Windows.Forms.Label maxFreqLabel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        public Biological_Signal_Processing_Using_AI.CustomHScrollBar frequencyScrollBar;
    }
}

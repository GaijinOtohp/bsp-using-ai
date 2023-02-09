
namespace BSP_Using_AI.MainFormFolder.SignalsCollectionFolder
{
    partial class FormSignalsCollector
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.selectSignalCheckBox = new System.Windows.Forms.CheckBox();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.signalsFlowLayoutPanel = new BSP_Using_AI.CustomFlowLayoutPanel();
            this.SuspendLayout();
            // 
            // selectSignalCheckBox
            // 
            this.selectSignalCheckBox.AutoSize = true;
            this.selectSignalCheckBox.Location = new System.Drawing.Point(12, 12);
            this.selectSignalCheckBox.Name = "selectSignalCheckBox";
            this.selectSignalCheckBox.Size = new System.Drawing.Size(110, 21);
            this.selectSignalCheckBox.TabIndex = 24;
            this.selectSignalCheckBox.Text = "Select signal";
            this.selectSignalCheckBox.UseVisualStyleBackColor = true;
            // 
            // vScrollBar
            // 
            this.vScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScrollBar.LargeChange = 376;
            this.vScrollBar.Location = new System.Drawing.Point(1190, 60);
            this.vScrollBar.Maximum = 375;
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(21, 462);
            this.vScrollBar.SmallChange = 20;
            this.vScrollBar.TabIndex = 25;
            this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // signalsFlowLayoutPanel
            // 
            this.signalsFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.signalsFlowLayoutPanel.Location = new System.Drawing.Point(12, 60);
            this.signalsFlowLayoutPanel.Name = "signalsFlowLayoutPanel";
            this.signalsFlowLayoutPanel.Size = new System.Drawing.Size(1175, 462);
            this.signalsFlowLayoutPanel.TabIndex = 26;
            this.signalsFlowLayoutPanel.SizeChanged += new System.EventHandler(this.signalsFlowLayoutPanel_SizeChanged);
            this.signalsFlowLayoutPanel.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseWheelScroll);
            // 
            // FormSignalsCollector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1218, 534);
            this.Controls.Add(this.signalsFlowLayoutPanel);
            this.Controls.Add(this.vScrollBar);
            this.Controls.Add(this.selectSignalCheckBox);
            this.Name = "FormSignalsCollector";
            this.Text = "FormSignalsCollector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.CheckBox selectSignalCheckBox;
        public System.Windows.Forms.VScrollBar vScrollBar;
        private CustomFlowLayoutPanel signalsFlowLayoutPanel;
    }
}
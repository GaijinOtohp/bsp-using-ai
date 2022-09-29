
namespace BSP_Using_AI.SignalHolderFolder.Input
{
    partial class InputForm
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
            this.titleLabel = new System.Windows.Forms.Label();
            this.inputFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.Location = new System.Drawing.Point(9, 7);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(265, 45);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Title";
            // 
            // inputFlowLayoutPanel
            // 
            this.inputFlowLayoutPanel.AutoScroll = true;
            this.inputFlowLayoutPanel.Location = new System.Drawing.Point(9, 54);
            this.inputFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.inputFlowLayoutPanel.Name = "inputFlowLayoutPanel";
            this.inputFlowLayoutPanel.Size = new System.Drawing.Size(279, 224);
            this.inputFlowLayoutPanel.TabIndex = 1;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(220, 282);
            this.okButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(68, 26);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // InputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 314);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.inputFlowLayoutPanel);
            this.Controls.Add(this.titleLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputForm";
            this.Text = "InputForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label titleLabel;
        public System.Windows.Forms.FlowLayoutPanel inputFlowLayoutPanel;
        private System.Windows.Forms.Button okButton;
    }
}
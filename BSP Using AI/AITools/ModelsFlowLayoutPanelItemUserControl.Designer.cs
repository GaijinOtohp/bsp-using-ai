
namespace BSP_Using_AI.AITools
{
    partial class ModelsFlowLayoutPanelItemUserControl
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
            this.modelNameLabel = new System.Windows.Forms.Label();
            this.datasetSizeLabel = new System.Windows.Forms.Label();
            this.updatesLabel = new System.Windows.Forms.Label();
            this.detailsButton = new System.Windows.Forms.Button();
            this.unfittedDataLabel = new System.Windows.Forms.Label();
            this.fitProgressBar = new System.Windows.Forms.ProgressBar();
            this.fitButton = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // modelNameLabel
            // 
            this.modelNameLabel.AutoSize = true;
            this.modelNameLabel.Location = new System.Drawing.Point(3, 3);
            this.modelNameLabel.Margin = new System.Windows.Forms.Padding(3);
            this.modelNameLabel.MaximumSize = new System.Drawing.Size(163, 0);
            this.modelNameLabel.MinimumSize = new System.Drawing.Size(163, 23);
            this.modelNameLabel.Name = "modelNameLabel";
            this.modelNameLabel.Size = new System.Drawing.Size(163, 23);
            this.modelNameLabel.TabIndex = 2;
            this.modelNameLabel.Text = "Model name";
            this.modelNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // datasetSizeLabel
            // 
            this.datasetSizeLabel.Location = new System.Drawing.Point(172, 3);
            this.datasetSizeLabel.Margin = new System.Windows.Forms.Padding(3);
            this.datasetSizeLabel.Name = "datasetSizeLabel";
            this.datasetSizeLabel.Size = new System.Drawing.Size(87, 23);
            this.datasetSizeLabel.TabIndex = 3;
            this.datasetSizeLabel.Text = "Dataset size";
            this.datasetSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // updatesLabel
            // 
            this.updatesLabel.Location = new System.Drawing.Point(265, 3);
            this.updatesLabel.Margin = new System.Windows.Forms.Padding(3);
            this.updatesLabel.Name = "updatesLabel";
            this.updatesLabel.Size = new System.Drawing.Size(53, 23);
            this.updatesLabel.TabIndex = 4;
            this.updatesLabel.Text = "Updates";
            this.updatesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // detailsButton
            // 
            this.detailsButton.Location = new System.Drawing.Point(419, 3);
            this.detailsButton.Margin = new System.Windows.Forms.Padding(2);
            this.detailsButton.Name = "detailsButton";
            this.detailsButton.Size = new System.Drawing.Size(62, 23);
            this.detailsButton.TabIndex = 19;
            this.detailsButton.Text = "Details";
            this.detailsButton.UseVisualStyleBackColor = true;
            this.detailsButton.Click += new System.EventHandler(this.detailsButton_Click);
            // 
            // unfittedDataLabel
            // 
            this.unfittedDataLabel.AutoSize = true;
            this.unfittedDataLabel.Location = new System.Drawing.Point(324, 3);
            this.unfittedDataLabel.Margin = new System.Windows.Forms.Padding(3);
            this.unfittedDataLabel.MaximumSize = new System.Drawing.Size(90, 0);
            this.unfittedDataLabel.MinimumSize = new System.Drawing.Size(90, 23);
            this.unfittedDataLabel.Name = "unfittedDataLabel";
            this.unfittedDataLabel.Size = new System.Drawing.Size(90, 23);
            this.unfittedDataLabel.TabIndex = 20;
            this.unfittedDataLabel.Text = "Unfitted data";
            this.unfittedDataLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fitProgressBar
            // 
            this.fitProgressBar.BackColor = System.Drawing.SystemColors.Control;
            this.fitProgressBar.Location = new System.Drawing.Point(517, 3);
            this.fitProgressBar.Name = "fitProgressBar";
            this.fitProgressBar.Size = new System.Drawing.Size(457, 23);
            this.fitProgressBar.TabIndex = 21;
            // 
            // fitButton
            // 
            this.fitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fitButton.Location = new System.Drawing.Point(1010, 2);
            this.fitButton.Margin = new System.Windows.Forms.Padding(2);
            this.fitButton.Name = "fitButton";
            this.fitButton.Size = new System.Drawing.Size(62, 24);
            this.fitButton.TabIndex = 22;
            this.fitButton.Text = "Fit";
            this.fitButton.UseVisualStyleBackColor = true;
            this.fitButton.Click += new System.EventHandler(this.fitButton_Click);
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
            // ModelsFlowLayoutPanelItemUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.fitButton);
            this.Controls.Add(this.fitProgressBar);
            this.Controls.Add(this.unfittedDataLabel);
            this.Controls.Add(this.detailsButton);
            this.Controls.Add(this.updatesLabel);
            this.Controls.Add(this.datasetSizeLabel);
            this.Controls.Add(this.modelNameLabel);
            this.Name = "ModelsFlowLayoutPanelItemUserControl";
            this.Size = new System.Drawing.Size(1074, 30);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button detailsButton;
        private System.Windows.Forms.Button fitButton;
        public System.Windows.Forms.Label modelNameLabel;
        public System.Windows.Forms.Label datasetSizeLabel;
        public System.Windows.Forms.Label updatesLabel;
        public System.Windows.Forms.Label unfittedDataLabel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        public System.Windows.Forms.ProgressBar fitProgressBar;
    }
}

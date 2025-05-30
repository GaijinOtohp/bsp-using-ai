﻿
namespace BSP_Using_AI
{
    partial class AIToolsForm
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
            this.modelsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.availableModelsListLabel = new System.Windows.Forms.Label();
            this.createNewModelButton = new System.Windows.Forms.Button();
            this.modelTypeComboBox = new System.Windows.Forms.ComboBox();
            this.unfittedDataLabel = new System.Windows.Forms.Label();
            this.updatesLabel = new System.Windows.Forms.Label();
            this.datasetSizeLabel = new System.Windows.Forms.Label();
            this.modelNameLabel = new System.Windows.Forms.Label();
            this.DatasetExplorerButton = new System.Windows.Forms.Button();
            this.aiGoalComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // modelsFlowLayoutPanel
            // 
            this.modelsFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.modelsFlowLayoutPanel.AutoScroll = true;
            this.modelsFlowLayoutPanel.Location = new System.Drawing.Point(4, 137);
            this.modelsFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.modelsFlowLayoutPanel.Name = "modelsFlowLayoutPanel";
            this.modelsFlowLayoutPanel.Size = new System.Drawing.Size(1253, 497);
            this.modelsFlowLayoutPanel.TabIndex = 0;
            this.modelsFlowLayoutPanel.SizeChanged += new System.EventHandler(this.modelsFlowLayoutPanel_SizeChanged);
            // 
            // availableModelsListLabel
            // 
            this.availableModelsListLabel.AutoSize = true;
            this.availableModelsListLabel.Location = new System.Drawing.Point(14, 70);
            this.availableModelsListLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.availableModelsListLabel.Name = "availableModelsListLabel";
            this.availableModelsListLabel.Size = new System.Drawing.Size(115, 15);
            this.availableModelsListLabel.TabIndex = 1;
            this.availableModelsListLabel.Text = "Available models list";
            // 
            // createNewModelButton
            // 
            this.createNewModelButton.Location = new System.Drawing.Point(13, 13);
            this.createNewModelButton.Margin = new System.Windows.Forms.Padding(2);
            this.createNewModelButton.Name = "createNewModelButton";
            this.createNewModelButton.Size = new System.Drawing.Size(126, 25);
            this.createNewModelButton.TabIndex = 18;
            this.createNewModelButton.Text = "Create new model";
            this.createNewModelButton.UseVisualStyleBackColor = true;
            this.createNewModelButton.Click += new System.EventHandler(this.createNewModelButton_Click);
            // 
            // modelTypeComboBox
            // 
            this.modelTypeComboBox.DisplayMember = "1";
            this.modelTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modelTypeComboBox.FormattingEnabled = true;
            this.modelTypeComboBox.Items.AddRange(new object[] {
            "Neural network",
            "K-Nearest neighbors",
            "Naive bayes",
            "TF.NET Neural network",
            "TF.Keras Neural network",
            "CNTK Neural network",
            "Reinforecment learning"});
            this.modelTypeComboBox.Location = new System.Drawing.Point(145, 14);
            this.modelTypeComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.modelTypeComboBox.Name = "modelTypeComboBox";
            this.modelTypeComboBox.Size = new System.Drawing.Size(150, 23);
            this.modelTypeComboBox.TabIndex = 17;
            this.modelTypeComboBox.Tag = "";
            // 
            // unfittedDataLabel
            // 
            this.unfittedDataLabel.AutoSize = true;
            this.unfittedDataLabel.Location = new System.Drawing.Point(374, 104);
            this.unfittedDataLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.unfittedDataLabel.MaximumSize = new System.Drawing.Size(105, 0);
            this.unfittedDataLabel.MinimumSize = new System.Drawing.Size(105, 27);
            this.unfittedDataLabel.Name = "unfittedDataLabel";
            this.unfittedDataLabel.Size = new System.Drawing.Size(105, 27);
            this.unfittedDataLabel.TabIndex = 24;
            this.unfittedDataLabel.Text = "Unfitted data";
            this.unfittedDataLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // updatesLabel
            // 
            this.updatesLabel.Location = new System.Drawing.Point(306, 104);
            this.updatesLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.updatesLabel.Name = "updatesLabel";
            this.updatesLabel.Size = new System.Drawing.Size(62, 27);
            this.updatesLabel.TabIndex = 23;
            this.updatesLabel.Text = "Updates";
            this.updatesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // datasetSizeLabel
            // 
            this.datasetSizeLabel.Location = new System.Drawing.Point(197, 104);
            this.datasetSizeLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.datasetSizeLabel.Name = "datasetSizeLabel";
            this.datasetSizeLabel.Size = new System.Drawing.Size(102, 27);
            this.datasetSizeLabel.TabIndex = 22;
            this.datasetSizeLabel.Text = "Dataset size";
            this.datasetSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // modelNameLabel
            // 
            this.modelNameLabel.AutoSize = true;
            this.modelNameLabel.Location = new System.Drawing.Point(0, 104);
            this.modelNameLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.modelNameLabel.MaximumSize = new System.Drawing.Size(190, 0);
            this.modelNameLabel.MinimumSize = new System.Drawing.Size(190, 27);
            this.modelNameLabel.Name = "modelNameLabel";
            this.modelNameLabel.Size = new System.Drawing.Size(190, 27);
            this.modelNameLabel.TabIndex = 21;
            this.modelNameLabel.Text = "Model name";
            this.modelNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DatasetExplorerButton
            // 
            this.DatasetExplorerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DatasetExplorerButton.Location = new System.Drawing.Point(1119, 12);
            this.DatasetExplorerButton.Margin = new System.Windows.Forms.Padding(2);
            this.DatasetExplorerButton.Name = "DatasetExplorerButton";
            this.DatasetExplorerButton.Size = new System.Drawing.Size(126, 25);
            this.DatasetExplorerButton.TabIndex = 25;
            this.DatasetExplorerButton.Text = "Dataset explorer";
            this.DatasetExplorerButton.UseVisualStyleBackColor = true;
            this.DatasetExplorerButton.Click += new System.EventHandler(this.DatasetExplorerButton_Click);
            // 
            // aiGoalComboBox
            // 
            this.aiGoalComboBox.DisplayMember = "1";
            this.aiGoalComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.aiGoalComboBox.FormattingEnabled = true;
            this.aiGoalComboBox.Items.AddRange(new object[] {
            "WPW syndrome detection"});
            this.aiGoalComboBox.Location = new System.Drawing.Point(300, 14);
            this.aiGoalComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.aiGoalComboBox.Name = "aiGoalComboBox";
            this.aiGoalComboBox.Size = new System.Drawing.Size(150, 23);
            this.aiGoalComboBox.TabIndex = 28;
            this.aiGoalComboBox.Tag = "";
            // 
            // AIToolsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1258, 636);
            this.Controls.Add(this.aiGoalComboBox);
            this.Controls.Add(this.DatasetExplorerButton);
            this.Controls.Add(this.unfittedDataLabel);
            this.Controls.Add(this.updatesLabel);
            this.Controls.Add(this.datasetSizeLabel);
            this.Controls.Add(this.modelNameLabel);
            this.Controls.Add(this.createNewModelButton);
            this.Controls.Add(this.modelTypeComboBox);
            this.Controls.Add(this.availableModelsListLabel);
            this.Controls.Add(this.modelsFlowLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "AIToolsForm";
            this.Text = "AI tools";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel modelsFlowLayoutPanel;
        private System.Windows.Forms.Label availableModelsListLabel;
        private System.Windows.Forms.Button createNewModelButton;
        private System.Windows.Forms.ComboBox modelTypeComboBox;
        private System.Windows.Forms.Label unfittedDataLabel;
        private System.Windows.Forms.Label updatesLabel;
        private System.Windows.Forms.Label datasetSizeLabel;
        private System.Windows.Forms.Label modelNameLabel;
        private System.Windows.Forms.Button DatasetExplorerButton;
        public System.Windows.Forms.ComboBox aiGoalComboBox;
    }
}
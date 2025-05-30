﻿
namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation
{
    partial class DataVisualisationForm
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.valDetailsTabPage = new System.Windows.Forms.TabPage();
            this.predictionDeviationTitlesPanel = new System.Windows.Forms.Panel();
            this.predictionDeviationFlowLayoutPanel = new BSP_Using_AI.CustomFlowLayoutPanel();
            this.ConfMatSaveImageButton = new System.Windows.Forms.Button();
            this.confusionMatrixPlot = new ScottPlot.FormsPlot();
            this.validationSaveButton = new System.Windows.Forms.Button();
            this.validationTitlesPanel = new System.Windows.Forms.Panel();
            this.metricsComboBox = new System.Windows.Forms.ComboBox();
            this.validationFlowLayoutPanel = new BSP_Using_AI.CustomFlowLayoutPanel();
            this.outpValResultsLabel = new System.Windows.Forms.Label();
            this.rawVisTabPage = new System.Windows.Forms.TabPage();
            this.rawScatterPlotSaveImageButton = new System.Windows.Forms.Button();
            this.xInputFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.yInputFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.outputFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.outputLabel = new System.Windows.Forms.Label();
            this.xInputLabel = new System.Windows.Forms.Label();
            this.yInputLabel = new System.Windows.Forms.Label();
            this.rawChart = new ScottPlot.FormsPlot();
            this.pcaVisTabPage = new System.Windows.Forms.TabPage();
            this.pcaSaveAsImageButton = new System.Windows.Forms.Button();
            this.pcaSaveChangesButton = new System.Windows.Forms.Button();
            this.pcFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.selectedPricipalComponentsLabel = new System.Windows.Forms.Label();
            this.pcaChart = new ScottPlot.FormsPlot();
            this.tsneVisTabPage = new System.Windows.Forms.TabPage();
            this.umapVisTabPage = new System.Windows.Forms.TabPage();
            this.stepLabel = new System.Windows.Forms.Label();
            this.rocThresholdsButton = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.valDetailsTabPage.SuspendLayout();
            this.rawVisTabPage.SuspendLayout();
            this.pcaVisTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.valDetailsTabPage);
            this.tabControl.Controls.Add(this.rawVisTabPage);
            this.tabControl.Controls.Add(this.pcaVisTabPage);
            this.tabControl.Controls.Add(this.tsneVisTabPage);
            this.tabControl.Controls.Add(this.umapVisTabPage);
            this.tabControl.Location = new System.Drawing.Point(15, 45);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1262, 910);
            this.tabControl.TabIndex = 0;
            // 
            // valDetailsTabPage
            // 
            this.valDetailsTabPage.Controls.Add(this.rocThresholdsButton);
            this.valDetailsTabPage.Controls.Add(this.predictionDeviationTitlesPanel);
            this.valDetailsTabPage.Controls.Add(this.predictionDeviationFlowLayoutPanel);
            this.valDetailsTabPage.Controls.Add(this.ConfMatSaveImageButton);
            this.valDetailsTabPage.Controls.Add(this.confusionMatrixPlot);
            this.valDetailsTabPage.Controls.Add(this.validationSaveButton);
            this.valDetailsTabPage.Controls.Add(this.validationTitlesPanel);
            this.valDetailsTabPage.Controls.Add(this.metricsComboBox);
            this.valDetailsTabPage.Controls.Add(this.validationFlowLayoutPanel);
            this.valDetailsTabPage.Controls.Add(this.outpValResultsLabel);
            this.valDetailsTabPage.Location = new System.Drawing.Point(4, 24);
            this.valDetailsTabPage.Name = "valDetailsTabPage";
            this.valDetailsTabPage.Size = new System.Drawing.Size(1254, 882);
            this.valDetailsTabPage.TabIndex = 4;
            this.valDetailsTabPage.Text = "Validation details";
            this.valDetailsTabPage.UseVisualStyleBackColor = true;
            // 
            // predictionDeviationTitlesPanel
            // 
            this.predictionDeviationTitlesPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.predictionDeviationTitlesPanel.Location = new System.Drawing.Point(19, 455);
            this.predictionDeviationTitlesPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.predictionDeviationTitlesPanel.Name = "predictionDeviationTitlesPanel";
            this.predictionDeviationTitlesPanel.Size = new System.Drawing.Size(683, 35);
            this.predictionDeviationTitlesPanel.TabIndex = 54;
            // 
            // predictionDeviationFlowLayoutPanel
            // 
            this.predictionDeviationFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.predictionDeviationFlowLayoutPanel.AutoScroll = true;
            this.predictionDeviationFlowLayoutPanel.Location = new System.Drawing.Point(19, 496);
            this.predictionDeviationFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.predictionDeviationFlowLayoutPanel.Name = "predictionDeviationFlowLayoutPanel";
            this.predictionDeviationFlowLayoutPanel.Size = new System.Drawing.Size(713, 370);
            this.predictionDeviationFlowLayoutPanel.TabIndex = 53;
            // 
            // ConfMatSaveImageButton
            // 
            this.ConfMatSaveImageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfMatSaveImageButton.Location = new System.Drawing.Point(1129, 13);
            this.ConfMatSaveImageButton.Margin = new System.Windows.Forms.Padding(2);
            this.ConfMatSaveImageButton.Name = "ConfMatSaveImageButton";
            this.ConfMatSaveImageButton.Size = new System.Drawing.Size(102, 23);
            this.ConfMatSaveImageButton.TabIndex = 52;
            this.ConfMatSaveImageButton.Text = "Save as image";
            this.ConfMatSaveImageButton.UseVisualStyleBackColor = true;
            this.ConfMatSaveImageButton.Visible = false;
            this.ConfMatSaveImageButton.Click += new System.EventHandler(this.ConfMatSaveImageButton_Click);
            // 
            // confusionMatrixPlot
            // 
            this.confusionMatrixPlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.confusionMatrixPlot.Location = new System.Drawing.Point(731, 29);
            this.confusionMatrixPlot.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.confusionMatrixPlot.Name = "confusionMatrixPlot";
            this.confusionMatrixPlot.Size = new System.Drawing.Size(521, 574);
            this.confusionMatrixPlot.TabIndex = 51;
            this.confusionMatrixPlot.Visible = false;
            // 
            // validationSaveButton
            // 
            this.validationSaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.validationSaveButton.Location = new System.Drawing.Point(740, 839);
            this.validationSaveButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.validationSaveButton.Name = "validationSaveButton";
            this.validationSaveButton.Size = new System.Drawing.Size(111, 27);
            this.validationSaveButton.TabIndex = 50;
            this.validationSaveButton.Text = "Save changes";
            this.validationSaveButton.UseVisualStyleBackColor = true;
            this.validationSaveButton.Click += new System.EventHandler(this.validationSaveButton_Click);
            // 
            // validationTitlesPanel
            // 
            this.validationTitlesPanel.Location = new System.Drawing.Point(19, 38);
            this.validationTitlesPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.validationTitlesPanel.Name = "validationTitlesPanel";
            this.validationTitlesPanel.Size = new System.Drawing.Size(683, 35);
            this.validationTitlesPanel.TabIndex = 49;
            // 
            // metricsComboBox
            // 
            this.metricsComboBox.FormattingEnabled = true;
            this.metricsComboBox.Location = new System.Drawing.Point(601, 13);
            this.metricsComboBox.Name = "metricsComboBox";
            this.metricsComboBox.Size = new System.Drawing.Size(131, 23);
            this.metricsComboBox.TabIndex = 48;
            this.metricsComboBox.SelectedIndexChanged += new System.EventHandler(this.metricsComboBox_SelectedIndexChanged);
            // 
            // validationFlowLayoutPanel
            // 
            this.validationFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.validationFlowLayoutPanel.AutoScroll = true;
            this.validationFlowLayoutPanel.Location = new System.Drawing.Point(19, 79);
            this.validationFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.validationFlowLayoutPanel.Name = "validationFlowLayoutPanel";
            this.validationFlowLayoutPanel.Size = new System.Drawing.Size(713, 370);
            this.validationFlowLayoutPanel.TabIndex = 47;
            // 
            // outpValResultsLabel
            // 
            this.outpValResultsLabel.AutoSize = true;
            this.outpValResultsLabel.Location = new System.Drawing.Point(24, 16);
            this.outpValResultsLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.outpValResultsLabel.Name = "outpValResultsLabel";
            this.outpValResultsLabel.Size = new System.Drawing.Size(142, 15);
            this.outpValResultsLabel.TabIndex = 46;
            this.outpValResultsLabel.Text = "Outputs validation results";
            // 
            // rawVisTabPage
            // 
            this.rawVisTabPage.Controls.Add(this.rawScatterPlotSaveImageButton);
            this.rawVisTabPage.Controls.Add(this.xInputFlowLayoutPanel);
            this.rawVisTabPage.Controls.Add(this.yInputFlowLayoutPanel);
            this.rawVisTabPage.Controls.Add(this.outputFlowLayoutPanel);
            this.rawVisTabPage.Controls.Add(this.outputLabel);
            this.rawVisTabPage.Controls.Add(this.xInputLabel);
            this.rawVisTabPage.Controls.Add(this.yInputLabel);
            this.rawVisTabPage.Controls.Add(this.rawChart);
            this.rawVisTabPage.Location = new System.Drawing.Point(4, 24);
            this.rawVisTabPage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rawVisTabPage.Name = "rawVisTabPage";
            this.rawVisTabPage.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rawVisTabPage.Size = new System.Drawing.Size(1254, 882);
            this.rawVisTabPage.TabIndex = 0;
            this.rawVisTabPage.Text = "Raw visualisation";
            this.rawVisTabPage.UseVisualStyleBackColor = true;
            // 
            // rawScatterPlotSaveImageButton
            // 
            this.rawScatterPlotSaveImageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rawScatterPlotSaveImageButton.Location = new System.Drawing.Point(782, 104);
            this.rawScatterPlotSaveImageButton.Margin = new System.Windows.Forms.Padding(2);
            this.rawScatterPlotSaveImageButton.Name = "rawScatterPlotSaveImageButton";
            this.rawScatterPlotSaveImageButton.Size = new System.Drawing.Size(102, 23);
            this.rawScatterPlotSaveImageButton.TabIndex = 32;
            this.rawScatterPlotSaveImageButton.Text = "Save as image";
            this.rawScatterPlotSaveImageButton.UseVisualStyleBackColor = true;
            this.rawScatterPlotSaveImageButton.Click += new System.EventHandler(this.rawScatterPlotSaveImageButton_Click);
            // 
            // xInputFlowLayoutPanel
            // 
            this.xInputFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.xInputFlowLayoutPanel.AutoScroll = true;
            this.xInputFlowLayoutPanel.Location = new System.Drawing.Point(477, 716);
            this.xInputFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.xInputFlowLayoutPanel.Name = "xInputFlowLayoutPanel";
            this.xInputFlowLayoutPanel.Size = new System.Drawing.Size(243, 132);
            this.xInputFlowLayoutPanel.TabIndex = 9;
            // 
            // yInputFlowLayoutPanel
            // 
            this.yInputFlowLayoutPanel.AutoScroll = true;
            this.yInputFlowLayoutPanel.Location = new System.Drawing.Point(19, 276);
            this.yInputFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.yInputFlowLayoutPanel.Name = "yInputFlowLayoutPanel";
            this.yInputFlowLayoutPanel.Size = new System.Drawing.Size(243, 132);
            this.yInputFlowLayoutPanel.TabIndex = 8;
            // 
            // outputFlowLayoutPanel
            // 
            this.outputFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputFlowLayoutPanel.AutoScroll = true;
            this.outputFlowLayoutPanel.Location = new System.Drawing.Point(1009, 147);
            this.outputFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.outputFlowLayoutPanel.Name = "outputFlowLayoutPanel";
            this.outputFlowLayoutPanel.Size = new System.Drawing.Size(222, 143);
            this.outputFlowLayoutPanel.TabIndex = 7;
            // 
            // outputLabel
            // 
            this.outputLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputLabel.AutoSize = true;
            this.outputLabel.Location = new System.Drawing.Point(1006, 121);
            this.outputLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(43, 15);
            this.outputLabel.TabIndex = 6;
            this.outputLabel.Text = "output";
            // 
            // xInputLabel
            // 
            this.xInputLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.xInputLabel.AutoSize = true;
            this.xInputLabel.Location = new System.Drawing.Point(474, 698);
            this.xInputLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.xInputLabel.Name = "xInputLabel";
            this.xInputLabel.Size = new System.Drawing.Size(45, 15);
            this.xInputLabel.TabIndex = 4;
            this.xInputLabel.Text = "X input";
            // 
            // yInputLabel
            // 
            this.yInputLabel.AutoSize = true;
            this.yInputLabel.Location = new System.Drawing.Point(16, 257);
            this.yInputLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.yInputLabel.Name = "yInputLabel";
            this.yInputLabel.Size = new System.Drawing.Size(45, 15);
            this.yInputLabel.TabIndex = 2;
            this.yInputLabel.Text = "Y input";
            // 
            // rawChart
            // 
            this.rawChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rawChart.Location = new System.Drawing.Point(287, 121);
            this.rawChart.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.rawChart.Name = "rawChart";
            this.rawChart.Size = new System.Drawing.Size(617, 448);
            this.rawChart.TabIndex = 0;
            // 
            // pcaVisTabPage
            // 
            this.pcaVisTabPage.Controls.Add(this.pcaSaveAsImageButton);
            this.pcaVisTabPage.Controls.Add(this.pcaSaveChangesButton);
            this.pcaVisTabPage.Controls.Add(this.pcFlowLayoutPanel);
            this.pcaVisTabPage.Controls.Add(this.selectedPricipalComponentsLabel);
            this.pcaVisTabPage.Controls.Add(this.pcaChart);
            this.pcaVisTabPage.Location = new System.Drawing.Point(4, 24);
            this.pcaVisTabPage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pcaVisTabPage.Name = "pcaVisTabPage";
            this.pcaVisTabPage.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pcaVisTabPage.Size = new System.Drawing.Size(1254, 882);
            this.pcaVisTabPage.TabIndex = 1;
            this.pcaVisTabPage.Text = "PCA visualisation";
            this.pcaVisTabPage.UseVisualStyleBackColor = true;
            // 
            // pcaSaveAsImageButton
            // 
            this.pcaSaveAsImageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pcaSaveAsImageButton.Location = new System.Drawing.Point(812, 147);
            this.pcaSaveAsImageButton.Margin = new System.Windows.Forms.Padding(2);
            this.pcaSaveAsImageButton.Name = "pcaSaveAsImageButton";
            this.pcaSaveAsImageButton.Size = new System.Drawing.Size(102, 23);
            this.pcaSaveAsImageButton.TabIndex = 32;
            this.pcaSaveAsImageButton.Text = "Save as image";
            this.pcaSaveAsImageButton.UseVisualStyleBackColor = true;
            this.pcaSaveAsImageButton.Click += new System.EventHandler(this.saveAsImageButton_Click);
            // 
            // pcaSaveChangesButton
            // 
            this.pcaSaveChangesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pcaSaveChangesButton.Location = new System.Drawing.Point(1107, 329);
            this.pcaSaveChangesButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pcaSaveChangesButton.Name = "pcaSaveChangesButton";
            this.pcaSaveChangesButton.Size = new System.Drawing.Size(111, 27);
            this.pcaSaveChangesButton.TabIndex = 10;
            this.pcaSaveChangesButton.Text = "Save changes";
            this.pcaSaveChangesButton.UseVisualStyleBackColor = true;
            this.pcaSaveChangesButton.Click += new System.EventHandler(this.saveChangesButton_Click);
            // 
            // pcFlowLayoutPanel
            // 
            this.pcFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pcFlowLayoutPanel.AutoScroll = true;
            this.pcFlowLayoutPanel.Location = new System.Drawing.Point(996, 162);
            this.pcFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pcFlowLayoutPanel.Name = "pcFlowLayoutPanel";
            this.pcFlowLayoutPanel.Size = new System.Drawing.Size(222, 143);
            this.pcFlowLayoutPanel.TabIndex = 9;
            // 
            // selectedPricipalComponentsLabel
            // 
            this.selectedPricipalComponentsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectedPricipalComponentsLabel.AutoSize = true;
            this.selectedPricipalComponentsLabel.Location = new System.Drawing.Point(993, 136);
            this.selectedPricipalComponentsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.selectedPricipalComponentsLabel.Name = "selectedPricipalComponentsLabel";
            this.selectedPricipalComponentsLabel.Size = new System.Drawing.Size(170, 15);
            this.selectedPricipalComponentsLabel.TabIndex = 8;
            this.selectedPricipalComponentsLabel.Text = "Selected principal components";
            // 
            // pcaChart
            // 
            this.pcaChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pcaChart.Location = new System.Drawing.Point(318, 162);
            this.pcaChart.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.pcaChart.Name = "pcaChart";
            this.pcaChart.Size = new System.Drawing.Size(617, 448);
            this.pcaChart.TabIndex = 1;
            this.pcaChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pcaChart_MouseDown);
            this.pcaChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pcaChart_MouseMove);
            this.pcaChart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pcaChart_MouseUp);
            // 
            // tsneVisTabPage
            // 
            this.tsneVisTabPage.Location = new System.Drawing.Point(4, 24);
            this.tsneVisTabPage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tsneVisTabPage.Name = "tsneVisTabPage";
            this.tsneVisTabPage.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tsneVisTabPage.Size = new System.Drawing.Size(1254, 882);
            this.tsneVisTabPage.TabIndex = 2;
            this.tsneVisTabPage.Text = "t-SNE visualisation";
            this.tsneVisTabPage.UseVisualStyleBackColor = true;
            // 
            // umapVisTabPage
            // 
            this.umapVisTabPage.Location = new System.Drawing.Point(4, 24);
            this.umapVisTabPage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.umapVisTabPage.Name = "umapVisTabPage";
            this.umapVisTabPage.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.umapVisTabPage.Size = new System.Drawing.Size(1254, 882);
            this.umapVisTabPage.TabIndex = 3;
            this.umapVisTabPage.Text = "UMAP visualisation";
            this.umapVisTabPage.UseVisualStyleBackColor = true;
            // 
            // stepLabel
            // 
            this.stepLabel.AutoSize = true;
            this.stepLabel.Location = new System.Drawing.Point(10, 10);
            this.stepLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.stepLabel.Name = "stepLabel";
            this.stepLabel.Size = new System.Drawing.Size(60, 15);
            this.stepLabel.TabIndex = 1;
            this.stepLabel.Text = "step Label";
            // 
            // rocThresholdsButton
            // 
            this.rocThresholdsButton.Location = new System.Drawing.Point(473, 11);
            this.rocThresholdsButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rocThresholdsButton.Name = "rocThresholdsButton";
            this.rocThresholdsButton.Size = new System.Drawing.Size(111, 27);
            this.rocThresholdsButton.TabIndex = 55;
            this.rocThresholdsButton.Text = "ROC thresholds";
            this.rocThresholdsButton.UseVisualStyleBackColor = true;
            this.rocThresholdsButton.Click += new System.EventHandler(this.rocThresholdsButton_Click);
            // 
            // DataVisualisationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1290, 961);
            this.Controls.Add(this.stepLabel);
            this.Controls.Add(this.tabControl);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "DataVisualisationForm";
            this.Text = "DataVisualisationForm";
            this.tabControl.ResumeLayout(false);
            this.valDetailsTabPage.ResumeLayout(false);
            this.valDetailsTabPage.PerformLayout();
            this.rawVisTabPage.ResumeLayout(false);
            this.rawVisTabPage.PerformLayout();
            this.pcaVisTabPage.ResumeLayout(false);
            this.pcaVisTabPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage rawVisTabPage;
        private ScottPlot.FormsPlot rawChart;
        private System.Windows.Forms.TabPage pcaVisTabPage;
        private System.Windows.Forms.TabPage tsneVisTabPage;
        private System.Windows.Forms.TabPage umapVisTabPage;
        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.Label xInputLabel;
        private System.Windows.Forms.Label yInputLabel;
        public System.Windows.Forms.Label stepLabel;
        private System.Windows.Forms.FlowLayoutPanel outputFlowLayoutPanel;
        private ScottPlot.FormsPlot pcaChart;
        private System.Windows.Forms.FlowLayoutPanel pcFlowLayoutPanel;
        private System.Windows.Forms.Label selectedPricipalComponentsLabel;
        private System.Windows.Forms.Button pcaSaveChangesButton;
        private System.Windows.Forms.FlowLayoutPanel xInputFlowLayoutPanel;
        private System.Windows.Forms.FlowLayoutPanel yInputFlowLayoutPanel;
        public System.Windows.Forms.Button rawScatterPlotSaveImageButton;
        public System.Windows.Forms.Button pcaSaveAsImageButton;
        private System.Windows.Forms.TabPage valDetailsTabPage;
        private System.Windows.Forms.Panel validationTitlesPanel;
        private System.Windows.Forms.ComboBox metricsComboBox;
        private CustomFlowLayoutPanel validationFlowLayoutPanel;
        private System.Windows.Forms.Label outpValResultsLabel;
        private System.Windows.Forms.Button validationSaveButton;
        private ScottPlot.FormsPlot confusionMatrixPlot;
        public System.Windows.Forms.Button ConfMatSaveImageButton;
        private System.Windows.Forms.Panel predictionDeviationTitlesPanel;
        private CustomFlowLayoutPanel predictionDeviationFlowLayoutPanel;
        private System.Windows.Forms.Button rocThresholdsButton;
    }
}
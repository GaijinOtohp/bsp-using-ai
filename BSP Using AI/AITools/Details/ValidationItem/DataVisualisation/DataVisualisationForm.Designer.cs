
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.rawVisTabPage = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.xInputFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.yInputFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.outputFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.outputLabel = new System.Windows.Forms.Label();
            this.xInputLabel = new System.Windows.Forms.Label();
            this.yInputLabel = new System.Windows.Forms.Label();
            this.rawChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.pcaVisTabPage = new System.Windows.Forms.TabPage();
            this.saveAsImageButton = new System.Windows.Forms.Button();
            this.saveChangesButton = new System.Windows.Forms.Button();
            this.pcFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.selectedPricipalComponentsLabel = new System.Windows.Forms.Label();
            this.pcaChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tsneVisTabPage = new System.Windows.Forms.TabPage();
            this.umapVisTabPage = new System.Windows.Forms.TabPage();
            this.stepLabel = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.rawVisTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rawChart)).BeginInit();
            this.pcaVisTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcaChart)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.rawVisTabPage);
            this.tabControl.Controls.Add(this.pcaVisTabPage);
            this.tabControl.Controls.Add(this.tsneVisTabPage);
            this.tabControl.Controls.Add(this.umapVisTabPage);
            this.tabControl.Location = new System.Drawing.Point(12, 135);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1082, 695);
            this.tabControl.TabIndex = 0;
            // 
            // rawVisTabPage
            // 
            this.rawVisTabPage.Controls.Add(this.button1);
            this.rawVisTabPage.Controls.Add(this.xInputFlowLayoutPanel);
            this.rawVisTabPage.Controls.Add(this.yInputFlowLayoutPanel);
            this.rawVisTabPage.Controls.Add(this.outputFlowLayoutPanel);
            this.rawVisTabPage.Controls.Add(this.outputLabel);
            this.rawVisTabPage.Controls.Add(this.xInputLabel);
            this.rawVisTabPage.Controls.Add(this.yInputLabel);
            this.rawVisTabPage.Controls.Add(this.rawChart);
            this.rawVisTabPage.Location = new System.Drawing.Point(4, 22);
            this.rawVisTabPage.Name = "rawVisTabPage";
            this.rawVisTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.rawVisTabPage.Size = new System.Drawing.Size(1074, 669);
            this.rawVisTabPage.TabIndex = 0;
            this.rawVisTabPage.Text = "Raw visualisation tab";
            this.rawVisTabPage.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(688, 80);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 20);
            this.button1.TabIndex = 32;
            this.button1.Text = "Save as image";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // xInputFlowLayoutPanel
            // 
            this.xInputFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.xInputFlowLayoutPanel.AutoScroll = true;
            this.xInputFlowLayoutPanel.Location = new System.Drawing.Point(409, 527);
            this.xInputFlowLayoutPanel.Name = "xInputFlowLayoutPanel";
            this.xInputFlowLayoutPanel.Size = new System.Drawing.Size(208, 114);
            this.xInputFlowLayoutPanel.TabIndex = 9;
            // 
            // yInputFlowLayoutPanel
            // 
            this.yInputFlowLayoutPanel.AutoScroll = true;
            this.yInputFlowLayoutPanel.Location = new System.Drawing.Point(16, 239);
            this.yInputFlowLayoutPanel.Name = "yInputFlowLayoutPanel";
            this.yInputFlowLayoutPanel.Size = new System.Drawing.Size(208, 114);
            this.yInputFlowLayoutPanel.TabIndex = 8;
            // 
            // outputFlowLayoutPanel
            // 
            this.outputFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputFlowLayoutPanel.AutoScroll = true;
            this.outputFlowLayoutPanel.Location = new System.Drawing.Point(865, 127);
            this.outputFlowLayoutPanel.Name = "outputFlowLayoutPanel";
            this.outputFlowLayoutPanel.Size = new System.Drawing.Size(190, 124);
            this.outputFlowLayoutPanel.TabIndex = 7;
            // 
            // outputLabel
            // 
            this.outputLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputLabel.AutoSize = true;
            this.outputLabel.Location = new System.Drawing.Point(862, 105);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(37, 13);
            this.outputLabel.TabIndex = 6;
            this.outputLabel.Text = "output";
            // 
            // xInputLabel
            // 
            this.xInputLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.xInputLabel.AutoSize = true;
            this.xInputLabel.Location = new System.Drawing.Point(406, 511);
            this.xInputLabel.Name = "xInputLabel";
            this.xInputLabel.Size = new System.Drawing.Size(40, 13);
            this.xInputLabel.TabIndex = 4;
            this.xInputLabel.Text = "X input";
            // 
            // yInputLabel
            // 
            this.yInputLabel.AutoSize = true;
            this.yInputLabel.Location = new System.Drawing.Point(14, 223);
            this.yInputLabel.Name = "yInputLabel";
            this.yInputLabel.Size = new System.Drawing.Size(40, 13);
            this.yInputLabel.TabIndex = 2;
            this.yInputLabel.Text = "Y input";
            // 
            // rawChart
            // 
            this.rawChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rawChart.BorderlineColor = System.Drawing.Color.Black;
            this.rawChart.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisY.MajorGrid.Enabled = false;
            chartArea1.Name = "ChartArea1";
            this.rawChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.rawChart.Legends.Add(legend1);
            this.rawChart.Location = new System.Drawing.Point(246, 105);
            this.rawChart.Name = "rawChart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series1.Legend = "Legend1";
            series1.MarkerBorderColor = System.Drawing.Color.Blue;
            series1.MarkerBorderWidth = 2;
            series1.MarkerColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            series1.MarkerSize = 10;
            series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series1.Name = "Series1";
            this.rawChart.Series.Add(series1);
            this.rawChart.Size = new System.Drawing.Size(529, 388);
            this.rawChart.TabIndex = 0;
            this.rawChart.Text = "rawChart";
            title1.Name = "RawScatterplot";
            title1.Text = "Raw data scatterplot";
            this.rawChart.Titles.Add(title1);
            this.rawChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rawChart_MouseDown);
            this.rawChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.rawChart_MouseMove);
            this.rawChart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rawChart_MouseUp);
            this.rawChart.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseWheel);
            // 
            // pcaVisTabPage
            // 
            this.pcaVisTabPage.Controls.Add(this.saveAsImageButton);
            this.pcaVisTabPage.Controls.Add(this.saveChangesButton);
            this.pcaVisTabPage.Controls.Add(this.pcFlowLayoutPanel);
            this.pcaVisTabPage.Controls.Add(this.selectedPricipalComponentsLabel);
            this.pcaVisTabPage.Controls.Add(this.pcaChart);
            this.pcaVisTabPage.Location = new System.Drawing.Point(4, 22);
            this.pcaVisTabPage.Name = "pcaVisTabPage";
            this.pcaVisTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.pcaVisTabPage.Size = new System.Drawing.Size(1074, 669);
            this.pcaVisTabPage.TabIndex = 1;
            this.pcaVisTabPage.Text = "PCA visualisation tab";
            this.pcaVisTabPage.UseVisualStyleBackColor = true;
            // 
            // saveAsImageButton
            // 
            this.saveAsImageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveAsImageButton.Location = new System.Drawing.Point(715, 115);
            this.saveAsImageButton.Margin = new System.Windows.Forms.Padding(2);
            this.saveAsImageButton.Name = "saveAsImageButton";
            this.saveAsImageButton.Size = new System.Drawing.Size(87, 20);
            this.saveAsImageButton.TabIndex = 32;
            this.saveAsImageButton.Text = "Save as image";
            this.saveAsImageButton.UseVisualStyleBackColor = true;
            this.saveAsImageButton.Click += new System.EventHandler(this.saveAsImageButton_Click);
            // 
            // saveChangesButton
            // 
            this.saveChangesButton.Location = new System.Drawing.Point(949, 285);
            this.saveChangesButton.Name = "saveChangesButton";
            this.saveChangesButton.Size = new System.Drawing.Size(95, 23);
            this.saveChangesButton.TabIndex = 10;
            this.saveChangesButton.Text = "Save changes";
            this.saveChangesButton.UseVisualStyleBackColor = true;
            this.saveChangesButton.Click += new System.EventHandler(this.saveChangesButton_Click);
            // 
            // pcFlowLayoutPanel
            // 
            this.pcFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pcFlowLayoutPanel.AutoScroll = true;
            this.pcFlowLayoutPanel.Location = new System.Drawing.Point(854, 140);
            this.pcFlowLayoutPanel.Name = "pcFlowLayoutPanel";
            this.pcFlowLayoutPanel.Size = new System.Drawing.Size(190, 124);
            this.pcFlowLayoutPanel.TabIndex = 9;
            // 
            // selectedPricipalComponentsLabel
            // 
            this.selectedPricipalComponentsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectedPricipalComponentsLabel.AutoSize = true;
            this.selectedPricipalComponentsLabel.Location = new System.Drawing.Point(851, 118);
            this.selectedPricipalComponentsLabel.Name = "selectedPricipalComponentsLabel";
            this.selectedPricipalComponentsLabel.Size = new System.Drawing.Size(152, 13);
            this.selectedPricipalComponentsLabel.TabIndex = 8;
            this.selectedPricipalComponentsLabel.Text = "Selected principal components";
            // 
            // pcaChart
            // 
            this.pcaChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pcaChart.BorderlineColor = System.Drawing.Color.Black;
            this.pcaChart.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea2.AxisX.Interval = 1D;
            chartArea2.AxisX.LabelStyle.IsStaggered = true;
            chartArea2.AxisX.MajorGrid.Enabled = false;
            chartArea2.AxisY.MajorGrid.Enabled = false;
            chartArea2.Name = "ChartArea1";
            this.pcaChart.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.pcaChart.Legends.Add(legend2);
            this.pcaChart.Location = new System.Drawing.Point(273, 140);
            this.pcaChart.Name = "pcaChart";
            series2.ChartArea = "ChartArea1";
            series2.IsValueShownAsLabel = true;
            series2.IsVisibleInLegend = false;
            series2.Legend = "Legend1";
            series2.MarkerBorderColor = System.Drawing.Color.Blue;
            series2.MarkerBorderWidth = 2;
            series2.MarkerColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            series2.MarkerSize = 10;
            series2.Name = "Principal Components";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            this.pcaChart.Series.Add(series2);
            this.pcaChart.Size = new System.Drawing.Size(529, 388);
            this.pcaChart.TabIndex = 1;
            this.pcaChart.Text = "pcaChart";
            title2.Name = "PCA";
            title2.Text = "PCA";
            this.pcaChart.Titles.Add(title2);
            this.pcaChart.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pcaChart_MouseClick);
            this.pcaChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rawChart_MouseDown);
            this.pcaChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pcaChart_MouseMove);
            this.pcaChart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rawChart_MouseUp);
            this.pcaChart.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseWheel);
            // 
            // tsneVisTabPage
            // 
            this.tsneVisTabPage.Location = new System.Drawing.Point(4, 22);
            this.tsneVisTabPage.Name = "tsneVisTabPage";
            this.tsneVisTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.tsneVisTabPage.Size = new System.Drawing.Size(1074, 669);
            this.tsneVisTabPage.TabIndex = 2;
            this.tsneVisTabPage.Text = "t-SNE visualisation tab";
            this.tsneVisTabPage.UseVisualStyleBackColor = true;
            // 
            // umapVisTabPage
            // 
            this.umapVisTabPage.Location = new System.Drawing.Point(4, 22);
            this.umapVisTabPage.Name = "umapVisTabPage";
            this.umapVisTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.umapVisTabPage.Size = new System.Drawing.Size(1074, 669);
            this.umapVisTabPage.TabIndex = 3;
            this.umapVisTabPage.Text = "UMAP visualisation tab";
            this.umapVisTabPage.UseVisualStyleBackColor = true;
            // 
            // stepLabel
            // 
            this.stepLabel.AutoSize = true;
            this.stepLabel.Location = new System.Drawing.Point(9, 9);
            this.stepLabel.Name = "stepLabel";
            this.stepLabel.Size = new System.Drawing.Size(56, 13);
            this.stepLabel.TabIndex = 1;
            this.stepLabel.Text = "step Label";
            // 
            // DataVisualisationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1106, 833);
            this.Controls.Add(this.stepLabel);
            this.Controls.Add(this.tabControl);
            this.Name = "DataVisualisationForm";
            this.Text = "DataVisualisationForm";
            this.tabControl.ResumeLayout(false);
            this.rawVisTabPage.ResumeLayout(false);
            this.rawVisTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rawChart)).EndInit();
            this.pcaVisTabPage.ResumeLayout(false);
            this.pcaVisTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcaChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage rawVisTabPage;
        private System.Windows.Forms.DataVisualization.Charting.Chart rawChart;
        private System.Windows.Forms.TabPage pcaVisTabPage;
        private System.Windows.Forms.TabPage tsneVisTabPage;
        private System.Windows.Forms.TabPage umapVisTabPage;
        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.Label xInputLabel;
        private System.Windows.Forms.Label yInputLabel;
        public System.Windows.Forms.Label stepLabel;
        private System.Windows.Forms.FlowLayoutPanel outputFlowLayoutPanel;
        private System.Windows.Forms.DataVisualization.Charting.Chart pcaChart;
        private System.Windows.Forms.FlowLayoutPanel pcFlowLayoutPanel;
        private System.Windows.Forms.Label selectedPricipalComponentsLabel;
        private System.Windows.Forms.Button saveChangesButton;
        private System.Windows.Forms.FlowLayoutPanel xInputFlowLayoutPanel;
        private System.Windows.Forms.FlowLayoutPanel yInputFlowLayoutPanel;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button saveAsImageButton;
    }
}
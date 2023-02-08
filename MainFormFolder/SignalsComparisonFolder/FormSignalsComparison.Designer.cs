
namespace BSP_Using_AI.MainFormFolder.SignalsComparisonFolder
{
    partial class FormSignalsComparison
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.firstSignalChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.secondSignalChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.comparisonChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sendSignalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analyseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectFirstSignalCheckBox = new System.Windows.Forms.CheckBox();
            this.selectSecondSignalCheckBox = new System.Windows.Forms.CheckBox();
            this.crosscorrelationRadioButton = new System.Windows.Forms.RadioButton();
            this.minimumSubtractionRadioButton = new System.Windows.Forms.RadioButton();
            this.dynamicTimeWrapingRadioButton = new System.Windows.Forms.RadioButton();
            this.distanceValueChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.pathChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.secondSignalPowerLabel = new System.Windows.Forms.Label();
            this.secondSignalPowerValueLabel = new System.Windows.Forms.Label();
            this.firstSignalPowerLabel = new System.Windows.Forms.Label();
            this.firstSignalPowerValueLabel = new System.Windows.Forms.Label();
            this.comparisonSignalPowerLabel = new System.Windows.Forms.Label();
            this.comparisonSignalPowerValueLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.firstSignalChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.secondSignalChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.comparisonChart)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.distanceValueChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pathChart)).BeginInit();
            this.SuspendLayout();
            // 
            // firstSignalChart
            // 
            chartArea1.AxisX.LabelStyle.Format = "0.00";
            chartArea1.AxisY.LabelStyle.Format = "0.00";
            chartArea1.Name = "ChartArea1";
            this.firstSignalChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.firstSignalChart.Legends.Add(legend1);
            this.firstSignalChart.Location = new System.Drawing.Point(9, 32);
            this.firstSignalChart.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.firstSignalChart.Name = "firstSignalChart";
            this.firstSignalChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SemiTransparent;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.Red;
            series1.Legend = "Legend1";
            series1.Name = "1st signal";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            this.firstSignalChart.Series.Add(series1);
            this.firstSignalChart.Size = new System.Drawing.Size(381, 168);
            this.firstSignalChart.TabIndex = 3;
            this.firstSignalChart.Text = "Signal periods Chart";
            this.firstSignalChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseDown);
            this.firstSignalChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseMove);
            this.firstSignalChart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseUp);
            this.firstSignalChart.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseWheel);
            // 
            // secondSignalChart
            // 
            chartArea2.AxisX.LabelStyle.Format = "0.00";
            chartArea2.AxisY.LabelStyle.Format = "0.00";
            chartArea2.Name = "ChartArea1";
            this.secondSignalChart.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.secondSignalChart.Legends.Add(legend2);
            this.secondSignalChart.Location = new System.Drawing.Point(596, 32);
            this.secondSignalChart.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.secondSignalChart.Name = "secondSignalChart";
            this.secondSignalChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SemiTransparent;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.Red;
            series2.Legend = "Legend1";
            series2.Name = "2nd signal";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series2.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            this.secondSignalChart.Series.Add(series2);
            this.secondSignalChart.Size = new System.Drawing.Size(381, 168);
            this.secondSignalChart.TabIndex = 4;
            this.secondSignalChart.Text = "Signal periods Chart";
            this.secondSignalChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseDown);
            this.secondSignalChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseMove);
            this.secondSignalChart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseUp);
            this.secondSignalChart.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseWheel);
            // 
            // comparisonChart
            // 
            chartArea3.AxisX.LabelStyle.Format = "0.00";
            chartArea3.AxisY.LabelStyle.Format = "0.00";
            chartArea3.Name = "ChartArea1";
            this.comparisonChart.ChartAreas.Add(chartArea3);
            this.comparisonChart.ContextMenuStrip = this.contextMenuStrip1;
            legend3.Name = "Legend1";
            this.comparisonChart.Legends.Add(legend3);
            this.comparisonChart.Location = new System.Drawing.Point(304, 254);
            this.comparisonChart.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comparisonChart.Name = "comparisonChart";
            this.comparisonChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SemiTransparent;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Color = System.Drawing.Color.Blue;
            series3.Legend = "Legend1";
            series3.Name = "Comparison";
            series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series3.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            this.comparisonChart.Series.Add(series3);
            this.comparisonChart.Size = new System.Drawing.Size(381, 168);
            this.comparisonChart.TabIndex = 5;
            this.comparisonChart.Text = "Fusion Exhibitor";
            this.comparisonChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseDown);
            this.comparisonChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseMove);
            this.comparisonChart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseUp);
            this.comparisonChart.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseWheel);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendSignalToolStripMenuItem,
            this.analyseToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(135, 48);
            // 
            // sendSignalToolStripMenuItem
            // 
            this.sendSignalToolStripMenuItem.Name = "sendSignalToolStripMenuItem";
            this.sendSignalToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.sendSignalToolStripMenuItem.Text = "Send signal";
            this.sendSignalToolStripMenuItem.Click += new System.EventHandler(this.sendSignalToolStripMenuItem_Click);
            // 
            // analyseToolStripMenuItem
            // 
            this.analyseToolStripMenuItem.Name = "analyseToolStripMenuItem";
            this.analyseToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.analyseToolStripMenuItem.Text = "Analyse";
            this.analyseToolStripMenuItem.Click += new System.EventHandler(this.analyseToolStripMenuItem_Click);
            // 
            // selectFirstSignalCheckBox
            // 
            this.selectFirstSignalCheckBox.AutoSize = true;
            this.selectFirstSignalCheckBox.Location = new System.Drawing.Point(9, 10);
            this.selectFirstSignalCheckBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.selectFirstSignalCheckBox.Name = "selectFirstSignalCheckBox";
            this.selectFirstSignalCheckBox.Size = new System.Drawing.Size(105, 17);
            this.selectFirstSignalCheckBox.TabIndex = 23;
            this.selectFirstSignalCheckBox.Text = "Select first signal";
            this.selectFirstSignalCheckBox.UseVisualStyleBackColor = true;
            // 
            // selectSecondSignalCheckBox
            // 
            this.selectSecondSignalCheckBox.AutoSize = true;
            this.selectSecondSignalCheckBox.Location = new System.Drawing.Point(596, 10);
            this.selectSecondSignalCheckBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.selectSecondSignalCheckBox.Name = "selectSecondSignalCheckBox";
            this.selectSecondSignalCheckBox.Size = new System.Drawing.Size(124, 17);
            this.selectSecondSignalCheckBox.TabIndex = 24;
            this.selectSecondSignalCheckBox.Text = "Select second signal";
            this.selectSecondSignalCheckBox.UseVisualStyleBackColor = true;
            // 
            // crosscorrelationRadioButton
            // 
            this.crosscorrelationRadioButton.Checked = true;
            this.crosscorrelationRadioButton.Location = new System.Drawing.Point(424, 76);
            this.crosscorrelationRadioButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.crosscorrelationRadioButton.Name = "crosscorrelationRadioButton";
            this.crosscorrelationRadioButton.Size = new System.Drawing.Size(136, 17);
            this.crosscorrelationRadioButton.TabIndex = 25;
            this.crosscorrelationRadioButton.TabStop = true;
            this.crosscorrelationRadioButton.Text = "Cross-correlation";
            this.crosscorrelationRadioButton.UseVisualStyleBackColor = true;
            this.crosscorrelationRadioButton.CheckedChanged += new System.EventHandler(this.crosscorrelationButton_CheckedChanged);
            // 
            // minimumSubtractionRadioButton
            // 
            this.minimumSubtractionRadioButton.Location = new System.Drawing.Point(424, 98);
            this.minimumSubtractionRadioButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.minimumSubtractionRadioButton.Name = "minimumSubtractionRadioButton";
            this.minimumSubtractionRadioButton.Size = new System.Drawing.Size(136, 17);
            this.minimumSubtractionRadioButton.TabIndex = 26;
            this.minimumSubtractionRadioButton.Text = "Minimum subtraction";
            this.minimumSubtractionRadioButton.UseVisualStyleBackColor = true;
            this.minimumSubtractionRadioButton.CheckedChanged += new System.EventHandler(this.minimumSubtractionRadioButton_CheckedChanged);
            // 
            // dynamicTimeWrapingRadioButton
            // 
            this.dynamicTimeWrapingRadioButton.Location = new System.Drawing.Point(424, 119);
            this.dynamicTimeWrapingRadioButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dynamicTimeWrapingRadioButton.Name = "dynamicTimeWrapingRadioButton";
            this.dynamicTimeWrapingRadioButton.Size = new System.Drawing.Size(136, 17);
            this.dynamicTimeWrapingRadioButton.TabIndex = 27;
            this.dynamicTimeWrapingRadioButton.Text = "Dynamic time wraping";
            this.dynamicTimeWrapingRadioButton.UseVisualStyleBackColor = true;
            this.dynamicTimeWrapingRadioButton.CheckedChanged += new System.EventHandler(this.dynamicTimeWrapingRadioButton_CheckedChanged);
            // 
            // distanceValueChart
            // 
            chartArea4.AxisX.LabelStyle.Format = "0.00";
            chartArea4.AxisY.LabelStyle.Format = "0.00";
            chartArea4.Name = "ChartArea1";
            this.distanceValueChart.ChartAreas.Add(chartArea4);
            legend4.Name = "Legend1";
            this.distanceValueChart.Legends.Add(legend4);
            this.distanceValueChart.Location = new System.Drawing.Point(9, 449);
            this.distanceValueChart.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.distanceValueChart.Name = "distanceValueChart";
            this.distanceValueChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SemiTransparent;
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Color = System.Drawing.Color.Blue;
            series4.Legend = "Legend1";
            series4.Name = "Distance value";
            series4.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series4.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            this.distanceValueChart.Series.Add(series4);
            this.distanceValueChart.Size = new System.Drawing.Size(381, 168);
            this.distanceValueChart.TabIndex = 28;
            this.distanceValueChart.Text = "Fusion Exhibitor";
            this.distanceValueChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseDown);
            this.distanceValueChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseMove);
            this.distanceValueChart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseUp);
            this.distanceValueChart.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseWheel);
            // 
            // pathChart
            // 
            chartArea5.AxisX.LabelStyle.Format = "0.00";
            chartArea5.AxisY.LabelStyle.Format = "0.00";
            chartArea5.Name = "ChartArea1";
            this.pathChart.ChartAreas.Add(chartArea5);
            legend5.Name = "Legend1";
            this.pathChart.Legends.Add(legend5);
            this.pathChart.Location = new System.Drawing.Point(596, 449);
            this.pathChart.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pathChart.Name = "pathChart";
            this.pathChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SemiTransparent;
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series5.Color = System.Drawing.Color.Blue;
            series5.Legend = "Legend1";
            series5.Name = "Path";
            series5.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series5.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            this.pathChart.Series.Add(series5);
            this.pathChart.Size = new System.Drawing.Size(381, 168);
            this.pathChart.TabIndex = 29;
            this.pathChart.Text = "Fusion Exhibitor";
            this.pathChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseDown);
            this.pathChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseMove);
            this.pathChart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseUp);
            this.pathChart.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseWheel);
            // 
            // secondSignalPowerLabel
            // 
            this.secondSignalPowerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.secondSignalPowerLabel.AutoSize = true;
            this.secondSignalPowerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.secondSignalPowerLabel.Location = new System.Drawing.Point(898, 205);
            this.secondSignalPowerLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.secondSignalPowerLabel.Name = "secondSignalPowerLabel";
            this.secondSignalPowerLabel.Size = new System.Drawing.Size(84, 13);
            this.secondSignalPowerLabel.TabIndex = 31;
            this.secondSignalPowerLabel.Text = "Signal power:";
            // 
            // secondSignalPowerValueLabel
            // 
            this.secondSignalPowerValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.secondSignalPowerValueLabel.AutoSize = true;
            this.secondSignalPowerValueLabel.Location = new System.Drawing.Point(898, 223);
            this.secondSignalPowerValueLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.secondSignalPowerValueLabel.Name = "secondSignalPowerValueLabel";
            this.secondSignalPowerValueLabel.Size = new System.Drawing.Size(13, 13);
            this.secondSignalPowerValueLabel.TabIndex = 30;
            this.secondSignalPowerValueLabel.Text = "0";
            // 
            // firstSignalPowerLabel
            // 
            this.firstSignalPowerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.firstSignalPowerLabel.AutoSize = true;
            this.firstSignalPowerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.firstSignalPowerLabel.Location = new System.Drawing.Point(7, 205);
            this.firstSignalPowerLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.firstSignalPowerLabel.Name = "firstSignalPowerLabel";
            this.firstSignalPowerLabel.Size = new System.Drawing.Size(84, 13);
            this.firstSignalPowerLabel.TabIndex = 33;
            this.firstSignalPowerLabel.Text = "Signal power:";
            // 
            // firstSignalPowerValueLabel
            // 
            this.firstSignalPowerValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.firstSignalPowerValueLabel.AutoSize = true;
            this.firstSignalPowerValueLabel.Location = new System.Drawing.Point(7, 223);
            this.firstSignalPowerValueLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.firstSignalPowerValueLabel.Name = "firstSignalPowerValueLabel";
            this.firstSignalPowerValueLabel.Size = new System.Drawing.Size(13, 13);
            this.firstSignalPowerValueLabel.TabIndex = 32;
            this.firstSignalPowerValueLabel.Text = "0";
            // 
            // comparisonSignalPowerLabel
            // 
            this.comparisonSignalPowerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comparisonSignalPowerLabel.AutoSize = true;
            this.comparisonSignalPowerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comparisonSignalPowerLabel.Location = new System.Drawing.Point(689, 389);
            this.comparisonSignalPowerLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comparisonSignalPowerLabel.Name = "comparisonSignalPowerLabel";
            this.comparisonSignalPowerLabel.Size = new System.Drawing.Size(84, 13);
            this.comparisonSignalPowerLabel.TabIndex = 35;
            this.comparisonSignalPowerLabel.Text = "Signal power:";
            // 
            // comparisonSignalPowerValueLabel
            // 
            this.comparisonSignalPowerValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comparisonSignalPowerValueLabel.AutoSize = true;
            this.comparisonSignalPowerValueLabel.Location = new System.Drawing.Point(689, 408);
            this.comparisonSignalPowerValueLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comparisonSignalPowerValueLabel.Name = "comparisonSignalPowerValueLabel";
            this.comparisonSignalPowerValueLabel.Size = new System.Drawing.Size(13, 13);
            this.comparisonSignalPowerValueLabel.TabIndex = 34;
            this.comparisonSignalPowerValueLabel.Text = "0";
            // 
            // FormSignalsComparison
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(988, 443);
            this.Controls.Add(this.comparisonSignalPowerLabel);
            this.Controls.Add(this.comparisonSignalPowerValueLabel);
            this.Controls.Add(this.firstSignalPowerLabel);
            this.Controls.Add(this.firstSignalPowerValueLabel);
            this.Controls.Add(this.secondSignalPowerLabel);
            this.Controls.Add(this.secondSignalPowerValueLabel);
            this.Controls.Add(this.pathChart);
            this.Controls.Add(this.distanceValueChart);
            this.Controls.Add(this.dynamicTimeWrapingRadioButton);
            this.Controls.Add(this.minimumSubtractionRadioButton);
            this.Controls.Add(this.crosscorrelationRadioButton);
            this.Controls.Add(this.selectSecondSignalCheckBox);
            this.Controls.Add(this.selectFirstSignalCheckBox);
            this.Controls.Add(this.comparisonChart);
            this.Controls.Add(this.secondSignalChart);
            this.Controls.Add(this.firstSignalChart);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximumSize = new System.Drawing.Size(1004, 482);
            this.MinimumSize = new System.Drawing.Size(1004, 482);
            this.Name = "FormSignalsComparison";
            this.Text = "Signals Compariator";
            ((System.ComponentModel.ISupportInitialize)(this.firstSignalChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.secondSignalChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.comparisonChart)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.distanceValueChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pathChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart firstSignalChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart secondSignalChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart comparisonChart;
        public System.Windows.Forms.CheckBox selectFirstSignalCheckBox;
        public System.Windows.Forms.CheckBox selectSecondSignalCheckBox;
        private System.Windows.Forms.RadioButton crosscorrelationRadioButton;
        private System.Windows.Forms.RadioButton minimumSubtractionRadioButton;
        private System.Windows.Forms.RadioButton dynamicTimeWrapingRadioButton;
        private System.Windows.Forms.DataVisualization.Charting.Chart distanceValueChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart pathChart;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem sendSignalToolStripMenuItem;
        private System.Windows.Forms.Label secondSignalPowerLabel;
        private System.Windows.Forms.Label secondSignalPowerValueLabel;
        private System.Windows.Forms.Label firstSignalPowerLabel;
        private System.Windows.Forms.Label firstSignalPowerValueLabel;
        private System.Windows.Forms.Label comparisonSignalPowerLabel;
        private System.Windows.Forms.Label comparisonSignalPowerValueLabel;
        private System.Windows.Forms.ToolStripMenuItem analyseToolStripMenuItem;
    }
}
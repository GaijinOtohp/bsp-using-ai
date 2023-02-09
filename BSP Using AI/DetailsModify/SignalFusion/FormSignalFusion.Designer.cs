
namespace BSP_Using_AI.DetailsModify.SignalFusion
{
    partial class FormSignalFusion
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.periodsChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sendSignalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analyseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fusionChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.periodPickerComboBox = new System.Windows.Forms.ComboBox();
            this.AdditionRadioButton = new System.Windows.Forms.RadioButton();
            this.multiplicationRadioButton = new System.Windows.Forms.RadioButton();
            this.crossCorrelationRadioButton = new System.Windows.Forms.RadioButton();
            this.OrthogonalisationRadioButton = new System.Windows.Forms.RadioButton();
            this.orthogonalSignalsComboBox = new System.Windows.Forms.ComboBox();
            this.offsetScrollBar = new System.Windows.Forms.HScrollBar();
            this.offsetLabel = new System.Windows.Forms.Label();
            this.synchronizePeriodsCheckBox = new System.Windows.Forms.CheckBox();
            this.centralizeCheckBox = new System.Windows.Forms.CheckBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.periodDurationTextBox = new System.Windows.Forms.TextBox();
            this.periodDurationLabel = new System.Windows.Forms.Label();
            this.fuseOrthogonalizationButton = new System.Windows.Forms.Button();
            this.pathLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.periodsChart)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fusionChart)).BeginInit();
            this.SuspendLayout();
            // 
            // periodsChart
            // 
            this.periodsChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea2.AxisX.LabelStyle.Format = "0.00";
            chartArea2.AxisY.LabelStyle.Format = "0.00";
            chartArea2.Name = "ChartArea1";
            this.periodsChart.ChartAreas.Add(chartArea2);
            this.periodsChart.ContextMenuStrip = this.contextMenuStrip1;
            legend2.Name = "Legend1";
            this.periodsChart.Legends.Add(legend2);
            this.periodsChart.Location = new System.Drawing.Point(9, 41);
            this.periodsChart.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.periodsChart.Name = "periodsChart";
            this.periodsChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SemiTransparent;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.Red;
            series2.Legend = "Legend1";
            series2.Name = "Signal";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series2.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            this.periodsChart.Series.Add(series2);
            this.periodsChart.Size = new System.Drawing.Size(381, 168);
            this.periodsChart.TabIndex = 2;
            this.periodsChart.Text = "Signal periods Chart";
            this.periodsChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseDown);
            this.periodsChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseMove);
            this.periodsChart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseUp);
            this.periodsChart.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseWheel);
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
            // fusionChart
            // 
            this.fusionChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea3.AxisX.LabelStyle.Format = "0.00";
            chartArea3.AxisY.LabelStyle.Format = "0.00";
            chartArea3.Name = "ChartArea1";
            this.fusionChart.ChartAreas.Add(chartArea3);
            this.fusionChart.ContextMenuStrip = this.contextMenuStrip1;
            legend3.Name = "Legend1";
            this.fusionChart.Legends.Add(legend3);
            this.fusionChart.Location = new System.Drawing.Point(9, 235);
            this.fusionChart.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.fusionChart.Name = "fusionChart";
            this.fusionChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SemiTransparent;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Color = System.Drawing.Color.Blue;
            series3.Legend = "Legend1";
            series3.Name = "Fusion";
            series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series3.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            this.fusionChart.Series.Add(series3);
            this.fusionChart.Size = new System.Drawing.Size(381, 168);
            this.fusionChart.TabIndex = 3;
            this.fusionChart.Text = "Fusion Exhibitor";
            this.fusionChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseDown);
            this.fusionChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseMove);
            this.fusionChart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseUp);
            this.fusionChart.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseWheel);
            // 
            // periodPickerComboBox
            // 
            this.periodPickerComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.periodPickerComboBox.DisplayMember = "1";
            this.periodPickerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.periodPickerComboBox.FormattingEnabled = true;
            this.periodPickerComboBox.Location = new System.Drawing.Point(464, 41);
            this.periodPickerComboBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.periodPickerComboBox.Name = "periodPickerComboBox";
            this.periodPickerComboBox.Size = new System.Drawing.Size(129, 21);
            this.periodPickerComboBox.TabIndex = 4;
            this.periodPickerComboBox.Tag = "";
            this.periodPickerComboBox.SelectedIndexChanged += new System.EventHandler(this.periodPickerComboBox_SelectedIndexChanged);
            // 
            // AdditionRadioButton
            // 
            this.AdditionRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AdditionRadioButton.Location = new System.Drawing.Point(394, 295);
            this.AdditionRadioButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.AdditionRadioButton.Name = "AdditionRadioButton";
            this.AdditionRadioButton.Size = new System.Drawing.Size(130, 17);
            this.AdditionRadioButton.TabIndex = 5;
            this.AdditionRadioButton.TabStop = true;
            this.AdditionRadioButton.Text = "Addition";
            this.AdditionRadioButton.UseVisualStyleBackColor = true;
            this.AdditionRadioButton.CheckedChanged += new System.EventHandler(this.AdditionRadioButton_CheckedChanged);
            // 
            // multiplicationRadioButton
            // 
            this.multiplicationRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.multiplicationRadioButton.Location = new System.Drawing.Point(394, 317);
            this.multiplicationRadioButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.multiplicationRadioButton.Name = "multiplicationRadioButton";
            this.multiplicationRadioButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.multiplicationRadioButton.Size = new System.Drawing.Size(130, 17);
            this.multiplicationRadioButton.TabIndex = 6;
            this.multiplicationRadioButton.TabStop = true;
            this.multiplicationRadioButton.Text = "Multiplication";
            this.multiplicationRadioButton.UseVisualStyleBackColor = true;
            this.multiplicationRadioButton.CheckedChanged += new System.EventHandler(this.multiplicationRadioButton_CheckedChanged);
            // 
            // crossCorrelationRadioButton
            // 
            this.crossCorrelationRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.crossCorrelationRadioButton.Location = new System.Drawing.Point(394, 339);
            this.crossCorrelationRadioButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.crossCorrelationRadioButton.Name = "crossCorrelationRadioButton";
            this.crossCorrelationRadioButton.Size = new System.Drawing.Size(130, 17);
            this.crossCorrelationRadioButton.TabIndex = 7;
            this.crossCorrelationRadioButton.TabStop = true;
            this.crossCorrelationRadioButton.Text = "Cross correlation";
            this.crossCorrelationRadioButton.UseVisualStyleBackColor = true;
            this.crossCorrelationRadioButton.CheckedChanged += new System.EventHandler(this.crossCorrelationRadioButton_CheckedChanged);
            // 
            // OrthogonalisationRadioButton
            // 
            this.OrthogonalisationRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OrthogonalisationRadioButton.Location = new System.Drawing.Point(394, 360);
            this.OrthogonalisationRadioButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.OrthogonalisationRadioButton.Name = "OrthogonalisationRadioButton";
            this.OrthogonalisationRadioButton.Size = new System.Drawing.Size(130, 17);
            this.OrthogonalisationRadioButton.TabIndex = 8;
            this.OrthogonalisationRadioButton.TabStop = true;
            this.OrthogonalisationRadioButton.Text = "Orthogonalisation";
            this.OrthogonalisationRadioButton.UseVisualStyleBackColor = true;
            this.OrthogonalisationRadioButton.CheckedChanged += new System.EventHandler(this.OrthogonalisationRadioButton_CheckedChanged);
            // 
            // orthogonalSignalsComboBox
            // 
            this.orthogonalSignalsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.orthogonalSignalsComboBox.DisplayMember = "1";
            this.orthogonalSignalsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.orthogonalSignalsComboBox.FormattingEnabled = true;
            this.orthogonalSignalsComboBox.Location = new System.Drawing.Point(466, 383);
            this.orthogonalSignalsComboBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.orthogonalSignalsComboBox.Name = "orthogonalSignalsComboBox";
            this.orthogonalSignalsComboBox.Size = new System.Drawing.Size(129, 21);
            this.orthogonalSignalsComboBox.TabIndex = 9;
            this.orthogonalSignalsComboBox.Tag = "";
            this.orthogonalSignalsComboBox.SelectedIndexChanged += new System.EventHandler(this.orthogonalSignalsComboBox_SelectedIndexChanged);
            // 
            // offsetScrollBar
            // 
            this.offsetScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.offsetScrollBar.Location = new System.Drawing.Point(392, 170);
            this.offsetScrollBar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.offsetScrollBar.Name = "offsetScrollBar";
            this.offsetScrollBar.Size = new System.Drawing.Size(198, 21);
            this.offsetScrollBar.TabIndex = 20;
            this.offsetScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.offsetScrollBar_Scroll);
            // 
            // offsetLabel
            // 
            this.offsetLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.offsetLabel.AutoSize = true;
            this.offsetLabel.Location = new System.Drawing.Point(470, 192);
            this.offsetLabel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.offsetLabel.Name = "offsetLabel";
            this.offsetLabel.Size = new System.Drawing.Size(38, 13);
            this.offsetLabel.TabIndex = 19;
            this.offsetLabel.Text = "Offset:";
            // 
            // synchronizePeriodsCheckBox
            // 
            this.synchronizePeriodsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.synchronizePeriodsCheckBox.AutoSize = true;
            this.synchronizePeriodsCheckBox.Enabled = false;
            this.synchronizePeriodsCheckBox.Location = new System.Drawing.Point(392, 148);
            this.synchronizePeriodsCheckBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.synchronizePeriodsCheckBox.Name = "synchronizePeriodsCheckBox";
            this.synchronizePeriodsCheckBox.Size = new System.Drawing.Size(121, 17);
            this.synchronizePeriodsCheckBox.TabIndex = 21;
            this.synchronizePeriodsCheckBox.Text = "Synchronize periods";
            this.synchronizePeriodsCheckBox.UseVisualStyleBackColor = true;
            this.synchronizePeriodsCheckBox.CheckedChanged += new System.EventHandler(this.synchronizePeriodsCheckBox_CheckedChanged);
            // 
            // centralizeCheckBox
            // 
            this.centralizeCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.centralizeCheckBox.AutoSize = true;
            this.centralizeCheckBox.Location = new System.Drawing.Point(392, 126);
            this.centralizeCheckBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.centralizeCheckBox.Name = "centralizeCheckBox";
            this.centralizeCheckBox.Size = new System.Drawing.Size(72, 17);
            this.centralizeCheckBox.TabIndex = 22;
            this.centralizeCheckBox.Text = "Centralize";
            this.centralizeCheckBox.UseVisualStyleBackColor = true;
            this.centralizeCheckBox.CheckedChanged += new System.EventHandler(this.centralizeCheckBox_CheckedChanged);
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.applyButton.Location = new System.Drawing.Point(469, 84);
            this.applyButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(56, 20);
            this.applyButton.TabIndex = 24;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // periodDurationTextBox
            // 
            this.periodDurationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.periodDurationTextBox.ForeColor = System.Drawing.Color.Black;
            this.periodDurationTextBox.Location = new System.Drawing.Point(397, 85);
            this.periodDurationTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.periodDurationTextBox.Name = "periodDurationTextBox";
            this.periodDurationTextBox.Size = new System.Drawing.Size(59, 20);
            this.periodDurationTextBox.TabIndex = 23;
            this.periodDurationTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.periodDurationTextBox_KeyPress);
            // 
            // periodDurationLabel
            // 
            this.periodDurationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.periodDurationLabel.AutoSize = true;
            this.periodDurationLabel.Location = new System.Drawing.Point(394, 69);
            this.periodDurationLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.periodDurationLabel.Name = "periodDurationLabel";
            this.periodDurationLabel.Size = new System.Drawing.Size(109, 13);
            this.periodDurationLabel.TabIndex = 25;
            this.periodDurationLabel.Text = "Period duration (secs)";
            // 
            // fuseOrthogonalizationButton
            // 
            this.fuseOrthogonalizationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fuseOrthogonalizationButton.Enabled = false;
            this.fuseOrthogonalizationButton.Location = new System.Drawing.Point(397, 382);
            this.fuseOrthogonalizationButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.fuseOrthogonalizationButton.Name = "fuseOrthogonalizationButton";
            this.fuseOrthogonalizationButton.Size = new System.Drawing.Size(56, 20);
            this.fuseOrthogonalizationButton.TabIndex = 26;
            this.fuseOrthogonalizationButton.Text = "Fuse";
            this.fuseOrthogonalizationButton.UseVisualStyleBackColor = true;
            this.fuseOrthogonalizationButton.Click += new System.EventHandler(this.fuseOrthogonalizationButton_Click);
            // 
            // pathLabel
            // 
            this.pathLabel.AutoSize = true;
            this.pathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathLabel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pathLabel.Location = new System.Drawing.Point(9, 7);
            this.pathLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(37, 17);
            this.pathLabel.TabIndex = 27;
            this.pathLabel.Text = "Path";
            // 
            // FormSignalFusion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 413);
            this.Controls.Add(this.pathLabel);
            this.Controls.Add(this.fuseOrthogonalizationButton);
            this.Controls.Add(this.periodDurationLabel);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.periodDurationTextBox);
            this.Controls.Add(this.centralizeCheckBox);
            this.Controls.Add(this.synchronizePeriodsCheckBox);
            this.Controls.Add(this.offsetScrollBar);
            this.Controls.Add(this.offsetLabel);
            this.Controls.Add(this.orthogonalSignalsComboBox);
            this.Controls.Add(this.OrthogonalisationRadioButton);
            this.Controls.Add(this.crossCorrelationRadioButton);
            this.Controls.Add(this.multiplicationRadioButton);
            this.Controls.Add(this.AdditionRadioButton);
            this.Controls.Add(this.periodPickerComboBox);
            this.Controls.Add(this.fusionChart);
            this.Controls.Add(this.periodsChart);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FormSignalFusion";
            this.Text = "Signal Fusion";
            ((System.ComponentModel.ISupportInitialize)(this.periodsChart)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fusionChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart periodsChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart fusionChart;
        private System.Windows.Forms.ComboBox periodPickerComboBox;
        private System.Windows.Forms.RadioButton AdditionRadioButton;
        private System.Windows.Forms.RadioButton multiplicationRadioButton;
        private System.Windows.Forms.RadioButton crossCorrelationRadioButton;
        private System.Windows.Forms.RadioButton OrthogonalisationRadioButton;
        private System.Windows.Forms.ComboBox orthogonalSignalsComboBox;
        private System.Windows.Forms.HScrollBar offsetScrollBar;
        private System.Windows.Forms.Label offsetLabel;
        public System.Windows.Forms.CheckBox synchronizePeriodsCheckBox;
        public System.Windows.Forms.CheckBox centralizeCheckBox;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.TextBox periodDurationTextBox;
        private System.Windows.Forms.Label periodDurationLabel;
        private System.Windows.Forms.Button fuseOrthogonalizationButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem sendSignalToolStripMenuItem;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.ToolStripMenuItem analyseToolStripMenuItem;
    }
}
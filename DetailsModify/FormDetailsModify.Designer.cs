namespace BSP_Using_AI.DetailsModify
{
    partial class FormDetailsModify
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
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.signalChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sendSignalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analyseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spectrumChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.signalsPickerComboBox = new System.Windows.Forms.ComboBox();
            this.samplingRateTextBox = new System.Windows.Forms.TextBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.filtersFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.ChoseFilterLabel = new System.Windows.Forms.Label();
            this.filtersComboBox = new System.Windows.Forms.ComboBox();
            this.autoApplyCheckBox = new System.Windows.Forms.CheckBox();
            this.choseTransformLabel = new System.Windows.Forms.Label();
            this.fftButton = new System.Windows.Forms.Button();
            this.dwtButton = new System.Windows.Forms.Button();
            this.dwtLevelsComboBox = new System.Windows.Forms.ComboBox();
            this.editButton = new System.Windows.Forms.Button();
            this.signalFusionButton = new System.Windows.Forms.Button();
            this.pathLabel = new System.Windows.Forms.Label();
            this.setFeaturesLabelsButton = new System.Windows.Forms.Button();
            this.separatorLabel = new System.Windows.Forms.Label();
            this.featuresSettingInstructionsLabel = new System.Windows.Forms.Label();
            this.previousButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.discardButton = new System.Windows.Forms.Button();
            this.aiGoalComboBox = new System.Windows.Forms.ComboBox();
            this.featuresTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.predictButton = new System.Windows.Forms.Button();
            this.modelTypeComboBox = new System.Windows.Forms.ComboBox();
            this.saveAsImageButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.signalChart)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spectrumChart)).BeginInit();
            this.SuspendLayout();
            // 
            // signalChart
            // 
            this.signalChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.AxisX.LabelStyle.Format = "0.00";
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisX.Title = "Time (s)";
            chartArea1.AxisY.LabelStyle.Format = "0.00";
            chartArea1.AxisY.MajorGrid.Enabled = false;
            chartArea1.AxisY.Title = "Voltage (mV)";
            chartArea1.Name = "ChartArea1";
            this.signalChart.ChartAreas.Add(chartArea1);
            this.signalChart.ContextMenuStrip = this.contextMenuStrip1;
            legend1.IsTextAutoFit = false;
            legend1.Name = "Legend1";
            this.signalChart.Legends.Add(legend1);
            this.signalChart.Location = new System.Drawing.Point(9, 67);
            this.signalChart.Margin = new System.Windows.Forms.Padding(2);
            this.signalChart.Name = "signalChart";
            this.signalChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SemiTransparent;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.Black;
            series1.Legend = "Legend1";
            series1.MarkerBorderColor = System.Drawing.Color.Black;
            series1.Name = "Signal";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series2.Color = System.Drawing.Color.Blue;
            series2.LabelForeColor = System.Drawing.Color.Transparent;
            series2.Legend = "Legend1";
            series2.MarkerSize = 6;
            series2.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series2.Name = "Up peaks";
            series2.SmartLabelStyle.Enabled = false;
            series2.YValuesPerPoint = 2;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series3.Color = System.Drawing.Color.Red;
            series3.LabelForeColor = System.Drawing.Color.Transparent;
            series3.Legend = "Legend1";
            series3.MarkerSize = 6;
            series3.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series3.Name = "Down peaks";
            series3.SmartLabelStyle.Enabled = false;
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series4.Color = System.Drawing.Color.Black;
            series4.LabelForeColor = System.Drawing.Color.Transparent;
            series4.Legend = "Legend1";
            series4.MarkerSize = 6;
            series4.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series4.Name = "Stable";
            series4.SmartLabelStyle.Enabled = false;
            series4.YValuesPerPoint = 2;
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bubble;
            series5.Color = System.Drawing.Color.Red;
            series5.EmptyPointStyle.MarkerSize = 8;
            series5.EmptyPointStyle.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;
            series5.Legend = "Legend1";
            series5.MarkerBorderWidth = 2;
            series5.MarkerColor = System.Drawing.Color.Red;
            series5.MarkerSize = 8;
            series5.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;
            series5.Name = "Selection";
            series5.YValuesPerPoint = 6;
            series6.ChartArea = "ChartArea1";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series6.Color = System.Drawing.Color.Blue;
            series6.Legend = "Legend1";
            series6.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series6.Name = "Labels";
            this.signalChart.Series.Add(series1);
            this.signalChart.Series.Add(series2);
            this.signalChart.Series.Add(series3);
            this.signalChart.Series.Add(series4);
            this.signalChart.Series.Add(series5);
            this.signalChart.Series.Add(series6);
            this.signalChart.Size = new System.Drawing.Size(626, 168);
            this.signalChart.TabIndex = 1;
            this.signalChart.Text = "Signal Chart";
            this.signalChart.MouseClick += new System.Windows.Forms.MouseEventHandler(this.signalChart_MouseClick);
            this.signalChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseDown);
            this.signalChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseMove);
            this.signalChart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseUp);
            this.signalChart.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseWheel);
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
            // spectrumChart
            // 
            this.spectrumChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea2.AxisX.LabelStyle.Format = "0.00";
            chartArea2.AxisY.LabelStyle.Format = "0.00";
            chartArea2.Name = "ChartArea1";
            this.spectrumChart.ChartAreas.Add(chartArea2);
            this.spectrumChart.ContextMenuStrip = this.contextMenuStrip1;
            legend2.Name = "Legend1";
            this.spectrumChart.Legends.Add(legend2);
            this.spectrumChart.Location = new System.Drawing.Point(9, 295);
            this.spectrumChart.Margin = new System.Windows.Forms.Padding(2);
            this.spectrumChart.Name = "spectrumChart";
            this.spectrumChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SemiTransparent;
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series7.Color = System.Drawing.Color.Blue;
            series7.Legend = "Legend1";
            series7.Name = "Spectrum";
            series7.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series7.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            this.spectrumChart.Series.Add(series7);
            this.spectrumChart.Size = new System.Drawing.Size(626, 251);
            this.spectrumChart.TabIndex = 2;
            this.spectrumChart.Text = "Spectrum Exhibitor";
            this.spectrumChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseDown);
            this.spectrumChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseMove);
            this.spectrumChart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseUp);
            this.spectrumChart.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseWheel);
            // 
            // signalsPickerComboBox
            // 
            this.signalsPickerComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.signalsPickerComboBox.DisplayMember = "1";
            this.signalsPickerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.signalsPickerComboBox.FormattingEnabled = true;
            this.signalsPickerComboBox.Items.AddRange(new object[] {
            "Original signal",
            "Filtered signal"});
            this.signalsPickerComboBox.Location = new System.Drawing.Point(507, 42);
            this.signalsPickerComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.signalsPickerComboBox.Name = "signalsPickerComboBox";
            this.signalsPickerComboBox.Size = new System.Drawing.Size(129, 21);
            this.signalsPickerComboBox.TabIndex = 3;
            this.signalsPickerComboBox.Tag = "";
            this.signalsPickerComboBox.SelectedIndexChanged += new System.EventHandler(this.signalsPickerComboBox_SelectedIndexChanged);
            // 
            // samplingRateTextBox
            // 
            this.samplingRateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.samplingRateTextBox.ForeColor = System.Drawing.Color.Silver;
            this.samplingRateTextBox.Location = new System.Drawing.Point(433, 42);
            this.samplingRateTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.samplingRateTextBox.Name = "samplingRateTextBox";
            this.samplingRateTextBox.Size = new System.Drawing.Size(71, 20);
            this.samplingRateTextBox.TabIndex = 6;
            this.samplingRateTextBox.Text = "Sampling rate";
            this.samplingRateTextBox.Enter += new System.EventHandler(this.samplingRateTextBox_Enter);
            this.samplingRateTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.samplingRateTextBox_KeyPress);
            this.samplingRateTextBox.Leave += new System.EventHandler(this.samplingRateTextBox_Leave);
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.applyButton.Location = new System.Drawing.Point(372, 41);
            this.applyButton.Margin = new System.Windows.Forms.Padding(2);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(56, 20);
            this.applyButton.TabIndex = 7;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // filtersFlowLayoutPanel
            // 
            this.filtersFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.filtersFlowLayoutPanel.AutoScroll = true;
            this.filtersFlowLayoutPanel.Location = new System.Drawing.Point(640, 67);
            this.filtersFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(2);
            this.filtersFlowLayoutPanel.Name = "filtersFlowLayoutPanel";
            this.filtersFlowLayoutPanel.Size = new System.Drawing.Size(298, 479);
            this.filtersFlowLayoutPanel.TabIndex = 8;
            // 
            // ChoseFilterLabel
            // 
            this.ChoseFilterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ChoseFilterLabel.AutoSize = true;
            this.ChoseFilterLabel.Location = new System.Drawing.Point(740, 45);
            this.ChoseFilterLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ChoseFilterLabel.Name = "ChoseFilterLabel";
            this.ChoseFilterLabel.Size = new System.Drawing.Size(59, 13);
            this.ChoseFilterLabel.TabIndex = 9;
            this.ChoseFilterLabel.Text = "Chose filter";
            // 
            // filtersComboBox
            // 
            this.filtersComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.filtersComboBox.DisplayMember = "1";
            this.filtersComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filtersComboBox.FormattingEnabled = true;
            this.filtersComboBox.Items.AddRange(new object[] {
            "Butter Worth",
            "Chebyshev I",
            "Chebyshev II",
            "DC removal",
            "Normalize signal",
            "Absolute signal",
            "Singal states viewer"});
            this.filtersComboBox.Location = new System.Drawing.Point(810, 42);
            this.filtersComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.filtersComboBox.Name = "filtersComboBox";
            this.filtersComboBox.Size = new System.Drawing.Size(129, 21);
            this.filtersComboBox.TabIndex = 10;
            this.filtersComboBox.Tag = "";
            this.filtersComboBox.SelectedIndexChanged += new System.EventHandler(this.filtersComboBox_SelectedIndexChanged);
            // 
            // autoApplyCheckBox
            // 
            this.autoApplyCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.autoApplyCheckBox.AutoSize = true;
            this.autoApplyCheckBox.Checked = true;
            this.autoApplyCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoApplyCheckBox.Location = new System.Drawing.Point(292, 43);
            this.autoApplyCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.autoApplyCheckBox.Name = "autoApplyCheckBox";
            this.autoApplyCheckBox.Size = new System.Drawing.Size(76, 17);
            this.autoApplyCheckBox.TabIndex = 11;
            this.autoApplyCheckBox.Text = "Auto apply";
            this.autoApplyCheckBox.UseVisualStyleBackColor = true;
            // 
            // choseTransformLabel
            // 
            this.choseTransformLabel.AutoSize = true;
            this.choseTransformLabel.Location = new System.Drawing.Point(9, 259);
            this.choseTransformLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.choseTransformLabel.Name = "choseTransformLabel";
            this.choseTransformLabel.Size = new System.Drawing.Size(83, 13);
            this.choseTransformLabel.TabIndex = 12;
            this.choseTransformLabel.Text = "Chose transform";
            // 
            // fftButton
            // 
            this.fftButton.Enabled = false;
            this.fftButton.Location = new System.Drawing.Point(110, 243);
            this.fftButton.Margin = new System.Windows.Forms.Padding(2);
            this.fftButton.Name = "fftButton";
            this.fftButton.Size = new System.Drawing.Size(56, 20);
            this.fftButton.TabIndex = 13;
            this.fftButton.Text = "FFT";
            this.fftButton.UseVisualStyleBackColor = true;
            this.fftButton.Click += new System.EventHandler(this.fftButton_Click);
            // 
            // dwtButton
            // 
            this.dwtButton.Location = new System.Drawing.Point(110, 268);
            this.dwtButton.Margin = new System.Windows.Forms.Padding(2);
            this.dwtButton.Name = "dwtButton";
            this.dwtButton.Size = new System.Drawing.Size(56, 20);
            this.dwtButton.TabIndex = 14;
            this.dwtButton.Text = "DWT";
            this.dwtButton.UseVisualStyleBackColor = true;
            this.dwtButton.Click += new System.EventHandler(this.dwtButton_Click);
            // 
            // dwtLevelsComboBox
            // 
            this.dwtLevelsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dwtLevelsComboBox.DisplayMember = "1";
            this.dwtLevelsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dwtLevelsComboBox.Enabled = false;
            this.dwtLevelsComboBox.FormattingEnabled = true;
            this.dwtLevelsComboBox.Location = new System.Drawing.Point(507, 269);
            this.dwtLevelsComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.dwtLevelsComboBox.Name = "dwtLevelsComboBox";
            this.dwtLevelsComboBox.Size = new System.Drawing.Size(129, 21);
            this.dwtLevelsComboBox.TabIndex = 15;
            this.dwtLevelsComboBox.Tag = "";
            this.dwtLevelsComboBox.SelectedIndexChanged += new System.EventHandler(this.dwtLayersComboBox_SelectedIndexChanged);
            // 
            // editButton
            // 
            this.editButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editButton.Enabled = false;
            this.editButton.Location = new System.Drawing.Point(446, 268);
            this.editButton.Margin = new System.Windows.Forms.Padding(2);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(56, 20);
            this.editButton.TabIndex = 16;
            this.editButton.Text = "Edit";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // signalFusionButton
            // 
            this.signalFusionButton.Location = new System.Drawing.Point(9, 41);
            this.signalFusionButton.Margin = new System.Windows.Forms.Padding(2);
            this.signalFusionButton.Name = "signalFusionButton";
            this.signalFusionButton.Size = new System.Drawing.Size(89, 20);
            this.signalFusionButton.TabIndex = 17;
            this.signalFusionButton.Text = "Signal fusion";
            this.signalFusionButton.UseVisualStyleBackColor = true;
            this.signalFusionButton.Click += new System.EventHandler(this.signalFusionButton_Click);
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
            this.pathLabel.TabIndex = 19;
            this.pathLabel.Text = "Path";
            // 
            // setFeaturesLabelsButton
            // 
            this.setFeaturesLabelsButton.BackColor = System.Drawing.SystemColors.Highlight;
            this.setFeaturesLabelsButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.setFeaturesLabelsButton.Location = new System.Drawing.Point(9, 568);
            this.setFeaturesLabelsButton.Margin = new System.Windows.Forms.Padding(2);
            this.setFeaturesLabelsButton.Name = "setFeaturesLabelsButton";
            this.setFeaturesLabelsButton.Size = new System.Drawing.Size(85, 34);
            this.setFeaturesLabelsButton.TabIndex = 20;
            this.setFeaturesLabelsButton.Text = "Set features labels";
            this.setFeaturesLabelsButton.UseVisualStyleBackColor = false;
            this.setFeaturesLabelsButton.Click += new System.EventHandler(this.setFeaturesLabelsButton_Click);
            // 
            // separatorLabel
            // 
            this.separatorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separatorLabel.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.separatorLabel.Location = new System.Drawing.Point(2, 564);
            this.separatorLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.separatorLabel.Name = "separatorLabel";
            this.separatorLabel.Size = new System.Drawing.Size(939, 1);
            this.separatorLabel.TabIndex = 21;
            // 
            // featuresSettingInstructionsLabel
            // 
            this.featuresSettingInstructionsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.featuresSettingInstructionsLabel.Location = new System.Drawing.Point(6, 604);
            this.featuresSettingInstructionsLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.featuresSettingInstructionsLabel.Name = "featuresSettingInstructionsLabel";
            this.featuresSettingInstructionsLabel.Size = new System.Drawing.Size(629, 110);
            this.featuresSettingInstructionsLabel.TabIndex = 22;
            // 
            // previousButton
            // 
            this.previousButton.Enabled = false;
            this.previousButton.Location = new System.Drawing.Point(5, 716);
            this.previousButton.Margin = new System.Windows.Forms.Padding(2);
            this.previousButton.Name = "previousButton";
            this.previousButton.Size = new System.Drawing.Size(56, 20);
            this.previousButton.TabIndex = 24;
            this.previousButton.Text = "Previous";
            this.previousButton.UseVisualStyleBackColor = true;
            this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Enabled = false;
            this.nextButton.Location = new System.Drawing.Point(75, 716);
            this.nextButton.Margin = new System.Windows.Forms.Padding(2);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(56, 20);
            this.nextButton.TabIndex = 25;
            this.nextButton.Text = "Next";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // discardButton
            // 
            this.discardButton.BackColor = System.Drawing.Color.Red;
            this.discardButton.Enabled = false;
            this.discardButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.discardButton.Location = new System.Drawing.Point(110, 568);
            this.discardButton.Margin = new System.Windows.Forms.Padding(2);
            this.discardButton.Name = "discardButton";
            this.discardButton.Size = new System.Drawing.Size(85, 34);
            this.discardButton.TabIndex = 26;
            this.discardButton.Text = "Discard";
            this.discardButton.UseVisualStyleBackColor = false;
            this.discardButton.Click += new System.EventHandler(this.discardButton_Click);
            // 
            // aiGoalComboBox
            // 
            this.aiGoalComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.aiGoalComboBox.DisplayMember = "1";
            this.aiGoalComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.aiGoalComboBox.DropDownWidth = 140;
            this.aiGoalComboBox.FormattingEnabled = true;
            this.aiGoalComboBox.Items.AddRange(new object[] {
            "WPW syndrome detection"});
            this.aiGoalComboBox.Location = new System.Drawing.Point(506, 568);
            this.aiGoalComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.aiGoalComboBox.Name = "aiGoalComboBox";
            this.aiGoalComboBox.Size = new System.Drawing.Size(129, 21);
            this.aiGoalComboBox.TabIndex = 27;
            this.aiGoalComboBox.Tag = "";
            // 
            // featuresTableLayoutPanel
            // 
            this.featuresTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.featuresTableLayoutPanel.AutoScroll = true;
            this.featuresTableLayoutPanel.ColumnCount = 1;
            this.featuresTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.featuresTableLayoutPanel.Location = new System.Drawing.Point(641, 568);
            this.featuresTableLayoutPanel.Margin = new System.Windows.Forms.Padding(2);
            this.featuresTableLayoutPanel.Name = "featuresTableLayoutPanel";
            this.featuresTableLayoutPanel.RowCount = 1;
            this.featuresTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.featuresTableLayoutPanel.Size = new System.Drawing.Size(298, 172);
            this.featuresTableLayoutPanel.TabIndex = 28;
            // 
            // predictButton
            // 
            this.predictButton.Location = new System.Drawing.Point(566, 716);
            this.predictButton.Margin = new System.Windows.Forms.Padding(2);
            this.predictButton.Name = "predictButton";
            this.predictButton.Size = new System.Drawing.Size(69, 22);
            this.predictButton.TabIndex = 30;
            this.predictButton.Text = "Predict";
            this.predictButton.UseVisualStyleBackColor = true;
            this.predictButton.Click += new System.EventHandler(this.predictButton_Click);
            // 
            // modelTypeComboBox
            // 
            this.modelTypeComboBox.DisplayMember = "1";
            this.modelTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modelTypeComboBox.DropDownWidth = 300;
            this.modelTypeComboBox.FormattingEnabled = true;
            this.modelTypeComboBox.Location = new System.Drawing.Point(433, 716);
            this.modelTypeComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.modelTypeComboBox.Name = "modelTypeComboBox";
            this.modelTypeComboBox.Size = new System.Drawing.Size(129, 21);
            this.modelTypeComboBox.TabIndex = 29;
            this.modelTypeComboBox.Tag = "";
            // 
            // saveAsImageButton
            // 
            this.saveAsImageButton.Location = new System.Drawing.Point(548, 239);
            this.saveAsImageButton.Margin = new System.Windows.Forms.Padding(2);
            this.saveAsImageButton.Name = "saveAsImageButton";
            this.saveAsImageButton.Size = new System.Drawing.Size(87, 20);
            this.saveAsImageButton.TabIndex = 31;
            this.saveAsImageButton.Text = "Save as image";
            this.saveAsImageButton.UseVisualStyleBackColor = true;
            this.saveAsImageButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // FormDetailsModify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(946, 740);
            this.Controls.Add(this.saveAsImageButton);
            this.Controls.Add(this.predictButton);
            this.Controls.Add(this.modelTypeComboBox);
            this.Controls.Add(this.featuresTableLayoutPanel);
            this.Controls.Add(this.aiGoalComboBox);
            this.Controls.Add(this.discardButton);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.previousButton);
            this.Controls.Add(this.featuresSettingInstructionsLabel);
            this.Controls.Add(this.separatorLabel);
            this.Controls.Add(this.setFeaturesLabelsButton);
            this.Controls.Add(this.pathLabel);
            this.Controls.Add(this.signalFusionButton);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.dwtLevelsComboBox);
            this.Controls.Add(this.dwtButton);
            this.Controls.Add(this.fftButton);
            this.Controls.Add(this.choseTransformLabel);
            this.Controls.Add(this.autoApplyCheckBox);
            this.Controls.Add(this.filtersComboBox);
            this.Controls.Add(this.ChoseFilterLabel);
            this.Controls.Add(this.filtersFlowLayoutPanel);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.samplingRateTextBox);
            this.Controls.Add(this.signalsPickerComboBox);
            this.Controls.Add(this.spectrumChart);
            this.Controls.Add(this.signalChart);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(604, 45);
            this.Name = "FormDetailsModify";
            this.Text = "FormDetailsModify";
            ((System.ComponentModel.ISupportInitialize)(this.signalChart)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spectrumChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.DataVisualization.Charting.Chart signalChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart spectrumChart;
        private System.Windows.Forms.FlowLayoutPanel filtersFlowLayoutPanel;
        private System.Windows.Forms.Label ChoseFilterLabel;
        public System.Windows.Forms.ComboBox filtersComboBox;
        public System.Windows.Forms.CheckBox autoApplyCheckBox;
        private System.Windows.Forms.Label choseTransformLabel;
        private System.Windows.Forms.ComboBox dwtLevelsComboBox;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem sendSignalToolStripMenuItem;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.ToolStripMenuItem analyseToolStripMenuItem;
        public System.Windows.Forms.Button setFeaturesLabelsButton;
        private System.Windows.Forms.Label separatorLabel;
        public System.Windows.Forms.Label featuresSettingInstructionsLabel;
        public System.Windows.Forms.Button previousButton;
        public System.Windows.Forms.Button nextButton;
        public System.Windows.Forms.Button discardButton;
        public System.Windows.Forms.ComboBox aiGoalComboBox;
        public System.Windows.Forms.TableLayoutPanel featuresTableLayoutPanel;
        public System.Windows.Forms.ComboBox signalsPickerComboBox;
        public System.Windows.Forms.TextBox samplingRateTextBox;
        public System.Windows.Forms.Button applyButton;
        public System.Windows.Forms.Button dwtButton;
        public System.Windows.Forms.Button signalFusionButton;
        public System.Windows.Forms.Button predictButton;
        public System.Windows.Forms.ComboBox modelTypeComboBox;
        public System.Windows.Forms.Button fftButton;
        public System.Windows.Forms.Button saveAsImageButton;
    }
}
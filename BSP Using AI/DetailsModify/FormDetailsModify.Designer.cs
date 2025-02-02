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
            this.signalChart = new ScottPlot.FormsPlot();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sendSignalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analyseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spectrumChart = new ScottPlot.FormsPlot();
            this.signalsPickerComboBox = new System.Windows.Forms.ComboBox();
            this.samplingRateTextBox = new System.Windows.Forms.TextBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.filtersFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.ChoseFilterLabel = new System.Windows.Forms.Label();
            this.filtersComboBox = new System.Windows.Forms.ComboBox();
            this.autoApplyCheckBox = new System.Windows.Forms.CheckBox();
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
            this.quantizationStepTextBox = new System.Windows.Forms.TextBox();
            this.samplingRateLabel = new System.Windows.Forms.Label();
            this.quantizationStepLabel = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.exportToWFDBButton = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // signalChart
            // 
            this.signalChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.signalChart.ContextMenuStrip = this.contextMenuStrip1;
            this.signalChart.Location = new System.Drawing.Point(10, 77);
            this.signalChart.Margin = new System.Windows.Forms.Padding(2);
            this.signalChart.Name = "signalChart";
            this.signalChart.Size = new System.Drawing.Size(730, 194);
            this.signalChart.TabIndex = 1;
            this.signalChart.DoubleClick += new System.EventHandler(this.signalChart_DoubleClick);
            this.signalChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.signalChart_MouseClick);
            this.signalChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseMove);
            this.signalChart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.signalChart_MouseUp);
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
            this.spectrumChart.ContextMenuStrip = this.contextMenuStrip1;
            this.spectrumChart.Location = new System.Drawing.Point(10, 340);
            this.spectrumChart.Margin = new System.Windows.Forms.Padding(2);
            this.spectrumChart.Name = "spectrumChart";
            this.spectrumChart.Size = new System.Drawing.Size(730, 290);
            this.spectrumChart.TabIndex = 2;
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
            this.signalsPickerComboBox.Location = new System.Drawing.Point(592, 48);
            this.signalsPickerComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.signalsPickerComboBox.Name = "signalsPickerComboBox";
            this.signalsPickerComboBox.Size = new System.Drawing.Size(150, 23);
            this.signalsPickerComboBox.TabIndex = 3;
            this.signalsPickerComboBox.Tag = "";
            this.signalsPickerComboBox.SelectedIndexChanged += new System.EventHandler(this.signalsPickerComboBox_SelectedIndexChanged);
            // 
            // samplingRateTextBox
            // 
            this.samplingRateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.samplingRateTextBox.ForeColor = System.Drawing.Color.Black;
            this.samplingRateTextBox.Location = new System.Drawing.Point(416, 47);
            this.samplingRateTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.samplingRateTextBox.Name = "samplingRateTextBox";
            this.samplingRateTextBox.Size = new System.Drawing.Size(82, 23);
            this.samplingRateTextBox.TabIndex = 6;
            this.samplingRateTextBox.Text = "Sampling rate";
            this.samplingRateTextBox.TextChanged += new System.EventHandler(this.samplingRateTextBox_TextChanged);
            this.samplingRateTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.samplingRateTextBox_KeyPress);
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.applyButton.Location = new System.Drawing.Point(346, 48);
            this.applyButton.Margin = new System.Windows.Forms.Padding(2);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(65, 23);
            this.applyButton.TabIndex = 7;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // filtersFlowLayoutPanel
            // 
            this.filtersFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.filtersFlowLayoutPanel.AutoScroll = true;
            this.filtersFlowLayoutPanel.Location = new System.Drawing.Point(747, 77);
            this.filtersFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(2);
            this.filtersFlowLayoutPanel.Name = "filtersFlowLayoutPanel";
            this.filtersFlowLayoutPanel.Size = new System.Drawing.Size(348, 553);
            this.filtersFlowLayoutPanel.TabIndex = 8;
            // 
            // ChoseFilterLabel
            // 
            this.ChoseFilterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ChoseFilterLabel.AutoSize = true;
            this.ChoseFilterLabel.Location = new System.Drawing.Point(863, 52);
            this.ChoseFilterLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ChoseFilterLabel.Name = "ChoseFilterLabel";
            this.ChoseFilterLabel.Size = new System.Drawing.Size(67, 15);
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
            "Butterworth",
            "Chebyshev I",
            "Chebyshev II",
            "Median filter",
            "DC removal",
            "Normalize signal",
            "Absolute signal",
            "DWT",
            "Peaks analyzer",
            "Corners scanner",
            "Distribution display"});
            this.filtersComboBox.Location = new System.Drawing.Point(945, 48);
            this.filtersComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.filtersComboBox.Name = "filtersComboBox";
            this.filtersComboBox.Size = new System.Drawing.Size(150, 23);
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
            this.autoApplyCheckBox.Location = new System.Drawing.Point(258, 51);
            this.autoApplyCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.autoApplyCheckBox.Name = "autoApplyCheckBox";
            this.autoApplyCheckBox.Size = new System.Drawing.Size(84, 19);
            this.autoApplyCheckBox.TabIndex = 11;
            this.autoApplyCheckBox.Text = "Auto apply";
            this.autoApplyCheckBox.UseVisualStyleBackColor = true;
            this.autoApplyCheckBox.CheckedChanged += new System.EventHandler(this.autoApplyCheckBox_CheckedChanged);
            // 
            // signalFusionButton
            // 
            this.signalFusionButton.Location = new System.Drawing.Point(10, 47);
            this.signalFusionButton.Margin = new System.Windows.Forms.Padding(2);
            this.signalFusionButton.Name = "signalFusionButton";
            this.signalFusionButton.Size = new System.Drawing.Size(104, 23);
            this.signalFusionButton.TabIndex = 17;
            this.signalFusionButton.Text = "Signal fusion";
            this.signalFusionButton.UseVisualStyleBackColor = true;
            this.signalFusionButton.Click += new System.EventHandler(this.signalFusionButton_Click);
            // 
            // pathLabel
            // 
            this.pathLabel.AutoSize = true;
            this.pathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.pathLabel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pathLabel.Location = new System.Drawing.Point(10, 8);
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
            this.setFeaturesLabelsButton.Location = new System.Drawing.Point(10, 655);
            this.setFeaturesLabelsButton.Margin = new System.Windows.Forms.Padding(2);
            this.setFeaturesLabelsButton.Name = "setFeaturesLabelsButton";
            this.setFeaturesLabelsButton.Size = new System.Drawing.Size(99, 39);
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
            this.separatorLabel.Location = new System.Drawing.Point(2, 651);
            this.separatorLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.separatorLabel.Name = "separatorLabel";
            this.separatorLabel.Size = new System.Drawing.Size(1096, 1);
            this.separatorLabel.TabIndex = 21;
            // 
            // featuresSettingInstructionsLabel
            // 
            this.featuresSettingInstructionsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.featuresSettingInstructionsLabel.Location = new System.Drawing.Point(7, 697);
            this.featuresSettingInstructionsLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.featuresSettingInstructionsLabel.Name = "featuresSettingInstructionsLabel";
            this.featuresSettingInstructionsLabel.Size = new System.Drawing.Size(734, 127);
            this.featuresSettingInstructionsLabel.TabIndex = 22;
            // 
            // previousButton
            // 
            this.previousButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.previousButton.Enabled = false;
            this.previousButton.Location = new System.Drawing.Point(6, 826);
            this.previousButton.Margin = new System.Windows.Forms.Padding(2);
            this.previousButton.Name = "previousButton";
            this.previousButton.Size = new System.Drawing.Size(65, 23);
            this.previousButton.TabIndex = 24;
            this.previousButton.Text = "Previous";
            this.previousButton.UseVisualStyleBackColor = true;
            this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nextButton.Enabled = false;
            this.nextButton.Location = new System.Drawing.Point(88, 826);
            this.nextButton.Margin = new System.Windows.Forms.Padding(2);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(65, 23);
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
            this.discardButton.Location = new System.Drawing.Point(128, 655);
            this.discardButton.Margin = new System.Windows.Forms.Padding(2);
            this.discardButton.Name = "discardButton";
            this.discardButton.Size = new System.Drawing.Size(99, 39);
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
            "WPW syndrome detection",
            "Characteristic waves delineation",
            "Arrhythmia classification"});
            this.aiGoalComboBox.Location = new System.Drawing.Point(590, 655);
            this.aiGoalComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.aiGoalComboBox.Name = "aiGoalComboBox";
            this.aiGoalComboBox.Size = new System.Drawing.Size(150, 23);
            this.aiGoalComboBox.TabIndex = 27;
            this.aiGoalComboBox.Tag = "";
            // 
            // featuresTableLayoutPanel
            // 
            this.featuresTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.featuresTableLayoutPanel.AutoScroll = true;
            this.featuresTableLayoutPanel.ColumnCount = 1;
            this.featuresTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.featuresTableLayoutPanel.Location = new System.Drawing.Point(748, 655);
            this.featuresTableLayoutPanel.Margin = new System.Windows.Forms.Padding(2);
            this.featuresTableLayoutPanel.Name = "featuresTableLayoutPanel";
            this.featuresTableLayoutPanel.RowCount = 1;
            this.featuresTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.featuresTableLayoutPanel.Size = new System.Drawing.Size(348, 198);
            this.featuresTableLayoutPanel.TabIndex = 28;
            // 
            // predictButton
            // 
            this.predictButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.predictButton.Location = new System.Drawing.Point(660, 826);
            this.predictButton.Margin = new System.Windows.Forms.Padding(2);
            this.predictButton.Name = "predictButton";
            this.predictButton.Size = new System.Drawing.Size(80, 25);
            this.predictButton.TabIndex = 30;
            this.predictButton.Text = "Predict";
            this.predictButton.UseVisualStyleBackColor = true;
            this.predictButton.Click += new System.EventHandler(this.predictButton_Click);
            // 
            // modelTypeComboBox
            // 
            this.modelTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.modelTypeComboBox.DisplayMember = "1";
            this.modelTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modelTypeComboBox.DropDownWidth = 300;
            this.modelTypeComboBox.FormattingEnabled = true;
            this.modelTypeComboBox.Location = new System.Drawing.Point(505, 826);
            this.modelTypeComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.modelTypeComboBox.Name = "modelTypeComboBox";
            this.modelTypeComboBox.Size = new System.Drawing.Size(150, 23);
            this.modelTypeComboBox.TabIndex = 29;
            this.modelTypeComboBox.Tag = "";
            // 
            // saveAsImageButton
            // 
            this.saveAsImageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveAsImageButton.Location = new System.Drawing.Point(639, 276);
            this.saveAsImageButton.Margin = new System.Windows.Forms.Padding(2);
            this.saveAsImageButton.Name = "saveAsImageButton";
            this.saveAsImageButton.Size = new System.Drawing.Size(102, 23);
            this.saveAsImageButton.TabIndex = 31;
            this.saveAsImageButton.Text = "Save as image";
            this.saveAsImageButton.UseVisualStyleBackColor = true;
            this.saveAsImageButton.Click += new System.EventHandler(this.saveAsImageButton_Click);
            // 
            // quantizationStepTextBox
            // 
            this.quantizationStepTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.quantizationStepTextBox.ForeColor = System.Drawing.Color.Black;
            this.quantizationStepTextBox.Location = new System.Drawing.Point(504, 47);
            this.quantizationStepTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.quantizationStepTextBox.Name = "quantizationStepTextBox";
            this.quantizationStepTextBox.Size = new System.Drawing.Size(82, 23);
            this.quantizationStepTextBox.TabIndex = 32;
            this.quantizationStepTextBox.Text = "Quantization step";
            this.quantizationStepTextBox.TextChanged += new System.EventHandler(this.quantizationStepTextBox_TextChanged);
            this.quantizationStepTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.samplingRateTextBox_KeyPress);
            // 
            // samplingRateLabel
            // 
            this.samplingRateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.samplingRateLabel.AutoSize = true;
            this.samplingRateLabel.Location = new System.Drawing.Point(413, 30);
            this.samplingRateLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.samplingRateLabel.Name = "samplingRateLabel";
            this.samplingRateLabel.Size = new System.Drawing.Size(80, 15);
            this.samplingRateLabel.TabIndex = 33;
            this.samplingRateLabel.Text = "Sampling rate";
            // 
            // quantizationStepLabel
            // 
            this.quantizationStepLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.quantizationStepLabel.AutoSize = true;
            this.quantizationStepLabel.Location = new System.Drawing.Point(503, 30);
            this.quantizationStepLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.quantizationStepLabel.Name = "quantizationStepLabel";
            this.quantizationStepLabel.Size = new System.Drawing.Size(100, 15);
            this.quantizationStepLabel.TabIndex = 34;
            this.quantizationStepLabel.Text = "Quantization step";
            // 
            // saveButton
            // 
            this.saveButton.BackColor = System.Drawing.Color.YellowGreen;
            this.saveButton.Enabled = false;
            this.saveButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.saveButton.Location = new System.Drawing.Point(246, 656);
            this.saveButton.Margin = new System.Windows.Forms.Padding(2);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(99, 39);
            this.saveButton.TabIndex = 36;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // exportToWFDBButton
            // 
            this.exportToWFDBButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.exportToWFDBButton.Location = new System.Drawing.Point(533, 276);
            this.exportToWFDBButton.Margin = new System.Windows.Forms.Padding(2);
            this.exportToWFDBButton.Name = "exportToWFDBButton";
            this.exportToWFDBButton.Size = new System.Drawing.Size(102, 23);
            this.exportToWFDBButton.TabIndex = 37;
            this.exportToWFDBButton.Text = "Export to WFDB";
            this.exportToWFDBButton.UseVisualStyleBackColor = true;
            this.exportToWFDBButton.Click += new System.EventHandler(this.exportToWFDBButton_Click);
            // 
            // FormDetailsModify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1104, 854);
            this.Controls.Add(this.exportToWFDBButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.quantizationStepLabel);
            this.Controls.Add(this.samplingRateLabel);
            this.Controls.Add(this.quantizationStepTextBox);
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
            this.Controls.Add(this.autoApplyCheckBox);
            this.Controls.Add(this.filtersComboBox);
            this.Controls.Add(this.ChoseFilterLabel);
            this.Controls.Add(this.filtersFlowLayoutPanel);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.samplingRateTextBox);
            this.Controls.Add(this.signalsPickerComboBox);
            this.Controls.Add(this.spectrumChart);
            this.Controls.Add(this.signalChart);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(702, 46);
            this.Name = "FormDetailsModify";
            this.Text = "FormDetailsModify";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormDetailsModify_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormDetailsModify_KeyUp);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public ScottPlot.FormsPlot signalChart;
        private ScottPlot.FormsPlot spectrumChart;
        private System.Windows.Forms.FlowLayoutPanel filtersFlowLayoutPanel;
        private System.Windows.Forms.Label ChoseFilterLabel;
        public System.Windows.Forms.ComboBox filtersComboBox;
        public System.Windows.Forms.CheckBox autoApplyCheckBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem sendSignalToolStripMenuItem;
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
        public System.Windows.Forms.Button signalFusionButton;
        public System.Windows.Forms.Button predictButton;
        public System.Windows.Forms.ComboBox modelTypeComboBox;
        public System.Windows.Forms.Button saveAsImageButton;
        public System.Windows.Forms.TextBox quantizationStepTextBox;
        private System.Windows.Forms.Label samplingRateLabel;
        private System.Windows.Forms.Label quantizationStepLabel;
        public System.Windows.Forms.Button saveButton;
        public System.Windows.Forms.Label pathLabel;
        public System.Windows.Forms.Button exportToWFDBButton;
    }
}
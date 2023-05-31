
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
            this.firstSignalChart = new ScottPlot.FormsPlot();
            this.secondSignalChart = new ScottPlot.FormsPlot();
            this.comparisonChart = new ScottPlot.FormsPlot();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sendSignalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analyseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectFirstSignalCheckBox = new System.Windows.Forms.CheckBox();
            this.selectSecondSignalCheckBox = new System.Windows.Forms.CheckBox();
            this.crosscorrelationRadioButton = new System.Windows.Forms.RadioButton();
            this.minimumDistanceRadioButton = new System.Windows.Forms.RadioButton();
            this.dynamicTimeWrapingRadioButton = new System.Windows.Forms.RadioButton();
            this.pathAccumulatedDistanceChart = new ScottPlot.FormsPlot();
            this.pathChart = new ScottPlot.FormsPlot();
            this.secondSignalPowerLabel = new System.Windows.Forms.Label();
            this.secondSignalPowerValueLabel = new System.Windows.Forms.Label();
            this.firstSignalPowerLabel = new System.Windows.Forms.Label();
            this.firstSignalPowerValueLabel = new System.Windows.Forms.Label();
            this.comparisonSignalPowerLabel = new System.Windows.Forms.Label();
            this.comparisonSignalPowerValueLabel = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // firstSignalChart
            // 
            this.firstSignalChart.Location = new System.Drawing.Point(10, 37);
            this.firstSignalChart.Margin = new System.Windows.Forms.Padding(2);
            this.firstSignalChart.Name = "firstSignalChart";
            this.firstSignalChart.Size = new System.Drawing.Size(444, 194);
            this.firstSignalChart.TabIndex = 3;
            // 
            // secondSignalChart
            // 
            this.secondSignalChart.Location = new System.Drawing.Point(695, 37);
            this.secondSignalChart.Margin = new System.Windows.Forms.Padding(2);
            this.secondSignalChart.Name = "secondSignalChart";
            this.secondSignalChart.Size = new System.Drawing.Size(444, 194);
            this.secondSignalChart.TabIndex = 4;
            // 
            // comparisonChart
            // 
            this.comparisonChart.ContextMenuStrip = this.contextMenuStrip1;
            this.comparisonChart.Location = new System.Drawing.Point(355, 293);
            this.comparisonChart.Margin = new System.Windows.Forms.Padding(2);
            this.comparisonChart.Name = "comparisonChart";
            this.comparisonChart.Size = new System.Drawing.Size(444, 194);
            this.comparisonChart.TabIndex = 5;
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
            this.selectFirstSignalCheckBox.Location = new System.Drawing.Point(10, 12);
            this.selectFirstSignalCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.selectFirstSignalCheckBox.Name = "selectFirstSignalCheckBox";
            this.selectFirstSignalCheckBox.Size = new System.Drawing.Size(114, 19);
            this.selectFirstSignalCheckBox.TabIndex = 23;
            this.selectFirstSignalCheckBox.Text = "Select first signal";
            this.selectFirstSignalCheckBox.UseVisualStyleBackColor = true;
            // 
            // selectSecondSignalCheckBox
            // 
            this.selectSecondSignalCheckBox.AutoSize = true;
            this.selectSecondSignalCheckBox.Location = new System.Drawing.Point(695, 12);
            this.selectSecondSignalCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.selectSecondSignalCheckBox.Name = "selectSecondSignalCheckBox";
            this.selectSecondSignalCheckBox.Size = new System.Drawing.Size(132, 19);
            this.selectSecondSignalCheckBox.TabIndex = 24;
            this.selectSecondSignalCheckBox.Text = "Select second signal";
            this.selectSecondSignalCheckBox.UseVisualStyleBackColor = true;
            // 
            // crosscorrelationRadioButton
            // 
            this.crosscorrelationRadioButton.Checked = true;
            this.crosscorrelationRadioButton.Location = new System.Drawing.Point(495, 88);
            this.crosscorrelationRadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.crosscorrelationRadioButton.Name = "crosscorrelationRadioButton";
            this.crosscorrelationRadioButton.Size = new System.Drawing.Size(159, 20);
            this.crosscorrelationRadioButton.TabIndex = 25;
            this.crosscorrelationRadioButton.TabStop = true;
            this.crosscorrelationRadioButton.Text = "Cross-correlation";
            this.crosscorrelationRadioButton.UseVisualStyleBackColor = true;
            this.crosscorrelationRadioButton.CheckedChanged += new System.EventHandler(this.crosscorrelationButton_CheckedChanged);
            // 
            // minimumDistanceRadioButton
            // 
            this.minimumDistanceRadioButton.Location = new System.Drawing.Point(495, 113);
            this.minimumDistanceRadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.minimumDistanceRadioButton.Name = "minimumDistanceRadioButton";
            this.minimumDistanceRadioButton.Size = new System.Drawing.Size(159, 20);
            this.minimumDistanceRadioButton.TabIndex = 26;
            this.minimumDistanceRadioButton.Text = "Minimum distance";
            this.minimumDistanceRadioButton.UseVisualStyleBackColor = true;
            this.minimumDistanceRadioButton.CheckedChanged += new System.EventHandler(this.minimumSubtractionRadioButton_CheckedChanged);
            // 
            // dynamicTimeWrapingRadioButton
            // 
            this.dynamicTimeWrapingRadioButton.Location = new System.Drawing.Point(495, 137);
            this.dynamicTimeWrapingRadioButton.Margin = new System.Windows.Forms.Padding(2);
            this.dynamicTimeWrapingRadioButton.Name = "dynamicTimeWrapingRadioButton";
            this.dynamicTimeWrapingRadioButton.Size = new System.Drawing.Size(159, 20);
            this.dynamicTimeWrapingRadioButton.TabIndex = 27;
            this.dynamicTimeWrapingRadioButton.Text = "Dynamic time wraping";
            this.dynamicTimeWrapingRadioButton.UseVisualStyleBackColor = true;
            this.dynamicTimeWrapingRadioButton.CheckedChanged += new System.EventHandler(this.dynamicTimeWrapingRadioButton_CheckedChanged);
            // 
            // pathAccumulatedDistanceChart
            // 
            this.pathAccumulatedDistanceChart.Location = new System.Drawing.Point(10, 518);
            this.pathAccumulatedDistanceChart.Margin = new System.Windows.Forms.Padding(2);
            this.pathAccumulatedDistanceChart.Name = "pathAccumulatedDistanceChart";
            this.pathAccumulatedDistanceChart.Size = new System.Drawing.Size(444, 194);
            this.pathAccumulatedDistanceChart.TabIndex = 28;
            // 
            // pathChart
            // 
            this.pathChart.Location = new System.Drawing.Point(695, 518);
            this.pathChart.Margin = new System.Windows.Forms.Padding(2);
            this.pathChart.Name = "pathChart";
            this.pathChart.Size = new System.Drawing.Size(444, 194);
            this.pathChart.TabIndex = 29;
            // 
            // secondSignalPowerLabel
            // 
            this.secondSignalPowerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.secondSignalPowerLabel.AutoSize = true;
            this.secondSignalPowerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.secondSignalPowerLabel.Location = new System.Drawing.Point(1048, 237);
            this.secondSignalPowerLabel.Margin = new System.Windows.Forms.Padding(2);
            this.secondSignalPowerLabel.Name = "secondSignalPowerLabel";
            this.secondSignalPowerLabel.Size = new System.Drawing.Size(84, 13);
            this.secondSignalPowerLabel.TabIndex = 31;
            this.secondSignalPowerLabel.Text = "Signal power:";
            // 
            // secondSignalPowerValueLabel
            // 
            this.secondSignalPowerValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.secondSignalPowerValueLabel.AutoSize = true;
            this.secondSignalPowerValueLabel.Location = new System.Drawing.Point(1048, 257);
            this.secondSignalPowerValueLabel.Margin = new System.Windows.Forms.Padding(2);
            this.secondSignalPowerValueLabel.Name = "secondSignalPowerValueLabel";
            this.secondSignalPowerValueLabel.Size = new System.Drawing.Size(13, 15);
            this.secondSignalPowerValueLabel.TabIndex = 30;
            this.secondSignalPowerValueLabel.Text = "0";
            // 
            // firstSignalPowerLabel
            // 
            this.firstSignalPowerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.firstSignalPowerLabel.AutoSize = true;
            this.firstSignalPowerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.firstSignalPowerLabel.Location = new System.Drawing.Point(8, 237);
            this.firstSignalPowerLabel.Margin = new System.Windows.Forms.Padding(2);
            this.firstSignalPowerLabel.Name = "firstSignalPowerLabel";
            this.firstSignalPowerLabel.Size = new System.Drawing.Size(84, 13);
            this.firstSignalPowerLabel.TabIndex = 33;
            this.firstSignalPowerLabel.Text = "Signal power:";
            // 
            // firstSignalPowerValueLabel
            // 
            this.firstSignalPowerValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.firstSignalPowerValueLabel.AutoSize = true;
            this.firstSignalPowerValueLabel.Location = new System.Drawing.Point(8, 257);
            this.firstSignalPowerValueLabel.Margin = new System.Windows.Forms.Padding(2);
            this.firstSignalPowerValueLabel.Name = "firstSignalPowerValueLabel";
            this.firstSignalPowerValueLabel.Size = new System.Drawing.Size(13, 15);
            this.firstSignalPowerValueLabel.TabIndex = 32;
            this.firstSignalPowerValueLabel.Text = "0";
            // 
            // comparisonSignalPowerLabel
            // 
            this.comparisonSignalPowerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comparisonSignalPowerLabel.AutoSize = true;
            this.comparisonSignalPowerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.comparisonSignalPowerLabel.Location = new System.Drawing.Point(804, 449);
            this.comparisonSignalPowerLabel.Margin = new System.Windows.Forms.Padding(2);
            this.comparisonSignalPowerLabel.Name = "comparisonSignalPowerLabel";
            this.comparisonSignalPowerLabel.Size = new System.Drawing.Size(84, 13);
            this.comparisonSignalPowerLabel.TabIndex = 35;
            this.comparisonSignalPowerLabel.Text = "Signal power:";
            // 
            // comparisonSignalPowerValueLabel
            // 
            this.comparisonSignalPowerValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comparisonSignalPowerValueLabel.AutoSize = true;
            this.comparisonSignalPowerValueLabel.Location = new System.Drawing.Point(804, 471);
            this.comparisonSignalPowerValueLabel.Margin = new System.Windows.Forms.Padding(2);
            this.comparisonSignalPowerValueLabel.Name = "comparisonSignalPowerValueLabel";
            this.comparisonSignalPowerValueLabel.Size = new System.Drawing.Size(13, 15);
            this.comparisonSignalPowerValueLabel.TabIndex = 34;
            this.comparisonSignalPowerValueLabel.Text = "0";
            // 
            // FormSignalsComparison
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1153, 511);
            this.Controls.Add(this.comparisonSignalPowerLabel);
            this.Controls.Add(this.comparisonSignalPowerValueLabel);
            this.Controls.Add(this.firstSignalPowerLabel);
            this.Controls.Add(this.firstSignalPowerValueLabel);
            this.Controls.Add(this.secondSignalPowerLabel);
            this.Controls.Add(this.secondSignalPowerValueLabel);
            this.Controls.Add(this.pathChart);
            this.Controls.Add(this.pathAccumulatedDistanceChart);
            this.Controls.Add(this.dynamicTimeWrapingRadioButton);
            this.Controls.Add(this.minimumDistanceRadioButton);
            this.Controls.Add(this.crosscorrelationRadioButton);
            this.Controls.Add(this.selectSecondSignalCheckBox);
            this.Controls.Add(this.selectFirstSignalCheckBox);
            this.Controls.Add(this.comparisonChart);
            this.Controls.Add(this.secondSignalChart);
            this.Controls.Add(this.firstSignalChart);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximumSize = new System.Drawing.Size(1169, 550);
            this.MinimumSize = new System.Drawing.Size(1169, 550);
            this.Name = "FormSignalsComparison";
            this.Text = "Signals Compariator";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ScottPlot.FormsPlot firstSignalChart;
        private ScottPlot.FormsPlot secondSignalChart;
        private ScottPlot.FormsPlot comparisonChart;
        public System.Windows.Forms.CheckBox selectFirstSignalCheckBox;
        public System.Windows.Forms.CheckBox selectSecondSignalCheckBox;
        private System.Windows.Forms.RadioButton crosscorrelationRadioButton;
        private System.Windows.Forms.RadioButton minimumDistanceRadioButton;
        private System.Windows.Forms.RadioButton dynamicTimeWrapingRadioButton;
        private ScottPlot.FormsPlot pathAccumulatedDistanceChart;
        private ScottPlot.FormsPlot pathChart;
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
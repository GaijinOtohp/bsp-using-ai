using BSP_Using_AI.DetailsModify.SignalFusion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.DetailsModify
{
    public partial class FormDetailsModify
    {
        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void saveButton_Click(object sender, EventArgs e)
        {
            // Open file dialogue to choose the path where to save the image
            using (SaveFileDialog sfd = new SaveFileDialog() { Title = "Save an Image File", ValidateNames = true, Filter = "PNG Image|*.png", RestoreDirectory = true })
            {
                // Check if the user clicked OK button
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // If yes then save teh chart in the selected path

                    // Get the path of specified file
                    String filePath = sfd.FileName;

                    saveChartAsImage(filePath);
                }
            }
        }

        private void saveChartAsImage(string filePath)
        {
            // Scale the size of the chart
            int scaling = 20;
            System.IO.MemoryStream myStream = new System.IO.MemoryStream();
            Chart chart2 = new Chart();
            signalChart.Serializer.Save(myStream);
            chart2.Serializer.Load(myStream);
            chart2.Height = chart2.Height * scaling;
            chart2.Width = chart2.Width * scaling;
            foreach (Series serie in chart2.Series)
            {
                serie.BorderWidth = serie.BorderWidth * scaling;
                serie.MarkerSize = serie.MarkerSize * 15;
                serie.Font = new System.Drawing.Font("Microsoft Sans Serif", serie.Font.Size * 15);
                serie.SmartLabelStyle.CalloutLineWidth = serie.SmartLabelStyle.CalloutLineWidth * 10;
            }
            chart2.ChartAreas[0].AxisX.MajorGrid.LineWidth = chart2.ChartAreas[0].AxisX.MajorGrid.LineWidth * scaling;
            chart2.ChartAreas[0].AxisY.MajorGrid.LineWidth = chart2.ChartAreas[0].AxisY.MajorGrid.LineWidth * scaling;
            chart2.ChartAreas[0].AxisX.MajorTickMark.LineWidth = chart2.ChartAreas[0].AxisX.MajorTickMark.LineWidth * scaling;
            chart2.ChartAreas[0].AxisY.MajorTickMark.LineWidth = chart2.ChartAreas[0].AxisY.MajorTickMark.LineWidth * scaling;
            chart2.ChartAreas[0].AxisX.LineWidth = chart2.ChartAreas[0].AxisX.LineWidth * scaling;
            chart2.ChartAreas[0].AxisY.LineWidth = chart2.ChartAreas[0].AxisY.LineWidth * scaling;
            chart2.ChartAreas[0].AxisX.LabelAutoFitMinFontSize = chart2.ChartAreas[0].AxisX.LabelAutoFitMinFontSize * scaling;
            chart2.ChartAreas[0].AxisY.LabelAutoFitMinFontSize = chart2.ChartAreas[0].AxisY.LabelAutoFitMinFontSize * scaling;
            chart2.ChartAreas[0].AxisX.LabelAutoFitMaxFontSize = chart2.ChartAreas[0].AxisX.LabelAutoFitMaxFontSize * scaling;
            chart2.ChartAreas[0].AxisY.LabelAutoFitMaxFontSize = chart2.ChartAreas[0].AxisY.LabelAutoFitMaxFontSize * scaling;
            chart2.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", chart2.ChartAreas[0].AxisX.TitleFont.Size * scaling);
            chart2.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", chart2.ChartAreas[0].AxisY.TitleFont.Size * scaling);
            chart2.Legends[0].Font = new System.Drawing.Font("Microsoft Sans Serif", chart2.Legends[0].Font.Size * scaling);
            if (chart2.Titles.Count > 0)
                chart2.Titles[0].Font = new System.Drawing.Font("Microsoft Sans Serif", chart2.Titles[0].Font.Size * scaling);

            // Save the image from the scaled chart
            chart2.SaveImage(filePath, ChartImageFormat.Png);
        }

        private void signalExhibitor_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;
            _previousMouseX = e.X;
            _previousMouseY = e.Y;
        }

        private void signalExhibitor_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown)
            {
                EventHandlers.signalExhibitor_MouseMove(sender, e, _previousMouseX, _previousMouseY);
                _previousMouseX = e.X;
                _previousMouseY = e.Y;
            }
            // Send event to ART-HT AI tools
            signalExhibitor_MouseMove_ARTHT(sender, e);
        }

        private void signalExhibitor_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }

        private void signalExhibitor_MouseWheel(object sender, MouseEventArgs e)
        {
            EventHandlers.signalExhibitor_MouseWheel(sender, e, _previousMouseX, _previousMouseY);
        }

        private void samplingRateTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.textBoxNumberOnly(sender, e);
        }

        private void autoApplyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_FilteringTools != null)
                if (!_FilteringTools._ignoreEvent)
                {
                    _FilteringTools._ignoreEvent = true;
                    _FilteringTools.SetAutoApply(autoApplyCheckBox.Checked);
                    _FilteringTools._ignoreEvent = false;
                }
        }

        public void applyButton_Click(object sender, EventArgs e)
        {
            _FilteringTools.ApplyFilters(true);
        }

        private void samplingRateTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_FilteringTools != null)
                if (!_FilteringTools._ignoreEvent)
                {
                    int samplingRate = 1;
                    if (samplingRateTextBox.Text.Length > 0 && !samplingRateTextBox.Text.Equals("."))
                        samplingRate = int.Parse(samplingRateTextBox.Text);
                    _FilteringTools._ignoreEvent = true;
                    _FilteringTools.SetOriginalSamplingRate(samplingRate);
                    _FilteringTools._ignoreEvent = false;
                }
        }

        private void quantizationStepTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_FilteringTools != null)
                if (!_FilteringTools._ignoreEvent)
                {
                    int quantizationStep = 1;
                    if (quantizationStepTextBox.Text.Length > 0 && !quantizationStepTextBox.Text.Equals("."))
                        quantizationStep = int.Parse(quantizationStepTextBox.Text);
                    _FilteringTools._ignoreEvent = true;
                    _FilteringTools.SetQuantizationStep(quantizationStep);
                    _FilteringTools._ignoreEvent = false;
                }
        }

        private void filtersComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (filtersComboBox.SelectedIndex == -1)
                return;
            FilterBase filter = null;
            // Check which filter is selected
            switch (filtersComboBox.SelectedItem)
            {
                case "DC removal":
                    filter = new DCRemoval(_FilteringTools);
                    break;
                case "Normalize signal":
                    filter = new Normalize(_FilteringTools);
                    break;
                case "Absolute signal":
                    filter = new Absolute(_FilteringTools);
                    break;
                case "DWT":
                    filter = new DWT(_FilteringTools);
                    break;
                case "Peaks analyzer":
                    filter = new PeaksAnalyzer(_FilteringTools);
                    break;
                default:
                    filter = new IIRFilter(_FilteringTools, (string)filtersComboBox.SelectedItem);
                    break;
            }
            // Insert the filter in and the filters flow layout panel
            filter.InsertFilter(filtersFlowLayoutPanel);
            // Set the combobox selection to nothing
            filtersComboBox.SelectedIndex = -1;
        }

        private void signalsPickerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check which signal is selected
            if (signalsPickerComboBox.SelectedIndex == 0)
                // Original signal is selected
                loadSignal(_FilteringTools._RawSamples, _FilteringTools._samplingRate, _FilteringTools._startingInSec);
            else if (signalsPickerComboBox.SelectedIndex == 1)
                // Filtered signal is selected
                loadSignal(_FilteringTools._FilteredSamples, _FilteringTools._samplingRate, _FilteringTools._startingInSec);
        }

        private void signalFusionButton_Click(object sender, EventArgs e)
        {
            // Check if the form is already opened, and close it if so
            if (Application.OpenForms.OfType<FormSignalFusion>().Count() == 1)
                Application.OpenForms.OfType<FormSignalFusion>().First().Close();

            if (_FilteringTools._FilteredSamples == null)
                return;

            // Get samples from signal chart
            double[] samples = (double[])_FilteringTools._FilteredSamples.Clone();

            // Get magor frequency from _samples after removing dc value
            double[] fftMag = applyFFT(Garage.removeDCValue(_FilteringTools._FilteredSamples));
            double mainFreuency = 0;
            double mainFreuencyMag = fftMag[0];
            for (int i = 1; i < fftMag.Length; i++)
                if (fftMag[i] > mainFreuencyMag)
                {
                    mainFreuencyMag = fftMag[i];
                    mainFreuency = i / ((fftMag.Length * 2) / _FilteringTools._samplingRate);
                }

            // Create the new form
            FormSignalFusion formSignalFusion = new FormSignalFusion(_FilteringTools.Clone(), mainFreuency, pathLabel.Text);
            formSignalFusion.Show();
        }

        private void sendSignalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Chart senderChart = (Chart)((sender as ToolStripItem).Owner as ContextMenuStrip).SourceControl;
            if (senderChart.Series[0].Points.Count < 1)
                return;

            // Clone _FilteringTools
            FilteringTools filteringTools = _FilteringTools.Clone();
            // Remove filters
            filteringTools.RemoveAllFilters();
            // Get samples from signal chart
            double[] samples = new double[senderChart.Series[0].Points.Count];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = senderChart.Series[0].Points[i].YValues[0];
            filteringTools.SetOriginalSamples(samples);

            // Check if the sender is spectrumChart
            if (senderChart.Name.Equals("spectrumChart"))
                // If yes then set the sampling rate as hertz sampling rate
                //EventHandlers.sendSignalTool(samples, (samples.Length * 2) / _FilteringTools._samplingRate, 1, pathLabel.Text + "\\Collector");
                filteringTools.SetOriginalSamplingRate(filteringTools._FilteredSamples.Length * 2 / filteringTools._samplingRate);
            //else
                //EventHandlers.sendSignalTool(samples, _FilteringTools._samplingRate, 1, pathLabel.Text + "\\Collector");
            EventHandlers.sendSignalTool(filteringTools, pathLabel.Text + "\\Collector");
        }

        private void analyseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Chart senderChart = (Chart)((sender as ToolStripItem).Owner as ContextMenuStrip).SourceControl;
            if (senderChart.Series[0].Points.Count < 1)
                return;

            // Clone _FilteringTools
            FilteringTools filteringTools = _FilteringTools.Clone();
            // Remove filters
            filteringTools.RemoveAllFilters();
            // Get samples from signal chart
            double[] samples = new double[senderChart.Series[0].Points.Count];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = senderChart.Series[0].Points[i].YValues[0] * filteringTools._quantizationStep;
            filteringTools.SetOriginalSamples(samples);

            // Check if the sender is spectrumChart
            if (senderChart.Name.Equals("spectrumChart"))
                // If yes then set the sampling rate as hertz sampling rate
                // EventHandlers.analyseSignalTool(samples, (samples.Length * 2) / _FilteringTools._samplingRate, 1, pathLabel.Text + "\\Analyser");
                filteringTools.SetOriginalSamplingRate(filteringTools._FilteredSamples.Length * 2 / filteringTools._samplingRate);
            //else
                // EventHandlers.analyseSignalTool(samples, _FilteringTools._samplingRate, 1, pathLabel.Text + "\\Analyser");
            EventHandlers.analyseSignalTool(filteringTools, pathLabel.Text + "\\Analyser");
        }
    }
}

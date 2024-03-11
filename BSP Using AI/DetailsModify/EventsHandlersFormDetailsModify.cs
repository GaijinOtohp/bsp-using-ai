using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.DetailsModify.SignalFusion;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Linq;
using System.Windows.Forms;

namespace BSP_Using_AI.DetailsModify
{
    public partial class FormDetailsModify
    {
        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void saveAsImageButton_Click(object sender, EventArgs e)
        {
            GeneralTools.saveChartAsImage(signalChart);
        }

        private void FormDetailsModify_KeyDown(object sender, KeyEventArgs e)
        {
            // Send event to Annotations tools
            FormDetailsModify_KeyDown_Anno(sender, e);
        }

        private void FormDetailsModify_KeyUp(object sender, KeyEventArgs e)
        {
            // Send event to Annotations tools
            FormDetailsModify_KeyUp_Anno(sender, e);
        }

        private void signalChart_MouseUp(object sender, MouseEventArgs e)
        {
            // Send event to Annotations tools
            signalChart_MouseUp_Anno(sender, e);
        }

        private void signalExhibitor_MouseMove(object sender, MouseEventArgs e)
        {
            // Send event to Annotations tools
            signalExhibitor_MouseMove_Anno(sender, e);
        }

        private void signalChart_MouseClick(object sender, MouseEventArgs e)
        {
            // Send event to Annotations tools
            signalChart_MouseClick_Anno(sender, e);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            // Send event to Annotations tools
            nextButton_Click_Anno(sender, e);
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            // Send event to Annotations tools
            previousButton_Click_Anno(sender, e);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Send event to Annotations tools
            saveButton_Click_Anno(sender, e);
        }

        private void predictButton_Click(object sender, EventArgs e)
        {
            // Send event to Annotations tools
            predictButton_Click_Anno(sender, e);
        }

        private void setFeaturesLabelsButton_Click(object sender, EventArgs e)
        {
            // Send event to Annotations tools
            setFeaturesLabelsButton_Click_Anno(sender, e);
        }

        private void discardButton_Click(object sender, EventArgs e)
        {
            // Send event to Annotations tools
            discardButton_Click_Anno(sender, e);
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
            double[] fftMag = applyFFT(GeneralTools.removeDCValue(_FilteringTools._FilteredSamples));
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
            FormsPlot senderChart = (FormsPlot)((sender as ToolStripItem).Owner as ContextMenuStrip).SourceControl;
            IPlottable[] plottable = senderChart.Plot.GetPlottables();
            if (plottable.Length == 0)
                return;

            if (plottable[0] is SignalPlot signalPlot)
            {
                if (signalPlot.PointCount < 1)
                    return;

                // Clone _FilteringTools
                FilteringTools filteringTools = _FilteringTools.Clone();
                // Remove filters
                filteringTools.RemoveAllFilters();
                // Get samples from signal chart
                double[] samples = new double[signalPlot.PointCount];
                for (int i = 0; i < samples.Length; i++)
                    samples[i] = signalPlot.Ys[i];
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
        }

        private void analyseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormsPlot senderChart = (FormsPlot)((sender as ToolStripItem).Owner as ContextMenuStrip).SourceControl;
            IPlottable[] plottable = senderChart.Plot.GetPlottables();
            if (plottable.Length == 0)
                return;

            if (plottable[0] is SignalPlot signalPlot)
            {
                if (signalPlot.PointCount < 1)
                    return;

                // Clone _FilteringTools
                FilteringTools filteringTools = _FilteringTools.Clone();
                // Remove filters
                filteringTools.RemoveAllFilters();
                // Get samples from signal chart
                double[] samples = new double[signalPlot.PointCount];
                for (int i = 0; i < samples.Length; i++)
                    samples[i] = signalPlot.Ys[i] * filteringTools._quantizationStep;
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
}

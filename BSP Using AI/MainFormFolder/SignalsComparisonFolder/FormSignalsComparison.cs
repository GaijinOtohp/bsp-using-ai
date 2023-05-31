using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace BSP_Using_AI.MainFormFolder.SignalsComparisonFolder
{
    public partial class FormSignalsComparison : Form
    {
        FilteringTools _Signal_1_FilteringTools { get; set; }
        FilteringTools _Signal_2_FilteringTools { get; set; }

        int _comparisonSamplingRate;

        public FormSignalsComparison()
        {
            InitializeComponent();

            // Set plots titles and labels
            firstSignalChart.Plot.Title("First signal");
            firstSignalChart.Refresh();
            secondSignalChart.Plot.Title("Second signal");
            secondSignalChart.Refresh();
            comparisonChart.Plot.Title("Cross-correlation");
            comparisonChart.Plot.XAxis.Label("Time (s)");
            comparisonChart.Plot.YAxis.Label("Voltage (mV)");
            comparisonChart.Refresh();
            pathAccumulatedDistanceChart.Plot.XAxis.Label("Time (s)");
            pathAccumulatedDistanceChart.Plot.YAxis.Label("Voltage (mV)");
            pathAccumulatedDistanceChart.Refresh();
            pathChart.Plot.XAxis.Label("Sig1 Time (s)");
            pathChart.Plot.YAxis.Label("Sig2 Time (s)");
            pathChart.Refresh();
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public void insertSignal(FilteringTools filteringTools)
        {
            // Compute signal power
            double signalPower = 0D;
            foreach (double sample in filteringTools._FilteredSamples)
                signalPower += Math.Pow(sample, 2) / filteringTools._FilteredSamples.Length;

            // Check if the first signal chart is open
            if (selectFirstSignalCheckBox.Checked)
            {
                // If yes then copy this signal in firstSignalChart
                _Signal_1_FilteringTools = filteringTools;
                // Insert signal values inside signal holder chart
                Garage.loadSignalInChart(firstSignalChart, filteringTools._FilteredSamples, filteringTools._samplingRate, 0, "FormSignalsComparison");
                // Set signal power
                firstSignalPowerValueLabel.Text = Math.Round(signalPower, 5).ToString();
                // Uncheck the box
                selectFirstSignalCheckBox.Checked = false;
            }
            // Check if the second signal chart is open
            if (selectSecondSignalCheckBox.Checked)
            {
                // If yes then copy this signal in secondSignalChart
                _Signal_2_FilteringTools = filteringTools;
                // Insert signal values inside signal holder chart
                Garage.loadSignalInChart(secondSignalChart, filteringTools._FilteredSamples, filteringTools._samplingRate, 0, "FormSignalsComparison");
                // Set signal power
                secondSignalPowerValueLabel.Text = Math.Round(signalPower, 5).ToString();
                // Uncheck the box
                selectSecondSignalCheckBox.Checked = false;
            }

            compareSignals();
        }

        private void compareSignals()
        {
            // Check if signals are available
            if (_Signal_1_FilteringTools == null || _Signal_2_FilteringTools == null)
                return;

            // Copy values
            double[] sig1 = (double[])_Signal_1_FilteringTools._FilteredSamples.Clone();
            double[] sig2 = (double[])_Signal_2_FilteringTools._FilteredSamples.Clone();
            // Upsample the signal with the low sampling rate
            _comparisonSamplingRate = Math.Max(_Signal_1_FilteringTools._samplingRate, _Signal_2_FilteringTools._samplingRate);
            if (_Signal_1_FilteringTools._samplingRate != _Signal_2_FilteringTools._samplingRate)
                if (_Signal_1_FilteringTools._samplingRate < _Signal_2_FilteringTools._samplingRate)
                    sig1 = Garage.UpDownSampling(sig1, _Signal_1_FilteringTools._samplingRate, _Signal_2_FilteringTools._samplingRate);
                else
                    sig2 = Garage.UpDownSampling(sig2, _Signal_2_FilteringTools._samplingRate, _Signal_1_FilteringTools._samplingRate);

            // Chekc which comparison is selected
            if (crosscorrelationRadioButton.Checked)
                // If yes then perform crosscorelation comparison
                Garage.loadSignalInChart(comparisonChart, Garage.crossCorrelation(sig1, sig2), _comparisonSamplingRate, 0, "FormSignalsComparison");
            else if (minimumDistanceRadioButton.Checked)
                // If yes then perform minimum subtraction comparison
                Garage.loadSignalInChart(comparisonChart, Garage.minimumDistance(sig1, sig2), _comparisonSamplingRate, 0, "FormSignalsComparison");
            else if (dynamicTimeWrapingRadioButton.Checked)
            {
                // If yes then perform dynamic time wraping comparison
                (double distance, double[,] dtw, (int sig1Indx, int sig2Indx)[] path, double[] pathDistance) = Garage.dynamicTimeWrapingDistancePath(sig1, sig2, 30);

                // Create the coordination of the path
                double[] pathX = path.Select(item => (double)item.sig1Indx / _comparisonSamplingRate).ToArray();
                double[] pathY = path.Select(item => (double)item.sig2Indx / _comparisonSamplingRate).ToArray();

                // Create a signal for the accumulated cost of the optimal path
                double[] pathAccumulatedCost = new double[pathX.Length];
                for (int i = 0; i < pathAccumulatedCost.Length; i++)
                    pathAccumulatedCost[i] = dtw[path[i].sig1Indx, path[i].sig2Indx];

                // Set the signals in their charts
                Garage.loadSignalInChart(comparisonChart, pathDistance, _comparisonSamplingRate, 0, "FormSignalsComparison");
                Garage.loadSignalInChart(pathAccumulatedDistanceChart, pathAccumulatedCost, _comparisonSamplingRate, 0, "FormSignalsComparison");
                Garage.loadSignalInChart(pathChart, pathX, pathY, 0d, "FormSignalsComparison");
            }

            // Set signal power

            // Get samples from signal chart
            IPlottable[] plottable = comparisonChart.Plot.GetPlottables();
            if (plottable.Length == 0)
                return;

            if (plottable[0] is SignalPlot comparisonSignalPlot)
            {
                if (comparisonSignalPlot.PointCount < 1)
                    return;

                double[] samples = new double[comparisonSignalPlot.PointCount];
                for (int i = 0; i < samples.Length; i++)
                    samples[i] = comparisonSignalPlot.Ys[i];

                double signalPower = 0D;
                foreach (double sample in samples)
                    signalPower += Math.Pow(sample, 2) / samples.Length;
                comparisonSignalPowerValueLabel.Text = Math.Round(signalPower, 5).ToString();
            }            
        }

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void crosscorrelationButton_CheckedChanged(object sender, EventArgs e)
        {
            if (crosscorrelationRadioButton.Checked)
            {
                // Set the title of the comparison
                comparisonChart.Plot.Title("Cross-correlation");
                comparisonChart.Refresh();

                // Change the form vertival size
                this.MinimumSize = new Size(1169, comparisonSignalPowerValueLabel.Location.Y + 60);
                this.MaximumSize = new Size(1169, comparisonSignalPowerValueLabel.Location.Y + 60);

                compareSignals();
            }
        }

        private void minimumSubtractionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (minimumDistanceRadioButton.Checked)
            {
                // Set the title of the comparison
                comparisonChart.Plot.Title("Cross-correlation minimum distance");
                comparisonChart.Refresh();

                // Change the form vertival size
                this.MinimumSize = new Size(1169, comparisonSignalPowerValueLabel.Location.Y + 60);
                this.MaximumSize = new Size(1169, comparisonSignalPowerValueLabel.Location.Y + 60);

                compareSignals();
            }
        }

        private void dynamicTimeWrapingRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (dynamicTimeWrapingRadioButton.Checked)
            {
                // Set the title of the comparison
                comparisonChart.Plot.Title("Distance of the optimal path");
                pathAccumulatedDistanceChart.Plot.Title("Accumulated distance of the optimal path");
                pathChart.Plot.Title("Optimal path");
                comparisonChart.Refresh();
                pathAccumulatedDistanceChart.Refresh();
                pathChart.Refresh();

                // Change the form vertival size
                this.MinimumSize = new Size(1169, pathChart.Location.Y + pathChart.Size.Height + 36);
                this.MaximumSize = new Size(1169, pathChart.Location.Y + pathChart.Size.Height + 36);

                compareSignals();
            }
        }

        private void sendSignalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IPlottable[] plottable = comparisonChart.Plot.GetPlottables();
            if (plottable.Length == 0)
                return;

            if (plottable[0] is SignalPlot comparisonSignalPlot)
            {
                if (comparisonSignalPlot.PointCount < 1)
                    return;

                // Get samples from signal chart
                double[] samples = new double[comparisonSignalPlot.PointCount];
                for (int i = 0; i < samples.Length; i++)
                    samples[i] = comparisonSignalPlot.Ys[i];
                // Create new filteringTools for the signal
                double meanQuanStep = (_Signal_1_FilteringTools._quantizationStep + _Signal_2_FilteringTools._quantizationStep) / 2d;
                FilteringTools filteringTools = new FilteringTools(_comparisonSamplingRate, meanQuanStep, null);
                filteringTools.SetStartingInSecond((_Signal_1_FilteringTools._startingInSec + _Signal_2_FilteringTools._startingInSec) / 2);
                filteringTools.SetOriginalSamples(samples);

                EventHandlers.sendSignalTool(filteringTools, "\\Comparator\\Collector");
            }
        }

        private void analyseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormsPlot senderChart = (FormsPlot)((sender as ToolStripItem).Owner as ContextMenuStrip).SourceControl;
            IPlottable[] plottable = senderChart.Plot.GetPlottables();
            if (plottable.Length == 0)
                return;

            if (plottable[0] is SignalPlot comparisonSignalPlot)
            {
                if (comparisonSignalPlot.PointCount < 1)
                    return;

                // Get samples from signal chart
                double[] samples = new double[comparisonSignalPlot.PointCount];
                for (int i = 0; i < samples.Length; i++)
                    samples[i] = comparisonSignalPlot.Ys[i];
                // Create new filteringTools for the signal
                double meanQuanStep = (_Signal_1_FilteringTools._quantizationStep + _Signal_2_FilteringTools._quantizationStep) / 2d;
                FilteringTools filteringTools = new FilteringTools(_comparisonSamplingRate, meanQuanStep, null);
                filteringTools.SetStartingInSecond((_Signal_1_FilteringTools._startingInSec + _Signal_2_FilteringTools._startingInSec) / 2);
                filteringTools.SetOriginalSamples(samples);

                EventHandlers.analyseSignalTool(filteringTools, "\\Comparator\\Analyser");
            }
        }
    }
}

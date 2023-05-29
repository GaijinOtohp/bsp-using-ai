using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace BSP_Using_AI.MainFormFolder.SignalsComparisonFolder
{
    public partial class FormSignalsComparison : Form
    {
        FilteringTools _Signal_1_FilteringTools { get; set; }
        FilteringTools _Signal_2_FilteringTools { get; set; }

        public double[] _signal1 { get; set; }
        public double[] _signal2 { get; set; }
        public double[] _comparison;
        public double[] _dtwPath;
        public double _dtwDistance { get; set; }

        private double _sign1SamplingRate { get; set; }
        private double _sign2SamplingRate { get; set; }

        bool _mouseDown = false;
        int _previousMouseX;
        int _previousMouseY;

        public FormSignalsComparison()
        {
            InitializeComponent();
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public void insertSignal(FilteringTools filteringTools)
        {
            // Normalize the signal
            Normalize normalize = new Normalize(filteringTools);
            normalize.InsertFilter(null);

            // Compute signal power
            double signalPower = 0D;
            foreach (double sample in filteringTools._FilteredSamples)
                signalPower += Math.Pow(sample / filteringTools._quantizationStep, 2) / filteringTools._FilteredSamples.Length;

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
            int meanSampRate = (int)(_Signal_1_FilteringTools._samplingRate + _Signal_2_FilteringTools._samplingRate / 2d);
            double meanQuanStep = _Signal_1_FilteringTools._quantizationStep + _Signal_2_FilteringTools._quantizationStep / 2d;

            // Chekc which comparison is selected
            if (crosscorrelationRadioButton.Checked)
                // If yes then perform crosscorelation comparison
                Garage.loadSignalInChart(comparisonChart, Garage.crossCorrelation(sig1, sig2), meanSampRate, 0, "FormSignalsComparison");
            else if (minimumSubtractionRadioButton.Checked)
                // If yes then perform minimum subtraction comparison
                Garage.loadSignalInChart(comparisonChart, Garage.minimumSubtraction(sig1, sig2), meanSampRate, 0, "FormSignalsComparison");
            else if (dynamicTimeWrapingRadioButton.Checked)
            {
                // If yes then perform dynamic time wraping comparison
                object[] dtwDistancePath = Garage.dynamicTimeWrapingDistancePath(sig1, sig2, 10);

                // Create the coordination of the path
                double[] pathX = new double[((List<int[]>)dtwDistancePath[1]).Count];
                double[] pathY = new double[pathX.Length];
                for (int i = 0; i < pathX.Length; i++)
                {
                    pathX[i] = ((List<int[]>)dtwDistancePath[1])[i][0];
                    pathY[i] = ((List<int[]>)dtwDistancePath[1])[i][1];
                }

                // Get the path signal from the dwt matrix
                double[] pathSignal = new double[pathX.Length];
                double[,] dwtMatrix = Garage.dynamicTimeWraping(sig1, sig2, 10);
                for (int i = 0; i < pathSignal.Length; i++)
                {
                    pathSignal[i] = dwtMatrix[(int)pathX[i], (int)pathY[i]];
                    if (double.IsInfinity(pathSignal[i]))
                        pathSignal[i] = 0D;
                }

                // Create a signal for distance value
                double[] distanceValue = new double[pathX.Length];
                for (int i = 0; i < distanceValue.Length; i++)
                    distanceValue[i] = (double)dtwDistancePath[0];

                // Set the signals in their charts
                Garage.loadSignalInChart(comparisonChart, pathSignal, meanSampRate, 0, "FormSignalsComparison");
                Garage.loadSignalInChart(distanceValueChart, distanceValue, meanSampRate, 0, "FormSignalsComparison");
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
                    signalPower += Math.Pow(sample / meanQuanStep, 2) / samples.Length;
                comparisonSignalPowerValueLabel.Text = Math.Round(signalPower, 5).ToString();
            }            
        }

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void crosscorrelationButton_CheckedChanged(object sender, EventArgs e)
        {
            if (crosscorrelationRadioButton.Checked)
            {
                // Change the form vertival size
                this.MinimumSize = new Size(1169, comparisonSignalPowerValueLabel.Location.Y + 60);
                this.MaximumSize = new Size(1169, comparisonSignalPowerValueLabel.Location.Y + 60);

                compareSignals();
            }
        }

        private void minimumSubtractionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (minimumSubtractionRadioButton.Checked)
            {
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
                int meanSampRate = (int)((_Signal_1_FilteringTools._samplingRate + _Signal_2_FilteringTools._samplingRate) / 2d);
                double meanQuanStep = (_Signal_1_FilteringTools._quantizationStep + _Signal_2_FilteringTools._quantizationStep) / 2d;
                FilteringTools filteringTools = new FilteringTools(meanSampRate, meanQuanStep, null);
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
                int meanSampRate = (int)((_Signal_1_FilteringTools._samplingRate + _Signal_2_FilteringTools._samplingRate) / 2d);
                double meanQuanStep = (_Signal_1_FilteringTools._quantizationStep + _Signal_2_FilteringTools._quantizationStep) / 2d;
                FilteringTools filteringTools = new FilteringTools(meanSampRate, meanQuanStep, null);
                filteringTools.SetStartingInSecond((_Signal_1_FilteringTools._startingInSec + _Signal_2_FilteringTools._startingInSec) / 2);
                filteringTools.SetOriginalSamples(samples);

                EventHandlers.analyseSignalTool(filteringTools, "\\Comparator\\Analyser");
            }
        }
    }
}

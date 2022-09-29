using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace BSP_Using_AI.MainFormFolder.SignalsComparisonFolder
{
    public partial class FormSignalsComparison : Form
    {
        public double[] _signal1;
        public double[] _signal2;
        public double[] _comparison;
        public double[] _dtwPath;
        public double _dtwDistance;

        private double _sign1SamplingRate;
        private double _sign1QuantizationStep;
        private double _sign2SamplingRate;
        private double _sign2QuantizationStep;

        bool _mouseDown = false;
        int _previousMouseX;
        int _previousMouseY;

        public FormSignalsComparison()
        {
            InitializeComponent();
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public void insertSignal(double[] signal, double samplingRate, double quantizationStep)
        {
            // Calculate normalization coefficient
            double normalizationCoef = 0D;
            if (selectFirstSignalCheckBox.Checked || selectSecondSignalCheckBox.Checked)
            foreach (double sample in signal)
                normalizationCoef += sample * sample;
            normalizationCoef = Math.Sqrt(normalizationCoef);

            double signalPower = 0D;

            // Check if the first signal chart is open
            if (selectFirstSignalCheckBox.Checked)
            {
                // If yes then copy this signal in firstSignalChart
                _signal1 = new double[signal.Length];
                if(normalizationCoef > 0)
                    for (int i = 0; i < signal.Length; i++)
                        _signal1[i] = signal[i] / normalizationCoef;

                _sign1SamplingRate = samplingRate;
                _sign1QuantizationStep = quantizationStep;

                // Insert signal values inside signal holder chart
                Garage.loadSignalInChart((Chart)Controls.Find("firstSignalChart", false)[0], _signal1, samplingRate, quantizationStep, 0, "FormSignalsComparison");

                // Set signal power
                foreach (double sample in _signal1)
                    signalPower += Math.Abs(sample * sample / _signal1.Length);
                firstSignalPowerValueLabel.Text = Math.Round(signalPower, 5).ToString();

                // Uncheck the box
                selectFirstSignalCheckBox.Checked = false;
            }
            // Check if the second signal chart is open
            if (selectSecondSignalCheckBox.Checked)
            {
                // If yes then copy this signal in secondSignalChart
                _signal2 = new double[signal.Length];
                if (normalizationCoef > 0)
                    for (int i = 0; i < signal.Length; i++)
                        _signal2[i] = signal[i] / normalizationCoef;

                _sign2SamplingRate = samplingRate;
                _sign2QuantizationStep = quantizationStep;

                // Insert signal values inside signal holder chart
                Garage.loadSignalInChart((Chart)Controls.Find("secondSignalChart", false)[0], _signal2, samplingRate, quantizationStep, 0, "FormSignalsComparison");

                // Set signal power
                foreach (double sample in _signal2)
                    signalPower += Math.Abs(sample * sample / _signal2.Length);
                secondSignalPowerValueLabel.Text = Math.Round(signalPower, 5).ToString();

                // Uncheck the box
                selectSecondSignalCheckBox.Checked = false;
            }

            compareSignals();
        }

        private void compareSignals()
        {
            // Check if signals are available
            if (_signal1 != null && _signal2 != null)
            {
                // Chekc which comparison should be done
                if (crosscorrelationRadioButton.Checked)
                    // If yes then perform crosscorelation comparison
                    Garage.loadSignalInChart(comparisonChart, Garage.crossCorrelation(_signal1, _signal2), (_sign1SamplingRate + _sign2SamplingRate) / 2, (_sign1QuantizationStep + _sign2QuantizationStep) / 2, 0, "FormSignalsComparison");
                if (minimumSubtractionRadioButton.Checked)
                    // If yes then perform minimum subtraction comparison
                    Garage.loadSignalInChart(comparisonChart, Garage.minimumSubtraction(_signal1, _signal2), (_sign1SamplingRate + _sign2SamplingRate) / 2, (_sign1QuantizationStep + _sign2QuantizationStep) / 2, 0, "FormSignalsComparison");
                if (dynamicTimeWrapingRadioButton.Checked)
                {
                    // If yes then perform dynamic time wraping comparison
                    object[] dtwDistancePath = Garage.dynamicTimeWrapingDistancePath(_signal1, _signal2, 10);

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
                    double[,] dwtMatrix = Garage.dynamicTimeWraping(_signal1, _signal2, 10);
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
                    Garage.loadSignalInChart(comparisonChart, pathSignal, (_sign1SamplingRate + _sign2SamplingRate) / 2, (_sign1QuantizationStep + _sign2QuantizationStep) / 2, 0, "FormSignalsComparison");
                    Garage.loadSignalInChart(distanceValueChart, distanceValue, (_sign1SamplingRate + _sign2SamplingRate) / 2, (_sign1QuantizationStep + _sign2QuantizationStep) / 2, 0, "FormSignalsComparison");
                    Garage.loadXYInChart(pathChart, pathX, pathY, null, 0d, 0, "FormSignalsComparison");
                }

                // Set signal power

                // Get samples from signal chart
                double[] samples = new double[comparisonChart.Series[0].Points.Count];
                for (int i = 0; i < samples.Length; i++)
                    samples[i] = comparisonChart.Series[0].Points[i].YValues[0];

                double signalPower = 0D;
                foreach (double sample in samples)
                    signalPower += Math.Abs(sample * sample / samples.Length);
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
                this.MinimumSize = new Size(1004, 469);
                this.MaximumSize = new Size(1004, 469);

                compareSignals();
            }
        }

        private void minimumSubtractionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (minimumSubtractionRadioButton.Checked)
            {
                // Change the form vertival size
                this.MinimumSize = new Size(1004, 469);
                this.MaximumSize = new Size(1004, 469);

                compareSignals();
            }
        }

        private void dynamicTimeWrapingRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (dynamicTimeWrapingRadioButton.Checked)
            {
                // Change the form vertival size
                this.MinimumSize = new Size(1004, 672);
                this.MaximumSize = new Size(1004, 672);

                compareSignals();
            }
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
        }

        private void signalExhibitor_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }

        private void signalExhibitor_MouseWheel(object sender, MouseEventArgs e)
        {
            EventHandlers.signalExhibitor_MouseWheel(sender, e, _previousMouseX, _previousMouseY);
        }

        private void sendSignalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Chart senderChart = (Chart)((sender as ToolStripItem).Owner as ContextMenuStrip).SourceControl;
            if (senderChart.Series[0].Points.Count < 1)
                return;

            // Get samples from signal chart
            double[] samples = new double[senderChart.Series[0].Points.Count];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = senderChart.Series[0].Points[i].YValues[0];

            EventHandlers.sendSignalTool(samples, (_sign1SamplingRate + _sign2SamplingRate) / 2, (_sign1QuantizationStep + _sign2QuantizationStep) / 2, "\\Comparator\\Collector");
        }

        private void analyseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Chart senderChart = (Chart)((sender as ToolStripItem).Owner as ContextMenuStrip).SourceControl;
            if (senderChart.Series[0].Points.Count < 1)
                return;

            // Get samples from signal chart
            double[] samples = new double[senderChart.Series[0].Points.Count];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = senderChart.Series[0].Points[i].YValues[0];

            EventHandlers.analyseSignalTool(samples, (_sign1SamplingRate + _sign2SamplingRate) / 2, (_sign1QuantizationStep + _sign2QuantizationStep) / 2, "\\Comparator\\Analyser");
        }
    }
}

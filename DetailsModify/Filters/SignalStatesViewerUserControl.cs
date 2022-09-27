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

namespace BSP_Using_AI.DetailsModify.Filters
{
    public partial class SignalStatesViewerUserControl : UserControl
    {
        FormDetailsModify _formDetailsModify;

        public double _thresholdRatio = 0.02d;
        public double _tdtThresholdRatio = 0.1d;
        public int _horThreshold = 1;

        public SignalStatesViewerUserControl(FormDetailsModify formDetailsModify)
        {
            InitializeComponent();

            _formDetailsModify = formDetailsModify;

            // Set the hThresholdScrollBar Maximum and change its value to 1
            if (_formDetailsModify._samples != null)
            {
                hThresholdScrollBar.Maximum = _formDetailsModify._samples.Length + hThresholdScrollBar.LargeChange - 1;
                hThresholdScrollBar.Value = 1;
            }
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public List<int>[] showSignalStates()
        {
            ///------------------------------------------------------------------------------------------///
            ///--------------------------------------Set new values--------------------------------------///
            // Change the threshold value
            _thresholdRatio = (double)(1000 - amplitudeThresholdScrollBar.Value) / 1000d;

            // Set the new value in thresholdRatioLabel
            thresholdRatioLabel.Text = "Threshold ratio: " + Math.Round(_thresholdRatio, 3).ToString();

            // Change the horizontal threshold value
            _horThreshold = hThresholdScrollBar.Value;

            // Set the new value in thresholdRatioLabel
            hThresholdLabel.Text = "Hor Threshold: " + Math.Round((double)_horThreshold / _formDetailsModify._samplingRate, 3).ToString() + " sec";

            // Change the tangent deviation tolerance threshold value
            _tdtThresholdRatio = (double)tdtThresholdScrollBar.Value / 100000d;

            // Set the new value in thresholdRatioLabel
            tdtThresholdLabel.Text = "Acc Threshold: " + Math.Round(_tdtThresholdRatio * 100, 3).ToString() + "%";
            ///------------------------------------------------------------------------------------------///
            ///------------------------------------------------------------------------------------------///

            // Get signal from chart
            Chart signalChart = ((FormDetailsModify)FindForm()).signalChart;
            if (signalChart.Series[0].Points.Count < 1)
                return null;
            // Get samplingRate
            double samplingRate = ((FormDetailsModify)FindForm())._samplingRate;

            // Get samples from signal chart
            double[] samples = new double[signalChart.Series[0].Points.Count];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = signalChart.Series[0].Points[i].YValues[0];

            // Get y values interval
            double interval = Garage.amplitudeInterval(samples);

            // Get states of the signal
            List<object[]> states = Garage.scanPeaks(samples, interval, _thresholdRatio, _horThreshold, _tdtThresholdRatio, showDeviationCheckBox.Checked);

            // Set states in the chart
            List<int>[] tempStatesList = new List<int>[] { new List<int>(), new List<int>(), new List<int>(), new List<int>() }; // {list for up-peaks, down-peaks, stable, selection}
            int ups = 0;
            int downs = 0;
            int stables = 0;
            foreach (object[] state in states)
                if (((string)state[0]).Equals("up"))
                    ups += 1;
                else if (((string)state[0]).Equals("down"))
                    downs += 1;
                else if (((string)state[0]).Equals("stable"))
                    stables += 1;

            double[] xUps = new double[ups];
            double[] yUps = new double[ups];
            string[] labelsUps = new string[ups];
            double[] xDowns = new double[downs];
            double[] yDowns = new double[downs];
            string[] labelsDowns = new string[downs];
            double[] xStables = new double[stables];
            double[] yStables = new double[stables];
            string[] labelsStables = new string[stables];

            ups = 0;
            downs = 0;
            stables = 0;
            foreach (object[] state in states)
                if (((string)state[0]).Equals("up"))
                {
                    tempStatesList[0].Add((int)state[1]);
                    xUps[ups] = (int)state[1] / samplingRate;
                    yUps[ups] = samples[(int)state[1]];
                    labelsUps[ups] = Math.Round((double)state[2], 2).ToString();
                    ups += 1;
                }
                else if (((string)state[0]).Equals("down"))
                {
                    tempStatesList[1].Add((int)state[1]);
                    xDowns[downs] = (int)state[1] / samplingRate;
                    yDowns[downs] = samples[(int)state[1]];
                    labelsDowns[downs] = Math.Round((double)state[2], 2).ToString();
                    downs += 1;
                }
                else if (((string)state[0]).Equals("stable"))
                {
                    tempStatesList[2].Add((int)state[1]);
                    xStables[stables] = (int)state[1] / samplingRate;
                    yStables[stables] = samples[(int)state[1]];
                    labelsStables[stables] = Math.Round((double)state[2], 2).ToString();
                    stables += 1;
                }

            Garage.loadXYInChart(signalChart, xUps, yUps, labelsUps, _formDetailsModify._startingInSec, 1, "SignalStatesViewerUserControl");
            Garage.loadXYInChart(signalChart, xDowns, yDowns, labelsDowns, _formDetailsModify._startingInSec, 2, "SignalStatesViewerUserControl");
            Garage.loadXYInChart(signalChart, xStables, yStables, labelsStables, _formDetailsModify._startingInSec, 3, "SignalStatesViewerUserControl");

            return tempStatesList;
        }

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void amplitudeThresholdScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            // Check if show and auto apply is checked
            if (showStatesCheckBox.Checked && autoApplyCheckBox.Checked)
            {
                // If yes then show signal states
                showSignalStates();
            }
        }

        private void hThresholdScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (_formDetailsModify._samplingRate == 0)
                return;

            // Check if show and auto apply is checked
            if (showStatesCheckBox.Checked && autoApplyCheckBox.Checked)
            {
                // If yes then show signal states
                showSignalStates();
            }
        }

        private void accelerationThresholdScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            // Check if show and auto apply is checked
            if (showStatesCheckBox.Checked && autoApplyCheckBox.Checked)
            {
                // If yes then show signal states
                showSignalStates();
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            showSignalStates();
        }

        private void showCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Chart signalChart = ((FormDetailsModify)FindForm()).signalChart;
            // If checked then show states
            if (showStatesCheckBox.Checked)
                for (int i = 1; i < 4; i++)
                    signalChart.Series[i].Enabled = true;
            // If not then hide states
            else
                for (int i = 1; i < 6; i++)
                    signalChart.Series[i].Enabled = false;
        }

        private void showAccelerationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Chart signalChart = ((FormDetailsModify)FindForm()).signalChart;

            // If checked then show accelerations
            // If not then hide accelerations
            for (int i = 1; i < 4; i++)
            {
                signalChart.Series[i].LabelForeColor = showDeviationCheckBox.Checked ? Color.Black: Color.Transparent;
                //signalChart.Series[i].SmartLabelStyle.CalloutLineColor = showAccelerationCheckBox.Checked ? Color.Black : Color.Transparent;
            }

            showSignalStates();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EventHandlers.deleteToolStripMenuItem_Click(this);
        }
    }
}

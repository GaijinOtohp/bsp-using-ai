using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace BSP_Using_AI.DetailsModify.Filters
{
    public partial class SignalStatesViewerUserControl : UserControl
    {
        PeaksAnalyzer _peaksAnalyzer;

        public SignalStatesViewerUserControl(PeaksAnalyzer peaksAnalyzer)
        {
            InitializeComponent();

            _peaksAnalyzer = peaksAnalyzer;

            // Set the hThresholdScrollBar Maximum and change its value to 1
            if (_peaksAnalyzer._ParentFilteringTools._FilteredSamples.Length > 0)
                hThresholdScrollBar.SetMax(_peaksAnalyzer._ParentFilteringTools._FilteredSamples.Length);
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public Dictionary<string, List<State>> showSignalStates(Dictionary<string, List<State>> statesDIc)
        {
            Chart signalChart = ((FormDetailsModify)FindForm()).signalChart;
            double samplingRate = _peaksAnalyzer._ParentFilteringTools._samplingRate;

            double[] xUps = statesDIc[SANamings.UpPeaks].Select(x => x._index / samplingRate).ToArray();
            double[] yUps = statesDIc[SANamings.UpPeaks].Select(x => x._value).ToArray();
            string[] labelsUps = statesDIc[SANamings.UpPeaks].Select(x => Math.Round(x._deviantionAngle, 2).ToString()).ToArray();
            double[] xDowns = statesDIc[SANamings.DownPeaks].Select(x => x._index / samplingRate).ToArray();
            double[] yDowns = statesDIc[SANamings.DownPeaks].Select(x => x._value).ToArray();
            string[] labelsDowns = statesDIc[SANamings.DownPeaks].Select(x => Math.Round(x._deviantionAngle, 2).ToString()).ToArray();
            double[] xStables = statesDIc[SANamings.StableStates].Select(x => x._index / samplingRate).ToArray();
            double[] yStables = statesDIc[SANamings.StableStates].Select(x => x._value).ToArray();
            string[] labelsStables = statesDIc[SANamings.StableStates].Select(x => Math.Round(x._deviantionAngle, 2).ToString()).ToArray();

            Garage.loadXYInChart(signalChart, xUps, yUps, labelsUps, _peaksAnalyzer._ParentFilteringTools._startingInSec, 1, "SignalStatesViewerUserControl");
            Garage.loadXYInChart(signalChart, xDowns, yDowns, labelsDowns, _peaksAnalyzer._ParentFilteringTools._startingInSec, 2, "SignalStatesViewerUserControl");
            Garage.loadXYInChart(signalChart, xStables, yStables, labelsStables, _peaksAnalyzer._ParentFilteringTools._startingInSec, 3, "SignalStatesViewerUserControl");

            return statesDIc;
        }

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void artValueTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.textBoxNumberOnly(sender, e);
        }

        private void amplitudeThresholdScrollBar_ValueChanged(object sender, EventArgs e)
        {
            // Update artValueTextBox
            if (!_peaksAnalyzer._ignoreEvent)
            {
                _peaksAnalyzer._ignoreEvent = true;
                // Change the threshold value
                _peaksAnalyzer.SetART((amplitudeThresholdScrollBar.GetMax() - amplitudeThresholdScrollBar.Value) / (double)amplitudeThresholdScrollBar.GetMax());
                // Set the new value in artValueTextBox
                artValueTextBox.Text = Math.Round(_peaksAnalyzer._art, 3).ToString();
                _peaksAnalyzer._ignoreEvent = false;
            }
        }
        private void artValueTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!_peaksAnalyzer._ignoreEvent)
            {
                _peaksAnalyzer._ignoreEvent = true;
                // Get ART from the textbox
                double art = 0;
                if (artValueTextBox.Text.Length > 0)
                    art = double.Parse(artValueTextBox.Text);
                // Check if it crosses the max of amplitudeThresholdScrollBar
                if (amplitudeThresholdScrollBar.GetMax() - (art * amplitudeThresholdScrollBar.GetMax()) < 0)
                {
                    // If yes then set it as the max of amplitudeThresholdScrollBar
                    art = (amplitudeThresholdScrollBar.GetMax() - 0) / (double)amplitudeThresholdScrollBar.GetMax();
                    artValueTextBox.Text = Math.Round(art, 4).ToString();
                }
                // Change the threshold value
                _peaksAnalyzer.SetART(art);
                // Update the scrollbar
                amplitudeThresholdScrollBar.Value = (int)(amplitudeThresholdScrollBar.GetMax() - (art * amplitudeThresholdScrollBar.GetMax()));
                _peaksAnalyzer._ignoreEvent = false;
            }
        }

        private void hThresholdScrollBar_ValueChanged(object sender, EventArgs e)
        {
            if (_peaksAnalyzer._ParentFilteringTools._samplingRate == 0)
                return;

            // Update htValueTextBox
            if (!_peaksAnalyzer._ignoreEvent)
            {
                _peaksAnalyzer._ignoreEvent = true;
                // Change the horizontal threshold value
                _peaksAnalyzer.SetHT(hThresholdScrollBar.Value / (double)hThresholdScrollBar.GetMax());
                // Set the new value in htValueTextBox
                htValueTextBox.Text = Math.Round(_peaksAnalyzer._ht * (double)hThresholdScrollBar.GetMax() / (double)_peaksAnalyzer._ParentFilteringTools._samplingRate, 3).ToString();
                _peaksAnalyzer._ignoreEvent = false;
            }
        }
        private void htValueTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!_peaksAnalyzer._ignoreEvent)
            {
                _peaksAnalyzer._ignoreEvent = true;
                // Get HT from the textbox
                int ht = 0;
                if (htValueTextBox.Text.Length > 0)
                    ht = (int)(double.Parse(htValueTextBox.Text) * _peaksAnalyzer._ParentFilteringTools._samplingRate);
                // Check if it crosses the max of hThresholdScrollBar
                if (ht > hThresholdScrollBar.GetMax())
                {
                    // If yes then set it as the max of hThresholdScrollBar
                    ht = hThresholdScrollBar.GetMax();
                    htValueTextBox.Text = ht.ToString();
                }
                // Change the horizontal threshold value
                _peaksAnalyzer.SetHT(ht / (double)hThresholdScrollBar.GetMax());
                // Update the scrollbar
                hThresholdScrollBar.Value = ht;
                _peaksAnalyzer._ignoreEvent = false;
            }
        }

        private void tdtThresholdScrollBar_ValueChanged(object sender, EventArgs e)
        {
            // Update tdtValueTextBox
            if (!_peaksAnalyzer._ignoreEvent)
            {
                _peaksAnalyzer._ignoreEvent = true;
                // Change the tangent deviation tolerance threshold value
                _peaksAnalyzer.SetTDT(tdtThresholdScrollBar.Value / (double)tdtThresholdScrollBar.GetMax());
                // Set the new value in thresholdRatioLabel
                tdtValueTextBox.Text = Math.Round(_peaksAnalyzer._tdt * 100, 3).ToString();
                _peaksAnalyzer._ignoreEvent = false;
            }
        }
        private void tdtValueTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!_peaksAnalyzer._ignoreEvent)
            {
                _peaksAnalyzer._ignoreEvent = true;
                // Get TDT from the textbox
                double tdt = 0;
                if (tdtValueTextBox.Text.Length > 0)
                    tdt = double.Parse(tdtValueTextBox.Text) / 100d;
                // Check if it crosses the max of tdtThresholdScrollBar
                if (tdt > 1)
                {
                    // If yes then set it as the max of tdtThresholdScrollBar
                    tdt = 1;
                    tdtValueTextBox.Text = Math.Round(tdt * 100, 3).ToString();
                }
                // Change the tangent deviation tolerance threshold value
                _peaksAnalyzer.SetTDT(tdt);
                // Update the scrollbar
                tdtThresholdScrollBar.Value = (int)(tdt * tdtThresholdScrollBar.GetMax());
                _peaksAnalyzer._ignoreEvent = false;
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            double[] filteredSamples = _peaksAnalyzer._ParentFilteringTools._FilteredSamples;
            if (filteredSamples != null)
                _peaksAnalyzer.ScanPeaks(filteredSamples);
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
            // Update _peaksAnalyzer
            if (!_peaksAnalyzer._ignoreEvent)
            {
                _peaksAnalyzer._ignoreEvent = true;
                _peaksAnalyzer.ActivateGenerally(showStatesCheckBox.Checked);
                _peaksAnalyzer._ignoreEvent = false;
            }
        }

        private void autoApplyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Update _peaksAnalyzer
            if (!_peaksAnalyzer._ignoreEvent)
            {
                _peaksAnalyzer._ignoreEvent = true;
                _peaksAnalyzer.ActivateAutoApply(autoApplyCheckBox.Checked);
                _peaksAnalyzer._ignoreEvent = false;
            }
        }

        private void showAccelerationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Chart signalChart = ((FormDetailsModify)FindForm()).signalChart;

            // If checked then show accelerations
            // If not then hide accelerations
            for (int i = 1; i < 4; i++)
            {
                signalChart.Series[i].LabelForeColor = showDeviationCheckBox.Checked ? Color.Black : Color.Transparent;
            }

            // Update _peaksAnalyzer
            if (!_peaksAnalyzer._ignoreEvent)
            {
                _peaksAnalyzer._ignoreEvent = true;
                _peaksAnalyzer.ActivateTDT(showDeviationCheckBox.Checked);
                _peaksAnalyzer._ignoreEvent = false;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _peaksAnalyzer.RemoveFilter();
        }
    }
}

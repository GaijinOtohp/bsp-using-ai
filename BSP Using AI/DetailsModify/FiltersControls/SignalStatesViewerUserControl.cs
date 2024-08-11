using Biological_Signal_Processing_Using_AI.DetailsModify.Filters;
using Biological_Signal_Processing_Using_AI.Garage;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.DetailsModify.FiltersControls
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
            FormsPlot signalChart = ((FormDetailsModify)FindForm()).signalChart;
            Dictionary<string, IPlottable> plots = ((FormDetailsModify)FindForm())._Plots;
            double samplingRate = _peaksAnalyzer._ParentFilteringTools._samplingRate;

            string[] labelsUps = null, labelsDowns = null, labelsStables = null;

            double[] xUps = statesDIc[SANamings.ScatterPlotsNames.UpPeaks].Select(x => x._index / samplingRate).ToArray();
            double[] yUps = statesDIc[SANamings.ScatterPlotsNames.UpPeaks].Select(x => x._value).ToArray();
            double[] xDowns = statesDIc[SANamings.ScatterPlotsNames.DownPeaks].Select(x => x._index / samplingRate).ToArray();
            double[] yDowns = statesDIc[SANamings.ScatterPlotsNames.DownPeaks].Select(x => x._value).ToArray();
            double[] xStables = statesDIc[SANamings.ScatterPlotsNames.StableStates].Select(x => x._index / samplingRate).ToArray();
            double[] yStables = statesDIc[SANamings.ScatterPlotsNames.StableStates].Select(x => x._value).ToArray();
            if (_peaksAnalyzer._activateTangentDeviationScan)
            {
                labelsUps = statesDIc[SANamings.ScatterPlotsNames.UpPeaks].Select(x => Math.Round(x._deviantionAngle, 2).ToString()).ToArray();
                labelsDowns = statesDIc[SANamings.ScatterPlotsNames.DownPeaks].Select(x => Math.Round(x._deviantionAngle, 2).ToString()).ToArray();
                labelsStables = statesDIc[SANamings.ScatterPlotsNames.StableStates].Select(x => Math.Round(x._deviantionAngle, 2).ToString()).ToArray();
            }

            GeneralTools.loadXYInChart(signalChart, plots[SANamings.ScatterPlotsNames.UpPeaks], xUps, yUps, labelsUps, _peaksAnalyzer._ParentFilteringTools._startingInSec, false, "SignalStatesViewerUserControl");
            GeneralTools.loadXYInChart(signalChart, plots[SANamings.ScatterPlotsNames.DownPeaks], xDowns, yDowns, labelsDowns, _peaksAnalyzer._ParentFilteringTools._startingInSec, false, "SignalStatesViewerUserControl");
            GeneralTools.loadXYInChart(signalChart, plots[SANamings.ScatterPlotsNames.StableStates], xStables, yStables, labelsStables, _peaksAnalyzer._ParentFilteringTools._startingInSec, false, "SignalStatesViewerUserControl");

            return statesDIc;
        }

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void artValueTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.keypressNumbersAndDecimalOnly(sender, e);
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
            Dictionary<string, IPlottable> plots = ((FormDetailsModify)FindForm())._Plots;
            // If checked then show states
            if (showStatesCheckBox.Checked)
                foreach (string stateName in new string[] { SANamings.ScatterPlotsNames.UpPeaks, SANamings.ScatterPlotsNames.DownPeaks, SANamings.ScatterPlotsNames.StableStates, SANamings.Selection, SANamings.ScatterPlotsNames.Labels })
                    plots[stateName].IsVisible = true;
            // If not then hide states
            else
                foreach (string stateName in new string[] { SANamings.ScatterPlotsNames.UpPeaks, SANamings.ScatterPlotsNames.DownPeaks, SANamings.ScatterPlotsNames.StableStates, SANamings.Selection, SANamings.ScatterPlotsNames.Labels })
                    plots[stateName].IsVisible = false;
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
            Dictionary<string, IPlottable> plots = ((FormDetailsModify)FindForm())._Plots;

            // If checked then show accelerations
            // If not then hide accelerations
            foreach (string stateName in new string[] { SANamings.ScatterPlotsNames.UpPeaks, SANamings.ScatterPlotsNames.DownPeaks, SANamings.ScatterPlotsNames.StableStates })
                ((ScatterPlot)plots[stateName]).DataPointLabelFont.Color = showDeviationCheckBox.Checked ? Color.Black : Color.Transparent;

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

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
using static BSP_Using_AI.DetailsModify.FormDetailsModify;
using static BSP_Using_AI.DetailsModify.FormDetailsModify.CornersScanner;

namespace BSP_Using_AI.DetailsModify.FiltersControls
{
    public partial class CornersScannerUserControl : UserControl
    {
        CornersScanner _cornersScanner;

        public CornersScannerUserControl(CornersScanner cornersScanner)
        {
            InitializeComponent();

            _cornersScanner = cornersScanner;
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public List<CornerSample> showSignalCorners(List<CornerSample> cornersList)
        {
            FormsPlot signalChart = ((FormDetailsModify)FindForm()).signalChart;
            Dictionary<string, IPlottable> plots = ((FormDetailsModify)FindForm())._Plots;
            double samplingRate = _cornersScanner._ParentFilteringTools._samplingRate;
            double startingInSec = _cornersScanner._ParentFilteringTools._startingInSec;

            string[] labelsCornerss = null;

            double[] xCorners = cornersList.Select(corner => corner._index / samplingRate).ToArray();
            double[] yCorners = cornersList.Select(corner => corner._value).ToArray();
            if (_cornersScanner._showAngles)
            {
                labelsCornerss = cornersList.Select(corner => Math.Round(corner._deviationAngle, 2).ToString()).ToArray();
            }

            GeneralTools.loadXYInChart(signalChart, plots[SANamings.ScatterPlotsNames.Labels], xCorners, yCorners, labelsCornerss, startingInSec, "CornersScannerUserControl");

            return cornersList;
        }

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void artValueTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.textBoxNumberOnly(sender, e);
        }

        private void artScrollBar_ValueChanged(object sender, EventArgs e)
        {
            // Update artValueTextBox
            if (!_cornersScanner._ignoreEvent)
            {
                _cornersScanner._ignoreEvent = true;
                // Change the threshold value
                _cornersScanner.SetART((artScrollBar.GetMax() - artScrollBar.Value) / (double)artScrollBar.GetMax());
                // Set the new value in artValueTextBox
                artValueTextBox.Text = Math.Round(_cornersScanner._art, 3).ToString();
                _cornersScanner._ignoreEvent = false;
            }
        }
        private void artValueTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!_cornersScanner._ignoreEvent)
            {
                _cornersScanner._ignoreEvent = true;
                // Get ART from the textbox
                double art = 0;
                if (artValueTextBox.Text.Length > 0)
                    art = double.Parse(artValueTextBox.Text);
                // Check if it crosses the max of amplitudeThresholdScrollBar
                if (artScrollBar.GetMax() - (art * artScrollBar.GetMax()) < 0)
                {
                    // If yes then set it as the max of amplitudeThresholdScrollBar
                    art = 1;
                    artValueTextBox.Text = Math.Round(art, 4).ToString();
                }
                // Change the threshold value
                _cornersScanner.SetART(art);
                // Update the scrollbar
                artScrollBar.Value = (int)(artScrollBar.GetMax() - (art * artScrollBar.GetMax()));
                _cornersScanner._ignoreEvent = false;
            }
        }

        private void angleThresholdScrollBar_ValueChanged(object sender, EventArgs e)
        {
            // Update atValueTextBox
            if (!_cornersScanner._ignoreEvent)
            {
                _cornersScanner._ignoreEvent = true;
                // Change the angle threshold value
                _cornersScanner.SetAT(angleThresholdScrollBar.Value / 10d);
                // Set the new value in thresholdRatioLabel
                atValueTextBox.Text = Math.Round(_cornersScanner._at, 2).ToString();
                _cornersScanner._ignoreEvent = false;
            }
        }
        private void atValueTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!_cornersScanner._ignoreEvent)
            {
                _cornersScanner._ignoreEvent = true;
                // Get angle threshold from the textbox
                double at = 0;
                if (atValueTextBox.Text.Length > 0)
                    at = double.Parse(atValueTextBox.Text);
                // Check if it crosses the max of tdtThresholdScrollBar
                if (at > 360)
                {
                    // If yes then set it as the max of tdtThresholdScrollBar
                    at = 360d;
                    atValueTextBox.Text = Math.Round(at, 2).ToString();
                }
                // Change the tangent deviation tolerance threshold value
                _cornersScanner.SetAT(at);
                // Update the scrollbar
                angleThresholdScrollBar.Value = (int)(at * 10);
                _cornersScanner._ignoreEvent = false;
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            double[] filteredSamples = _cornersScanner._ParentFilteringTools._FilteredSamples;
            if (filteredSamples != null)
                _cornersScanner.ScanCorners(filteredSamples);
        }

        private void showCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Dictionary<string, IPlottable> plots = ((FormDetailsModify)FindForm())._Plots;
            // If checked then show states
            if (showCornersCheckBox.Checked)
                foreach (string stateName in new string[] { SANamings.Selection, SANamings.ScatterPlotsNames.Labels })
                    plots[stateName].IsVisible = true;
            // If not then hide states
            else
                foreach (string stateName in new string[] { SANamings.Selection, SANamings.ScatterPlotsNames.Labels })
                    plots[stateName].IsVisible = false;

            // Update _peaksAnalyzer
            if (!_cornersScanner._ignoreEvent)
            {
                _cornersScanner._ignoreEvent = true;
                _cornersScanner.ActivateGenerally(showCornersCheckBox.Checked);
                _cornersScanner._ignoreEvent = false;
            }
        }

        private void autoApplyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Update _peaksAnalyzer
            if (!_cornersScanner._ignoreEvent)
            {
                _cornersScanner._ignoreEvent = true;
                _cornersScanner.ActivateAutoApply(autoApplyCheckBox.Checked);
                _cornersScanner._ignoreEvent = false;
            }
        }

        private void showDeviationAnglesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Dictionary<string, IPlottable> plots = ((FormDetailsModify)FindForm())._Plots;

            // If checked then show deviation angles
            // If not then hide deviation angles
            ((ScatterPlot)plots[SANamings.ScatterPlotsNames.Labels]).DataPointLabelFont.Color = showDeviationCheckBox.Checked ? Color.Black : Color.Transparent;

            // Update _peaksAnalyzer
            if (!_cornersScanner._ignoreEvent)
            {
                _cornersScanner._ignoreEvent = true;
                _cornersScanner.ShowAngles(showDeviationCheckBox.Checked);
                _cornersScanner._ignoreEvent = false;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _cornersScanner.RemoveFilter();
        }
    }
}

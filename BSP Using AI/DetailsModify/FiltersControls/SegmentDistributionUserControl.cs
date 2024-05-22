using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI;
using BSP_Using_AI.DetailsModify;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace Biological_Signal_Processing_Using_AI.DetailsModify.FiltersControls
{
    public partial class SegmentDistributionUserControl : UserControl
    {
        DistributionDisplay _DistributionDisplay;

        FormDetailsModify _FormDetailsModify;

        BarSeries _BarsPlot;

        HSpan distributionHSpan;

        public SegmentDistributionUserControl(DistributionDisplay distributionDisplay)
        {
            InitializeComponent();

            _DistributionDisplay = distributionDisplay;

            _FormDetailsModify = distributionDisplay._ParentFilteringTools._FormDetailModify;

            _BarsPlot = distributionSignalChart.Plot.AddBarSeries();

            FormsPlot signalChart = _FormDetailsModify.signalChart;
            distributionHSpan = GeneralTools.AddHorizontalSpan(signalChart, Color.Red, label: "Distribution horizontal span", HorizSpan_Dragged);
            distributionHSpan.X2 = 0.1d;
            distributionHSpan.IsVisible = true;
            _FormDetailsModify.signalChart.Refresh();
        }

        public void ShowDistribution(double[] distribution, double xOffset, double step)
        {
            _BarsPlot.Bars.Clear();
            List<Bar> barsList = new List<Bar>(distribution.Length);
            for (int barIndex = 0; barIndex < distribution.Length; barIndex++)
            {
                Bar newBar = new Bar();
                newBar.FillColor = Color.Blue;
                newBar.Thickness = step;
                newBar.Value = distribution[barIndex];
                newBar.Position = xOffset + barIndex * step;

                barsList.Add(newBar);
            }

            _BarsPlot.Bars.AddRange(barsList);

            distributionSignalChart.Plot.AxisAuto();
            distributionSignalChart.Refresh();
        }

        private void HorizSpan_Dragged(object sender, EventArgs e)
        {
            if (!_DistributionDisplay._ignoreEvent)
            {
                SignalPlot signalPlot = (SignalPlot)_FormDetailsModify._Plots[SANamings.Signal];

                // Get the highlighted span interval
                int spanStart = signalPlot.GetPointNearestX(distributionHSpan.X1).index;
                int spanEnd = signalPlot.GetPointNearestX(distributionHSpan.X2).index;
                (spanStart, spanEnd) = spanStart < spanEnd ? (spanStart, spanEnd) : (spanEnd, spanStart);

                _DistributionDisplay._ignoreEvent = true;

                if (_DistributionDisplay._segmentStarting != spanStart)
                    _DistributionDisplay.SetStartingIndex(spanStart);
                startingIndexTextBox.Text = _DistributionDisplay._segmentStarting.ToString();

                if (_DistributionDisplay._segmentEnding != spanEnd)
                    _DistributionDisplay.SetEndingIndex(spanEnd);
                endingIndexTextBox.Text = _DistributionDisplay._segmentEnding.ToString();

                _DistributionDisplay._ignoreEvent = false;
            }
        }

        private void startingIndexTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.keypressNumbersOnly(sender, e);
        }

        private void startingIndexTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!_DistributionDisplay._ignoreEvent)
            {
                SignalPlot signalPlot = (SignalPlot)_FormDetailsModify._Plots[SANamings.Signal];

                _DistributionDisplay._ignoreEvent = true;
                int startingIndex = 0;
                if (startingIndexTextBox.Text.Length > 0)
                    startingIndex = int.Parse(startingIndexTextBox.Text);

                _DistributionDisplay.SetStartingIndex(startingIndex);

                if (distributionHSpan.X1 < distributionHSpan.X2)
                    distributionHSpan.X1 = signalPlot.OffsetX + startingIndex / signalPlot.SampleRate;
                else
                    distributionHSpan.X2 = signalPlot.OffsetX + startingIndex / signalPlot.SampleRate;

                _FormDetailsModify.signalChart.Refresh();

                _DistributionDisplay._ignoreEvent = false;
            }
        }

        private void endingIndexTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!_DistributionDisplay._ignoreEvent)
            {
                SignalPlot signalPlot = (SignalPlot)_FormDetailsModify._Plots[SANamings.Signal];

                _DistributionDisplay._ignoreEvent = true;
                int endingIndex = 0;
                if (endingIndexTextBox.Text.Length > 0)
                    endingIndex = int.Parse(endingIndexTextBox.Text);

                _DistributionDisplay.SetEndingIndex(endingIndex);

                if (distributionHSpan.X1 > distributionHSpan.X2)
                    distributionHSpan.X1 = signalPlot.OffsetX + endingIndex / signalPlot.SampleRate;
                else
                    distributionHSpan.X2 = signalPlot.OffsetX + endingIndex / signalPlot.SampleRate;

                _FormDetailsModify.signalChart.Refresh();

                _DistributionDisplay._ignoreEvent = false;
            }
        }

        private void resolutionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!_DistributionDisplay._ignoreEvent)
            {
                _DistributionDisplay._ignoreEvent = true;
                int resolution = 1;
                if (resolutionTextBox.Text.Length > 0)
                    resolution = int.Parse(resolutionTextBox.Text);

                _DistributionDisplay.SetResolution(resolution);
                _DistributionDisplay._ignoreEvent = false;
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            (double[] distribution, double xOffset, double step) = _DistributionDisplay.CoputeDistribution();
            ShowDistribution(distribution, xOffset, step);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormsPlot signalChart = ((FormDetailsModify)this.FindForm()).signalChart;
            signalChart.Plot.Remove(distributionHSpan);
            _DistributionDisplay.RemoveFilter();
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
                FilteringTools filteringTools = _DistributionDisplay._ParentFilteringTools.Clone();
                // Remove filters
                filteringTools.RemoveAllFilters();
                // Get samples from signal chart
                double[] samples = new double[signalPlot.PointCount];
                for (int i = 0; i < samples.Length; i++)
                    samples[i] = signalPlot.Ys[i];
                filteringTools.SetOriginalSamples(samples);

                EventHandlers.sendSignalTool(filteringTools, _FormDetailsModify.pathLabel.Text + "\\Distribution");
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
                FilteringTools filteringTools = _DistributionDisplay._ParentFilteringTools.Clone();
                // Remove filters
                filteringTools.RemoveAllFilters();
                // Get samples from signal chart
                double[] samples = new double[signalPlot.PointCount];
                for (int i = 0; i < samples.Length; i++)
                    samples[i] = signalPlot.Ys[i] * filteringTools._quantizationStep;
                filteringTools.SetOriginalSamples(samples);

                EventHandlers.analyseSignalTool(filteringTools, _FormDetailsModify.pathLabel.Text + "\\Distribution");
            }
        }
    }
}

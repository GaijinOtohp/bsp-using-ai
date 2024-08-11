using Biological_Signal_Processing_Using_AI.DetailsModify.FiltersControls;
using Biological_Signal_Processing_Using_AI.Garage;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace Biological_Signal_Processing_Using_AI.DetailsModify.Filters
{
    public class DistributionDisplay : FilterBase
    {
        public int _segmentStarting;
        public int _segmentEnding;

        public int _resolution = 100;

        public bool _autoApply { get; set; } = true;

        public override DistributionDisplay Clone(FilteringTools filteringTools)
        {
            // Clone filter properties
            DistributionDisplay distributionDisplay = new DistributionDisplay(filteringTools);
            distributionDisplay._segmentStarting = _segmentStarting;
            distributionDisplay._segmentEnding = _segmentEnding;
            distributionDisplay._resolution = _resolution;
            distributionDisplay._autoApply = _autoApply;

            distributionDisplay.CloneBase(this);
            // CLone the control
            if (_FilterControl != null)
            {
                distributionDisplay._FilterControl = new SegmentDistributionUserControl(distributionDisplay);
                distributionDisplay.ActivateAutoApply(_autoApply);
                distributionDisplay.SetStartingIndex(_segmentStarting);
                distributionDisplay.SetEndingIndex(_segmentEnding);
                distributionDisplay.SetResolution(_resolution);
                distributionDisplay.ActivateGenerally(_activated);
            }
            return distributionDisplay;
        }

        public DistributionDisplay(FilteringTools parentFilteringTools)
        {
            _ParentFilteringTools = parentFilteringTools;
            Name = GetType().Name;
        }
        public override Control InitializeFilterControl()
        {
            return new SegmentDistributionUserControl(this);
        }
        public override (double[] filteredSignal, bool reloadSignal) ApplyFilter(double[] filteredSamples, bool forceApply, bool showResultsInChart)
        {
            if (this._autoApply || forceApply)
            {
                // Show distribution in the chart
                (double[] distribution, double xOffset, double step) = CoputeDistribution();
                if (this._FilterControl != null && showResultsInChart)
                    if (this._FilterControl.IsHandleCreated) ((SegmentDistributionUserControl)this._FilterControl).ShowDistribution(distribution, xOffset, step);
            }
            return (filteredSamples, false);
        }
        public override void Activate(bool activate)
        {
            ((SegmentDistributionUserControl)_FilterControl).autoApplyCheckBox.Checked = activate;
        }

        public void SetStartingIndex(int startingIndex)
        {
            if (startingIndex > _segmentEnding)
                return;
            _segmentStarting = startingIndex;
            // Update the control
            UpdateControl();
            ApplyFilter(((SignalPlot)_ParentFilteringTools._FormDetailModify._Plots[SANamings.Signal]).Ys, false, true);
        }
        public void SetEndingIndex(int endingIndex)
        {
            if (endingIndex < _segmentStarting)
                return;
            _segmentEnding = endingIndex;
            // Update the control
            UpdateControl();
            ApplyFilter(((SignalPlot)_ParentFilteringTools._FormDetailModify._Plots[SANamings.Signal]).Ys, false, true);
        }
        public void SetResolution(int resolution)
        {
            if (resolution < 1)
                return;
            _resolution = resolution;
            // Update the control
            UpdateControl();
            _ParentFilteringTools?.ApplyFilters(false);
        }
        public void ActivateAutoApply(bool activate)
        {
            _autoApply = activate;
            // Update the control
            UpdateControl();
            _ParentFilteringTools?.ApplyFilters(false);
        }

        public void UpdateControl()
        {
            if (_FilterControl != null)
            {
                SegmentDistributionUserControl distributionDisplay = (SegmentDistributionUserControl)_FilterControl;
                if (!_ignoreEvent)
                {
                    _ignoreEvent = true;
                    distributionDisplay.startingIndexTextBox.Text = _segmentStarting.ToString();
                    distributionDisplay.endingIndexTextBox.Text = _segmentEnding.ToString();
                    distributionDisplay.resolutionTextBox.Text = _resolution.ToString();
                    distributionDisplay.autoApplyCheckBox.Checked = _autoApply;
                    _ignoreEvent = false;
                }
            }
        }

        public (double[] distribution, double xOffset, double step) CoputeDistribution()
        {
            // Get the selected segment's samples
            double[] segmentSamples = _ParentFilteringTools._FilteredSamples.Where((sample, index) => _segmentStarting <= index && index <= _segmentEnding).ToArray();

            (double[] distribution, double xOffset, double step) = CoputeDistribution(segmentSamples, _resolution);

            return (distribution, xOffset, step);
        }

        public static (double[] distribution, double xOffset, double step) CoputeDistribution(double[] segmentSamples, int resolution)
        {
            // Get the segment characteristics
            (double mean, double min, double max) = GeneralTools.MeanMinMax(segmentSamples);
            // Compute the step of the distribution based on the resolution
            double step = (max - min) / resolution;

            // Compute the distribution
            int[] distribution = new int[resolution];
            double[] distributionNormalized = new double[resolution];

            if (step != 0)
                foreach (double sample in segmentSamples)
                {
                    int barIndex = (int)((sample - min) / step);
                    if (barIndex == distribution.Length)
                        barIndex--;
                    distribution[barIndex]++;
                }
            for (int i = 0; i < distribution.Length; i++)
                distributionNormalized[i] = (double)distribution[i] / segmentSamples.Length;

            return (distributionNormalized, min, step);
        }
    }
}

using Biological_Signal_Processing_Using_AI.DetailsModify.Filters;
using Biological_Signal_Processing_Using_AI.DetailsModify.FiltersControls;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.DetailsModify.Filters;
using BSP_Using_AI.DetailsModify.Filters.IIRFilters;
using BSP_Using_AI.DetailsModify.FiltersControls;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.DetailsModify
{
    public partial class FormDetailsModify
    {
        FilteringTools _FilteringTools { get; set; }

        public partial class FilteringTools
        {
            public double[] _OriginalRawSamples { get; set; } = new double[1];
            public double[] _RawSamples { get; set; } = new double[1];
            public double[] _FilteredSamples { get; set; } = new double[1];
            public int _originalSamplingRate { get; set; }
            public int _samplingRate { get; set; }
            public double _quantizationStep { get; set; }
            public double _startingInSec { get; set; } = 0;

            public Dictionary<string, FilterBase> _FiltersDic = new Dictionary<string, FilterBase>();
            public FormDetailsModify _FormDetailModify;
            private bool _autoApply { get; set; } = true;
            public bool _ignoreFiltering { get; set; } = false;
            private bool _showResultInChart { get; set; } = true;

            public bool _ignoreEvent { get; set; } = false;

            public FilteringTools Clone()
            {
                FilteringTools clonedFilteringTools = new FilteringTools(_samplingRate, _quantizationStep, null);
                // Clone signal properties
                clonedFilteringTools._OriginalRawSamples = (double[])_OriginalRawSamples.Clone();
                clonedFilteringTools._RawSamples = (double[])_RawSamples.Clone();
                clonedFilteringTools._FilteredSamples = (double[])_FilteredSamples.Clone();
                clonedFilteringTools._originalSamplingRate = _originalSamplingRate;
                clonedFilteringTools._startingInSec = _startingInSec;
                // Clone filters
                clonedFilteringTools.SetAutoApply(false);
                foreach (string filterName in _FiltersDic.Keys)
                    clonedFilteringTools._FiltersDic.Add(filterName, _FiltersDic[filterName].Clone(clonedFilteringTools));
                clonedFilteringTools.SetAutoApply(true);
                // Clone other properties
                clonedFilteringTools._autoApply = _autoApply;
                clonedFilteringTools._ignoreFiltering = _ignoreFiltering;
                clonedFilteringTools._showResultInChart = _showResultInChart;
                clonedFilteringTools._ignoreEvent = _ignoreEvent;

                return clonedFilteringTools;
            }

            public FilteringTools(int samplingRate, double quantizationStep, FormDetailsModify formDetailModify)
            {
                _originalSamplingRate = samplingRate;
                _samplingRate = samplingRate;
                _quantizationStep = quantizationStep;
                _FormDetailModify = formDetailModify;
            }

            public void SetOriginalSamples(double[] originalSamples)
            {
                _OriginalRawSamples = (double[])originalSamples.Clone();
                _RawSamples = (double[])originalSamples.Clone();
                _FilteredSamples = (double[])originalSamples.Clone();
            }

            public void SetOriginalSamplingRate(int originalSamplingRate)
            {
                if (originalSamplingRate < 1)
                    return;
                _originalSamplingRate = originalSamplingRate;
                SetSamplingRate(_originalSamplingRate);
            }

            public void SetSamplingRate(int samplingRate)
            {
                if (samplingRate < 1)
                    return;
                _samplingRate = samplingRate;
                UpdateControl();
                // Update other filters that relate sampling rate
                foreach (FilterBase filter in _FiltersDic.Values)
                    if (filter is IIRFilter)
                    {
                        if ((filter as IIRFilter)._maxFreq * 2 > _samplingRate)
                        {
                            (filter as IIRFilter).SetMaxFreq(_samplingRate / 2);
                            (filter as IIRFilter).UpdateControl();
                        }
                    }
                    else if (filter is PeaksAnalyzer)
                        (filter as PeaksAnalyzer).UpdateControl();
                ApplyFilters(false);
            }
            public void SetQuantizationStep(double quantizationStep)
            {
                if (quantizationStep < 1)
                    return;
                _quantizationStep = quantizationStep;
                UpdateControl();
                ApplyFilters(false);
            }
            public void SetFormDetailModify(FormDetailsModify formDetailModify)
            {
                _FormDetailModify = formDetailModify;
            }
            public void SetStartingInSecond(double startingInSec)
            {
                _startingInSec = startingInSec;
                ApplyFilters(false);
            }
            public void SetAutoApply(bool activate)
            {
                _autoApply = activate;
                UpdateControl();
            }
            public void ShowResultInChart(bool showResultInChart)
            {
                _showResultInChart = showResultInChart;
            }

            private void UpdateControl()
            {
                if (_FormDetailModify != null)
                {
                    if (!_ignoreEvent)
                    {
                        _ignoreEvent = true;
                        _FormDetailModify.autoApplyCheckBox.Checked = _autoApply;
                        _FormDetailModify.samplingRateTextBox.Text = _samplingRate.ToString();
                        _FormDetailModify.quantizationStepTextBox.Text = Math.Round(_quantizationStep, 2).ToString();
                        _ignoreEvent = false;
                    }
                }
            }

            public void RemoveAllFilters()
            {
                if (_FiltersDic.Count == 0)
                    return;
                SetAutoApply(false);
                List<FilterBase> filtersList = _FiltersDic.Select(filter => filter.Value).ToList();
                foreach (FilterBase filter in filtersList)
                    filter.RemoveFilter();
                SetAutoApply(true);
            }

            public void ApplyFilters(bool forceApply)
            {
                if (!(forceApply || _autoApply) || _RawSamples == null || _ignoreFiltering)
                    return;
                _ignoreFiltering = true;

                // Copy rawf samples to filtered samples
                _FilteredSamples = _RawSamples.Select(sample => sample / _quantizationStep).ToArray();
                SetSamplingRate(_originalSamplingRate);

                List<FilterBase> sortedFiltersDic = _FiltersDic.OrderBy(filter => filter.Value._sortOrder).Select(filter => filter.Value).ToList();

                bool reloadSignal = sortedFiltersDic.Count > 0 ? false : true;
                foreach (FilterBase filter in sortedFiltersDic)
                    if (filter._activated)
                    {
                        // Apply the filter to _FilteredSamples
                        (_FilteredSamples, bool tempReloadSignal) = filter.ApplyFilter(_FilteredSamples, forceApply, _showResultInChart);
                        reloadSignal |= tempReloadSignal;
                    }

                if(reloadSignal)
                    // Show signal in the chart
                    if (_FormDetailModify != null && _showResultInChart)
                        if (_FormDetailModify.IsHandleCreated)
                            _FormDetailModify.loadSignal(_FilteredSamples, _samplingRate, _startingInSec);

                _ignoreFiltering = false;
            }
        }
    }
}

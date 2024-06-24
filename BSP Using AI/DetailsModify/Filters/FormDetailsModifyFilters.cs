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

        public abstract partial class FilterBase
        {
            public string Name { get; set; }
            public bool _activated { get; set; } = true;
            public int _sortOrder { get; set; }

            public FilteringTools _ParentFilteringTools { get; set; }

            public Control _FilterControl;
            public bool _ignoreEvent { get; set; } = false;

            public abstract Control InitializeFilterControl();
            public abstract (double[] filteredSignal, bool reloadSignal) ApplyFilter(double[] filteredSamples, bool forceApply, bool showResultsInChart);
            public abstract void Activate(bool activate);

            /// <summary>
            /// Insert filter in filters dictionary and filters flow layout panel
            /// </summary>
            /// <param name="filtersFlowLayout">if null, then insert filter in dictionary only.</param>
            public void InsertFilter(FlowLayoutPanel filtersFlowLayout)
            {
                // Set the order of the filter
                if (_ParentFilteringTools._FiltersDic.Count > 0)
                    _sortOrder = _ParentFilteringTools._FiltersDic.Max(filter => filter.Value._sortOrder);
                _sortOrder++;
                // Append order of the filter to its name if the name already exists
                if (_ParentFilteringTools._FiltersDic.ContainsKey(Name))
                    Name += _sortOrder;

                // Initialize the filter user control
                _FilterControl = InitializeFilterControl();

                // Insert the filter in FiltersDic
                _ParentFilteringTools._FiltersDic.Add(Name, this);

                // Insert the filter control in filtersFlowLayout
                filtersFlowLayout?.Controls.Add(_FilterControl);
                // Apply filtering
                _ParentFilteringTools?.ApplyFilters(false);
            }

            /// <summary>
            /// Remove filter from filters dictionary and filters flow layout panel
            /// </summary>
            /// <param name="filtersFlowLayout">if null, then remove filter from dictionary only.</param>
            public void RemoveFilter()
            {
                // Remove the filter from FiltersDic
                if (_ParentFilteringTools._FiltersDic.ContainsKey(Name))
                    _ParentFilteringTools._FiltersDic.Remove(Name);
                // Remove filter from flow layout panel
                if (_FilterControl != null)
                {
                    FlowLayoutPanel filtersFlowLayout = (FlowLayoutPanel)_FilterControl.Parent;
                    if (filtersFlowLayout != null)
                        filtersFlowLayout.Controls.Remove(_FilterControl);
                }
                // Apply filtering
                _ParentFilteringTools?.ApplyFilters(false);
            }

            public void SetSortOrder(int order)
            {
                // Sort filters according to _sortOrder
                List<FilterBase> sortedFilters = _ParentFilteringTools._FiltersDic.OrderBy(filter => filter.Value._sortOrder).Select(filter => filter.Value).ToList();
                // Set this filter's new sort order
                _sortOrder = sortedFilters[order]._sortOrder;
                if (!Name.Equals(GetType().Name))
                    SetName(GetType().Name + _sortOrder);
                // Update sort order of the next filters
                for (int i = order; i < sortedFilters.Count; i++)
                    if (sortedFilters[i] != this)
                    {
                        sortedFilters[i]._sortOrder++;
                        if (!sortedFilters[i].Name.Equals(sortedFilters[i].GetType().Name))
                            sortedFilters[i].SetName(sortedFilters[i].GetType().Name + sortedFilters[i]._sortOrder);
                    }
                // Set the order of the filter control in filtersFlowLayout
                if (_FilterControl != null)
                    if (_FilterControl.Parent != null)
                        ((FlowLayoutPanel)_FilterControl.Parent).Controls.SetChildIndex(_FilterControl, order);
            }
            public void SetName(string name)
            {
                // Update filters dictionary
                if (_ParentFilteringTools._FiltersDic.ContainsKey(Name))
                {
                    _ParentFilteringTools._FiltersDic.Remove(Name);
                    _ParentFilteringTools._FiltersDic.Add(name, this);
                }
                // Update control's name
                if (_FilterControl != null)
                {
                    _FilterControl.Name = name;
                    // Check if the control is IIRFilterUserControl
                    if (_FilterControl is IIRFilterUserControl)
                        // Update the label of the filter
                        (_FilterControl as IIRFilterUserControl).nameFilterLabel.Text = name;
                }

                Name = name;
            }

            public void ActivateGenerally(bool activate)
            {
                _activated = activate;
                // Update the control
                if (_FilterControl != null && !_ignoreEvent)
                {
                    _ignoreEvent = true;
                    Activate(activate);
                    _ignoreEvent = false;
                }
                // Apply filtering
                _ParentFilteringTools?.ApplyFilters(false);
            }
        }

        //______________________________________________________________________//
        //:::::::::::::::::::::::::::::DC removal::::::::::::::::::::::::::::::://
        public partial class DCRemoval : FilterBase
        {
            public DCRemoval(FilteringTools parentFilteringTools)
            {
                _ParentFilteringTools = parentFilteringTools;
                Name = GetType().Name;
            }
            public override Control InitializeFilterControl()
            {
                return new DCRemovalUserControl(this);
            }
            public override (double[] filteredSignal, bool reloadSignal) ApplyFilter(double[] filteredSamples, bool forceApply, bool showResultsInChart)
            {
                return (GeneralTools.removeDCValue(filteredSamples), true);
            }
            public override void Activate(bool activate)
            {
                ((DCRemovalUserControl)_FilterControl).dcValueRemoveCheckBox.Checked = activate;
            }
        }
        //______________________________________________________________________//
        //::::::::::::::::::::::::::::::Normalize::::::::::::::::::::::::::::::://
        public partial class Normalize : FilterBase
        {
            public Normalize(FilteringTools parentFilteringTools)
            {
                _ParentFilteringTools = parentFilteringTools;
                Name = GetType().Name;
            }
            public override Control InitializeFilterControl()
            {
                return new NormalizedSignalUserControl(this);
            }
            public override (double[] filteredSignal, bool reloadSignal) ApplyFilter(double[] filteredSamples, bool forceApply, bool showResultsInChart)
            {
                return (GeneralTools.normalizeSignal(filteredSamples), true);
            }
            public override void Activate(bool activate)
            {
                ((NormalizedSignalUserControl)_FilterControl).normalizeSignalCheckBox.Checked = activate;
            }
        }
        //______________________________________________________________________//
        //:::::::::::::::::::::::::::::::Absolute::::::::::::::::::::::::::::::://
        public partial class Absolute : FilterBase
        {
            public Absolute(FilteringTools parentFilteringTools)
            {
                _ParentFilteringTools = parentFilteringTools;
                Name = GetType().Name;
            }
            public override Control InitializeFilterControl()
            {
                return new AbsoluteSignalUserControl(this);
            }
            public override (double[] filteredSignal, bool reloadSignal) ApplyFilter(double[] filteredSamples, bool forceApply, bool showResultsInChart)
            {
                return (GeneralTools.absoluteSignal(filteredSamples), true);
            }
            public override void Activate(bool activate)
            {
                ((AbsoluteSignalUserControl)_FilterControl).absoluteSignalCheckBox.Checked = activate;
            }
        }
        //______________________________________________________________________//
        //:::::::::::::::::::::::::Existance declare:::::::::::::::::::::::::::://
        public partial class ExistanceDeclare : FilterBase
        {
            public int _exists { get; set; } = 0;
            public string _Label { get; set; }

            public ExistanceDeclare(FilteringTools parentFilteringTools, string label)
            {
                _ParentFilteringTools = parentFilteringTools;
                _Label = label;
                Name = GetType().Name;
            }
            public override Control InitializeFilterControl()
            {
                return new CheckExistanceUserControl(this);
            }
            public override (double[] filteredSignal, bool reloadSignal) ApplyFilter(double[] filteredSamples, bool forceApply, bool showResultsInChart)
            {
                return (filteredSamples, false);
            }
            public override void Activate(bool activate)
            {

            }

            public void SetExistance(bool exists)
            {
                _exists = Convert.ToInt32(exists);
                if (!_ignoreEvent)
                {
                    _ignoreEvent = true;
                    ((CheckExistanceUserControl)_FilterControl).existanceOfCheckBox.Checked = exists;
                    _ignoreEvent = false;
                }
            }
        }
        //______________________________________________________________________//
        //:::::::::::::::::::::::::::::IIR filter::::::::::::::::::::::::::::::://
        public partial class IIRFilter : FilterBase
        {
            public enum FilterType
            {
                Butterworth,
                ChebyshevI,
                ChebyshevII
            }
            public enum FilterBand
            {
                LowPass,
                HighPass
            }

            public FilterType _selectedType { get; set; }
            public FilterBand _selectedBand { get; set; } = FilterBand.LowPass;
            public int _order { get; set; } = 4;
            public double _minFreq { get; set; } = 1d;
            public double _normalizedFreq { get; set; } = 1d;
            public double _maxFreq { get; set; } = 1d;

            public IIRFilter(FilteringTools parentFilteringTools, string filterType)
            {
                _ParentFilteringTools = parentFilteringTools;
                Name = filterType;
                switch (filterType)
                {
                    case "Butterworth":
                        _selectedType = FilterType.Butterworth;
                        break;
                    case "Chebyshev I":
                        _selectedType = FilterType.ChebyshevI;
                        break;
                    case "Chebyshev II":
                        _selectedType = FilterType.ChebyshevII;
                        break;
                }
            }
            public override Control InitializeFilterControl()
            {
                return new IIRFilterUserControl(this);
            }
            public override (double[] filteredSignal, bool reloadSignal) ApplyFilter(double[] filteredSamples, bool forceApply, bool showResultsInChart)
            {
                this.Filter(filteredSamples);
                return (filteredSamples, true);
            }
            public override void Activate(bool activate)
            {

            }

            public void SetFilterBand(int bandType)
            {
                _selectedBand = (FilterBand)bandType;
                UpdateControl();
                _ParentFilteringTools?.ApplyFilters(false);
            }

            public void SetOrder(int order)
            {
                if (order < 1)
                    return;
                _order = order;
                UpdateControl();
                _ParentFilteringTools?.ApplyFilters(false);
            }

            public bool SetMinFreq(double minFreq)
            {
                if (minFreq >= _maxFreq || minFreq < 0) // It should neither reach the max frequency, nor be negative
                    return false;
                if (minFreq / _ParentFilteringTools._samplingRate > _normalizedFreq) // Check if the normalized frequency should be resetted
                    _normalizedFreq = minFreq / _ParentFilteringTools._samplingRate;
                _minFreq = minFreq;
                UpdateControl();
                _ParentFilteringTools?.ApplyFilters(false);
                return true;
            }
            public void SetNormalizedFreq(double normalizedFreq)
            {
                if (normalizedFreq < 0 || normalizedFreq > 1)
                    return;
                _normalizedFreq = normalizedFreq;
                UpdateControl();
                _ParentFilteringTools?.ApplyFilters(false);
            }
            public bool SetMaxFreq(double maxFreq)
            {
                if (maxFreq <= _minFreq || maxFreq * 2 > _ParentFilteringTools._samplingRate) // It should neither reach teh min frequency, nor be greater than the half of the sampling rate
                    return false;
                if (maxFreq / _ParentFilteringTools._samplingRate < _normalizedFreq) // Check if the normalized frequency should be resetted
                    _normalizedFreq = maxFreq / _ParentFilteringTools._samplingRate;
                _maxFreq = maxFreq;
                UpdateControl();
                _ParentFilteringTools?.ApplyFilters(false);
                return true;
            }

            public void UpdateControl()
            {
                if (_FilterControl != null)
                {
                    IIRFilterUserControl iirFilter = (IIRFilterUserControl)_FilterControl;
                    if (!_ignoreEvent)
                    {
                        _ignoreEvent = true;
                        iirFilter.filterBandComboBox.SelectedIndex = (int)_selectedBand;
                        iirFilter.orderTextBox.Text = _order.ToString();
                        iirFilter.minFreqTextBox.Text = _minFreq.ToString();
                        iirFilter.maxFreqTextBox.Text = _maxFreq.ToString();
                        double cutoffFreq = _normalizedFreq * _ParentFilteringTools._samplingRate;
                        iirFilter.frequencyScrollBar.Value = (int)(((cutoffFreq - _minFreq) * iirFilter.frequencyScrollBar.GetMax()) / (_maxFreq - _minFreq));
                        iirFilter.cutoffFreqLabel.Text = "Cutoff freq: " + Math.Round(cutoffFreq, 3) + " Hz";
                        _ignoreEvent = false;
                    }
                }
            }

            public void Filter(double[] samples)
            {
                if (samples.Length == 0)
                    return;

                // Set the signal as float
                float[] floatSamples = new float[samples.Length];
                for (int i = 0; i < samples.Length; i++)
                    floatSamples[i] = (float)samples[i];

                // Set the filter according to its type and band
                NWaves.Filters.Base.IirFilter filter = null;
                NWaves.Signals.DiscreteSignal filteredSignal = null;
                switch (_selectedType)
                {
                    case FilterType.Butterworth:
                        if (_selectedBand == FilterBand.LowPass)
                            filter = new NWaves.Filters.Butterworth.LowPassFilter(_normalizedFreq, _order);
                        else if (_selectedBand == FilterBand.HighPass)
                            filter = new NWaves.Filters.Butterworth.HighPassFilter(_normalizedFreq, _order);
                        break;
                    case FilterType.ChebyshevI:
                        if (_selectedBand == FilterBand.LowPass)
                            filter = new NWaves.Filters.ChebyshevI.LowPassFilter(_normalizedFreq, _order);
                        else if (_selectedBand == FilterBand.HighPass)
                            filter = new NWaves.Filters.ChebyshevI.HighPassFilter(_normalizedFreq, _order);
                        break;
                    case FilterType.ChebyshevII:
                        if (_selectedBand == FilterBand.LowPass)
                            filter = new NWaves.Filters.ChebyshevII.LowPassFilter(_normalizedFreq, _order);
                        else if (_selectedBand == FilterBand.HighPass)
                            filter = new NWaves.Filters.ChebyshevII.HighPassFilter(_normalizedFreq, _order);
                        break;
                }

                // Filter the signal
                filteredSignal = filter.ApplyTo(new NWaves.Signals.DiscreteSignal(_ParentFilteringTools._samplingRate, floatSamples));

                // Copy the filtered signal to samples
                for (int i = 0; i < filteredSignal.Length; i++)
                    samples[i] = filteredSignal.Samples[i];
            }
        }
        //______________________________________________________________________//
        //:::::::::::::::::::::::::::::::::DWT:::::::::::::::::::::::::::::::::://
        public partial class DWT : FilterBase
        {
            public enum WaveletType
            {
                Haar,
                Daubechies,
                Symlet,
                Coiflet
            }

            public WaveletType _waveletType { get; set; } = WaveletType.Haar;
            List<double[]> _DWTLevelsSamples { get; set; }

            public string _SelectedWavelet { get; set; } = "haar";
            public int _maxLevel { get; set; } = int.MaxValue;
            public int _selectedLevel { get; set; } = 0;

            public DWT(FilteringTools parentFilteringTools)
            {
                _ParentFilteringTools = parentFilteringTools;
                Name = GetType().Name;
            }
            public override Control InitializeFilterControl()
            {
                // Check if _FiltersDic in _ParentFilteringTools contains PeaksAnalyzer
                foreach (FilterBase filter in _ParentFilteringTools._FiltersDic.Values)
                    if (filter is PeaksAnalyzer peaksAnalyzer)
                        peaksAnalyzer._DWT = this;

                return new DWTUserControl(this);
            }
            public override (double[] filteredSignal, bool reloadSignal) ApplyFilter(double[] filteredSamples, bool forceApply, bool showResultsInChart)
            {
                return (this.TransformSamples(filteredSamples), true);
            }
            public override void Activate(bool activate)
            {

            }

            public void SetWaveletType(WaveletType waveletType)
            {
                switch (waveletType)
                {
                    case WaveletType.Haar:
                        _SelectedWavelet = "haar";
                        break;
                    case WaveletType.Daubechies:
                        _SelectedWavelet = "db1";
                        break;
                    case WaveletType.Symlet:
                        _SelectedWavelet = "sym2";
                        break;
                    case WaveletType.Coiflet:
                        _SelectedWavelet = "coif1";
                        break;
                }
                _waveletType = waveletType;
                _selectedLevel = 0;
                UpdateControl();
                _ParentFilteringTools?.ApplyFilters(false);
            }

            /// <summary>
            /// Vanishing moments number for:
            /// . Haar: doesn't exist
            /// . Daubechies: from 1 to 20
            /// . Symlet: from 2 to 20
            /// . Coiflet: from 1 to 5
            /// </summary>
            /// <param name="numOfVanMo"></param>
            /// <returns>true if the wavelet is changed correctly</returns>
            public bool SetNumberOfVanishingMoments(int numOfVanMo)
            {
                bool processStatus = false;
                switch (_waveletType)
                {
                    case WaveletType.Daubechies:
                        if (numOfVanMo >= 1 && numOfVanMo <= 20)
                        {
                            _SelectedWavelet = "db" + numOfVanMo;
                            processStatus = true;
                        }
                        break;
                    case WaveletType.Symlet:
                        if (numOfVanMo >= 2 && numOfVanMo <= 20)
                        {
                            _SelectedWavelet = "sym" + numOfVanMo;
                            processStatus = true;
                        }
                        break;
                    case WaveletType.Coiflet:
                        if (numOfVanMo >= 1 && numOfVanMo <= 5)
                        {
                            _SelectedWavelet = "coif1" + numOfVanMo;
                            processStatus = true;
                        }
                        break;
                }
                if (processStatus)
                {
                    _selectedLevel = 0;
                    UpdateControl();
                    _ParentFilteringTools?.ApplyFilters(false);
                }
                return processStatus;
            }

            public void SetMaxLevel(int maxLevel)
            {
                if (maxLevel > 0)
                    _maxLevel = maxLevel;
            }

            /// <summary>
            /// For the first level insert 0
            /// </summary>
            /// <param name="level"></param>
            public void SelectLevel(int level)
            {
                _selectedLevel = level;
                UpdateControl();
                _ParentFilteringTools?.ApplyFilters(false);
            }

            private void UpdateControl()
            {
                if (_FilterControl != null)
                {
                    DWTUserControl dwtUserControl = (DWTUserControl)_FilterControl;
                    if (!_ignoreEvent)
                    {
                        _ignoreEvent = true;
                        dwtUserControl.waveletTypeComboBox.SelectedIndex = (int)_waveletType;
                        dwtUserControl.numOfVanMoComboBox.Text = _SelectedWavelet;
                        dwtUserControl.levelComboBox.SelectedIndex = _selectedLevel;
                        _ignoreEvent = false;
                    }
                }
            }

            public double[] TransformSamples(double[] samples)
            {
                _DWTLevelsSamples = GeneralTools.calculateDWT(samples, _SelectedWavelet, _maxLevel);
                // Reset the selected level if needed
                if (_selectedLevel >= _DWTLevelsSamples.Count)
                    _selectedLevel = _DWTLevelsSamples.Count - 1;
                // Update levels of the transformations
                if (_FilterControl != null)
                {
                    _ignoreEvent = true;
                    DWTUserControl dwtUserControl = (DWTUserControl)_FilterControl;

                    List<string> typesOfWavelets = new List<string>(_DWTLevelsSamples.Count);
                    for (int i = 1; i <= _DWTLevelsSamples.Count; i++)
                        typesOfWavelets.Add("Level " + i);
                    dwtUserControl.levelComboBox.DataSource = typesOfWavelets;
                    dwtUserControl.levelComboBox.Text = "Level " + (_selectedLevel + 1);
                    _ignoreEvent = false;
                }
                // Set samples as the selected level
                if (_DWTLevelsSamples.Count > 0 && _activated)
                {
                    samples = (double[])_DWTLevelsSamples[_selectedLevel].Clone();
                    // Set sampling rate according to the selected level
                    _ParentFilteringTools.SetSamplingRate((int)(_ParentFilteringTools._originalSamplingRate / Math.Pow(2, _selectedLevel + 1)));
                }
                return samples;
            }
        }
        //______________________________________________________________________//
        //::::::::::::::::::::::::::Peaks analyzer:::::::::::::::::::::::::::::://
        public partial class PeaksAnalyzer : FilterBase
        {
            public double _art { get; set; } = 0.2;
            public double _ht { get; set; } = 0.01;
            public double _tdt { get; set; } = 0.1;
            public bool _autoApply { get; set; } = true;
            public bool _activateTangentDeviationScan { get; set; } = false;
            public Dictionary<string, List<State>> _StatesDIc { get; set; }

            public Dictionary<int, Dictionary<string, List<State>>> _DWTLevelsStatesDIc { get; set; } = new Dictionary<int, Dictionary<string, List<State>>>();
            public DWT _DWT { get; set; }

            public PeaksAnalyzer(FilteringTools parentFilteringTools)
            {
                _ParentFilteringTools = parentFilteringTools;
                Name = GetType().Name;
            }
            public override Control InitializeFilterControl()
            {
                // Check if _FiltersDic in _ParentFilteringTools contains dwt filter
                foreach (FilterBase filter in _ParentFilteringTools._FiltersDic.Values)
                    if (filter is DWT dwt)
                        this._DWT = dwt;

                return new SignalStatesViewerUserControl(this);
            }
            public override (double[] filteredSignal, bool reloadSignal) ApplyFilter(double[] filteredSamples, bool forceApply, bool showResultsInChart)
            {
                if (this._autoApply || forceApply)
                {
                    this.ScanPeaks(filteredSamples);
                    // Show states in the chart
                    if (this._FilterControl != null && showResultsInChart)
                        if (this._FilterControl.IsHandleCreated) ((SignalStatesViewerUserControl)this._FilterControl).showSignalStates(this._StatesDIc);
                }
                return (filteredSamples, false);
            }
            public override void Activate(bool activate)
            {
                ((SignalStatesViewerUserControl)_FilterControl).showStatesCheckBox.Checked = activate;
            }

            public void SetART(double art)
            {
                if (art < 0 || art > 1)
                    return;
                _art = art;
                // Update the control
                UpdateControl();
                _ParentFilteringTools?.ApplyFilters(false);
            }
            public void SetHT(double ht)
            {
                if (ht < 0 || ht > 1)
                    return;
                _ht = ht;
                // Update the control
                UpdateControl();
                _ParentFilteringTools?.ApplyFilters(false);
            }
            public void SetTDT(double tdt)
            {
                if (tdt < 0 || tdt > 1)
                    return;
                _tdt = tdt;
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
            public void ActivateTDT(bool activate)
            {
                _activateTangentDeviationScan = activate;
                // Update the control
                UpdateControl();
                _ParentFilteringTools?.ApplyFilters(false);
            }

            public void UpdateControl()
            {
                if (_FilterControl != null)
                {
                    SignalStatesViewerUserControl peaksAnalyzer = (SignalStatesViewerUserControl)_FilterControl;
                    if (!_ignoreEvent)
                    {
                        _ignoreEvent = true;
                        peaksAnalyzer.amplitudeThresholdScrollBar.Value = peaksAnalyzer.amplitudeThresholdScrollBar.GetMax() - (int)(_art * peaksAnalyzer.amplitudeThresholdScrollBar.GetMax());
                        peaksAnalyzer.artValueTextBox.Text = Math.Round(_art, 3).ToString();
                        peaksAnalyzer.hThresholdScrollBar.SetMax(_ParentFilteringTools._RawSamples.Length);
                        peaksAnalyzer.hThresholdScrollBar.Value = (int)(_ht * peaksAnalyzer.hThresholdScrollBar.GetMax());
                        peaksAnalyzer.htValueTextBox.Text = Math.Round(_ht * peaksAnalyzer.hThresholdScrollBar.GetMax() / (double)_ParentFilteringTools._samplingRate, 3).ToString();
                        peaksAnalyzer.tdtThresholdScrollBar.Value = (int)(_tdt * peaksAnalyzer.tdtThresholdScrollBar.GetMax());
                        peaksAnalyzer.tdtValueTextBox.Text = Math.Round(_tdt * 100, 3).ToString();
                        peaksAnalyzer.autoApplyCheckBox.Checked = _autoApply;
                        peaksAnalyzer.showDeviationCheckBox.Checked = _activateTangentDeviationScan;
                        _ignoreEvent = false;
                    }
                }
            }

            public void ScanPeaks(double[] samples)
            {
                Dictionary<string, List<State>> statesDIc = GeneralTools.scanPeaks(samples, _art, (int)(_ht * samples.Length), _tdt, _ParentFilteringTools._samplingRate, _activateTangentDeviationScan);
                _StatesDIc = statesDIc;
                // Check if dwt filter exists
                if (_DWT != null)
                    if (_ParentFilteringTools._FiltersDic.ContainsKey(_DWT.Name))
                        if (!_DWTLevelsStatesDIc.ContainsKey(_DWT._selectedLevel))
                            _DWTLevelsStatesDIc.Add(_DWT._selectedLevel, statesDIc);
                        else
                            _DWTLevelsStatesDIc[_DWT._selectedLevel] = statesDIc;
            }
        }

        //______________________________________________________________________//
        //::::::::::::::::::::::::::Corners scanner::::::::::::::::::::::::::::://
        public partial class CornersScanner : FilterBase
        {
            public class CornerSample
            {
                public int _index;
                public double _value;

                public double _prevTan;
                public double _nextTan;
                public double _deviationAngle; // Argument

                public double _prevMag;
                public double _nextMag;

                public CornerSample Clone()
                {
                    return new CornerSample() 
                    {   _index = _index,
                        _value = _value,
                        _prevTan = _prevTan,
                        _nextTan = _nextTan,
                        _deviationAngle = _deviationAngle,
                        _prevMag = _prevMag,
                        _nextMag = _nextMag
                    };
                }
            }

            public class CornerInterval
            {
                public string Name;

                public int starting;
                public int ending;

                public int cornerIndex;
            }

            private double[] SpanSamples;

            private int _scanStartingIndex { get; set; } = 0;

            public bool _autoApply { get; set; } = true;
            public bool _showAngles { get; set; } = false;

            private bool _forSelectionBubbles = false;
            public delegate void SelectAllTypesPoints();
            public SelectAllTypesPoints _SelectAllTypesPointsDelegate;

            public double _art { get; set; } = 0.2; // Amplitude ratio threshold
            public double _at { get; set; } = 20; // Angle threshold

            public List<CornerSample> _CornersList { get; set; }

            public CornersScanner(FilteringTools parentFilteringTools)
            {
                _ParentFilteringTools = parentFilteringTools;
                Name = GetType().Name;
            }
            public override Control InitializeFilterControl()
            {
                return new CornersScannerUserControl(this);
            }
            public override (double[] filteredSignal, bool reloadSignal) ApplyFilter(double[] filteredSamples, bool forceApply, bool showResultsInChart)
            {
                if (this._autoApply || forceApply)
                {
                    if (_forSelectionBubbles && SpanSamples != null)
                    {
                        this.ScanCorners(SpanSamples);
                        _SelectAllTypesPointsDelegate();
                    }
                    else if (!_forSelectionBubbles)
                    {
                        this.ScanCorners(filteredSamples);
                        // Show states in the chart
                        if (this._FilterControl != null && showResultsInChart)
                            if (this._FilterControl.IsHandleCreated) ((CornersScannerUserControl)this._FilterControl).showSignalCorners(this._CornersList);
                    }
                }
                return (filteredSamples, false);
            }
            public override void Activate(bool activate)
            {
                ((CornersScannerUserControl)_FilterControl).showCornersCheckBox.Checked = activate;
            }

            public void SetART(double art)
            {
                if (art < 0 || art > 1)
                    return;
                _art = art;
                // Update the control
                UpdateControl();
                _ParentFilteringTools?.ApplyFilters(false);
            }
            public void SetAT(double at)
            {
                if (at < 0 || at > 360)
                    return;
                _at = at;
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
            public void ShowAngles(bool show)
            {
                _showAngles = show;
                // Update the control
                UpdateControl();
                _ParentFilteringTools?.ApplyFilters(false);
            }
            public void SetScanStartIndex(int startingIndex)
            {
                _scanStartingIndex = startingIndex;
            }
            public void SetSpanSamples(double[] spanSamples)
            {
                SpanSamples = spanSamples;
                _ParentFilteringTools?.ApplyFilters(false);
            }
            public void SetForSelectionBubbles(bool activate, SelectAllTypesPoints selectAllTypesPointsDelegate)
            {
                _forSelectionBubbles = activate;
                _SelectAllTypesPointsDelegate = selectAllTypesPointsDelegate;
            }

            public void UpdateControl()
            {
                if (_FilterControl != null)
                {
                    CornersScannerUserControl cornersScanner = (CornersScannerUserControl)_FilterControl;
                    if (!_ignoreEvent)
                    {
                        _ignoreEvent = true;
                        cornersScanner.artScrollBar.Value = cornersScanner.artScrollBar.GetMax() - (int)(_art * cornersScanner.artScrollBar.GetMax());
                        cornersScanner.artValueTextBox.Text = Math.Round(_art, 3).ToString();
                        cornersScanner.angleThresholdScrollBar.Value = (int)(_at * 10);
                        cornersScanner.atValueTextBox.Text = Math.Round(_at, 2).ToString();
                        cornersScanner.autoApplyCheckBox.Checked = _autoApply;
                        cornersScanner.showDeviationCheckBox.Checked = _showAngles;
                        _ignoreEvent = false;
                    }
                }
            }

            //------------------------------------------------------------------------------//
            private static (double mag, double tan) MagTan(CornerSample beginningSample, CornerSample endingSample, double samplingRate)
            {
                double xDiff = (endingSample._index - beginningSample._index) / samplingRate;
                double yDiff = endingSample._value - beginningSample._value;

                double mag = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
                double tan = yDiff / xDiff;

                return (mag, tan);
            }

            public List<CornerSample> ScanCorners(double[] samples)
            {
                _CornersList = ScanCorners(samples, _scanStartingIndex, (double)this._ParentFilteringTools._samplingRate, _art, _at);
                
                return _CornersList;
            }
            public static List<CornerSample> ScanCorners(double[] samples, int scanStartingIndex, double samplingRate, double art, double at)
            {
                List<CornerSample> cornersList = new List<CornerSample>();
                double amplitudeInterval = GeneralTools.amplitudeInterval(samples);

                CornerSample[] corners = new CornerSample[samples.Length];
                CornerSample latestCorner = null;


                for (int i = 0; i < samples.Length; i++)
                {
                    // Set current sample
                    // samples is an excerpt from the the _OriginalRawSamples.
                    // That's why "_scanStartingIndex" is added as the padding of the excerpt
                    corners[i] = new CornerSample { _index = scanStartingIndex + i, _value = samples[i] };

                    if (i == 0)
                        latestCorner = corners[0];

                    // Get the index of the last corner without the padding
                    int latCorShiftIndx = latestCorner._index - scanStartingIndex;

                    // Compute _prevMag and _prevTan of the current sample
                    if (i - latCorShiftIndx > 0)
                        (corners[i]._prevMag, corners[i]._prevTan) = MagTan(latestCorner, corners[i], samplingRate);

                    // Check if the current sample is two indexes ahead of the latest corner
                    if (i - latCorShiftIndx > 1)
                    {
                        // Update _nextMag, _nextTan, and _deviationAngle of the samples between the latest state and the current sample
                        for (int j = latCorShiftIndx + 1; j < i; j++)
                        {
                            (corners[j]._nextMag, corners[j]._nextTan) = MagTan(corners[j], corners[i], samplingRate);

                            corners[j]._deviationAngle = (Math.Atan(corners[j]._nextTan) - Math.Atan(corners[j]._prevTan)) * 180 / Math.PI;
                        }

                        // Select the samples with the angle deviation that exceeds angThreshold
                        // and both of _prevMeanMag and _nextMeanMag exceeds amplitudeInterval * magThreshold
                        CornerSample[] selectedSamples = corners.Select((corner, index) => (corner, index)).Where(tuple => tuple.index > latCorShiftIndx && tuple.index < i).
                                                                                               Where(tuple => tuple.corner._nextMag > amplitudeInterval * art
                                                                                               && tuple.corner._prevMag > amplitudeInterval * art
                                                                                               && Math.Abs(tuple.corner._deviationAngle) > at).
                                                                                               Select(tuple => tuple.corner).ToArray();

                        // Check if there is any selected samples that fulfills the conditions
                        if (selectedSamples.Length > 0)
                        {
                            // Select the one with the largest segments
                            cornersList.Add(selectedSamples.OrderByDescending(corner => corner._prevMag + corner._nextMag).ToArray()[0]);
                            latestCorner = cornersList[cornersList.Count - 1];

                            // If new corner is created
                            // then update all previousMag and _prevTan of the new corner's next samples
                            latCorShiftIndx = latestCorner._index - scanStartingIndex;
                            for (int j = 1; j <= i - latCorShiftIndx; j++)
                                (corners[j + latCorShiftIndx]._prevMag, corners[j + latCorShiftIndx]._prevTan) = MagTan(latestCorner, corners[j + latCorShiftIndx], samplingRate);
                        }
                    }
                }


                return cornersList;
            }

            public static List<CornerInterval> ApproximateIndexesToIntervals(AnnotationECG[] cornersIndexes, double tolerance, double[] fullSignal, int samplingRate)
            {
                // There might be two corners on the same index
                // They should have the same interval but with their unique label
                Dictionary<int, CornerInterval> intervalsDict = new Dictionary<int, CornerInterval>();
                List<CornerInterval> intervals = new List<CornerInterval>();

                for (int i = 0; i < cornersIndexes.Length; i++)
                {
                    CornerInterval indexInterval = new CornerInterval() { cornerIndex = cornersIndexes[i].GetIndexes().starting, Name = cornersIndexes[i].Name };
                    // Check if the interval of the current corner already exists
                    if (intervalsDict.ContainsKey(indexInterval.cornerIndex))
                    {
                        // If yes then just clounn the corner interval but with the different naming
                        indexInterval.starting = intervalsDict[indexInterval.cornerIndex].starting;
                        indexInterval.ending = intervalsDict[indexInterval.cornerIndex].ending;
                    }
                    else
                    {
                        // Create a new interval for the corner
                        // Get the index of the next and previous corners if exists
                        int[] prevCorIndexArray = cornersIndexes.Where(ecgAnno => ecgAnno.GetIndexes().starting < indexInterval.cornerIndex).Select(ecgAnno => ecgAnno.GetIndexes().starting).ToArray();
                        int[] nextCorIndexArray = cornersIndexes.Where(ecgAnno => ecgAnno.GetIndexes().starting > indexInterval.cornerIndex).Select(ecgAnno => ecgAnno.GetIndexes().starting).ToArray();
                        if (prevCorIndexArray.Length > 0)
                        {
                            // Compute the distance of the tolerance and find its index
                            double prevDistAmpTol = Math.Sqrt(Math.Pow((indexInterval.cornerIndex - prevCorIndexArray.Max()) / (double)samplingRate, 2) + Math.Pow(fullSignal[indexInterval.cornerIndex] - fullSignal[prevCorIndexArray.Max()], 2)) *
                                                    tolerance / 100d;
                            // Compute the starting index using prevDistAmpTol and the ending index "indexInterval.cornerIndex"
                            // by computing distances from the latest corner in prevCorIndexArray
                            double distDiff = double.PositiveInfinity;
                            for (int iCornIndex = prevCorIndexArray.Max(); iCornIndex < indexInterval.cornerIndex; iCornIndex++)
                            {
                                double iDistAmp = Math.Sqrt(Math.Pow((indexInterval.cornerIndex - iCornIndex) / (double)samplingRate, 2) + Math.Pow(fullSignal[indexInterval.cornerIndex] - fullSignal[iCornIndex], 2));
                                if (Math.Abs(iDistAmp - prevDistAmpTol) < distDiff)
                                {
                                    indexInterval.starting = iCornIndex;
                                    distDiff = Math.Abs(iDistAmp - prevDistAmpTol);
                                }
                            }
                        }
                        else
                            indexInterval.starting = indexInterval.cornerIndex - (int)(tolerance * (indexInterval.cornerIndex - 0) / 100f);

                        if (nextCorIndexArray.Length > 0)
                        {
                            // Compute the distance of the tolerance and find its index
                            double nextDistAmpTol = Math.Sqrt(Math.Pow((nextCorIndexArray.Min() - indexInterval.cornerIndex) / (double)samplingRate, 2) + Math.Pow(fullSignal[nextCorIndexArray.Min()] - fullSignal[indexInterval.cornerIndex], 2)) *
                                                    tolerance / 100d;
                            // Compute the ending index using prevDistAmpTol and the staring index "indexInterval.cornerIndex"
                            // by computing distances from the latest corner in nextCorIndexArray
                            double distDiff = double.PositiveInfinity;
                            for (int iCornIndex = indexInterval.cornerIndex + 1; iCornIndex <= nextCorIndexArray.Min(); iCornIndex++)
                            {
                                double iDistAmp = Math.Sqrt(Math.Pow((iCornIndex - indexInterval.cornerIndex) / (double)samplingRate, 2) + Math.Pow(fullSignal[iCornIndex] - fullSignal[indexInterval.cornerIndex], 2));
                                if (Math.Abs(iDistAmp - nextDistAmpTol) < distDiff)
                                {
                                    indexInterval.ending = iCornIndex;
                                    distDiff = Math.Abs(iDistAmp - nextDistAmpTol);
                                }
                            }
                        }
                        else
                            indexInterval.ending = indexInterval.cornerIndex + (int)(tolerance * ((fullSignal.Length - 1) - indexInterval.cornerIndex) / 100f);

                        // Add the new interval in the dictionary
                        intervalsDict.Add(indexInterval.cornerIndex, indexInterval);
                    }
                    // Add the corner's interval to the list
                    intervals.Add(indexInterval);
                }

                return intervals;
            }
        }

        //______________________________________________________________________//
        //::::::::::::::::::::::::Distribution display:::::::::::::::::::::::::://
        public partial class DistributionDisplay : FilterBase
        {
            public int _segmentStarting;
            public int _segmentEnding;

            public int _resolution = 100;

            public bool _autoApply { get; set; } = true;

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
}

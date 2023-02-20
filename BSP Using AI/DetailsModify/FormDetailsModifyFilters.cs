using Biological_Signal_Processing_Using_AI.DetailsModify.Filters;
using BSP_Using_AI.DetailsModify.Filters;
using BSP_Using_AI.DetailsModify.Filters.IIRFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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
            FormDetailsModify _FormDetailModify;
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

                foreach (FilterBase filter in sortedFiltersDic)
                    if (filter._activated)
                    {
                        if (filter is DCRemoval)
                            _FilteredSamples = Garage.removeDCValue(_FilteredSamples);
                        else if (filter is Normalize)
                            _FilteredSamples = Garage.normalizeSignal(_FilteredSamples);
                        else if (filter is Absolute)
                            _FilteredSamples = Garage.absoluteSignal(_FilteredSamples);
                        else if (filter is IIRFilter iirFilter)
                            iirFilter.Filter(_FilteredSamples);
                        else if (filter is DWT dwt)
                            _FilteredSamples = dwt.TransformSamples(_FilteredSamples);
                        else if (filter is PeaksAnalyzer peaksAnalyzer)
                            if (peaksAnalyzer._autoApply || forceApply)
                            {
                                peaksAnalyzer.ScanPeaks(_FilteredSamples);
                                // Show states in the chart
                                if (peaksAnalyzer._FilterControl != null && _showResultInChart)
                                    if (peaksAnalyzer._FilterControl.IsHandleCreated) ((SignalStatesViewerUserControl)peaksAnalyzer._FilterControl).showSignalStates(peaksAnalyzer._StatesDIc);
                            }
                    }

                // Show signal in the chart
                if (_FormDetailModify != null && _showResultInChart)
                    if (_FormDetailModify.IsHandleCreated)
                        _FormDetailModify.loadSignal(_FilteredSamples, _samplingRate, _startingInSec);

                _ignoreFiltering = false;
            }
        }

        public partial class FilterBase
        {
            public string Name { get; set; }
            public bool _activated { get; set; } = true;
            public int _sortOrder { get; set; }

            public FilteringTools _ParentFilteringTools { get; set; }

            public Control _FilterControl;
            public bool _ignoreEvent { get; set; } = false;

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
                // Check the type of the filter
                if (this is DCRemoval dcRemoval)
                    _FilterControl = new DCRemovalUserControl(dcRemoval);
                else if (this is Normalize normalize)
                    _FilterControl = new NormalizedSignalUserControl(normalize);
                else if (this is Absolute absolute)
                    _FilterControl = new AbsoluteSignalUserControl(absolute);
                else if (this is ExistanceDeclare existanceDeclare)
                    _FilterControl = new CheckExistanceUserControl(existanceDeclare);
                else if (this is IIRFilter iirFilter)
                    _FilterControl = new IIRFilterUserControl(iirFilter);
                else if (this is DWT dwt)
                {
                    _FilterControl = new DWTUserControl(dwt);
                    // Check if _FiltersDic in _ParentFilteringTools contains PeaksAnalyzer
                    foreach (FilterBase filter in _ParentFilteringTools._FiltersDic.Values)
                        if (filter is PeaksAnalyzer peaksAnalyzer)
                            peaksAnalyzer._DWT = dwt;
                }
                else if (this is PeaksAnalyzer peaksAnalyzer)
                {
                    _FilterControl = new SignalStatesViewerUserControl(peaksAnalyzer);
                    // Check if _FiltersDic in _ParentFilteringTools contains dwt filter
                    foreach (FilterBase filter in _ParentFilteringTools._FiltersDic.Values)
                        if (filter is DWT dWT)
                            peaksAnalyzer._DWT = dWT;
                }

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
                    if (this is DCRemoval)
                        ((DCRemovalUserControl)_FilterControl).dcValueRemoveCheckBox.Checked = _activated;
                    else if (this is Normalize)
                        ((NormalizedSignalUserControl)_FilterControl).normalizeSignalCheckBox.Checked = _activated;
                    else if (this is Absolute)
                        ((AbsoluteSignalUserControl)_FilterControl).absoluteSignalCheckBox.Checked = _activated;
                    else if (this is PeaksAnalyzer)
                        ((SignalStatesViewerUserControl)_FilterControl).showStatesCheckBox.Checked = _activated;
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
                _DWTLevelsSamples = Garage.calculateDWT(samples, _SelectedWavelet, _maxLevel);
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
                Dictionary<string, List<State>> statesDIc = Garage.scanPeaks(samples, _art, (int)(_ht * samples.Length), _tdt, _ParentFilteringTools._samplingRate, _activateTangentDeviationScan);
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
    }
}

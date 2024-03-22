using Biological_Signal_Processing_Using_AI.DetailsModify.FiltersControls;
using BSP_Using_AI.DetailsModify.Filters;
using BSP_Using_AI.DetailsModify.Filters.IIRFilters;
using BSP_Using_AI.DetailsModify.FiltersControls;
using System.Collections.Generic;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.DetailsModify
{
    public partial class FormDetailsModify
    {
        public partial class FilteringTools
        {
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
                    if (_FiltersDic[filterName] is DCRemoval dcRemoval)
                        clonedFilteringTools._FiltersDic.Add(filterName, dcRemoval.Clone(clonedFilteringTools));
                    else if (_FiltersDic[filterName] is Normalize normalize)
                        clonedFilteringTools._FiltersDic.Add(filterName, normalize.Clone(clonedFilteringTools));
                    else if (_FiltersDic[filterName] is Absolute absolute)
                        clonedFilteringTools._FiltersDic.Add(filterName, absolute.Clone(clonedFilteringTools));
                    else if (_FiltersDic[filterName] is IIRFilter iirFilter)
                        clonedFilteringTools._FiltersDic.Add(filterName, iirFilter.Clone(clonedFilteringTools));
                    else if (_FiltersDic[filterName] is DWT dwt)
                        clonedFilteringTools._FiltersDic.Add(filterName, dwt.Clone(clonedFilteringTools));
                    else if (_FiltersDic[filterName] is PeaksAnalyzer peaksAnalyzer)
                        clonedFilteringTools._FiltersDic.Add(filterName, peaksAnalyzer.Clone(clonedFilteringTools));
                clonedFilteringTools.SetAutoApply(true);
                // Clone other properties
                clonedFilteringTools._autoApply = _autoApply;
                clonedFilteringTools._ignoreFiltering = _ignoreFiltering;
                clonedFilteringTools._showResultInChart = _showResultInChart;
                clonedFilteringTools._ignoreEvent = _ignoreEvent;

                return clonedFilteringTools;
            }
        }

        public partial class FilterBase
        {
            public void CloneBase(FilterBase sourceFilter)
            {
                // Clone filter basic properties
                Name = sourceFilter.Name;
                _activated = sourceFilter._activated;
                _sortOrder = sourceFilter._sortOrder;
                _ignoreEvent = sourceFilter._ignoreEvent;
            }
        }

        //______________________________________________________________________//
        //:::::::::::::::::::::::::::::DC removal::::::::::::::::::::::::::::::://
        public partial class DCRemoval : FilterBase
        {
            public DCRemoval Clone(FilteringTools filteringTools)
            {
                // Clone filter properties
                DCRemoval clonedDCRemoval = new DCRemoval(filteringTools);
                clonedDCRemoval.CloneBase(this);
                // CLone the control
                if (_FilterControl != null)
                {
                    clonedDCRemoval._FilterControl = new DCRemovalUserControl(clonedDCRemoval);
                    clonedDCRemoval.ActivateGenerally(_activated);
                }
                return clonedDCRemoval;
            }
        }
        //______________________________________________________________________//
        //::::::::::::::::::::::::::::::Normalize::::::::::::::::::::::::::::::://
        public partial class Normalize : FilterBase
        {
            public Normalize Clone(FilteringTools filteringTools)
            {
                // Clone filter properties
                Normalize clonedNormalize = new Normalize(filteringTools);
                clonedNormalize.CloneBase(this);
                // CLone the control
                if (_FilterControl != null)
                {
                    clonedNormalize._FilterControl = new NormalizedSignalUserControl(clonedNormalize);
                    clonedNormalize.ActivateGenerally(_activated);
                }
                return clonedNormalize;
            }
        }
        //______________________________________________________________________//
        //:::::::::::::::::::::::::::::::Absolute::::::::::::::::::::::::::::::://
        public partial class Absolute : FilterBase
        {
            public Absolute Clone(FilteringTools filteringTools)
            {
                // Clone filter properties
                Absolute clonedAbsolute = new Absolute(filteringTools);
                clonedAbsolute.CloneBase(this);
                // CLone the control
                if (_FilterControl != null)
                {
                    clonedAbsolute._FilterControl = new AbsoluteSignalUserControl(clonedAbsolute);
                    clonedAbsolute.ActivateGenerally(_activated);
                }
                return clonedAbsolute;
            }
        }
        //______________________________________________________________________//
        //:::::::::::::::::::::::::::::IIR filter::::::::::::::::::::::::::::::://
        public partial class IIRFilter : FilterBase
        {
            public IIRFilter Clone(FilteringTools filteringTools)
            {
                // Clone filter properties
                IIRFilter clonedIIRFilter = new IIRFilter(filteringTools, Name);
                clonedIIRFilter._selectedType = _selectedType;
                clonedIIRFilter._selectedBand = _selectedBand;
                clonedIIRFilter._order = _order;
                clonedIIRFilter._minFreq = _minFreq;
                clonedIIRFilter._normalizedFreq = _normalizedFreq;
                clonedIIRFilter._maxFreq = _maxFreq;
                clonedIIRFilter.CloneBase(this);
                // CLone the control
                if (_FilterControl != null)
                {
                    clonedIIRFilter._FilterControl = new IIRFilterUserControl(clonedIIRFilter);
                    clonedIIRFilter.SetFilterBand((int)_selectedBand);
                    clonedIIRFilter.SetOrder(_order);
                    clonedIIRFilter.SetMinFreq(_minFreq);
                    clonedIIRFilter.SetMaxFreq(_maxFreq);
                    clonedIIRFilter.SetNormalizedFreq(_normalizedFreq);
                }
                return clonedIIRFilter;
            }
        }
        //______________________________________________________________________//
        //:::::::::::::::::::::::::::::::::DWT:::::::::::::::::::::::::::::::::://
        public partial class DWT : FilterBase
        {
            public DWT Clone(FilteringTools filteringTools)
            {
                // Clone filter properties
                DWT clonedDWT = new DWT(filteringTools);
                clonedDWT._waveletType = _waveletType;
                clonedDWT._DWTLevelsSamples = new List<double[]>(_DWTLevelsSamples.Count);
                foreach (double[] levelSamples in _DWTLevelsSamples)
                    clonedDWT._DWTLevelsSamples.Add((double[])levelSamples.Clone());
                clonedDWT._SelectedWavelet = _SelectedWavelet;
                clonedDWT._maxLevel = _maxLevel;
                clonedDWT._selectedLevel = _selectedLevel;
                clonedDWT.CloneBase(this);
                // CLone the control
                if (_FilterControl != null)
                {
                    clonedDWT._FilterControl = new DWTUserControl(clonedDWT);
                    // Check if _FiltersDic in _ParentFilteringTools contains PeaksAnalyzer
                    foreach (FilterBase filter in clonedDWT._ParentFilteringTools._FiltersDic.Values)
                        if (filter is PeaksAnalyzer peaksAnalyzer)
                            peaksAnalyzer._DWT = clonedDWT;
                    clonedDWT.SetWaveletType(_waveletType);
                    ((DWTUserControl)clonedDWT._FilterControl).numOfVanMoComboBox.SelectedIndex = ((DWTUserControl)_FilterControl).numOfVanMoComboBox.SelectedIndex;
                    clonedDWT.SelectLevel(_selectedLevel);
                }
                return clonedDWT;
            }
        }
        //______________________________________________________________________//
        //::::::::::::::::::::::::::Peaks analyzer:::::::::::::::::::::::::::::://
        public partial class PeaksAnalyzer : FilterBase
        {
            public PeaksAnalyzer Clone(FilteringTools filteringTools)
            {
                // Clone filter properties
                PeaksAnalyzer clonedPeaksAnalyzer = new PeaksAnalyzer(filteringTools);
                clonedPeaksAnalyzer._art = _art;
                clonedPeaksAnalyzer._ht = _ht;
                clonedPeaksAnalyzer._tdt = _tdt;
                clonedPeaksAnalyzer._autoApply = _autoApply;
                clonedPeaksAnalyzer._activateTangentDeviationScan = _activateTangentDeviationScan;
                clonedPeaksAnalyzer._StatesDIc = new Dictionary<string, List<State>>(_StatesDIc.Count);
                foreach (string peaksLabel in _StatesDIc.Keys)
                {
                    List<State> states = new List<State>();
                    foreach (State state in _StatesDIc[peaksLabel])
                        states.Add(state.Clone());
                    clonedPeaksAnalyzer._StatesDIc.Add(peaksLabel, states);
                }
                clonedPeaksAnalyzer._DWTLevelsStatesDIc = new Dictionary<int, Dictionary<string, List<State>>>(_DWTLevelsStatesDIc.Count);
                foreach (int levelIndex in _DWTLevelsStatesDIc.Keys)
                {
                    Dictionary<string, List<State>> statesDic = new Dictionary<string, List<State>>(_DWTLevelsStatesDIc[levelIndex].Count);
                    foreach (string peaksLabel in _DWTLevelsStatesDIc[levelIndex].Keys)
                    {
                        List<State> states = new List<State>();
                        foreach (State state in _DWTLevelsStatesDIc[levelIndex][peaksLabel])
                            states.Add(state.Clone());
                        statesDic.Add(peaksLabel, states);
                    }
                    clonedPeaksAnalyzer._DWTLevelsStatesDIc.Add(levelIndex, statesDic);
                }
                clonedPeaksAnalyzer.CloneBase(this);
                // CLone the control
                if (_FilterControl != null)
                {
                    clonedPeaksAnalyzer._FilterControl = new SignalStatesViewerUserControl(clonedPeaksAnalyzer);
                    // Check if _FiltersDic in _ParentFilteringTools contains dwt filter
                    foreach (FilterBase filter in clonedPeaksAnalyzer._ParentFilteringTools._FiltersDic.Values)
                        if (filter is DWT dwt)
                            clonedPeaksAnalyzer._DWT = dwt;
                    clonedPeaksAnalyzer.ActivateAutoApply(_autoApply);
                    clonedPeaksAnalyzer.ActivateTDT(_activateTangentDeviationScan);
                    clonedPeaksAnalyzer.SetART(_art);
                    clonedPeaksAnalyzer.SetHT(_ht);
                    clonedPeaksAnalyzer.SetTDT(_tdt);
                    clonedPeaksAnalyzer.ActivateGenerally(_activated);
                }
                return clonedPeaksAnalyzer;
            }
        }
    }
}

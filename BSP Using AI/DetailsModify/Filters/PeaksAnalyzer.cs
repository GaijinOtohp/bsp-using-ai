using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.DetailsModify.FiltersControls;
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
    public class PeaksAnalyzer : FilterBase
    {
        public double _art { get; set; } = 0.2;
        public double _ht { get; set; } = 0.01;
        public double _tdt { get; set; } = 0.1;
        public bool _autoApply { get; set; } = true;
        public bool _activateTangentDeviationScan { get; set; } = false;
        public Dictionary<string, List<State>> _StatesDIc { get; set; }

        public Dictionary<int, Dictionary<string, List<State>>> _DWTLevelsStatesDIc { get; set; } = new Dictionary<int, Dictionary<string, List<State>>>();
        public DWT _DWT { get; set; }

        public override PeaksAnalyzer Clone(FilteringTools filteringTools)
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
}

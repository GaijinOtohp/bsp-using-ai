using Biological_Signal_Processing_Using_AI.DetailsModify.FiltersControls;
using Biological_Signal_Processing_Using_AI.Garage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace Biological_Signal_Processing_Using_AI.DetailsModify.Filters
{
    public class DWT : FilterBase
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

        public override DWT Clone(FilteringTools filteringTools)
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
}

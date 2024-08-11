using BSP_Using_AI.DetailsModify.Filters.IIRFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace Biological_Signal_Processing_Using_AI.DetailsModify.Filters
{
    public class IIRFilter : FilterBase
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

        public override IIRFilter Clone(FilteringTools filteringTools)
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
}

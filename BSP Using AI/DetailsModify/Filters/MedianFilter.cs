using Biological_Signal_Processing_Using_AI.DetailsModify.FiltersControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace Biological_Signal_Processing_Using_AI.DetailsModify.Filters
{
    public class MedianFilter : FilterBase
    {
        public int _windowSize = 3;
        public int _strideSize = 1;

        public override MedianFilter Clone(FilteringTools filteringTools)
        {
            // Clone filter properties
            MedianFilter clonedMedianFilter = new MedianFilter(filteringTools);
            clonedMedianFilter._windowSize = _windowSize;
            clonedMedianFilter._strideSize = _strideSize;
            clonedMedianFilter.CloneBase(this);
            // CLone the control
            if (_FilterControl != null)
            {
                clonedMedianFilter._FilterControl = new MedianFilterUserControl(clonedMedianFilter);
                clonedMedianFilter.ActivateGenerally(_activated);
            }
            return clonedMedianFilter;
        }

        public MedianFilter(FilteringTools parentFilteringTools)
        {
            _ParentFilteringTools = parentFilteringTools;
            Name = GetType().Name;
        }
        public override Control InitializeFilterControl()
        {
            return new MedianFilterUserControl(this);
        }
        public override (double[] filteredSignal, bool reloadSignal) ApplyFilter(double[] filteredSamples, bool forceApply, bool showResultsInChart)
        {
            filteredSamples = ApplyMedianFilter(filteredSamples, _windowSize, _strideSize);
            return (filteredSamples, true);
        }
        public override void Activate(bool activate)
        {
        }

        public void SetWindowSize(int windowSize)
        {
            if (windowSize == 0)
                return;
            _windowSize = windowSize;
            // Update the control
            UpdateControl();
            _ParentFilteringTools?.ApplyFilters(false);
        }
        public void SetStrideSize(int strideSize)
        {
            if (strideSize == 0)
                return;
            _strideSize = strideSize;
            // Update the control
            UpdateControl();
            _ParentFilteringTools?.ApplyFilters(false);
        }

        public void UpdateControl()
        {
            if (_FilterControl != null)
            {
                MedianFilterUserControl medianFilter = (MedianFilterUserControl)_FilterControl;
                if (!_ignoreEvent)
                {
                    _ignoreEvent = true;
                    medianFilter.windowSizeTextBox.Text = _windowSize.ToString();
                    medianFilter.strideSizeTextBox.Text = _strideSize.ToString();
                    _ignoreEvent = false;
                }
            }
        }

        public static double[] ApplyMedianFilter(double[] signalSamples, int windowSize = 3, int strideSize = 1)
        {
            int newSamplesCount = signalSamples.Length / strideSize + (signalSamples.Length % strideSize > 0 ? 1 : 0);
            double[] filteredSamples = new double[newSamplesCount];

            for (int iMedian = 0; iMedian < filteredSamples.Length; iMedian++)
            {
                double[] windowSamples = signalSamples.Where((value, index) => ((iMedian * strideSize) - (windowSize / 2)) <= index
                                                                                 && index <= ((iMedian * strideSize) + (windowSize - 1 - (windowSize / 2)))).ToArray();
                int windowMid = windowSamples.Length / 2;
                Array.Sort(windowSamples);

                double median = windowSamples[windowMid];

                filteredSamples[iMedian] = median;
            }

            return filteredSamples;
        }
    }
}

using Biological_Signal_Processing_Using_AI.DetailsModify.Filters;
using System;
using System.Windows.Forms;

namespace BSP_Using_AI.DetailsModify.Filters.IIRFilters
{
    public partial class IIRFilterUserControl : UserControl
    {
        IIRFilter Filter;

        public IIRFilterUserControl(IIRFilter filter)
        {
            InitializeComponent();

            Filter = filter;

            Filter._ParentFilteringTools.SetAutoApply(false);
            Name = filter.Name;
            nameFilterLabel.Text = filter.Name;
            filterBandComboBox.SelectedIndex = 0;
            Filter._ParentFilteringTools.SetAutoApply(true);

            // Set the maximum frequency and selected frequency
            maxFreqTextBox.Text = (Filter._ParentFilteringTools._samplingRate / 2).ToString();
            cutoffFreqLabel.Text = "Cutoff freq: " + maxFreqTextBox.Text + " Hz";
        }

        private void filterTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Set the filter type value to the combo box index in _filtersHashtable
            if (!Filter._ignoreEvent)
            {
                Filter._ignoreEvent = true;
                Filter.SetFilterBand(filterBandComboBox.SelectedIndex);
                Filter._ignoreEvent = false;
            }
        }

        private void orderTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Accept only numbers
            EventHandlers.keypressNumbersAndDecimalOnly(sender, e);
        }

        private void orderTextBox_TextChanged(object sender, EventArgs e)
        {
            // Set the filter order value in _filtersHashtable
            if (!Filter._ignoreEvent)
            {
                Filter._ignoreEvent = true;
                int order = 1;
                if (orderTextBox.Text.Length > 0 && !orderTextBox.Text.Equals("."))
                    order = int.Parse(orderTextBox.Text);
                Filter.SetOrder(order);
                Filter._ignoreEvent = false;
            }
        }

        private void minFreqTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Accept only numbers
            EventHandlers.keypressNumbersAndDecimalOnly(sender, e);
        }

        private void minFreqTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!Filter._ignoreEvent)
            {
                Filter._ignoreEvent = true;
                double minFreq = 0;
                if (minFreqTextBox.Text.Length > 0 && !minFreqTextBox.Text.Equals("."))
                    minFreq = double.Parse(minFreqTextBox.Text);
                if (Filter.SetMinFreq(minFreq))
                {
                    double cutoffFreq = Filter._normalizedFreq * Filter._ParentFilteringTools._samplingRate;
                    frequencyScrollBar.Value = (int)(((cutoffFreq - Filter._minFreq) * frequencyScrollBar.GetMax()) / (Filter._maxFreq - Filter._minFreq));
                    cutoffFreqLabel.Text = "Cutoff freq: " + Math.Round(cutoffFreq, 3) + " Hz";
                }
                Filter._ignoreEvent = false;
            }
        }

        private void maxFreqTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Accept only numbers
            EventHandlers.keypressNumbersAndDecimalOnly(sender, e);
        }

        private void maxFreqTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!Filter._ignoreEvent)
            {
                Filter._ignoreEvent = true;
                double maxFreq = 0;
                if (maxFreqTextBox.Text.Length > 0 && !maxFreqTextBox.Text.Equals("."))
                    maxFreq = double.Parse(maxFreqTextBox.Text);
                if (Filter.SetMaxFreq(maxFreq))
                {
                    double cutoffFreq = Filter._normalizedFreq * Filter._ParentFilteringTools._samplingRate;
                    frequencyScrollBar.Value = (int)(((cutoffFreq - Filter._minFreq) * frequencyScrollBar.GetMax()) / (Filter._maxFreq - Filter._minFreq));
                    cutoffFreqLabel.Text = "Cutoff freq: " + Math.Round(cutoffFreq, 3) + " Hz";
                }
                Filter._ignoreEvent = false;
            }
        }

        private void frequencyScrollBar_ValueChanged(object sender, EventArgs e)
        {
            if (!Filter._ignoreEvent)
            {
                Filter._ignoreEvent = true;
                // Set cutoffFreqLabel to "Cutoff freq: value"
                double cutoffFreq = (frequencyScrollBar.Value * (Filter._maxFreq - Filter._minFreq) / frequencyScrollBar.GetMax()) + Filter._minFreq;
                cutoffFreqLabel.Text = "Cutoff freq: " + cutoffFreq + " Hz";
                Filter.SetNormalizedFreq(cutoffFreq / Filter._ParentFilteringTools._samplingRate);
                Filter._ignoreEvent = false;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filter.RemoveFilter();
        }
    }
}

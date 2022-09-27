using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BSP_Using_AI.DetailsModify.Filters.IIRFilters
{
    public partial class IIRFilterUserControl : UserControl
    {
        public IIRFilterUserControl(double samplingRate)
        {
            InitializeComponent();

            // Set the selectd index of combobox to low pass
            filterTypeComboBox.SelectedIndex = 0;

            // Set the maximum frequency and selected frequency
            maxFreqTextBox.Text = (samplingRate / 2).ToString();
            cutoffFreqLabel.Text = "Cutoff freq: " + maxFreqTextBox.Text + "Hz";
        }

        private void filterTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Set the filter type value to the combo box index in _filtersHashtable
            if (Parent != null)
                // Set the type of the filter according to its name
                ((object[])((FormDetailsModify)this.FindForm())._filteresHashtable[this.Name])[1] = (sender as ComboBox).SelectedIndex;
        }

        private void orderTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Accept only numbers
            EventHandlers.textBoxNumberOnly(sender, e);
        }

        private void orderTextBox_TextChanged(object sender, EventArgs e)
        {
            // Set the filter order value in _filtersHashtable
            if (!orderTextBox.Text.Equals("") && !orderTextBox.Text.Equals(".") && !orderTextBox.Text.Equals("0"))
                ((object[])((FormDetailsModify)this.FindForm())._filteresHashtable[this.Name])[2] = int.Parse(orderTextBox.Text);
        }

        private void minFreqTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Accept only numbers
            EventHandlers.textBoxNumberOnly(sender, e);
        }

        private void minFreqTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!minFreqTextBox.Text.Equals("") && !minFreqTextBox.Text.Equals("."))
                frequencyScrollBar.Minimum = int.Parse(minFreqTextBox.Text);
        }

        private void maxFreqTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Accept only numbers
            EventHandlers.textBoxNumberOnly(sender, e);
        }

        private void maxFreqTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!maxFreqTextBox.Text.Equals("") && !maxFreqTextBox.Text.Equals("."))
                frequencyScrollBar.Maximum = int.Parse(maxFreqTextBox.Text) + frequencyScrollBar.LargeChange - frequencyScrollBar.SmallChange;
        }

        private void frequencyScrollBar_ValueChanged(object sender, EventArgs e)
        {
            // Set cutoffFreqLabel to "Cutoff freq: value"
            cutoffFreqLabel.Text = "Cutoff freq: " + frequencyScrollBar.Value + "Hz";

            // Get sampling rate
            double samplingRate = ((FormDetailsModify)this.FindForm())._samplingRate;

            // normalize frequency onto [0, 0.5] range
            if (samplingRate != 0D)
            {
                double normalizedFreq = frequencyScrollBar.Value / samplingRate;

                // Check that cut off frequency is at least less than half of the sampling frequency
                if (normalizedFreq < 0.5D)
                {
                    // If yes then set the normalized frequency in the filter object
                    ((object[])((FormDetailsModify)this.FindForm())._filteresHashtable[this.Name])[3] = normalizedFreq;
                }
                else
                {
                    // If yes then set the frequencyScrollBar to the maximum allowed frequency
                    frequencyScrollBar.Value = (int)(0.5D * samplingRate);
                }

                // Check if the auto apply is checked
                if (((FormDetailsModify)this.FindForm()).autoApplyCheckBox.Checked)
                    // If yes then apply the changes
                    ((FormDetailsModify)this.FindForm()).applyButton_Click(null, null);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EventHandlers.deleteToolStripMenuItem_Click(this);
        }

        private void applyAfterTransformCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Check if the box is checked or not
            if ((sender as CheckBox).Checked)
                // If yes then the box is checked
                // Set the filter value to 1 in _filtersHashtable
                ((object[])((FormDetailsModify)this.FindForm())._filteresHashtable[this.Name])[4] = 1;
            else
                // Set the filter value to 0
                ((object[])((FormDetailsModify)this.FindForm())._filteresHashtable[this.Name])[4] = 0;

            // Check if the auto apply is checked
            if (((FormDetailsModify)this.FindForm()).autoApplyCheckBox.Checked)
                // If yes then apply the changes
                ((FormDetailsModify)this.FindForm()).applyButton_Click(null, null);
        }
    }
}

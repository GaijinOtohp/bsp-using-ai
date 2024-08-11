using Biological_Signal_Processing_Using_AI.DetailsModify.Filters;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Filters.DWT;

namespace Biological_Signal_Processing_Using_AI.DetailsModify.FiltersControls
{
    public partial class DWTUserControl : UserControl
    {
        DWT _dwt;

        public DWTUserControl(DWT dwt)
        {
            InitializeComponent();
            _dwt = dwt;

            _dwt._ParentFilteringTools.SetAutoApply(false);
            Name = dwt.Name;
            waveletTypeComboBox.SelectedIndex = 0;
            _dwt._ParentFilteringTools.SetAutoApply(true);
        }

        private void waveletTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Set numOfVanMoComboBox according to the selected wavelet
            List<string> typesOfWavelets = null;
            switch ((WaveletType)waveletTypeComboBox.SelectedIndex)
            {
                case WaveletType.Haar:
                    // This is Haar wavelet
                    // set the different types of vanishing moments
                    typesOfWavelets = new List<string>(1);
                    typesOfWavelets.Add("haar");
                    break;
                case WaveletType.Daubechies:
                    // This is Daubechies
                    // set the different types of vanishing moments
                    typesOfWavelets = new List<string>(20);
                    for (int i = 1; i <= 20; i++)
                        typesOfWavelets.Add(i + ": db" + i);
                    break;
                case WaveletType.Symlet:
                    // This is Symlet
                    // set the different types of vanishing moments
                    typesOfWavelets = new List<string>(19);
                    for (int i = 2; i <= 20; i++)
                        typesOfWavelets.Add(i + ": sym" + i);
                    break;
                case WaveletType.Coiflet:
                    // This is Coiflet
                    // set the different types of vanishing moments
                    typesOfWavelets = new List<string>(5);
                    for (int i = 1; i <= 5; i++)
                        typesOfWavelets.Add(i + ": coif" + i);
                    break;
            }
            // Add new data source in numOfVanMoComboBox
            numOfVanMoComboBox.DataSource = typesOfWavelets;
            numOfVanMoComboBox.SelectedIndex = 0;

            // Update _dwt
            if (!_dwt._ignoreEvent)
            {
                _dwt._ignoreEvent = true;
                _dwt.SetWaveletType((WaveletType)waveletTypeComboBox.SelectedIndex);
                _dwt._ignoreEvent = false;
            }
        }

        private void numOfVanMoComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update _dwt
            if (!_dwt._ignoreEvent)
            {
                _dwt._ignoreEvent = true;
                if (_dwt._waveletType == WaveletType.Daubechies || _dwt._waveletType == WaveletType.Coiflet)
                    _dwt.SetNumberOfVanishingMoments(numOfVanMoComboBox.SelectedIndex + 1); // Daubechies and Coiflet starts from 1 vanishing moment
                else if (_dwt._waveletType == WaveletType.Symlet)
                    _dwt.SetNumberOfVanishingMoments(numOfVanMoComboBox.SelectedIndex + 2); // Symlet starts from 2 vanishing moments
                _dwt._ignoreEvent = false;
            }
        }

        private void levelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update _dwt
            if (!_dwt._ignoreEvent)
            {
                _dwt._ignoreEvent = true;
                _dwt.SelectLevel(levelComboBox.SelectedIndex);
                _dwt._ignoreEvent = false;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _dwt.RemoveFilter();
        }
    }
}

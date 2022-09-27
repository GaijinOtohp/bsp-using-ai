using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BSP_Using_AI.DetailsModify.Transforms.DWT
{
    public partial class FormDWT : Form
    {
        FormDetailsModify _formDetailsModify = null;

        public FormDWT(FormDetailsModify formDetailsModify)
        {
            InitializeComponent();

            // Get current wavelet specs and set it in this form
            waveletTypeComboBox.SelectedIndex = (int)formDetailsModify._dwtSpecs[0];
            numOfVanMoComboBox.SelectedIndex = (int)formDetailsModify._dwtSpecs[1];

            // Set _formDetailsModify
            _formDetailsModify = formDetailsModify;
        }

        private void waveletTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if _formDetailsModify is not null
            if (_formDetailsModify != null)
                // If yes then set the selected index in _dwtSpecs
                _formDetailsModify._dwtSpecs[0] = waveletTypeComboBox.SelectedIndex;

            // Set numOfVanMoComboBox according to the selected wavelet
            List<String> typesOfWavelets = null;
            switch (waveletTypeComboBox.SelectedIndex)
            {
                case 0:
                    // This is Haar wavelet
                    // set the different types of vanishing moments
                    typesOfWavelets = new List<String>(1);
                    typesOfWavelets.Add("haar");
                    break;
                case 1:
                    // This is Daubechies
                    // set the different types of vanishing moments
                    typesOfWavelets = new List<String>(20);
                    for (int i = 1; i <= 20; i++)
                        typesOfWavelets.Add("db" + i.ToString());
                    break;
                case 2:
                    // This is Symlet
                    // set the different types of vanishing moments
                    typesOfWavelets = new List<String>(19);
                    for (int i = 2; i <= 20; i++)
                        typesOfWavelets.Add("sym" + i.ToString());
                    numOfVanMoComboBox.DataSource = typesOfWavelets;
                    break;
                case 3:
                    // This is Coiflet
                    // set the different types of vanishing moments
                    typesOfWavelets = new List<String>(5);
                    for (int i = 1; i <= 5; i++)
                        typesOfWavelets.Add("coif" + i.ToString());
                    numOfVanMoComboBox.DataSource = typesOfWavelets;
                    break;
            }
            // Add new data source in numOfVanMoComboBox
            numOfVanMoComboBox.DataSource = typesOfWavelets;
        }

        private void numOfVanMoComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if _formDetailsModify is not null
            if (_formDetailsModify != null)
            {
                // If yes then set the selected index in _dwtSpecs
                _formDetailsModify._dwtSpecs[1] = numOfVanMoComboBox.SelectedIndex;
                // Set the name of the wavelet
                _formDetailsModify._dwtSpecs[2] = numOfVanMoComboBox.SelectedItem.ToString();
            }
        }

        private void FormDWT_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Rerun dwt transform
            _formDetailsModify.dwtButton_Click(null, null);
        }
    }
}

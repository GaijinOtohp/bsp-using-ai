using Biological_Signal_Processing_Using_AI.Garage;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.DetailsModify
{
    public partial class FormDetailsModify
    {
        private void signalExhibitor_MouseMove_Anno(object sender, MouseEventArgs e)
        {
            // Send event to ART-HT AI tools
            signalExhibitor_MouseMove_ARTHT(sender, e);
        }

        private void signalChart_MouseClick_Anno(object sender, MouseEventArgs e)
        {
            // Send event to ART-HT AI tools
            signalChart_MouseClick_ARTHT(sender, e);
        }

        //*******************************************************************************************************//
        //*******************************************************************************************************//
        //************************************************AI TOOLS***********************************************//

        private void predictButton_Click_Anno(object sender, EventArgs e)
        {
            // Get the selected model from modelTypeComboBox
            if (modelTypeComboBox.SelectedIndex > -1)
            {
                // Disable this button and modelTypeComboBox
                predictButton.Enabled = false;
                modelTypeComboBox.Enabled = false;
                // Set prediction to true
                _predictionOn = true;

                // Send event to ART-HT AI tools
                predictButton_Click_ARTHT(sender, e);
            }
        }

        private double[] askForPrediction(double[] features, string stepName)
        {
            return null;
        }

        private void setFeaturesLabelsButton_Click_Anno(object sender, EventArgs e)
        {
            // Disable everything except ai tools
            enableAITools(true);

            // Send event to ART-HT AI tools
            setFeaturesLabelsButton_Click_ARTHT(sender, e);

            // Enable discard and next button, and disable setFeaturesLabelsButton button
            discardButton.Enabled = true;
            nextButton.Enabled = true;
            setFeaturesLabelsButton.Enabled = false;
        }

        private void discardButton_Click_Anno(object sender, EventArgs e)
        {
            // Show message for confirming the action
            DialogResult dialogResult = MessageBox.Show("Are you sure about discarding features selection?", "Action confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                // Disable save button in signal holder
                _signalHolder.saveButton.Enabled = false;
                // Enable everything except ai tools
                enableAITools(false);
                // Remove instructions
                featuresSettingInstructionsLabel.Text = "";
                // Enable setFeaturesLabelsButton, and disable other tools
                setFeaturesLabelsButton.Enabled = true;
                previousButton.Enabled = false;
                nextButton.Enabled = false;
                discardButton.Enabled = false;
                nextButton.Text = "Next";

                // Remove selected features
                _arthtFeatures.Clear();
                featuresTableLayoutPanel.Controls.Clear();

                ((ScatterPlot)_Plots[SANamings.UpPeaks]).DataPointLabelFont.Color = Color.Transparent;
                foreach (string stateName in new string[] { SANamings.UpPeaks, SANamings.DownPeaks, SANamings.StableStates, SANamings.Selection, SANamings.Labels })
                    GeneralTools.loadXYInChart(signalChart, _Plots[stateName], null, null, null, 0, "EventsHAnnotationsFormDetailsModify");

                // Reset the signal
                _FilteringTools._RawSamples = new double[_FilteringTools._OriginalRawSamples.Length];
                for (int i = 0; i < _FilteringTools._RawSamples.Length; i++)
                    _FilteringTools._RawSamples[i] = _FilteringTools._OriginalRawSamples[i];
                _FilteringTools.ApplyFilters(false);
            }
        }

        private void enableAITools(bool enable)
        {
            // Remove all filters
            _FilteringTools.RemoveAllFilters();
            // Disable auto apply to filters
            _FilteringTools.SetAutoApply(!enable);

            signalFusionButton.Enabled = !enable;
            signalsPickerComboBox.Enabled = !enable;
            filtersComboBox.Enabled = !enable;

            if (enable)
                signalsPickerComboBox.SelectedIndex = 1;
        }
    }
}

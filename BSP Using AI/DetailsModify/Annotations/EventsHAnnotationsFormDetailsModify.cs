using Biological_Signal_Processing_Using_AI.DetailsModify.Annotations;
using Biological_Signal_Processing_Using_AI.Garage;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.DetailsModify
{
    public partial class FormDetailsModify
    {
        public delegate void HorizSpan_Dragged_Delegate(object sender, EventArgs e);

        public class AnnoKeys
        {
            public bool ctrl = false;
            public bool shift = false;
            public bool h = false;
            public bool l = false;
            public bool c = false;
            public bool s = false;
        }

        public AnnoKeys _AnnoKeys = new AnnoKeys();

        private bool _intervalHorSpanDragged = false;

        public AnnotationData _AnnotationData = new AnnotationData(CharacteristicWavesDelineation.ObjectiveName);

        private void UpdatePointsAnnoPlot()
        {
            SignalPlot signalPlot = (SignalPlot)_Plots[SANamings.Signal];

            // Get the annotation data from _CWDAnnotationData
            List<AnnotationECG> annotationsList = _AnnotationData.GetAnnotations();
            double[] xValues = annotationsList.Where(anno => anno.GetAnnotationType() == AnnotationType.Point).Select(anno => anno.GetIndexes().starting / signalPlot.SampleRate).ToArray();
            double[] yValues = annotationsList.Where(anno => anno.GetAnnotationType() == AnnotationType.Point).Select(anno => signalPlot.Ys[anno.GetIndexes().starting]).ToArray();
            string[] labels = annotationsList.Where(anno => anno.GetAnnotationType() == AnnotationType.Point).Select(anno => anno.Name).ToArray();

            // Plot innotation data
            GeneralTools.loadXYInChart(signalChart, (ScatterPlot)_Plots[SANamings.ScatterPlotsNames.Labels], xValues, yValues, labels, signalPlot.OffsetX, false, "EventsHAnnotationsFormDetailsModify");
        }

        private void UpdateIntervalsAnnoPlot()
        {
            SignalPlot signalPlot = (SignalPlot)_Plots[SANamings.Signal];

            // Get the annotation data from _CWDAnnotationData
            List<AnnotationECG> annotationsList = _AnnotationData.GetAnnotations();
            double[] xValues = annotationsList.Where(anno => anno.GetAnnotationType() == AnnotationType.Interval).
                                               Select(anno => (anno.GetIndexes().starting + anno.GetIndexes().ending) / 2 / signalPlot.SampleRate).ToArray();
            double[] yValues = annotationsList.Where(anno => anno.GetAnnotationType() == AnnotationType.Interval).
                                               Select(anno =>
                                               {
                                                   double yMax = signalPlot.Ys[anno.GetIndexes().starting];
                                                   double yMin = signalPlot.Ys[anno.GetIndexes().starting];
                                                   for (int i = anno.GetIndexes().starting; i < anno.GetIndexes().ending; i++)
                                                   {
                                                       if (signalPlot.Ys[i] > yMax)
                                                           yMax = signalPlot.Ys[i];
                                                       if (signalPlot.Ys[i] < yMin)
                                                           yMin = signalPlot.Ys[i];
                                                   }
                                                   return yMin + (yMax - yMin) * 3 / 4;
                                               }
                                               ).ToArray();
            string[] labels = annotationsList.Where(anno => anno.GetAnnotationType() == AnnotationType.Interval).Select(anno => anno.Name).ToArray();

            // Add the new intervals annotations
            GeneralTools.loadXYInChart(signalChart, (ScatterPlot)_Plots[SANamings.ScatterPlotsNames.SpanAnnotations], xValues, yValues, labels, signalPlot.OffsetX, false, "EventsHAnnotationsFormDetailsModify");
        }

        public void ShowAnnotationDetails()
        {
            foreach (AnnotationECG annoECG in _AnnotationData.GetAnnotations())
                // Create the new annotation item and add it in featuresTableLayoutPanel
                featuresTableLayoutPanel.Controls.Add(new AnnotationItemUserControl(annoECG, this));

            // Update the annotations in the plots
            UpdateAnnotationsPlots();
        }

        public void UpdateAnnotationsPlots()
        {
            // Points annotations should be updated only in Characteristic waves delineation
            if (aiGoalComboBox.SelectedItem.Equals(CharacteristicWavesDelineation.ObjectiveName))
                UpdatePointsAnnoPlot();
            // Update intervals annotations
            UpdateIntervalsAnnoPlot();

            signalChart.Refresh();
        }

        //*******************************************************************************************************//
        //*******************************************************************************************************//
        //******************************************Annotation events********************************************//
        private void FormDetailsModify_KeyDown_Anno(object sender, KeyEventArgs e)
        {
            // Check if AI tool is activated
            if (discardButton.Enabled && !saveButton.Enabled)
            {
                // Check which objective is selected
                if (aiGoalComboBox.SelectedItem.Equals(CharacteristicWavesDelineation.ObjectiveName))
                    // Send event to CWD AI tools
                    FormDetailsModify_KeyDown_CWD(sender, e);
                else if (aiGoalComboBox.SelectedItem.Equals(ArrhythmiaClassification.ObjectiveName))
                    // Send event to ArrhyCla AI tools
                    FormDetailsModify_KeyDown_ArrhyCla(sender, e);
            }
        }
        private void FormDetailsModify_KeyUp_Anno(object sender, KeyEventArgs e)
        {
            // Check if AI tool is activated
            if (discardButton.Enabled && !saveButton.Enabled)
            {
                // Check which objective is selected
                if (aiGoalComboBox.SelectedItem.Equals(CharacteristicWavesDelineation.ObjectiveName))
                    // Send event to CWD AI tools
                    FormDetailsModify_KeyUp_CWD(sender, e);
                else if (aiGoalComboBox.SelectedItem.Equals(ArrhythmiaClassification.ObjectiveName))
                    // Send event to ArrhyCla AI tools
                    FormDetailsModify_KeyUp_ArrhyCla(sender, e);
            }
        }

        private void HorizSpan_Dragged(object sender, EventArgs e)
        {
            if ((sender as HSpan).Label.Equals(SANamings.PointHorizSpan))
                _pointHorSpanDragged = true;
            else if ((sender as HSpan).Label.Equals(SANamings.IntervalHorizSpan))
                _intervalHorSpanDragged = true;
        }

        private void signalChart_MouseUp_Anno(object sender, MouseEventArgs e)
        {
            // Check if AI tool is activated
            if (discardButton.Enabled && !saveButton.Enabled)
            {
                // Check which objective is selected
                if (aiGoalComboBox.SelectedItem.Equals(CharacteristicWavesDelineation.ObjectiveName))
                    // Send event to CWD AI tools
                    signalChart_MouseUp_CWD(sender, e);
            }
        }

        private void signalExhibitor_MouseMove_Anno(object sender, MouseEventArgs e)
        {
            // Check if AI tool is activated
            if (discardButton.Enabled && !saveButton.Enabled)
            {
                // Check which objective is selected
                if (aiGoalComboBox.SelectedItem.Equals(WPWSyndromeDetection.ObjectiveName))
                    // Send event to ART-HT AI tools
                    signalExhibitor_MouseMove_ARTHT(sender, e);
                else if (aiGoalComboBox.SelectedItem.Equals(CharacteristicWavesDelineation.ObjectiveName))
                    // Send event to CWD AI tools
                    signalExhibitor_MouseMove_CWD(sender, e);
            }
        }

        private void signalChart_MouseClick_Anno(object sender, MouseEventArgs e)
        {
            // Check if AI tool is activated
            if (discardButton.Enabled && !saveButton.Enabled)
            {
                // Check which objective is selected
                if (aiGoalComboBox.SelectedItem.Equals(WPWSyndromeDetection.ObjectiveName))
                    // Send event to ART-HT AI tools
                    signalChart_MouseClick_ARTHT(sender, e);
                else if (aiGoalComboBox.SelectedItem.Equals(CharacteristicWavesDelineation.ObjectiveName))
                    // Send event to CWD AI tools
                    signalChart_MouseClick_CWD(sender, e);
                else if (aiGoalComboBox.SelectedItem.Equals(ArrhythmiaClassification.ObjectiveName))
                    // Send event to ArrhyCla AI tools
                    signalChart_MouseClick_ArrhyCla(sender, e);
            }
        }

        private void nextButton_Click_Anno(object sender, EventArgs e)
        {
            // Check if AI tool is activated
            if (discardButton.Enabled)
            {
                // Check which objective is selected
                if (aiGoalComboBox.SelectedItem.Equals(WPWSyndromeDetection.ObjectiveName))
                    // Send event to ART-HT AI tools
                    nextButton_Click_ARTHT(sender, e);
                else if (aiGoalComboBox.SelectedItem.Equals(CharacteristicWavesDelineation.ObjectiveName))
                    // Send event to CWD AI tools
                    nextButton_Click_CWD(sender, e);
                else if (aiGoalComboBox.SelectedItem.Equals(ArrhythmiaClassification.ObjectiveName))
                    // Send event to ArrhyCla AI tools
                    nextButton_Click_ArrhyCla(sender, e);
            }
        }

        private void previousButton_Click_Anno(object sender, EventArgs e)
        {
            // Check if AI tool is activated
            if (discardButton.Enabled)
            {
                // Check which objective is selected
                if (aiGoalComboBox.SelectedItem.Equals(WPWSyndromeDetection.ObjectiveName))
                    // Send event to ART-HT AI tools
                    previousButton_Click_ARTHT(sender, e);
                else if (aiGoalComboBox.SelectedItem.Equals(CharacteristicWavesDelineation.ObjectiveName))
                    // Send event to CWD AI tools
                    previousButton_Click_CWD(sender, e);
                else if (aiGoalComboBox.SelectedItem.Equals(ArrhythmiaClassification.ObjectiveName))
                    // Send event to ArrhyCla AI tools
                    previousButton_Click_ArrhyCla(sender, e);
            }
        }

        private void saveButton_Click_Anno(object sender, EventArgs e)
        {
            // Check if AI tool is activated
            if (discardButton.Enabled)
            {
                // Check which objective is selected
                if (aiGoalComboBox.SelectedItem.Equals(WPWSyndromeDetection.ObjectiveName))
                    // Send event to ART-HT AI tools
                    saveButton_Click_ARTHT(sender, e);
                else if (aiGoalComboBox.SelectedItem.Equals(CharacteristicWavesDelineation.ObjectiveName))
                    // Send event to CWD AI tools
                    saveButton_Click_CWD(sender, e);
                else if (aiGoalComboBox.SelectedItem.Equals(ArrhythmiaClassification.ObjectiveName))
                    // Send event to ArrhyCla AI tools
                    saveButton_Click_ArrhyCla(sender, e);
            }
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
            SetLabelingTools();
        }

        public void SetLabelingTools()
        {
            // Disable everything except ai tools
            enableAITools(true);

            // Check which objective is selected
            if (aiGoalComboBox.SelectedItem.Equals(WPWSyndromeDetection.ObjectiveName))
                // Send event to ART-HT AI tools
                setFeaturesLabelsButton_Click_ARTHT(null, null);
            else if (aiGoalComboBox.SelectedItem.Equals(CharacteristicWavesDelineation.ObjectiveName))
                // Send event to CWD AI tools
                setFeaturesLabelsButton_Click_CWD(null, null);
            else if (aiGoalComboBox.SelectedItem.Equals(ArrhythmiaClassification.ObjectiveName))
                // Send event to ArrhyCla AI tools
                setFeaturesLabelsButton_Click_ArrhyCla(null, null);

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
                // Disable save button
                saveButton.Enabled = false;
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
                _AnnotationData.Clear();
                featuresTableLayoutPanel.Controls.Clear();

                ((ScatterPlot)_Plots[SANamings.ScatterPlotsNames.UpPeaks]).DataPointLabelFont.Color = Color.Transparent;
                foreach (PropertyInfo statesLabelProperty in typeof(SANamings.ScatterPlotsNames).GetProperties())
                    if (statesLabelProperty.GetValue(null) is string statesLabel)
                        GeneralTools.loadXYInChart(signalChart, _Plots[statesLabel], null, null, null, 0, true, "EventsHAnnotationsFormDetailsModify");

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

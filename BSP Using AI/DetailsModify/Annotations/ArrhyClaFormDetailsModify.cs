using Biological_Signal_Processing_Using_AI.DetailsModify.Annotations;
using Biological_Signal_Processing_Using_AI.Garage;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.DetailsModify
{
    public partial class FormDetailsModify
    {
        private void IntervalTimesSpanVisibility()
        {
            // Check if the interval horizontal span should be visible and it 
            if ((_AnnoKeys.s))
            {
                // Check if the interval horizontal span is not already visible
                if (!_Plots[SANamings.IntervalHorizSpan].IsVisible)
                    // Show the horizontal span
                    GeneralTools.TimeSpanVisibility(signalChart, _Plots[SANamings.IntervalHorizSpan], true);
            }
            // If not then check if it is visible
            else if (_Plots[SANamings.IntervalHorizSpan].IsVisible)
                // Hide the interval horizontal span
                GeneralTools.TimeSpanVisibility(signalChart, _Plots[SANamings.IntervalHorizSpan], false);
        }

        //*******************************************************************************************************//
        //*******************************************************************************************************//
        //******************************************Annotation events********************************************//
        private void setFeaturesLabelsButton_Click_ArrhyCla(object sender, EventArgs e)
        {
            ((HSpan)_Plots[SANamings.IntervalHorizSpan]).X1 = ((SignalPlot)_Plots[SANamings.Signal]).OffsetX;
            ((HSpan)_Plots[SANamings.IntervalHorizSpan]).X2 = ((SignalPlot)_Plots[SANamings.Signal]).OffsetX;

            // Show the instructions of annotating the signal
            featuresSettingInstructionsLabel.Text = "Press \"S\" for labeling the highlighted time span from the signal.\n" +
                                                    "Left-click for annotating the selection, and right-click for deleting the selection from the list.\n" +
                                                    "Press \"finish\" after finishing annotating the signal.";

            nextButton.Text = "Finish";
        }

        private void FormDetailsModify_KeyDown_ArrhyCla(object sender, KeyEventArgs e)
        {
            FormDetailsModify_KeyDownUp_ArrhyCla(sender, e, true);
        }
        private void FormDetailsModify_KeyUp_ArrhyCla(object sender, KeyEventArgs e)
        {
            FormDetailsModify_KeyDownUp_ArrhyCla(sender, e, false);
        }

        private void FormDetailsModify_KeyDownUp_ArrhyCla(object sender, KeyEventArgs e, bool keyDown)
        {
            // Check if which key is clicked
            if (e.KeyCode.ToString().ToUpper().Equals("S") && _AnnoKeys.s != keyDown)
                _AnnoKeys.s = keyDown; // S key is clicked (for labeling the highlighted time span from the signal)

            // Show or hide points horizontal span
            PointsTimesSpanVisibility();
            // Show or hide interval horizontal span
            IntervalTimesSpanVisibility();
        }

        private void signalChart_MouseClick_ArrhyCla(object sender, MouseEventArgs e)
        {
            // Check if the cursor is not the dragging cursor
            // and if an interval span is selected
            if ((sender as FormsPlot).Cursor != Cursors.SizeWE && _AnnoKeys.s)
            {
                // Add the annotations into _AnnotationData
                SignalPlot signalPlot = (SignalPlot)_Plots[SANamings.Signal];

                // Get the highlighted span interval
                int spanStart = signalPlot.GetPointNearestX(((HSpan)_Plots[SANamings.IntervalHorizSpan]).X1).index;
                int spanEnd = signalPlot.GetPointNearestX(((HSpan)_Plots[SANamings.IntervalHorizSpan]).X2).index;
                (spanStart, spanEnd) = spanStart < spanEnd ? (spanStart, spanEnd) : (spanEnd, spanStart);

                _AnnotationData.InsertAnnotation("label", AnnotationType.Interval, spanStart, spanEnd);

                // Add the annotations into featuresTableLayoutPanel
                AnnotationItemUserControl newAnnoItem = null;
                foreach ((AnnotationECG annoECG, int index) in _AnnotationData.GetAnnotations().Select((anno, index) => (anno, index)))
                    // Check if the selected annotation is not in featuresTableLayoutPanel controls
                    if (featuresTableLayoutPanel.Controls.OfType<AnnotationItemUserControl>().Where(annoItem => annoItem._Anno == annoECG).ToArray().Length == 0)
                    {
                        // Create the new annotation item and add it in featuresTableLayoutPanel
                        newAnnoItem = new AnnotationItemUserControl(annoECG, this);
                        featuresTableLayoutPanel.Controls.Add(newAnnoItem);
                        // Order the new item according to _AnnotationData's order
                        featuresTableLayoutPanel.Controls.SetChildIndex(newAnnoItem, index);
                    }

                // Update the annotations in the plots
                UpdateAnnotationsPlots();

                // Show annotation modify form
                AnnotationModify annotationModify = new AnnotationModify(newAnnoItem);
                annotationModify.Show();

                // Set all annotation keys to false
                foreach (FieldInfo field in _AnnoKeys.GetType().GetFields())
                    field.SetValue(_AnnoKeys, false);
            }
        }

        private void nextButton_Click_ArrhyCla(object sender, EventArgs e)
        {
            // Enable everything
            signalFusionButton.Enabled = true;
            signalsPickerComboBox.Enabled = true;
            filtersComboBox.Enabled = true;
            // Disable finish button
            nextButton.Enabled = false;
            previousButton.Enabled = true;

            // Enable save button
            saveButton.Enabled = true;
        }

        private void previousButton_Click_ArrhyCla(object sender, EventArgs e)
        {
            // Disable save button
            saveButton.Enabled = false;
            // Disable everything except ai tools
            enableAITools(true);

            // Enable next button
            nextButton.Enabled = true;
            nextButton.Text = "Next";

            // Disable previous button
            previousButton.Enabled = false;
        }

        private void saveButton_Click_ArrhyCla(object sender, EventArgs e)
        {
            // Save the signal with its features in dataset
            DbStimulator dbStimulator = new DbStimulator();
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Insert("anno_ds",
                new string[] { "sginal_name", "starting_index", "signal_data", "sampling_rate", "quantisation_step", "anno_objective", "anno_data" },
                new Object[] { pathLabel.Text, _FilteringTools._startingInSec, GeneralTools.ObjectToByteArray(_FilteringTools._OriginalRawSamples), _FilteringTools._samplingRate,
                               _FilteringTools._quantizationStep, ArrhythmiaClassification.ObjectiveName, GeneralTools.ObjectToByteArray(_AnnotationData) }, "ArrhyClaFormDetailModify"));
            dbStimulatorThread.Start();

            // Update the notification badge for unfitted signals
            Control badge = MainFormFolder.BadgeControl.GetBadge(_signalHolder.FindForm());
            badge.Text = (int.Parse(badge.Text) + 1).ToString();
            badge.Visible = true;

            // Disable save button
            saveButton.Enabled = false;
        }
    }
}

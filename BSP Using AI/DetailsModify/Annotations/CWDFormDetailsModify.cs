using Biological_Signal_Processing_Using_AI.DetailsModify.Annotations;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.DetailsModify.Filters;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.DetailsModify
{
    public partial class FormDetailsModify
    {
        private bool _pointHorSpanDragged = false;

        private int _heighestPointInTimeSpanIndex = 0;
        private int _lowestPointInTimeSpanIndex = 0;

        private List<int> _SelectedPointsList = new List<int>();

        public int _lastSelectedItem_shift = -1;

        private void SelectHishest_LowestPoint()
        {
            SignalPlot signalPlot = (SignalPlot)_Plots[SANamings.Signal];

            // Get the highlighted span interval
            int spanStart = signalPlot.GetPointNearestX(((HSpan)_Plots[SANamings.PointHorizSpan]).X1).index;
            int spanEnd = signalPlot.GetPointNearestX(((HSpan)_Plots[SANamings.PointHorizSpan]).X2).index;
            (spanStart, spanEnd) = spanStart < spanEnd ? (spanStart, spanEnd) : (spanEnd, spanStart);

            // check if H key is clicked for selecting the highest point
            if (_AnnoKeys.h)
                _heighestPointInTimeSpanIndex = signalPlot.Ys.Select((y, indx) => (y, indx)).
                                                              Where(tuple => spanStart <= tuple.indx && tuple.indx <= spanEnd).Max().indx;
            // check if L key is clicked for selecting the lowest point
            if (_AnnoKeys.l)
                _lowestPointInTimeSpanIndex = signalPlot.Ys.Select((y, indx) => (y, indx)).
                                                            Where(tuple => spanStart <= tuple.indx && tuple.indx <= spanEnd).Min().indx;

            // Show the selected points
            SelectHighlightedSpanPoints();
        }

        private void PointsTimesSpanVisibility()
        {
            // Check if the pointing horizontal span should be visible and it 
            if ((_AnnoKeys.h || _AnnoKeys.l || _AnnoKeys.d))
            {
                // Check if the pointing horizontal span is not already visible
                if (!_Plots[SANamings.PointHorizSpan].IsVisible)
                {
                    // Show the horizontal span
                    GeneralTools.TimeSpanVisibility(signalChart, _Plots[SANamings.PointHorizSpan], true);
                    // Compute the highest and lowest point in the highlighted span
                    SelectHishest_LowestPoint();
                }
            }
            // If not then check if it is visible
            else if (_Plots[SANamings.PointHorizSpan].IsVisible)
            {
                // Hide the pointing horizontal span
                GeneralTools.TimeSpanVisibility(signalChart, _Plots[SANamings.PointHorizSpan], false);

                // Clear the old selection
                ClearSelection();
                signalChart.Refresh();
            }
        }

        private void SelectHighlightedSpanPoints()
        {
            // Check if H is clicked for labeling the highest point in the highlighted time span
            if (_AnnoKeys.h)
                // If yes then insert the selection for the highest point in the highlighted span
                SelectPoint(_heighestPointInTimeSpanIndex);
            // Check if L is clicked for labeling the lowest point in the highlighted time span
            if (_AnnoKeys.l)
                // If yes then insert the selection for the lowest point in the highlighted span
                SelectPoint(_lowestPointInTimeSpanIndex);

            signalChart.Refresh();
        }

        private void SelectPoint(int index)
        {
            // Get the plots
            SignalPlot signalPlot = (SignalPlot)_Plots[SANamings.Signal];
            BubblePlot selectionBubble = (BubblePlot)_Plots[SANamings.Selection];

            // Insert the index in the temporary selected points list
            _SelectedPointsList.Add(index);

            selectionBubble.Add(index / signalPlot.SampleRate, signalPlot.Ys[index], 5, Color.Red, 2, ForeColor);
        }

        private void ClearSelection()
        {
            _SelectedPointsList.Clear();

            ((BubblePlot)_Plots[SANamings.Selection]).Clear();
        }

        //*******************************************************************************************************//
        //*******************************************************************************************************//
        //******************************************Annotation events********************************************//
        private void setFeaturesLabelsButton_Click_CWD(object sender, EventArgs e)
        {
            // Show the instructions of annotating the signal
            featuresSettingInstructionsLabel.Text = "Press \"Ctrl\" for labeling any point from the signal.\n" +
                                                    "Press \"H\" for labeling the highest point in the highlighted time span.\n" +
                                                    "Press \"L\" for labeling the lowest point in the highlighted time span.\n" +
                                                    "Press \"D\" for labeling the tangent deviation corner points in the highlighted time span.\n" +
                                                    "Press \"S\" for labeling the highlighted time span from the signal.\n" +
                                                    "Left-click for annotating the selection, and right-click for deleting the selection from the list.\n" +
                                                    "Press \"finish\" after finishing annotating the signal.";

            nextButton.Text = "Finish";
        }

        private void FormDetailsModify_KeyDown_CWD(object sender, KeyEventArgs e)
        {
            FormDetailsModify_KeyDownUp_CWD(sender, e, true);
        }
        private void FormDetailsModify_KeyUp_CWD(object sender, KeyEventArgs e)
        {
            FormDetailsModify_KeyDownUp_CWD(sender, e, false);
        }

        private void FormDetailsModify_KeyDownUp_CWD(object sender, KeyEventArgs e, bool keyDown)
        {
            // Check if which key is clicked
            if (e.KeyCode == Keys.ControlKey && _AnnoKeys.ctrl != keyDown)
                _AnnoKeys.ctrl = keyDown; // Control key is clicked (for labeling any point from the signal)
            if (e.KeyCode == Keys.ShiftKey && _AnnoKeys.shift != keyDown)
                _AnnoKeys.shift = keyDown; // Control key is clicked (for labeling any point from the signal)
            if (e.KeyCode.ToString().ToUpper().Equals("H") && _AnnoKeys.h != keyDown)
                _AnnoKeys.h = keyDown; // H key is clicked (for labeling the highest point in the highlighted time span)
            if (e.KeyCode.ToString().ToUpper().Equals("L") && _AnnoKeys.l != keyDown)
                _AnnoKeys.l = keyDown; // L key is clicked (for labeling the lowest point in the highlighted time span)
            if (e.KeyCode.ToString().ToUpper().Equals("D") && _AnnoKeys.d != keyDown)
                _AnnoKeys.d = keyDown; // C key is clicked (for labeling the corner point in the highlighted time span)
            if (e.KeyCode.ToString().ToUpper().Equals("S") && _AnnoKeys.s != keyDown)
                _AnnoKeys.s = keyDown; // S key is clicked (for labeling the highlighted time span from the signal)

            // Show or hide points horizontal span
            PointsTimesSpanVisibility();
            // Show or hide interval horizontal span
            IntervalTimesSpanVisibility();
        }

        private void signalChart_MouseUp_CWD(object sender, MouseEventArgs e)
        {
            // Check if points horizontal span was dragged
            if (_pointHorSpanDragged)
            {
                // Recompute the highest and lowest point in the new span
                SelectHishest_LowestPoint();
                _pointHorSpanDragged = false;
            }
        }

        private void signalExhibitor_MouseMove_CWD(object sender, MouseEventArgs e)
        {
            // Get the plots
            Plot chartPlot = signalChart.Plot;
            SignalPlot signalPlot = (SignalPlot)_Plots[SANamings.Signal];
            ScatterPlot labelsScatPlot = (ScatterPlot)_Plots[SANamings.Labels];
            BubblePlot selectionBubble = (BubblePlot)_Plots[SANamings.Selection];

            // Clear the old selection
            ClearSelection();

            // Check if Ctrl is clicked for labeling any point from the signal
            if (_AnnoKeys.ctrl)
            {
                // Get the cursor coordinates
                (double curXCor, double curYCor) = chartPlot.GetCoordinate(e.X, e.Y);

                // Get the nearest point in the signal to the cursor's coordinates
                (double sigX, double sigY, int sigIndx) = GeneralTools.GetPointNearestXYSignalPlot(signalPlot, curXCor, curYCor);

                // Get the pixel of the nearest point
                (float xPix, float yPix) = chartPlot.GetPixel(sigX, sigY);

                // Check if the nearest state is less than 20 pixels from the cursor
                if (Math.Abs(e.X - xPix) < 20 && Math.Abs(e.Y - yPix) < 20)
                    // If yes then insert the new selection
                    SelectPoint(sigIndx);
            }

            SelectHighlightedSpanPoints();
        }

        private void signalChart_MouseClick_CWD(object sender, MouseEventArgs e)
        {
            // Check if the cursor is not the dragging cursor
            // and if any annotation key is clicked
            if ((sender as FormsPlot).Cursor != Cursors.SizeWE &&
                (_AnnoKeys.ctrl || _AnnoKeys.h || _AnnoKeys.l || _AnnoKeys.d || _AnnoKeys.s))
            {
                // Add the annotations into _AnnotationData
                foreach (int index in _SelectedPointsList)
                    _AnnotationData.InsertAnnotation("label", AnnotationType.Point, index, 0);

                // Check if an interval span is selected
                if (_AnnoKeys.s)
                {
                    SignalPlot signalPlot = (SignalPlot)_Plots[SANamings.Signal];

                    // Get the highlighted span interval
                    int spanStart = signalPlot.GetPointNearestX(((HSpan)_Plots[SANamings.IntervalHorizSpan]).X1).index;
                    int spanEnd = signalPlot.GetPointNearestX(((HSpan)_Plots[SANamings.IntervalHorizSpan]).X2).index;
                    (spanStart, spanEnd) = spanStart < spanEnd ? (spanStart, spanEnd) : (spanEnd, spanStart);

                    _AnnotationData.InsertAnnotation("label", AnnotationType.Interval, spanStart, spanEnd);
                }

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

                // Show annotation modify form if onlly one annotation was selected
                if (_SelectedPointsList.Count == 1 && !_AnnoKeys.s || _SelectedPointsList.Count == 0 && _AnnoKeys.s)
                {
                    AnnotationModify annotationModify = new AnnotationModify(newAnnoItem);
                    annotationModify.Show();

                    // Set all annotation keys to false
                    foreach (FieldInfo field in _AnnoKeys.GetType().GetFields())
                        field.SetValue(_AnnoKeys, false);
                }

                // Clear _SelectedPointsList
                _SelectedPointsList.Clear();
            }
        }

        private void nextButton_Click_CWD(object sender, EventArgs e)
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

        private void previousButton_Click_CWD(object sender, EventArgs e)
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

        private void saveButton_Click_CWD(object sender, EventArgs e)
        {
            // Save the signal with its features in dataset
            DbStimulator dbStimulator = new DbStimulator();
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Insert("anno_ds",
                new string[] { "sginal_name", "starting_index", "signal_data", "sampling_rate", "quantisation_step", "anno_objective", "anno_data" },
                new Object[] { pathLabel.Text, _FilteringTools._startingInSec, GeneralTools.ObjectToByteArray(_FilteringTools._OriginalRawSamples), _FilteringTools._samplingRate,
                               _FilteringTools._quantizationStep, CharacteristicWavesDelineation.ObjectiveName, GeneralTools.ObjectToByteArray(_AnnotationData) }, "CWDFormDetailModify"));
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

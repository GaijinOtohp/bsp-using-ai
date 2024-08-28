using Biological_Signal_Processing_Using_AI.DetailsModify.Annotations;
using Biological_Signal_Processing_Using_AI.DetailsModify.Filters;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.DetailsModify.Filters;
using BSP_Using_AI.DetailsModify.FiltersControls;
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
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Filters.CornersScanner;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.DetailsModify
{
    public partial class FormDetailsModify
    {
        private MouseEventArgs _MouseCursor = null;

        private bool _pointHorSpanDragged = false;

        private int _heighestPointInTimeSpanIndex = 0;
        private int _lowestPointInTimeSpanIndex = 0;

        private List<int> _SelectedPointsList = new List<int>();

        public int _lastSelectedItem_shift = -1;

        private void SetPointsHighlitedSpanProperties()
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
            // check if C key is clicked for selecting the corners
            if (_AnnoKeys.c)
            {
                ((CornersScanner)_FilteringTools._FiltersDic[typeof(CornersScanner).Name]).SetScanStartIndex(spanStart);
                double[] spanSamples = signalPlot.Ys.Select((y, indx) => (y, indx)).
                                                     Where(tuple => spanStart <= tuple.indx && tuple.indx <= spanEnd).
                                                     Select(tuple => tuple.y).ToArray();
                ((CornersScanner)_FilteringTools._FiltersDic[typeof(CornersScanner).Name]).SetSpanSamples(spanSamples);
            }
            else
                // Show the selected points
                SelectAllTypesPoints();
        }

        private void PointsTimesSpanVisibility()
        {
            // Check if the pointing horizontal span should be visible and it 
            if ((_AnnoKeys.h || _AnnoKeys.l || _AnnoKeys.c))
            {
                // Check if the pointing horizontal span is not already visible
                if (!_Plots[SANamings.PointHorizSpan].IsVisible)
                    // Show the horizontal span
                    GeneralTools.TimeSpanVisibility(signalChart, _Plots[SANamings.PointHorizSpan], true);
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

        private void SelectAllTypesPoints()
        {
            // Clear the old selection
            ClearSelection();

            // Check if Ctrl is clicked for labeling any point from the signal
            if (_AnnoKeys.ctrl && _MouseCursor != null)
                SelectPointCloseToCursor();

            SelectHighlightedSpanPoints();

            signalChart.Refresh();
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
            // Check if C is clicked for labeling the corners in the highlighted time span
            if (_AnnoKeys.c)
                // If yes then select the corners
                SelectCorners();
        }

        private void SelectPoint(int index)
        {
            // Get the plots
            SignalPlot signalPlot = (SignalPlot)_Plots[SANamings.Signal];
            BubblePlot selectionBubble = (BubblePlot)_Plots[SANamings.Selection];

            // Insert the index in the temporary selected points list
            _SelectedPointsList.Add(index);

            selectionBubble.Add(index / signalPlot.SampleRate + signalPlot.OffsetX, signalPlot.Ys[index], 5, Color.Red, 2, ForeColor);
        }

        private void SelectPointCloseToCursor()
        {
            // Get the plots
            Plot chartPlot = signalChart.Plot;
            SignalPlot signalPlot = (SignalPlot)_Plots[SANamings.Signal];
            ScatterPlot labelsScatPlot = (ScatterPlot)_Plots[SANamings.ScatterPlotsNames.Labels];
            BubblePlot selectionBubble = (BubblePlot)_Plots[SANamings.Selection];

            // Get the cursor coordinates
            (double curXCor, double curYCor) = chartPlot.GetCoordinate(_MouseCursor.X, _MouseCursor.Y);

            // Get the nearest point in the signal to the cursor's coordinates
            (double sigX, double sigY, int sigIndx) = GeneralTools.GetPointNearestXYSignalPlot(signalPlot, curXCor, curYCor);

            // Get the pixel of the nearest point
            (float xPix, float yPix) = chartPlot.GetPixel(sigX, sigY);

            // Check if the nearest state is less than 20 pixels from the cursor
            if (Math.Abs(_MouseCursor.X - xPix) < 20 && Math.Abs(_MouseCursor.Y - yPix) < 20)
                // If yes then insert the new selection
                SelectPoint(sigIndx);
        }

        private void SelectCorners()
        {
            BubblePlot selectionBubble = (BubblePlot)_Plots[SANamings.Selection];
            List<CornerSample> cornersList = ((CornersScanner)_FilteringTools._FiltersDic[typeof(CornersScanner).Name])._CornersList;
            if (cornersList != null)
            foreach (CornerSample corner in cornersList)
                SelectPoint(corner._index);
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
            // Set corners scanner
            CornersScanner cornersScanner = new CornersScanner(_FilteringTools);
            cornersScanner.SetForSelectionBubbles(true, SelectAllTypesPoints);
            cornersScanner.InsertFilter(filtersFlowLayoutPanel);
            ((CornersScannerUserControl)cornersScanner._FilterControl).showCornersCheckBox.Enabled = false;
            ((CornersScannerUserControl)cornersScanner._FilterControl).showDeviationCheckBox.Enabled = false;
            ((HSpan)_Plots[SANamings.PointHorizSpan]).X1 = ((SignalPlot)_Plots[SANamings.Signal]).OffsetX;
            ((HSpan)_Plots[SANamings.PointHorizSpan]).X2 = ((SignalPlot)_Plots[SANamings.Signal]).OffsetX;
            ((HSpan)_Plots[SANamings.IntervalHorizSpan]).X1 = ((SignalPlot)_Plots[SANamings.Signal]).OffsetX;
            ((HSpan)_Plots[SANamings.IntervalHorizSpan]).X2 = ((SignalPlot)_Plots[SANamings.Signal]).OffsetX;
            _FilteringTools.SetAutoApply(true);

            // Show the instructions of annotating the signal
            featuresSettingInstructionsLabel.Text = "Press \"Ctrl\" for labeling any point from the signal.\n" +
                                                    "Press \"H\" for labeling the highest point in the highlighted time span.\n" +
                                                    "Press \"L\" for labeling the lowest point in the highlighted time span.\n" +
                                                    "Press \"C\" for labeling the corner points in the highlighted time span.\n" +
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

        private bool ChangeKeyStatus(ref bool key, bool status)
        {
            key = status;
            return true;
        }

        private void FormDetailsModify_KeyDownUp_CWD(object sender, KeyEventArgs e, bool keyDown)
        {
            bool updatePointsSpan = false;
            // Check if which key is clicked
            if (e.KeyCode == Keys.ControlKey && _AnnoKeys.ctrl != keyDown)
                updatePointsSpan = ChangeKeyStatus(ref _AnnoKeys.ctrl, keyDown); // Control key is clicked (for labeling any point from the signal)
            if (e.KeyCode == Keys.ShiftKey && _AnnoKeys.shift != keyDown)
                _AnnoKeys.shift = keyDown; // Shift key is clicked (for selecting multiple point to be deleted)
            if (e.KeyCode.ToString().ToUpper().Equals("H") && _AnnoKeys.h != keyDown)
                updatePointsSpan = ChangeKeyStatus(ref _AnnoKeys.h, keyDown); // H key is clicked (for labeling the highest point in the highlighted time span)
            if (e.KeyCode.ToString().ToUpper().Equals("L") && _AnnoKeys.l != keyDown)
                updatePointsSpan = ChangeKeyStatus(ref _AnnoKeys.l, keyDown); // L key is clicked (for labeling the lowest point in the highlighted time span)
            if (e.KeyCode.ToString().ToUpper().Equals("C") && _AnnoKeys.c != keyDown)
                updatePointsSpan = ChangeKeyStatus(ref _AnnoKeys.c, keyDown); // C key is clicked (for labeling the corner point in the highlighted time span)
            if (e.KeyCode.ToString().ToUpper().Equals("S") && _AnnoKeys.s != keyDown)
                updatePointsSpan = ChangeKeyStatus(ref _AnnoKeys.s, keyDown); // S key is clicked (for labeling the highlighted time span from the signal)

            // Show or hide points horizontal span
            PointsTimesSpanVisibility();
            // Show or hide interval horizontal span
            IntervalTimesSpanVisibility();

            if (updatePointsSpan)
                SetPointsHighlitedSpanProperties();
        }

        private void signalChart_MouseUp_CWD(object sender, MouseEventArgs e)
        {
            // Check if points horizontal span was dragged
            if (_pointHorSpanDragged)
            {
                // Recompute the highest and lowest point in the new span
                SetPointsHighlitedSpanProperties();
                _pointHorSpanDragged = false;
            }
        }

        private void signalExhibitor_MouseMove_CWD(object sender, MouseEventArgs e)
        {
            _MouseCursor = e;
            if (_AnnoKeys.ctrl)
                SelectAllTypesPoints();
        }

        private void signalChart_MouseClick_CWD(object sender, MouseEventArgs e)
        {
            // Check if the cursor is not the dragging cursor
            // and if any annotation key is clicked
            if ((sender as FormsPlot).Cursor != Cursors.SizeWE &&
                (_AnnoKeys.ctrl || _AnnoKeys.h || _AnnoKeys.l || _AnnoKeys.c || _AnnoKeys.s))
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

        //*******************************************************************************************************//
        //*******************************************************************************************************//
        //************************************************AI TOOLS***********************************************//
        private void predictButton_Click_CWD(string modelName, string modelNameProblem)
        {
            // Check if this is for peaks scan using the deep Q-learning model
            // or for the delineation of the peaks using LSTM
            if (modelName.Equals(TFNETReinforcementL.ModelName))
                predictButton_Click_CWDRL(modelNameProblem);
            else if (modelName.Equals(TFNETLSTMModel.ModelName))
                predictButton_Click_CWDLSTM(modelNameProblem);
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
            // Check if we should insert or update the annotations data
            if (_id > 0)
            {
                DbStimulator dbStimulator = new DbStimulator();
                Thread dbStimulatorThread = new Thread(() => dbStimulator.Update("anno_ds",
                    new string[] { "sampling_rate", "quantisation_step", "anno_data" },
                    new object[] { _FilteringTools._samplingRate, _FilteringTools._quantizationStep, GeneralTools.ObjectToByteArray(_AnnotationData) },
                    _id,
                    "CWDFormDetailModify"));
                dbStimulatorThread.Start();
            }
            else
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
            }

            // Disable save button
            saveButton.Enabled = false;
        }
    }
}

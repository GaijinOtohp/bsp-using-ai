using Biological_Signal_Processing_Using_AI.DetailsModify.Filters;
using BSP_Using_AI.AITools;
using BSP_Using_AI.DetailsModify.Filters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.DetailsModify
{
    public partial class FormDetailsModify
    {
        public class ARTHTFiltersNames
        {
            public static string DCRemoval = "DCRemoval";
            public static string Normalize = "Normalize";
            public static string Absolute = "Absolute";
            public static string ExistanceDeclare = "ExistanceDeclare";
            public static string IIRFilter = "IIRFilter";
            public static string DWT = "DWT";
            public static string PeaksAnalyzer = "PeaksAnalyzer";
        }

        private void signalExhibitor_MouseMove_ARTHT(object sender, MouseEventArgs e)
        {
            // Check if AI tool is activated and in R selection step
            if ((_arthtFeatures._processedStep == 2 || _arthtFeatures._processedStep == 4) && (sender as Chart).Name.Equals("signalChart"))
            {
                // If yes then it is activated
                Chart chart = sender as Chart;
                // Check if cursor is inside the chart
                if (e.X >= chart.Width || e.Y >= chart.Height)
                    return;
                // Check if mouse cursor is near to a any state by 20% to the nearest state
                // Calculate x interval and y interval
                double xInterval = ((sender as Chart).ChartAreas[0].AxisX.Maximum - (sender as Chart).ChartAreas[0].AxisX.Minimum) * _FilteringTools._samplingRate;
                double yInterval = (sender as Chart).ChartAreas[0].AxisY.Maximum - (sender as Chart).ChartAreas[0].AxisY.Minimum;

                // Get values of the position
                double xValue = ((sender as Chart).ChartAreas[0].AxisX.PixelPositionToValue(e.X) - _FilteringTools._startingInSec) * _FilteringTools._samplingRate;
                double yValue = (sender as Chart).ChartAreas[0].AxisY.PixelPositionToValue(e.Y);

                // Get all up points
                // Take the nearest point of up points in X
                Dictionary<string, List<State>> statesDic = ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer])._StatesDIc;
                State nearestState = new State();
                double nearestStateMag = double.PositiveInfinity;
                foreach (string statesLabel in new string[] { SANamings.UpPeaks, SANamings.DownPeaks, SANamings.StableStates })
                    foreach (State xState in statesDic[statesLabel])
                    {
                        // Compute magnitude between the state and the cursor position
                        double mag = Math.Sqrt(Math.Pow((xValue - xState._index) / xInterval, 2) + Math.Pow((yValue - xState._value) / yInterval, 2));
                        // Compare it with the previous nearest state
                        if (mag < nearestStateMag)
                        {
                            nearestState = xState;
                            nearestStateMag = mag;
                        }
                    }

                // Check if cursor is near to the point by less than 20% in X and Y
                if (!statesDic.ContainsKey(SANamings.Selection))
                    statesDic.Add(SANamings.Selection, new List<State>());
                if (nearestStateMag < 0.2)
                {
                    // If yes then make a selection point at this position
                    Garage.loadXYInChart(signalChart, new double[1] { (nearestState._index / (double)_FilteringTools._samplingRate) + _FilteringTools._startingInSec }, new double[1] { nearestState._value }, null, 0, 4, "FormDetailModify");
                    // Set selection state's index
                    statesDic[SANamings.Selection].Clear();
                    statesDic[SANamings.Selection].Add(nearestState);
                }
                else
                {
                    (sender as Chart).Series[SANamings.Selection].Points.Clear();
                    statesDic[SANamings.Selection].Clear();
                }
            }
        }

        private void signalChart_MouseClick(object sender, MouseEventArgs e)
        {
            // Check if AI tool is activated and in R selection step
            if ((_arthtFeatures._processedStep == 2 || _arthtFeatures._processedStep == 4))
            {
                Dictionary<string, List<State>> statesDic = ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer])._StatesDIc;
                if (!statesDic.ContainsKey(SANamings.Selection))
                    statesDic.Add(SANamings.Selection, new List<State>());
                // If yes then it is activated
                // Get the selcted point if it exists
                if (statesDic[SANamings.Selection].Count > 0)
                {
                    // Get the index of the point
                    foreach (string statesLabel in new string[] { SANamings.UpPeaks, SANamings.DownPeaks, SANamings.StableStates })
                        for (int i = 0; i < statesDic[statesLabel].Count; i++)
                            if (statesDic[SANamings.Selection][0]._index == statesDic[statesLabel][i]._index)
                            {
                                // Check if it has a label then remove it, and if not add it
                                int selectedBeatIndx = 0;
                                if (featuresTableLayoutPanel.Controls.ContainsKey(ARTHTNamings.Step4PTSelectionData))
                                    selectedBeatIndx = ((ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[ARTHTNamings.Step4PTSelectionData]).Items[0]).DropDownItems.Count;
                                if (!(sender as Chart).Series[statesLabel].Points[i].Label.Equals(""))
                                {
                                    if ((sender as Chart).Series[statesLabel].Points[i].Label.Equals(SANamings.P))
                                        _arthtFeatures.SignalBeats[selectedBeatIndx]._pIndex = int.MinValue;
                                    else if ((sender as Chart).Series[statesLabel].Points[i].Label.Equals(SANamings.T))
                                        _arthtFeatures.SignalBeats[selectedBeatIndx]._tIndex = int.MinValue;
                                    (sender as Chart).Series[statesLabel].Points[i].Label = "";
                                }
                                else if (_arthtFeatures._processedStep == 2)
                                    (sender as Chart).Series[statesLabel].Points[i].Label = SANamings.R;
                                else if (_arthtFeatures._processedStep == 4)
                                {
                                    int selectedIndx = _arthtFeatures.SignalBeats[selectedBeatIndx]._startingIndex + statesDic[statesLabel][i]._index;
                                    if (_arthtFeatures.SignalBeats[selectedBeatIndx]._pIndex == int.MinValue)
                                    {
                                        (sender as Chart).Series[statesLabel].Points[i].Label = SANamings.P;
                                        _arthtFeatures.SignalBeats[selectedBeatIndx]._pIndex = selectedIndx;
                                    }
                                    else if (_arthtFeatures.SignalBeats[selectedBeatIndx]._tIndex == int.MinValue)
                                    {
                                        (sender as Chart).Series[statesLabel].Points[i].Label = SANamings.T;
                                        _arthtFeatures.SignalBeats[selectedBeatIndx]._tIndex = selectedIndx;
                                    }
                                }
                            }
                }
            }
        }

        //*******************************************************************************************************//
        //*******************************************************************************************************//
        //************************************************AI TOOLS***********************************************//

        private void predictButton_Click(object sender, EventArgs e)
        {
            // Get the selected model from modelTypeComboBox
            if (modelTypeComboBox.SelectedIndex > -1)
            {
                // Disable this button and modelTypeComboBox
                predictButton.Enabled = false;
                modelTypeComboBox.Enabled = false;
                // Set prediction to true
                _predictionOn = true;
                // Disable auto apply to filters
                _FilteringTools.SetAutoApply(false);
                // Initialize tools
                setFeaturesLabelsButton_Click(null, null);
                // Start prediction
                nextButton_Click(null, null);
            }
        }

        public TFBackThread _tFBackThread;
        public readonly AutoResetEvent _signal = new AutoResetEvent(false);
        public readonly ConcurrentQueue<QueueSignalInfo> _queue = new ConcurrentQueue<QueueSignalInfo>();
        private double[] askForPrediction(double[] features, string stepName)
        {
            // Send information to TFBackThread
            string modelsName = null;
            this.Invoke(new MethodInvoker(delegate () { modelsName = modelTypeComboBox.Text; }));
            // Check which model is selected
            if (modelsName.Contains("Neural network"))
            {
                // This is for neural network
                _tFBackThread._queue.Enqueue(new QueueSignalInfo()
                {
                    TargetFunc = "predict",
                    CallingClass = "FormDetailsModify",
                    Features = features,
                    ModelsName = modelsName,
                    StepName = stepName,
                    Signal = _signal,
                    Queue = _queue
                });
                _tFBackThread._signal.Set();

                // Wait for the answer
                _signal.WaitOne();

                QueueSignalInfo item = null;
                while (_queue.TryDequeue(out item))
                    // Check which function is selected
                    return item.Outputs;
            }
            else if (modelsName.Contains("K-Nearest neighbor"))
            {
                // This is for knn
                return KNNBackThread.predict(features, (KNNModel)_arthtModelsDic[modelsName].ARTHTModelsDic[stepName]);
            }
            else if (modelsName.Contains("Naive bayes"))
            {
                // This is for naive bayes
                return NaiveBayesBackThread.predict(features, (NaiveBayesModel)_arthtModelsDic[modelsName].ARTHTModelsDic[stepName]);
            }

            return null;
        }

        private void setFeaturesLabelsButton_Click(object sender, EventArgs e)
        {
            // Disable everything except ai tools
            enableAITools(true);

            // Set dc removal filter. Check it then set it disabled
            DCRemoval dcRemoval = new DCRemoval(_FilteringTools);
            dcRemoval.InsertFilter(filtersFlowLayoutPanel);
            ((DCRemovalUserControl)dcRemoval._FilterControl).Enabled = false;
            // Get first 3 levels of DWT of the signal
            DWT dwt = new DWT(_FilteringTools);
            dwt.InsertFilter(filtersFlowLayoutPanel);
            dwt.SetMaxLevel(3);
            ((DWTUserControl)dwt._FilterControl).waveletTypeComboBox.Enabled = false;
            ((DWTUserControl)dwt._FilterControl).numOfVanMoComboBox.Enabled = false;
            // Normalize signal and make its values absolute after DWT transform
            Absolute absolute = new Absolute(_FilteringTools);
            absolute.InsertFilter(filtersFlowLayoutPanel);
            ((AbsoluteSignalUserControl)absolute._FilterControl).Enabled = false;
            Normalize normalize = new Normalize(_FilteringTools);
            normalize.InsertFilter(filtersFlowLayoutPanel);
            ((NormalizedSignalUserControl)normalize._FilterControl).Enabled = false;
            // Set peaks analizer
            PeaksAnalyzer peaksAnalyzer = new PeaksAnalyzer(_FilteringTools);
            peaksAnalyzer.InsertFilter(filtersFlowLayoutPanel);
            ((SignalStatesViewerUserControl)peaksAnalyzer._FilterControl).showStatesCheckBox.Enabled = false;
            ((SignalStatesViewerUserControl)peaksAnalyzer._FilterControl).showDeviationCheckBox.Enabled = false;
            ((SignalStatesViewerUserControl)peaksAnalyzer._FilterControl).tdtThresholdScrollBar.Enabled = false;

            // Show instructions of the first goal
            featuresSettingInstructionsLabel.Text = "Set the best \"amplitude reatio threshold (ART)\" and \"horizontal threshold (HT)\" so that only high peaks are visible.\n" +
                                                    "Press next after you finish.";

            // Enable discard and next button, and disable setFeaturesLabelsButton button
            discardButton.Enabled = true;
            nextButton.Enabled = true;
            setFeaturesLabelsButton.Enabled = false;
        }

        private void discardButton_Click(object sender, EventArgs e)
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

                signalChart.Series[SANamings.UpPeaks].LabelForeColor = Color.Transparent;
                foreach (Series series in signalChart.Series)
                    if (!series.Name.Equals(SANamings.Signal))
                        series.Points.Clear();

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

        //____________________________________________________________________________________________________________________________________________________//
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        private void nextButton_Click(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                // Check if next button is finish
                if (nextButton.Text.Equals("Finish"))
                {
                    // If yes then lunch finish function and return
                    finish();

                    // Check if prediction is on
                    if (_predictionOn)
                    {
                        // If yes then return prediction stuff to normal state
                        _predictionOn = false;
                        predictButton.Enabled = true;
                        modelTypeComboBox.Enabled = true;
                    }

                    return;
                }

                ToolStripMenuItem featuresItems = null;
                ToolStripMenuItem flowLayoutItems01 = null;
                ToolStripMenuItem flowLayoutItems02 = null;
                ToolStripMenuItem flowLayoutItems03 = null;

                if (_arthtFeatures._processedStep == 0)
                    _arthtFeatures._processedStep = 1;

                // Get curretn step threshold
                float threshold = 0.5f;

                // Check which step of features selectoin is this
                switch (_arthtFeatures._processedStep)
                {
                    case 1:
                        // This is for QRS detection
                        featuresItems = new ToolStripMenuItem(ARTHTNamings.Step1RPeaksScanData) { Name = ARTHTNamings.Step1RPeaksScanData };
                        Sample rPeaksScanSamp = new Sample(ARTHTNamings.Step1RPeaksScanData + "0", 15, 2, _arthtFeatures.StepsDataDic[ARTHTNamings.Step1RPeaksScanData]);
                        // Disable showing results in chart
                        _FilteringTools.ShowResultInChart(false);
                        // Iterate through 3 levels of dwt
                        flowLayoutItems01 = new ToolStripMenuItem(ARTHTNamings.Features);
                        List<StatParam> statParams;
                        for (int i = 0; i < 3; i++)
                        {
                            // Normalize the absolute values of levelSignal
                            ((DWT)_FilteringTools._FiltersDic[ARTHTFiltersNames.DWT]).SelectLevel(i);
                            _FilteringTools.ApplyFilters(false);

                            // Create the pdf of the signal
                            statParams = Garage.statParams(_FilteringTools._FilteredSamples);
                            for (int j = 0; j < statParams.Count; j++)
                            {
                                rPeaksScanSamp.insertFeature(i * 5 + j, statParams[j].Name + (i + 1), statParams[j]._value);
                                flowLayoutItems01.DropDownItems.Add(statParams[j].Name + (i + 1) + ": " + statParams[j]._value.ToString());
                            }
                        }
                        featuresItems.DropDownItems.Add(flowLayoutItems01);
                        // Reenable showing results in chart
                        _FilteringTools.ShowResultInChart(true);
                        // get signal states viewer user control and set the features output
                        flowLayoutItems01 = new ToolStripMenuItem(ARTHTNamings.Outputs);

                        // Check if outputs should be predicted
                        if (_predictionOn)
                        {
                            rPeaksScanSamp.insertOutputArray(new string[] { ARTHTNamings.ART, ARTHTNamings.HT },
                                askForPrediction(rPeaksScanSamp.getFeatures(), ARTHTNamings.Step1RPeaksScanData));
                        }
                        else
                            rPeaksScanSamp.insertOutputArray(new string[] { ARTHTNamings.ART, ARTHTNamings.HT },
                                new double[2] { ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer])._art,
                                                    ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer])._ht });
                        flowLayoutItems01.DropDownItems.Add(ARTHTNamings.ART + ": " + rPeaksScanSamp.getOutputByLabel(ARTHTNamings.ART));
                        flowLayoutItems01.DropDownItems.Add(ARTHTNamings.HT + ": " + rPeaksScanSamp.getOutputByLabel(ARTHTNamings.HT));

                        featuresItems.DropDownItems.Add(flowLayoutItems01);

                        // Make the form ready for next goal
                        _arthtFeatures._processedStep++;
                        // Remove absolute signal filter and DWT filter
                        _FilteringTools._FiltersDic[ARTHTFiltersNames.Absolute].RemoveFilter();
                        _FilteringTools._FiltersDic[ARTHTFiltersNames.DWT].RemoveFilter();
                        // Refresh the signal to restore the original sampling rate from DWT effect
                        _FilteringTools.ApplyFilters(false);
                        // Set ART and HT of peaks analyzer
                        ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]).SetART(rPeaksScanSamp.getOutputByLabel(ARTHTNamings.ART));
                        ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]).SetHT(rPeaksScanSamp.getOutputByLabel(ARTHTNamings.HT));
                        // Apply the filters for the next step
                        _FilteringTools.ApplyFilters(false);
                        _FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]._FilterControl.Enabled = false;
                        // Set the up peaks labels ready
                        signalChart.Series[SANamings.UpPeaks].LabelForeColor = Color.Black;
                        for (int i = 0; i < signalChart.Series[SANamings.UpPeaks].Points.Count; i++)
                            signalChart.Series[SANamings.UpPeaks].Points[i].Label = "";

                        // Give the instruction for next goal, and enable previous button
                        featuresSettingInstructionsLabel.Text = "Select R peaks from the visible states in the chart.\nClick once on the peak to select, or click once again to unselect.\nPress next after you finish.";
                        previousButton.Enabled = true;
                        break;
                    case 2:
                        // This is for QRS selection
                        featuresItems = new ToolStripMenuItem(ARTHTNamings.Step2RPeaksSelectionData) { Name = ARTHTNamings.Step2RPeaksSelectionData };

                        // Create a hashtable where to store R indexes and their amplitude
                        Dictionary<int, State> rDictionary = new Dictionary<int, State>();

                        // Get the states of the normalized signal
                        List<State> signalUpStates = ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer])._StatesDIc[SANamings.UpPeaks];

                        // Get the states of each of 3 levels of normalized absolute dwt
                        List<State> dwtUpStates;
                        int lastSavedRIndx;
                        int interval;
                        for (int i = 0; i < 3; i++)
                        {
                            lastSavedRIndx = 0;
                            // Get its states
                            dwtUpStates = ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer])._DWTLevelsStatesDIc[i][SANamings.UpPeaks];

                            // Iterate through each up state from absoluteDWTStates and compare it with states in normalizedSignalStates
                            // and save the nearest state of normalizedSignalStates to the up state of absoluteDWTStates in qrsList as R state
                            foreach (State dwtUpState in dwtUpStates)
                            {
                                // Start from the last saved qrs index in qrsList
                                interval = int.MaxValue;
                                for (int j = lastSavedRIndx; j < signalUpStates.Count; j++)
                                {
                                    // Check if the state is getting closer
                                    if (interval >= Math.Abs((dwtUpState._index * (int)Math.Pow(2, i + 1)) - signalUpStates[j]._index))
                                    {
                                        // Update the new interval and save the index of current state
                                        interval = Math.Abs((dwtUpState._index * (int)Math.Pow(2, i + 1)) - signalUpStates[j]._index);
                                        lastSavedRIndx = j;
                                    }
                                    else
                                    {
                                        // If yes then the last state is the closest.
                                        // Then save the 6 states near it in qrsList if they didn't already exist
                                        if (!rDictionary.ContainsKey(signalUpStates[lastSavedRIndx]._index))
                                            rDictionary.Add(signalUpStates[lastSavedRIndx]._index, signalUpStates[lastSavedRIndx]);
                                        break;
                                    }
                                }
                                // Save last state in qrsList if it didn't already exist
                                if (!rDictionary.ContainsKey(signalUpStates[lastSavedRIndx]._index))
                                    rDictionary.Add(signalUpStates[lastSavedRIndx]._index, signalUpStates[lastSavedRIndx]);
                            }
                        }

                        // Order R peaks in a list
                        List<State> orderedRPeaks = rDictionary.OrderBy(rState => rState.Key).Select(rState => rState.Value).ToList();
                        // Calculate RR average from qrsHashtable
                        double averageRR = 0;
                        int intervalsNum = orderedRPeaks.Count - 1;
                        for (int i = 1; i < orderedRPeaks.Count; i++)
                            averageRR += (orderedRPeaks[i]._index - orderedRPeaks[i - 1]._index) / (double)intervalsNum;

                        // Initialize SognalBeats list
                        _arthtFeatures.SignalBeats = new List<Beat>();
                        // Set the inputs and outputs for each qrs in qrsOrderedDictionary
                        Sample rPeaksSelectionSamp;
                        for (int i = 0; i < orderedRPeaks.Count; i++)
                        {
                            // Set the samples of removing this state
                            rPeaksSelectionSamp = new Sample(ARTHTNamings.Step2RPeaksSelectionData + i, 2, 1, _arthtFeatures.StepsDataDic[ARTHTNamings.Step2RPeaksSelectionData]);
                            // inputs = { (R_cur_indx - R_prev_indx) / averageRR, R_cur_amp / R_prev_amp }
                            // outputs = { remove_R_cur }
                            if (i == 0)
                            {
                                rPeaksSelectionSamp.insertFeature(0, ARTHTNamings.RIntrvl, Math.Abs(orderedRPeaks[i]._index - orderedRPeaks[i + 1]._index) / averageRR);
                                rPeaksSelectionSamp.insertFeature(1, ARTHTNamings.RAmp, orderedRPeaks[i]._value / orderedRPeaks[i + 1]._value);
                            }
                            else
                            {
                                rPeaksSelectionSamp.insertFeature(0, ARTHTNamings.RIntrvl, Math.Abs(orderedRPeaks[i]._index - orderedRPeaks[i - 1]._index) / averageRR);
                                rPeaksSelectionSamp.insertFeature(1, ARTHTNamings.RAmp, orderedRPeaks[i]._value / orderedRPeaks[i - 1]._value);
                            }

                            // Initialize the output as the R peak should be removed
                            rPeaksSelectionSamp.insertOutputArray(new string[] { ARTHTNamings.RemoveR },
                                                    new double[1] { 1 });
                            // Check if outputs should be predicted
                            if (_predictionOn)
                            {
                                rPeaksSelectionSamp.insertOutputArray(new string[] { ARTHTNamings.RemoveR },
                                    askForPrediction(rPeaksSelectionSamp.getFeatures(), ARTHTNamings.Step2RPeaksSelectionData));

                                threshold = _arthtModelsDic[modelTypeComboBox.Text].ARTHTModelsDic[ARTHTNamings.Step2RPeaksSelectionData].OutputsThresholds[0];
                                // Check if this R state is selected not to be removed
                                if (rPeaksSelectionSamp.getOutputByLabel(ARTHTNamings.RemoveR) < threshold)
                                    // If yes then add current state as R
                                    _arthtFeatures.SignalBeats.Add(new Beat() { _rIndex = orderedRPeaks[i]._index });
                            }
                            else
                            {
                                // Check if this R is selected
                                // Start from the last saved qrs index in qrsList
                                for (int j = 0; j < signalChart.Series[SANamings.UpPeaks].Points.Count; j++)
                                {
                                    // Check if the state is getting closer
                                    if (Math.Abs(signalUpStates[j]._index - orderedRPeaks[i]._index) == 0)
                                    {
                                        // If yes then the last state is the closest. Then check if this point's label is selected as R or not
                                        if (signalChart.Series[SANamings.UpPeaks].Points[j].Label.Equals(SANamings.R))
                                        {
                                            // If yes then set the output to 0 (do not remove state)
                                            rPeaksSelectionSamp.insertOutputArray(new string[] { ARTHTNamings.RemoveR },
                                                    new double[1] { 0d });
                                            // Set the selected R index and its beat's start and end indexex
                                            _arthtFeatures.SignalBeats.Add(new Beat() { _rIndex = orderedRPeaks[i]._index });
                                        }
                                        break;
                                    }
                                }
                            }

                            flowLayoutItems01 = new ToolStripMenuItem(SANamings.R + i);
                            flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Features);
                            flowLayoutItems02.DropDownItems.Add(ARTHTNamings.RIntrvl + ": " + rPeaksSelectionSamp.getFeatureByLabel(ARTHTNamings.RIntrvl));
                            flowLayoutItems02.DropDownItems.Add(ARTHTNamings.RAmp + ": " + rPeaksSelectionSamp.getFeatureByLabel(ARTHTNamings.RAmp));
                            flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);
                            flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Outputs);
                            flowLayoutItems02.DropDownItems.Add(ARTHTNamings.RemoveR + ": " + rPeaksSelectionSamp.getOutputByLabel(ARTHTNamings.RemoveR));
                            flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                            featuresItems.DropDownItems.Add(flowLayoutItems01);
                        }
                        // Check if there is no beats selected
                        if (_arthtFeatures.SignalBeats.Count == 0)
                        {
                            // If yes then show a message of beats must be selected
                            MessageBox.Show("There must be at least one beat selected", "Warning \"Unexpected data\"", MessageBoxButtons.OK);
                            return;
                        }

                        // Set selected R's beats starting and ending indexes
                        int starting = 0;
                        int ending = 0;
                        for (int i = 0; i < _arthtFeatures.SignalBeats.Count; i++)
                        {
                            // Set the starting to be from 3/4 of the last R peak
                            if (i != 0)
                                starting = _arthtFeatures.SignalBeats[i - 1]._rIndex + (_arthtFeatures.SignalBeats[i]._rIndex - _arthtFeatures.SignalBeats[i - 1]._rIndex) / 4;
                            // and the ending to be to 3/4 of the next R peak
                            if (i != _arthtFeatures.SignalBeats.Count - 1)
                                ending = _arthtFeatures.SignalBeats[i + 1]._rIndex - (_arthtFeatures.SignalBeats[i + 1]._rIndex - _arthtFeatures.SignalBeats[i]._rIndex) / 4;
                            else
                                ending = _FilteringTools._OriginalRawSamples.Length - 1;

                            _arthtFeatures.SignalBeats[i]._startingIndex = starting;
                            _arthtFeatures.SignalBeats[i]._endingIndex = ending;
                        }

                        // Make the form ready for next goal
                        _arthtFeatures._processedStep++;
                        signalChart.Series[SANamings.Selection].Points.Clear();
                        // Show the first selected Beat
                        _FilteringTools._RawSamples = new double[(_arthtFeatures.SignalBeats[0]._endingIndex - _arthtFeatures.SignalBeats[0]._startingIndex) + 1];
                        for (int i = _arthtFeatures.SignalBeats[0]._startingIndex; i < _arthtFeatures.SignalBeats[0]._endingIndex + 1; i++)
                            _FilteringTools._RawSamples[i - _arthtFeatures.SignalBeats[0]._startingIndex] = _FilteringTools._OriginalRawSamples[i];
                        _FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]._FilterControl.Enabled = true;
                        ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]).SetART(0.2d);
                        ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]).SetHT(0.01d);
                        // Refresh the apply button if autoApply is checked
                        signalChart.Series["Up peaks"].LabelForeColor = Color.Transparent;
                        _FilteringTools.ApplyFilters(false);

                        // Give the instruction for next goal, and enable previous button
                        featuresSettingInstructionsLabel.Text = "Set the best \"amplitude reatio threshold (ART)\" and \"horizontal threshold (HT)\" for the segmentation of P and T waves.\n" + 1 + "/" + _arthtFeatures.SignalBeats.Count + "\nPress next after you finish.";
                        break;
                    case 3:
                        // This is for beats peaks detection (P, T)
                        // Get previous saved features if exists
                        if (featuresTableLayoutPanel.Controls.Count == 3)
                            featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[ARTHTNamings.Step3BeatPeaksScanData]).Items[0];
                        else
                            featuresItems = new ToolStripMenuItem(ARTHTNamings.Step3BeatPeaksScanData) { Name = ARTHTNamings.Step3BeatPeaksScanData };

                        // Create the pdf of the signal and insert its coefs as inputs
                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Features);
                        statParams = Garage.statParams(_FilteringTools._FilteredSamples);
                        Sample beatPeaksScanSamp = new Sample(ARTHTNamings.Step3BeatPeaksScanData + _arthtFeatures.StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].Samples.Count, 5, 2, _arthtFeatures.StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData]);
                        for (int j = 0; j < statParams.Count; j++)
                        {
                            beatPeaksScanSamp.insertFeature(j, statParams[j].Name, statParams[j]._value);
                            flowLayoutItems02.DropDownItems.Add(statParams[j].Name + ": " + statParams[j]._value.ToString());
                        }
                        flowLayoutItems01 = new ToolStripMenuItem(ARTHTNamings.Beat + featuresItems.DropDownItems.Count);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        // get signal states viewer user control and set the features output
                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Outputs);

                        // Check if outputs should be predicted
                        if (_predictionOn)
                        {
                            beatPeaksScanSamp.insertOutputArray(new string[] { ARTHTNamings.ART, ARTHTNamings.HT },
                                askForPrediction(beatPeaksScanSamp.getFeatures(), ARTHTNamings.Step3BeatPeaksScanData));
                            // Scan Q and S peaks with the predicted ART and HT parameters
                            ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]).SetART(beatPeaksScanSamp.getOutputByLabel(ARTHTNamings.ART));
                            ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]).SetHT(beatPeaksScanSamp.getOutputByLabel(ARTHTNamings.HT));
                            _FilteringTools.ApplyFilters(false);
                        }
                        else
                            beatPeaksScanSamp.insertOutputArray(new string[] { ARTHTNamings.ART, ARTHTNamings.HT },
                                new double[2] { ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer])._art,
                                                    ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer])._ht });

                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.ART + ": " + beatPeaksScanSamp.getOutputByLabel(ARTHTNamings.ART));
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.HT + ": " + beatPeaksScanSamp.getOutputByLabel(ARTHTNamings.HT));
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);
                        featuresItems.DropDownItems.Add(flowLayoutItems01);

                        // Set Q and S indexes of the beat
                        int nextBeatIndex = featuresItems.DropDownItems.Count;
                        int currentBeatIndex = nextBeatIndex - 1;
                        List<State> signalStates = ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer])._StatesDIc[SANamings.AllPeaks];
                        for (int i = 0; i < signalStates.Count; i++)
                            if (signalStates[i]._index == (_arthtFeatures.SignalBeats[currentBeatIndex]._rIndex - _arthtFeatures.SignalBeats[currentBeatIndex]._startingIndex))
                            {
                                int qIndx = 0;
                                int sIndx = signalStates.Count - 1;
                                if (i > 0)
                                    qIndx = i - 1;
                                if (i < signalStates.Count - 1)
                                    sIndx = i + 1;
                                _arthtFeatures.SignalBeats[currentBeatIndex]._qIndex = signalStates[qIndx]._index + _arthtFeatures.SignalBeats[currentBeatIndex]._startingIndex;
                                _arthtFeatures.SignalBeats[currentBeatIndex]._sIndex = signalStates[sIndx]._index + _arthtFeatures.SignalBeats[currentBeatIndex]._startingIndex;
                            }

                        // Check if there exist next beat
                        if (nextBeatIndex < _arthtFeatures.SignalBeats.Count)
                        {
                            // If yes then set the next beat for segmentation
                            _FilteringTools._RawSamples = new double[(_arthtFeatures.SignalBeats[nextBeatIndex]._endingIndex - _arthtFeatures.SignalBeats[nextBeatIndex]._startingIndex) + 1];
                            for (int i = _arthtFeatures.SignalBeats[nextBeatIndex]._startingIndex; i < _arthtFeatures.SignalBeats[nextBeatIndex]._endingIndex + 1; i++)
                                _FilteringTools._RawSamples[i - _arthtFeatures.SignalBeats[nextBeatIndex]._startingIndex] = _FilteringTools._OriginalRawSamples[i];
                            ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]).SetHT(0.01d);
                            // Refresh the apply button if autoApply is checked
                            _FilteringTools.ApplyFilters(false);

                            // Give the instruction for next goal, and enable previous button
                            featuresSettingInstructionsLabel.Text = "Set the best \"amplitude reatio threshold (ART)\" and \"horizontal threshold (HT)\" for the segmentation of P and T waves.\n" + (nextBeatIndex + 1) + "/" + _arthtFeatures.SignalBeats.Count + "\nPress next after you finish.";
                        }
                        else
                        {
                            // Make the form ready for next goal
                            _arthtFeatures._processedStep++;
                            // Remove access to peaks analyzer control
                            ((SignalStatesViewerUserControl)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]._FilterControl).amplitudeThresholdScrollBar.Enabled = false;
                            ((SignalStatesViewerUserControl)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]._FilterControl).hThresholdScrollBar.Enabled = false;
                            _FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]._FilterControl.Enabled = false;
                            // Refresh the apply button if autoApply is checked
                            setNextBeat(_arthtFeatures.SignalBeats[0], _arthtFeatures.StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].Samples[0], null);

                            // Give the instruction for next goal, and enable previous button
                            featuresSettingInstructionsLabel.Text = "Select P and T peaks from the visible states in the chart.\nClick once on the peak to select, or click once again to unselect.\n" + 1 + "/" + _arthtFeatures.SignalBeats.Count + "\nPress next after you finish.";
                        }
                        break;
                    case 4:
                        // This is for P and T selection
                        // Get previous saved features if exists
                        if (featuresTableLayoutPanel.Controls.Count == 4)
                            featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[ARTHTNamings.Step4PTSelectionData]).Items[0];
                        else
                            featuresItems = new ToolStripMenuItem(ARTHTNamings.Step4PTSelectionData) { Name = ARTHTNamings.Step4PTSelectionData };

                        // Get the index of selected beat
                        int selectedBeatIndx = featuresItems.DropDownItems.Count;

                        // Get states of the selected beat
                        signalStates = ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer])._StatesDIc[SANamings.AllPeaks];
                        // Iterate through each state and set its features
                        // starting from the second and ending by the one berfore last
                        flowLayoutItems01 = new ToolStripMenuItem(ARTHTNamings.Beat + selectedBeatIndx);
                        Sample ptSelectionSamp;
                        int previousStateIndx;
                        int nextStateIndx;
                        double pProba = 0;
                        double tProba = 0;
                        for (int i = 1; i < signalStates.Count - 1; i++)
                        {
                            ptSelectionSamp = new Sample(ARTHTNamings.Step4PTSelectionData + selectedBeatIndx + ", " + i, 3, 2, _arthtFeatures.StepsDataDic[ARTHTNamings.Step4PTSelectionData]);
                            flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.State + i);
                            // Set the inputs of the state
                            flowLayoutItems03 = new ToolStripMenuItem(ARTHTNamings.Features);
                            ptSelectionSamp.insertFeature(0, ARTHTNamings.Stx, signalStates[i]._value);
                            flowLayoutItems03.DropDownItems.Add("ampSt" + i + ": " + ptSelectionSamp.getFeatureByLabel(ARTHTNamings.Stx));
                            if (selectedBeatIndx == 0)
                            {
                                ptSelectionSamp.insertFeature(1,
                                                              ARTHTNamings.StRIntrvl,
                                                              (double)(signalStates[i]._index + _arthtFeatures.SignalBeats[selectedBeatIndx]._startingIndex - _arthtFeatures.SignalBeats[selectedBeatIndx]._rIndex) /
                                                              (double)(_arthtFeatures.SignalBeats[selectedBeatIndx + 1]._rIndex - _arthtFeatures.SignalBeats[selectedBeatIndx]._rIndex));
                                flowLayoutItems03.DropDownItems.Add("(St" + i + "-R" + selectedBeatIndx + ") / (R1-R" + selectedBeatIndx + "): " + ptSelectionSamp.getFeatures()[1]);
                            }
                            else
                            {
                                ptSelectionSamp.insertFeature(1,
                                                              ARTHTNamings.StRIntrvl,
                                                              (double)(signalStates[i]._index + _arthtFeatures.SignalBeats[selectedBeatIndx]._startingIndex - _arthtFeatures.SignalBeats[selectedBeatIndx]._rIndex) /
                                                              (double)(_arthtFeatures.SignalBeats[selectedBeatIndx]._rIndex - _arthtFeatures.SignalBeats[selectedBeatIndx - 1]._rIndex));
                                flowLayoutItems03.DropDownItems.Add("(St" + i + "-R" + selectedBeatIndx + ") / (R" + selectedBeatIndx + "-R" + (selectedBeatIndx - 1) + "): " + ptSelectionSamp.getFeatures()[1]);
                            }
                            // Get next and previous state's index
                            previousStateIndx = i - 1;
                            nextStateIndx = i + 1;
                            for (int j = i + 1; j < signalStates.Count; j++)
                            {
                                if (!signalStates[i].Name.Equals("stable"))
                                    if (signalStates[j].Name.Equals(signalStates[i].Name))
                                        break;

                                nextStateIndx = j;

                                if (j + 1 < signalStates.Count && signalStates[i].Name.Equals("stable"))
                                    if (!signalStates[j + 1].Name.Equals(signalStates[i + 1].Name))
                                        break;
                            }
                            for (int j = i - 1; j > -1; j--)
                            {
                                if (!signalStates[i].Name.Equals("stable"))
                                    if (signalStates[j].Name.Equals(signalStates[i].Name))
                                        break;

                                previousStateIndx = j;

                                if (j - 1 > -1 && signalStates[i].Name.Equals("stable"))
                                    if (!signalStates[j - 1].Name.Equals(signalStates[i - 1].Name))
                                        break;
                            }
                            ptSelectionSamp.insertFeature(2,
                                                          ARTHTNamings.StAmp,
                                                          ((signalStates[i]._value * 2) - signalStates[previousStateIndx]._value - signalStates[nextStateIndx]._value) / 2);
                            flowLayoutItems03.DropDownItems.Add("((ampSt" + i + " - ampSt" + (previousStateIndx) + ") + " + "(ampSt" + i + " - ampSt" + nextStateIndx + ")) / 2" + ": " + ptSelectionSamp.getFeatures()[2]);
                            flowLayoutItems02.DropDownItems.Add(flowLayoutItems03);

                            // Set the ooutputs of the state
                            flowLayoutItems03 = new ToolStripMenuItem(ARTHTNamings.Outputs);
                            int stateIndx = signalStates[i]._index + _arthtFeatures.SignalBeats[selectedBeatIndx]._startingIndex;
                            // Initialize the output as there are no P and T wave
                            ptSelectionSamp.insertOutputArray(new string[] { ARTHTNamings.PWave, ARTHTNamings.TWave },
                                                                  new double[] { 0, 0 });
                            // Check if outputs should be predicted
                            if (_predictionOn)
                            {
                                ptSelectionSamp.insertOutputArray(new string[] { ARTHTNamings.PWave, ARTHTNamings.TWave },
                                                                  askForPrediction(ptSelectionSamp.getFeatures(), ARTHTNamings.Step4PTSelectionData));
                                // Check if state is predicted as P or T peak
                                if (ptSelectionSamp.getOutputByLabel(ARTHTNamings.PWave) > pProba)
                                {
                                    // If yes then this one is selected as P peak
                                    // set its index in ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][1]
                                    _arthtFeatures.SignalBeats[selectedBeatIndx]._pIndex = stateIndx;
                                    pProba = ptSelectionSamp.getOutputByLabel(ARTHTNamings.PWave);
                                }
                                if (ptSelectionSamp.getOutputByLabel(ARTHTNamings.TWave) > tProba)
                                {
                                    // If yes then this one is selected as T peak
                                    // set its index in ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][6]
                                    _arthtFeatures.SignalBeats[selectedBeatIndx]._tIndex = stateIndx;
                                    tProba = ptSelectionSamp.getOutputByLabel(ARTHTNamings.TWave);
                                }
                            }
                            else
                            {
                                if ((stateIndx - 5) <= _arthtFeatures.SignalBeats[selectedBeatIndx]._pIndex && _arthtFeatures.SignalBeats[selectedBeatIndx]._pIndex <= (stateIndx + 5))
                                    ptSelectionSamp.insertOutput(0, ARTHTNamings.PWave, 1);
                                if ((stateIndx - 5) <= _arthtFeatures.SignalBeats[selectedBeatIndx]._tIndex && _arthtFeatures.SignalBeats[selectedBeatIndx]._tIndex <= (stateIndx + 5))
                                    ptSelectionSamp.insertOutput(1, ARTHTNamings.TWave, 1);
                            }
                            flowLayoutItems03.DropDownItems.Add("P wave: " + ptSelectionSamp.getOutputByLabel(ARTHTNamings.PWave));
                            flowLayoutItems03.DropDownItems.Add("T wave: " + ptSelectionSamp.getOutputByLabel(ARTHTNamings.TWave));
                            flowLayoutItems02.DropDownItems.Add(flowLayoutItems03);
                            flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);
                        }
                        featuresItems.DropDownItems.Add(flowLayoutItems01);

                        // Check if there exist next beat
                        selectedBeatIndx = featuresItems.DropDownItems.Count;
                        if (selectedBeatIndx < _arthtFeatures.SignalBeats.Count)
                        {
                            // If yes then set the next beat for segmentation
                            setNextBeat(_arthtFeatures.SignalBeats[selectedBeatIndx], _arthtFeatures.StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].Samples[selectedBeatIndx], null);

                            // Give the instruction for next goal, and enable previous button
                            featuresSettingInstructionsLabel.Text = "Select P and T peaks from the visible states in the chart.\nClick once on the peak to select, or click once again to unselect.\n" + (selectedBeatIndx + 1) + "/" + _arthtFeatures.SignalBeats.Count + "\nPress next after you finish.";
                        }
                        else
                        {
                            // Make the form ready for next goal
                            _arthtFeatures._processedStep++;
                            // Add the check box of short PR declaration in filtersFlowLayoutPanel
                            ExistanceDeclare existanceDeclare = new ExistanceDeclare(_FilteringTools, "Existance of short PR");
                            existanceDeclare.InsertFilter(filtersFlowLayoutPanel);
                            // Show the first selected R
                            setNextBeat(_arthtFeatures.SignalBeats[0], _arthtFeatures.StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].Samples[0], null);

                            // Give the instruction for next goal, and enable previous button
                            featuresSettingInstructionsLabel.Text = "Check the box in the right list of filters if short PR is detected.\n" + 1 + "/" + _arthtFeatures.SignalBeats.Count + "\nPress next after you finish.";
                        }
                        break;
                    case 5:
                        // This is for short PR detection
                        // Get previous saved features if exists
                        if (featuresTableLayoutPanel.Controls.Count == 5)
                            featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[ARTHTNamings.Step5ShortPRScanData]).Items[0];
                        else
                            featuresItems = new ToolStripMenuItem(ARTHTNamings.Step5ShortPRScanData) { Name = ARTHTNamings.Step5ShortPRScanData };

                        // Get the index of selected beat
                        selectedBeatIndx = featuresItems.DropDownItems.Count;

                        Sample shortPRScanSamp = new Sample(ARTHTNamings.Step5ShortPRScanData + selectedBeatIndx, 1, 1, _arthtFeatures.StepsDataDic[ARTHTNamings.Step5ShortPRScanData]);
                        // Set the input and output of detecting short PR of the selected beat
                        flowLayoutItems01 = new ToolStripMenuItem(ARTHTNamings.Beat + selectedBeatIndx);
                        // Set the inputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Features);
                        // Check if P state exists
                        if (_arthtFeatures.SignalBeats[selectedBeatIndx]._pIndex != int.MinValue)
                        {
                            shortPRScanSamp.insertFeature(0,
                                                          ARTHTNamings.PQIntrvl,
                                                          (double)(_arthtFeatures.SignalBeats[selectedBeatIndx]._qIndex - _arthtFeatures.SignalBeats[selectedBeatIndx]._pIndex) /
                                                          (double)(_arthtFeatures.SignalBeats[selectedBeatIndx]._rIndex - _arthtFeatures.SignalBeats[selectedBeatIndx]._pIndex));
                            flowLayoutItems02.DropDownItems.Add(ARTHTNamings.PQIntrvl + ": " + shortPRScanSamp.getFeatureByLabel(ARTHTNamings.PQIntrvl));
                        }
                        else
                            flowLayoutItems02.DropDownItems.Add(ARTHTNamings.PQIntrvl + ": NaN");
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        // Set the outputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Outputs);
                        shortPRScanSamp.insertOutput(0,
                                                    ARTHTNamings.ShortPR,
                                                    0);
                        // Check if P state exists
                        if (_arthtFeatures.SignalBeats[selectedBeatIndx]._pIndex != int.MinValue)
                        {
                            // Check if outputs should be predicted
                            if (_predictionOn)
                                shortPRScanSamp.insertOutputArray(new string[] { ARTHTNamings.ShortPR },
                                                             askForPrediction(shortPRScanSamp.getFeatures(), ARTHTNamings.Step5ShortPRScanData));
                            else
                                shortPRScanSamp.insertOutput(0,
                                                             ARTHTNamings.ShortPR,
                                                             ((ExistanceDeclare)_FilteringTools._FiltersDic[ARTHTFiltersNames.ExistanceDeclare])._exists);
                            flowLayoutItems02.DropDownItems.Add(ARTHTNamings.ShortPR + ": " + shortPRScanSamp.getOutputByLabel(ARTHTNamings.ShortPR));
                        }
                        else
                            flowLayoutItems02.DropDownItems.Add(ARTHTNamings.ShortPR + ": NaN");
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);
                        featuresItems.DropDownItems.Add(flowLayoutItems01);

                        // Check if there exist next beat
                        selectedBeatIndx = featuresItems.DropDownItems.Count;
                        int shortPRNmbr = 0;
                        int shortPRBeatIndx = -1;
                        if (selectedBeatIndx < _arthtFeatures.SignalBeats.Count)
                        {
                            // If yes then set the next beat for segmentation
                            // Uncheck short PR declaration in filtersFlowLayoutPanel
                            ((ExistanceDeclare)_FilteringTools._FiltersDic[ARTHTFiltersNames.ExistanceDeclare]).SetExistance(false);
                            // Refresh the apply button if autoApply is checked
                            setNextBeat(_arthtFeatures.SignalBeats[selectedBeatIndx], _arthtFeatures.StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].Samples[selectedBeatIndx], null);

                            // Give the instruction for next goal, and enable previous button
                            featuresSettingInstructionsLabel.Text = "Check the box in the right list of filters if short PR is detected.\n" + (selectedBeatIndx + 1) + "/" + _arthtFeatures.SignalBeats.Count + "\nPress next after you finish.";
                        }
                        else
                        {
                            // Make the form ready for next goal
                            _arthtFeatures._processedStep++;
                            // Remove the check box of short PR declaration in filtersFlowLayoutPanel
                            _FilteringTools._FiltersDic[ARTHTFiltersNames.ExistanceDeclare].RemoveFilter();
                            // Get short PR declaration step threshold from the model if prediction is activated
                            if (_predictionOn)
                                threshold = _arthtModelsDic[modelTypeComboBox.Text].ARTHTModelsDic[ARTHTNamings.Step5ShortPRScanData].OutputsThresholds[0];
                            // Get number of declared short PR
                            for (int i = 0; i < _arthtFeatures.StepsDataDic[ARTHTNamings.Step5ShortPRScanData].Samples.Count; i++)
                                if (_arthtFeatures.StepsDataDic[ARTHTNamings.Step5ShortPRScanData].Samples[i].getOutputByLabel(ARTHTNamings.ShortPR) >= threshold)
                                {
                                    if (shortPRBeatIndx == -1)
                                        shortPRBeatIndx = i;
                                    shortPRNmbr++;
                                }
                            // Check if there exist short PR beats
                            if (shortPRNmbr > 0)
                            {
                                // If yes then show the first beat with short PR
                                _FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]._FilterControl.Enabled = true;
                                // Enable tangent deviation scan
                                ((SignalStatesViewerUserControl)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]._FilterControl).tdtThresholdScrollBar.Enabled = true;
                                ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]).ActivateTDT(true);
                                // Remove normalization
                                _FilteringTools._FiltersDic[ARTHTFiltersNames.Normalize].RemoveFilter();
                                // Refresh the apply button if autoApply is checked
                                setNextBeat(_arthtFeatures.SignalBeats[shortPRBeatIndx], _arthtFeatures.StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].Samples[shortPRBeatIndx], null);

                                // Give the instruction for next goal, and enable previous button
                                featuresSettingInstructionsLabel.Text = "Set the best \"tangent deviation threshold (TDT) threshold\" where the state before R must be as high as possible for delta of WPW syndrome detection.\n" + 1 + "/" + shortPRNmbr + "\nPress next after you finish.";
                            }
                            else
                            {
                                // Show finish button
                                // Clear all filters from filtersFlowLayoutPanel
                                _FilteringTools.RemoveAllFilters();

                                // Set next button to finish
                                nextButton.Text = "Finish";

                                // Remove instruction
                                featuresSettingInstructionsLabel.Text = "";
                            }
                        }
                        break;
                    case 6:
                        // This is for delta detection
                        // Get previous saved features if exists
                        if (featuresTableLayoutPanel.Controls.Count == 6)
                            featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[ARTHTNamings.Step6UpstrokesScanData]).Items[0];
                        else
                            featuresItems = new ToolStripMenuItem(ARTHTNamings.Step6UpstrokesScanData) { Name = ARTHTNamings.Step6UpstrokesScanData };

                        // Get short PR declaration step threshold from the model if prediction is activated
                        if (_predictionOn)
                            threshold = _arthtModelsDic[modelTypeComboBox.Text].ARTHTModelsDic[ARTHTNamings.Step5ShortPRScanData].OutputsThresholds[0];
                        // Get current short PR beat and next short PR beat
                        shortPRNmbr = 0;
                        shortPRBeatIndx = -1;
                        int nextShortPRBeatIndx = -1;
                        for (int i = 0; i < _arthtFeatures.StepsDataDic[ARTHTNamings.Step5ShortPRScanData].Samples.Count; i++)
                            if (_arthtFeatures.StepsDataDic[ARTHTNamings.Step5ShortPRScanData].Samples[i].getOutputByLabel(ARTHTNamings.ShortPR) >= threshold)
                            {
                                if (shortPRNmbr == _arthtFeatures.StepsDataDic[ARTHTNamings.Step6UpstrokesScanData].Samples.Count)
                                    shortPRBeatIndx = i;
                                else if (shortPRBeatIndx != -1 && nextShortPRBeatIndx == -1)
                                    nextShortPRBeatIndx = i;
                                shortPRNmbr++;
                            }

                        // Set the input and output of detecting short PR of the selected beat
                        flowLayoutItems01 = new ToolStripMenuItem(ARTHTNamings.Beat + shortPRBeatIndx);
                        Sample upstrokeScanSamp = new Sample(ARTHTNamings.Step6UpstrokesScanData + shortPRBeatIndx, 6, 1, _arthtFeatures.StepsDataDic[ARTHTNamings.Step6UpstrokesScanData]);

                        // Set the inputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Features);
                        upstrokeScanSamp.insertFeature(0, ARTHTNamings.PQIntrvl, _arthtFeatures.StepsDataDic[ARTHTNamings.Step5ShortPRScanData].getFeatureByLabel(shortPRBeatIndx, ARTHTNamings.PQIntrvl));
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.PQIntrvl + ": " + upstrokeScanSamp.getFeatureByLabel(ARTHTNamings.PQIntrvl));

                        statParams = Garage.statParams(Garage.normalizeSignal(_FilteringTools._FilteredSamples));
                        for (int j = 0; j < statParams.Count; j++)
                        {
                            upstrokeScanSamp.insertFeature(j + 1, statParams[j].Name, statParams[j]._value);
                            flowLayoutItems02.DropDownItems.Add(statParams[j].Name + ": " + statParams[j]._value.ToString());
                        }
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        // Set the outputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Outputs);
                        // Check if outputs should be predicted
                        if (_predictionOn)
                        {
                            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
                            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
                            upstrokeScanSamp.insertOutputArray(new string[] { ARTHTNamings.TDT }, askForPrediction(upstrokeScanSamp.getFeatures(), ARTHTNamings.Step6UpstrokesScanData));
                            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
                            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://

                            /*// Calculate it automatically where tangent deviation of the state before R must be the highest
                            // Iterate through all possible tangent deviations
                            double[] selectedTdtVal = new double[10];
                            double tdt = upstrokeScanSamp.getOutputByLabel(ARTHTNamings.TDT);
                            int start = (tdt * 100000) - 5 > 0 ? (int)((tdt * 100000) - 5) : 1;
                            int end = (tdt * 100000) + 5 < 100000 ? (int)((tdt * 100000) + 5) : 100000;
                            for (int i = start; i < end; i++)
                            {
                                signalStates = Garage.scanPeaks(_filteredSamples, signalStatesViewerUserControl._thresholdRatio,
                                                                signalStatesViewerUserControl.hThresholdScrollBar.Value, i / 100000d, _samplingRate, true);
                                // Iterate through all states and take the one before R if existed
                                int selectedSt = 0;
                                for (int j = 0; j < signalStates.Count; j++)
                                    if (signalStates[j]._index + _arthtFeatures.SignalBeats[shortPRBeatIndx]._startingIndex == _arthtFeatures.SignalBeats[shortPRBeatIndx]._rIndex)
                                    {
                                        if (j > 0)
                                            selectedSt = j - 1;
                                        break;
                                    }
                                // Check if its just Q state
                                if (signalStates[selectedSt]._index + _arthtFeatures.SignalBeats[shortPRBeatIndx]._startingIndex == _arthtFeatures.SignalBeats[shortPRBeatIndx]._qIndex)
                                {
                                    // If yes then select R state
                                    selectedTdtVal[i - start] = signalStates[selectedSt + 1]._deviantionAngle;
                                }
                            }
                            // Now select the one with the highest deviation
                            double highestNmbr = 0;
                            for (int i = 0; i < 10; i++)
                                if (selectedTdtVal[i] > highestNmbr)
                                {
                                    highestNmbr = selectedTdtVal[i];
                                    upstrokeScanSamp.insertOutput(0, ARTHTNamings.TDT, (double)(i + start) / 100000d);
                                }*/
                            // Set the selected _tdtThresholdRatio
                            ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]).SetTDT(upstrokeScanSamp.getOutputByLabel(ARTHTNamings.TDT));
                            // Apply peaks scan with the new tangent deviation threshold parameter
                            _FilteringTools.ApplyFilters(false);
                        }
                        else
                            upstrokeScanSamp.insertOutput(0, ARTHTNamings.TDT, ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer])._tdt);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.TDT + ": " + upstrokeScanSamp.getOutputByLabel(ARTHTNamings.TDT));
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        featuresItems.DropDownItems.Add(flowLayoutItems01);

                        // Set delta index of the beat
                        signalStates = ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer])._StatesDIc[SANamings.AllPeaks];
                        for (int i = 0; i < signalStates.Count; i++)
                        {
                            int qStateIndx = _arthtFeatures.SignalBeats[shortPRBeatIndx]._qIndex - _arthtFeatures.SignalBeats[shortPRBeatIndx]._startingIndex;
                            int rStateIndx = _arthtFeatures.SignalBeats[shortPRBeatIndx]._rIndex - _arthtFeatures.SignalBeats[shortPRBeatIndx]._startingIndex;
                            //if ((stateIndx - 1) <= (int)signalStates[i][1] && (int)signalStates[i][1] <= (stateIndx + 1))
                            if (qStateIndx <= signalStates[i]._index && signalStates[i]._index < rStateIndx)
                            {
                                _arthtFeatures.SignalBeats[shortPRBeatIndx]._deltaDetected = true;
                                _arthtFeatures.SignalBeats[shortPRBeatIndx]._slurredUpstrokeIndex = signalStates[i]._index + _arthtFeatures.SignalBeats[shortPRBeatIndx]._startingIndex;
                            }
                        }

                        // Check if there exist next beat
                        if (nextShortPRBeatIndx != -1)
                        {
                            // If yes then set the next short PR beat
                            setNextBeat(_arthtFeatures.SignalBeats[nextShortPRBeatIndx], _arthtFeatures.StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].Samples[nextShortPRBeatIndx], null);

                            // Give the instruction for next goal, and enable previous button
                            featuresSettingInstructionsLabel.Text = "Set the best \"tangent deviation threshold (TDT) threshold\" where the state before R must be as high as possible for delta of WPW syndrome detection.\n" + (featuresItems.DropDownItems.Count + 1) + "/" + shortPRNmbr + "\nPress next after you finish.";
                        }
                        else
                        {
                            // Make the form ready for next goal
                            _arthtFeatures._processedStep++;
                            // Add the check box of delta declaration in filtersFlowLayoutPanel
                            ExistanceDeclare existanceDeclare = new ExistanceDeclare(_FilteringTools, "Existance of delta");
                            existanceDeclare.InsertFilter(filtersFlowLayoutPanel);
                            // Get number of declared short PR
                            shortPRBeatIndx = -1;
                            shortPRNmbr = 0;
                            for (int i = 0; i < _arthtFeatures.StepsDataDic[ARTHTNamings.Step5ShortPRScanData].Samples.Count; i++)
                                if (_arthtFeatures.StepsDataDic[ARTHTNamings.Step5ShortPRScanData].Samples[i].getOutputByLabel(ARTHTNamings.ShortPR) >= threshold)
                                {
                                    if (shortPRBeatIndx == -1)
                                        shortPRBeatIndx = i;
                                    shortPRNmbr++;
                                }
                            // Disable peaks analyzer
                            _FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]._FilterControl.Enabled = false;
                            // Show the first beat with short PR
                            // Refresh the apply button if autoApply is checked
                            setNextBeat(_arthtFeatures.SignalBeats[shortPRBeatIndx], _arthtFeatures.StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].Samples[shortPRBeatIndx], _arthtFeatures.StepsDataDic[ARTHTNamings.Step6UpstrokesScanData].Samples[0]);

                            // Give the instruction for next goal, and enable previous button
                            featuresSettingInstructionsLabel.Text = "Check the box in the right list of filters if delta of WPW syndrome is detected.\n" + 1 + "/" + shortPRNmbr + "\nPress next after you finish.";
                        }
                        break;
                    case 7:
                        // This is for delta selection
                        // Get previous saved features if exists
                        if (featuresTableLayoutPanel.Controls.Count == 7)
                            featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[ARTHTNamings.Step7DeltaExaminationData]).Items[0];
                        else
                            featuresItems = new ToolStripMenuItem(ARTHTNamings.Step7DeltaExaminationData) { Name = ARTHTNamings.Step7DeltaExaminationData };

                        // Get short PR declaration step threshold from the model if prediction is activated
                        if (_predictionOn)
                            threshold = _arthtModelsDic[modelTypeComboBox.Text].ARTHTModelsDic[ARTHTNamings.Step5ShortPRScanData].OutputsThresholds[0];
                        // Get current short PR beat and next short PR beat
                        shortPRNmbr = 0;
                        shortPRBeatIndx = -1;
                        nextShortPRBeatIndx = -1;
                        for (int i = 0; i < _arthtFeatures.StepsDataDic[ARTHTNamings.Step5ShortPRScanData].Samples.Count; i++)
                            if (_arthtFeatures.StepsDataDic[ARTHTNamings.Step5ShortPRScanData].Samples[i].getOutputByLabel(ARTHTNamings.ShortPR) >= threshold)
                            {
                                if (shortPRNmbr == _arthtFeatures.StepsDataDic[ARTHTNamings.Step7DeltaExaminationData].Samples.Count)
                                    shortPRBeatIndx = i;
                                else if (shortPRBeatIndx != -1 && nextShortPRBeatIndx == -1)
                                    nextShortPRBeatIndx = i;
                                shortPRNmbr++;
                            }

                        // Set the input and output of examining delta of the selected beat
                        flowLayoutItems01 = new ToolStripMenuItem(ARTHTNamings.Beat + shortPRBeatIndx);
                        Sample deltaExamSamp = new Sample(ARTHTNamings.Step7DeltaExaminationData + shortPRBeatIndx, 6, 1, _arthtFeatures.StepsDataDic[ARTHTNamings.Step7DeltaExaminationData]);

                        // Set the inputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Features);

                        deltaExamSamp.insertFeature(0, ARTHTNamings.DeltaAmp,
                                                    (double)(_arthtFeatures.SignalBeats[shortPRBeatIndx]._slurredUpstrokeIndex - _arthtFeatures.SignalBeats[shortPRBeatIndx]._qIndex) /
                                                    (double)(_arthtFeatures.SignalBeats[shortPRBeatIndx]._rIndex - _arthtFeatures.SignalBeats[shortPRBeatIndx]._qIndex));
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.DeltaAmp + ": " + deltaExamSamp.getFeatureByLabel(ARTHTNamings.DeltaAmp));

                        statParams = Garage.statParams(Garage.normalizeSignal(_FilteringTools._FilteredSamples));
                        for (int j = 0; j < statParams.Count; j++)
                        {
                            deltaExamSamp.insertFeature(j + 1, statParams[j].Name, statParams[j]._value);
                            flowLayoutItems02.DropDownItems.Add(statParams[j].Name + ": " + statParams[j]._value.ToString());
                        }
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        // Set the outputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Outputs);
                        // Check if outputs should be predicted
                        if (_predictionOn)
                            deltaExamSamp.insertOutputArray(new string[] { ARTHTNamings.WPWPattern }, askForPrediction(deltaExamSamp.getFeatures(), ARTHTNamings.Step7DeltaExaminationData));
                        else
                            deltaExamSamp.insertOutput(0, ARTHTNamings.WPWPattern,
                                                       ((ExistanceDeclare)_FilteringTools._FiltersDic[ARTHTFiltersNames.ExistanceDeclare])._exists);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.WPWPattern + ": " + deltaExamSamp.getOutputByLabel(ARTHTNamings.WPWPattern));
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        featuresItems.DropDownItems.Add(flowLayoutItems01);

                        // Get WPW declaration step threshold from the model if prediction is activated
                        if (_predictionOn)
                            threshold = _arthtModelsDic[modelTypeComboBox.Text].ARTHTModelsDic[ARTHTNamings.Step7DeltaExaminationData].OutputsThresholds[0];
                        // Set WPW syndrome index if existed
                        if (deltaExamSamp.getOutputByLabel(ARTHTNamings.WPWPattern) > threshold)
                            _arthtFeatures.SignalBeats[shortPRBeatIndx]._wpwDetected = true;

                        // Check if there exist next beat
                        if (nextShortPRBeatIndx != -1)
                        {
                            // If yes then set the next short PR beat
                            // Uncheck delta existance in filtersFlowLayoutPanel
                            ((ExistanceDeclare)_FilteringTools._FiltersDic[ARTHTFiltersNames.ExistanceDeclare]).SetExistance(false);
                            setNextBeat(_arthtFeatures.SignalBeats[nextShortPRBeatIndx], _arthtFeatures.StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].Samples[nextShortPRBeatIndx], _arthtFeatures.StepsDataDic[ARTHTNamings.Step6UpstrokesScanData].Samples[featuresItems.DropDownItems.Count]);

                            // Give the instruction for next goal, and enable previous button
                            featuresSettingInstructionsLabel.Text = "Set the best \"tangent deviation threshold (TDT) threshold\" where the state before R must be as high as possible for delta of WPW syndrome detection.\n" + (featuresItems.DropDownItems.Count + 1) + "/" + shortPRNmbr + "\nPress next after you finish.";
                        }
                        else
                        {
                            _arthtFeatures._processedStep++;
                            // Clear all filters from filtersFlowLayoutPanel
                            _FilteringTools.RemoveAllFilters();

                            // Set next button to finish
                            nextButton.Text = "Finish";

                            // Remove instruction
                            featuresSettingInstructionsLabel.Text = "";
                        }
                        break;
                }

                // Show selected features in featuresFlowLayoutPanel
                MenuStrip selectedFeature = new MenuStrip();
                selectedFeature.Name = featuresItems.Name;
                selectedFeature.Items.Add(featuresItems);
                if (featuresTableLayoutPanel.Controls.ContainsKey(selectedFeature.Name))
                    featuresTableLayoutPanel.Controls.RemoveByKey(selectedFeature.Name);
                featuresTableLayoutPanel.Controls.Add(selectedFeature);

                // Check if prediction is on
                if (_predictionOn)
                {
                    // If yes then press next button
                    Thread nextButtonClickThread = new Thread(() => nextButton_Click(sender, e));
                    nextButtonClickThread.Start();
                }
            }));
        }

        private void setNextBeat(Beat BeatInfo, Sample ARTHTSample, Sample TDTSample)
        {
            _FilteringTools._RawSamples = new double[(BeatInfo._endingIndex - BeatInfo._startingIndex) + 1]; ;
            for (int i = BeatInfo._startingIndex; i < BeatInfo._endingIndex + 1; i++)
                _FilteringTools._RawSamples[i - BeatInfo._startingIndex] = _FilteringTools._OriginalRawSamples[i];

            foreach (string statesLabel in new string[] { SANamings.UpPeaks, SANamings.DownPeaks, SANamings.StableStates })
                signalChart.Series[statesLabel].Points.Clear();
            ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]).SetART(ARTHTSample.getOutputByLabel(ARTHTNamings.ART));
            ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]).SetHT(ARTHTSample.getOutputByLabel(ARTHTNamings.HT));
            if (TDTSample != null)
                ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]).SetTDT(TDTSample.getOutputByLabel(ARTHTNamings.TDT));
            // Apply changes
            _FilteringTools.ApplyFilters(false);

            // Set the up peaks labels ready
            Dictionary<string, List<State>> statesDic = ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer])._StatesDIc;
            foreach (string statesLabel in new string[] { SANamings.UpPeaks, SANamings.DownPeaks, SANamings.StableStates })
            {
                signalChart.Series[statesLabel].LabelForeColor = Color.Black;
                for (int j = 0; j < statesDic[statesLabel].Count; j++)
                {
                    int stateIndx = statesDic[statesLabel][j]._index + BeatInfo._startingIndex;
                    if (stateIndx == BeatInfo._pIndex)
                        signalChart.Series[statesLabel].Points[j].Label = SANamings.P;
                    else if (stateIndx == BeatInfo._qIndex)
                        signalChart.Series[statesLabel].Points[j].Label = SANamings.Q;
                    else if (stateIndx == BeatInfo._slurredUpstrokeIndex)
                    {
                        signalChart.Series[statesLabel].Points[j].Label = SANamings.Delta;
                        if (BeatInfo._wpwDetected)
                            signalChart.Series[statesLabel].Points[j].Label = SANamings.WPW;
                    }
                    else if (stateIndx == BeatInfo._rIndex)
                        signalChart.Series[statesLabel].Points[j].Label = SANamings.R;
                    else if (stateIndx == BeatInfo._sIndex)
                        signalChart.Series[statesLabel].Points[j].Label = SANamings.S;
                    else if (stateIndx == BeatInfo._tIndex)
                        signalChart.Series[statesLabel].Points[j].Label = SANamings.T;
                    else
                        signalChart.Series[statesLabel].Points[j].Label = "";
                }
            }
        }

        public void finish()
        {
            // Show signal with peaks labels
            _FilteringTools._RawSamples = new double[_FilteringTools._OriginalRawSamples.Length];
            for (int i = 0; i < _FilteringTools._RawSamples.Length; i++)
                _FilteringTools._RawSamples[i] = _FilteringTools._OriginalRawSamples[i];
            _FilteringTools.ApplyFilters(false);
            double[] samples = _FilteringTools._FilteredSamples;

            List<object[]> peaksLabels = new List<object[]>();
            foreach (Beat beat in _arthtFeatures.SignalBeats)
            {
                if (beat._pIndex != int.MinValue)
                    peaksLabels.Add(new object[] { (double)beat._pIndex, samples[beat._pIndex], SANamings.P });
                if (beat._qIndex != int.MinValue)
                    peaksLabels.Add(new object[] { (double)beat._qIndex, samples[beat._qIndex], SANamings.Q });
                if (beat._slurredUpstrokeIndex != int.MinValue)
                {
                    if (beat._wpwDetected)
                        peaksLabels.Add(new object[] { (double)beat._slurredUpstrokeIndex, samples[beat._slurredUpstrokeIndex], SANamings.WPW });
                    else
                        peaksLabels.Add(new object[] { (double)beat._slurredUpstrokeIndex, samples[beat._slurredUpstrokeIndex], SANamings.Delta });
                }
                if (beat._rIndex != int.MinValue)
                    peaksLabels.Add(new object[] { (double)beat._rIndex, samples[beat._rIndex], SANamings.R });
                if (beat._sIndex != int.MinValue)
                    peaksLabels.Add(new object[] { (double)beat._sIndex, samples[beat._sIndex], SANamings.S });
                if (beat._tIndex != int.MinValue)
                    peaksLabels.Add(new object[] { (double)beat._tIndex, samples[beat._tIndex], SANamings.T });
            }

            for (int i = 1; i < 5; i++)
                signalChart.Series[i].Points.Clear();
            Garage.loadXYInChart(signalChart,
                                 peaksLabels.Select(labelX => (double)labelX[0] / _FilteringTools._samplingRate).ToArray(),
                                 peaksLabels.Select(labelY => (double)labelY[1]).ToArray(),
                                 peaksLabels.Select(label => (string)label[2]).ToArray(),
                                 _FilteringTools._startingInSec, 5, "ARTHTFormDetailsModify");

            // Enable everything
            signalFusionButton.Enabled = true;
            signalsPickerComboBox.Enabled = true;
            filtersComboBox.Enabled = true;
            // Disable finish button
            nextButton.Enabled = false;

            // Enable save button in signal holder
            if (_signalHolder != null)
                _signalHolder.saveButton.Enabled = true;
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem featuresItems = null;

            // Check if last features array should be removed
            if (_arthtFeatures._processedStep == featuresTableLayoutPanel.Controls.Count + 1)
                _arthtFeatures._processedStep--;

            // Get short PR declaration step threshold from the model if prediction is activated
            float threshold = 0.5f;
            if (_predictionOn)
                threshold = _arthtModelsDic[modelTypeComboBox.Text].ARTHTModelsDic[ARTHTNamings.Step5ShortPRScanData].OutputsThresholds[0];
            // Check which step of features selectoin is this
            switch (_arthtFeatures._processedStep)
            {
                case 7:
                    // This is for delta declaration
                    if (nextButton.Text.Equals("Finish"))
                    {
                        // Disable save button in signal holder
                        _signalHolder.saveButton.Enabled = false;
                        // Disable everything except ai tools
                        enableAITools(true);

                        // Set dc removal filter. Set it disabled
                        DCRemoval dcRemoval = new DCRemoval(_FilteringTools);
                        dcRemoval.InsertFilter(filtersFlowLayoutPanel);
                        ((DCRemovalUserControl)dcRemoval._FilterControl).Enabled = false;

                        // Set peaks analizer
                        PeaksAnalyzer peaksAnalyzer = new PeaksAnalyzer(_FilteringTools);
                        peaksAnalyzer.InsertFilter(filtersFlowLayoutPanel);
                        ((SignalStatesViewerUserControl)peaksAnalyzer._FilterControl).showStatesCheckBox.Enabled = false;
                        ((SignalStatesViewerUserControl)peaksAnalyzer._FilterControl).showDeviationCheckBox.Enabled = false;
                        ((SignalStatesViewerUserControl)peaksAnalyzer._FilterControl).amplitudeThresholdScrollBar.Enabled = false;
                        ((SignalStatesViewerUserControl)peaksAnalyzer._FilterControl).hThresholdScrollBar.Enabled = false;
                        peaksAnalyzer.ActivateTDT(true);
                        peaksAnalyzer._FilterControl.Enabled = false;

                        // Add the check box of delta declaration in filtersFlowLayoutPanel
                        ExistanceDeclare existanceDeclare = new ExistanceDeclare(_FilteringTools, "Existance of delta");
                        existanceDeclare.InsertFilter(filtersFlowLayoutPanel);

                        // Enable next button
                        nextButton.Enabled = true;
                        nextButton.Text = "Next";
                    }

                    // Get previous saved features if exists
                    featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[ARTHTNamings.Step7DeltaExaminationData]).Items[0];
                    // Remove last feature from featuresItems if existed
                    featuresItems.DropDownItems.RemoveAt(featuresItems.DropDownItems.Count - 1);
                    _arthtFeatures.StepsDataDic[ARTHTNamings.Step7DeltaExaminationData].removeLastSample();

                    // Get previous short PR beat
                    int shortPRNmbr = 0;
                    int previousShortPRBeatIndx = -1;
                    for (int i = 0; i < _arthtFeatures.StepsDataDic[ARTHTNamings.Step5ShortPRScanData].Samples.Count; i++)
                        if (_arthtFeatures.StepsDataDic[ARTHTNamings.Step5ShortPRScanData].Samples[i].getOutputByLabel(ARTHTNamings.ShortPR) >= threshold)
                        {
                            if (shortPRNmbr == _arthtFeatures.StepsDataDic[ARTHTNamings.Step7DeltaExaminationData].Samples.Count)
                                previousShortPRBeatIndx = i;
                            shortPRNmbr++;
                        }
                    // Set the previous short PR beat
                    // Uncheck delta existance in filtersFlowLayoutPanel
                    ((ExistanceDeclare)_FilteringTools._FiltersDic[ARTHTFiltersNames.ExistanceDeclare]).SetExistance(false);
                    setNextBeat(_arthtFeatures.SignalBeats[previousShortPRBeatIndx], _arthtFeatures.StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].Samples[previousShortPRBeatIndx], _arthtFeatures.StepsDataDic[ARTHTNamings.Step6UpstrokesScanData].Samples[featuresItems.DropDownItems.Count]);

                    // Give the instruction for next goal, and enable previous button
                    featuresSettingInstructionsLabel.Text = "Set the best \"tangent deviation threshold (TDT) threshold\" where the state before R must be as high as possible for delta of WPW syndrome detection.\n" + (featuresItems.DropDownItems.Count + 1) + "/" + shortPRNmbr + "\nPress next after you finish.";
                    break;
                case 6:
                    // This is for delta detection
                    // Get previous saved features if exists
                    featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[ARTHTNamings.Step6UpstrokesScanData]).Items[0];
                    // Remove last feature from featuresItems if existed
                    featuresItems.DropDownItems.RemoveAt(featuresItems.DropDownItems.Count - 1);
                    _arthtFeatures.StepsDataDic[ARTHTNamings.Step6UpstrokesScanData].removeLastSample();

                    // Get previous short PR beat
                    shortPRNmbr = 0;
                    previousShortPRBeatIndx = -1;
                    for (int i = 0; i < _arthtFeatures.StepsDataDic[ARTHTNamings.Step5ShortPRScanData].Samples.Count; i++)
                        if (_arthtFeatures.StepsDataDic[ARTHTNamings.Step5ShortPRScanData].Samples[i].getOutputByLabel(ARTHTNamings.ShortPR) >= threshold)
                        {
                            if (shortPRNmbr == _arthtFeatures.StepsDataDic[ARTHTNamings.Step7DeltaExaminationData].Samples.Count)
                                previousShortPRBeatIndx = i;
                            shortPRNmbr++;
                        }

                    // Set last short PR for delta detection
                    if (featuresItems.DropDownItems.Count + 1 == shortPRNmbr)
                    {
                        // Remove the check box of delta declaration in filtersFlowLayoutPanel
                        _FilteringTools._FiltersDic[ARTHTFiltersNames.ExistanceDeclare].RemoveFilter();
                        // Enable peaks analyzer
                        ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer])._FilterControl.Enabled = true;
                    }
                    // Show the last beat with short PR
                    for (int i = 1; i < 4; i++)
                        signalChart.Series[i].Points.Clear();
                    setNextBeat(_arthtFeatures.SignalBeats[previousShortPRBeatIndx], _arthtFeatures.StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].Samples[previousShortPRBeatIndx], null);

                    // Give the instruction for next goal, and enable previous button
                    featuresSettingInstructionsLabel.Text = "Set the best \"tangent deviation threshold (TDT) threshold\" where the state before R must be as high as possible for delta of WPW syndrome detection.\n" + (featuresItems.DropDownItems.Count + 1) + "/" + shortPRNmbr + "\nPress next after you finish.";
                    break;
                case 5:
                    // This is for short PR detection
                    if (nextButton.Text.Equals("Finish"))
                    {
                        // Disable save button in signal holder
                        _signalHolder.saveButton.Enabled = false;
                        // Disable everything except ai tools
                        enableAITools(true);

                        // Set dc removal filter. Set it disabled
                        DCRemoval dcRemoval = new DCRemoval(_FilteringTools);
                        dcRemoval.InsertFilter(filtersFlowLayoutPanel);
                        ((DCRemovalUserControl)dcRemoval._FilterControl).Enabled = false;
                        Normalize normalize = new Normalize(_FilteringTools);
                        normalize.InsertFilter(filtersFlowLayoutPanel);
                        ((NormalizedSignalUserControl)normalize._FilterControl).Enabled = false;

                        // Set peaks analizer
                        PeaksAnalyzer peaksAnalyzer = new PeaksAnalyzer(_FilteringTools);
                        peaksAnalyzer.InsertFilter(filtersFlowLayoutPanel);
                        ((SignalStatesViewerUserControl)peaksAnalyzer._FilterControl).showStatesCheckBox.Enabled = false;
                        ((SignalStatesViewerUserControl)peaksAnalyzer._FilterControl).showDeviationCheckBox.Enabled = false;
                        ((SignalStatesViewerUserControl)peaksAnalyzer._FilterControl).tdtThresholdScrollBar.Enabled = false;
                        peaksAnalyzer._FilterControl.Enabled = false;

                        // Enable next button
                        nextButton.Enabled = true;
                        nextButton.Text = "Next";
                    }

                    // Get previous saved features if exists
                    featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[ARTHTNamings.Step5ShortPRScanData]).Items[0];
                    // Remove last feature from featuresItems if existed
                    featuresItems.DropDownItems.RemoveAt(featuresItems.DropDownItems.Count - 1);
                    _arthtFeatures.StepsDataDic[ARTHTNamings.Step5ShortPRScanData].removeLastSample();

                    // Add the check box of short PR declaration in filtersFlowLayoutPanel
                    if (featuresItems.DropDownItems.Count + 1 == _arthtFeatures.SignalBeats.Count)
                    {
                        ((SignalStatesViewerUserControl)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]._FilterControl).tdtThresholdScrollBar.Enabled = false;
                        ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]).ActivateTDT(false);
                        _FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]._FilterControl.Enabled = false;

                        ExistanceDeclare existanceDeclare = new ExistanceDeclare(_FilteringTools, "Existance of short PR");
                        existanceDeclare.InsertFilter(filtersFlowLayoutPanel);
                    }

                    // Uncheck short PR declaration in filtersFlowLayoutPanel
                    ((ExistanceDeclare)_FilteringTools._FiltersDic[ARTHTFiltersNames.ExistanceDeclare]).SetExistance(false);
                    // Refresh the apply button if autoApply is checked
                    for (int i = 1; i < 4; i++)
                        signalChart.Series[i].Points.Clear();
                    int selectedBeatIndx = featuresItems.DropDownItems.Count;
                    setNextBeat(_arthtFeatures.SignalBeats[selectedBeatIndx], _arthtFeatures.StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].Samples[selectedBeatIndx], null);

                    // Give the instruction for next goal, and enable previous button
                    featuresSettingInstructionsLabel.Text = "Check the box in the right list of filters if short PR is detected.\n" + (selectedBeatIndx + 1) + "/" + _arthtFeatures.SignalBeats.Count + "\nPress next after you finish.";
                    break;
                case 4:
                    // This is for P and T selection
                    // Get previous saved features if exists
                    featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[ARTHTNamings.Step4PTSelectionData]).Items[0];
                    // Remove last feature from featuresItems if existed
                    featuresItems.DropDownItems.RemoveAt(featuresItems.DropDownItems.Count - 1);
                    _arthtFeatures.StepsDataDic[ARTHTNamings.Step4PTSelectionData].removeLastSample();

                    // Remove the check box of short PR declaration in filtersFlowLayoutPanel
                    if (featuresItems.DropDownItems.Count + 1 == _arthtFeatures.SignalBeats.Count)
                        _FilteringTools._FiltersDic[ARTHTFiltersNames.ExistanceDeclare].RemoveFilter();

                    // Set the next beat for segmentation
                    for (int i = 1; i < 4; i++)
                        signalChart.Series[i].Points.Clear();
                    selectedBeatIndx = featuresItems.DropDownItems.Count;
                    setNextBeat(_arthtFeatures.SignalBeats[selectedBeatIndx], _arthtFeatures.StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].Samples[selectedBeatIndx], null);

                    // Give the instruction for next goal, and enable previous button
                    featuresSettingInstructionsLabel.Text = "Select P and T peaks from the visible states in the chart.\nClick once on the peak to select, or click once again to unselect.\n" + (selectedBeatIndx + 1) + "/" + _arthtFeatures.SignalBeats.Count + "\nPress next after you finish.";
                    break;
                case 3:
                    // This is for beats peaks detection (P, T)
                    // Get previous saved features if exists
                    featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[ARTHTNamings.Step3BeatPeaksScanData]).Items[0];
                    // Remove last feature from featuresItems if existed
                    featuresItems.DropDownItems.RemoveAt(featuresItems.DropDownItems.Count - 1);
                    _arthtFeatures.StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].removeLastSample();

                    // Set filters for this step
                    if (featuresItems.DropDownItems.Count + 1 == _arthtFeatures.SignalBeats.Count)
                    {
                        for (int i = 1; i < 4; i++)
                            signalChart.Series[i].LabelForeColor = Color.Transparent;

                        _FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]._FilterControl.Enabled = true;
                        ((SignalStatesViewerUserControl)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]._FilterControl).amplitudeThresholdScrollBar.Enabled = true;
                        ((SignalStatesViewerUserControl)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]._FilterControl).hThresholdScrollBar.Enabled = true;
                    }

                    // Set the previous beat for segmentation
                    int previousBeatIndex = featuresItems.DropDownItems.Count;
                    _FilteringTools._RawSamples = new double[(_arthtFeatures.SignalBeats[previousBeatIndex]._endingIndex - _arthtFeatures.SignalBeats[previousBeatIndex]._startingIndex) + 1];
                    for (int i = _arthtFeatures.SignalBeats[previousBeatIndex]._startingIndex; i < _arthtFeatures.SignalBeats[previousBeatIndex]._endingIndex + 1; i++)
                        _FilteringTools._RawSamples[i - _arthtFeatures.SignalBeats[previousBeatIndex]._startingIndex] = _FilteringTools._OriginalRawSamples[i];
                    ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]).SetHT(0.01d);

                    // Refresh the apply button if autoApply is checked
                    for (int i = 1; i < 4; i++)
                        signalChart.Series[i].Points.Clear();
                    _FilteringTools.ApplyFilters(false);

                    // Give the instruction for next goal, and enable previous button
                    featuresSettingInstructionsLabel.Text = "Set the best \"amplitude reatio threshold (ART)\" and \"horizontal threshold (HT)\" for the segmentation of P and T waves.\n" + (previousBeatIndex + 1) + "/" + _arthtFeatures.SignalBeats.Count + "\nPress next after you finish.";
                    break;
                case 2:
                    // This is for QRS selection
                    // Get previous saved features if exists
                    featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[ARTHTNamings.Step2RPeaksSelectionData]).Items[0];
                    // Remove last feature from featuresItems if existed
                    featuresItems.DropDownItems.Clear();
                    _arthtFeatures.StepsDataDic[ARTHTNamings.Step2RPeaksSelectionData].removeLastSample();

                    // Refresh the apply button if autoApply is checked
                    for (int i = 1; i < 4; i++)
                        signalChart.Series[i].Points.Clear();
                    _FilteringTools._RawSamples = new double[_FilteringTools._OriginalRawSamples.Length];
                    for (int i = 0; i < _FilteringTools._OriginalRawSamples.Length; i++)
                        _FilteringTools._RawSamples[i] = _FilteringTools._OriginalRawSamples[i];

                    // Get first 3 levels of DWT of the signal
                    DWT dwt = new DWT(_FilteringTools);
                    dwt.InsertFilter(filtersFlowLayoutPanel);
                    dwt.SetSortOrder(1);
                    dwt.SetMaxLevel(3);
                    ((DWTUserControl)dwt._FilterControl).waveletTypeComboBox.Enabled = false;
                    ((DWTUserControl)dwt._FilterControl).numOfVanMoComboBox.Enabled = false;
                    // set the signal to absolute after DWT transform
                    Absolute absolute = new Absolute(_FilteringTools);
                    absolute.InsertFilter(filtersFlowLayoutPanel);
                    absolute.SetSortOrder(2);
                    ((AbsoluteSignalUserControl)absolute._FilterControl).Enabled = false;

                    // Set the peaks analyzer with the parameters selected from the first step
                    ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]).SetART(_arthtFeatures.StepsDataDic[ARTHTNamings.Step1RPeaksScanData].getOutputByLabel(0, ARTHTNamings.ART));
                    ((PeaksAnalyzer)_FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]).SetHT(_arthtFeatures.StepsDataDic[ARTHTNamings.Step1RPeaksScanData].getOutputByLabel(0, ARTHTNamings.HT));
                    _FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]._FilterControl.Enabled = false;

                    // Disable showing results in chart
                    _FilteringTools.ShowResultInChart(false);
                    // Iterate through 3 levels of dwt
                    for (int i = 0; i < 3; i++)
                    {
                        // Normalize the absolute values of levelSignal
                        ((DWT)_FilteringTools._FiltersDic[ARTHTFiltersNames.DWT]).SelectLevel(i);
                        _FilteringTools.ApplyFilters(false);
                    }
                    // Reenable showing results in chart
                    _FilteringTools.ShowResultInChart(true);

                    // Remove DWT and absolute filters
                    dwt.RemoveFilter();
                    absolute.RemoveFilter();
                    // Show the signal
                    _FilteringTools.ApplyFilters(false);

                    // Set the up peaks labels ready
                    signalChart.Series[SANamings.UpPeaks].LabelForeColor = Color.Black;
                    for (int i = 0; i < signalChart.Series[SANamings.UpPeaks].Points.Count; i++)
                        signalChart.Series[SANamings.UpPeaks].Points[i].Label = "";

                    // Give the instruction for next goal, and enable previous button
                    featuresSettingInstructionsLabel.Text = "Select R peaks from the visible states in the chart.\nClick once on the peak to select, or click once again to unselect.\nPress next after you finish.";
                    break;
                case 1:
                    // This is for QRS detection
                    // Get previous saved features if exists
                    featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[ARTHTNamings.Step1RPeaksScanData]).Items[0];
                    // Remove last feature from featuresItems if existed
                    featuresItems.DropDownItems.Clear();
                    _arthtFeatures.StepsDataDic[ARTHTNamings.Step1RPeaksScanData].removeLastSample();

                    // Get first 3 levels of DWT of the signal
                    dwt = new DWT(_FilteringTools);
                    dwt.InsertFilter(filtersFlowLayoutPanel);
                    dwt.SetSortOrder(1);
                    dwt.SetMaxLevel(3);
                    ((DWTUserControl)dwt._FilterControl).waveletTypeComboBox.Enabled = false;
                    ((DWTUserControl)dwt._FilterControl).numOfVanMoComboBox.Enabled = false;
                    // set the signal to absolute after DWT transform
                    absolute = new Absolute(_FilteringTools);
                    absolute.InsertFilter(filtersFlowLayoutPanel);
                    absolute.SetSortOrder(2);
                    ((AbsoluteSignalUserControl)absolute._FilterControl).Enabled = false;
                    // Activate peaks analyzer filters
                    _FilteringTools._FiltersDic[ARTHTFiltersNames.PeaksAnalyzer]._FilterControl.Enabled = true;

                    // Set the up peaks labels ready
                    for (int i = 1; i < 4; i++)
                        signalChart.Series[i].Points.Clear();
                    signalChart.Series[SANamings.UpPeaks].LabelForeColor = Color.Transparent;

                    // Show the signal
                    _FilteringTools.ApplyFilters(false);

                    // Show instructions of the first goal
                    featuresSettingInstructionsLabel.Text = "Set the best \"amplitude reatio threshold (ART)\" and \"horizontal threshold (HT)\" so that only high peaks are visible.\nPress next after you finish.";

                    // Disable previous button
                    previousButton.Enabled = false;
                    break;
            }

            // Show selected features in featuresFlowLayoutPanel
            MenuStrip selectedFeature = new MenuStrip();
            selectedFeature.Name = featuresItems.Name;
            selectedFeature.Items.Add(featuresItems);
            if (featuresTableLayoutPanel.Controls.ContainsKey(selectedFeature.Name))
                featuresTableLayoutPanel.Controls.RemoveByKey(selectedFeature.Name);
            if (featuresItems.DropDownItems.Count > 0)
                featuresTableLayoutPanel.Controls.Add(selectedFeature);
        }

        public void initializeAITools()
        {
            // Set features in featuresTableLayoutPanel from _featuresOrderedDictionary
            ToolStripMenuItem featuresItems = null;
            ToolStripMenuItem flowLayoutItems01 = null;
            ToolStripMenuItem flowLayoutItems02 = null;
            ToolStripMenuItem flowLayoutItems03 = null;
            double[] inputs;
            double[] outputs;
            for (int i = 1; i < _arthtFeatures._processedStep; i++)
            {
                if (i == 1)
                {
                    // This is for QRS detection
                    featuresItems = new ToolStripMenuItem(ARTHTNamings.Step1RPeaksScanData) { Name = ARTHTNamings.Step1RPeaksScanData };

                    inputs = _arthtFeatures.StepsDataDic[ARTHTNamings.Step1RPeaksScanData].Samples[0].getFeatures();
                    outputs = _arthtFeatures.StepsDataDic[ARTHTNamings.Step1RPeaksScanData].Samples[0].getOutputs();
                    // Iterate through 3 levels of dwt
                    flowLayoutItems01 = new ToolStripMenuItem(ARTHTNamings.Features);
                    for (int j = 0; j < 3; j++)
                        for (int k = 0; k < 5; k++)
                        {
                            if (k == 0)
                                flowLayoutItems01.DropDownItems.Add(ARTHTNamings.Mean + (j + 1) + ": " + inputs[j * 5 + k]);
                            else if (k == 1)
                                flowLayoutItems01.DropDownItems.Add(ARTHTNamings.Min + (j + 1) + ": " + inputs[j * 5 + k]);
                            else if (k == 2)
                                flowLayoutItems01.DropDownItems.Add(ARTHTNamings.Max + (j + 1) + ": " + inputs[j * 5 + k]);
                            else if (k == 3)
                                flowLayoutItems01.DropDownItems.Add(ARTHTNamings.StdDev + (j + 1) + ": " + inputs[j * 5 + k]);
                            else
                                flowLayoutItems01.DropDownItems.Add(ARTHTNamings.IQR + (j + 1) + ": " + inputs[j * 5 + k]);
                        }
                    featuresItems.DropDownItems.Add(flowLayoutItems01);
                    // get signal states viewer user control and set the features output
                    flowLayoutItems01 = new ToolStripMenuItem(ARTHTNamings.Outputs);
                    flowLayoutItems01.DropDownItems.Add(ARTHTNamings.ART + ": " + outputs[0]);
                    flowLayoutItems01.DropDownItems.Add(ARTHTNamings.HT + ": " + outputs[1]);

                    featuresItems.DropDownItems.Add(flowLayoutItems01);
                }
                else if (i == 2)
                {
                    // This is for QRS selection
                    featuresItems = new ToolStripMenuItem(ARTHTNamings.Step2RPeaksSelectionData) { Name = ARTHTNamings.Step2RPeaksSelectionData };

                    for (int j = 0; j < _arthtFeatures.StepsDataDic[ARTHTNamings.Step2RPeaksSelectionData].Samples.Count; j++)
                    {
                        inputs = _arthtFeatures.StepsDataDic[ARTHTNamings.Step2RPeaksSelectionData].Samples[j].getFeatures();
                        outputs = _arthtFeatures.StepsDataDic[ARTHTNamings.Step2RPeaksSelectionData].Samples[j].getOutputs();
                        flowLayoutItems01 = new ToolStripMenuItem(ARTHTNamings.Beat + j);
                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Features);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.RIntrvl + ": " + inputs[0]);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.RAmp + ": " + inputs[1]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);
                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Outputs);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.RemoveR + ": " + outputs[0]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        featuresItems.DropDownItems.Add(flowLayoutItems01);
                    }
                }
                else if (i == 3)
                {
                    // This is for beats peaks detection (P, T)
                    featuresItems = new ToolStripMenuItem(ARTHTNamings.Step3BeatPeaksScanData) { Name = ARTHTNamings.Step3BeatPeaksScanData };

                    for (int j = 0; j < _arthtFeatures.StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].Samples.Count; j++)
                    {
                        inputs = _arthtFeatures.StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].Samples[j].getFeatures();
                        outputs = _arthtFeatures.StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].Samples[j].getOutputs();
                        flowLayoutItems01 = new ToolStripMenuItem(ARTHTNamings.Beat + j);
                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Features);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.Mean + ": " + inputs[0]);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.Min + ": " + inputs[1]);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.Max + ": " + inputs[2]);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.StdDev + ": " + inputs[3]);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.IQR + ": " + inputs[4]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);
                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Outputs);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.ART + ": " + outputs[0]);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.HT + ": " + outputs[1]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        featuresItems.DropDownItems.Add(flowLayoutItems01);
                    }
                }
                else if (i == 4)
                {
                    // This is for P and T selection
                    featuresItems = new ToolStripMenuItem(ARTHTNamings.Step4PTSelectionData) { Name = ARTHTNamings.Step4PTSelectionData };

                    // Data of all beats are combined in one list
                    // We need to separate each beat on its own
                    string prevBeatName = "";
                    int beatIndx = -1;
                    int stateIndx = 0;
                    for (int j = 0; j < _arthtFeatures.StepsDataDic[ARTHTNamings.Step4PTSelectionData].Samples.Count; j++)
                    {
                        // Check if beat name is changed
                        if (!_arthtFeatures.StepsDataDic[ARTHTNamings.Step4PTSelectionData].Samples[j].Name.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0].Equals(prevBeatName))
                        {
                            // Check if this is not the first beat asignment
                            if (beatIndx != -1)
                                // If yes then add previous beat to featuresItems
                                featuresItems.DropDownItems.Add(flowLayoutItems01);
                            // Proceed with the new beat
                            prevBeatName = _arthtFeatures.StepsDataDic[ARTHTNamings.Step4PTSelectionData].Samples[j].Name.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0];
                            beatIndx++;
                            flowLayoutItems01 = new ToolStripMenuItem(ARTHTNamings.Beat + beatIndx);
                            stateIndx = 0;
                        }

                        inputs = _arthtFeatures.StepsDataDic[ARTHTNamings.Step4PTSelectionData].Samples[j].getFeatures();
                        outputs = _arthtFeatures.StepsDataDic[ARTHTNamings.Step4PTSelectionData].Samples[j].getOutputs();

                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.State + stateIndx);
                        // Set the inputs of the state
                        flowLayoutItems03 = new ToolStripMenuItem(ARTHTNamings.Features);
                        flowLayoutItems03.DropDownItems.Add("ampSt" + stateIndx + ": " + inputs[0]);
                        if (stateIndx == 0)
                            flowLayoutItems03.DropDownItems.Add("(St" + stateIndx + "-R) / " + beatIndx + "(R1-R" + beatIndx + "): " + inputs[1]);
                        else
                            flowLayoutItems03.DropDownItems.Add("(St" + stateIndx + "-R" + beatIndx + ") / (R" + beatIndx + "-R" + (beatIndx - 1) + "): " + inputs[1]);
                        flowLayoutItems03.DropDownItems.Add("((ampSt" + stateIndx + " - ampSt" + (stateIndx - 1) + ") + " + "(ampSt" + stateIndx + " - ampSt" + (stateIndx + 1) + ")) / 2" + ": " + inputs[2]);
                        flowLayoutItems02.DropDownItems.Add(flowLayoutItems03);
                        // Set the ooutputs of the state
                        flowLayoutItems03 = new ToolStripMenuItem(ARTHTNamings.Outputs);
                        flowLayoutItems03.DropDownItems.Add(ARTHTNamings.PWave + ": " + outputs[0]);
                        flowLayoutItems03.DropDownItems.Add(ARTHTNamings.TWave + ": " + outputs[1]);
                        flowLayoutItems02.DropDownItems.Add(flowLayoutItems03);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        // Check if this is the last sample
                        if (j == _arthtFeatures.StepsDataDic[ARTHTNamings.Step4PTSelectionData].Samples.Count - 1)
                            // If yes then add the last beat data in featuresItems
                            featuresItems.DropDownItems.Add(flowLayoutItems01);

                        stateIndx++;
                    }
                }
                else if (i == 5)
                {
                    // This is for short PR detection
                    featuresItems = new ToolStripMenuItem(ARTHTNamings.Step5ShortPRScanData) { Name = ARTHTNamings.Step5ShortPRScanData };

                    for (int j = 0; j < _arthtFeatures.StepsDataDic[ARTHTNamings.Step5ShortPRScanData].Samples.Count; j++)
                    {
                        inputs = _arthtFeatures.StepsDataDic[ARTHTNamings.Step5ShortPRScanData].Samples[j].getFeatures();
                        outputs = _arthtFeatures.StepsDataDic[ARTHTNamings.Step5ShortPRScanData].Samples[j].getOutputs();

                        // Set the input and output of detecting short PR of the selected beat
                        flowLayoutItems01 = new ToolStripMenuItem(ARTHTNamings.Beat + j);
                        // Set the inputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Features);
                        // Check if P state exists
                        if (inputs != null)
                            flowLayoutItems02.DropDownItems.Add(ARTHTNamings.PQIntrvl + ": " + inputs[0]);
                        else
                            flowLayoutItems02.DropDownItems.Add(ARTHTNamings.PQIntrvl + ": NaN");
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        // Set the outputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Outputs);
                        // Check if P state exists
                        if (outputs != null)
                            flowLayoutItems02.DropDownItems.Add(ARTHTNamings.ShortPR + ": " + outputs[0]);
                        else
                            flowLayoutItems02.DropDownItems.Add(ARTHTNamings.ShortPR + ": NaN");
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);
                        featuresItems.DropDownItems.Add(flowLayoutItems01);
                    }
                }
                else if (i == 6)
                {
                    // This is for delta detection
                    featuresItems = new ToolStripMenuItem(ARTHTNamings.Step6UpstrokesScanData) { Name = ARTHTNamings.Step6UpstrokesScanData };

                    for (int j = 0; j < _arthtFeatures.StepsDataDic[ARTHTNamings.Step6UpstrokesScanData].Samples.Count; j++)
                    {
                        inputs = _arthtFeatures.StepsDataDic[ARTHTNamings.Step6UpstrokesScanData].Samples[j].getFeatures();
                        outputs = _arthtFeatures.StepsDataDic[ARTHTNamings.Step6UpstrokesScanData].Samples[j].getOutputs();

                        // Set the input and output of detecting short PR of the selected beat
                        flowLayoutItems01 = new ToolStripMenuItem(ARTHTNamings.Beat + j);
                        // Set the inputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Features);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.PQIntrvl + ": " + inputs[0]);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.Mean + ": " + inputs[1]);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.Min + ": " + inputs[2]);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.Max + ": " + inputs[3]);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.StdDev + ": " + inputs[4]);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.IQR + ": " + inputs[5]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        // Set the outputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Outputs);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.TDT + ": " + outputs[0]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        featuresItems.DropDownItems.Add(flowLayoutItems01);
                    }
                }
                else if (i == 7)
                {
                    // This is for delta declaration
                    featuresItems = new ToolStripMenuItem(ARTHTNamings.Step7DeltaExaminationData) { Name = ARTHTNamings.Step7DeltaExaminationData };

                    for (int j = 0; j < _arthtFeatures.StepsDataDic[ARTHTNamings.Step7DeltaExaminationData].Samples.Count; j++)
                    {
                        inputs = _arthtFeatures.StepsDataDic[ARTHTNamings.Step7DeltaExaminationData].Samples[j].getFeatures();
                        outputs = _arthtFeatures.StepsDataDic[ARTHTNamings.Step7DeltaExaminationData].Samples[j].getOutputs();

                        // Set the input and output of detecting short PR of the selected beat
                        flowLayoutItems01 = new ToolStripMenuItem(ARTHTNamings.Beat + j);
                        // Set the inputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Features);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.DeltaAmp + ": " + inputs[0]);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.Mean + ": " + inputs[1]);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.Min + ": " + inputs[2]);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.Max + ": " + inputs[3]);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.StdDev + ": " + inputs[4]);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.IQR + ": " + inputs[5]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        // Set the outputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem(ARTHTNamings.Outputs);
                        flowLayoutItems02.DropDownItems.Add(ARTHTNamings.WPWPattern + ": " + outputs[0]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        featuresItems.DropDownItems.Add(flowLayoutItems01);
                    }
                }

                // Show selected features in featuresFlowLayoutPanel
                MenuStrip selectedFeature = new MenuStrip();
                selectedFeature.Name = featuresItems.Name;
                selectedFeature.Items.Add(featuresItems);
                featuresTableLayoutPanel.Controls.Add(selectedFeature);
            }
        }

        public void initializeAIFilters()
        {
            // Disable unnecessary controls
            signalFusionButton.Enabled = false;
            signalsPickerComboBox.Enabled = false;
            filtersComboBox.Enabled = false;

            // Enable discard and next button, and disable set features button
            discardButton.Enabled = true;
            previousButton.Enabled = true;
            nextButton.Enabled = true;
            setFeaturesLabelsButton.Enabled = false;

            if (_arthtFeatures._processedStep == 8)
                // Set next button to finish
                nextButton.Text = "Finish";
        }
    }
}

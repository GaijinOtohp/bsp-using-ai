using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.AITools.Details.ValidationDataSelection;
using Biological_Signal_Processing_Using_AI.AITools.Keras_NET_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.AITools.DatasetExplorer;
using BSP_Using_AI.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.AITools.Details.ValidationDataSelection.ValDataSelectionForm;

namespace BSP_Using_AI.AITools.Details
{
    public partial class DetailsForm : Form, DbStimulatorReportHolder
    {
        public Dictionary<string, ObjectiveBaseModel> _objectivesModelsDic = null;

        public long _modelId;
        public ObjectiveBaseModel _ObjectiveModel;
        public Dictionary<string, CustomArchiBaseModel> _InnerObjectiveModels;

        public DetailsForm(Dictionary<string, ObjectiveBaseModel> objectivesModelsDic, long modelId, ObjectiveBaseModel objectiveModel, ARTHT_Keras_NET_NN tFBackThread)
        {
            InitializeComponent();

            _objectivesModelsDic = objectivesModelsDic;

            _modelId = modelId;
            _ObjectiveModel = objectiveModel;
            _tFBackThread = tFBackThread;

            this.Text = _ObjectiveModel.ModelName + _ObjectiveModel.ObjectiveName + " Details";

            // Get the models
            _InnerObjectiveModels = new Dictionary<string, CustomArchiBaseModel>();
            if (_ObjectiveModel is ARTHTModels arthtModels)
                foreach (CustomArchiBaseModel baseModel in arthtModels.ARTHTModelsDic.Values)
                    _InnerObjectiveModels.Add(baseModel.Name, baseModel);
            else if (_ObjectiveModel is CWDReinforcementL cwdReinforcementL)
                _InnerObjectiveModels.Add(cwdReinforcementL.CWDReinforcementLModel.Name, cwdReinforcementL.CWDReinforcementLModel);
            else if (_ObjectiveModel is CWDLSTM cwdLSTM)
            {
                _InnerObjectiveModels.Add(cwdLSTM.CWDReinforcementLModel.Name, cwdLSTM.CWDReinforcementLModel);
                _InnerObjectiveModels.Add(cwdLSTM.CWDLSTMModel.Name, cwdLSTM.CWDLSTMModel);
            }
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public void queryForSignals_ARTHT()
        {
            // Qurey for the signals in all selected intervals from dataset
            List<IdInterval> allDataIdsIntervalsList = _ObjectiveModel.DataIdsIntervalsList.SelectMany(IDIntervalsList => IDIntervalsList).ToList();
            (string selection, object[] selectionArgs) = DatasetExplorerForm.SelectDataFromIntervals(allDataIdsIntervalsList, null, null);

            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("dataset",
                                        new string[] { "_id", "sginal_name", "starting_index", "sampling_rate", "quantisation_step" },
                                        selection,
                                        selectionArgs,
                                        "", "DetailsFormForDataset"));
            dbStimulatorThread.Start();
        }

        public void queryForSignals_Anno(string annoObjective)
        {
            // Qurey for the signals in all selected intervals from dataset
            List<IdInterval> allDataIdsIntervalsList = _ObjectiveModel.DataIdsIntervalsList.SelectMany(IDIntervalsList => IDIntervalsList).ToList();
            (string selection, object[] selectionArgs) = DatasetExplorerForm.SelectDataFromIntervals(allDataIdsIntervalsList, "and anno_objective=?", new object[] { annoObjective });

            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("anno_ds",
                                        new string[] { "_id", "sginal_name", "starting_index", "sampling_rate", "quantisation_step" },
                                        selection,
                                        selectionArgs,
                                        "", "DetailsFormForDataset"));
            dbStimulatorThread.Start();
        }

        public void initializeForm()
        {
            // Set previous validation data
            refreshValidationData();

            timeToFinishLabel.Text = "Processed in: " + GeneralTools.PeriodInSecToString(_ObjectiveModel._validationTimeCompelxity) + ", " + _ObjectiveModel._ValidationInfo;

            // Query for all signals in dataset table
            if (_ObjectiveModel is ARTHTModels)
                queryForSignals_ARTHT();
            else if (_ObjectiveModel is CWDReinforcementL || _ObjectiveModel is CWDLSTM)
                queryForSignals_Anno(CharacteristicWavesDelineation.ObjectiveName);
        }

        private void refreshValidationData()
        {
            // Insert new vallidation data in validationFlowLayoutPanel
            validationFlowLayoutPanel.Controls.Clear();
            double classifModelsCount = _InnerObjectiveModels.Values.Where(baseModel => baseModel.Type == ObjectiveType.Classification).Count();
            double regrfModelsCount = _InnerObjectiveModels.Values.Where(baseModel => baseModel.Type == ObjectiveType.Regression).Count();
            double overallTP = 0, overAllTN = 0, overallFP = 0, overallFN = 0, overallMASE = 0;
            foreach (CustomArchiBaseModel innerObjectiveModel in _InnerObjectiveModels.Values)
            {
                ValidationFlowLayoutPanelUserControl validationFlowLayoutPanelUserControl = new ValidationFlowLayoutPanelUserControl(_objectivesModelsDic, _ObjectiveModel, innerObjectiveModel);
                ValidationData validationData = innerObjectiveModel.ValidationData;
                string TP = "/", TN = "/", FP = "/", FN = "/", MASE = "/";

                if (innerObjectiveModel.Type == ObjectiveType.Regression)
                {
                    double maseAverage = 0;
                    foreach (OutputMetrics outputMet in innerObjectiveModel.ValidationData._ModelOutputsValidMetrics)
                        maseAverage += outputMet._mase / innerObjectiveModel.ValidationData._ModelOutputsValidMetrics.Length;
                    MASE = Math.Round(maseAverage, 4).ToString();

                    overallMASE += maseAverage / regrfModelsCount;
                }
                else
                {
                    int accumTP = 0, accumTN = 0, accumFP = 0, accumFN = 0;
                    foreach (OutputMetrics outputMet in innerObjectiveModel.ValidationData._ModelOutputsValidMetrics)
                    {
                        accumTP += outputMet._truePositive;
                        accumTN += outputMet._trueNegative;
                        accumFP += outputMet._falsePositive;
                        accumFN += outputMet._falseNegative;
                    }
                    TP = accumTP.ToString();
                    TN = accumTN.ToString();
                    FP = accumFP.ToString();
                    FN = accumFN.ToString();

                    overallTP += accumTP;
                    overAllTN += accumTN;
                    overallFP += accumFP;
                    overallFN += accumFN;
                }

                validationFlowLayoutPanelUserControl.Name = innerObjectiveModel.Name;
                validationFlowLayoutPanelUserControl.modelTargetLabel.Text = innerObjectiveModel.Name;
                validationFlowLayoutPanelUserControl.algorithmTypeLabel.Text = innerObjectiveModel.Type.ToString();
                validationFlowLayoutPanelUserControl.datasetSizeLabel.Text = validationData._datasetSize.ToString();
                validationFlowLayoutPanelUserControl.trainingDatasetLabel.Text = Math.Round(validationData._trainingDatasetSize, 2).ToString();
                validationFlowLayoutPanelUserControl.validationDatasetLabel.Text = Math.Round(validationData._validationDatasetSize, 2).ToString();

                validationFlowLayoutPanelUserControl.truePositiveLabel.Text = TP;
                validationFlowLayoutPanelUserControl.trueNegativeLabel.Text = TN;
                validationFlowLayoutPanelUserControl.falsePositiveLabel.Text = FP;
                validationFlowLayoutPanelUserControl.falseNegativeLabel.Text = FN;
                validationFlowLayoutPanelUserControl.maseLabel.Text = MASE;

                this.Invoke(new MethodInvoker(delegate () { validationFlowLayoutPanel.Controls.Add(validationFlowLayoutPanelUserControl); }));
            }

            // Insert overall accuracy, sensitivity, specificity, and mase in their controls
            this.Invoke(new MethodInvoker(delegate ()
            {
                overallTPLabel.Text = "Overall true positives: " + overallTP.ToString();
                overallTNLabel.Text = "Overall true negatives: " + overAllTN.ToString();
                overallFPLabel.Text = "Overall false positives: " + overallFP.ToString();
                overallFPLabel.Text = "Overall false negatives: " + overallFN.ToString();
                overallFNLabel.Text = "Overall MASE: " + Math.Round(overallMASE, 2).ToString();
            }));
        }

        private int _totalSamples;
        private int _remainingSamples;
        private long _startingTime;
        private long _remainingTime;
        private System.Threading.Timer _timer;
        private int TIME_INTERVAL_IN_MILLISECONDS = 1000 * 1;
        private void calculateTimeToFinish()
        {
            // Get current time
            _startingTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            _timer = new System.Threading.Timer(timerCallback, null, TIME_INTERVAL_IN_MILLISECONDS, Timeout.Infinite);
        }

        private void timerCallback(Object state)
        {
            // Check if data has been processed
            if (_remainingSamples < _totalSamples)
            {
                // If yes then recalculated remaining time to finish
                long processTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - _startingTime;
                long remainingTime = processTime * _remainingSamples / (_totalSamples - _remainingSamples);

                // Update timer waiting interval
                TIME_INTERVAL_IN_MILLISECONDS += (int)(remainingTime - _remainingTime) * 10;

                _remainingTime = remainingTime;


                this.Invoke(new MethodInvoker(delegate () { timeToFinishLabel.Text = "Time to finish: " + GeneralTools.PeriodInMillSecToString(_remainingTime); }));
            }

            // Run again
            if (_remainingSamples == 0L)
            {
                // Set final processing time
                long processingTime = (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _startingTime);
                this.Invoke(new MethodInvoker(delegate () { timeToFinishLabel.Text = "Processed in: " + GeneralTools.PeriodInMillSecToString(processingTime) + ", " + _ObjectiveModel._ValidationInfo; }));
                // Insert it in _validationData
                _ObjectiveModel._validationTimeCompelxity = processingTime;

                // Update validation data in models table
                DbStimulator dbStimulator = new DbStimulator();
                dbStimulator.Update("models", new string[] { "the_model" },
                    new Object[] { GeneralTools.ObjectToByteArray(_ObjectiveModel.Clone()) }, _modelId, "DetailsForm");
            }
            else
                _timer.Change(TIME_INTERVAL_IN_MILLISECONDS, Timeout.Infinite);
        }

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void validateButton_Click(object sender, EventArgs e)
        {
            // Open ValidationDataSelectionForm
            ValDataSelectionForm validationDataSelectionForm = new ValDataSelectionForm(_ObjectiveModel, CallModelValidation);
            validationDataSelectionForm.Show();
            validationDataSelectionForm.initializeForm();
        }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //:::::::::::::::::::::::::::CROSS PROCESS FORM FUNCTIONS (INTERFACES)::::::::::::::::::::::://
        public void holdRecordReport(DataTable dataTable, string callingClassName)
        {
            if (!callingClassName.Contains("DetailsForm"))
                return;

            if (dataTable.Rows.Count > 0)
            {
                if (callingClassName.Equals("DetailsFormForDataset"))
                {
                    // Show dataset signals and updates values
                    // Set updates iterations
                    // Iterate through each training update
                    long[] updatesDatasetSize = new long[_ObjectiveModel.DataIdsIntervalsList.Count];
                    int updateListIndex;
                    for (int i = 0; i < _ObjectiveModel.DataIdsIntervalsList.Count; i++)
                    {
                        DatasetFlowLayoutPanelItem2UserControl datasetFlowLayoutPanelItem2UserControl = new DatasetFlowLayoutPanelItem2UserControl();
                        datasetFlowLayoutPanelItem2UserControl.Name = "trainingUpdate" + i;
                        datasetFlowLayoutPanelItem2UserControl.trainingUpdateLabel.Text += " " + (i + 1);
                        this.Invoke(new MethodInvoker(delegate () { signalsFlowLayoutPanel.Controls.Add(datasetFlowLayoutPanelItem2UserControl); }));
                    }

                    // Order datatable by name
                    List<DataRow> rowsList = new List<DataRow>(dataTable.AsEnumerable());
                    List<string> namesList = new List<string>();
                    foreach (DataRow row in rowsList)
                        namesList.Add(row.Field<string>("sginal_name"));
                    rowsList = GeneralTools.OrderByTextWithNumbers(rowsList, namesList);

                    // Insert new items from records
                    foreach (DataRow row in rowsList)
                    {
                        // Check which update does this signal belong to
                        for (int i = 0; i < _ObjectiveModel.DataIdsIntervalsList.Count; i++)
                        {
                            List<IdInterval> training = _ObjectiveModel.DataIdsIntervalsList[i];
                            foreach (IdInterval datasetInterval in training)
                            {
                                if (row.Field<long>("_id") >= datasetInterval.starting && row.Field<long>("_id") <= datasetInterval.ending)
                                {
                                    // If yes then create an item of the model
                                    DatasetFlowLayoutPanelItemUserControl datasetFlowLayoutPanelItemUserControl = new DatasetFlowLayoutPanelItemUserControl();
                                    datasetFlowLayoutPanelItemUserControl.signalNameLabel.Text = row.Field<string>("sginal_name");
                                    datasetFlowLayoutPanelItemUserControl.startingIndexLabel.Text = row.Field<long>("starting_index").ToString();
                                    datasetFlowLayoutPanelItemUserControl.samplingRateLabel.Text = row.Field<long>("sampling_rate").ToString();
                                    datasetFlowLayoutPanelItemUserControl.quantizationStepLabel.Text = row.Field<long>("quantisation_step").ToString();

                                    datasetFlowLayoutPanelItemUserControl.selectionCheckBox.Visible = false;
                                    datasetFlowLayoutPanelItemUserControl.featuresDetailsButton.Visible = false;

                                    this.Invoke(new MethodInvoker(delegate () { signalsFlowLayoutPanel.Controls.Add(datasetFlowLayoutPanelItemUserControl); }));

                                    // Set the signal to the correct update list
                                    if (i < _ObjectiveModel.DataIdsIntervalsList.Count - 1)
                                    {
                                        updateListIndex = signalsFlowLayoutPanel.Controls.GetChildIndex(signalsFlowLayoutPanel.Controls.Find("trainingUpdate" + (i + 1), false)[0]);
                                        this.Invoke(new MethodInvoker(delegate () { signalsFlowLayoutPanel.Controls.SetChildIndex(datasetFlowLayoutPanelItemUserControl, updateListIndex); }));
                                    }

                                    updatesDatasetSize[i]++;
                                }
                            }
                        }
                    }

                    // Insert updates dataset size values
                    for (int i = 0; i < updatesDatasetSize.Length; i++)
                    {
                        this.Invoke(new MethodInvoker(delegate () { ((DatasetFlowLayoutPanelItem2UserControl)signalsFlowLayoutPanel.Controls.Find("trainingUpdate" + i, false)[0]).trainingUpdateLabel.Text += " (" + updatesDatasetSize[i] + ")"; }));
                        this.Invoke(new MethodInvoker(delegate () { trainingsDetailsListBox.Items.Add((i + 1) + ". " + updatesDatasetSize[i] + " signal"); }));
                    }
                }
            }
        }

        //______________________________________________________________________________________________________//
        //:::::::::::::::::::::::::::::::::::::::::Validation process::::::::::::::::::::::::::::::::::::::::::://
        public void CallModelValidation(List<ModelData> valModelsData, string validationInfo)
        {
            // Clear validationFlowLayoutPanel
            this.Invoke(new MethodInvoker(delegate () { EventHandlers.fastClearFlowLayout(ref validationFlowLayoutPanel); }));
            // Set validation info
            _ObjectiveModel._ValidationInfo = validationInfo;

            // Check if data is sufficient
            bool showError = false;
            if (valModelsData.Count == 0) showError = true;
            else if (valModelsData[0].TrainingData.Count == 0 && valModelsData[0].ValidationData.Count == 0) showError = true;
            if (showError)
            {
                // If yes then data is not sufficient for validation
                // Show a message aboutit and return
                MessageBox.Show("Datasize not sufficient", "Warning \"Unexpected data\"", MessageBoxButtons.OK);
                return;
            }

            if (_ObjectiveModel is ARTHTModels arthtModels)
                ARTHRValidateModel(valModelsData, arthtModels);
            else if (_ObjectiveModel is CWDReinforcementL cwdReinforcementL)
                CWDReinforcementL_ValidateModel(valModelsData, cwdReinforcementL);
            else if (_ObjectiveModel is CWDLSTM cwdLSTM)
                CWDLSTM_ValidateModel(valModelsData, cwdLSTM);
        }
    }
}

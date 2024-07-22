using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.AITools.Details.ValidationDataSelection;
using Biological_Signal_Processing_Using_AI.AITools.Keras_NET_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.KNN_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.NaiveBayes_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.AITools.DatasetExplorer;
using BSP_Using_AI.Database;
using System;
using System.Collections.Concurrent;
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
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools.Details
{
    public partial class DetailsForm : Form, DbStimulatorReportHolder
    {
        public ARTHT_Keras_NET_NN _tFBackThread;

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
            // Query for all signals in dataset table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("dataset",
                                new String[] { "_id", "sginal_name", "starting_index", "sampling_rate", "quantisation_step" },
                                null,
                                null,
                                " ORDER BY sginal_name ASC", "DetailsFormForDataset"));
            dbStimulatorThread.Start();
        }

        public void queryForSignals_Anno(string annoObjective)
        {
            // Query for all signals in dataset table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("anno_ds",
                                new String[] { "_id", "sginal_name", "starting_index", "sampling_rate", "quantisation_step" },
                                "anno_objective=?",
                                new object[] { annoObjective },
                                " ORDER BY sginal_name ASC", "DetailsFormForDataset"));
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
            double overallAccuracy = 0, overAllSensitivity = 0, overallSpecificity = 0, overallMASE = 0;
            foreach (CustomArchiBaseModel innerObjectiveModel in _InnerObjectiveModels.Values)
            {
                ValidationFlowLayoutPanelUserControl validationFlowLayoutPanelUserControl = new ValidationFlowLayoutPanelUserControl(_objectivesModelsDic, _ObjectiveModel, innerObjectiveModel);
                ValidationData validationData = innerObjectiveModel.ValidationData;
                string accuracy = "/", sensitivity = "/", specificity = "/";

                if (innerObjectiveModel.Type == ObjectiveType.Regression)
                {
                    accuracy = Math.Round(validationData._accuracy, 4).ToString();

                    overallMASE += validationData._accuracy / regrfModelsCount;
                }
                else
                {
                    accuracy = Math.Round(validationData._accuracy * 100, 2).ToString() + "%";
                    sensitivity = Math.Round(validationData._sensitivity * 100, 2).ToString() + "%";
                    specificity = Math.Round(validationData._specificity * 100, 2).ToString() + "%";

                    overallAccuracy += validationData._accuracy / classifModelsCount;
                    overAllSensitivity += validationData._sensitivity / classifModelsCount;
                    overallSpecificity += validationData._specificity / classifModelsCount;
                }

                validationFlowLayoutPanelUserControl.Name = innerObjectiveModel.Name;
                validationFlowLayoutPanelUserControl.modelTargetLabel.Text = innerObjectiveModel.Name;
                validationFlowLayoutPanelUserControl.algorithmTypeLabel.Text = innerObjectiveModel.Type.ToString();
                validationFlowLayoutPanelUserControl.datasetSizeLabel.Text = validationData._datasetSize.ToString();
                validationFlowLayoutPanelUserControl.trainingDatasetLabel.Text = Math.Round(validationData._trainingDatasetSize, 2).ToString();
                validationFlowLayoutPanelUserControl.validationDatasetLabel.Text = Math.Round(validationData._validationDatasetSize, 2).ToString();

                validationFlowLayoutPanelUserControl.accuracyLabel.Text = accuracy;
                validationFlowLayoutPanelUserControl.sensitivityLabel.Text = sensitivity;
                validationFlowLayoutPanelUserControl.specificityLabel.Text = specificity;

                this.Invoke(new MethodInvoker(delegate () { validationFlowLayoutPanel.Controls.Add(validationFlowLayoutPanelUserControl); }));
            }

            // Insert overall accuracy, sensitivity, specificity, and mase in their controls
            this.Invoke(new MethodInvoker(delegate ()
            {
                overallAccuracyLabel.Text = "Overall accuracy: " + Math.Round(overallAccuracy * 100, 2).ToString() + "%";
                overallSensitivityLabel.Text = "Overall sensitivity: " + Math.Round(overAllSensitivity * 100, 2).ToString() + "%";
                overallSpecificityLabel.Text = "Overall specificity: " + Math.Round(overallSpecificity * 100, 2).ToString() + "%";
                overallMASELabel.Text = "Overall MASE: " + Math.Round(overallMASE, 2).ToString();
            }));
        }

        private void UpdateModelValidatoinData(double trainingSize, double validationSize, double accuracy, double sensitivity, double specificity, string baseModelName)
        {
            ValidationData validationData = _InnerObjectiveModels[baseModelName].ValidationData;

            validationData._datasetSize = (int)(trainingSize + validationSize);
            validationData._trainingDatasetSize = trainingSize;
            validationData._validationDatasetSize = validationSize;
            validationData._accuracy = accuracy;
            validationData._sensitivity = sensitivity;
            validationData._specificity = specificity;

            refreshValidationData();
        }

        private double[] askForPrediction(double[] features, string modelName, CustomArchiBaseModel model, string stepName)
        {
            // Check which model is selected
            if (modelName.Equals(KerasNETNeuralNetworkModel.ModelName))
            {
                // This is for neural network
                AutoResetEvent signal = new AutoResetEvent(false);
                ConcurrentQueue<QueueSignalInfo> queue = new ConcurrentQueue<QueueSignalInfo>();
                _tFBackThread._queue.Enqueue(new QueueSignalInfo()
                {
                    TargetFunc = "predict",
                    CallingClass = "DetailsForm",
                    Features = features,
                    Signal = signal,
                    Queue = queue
                });
                _tFBackThread._signal.Set();

                // Wait for the answer
                signal.WaitOne();

                QueueSignalInfo item = null;
                while (queue.TryDequeue(out item))
                    // Check which function is selected
                    return item.Outputs;
            }
            else if (modelName.Equals(KNNModel.ModelName))
            {
                // This is for knn
                return KNN.predict(features, (KNNModel)model);
            }
            else if (modelName.Equals(NaiveBayesModel.ModelName))
            {
                // This is for naive bayes
                return NaiveBayes.predict(features, (NaiveBayesModel)model);
            }
            else if (modelName.Equals(TFNETNeuralNetworkModel.ModelName))
            {
                // This is for Tensorflow.Net Neural Networks
                return TF_NET_NN.predict(features, model, ((TFNETNeuralNetworkModel)model).BaseModel.Session);
            }
            else if (modelName.Equals(TFKerasNeuralNetworkModel.ModelName))
            {
                // This is for Tensorflow.Keras Neural Networks
                return TF_KERAS_NN.predict(features, (TFKerasNeuralNetworkModel)model);
            }

            return null;
        }

        private int _data;
        private int _remainingData;
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
            if (_remainingData < _data)
            {
                // If yes then recalculated remaining time to finish
                long processTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - _startingTime;
                long remainingTime = processTime * _remainingData / (_data - _remainingData);
                remainingTime /= 1000;

                // Update timer waiting interval
                TIME_INTERVAL_IN_MILLISECONDS += (int)(remainingTime - _remainingTime) * 10;

                _remainingTime = remainingTime;


                this.Invoke(new MethodInvoker(delegate () { timeToFinishLabel.Text = "Time to finish: " + GeneralTools.PeriodInSecToString(_remainingTime); }));
            }

            // Run again
            if (_remainingData == 0L)
            {
                // Set final processing time
                long processingTime = (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _startingTime) / 1000;
                this.Invoke(new MethodInvoker(delegate () { timeToFinishLabel.Text = "Processed in: " + GeneralTools.PeriodInSecToString(processingTime) + ", " + _ObjectiveModel._ValidationInfo; }));
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
            queryFeatures("DetailsFormForFeatures");
        }

        private void queryFeatures(string callingClassName)
        {
            // Qurey for signals features in all selected intervals from dataset
            string selection = "_id>=? and _id<=?";
            int intervalsNum = 1;
            foreach (List<IdInterval> training in _ObjectiveModel.DataIdsIntervalsList)
                intervalsNum += training.Count;
            object[] selectionArgs = new object[intervalsNum * 2];
            intervalsNum = 0;
            selectionArgs[intervalsNum] = 0;
            selectionArgs[intervalsNum + 1] = 0;
            foreach (List<IdInterval> training in _ObjectiveModel.DataIdsIntervalsList)
                foreach (IdInterval datasetInterval in training)
                {
                    intervalsNum += 2;
                    selection += " or _id>=? and _id<=?";
                    selectionArgs[intervalsNum] = datasetInterval.starting;
                    selectionArgs[intervalsNum + 1] = datasetInterval.ending;
                }

            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("dataset",
                                        new String[] { "_id", "sginal_name", "starting_index", "sampling_rate", "quantisation_step", "features" },
                                        selection,
                                        selectionArgs,
                                        "", callingClassName));
            dbStimulatorThread.Start();
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
                else if (callingClassName.Equals("DetailsFormForFeatures"))
                {
                    // Open ValidationDataSelectionForm
                    Invoke(new MethodInvoker(delegate ()
                    {
                        ValDataSelectionForm validationDataSelectionForm = new ValDataSelectionForm(dataTable, ValidateModel);
                        validationDataSelectionForm.Show();
                    }));

                    //ValidateModel(dataTable);
                }
            }
        }

        //______________________________________________________________________________________________________//
        //:::::::::::::::::::::::::::::::::::::::::Validation process::::::::::::::::::::::::::::::::::::::::::://
        public void ValidateModel(List<ModelData> valModelsData, string validationInfo)
        {
            // Clear validationFlowLayoutPanel
            this.Invoke(new MethodInvoker(delegate () { EventHandlers.fastClearFlowLayout(ref validationFlowLayoutPanel); }));
            // Set validation info
            _ObjectiveModel._ValidationInfo = validationInfo;

            // Check if data is sufficient
            bool showError = false;
            if (valModelsData.Count == 0) showError = true;
            else if (valModelsData[0].TrainingData.Count == 0 || valModelsData[0].ValidationData.Count == 0) showError = true;
            if (showError)
            {
                // If yes then data is not sufficient for validation
                // Show a message aboutit and return
                MessageBox.Show("Datasize not sufficient", "Warning \"Unexpected data\"", MessageBoxButtons.OK);
                return;
            }

            if (_ObjectiveModel is ARTHTModels arthtModels)
                ARTHRValidateModel(valModelsData, arthtModels);
        }
    }
}

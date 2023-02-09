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
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools.Details
{
    public partial class DetailsForm : Form, DbStimulatorReportHolder
    {
        public TFBackThread _tFBackThread;

        public long _modelId;
        public ARTHTModels _aRTHTModels;

        Dictionary<string, float[]> OutputsThresholdsDic = new Dictionary<string, float[]>(7);

        public DetailsForm(long modelId, ARTHTModels aRTHTModels, TFBackThread tFBackThread)
        {
            InitializeComponent();

            _modelId = modelId;
            _aRTHTModels = aRTHTModels;
            _tFBackThread = tFBackThread;

            this.Text = _aRTHTModels.Name + " Details";
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public void initializeForm()
        {
            // Set previous validation data and copy thresholds
            foreach (string stepName in _aRTHTModels.ARTHTModelsDic.Keys)
            {
                OutputsThresholdsDic.Add(stepName, (float[])_aRTHTModels.ARTHTModelsDic[stepName].OutputsThresholds.Clone());
                refreshValidationData(stepName, false);
            }

            timeToFinishLabel.Text = "Processed in: " + Garage.PeriodInSecToString(_aRTHTModels._validationTimeCompelxity);

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

        private void refreshValidationData(string stepName, bool replace)
        {
            // Insert new vallidation data in validationFlowLayoutPanel
            ValidationData validationData = _aRTHTModels.ARTHTModelsDic[stepName].ValidationData;
            ValidationFlowLayoutPanelUserControl validationFlowLayoutPanelUserControl = new ValidationFlowLayoutPanelUserControl(OutputsThresholdsDic[stepName]);
            validationFlowLayoutPanelUserControl.Name = stepName;
            validationFlowLayoutPanelUserControl.modelTargetLabel.Text = stepName;
            validationFlowLayoutPanelUserControl.algorithmTypeLabel.Text = validationData.AlgorithmType;
            validationFlowLayoutPanelUserControl.datasetSizeLabel.Text = validationData._datasetSize.ToString();
            validationFlowLayoutPanelUserControl.trainingDatasetLabel.Text = validationData._trainingDatasetSize.ToString();
            validationFlowLayoutPanelUserControl.validationDatasetLabel.Text = validationData._validationDatasetSize.ToString();
            validationFlowLayoutPanelUserControl.accuracyLabel.Text = Math.Round(validationData._accuracy, 4).ToString();
            validationFlowLayoutPanelUserControl.sensitivityLabel.Text = Math.Round(validationData._sensitivity, 4).ToString();
            validationFlowLayoutPanelUserControl.specificityLabel.Text = Math.Round(validationData._specificity, 4).ToString();

            if (stepName.Equals(ARTHTNamings.Step1RPeaksScanData) || stepName.Equals(ARTHTNamings.Step3BeatPeaksScanData) || stepName.Equals(ARTHTNamings.Step6UpstrokesScanData))
            {
                validationFlowLayoutPanelUserControl.thresholdTextBox.Enabled = false;
                validationFlowLayoutPanelUserControl.sensitivityLabel.Text = "/";
                validationFlowLayoutPanelUserControl.specificityLabel.Text = "/";
            }
            else
                validationFlowLayoutPanelUserControl.thresholdTextBox.Text = _aRTHTModels.ARTHTModelsDic[stepName].OutputsThresholds[0].ToString();

            if (replace)
                this.Invoke(new MethodInvoker(delegate ()
                {
                    int index = validationFlowLayoutPanel.Controls.IndexOfKey(stepName);
                    validationFlowLayoutPanel.Controls.RemoveByKey(stepName);
                    validationFlowLayoutPanel.Controls.Add(validationFlowLayoutPanelUserControl);
                    validationFlowLayoutPanel.Controls.SetChildIndex(validationFlowLayoutPanelUserControl, index);
                }));
            else
                this.Invoke(new MethodInvoker(delegate () { validationFlowLayoutPanel.Controls.Add(validationFlowLayoutPanelUserControl); }));
        }

        private void insertValidatoinData(int trainingSize, int validationSize, double accuracy, double sensitivity, double specificity, string stepName)
        {
            ValidationData validationData = _aRTHTModels.ARTHTModelsDic[stepName].ValidationData;

            validationData._datasetSize = trainingSize + validationSize;
            validationData._trainingDatasetSize = trainingSize;
            validationData._validationDatasetSize = validationSize;
            validationData._accuracy = accuracy;
            validationData._sensitivity = sensitivity;
            validationData._specificity = specificity;

            if (stepName.Equals(ARTHTNamings.Step1RPeaksScanData) || stepName.Equals(ARTHTNamings.Step3BeatPeaksScanData) || stepName.Equals(ARTHTNamings.Step6UpstrokesScanData))
            {
                validationData.AlgorithmType = "Regression";
            }
            else
                validationData.AlgorithmType = "Classification";
        }

        private double[] askForPrediction(double[] features, string modelType, KNNModel knnModel, NaiveBayesModel nbModel, string stepName)
        {
            // Check which model is selected
            if (modelType.Contains("Neural network"))
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
            else if (modelType.Contains("K-Nearest neighbor"))
            {
                // This is for knn
                return KNNBackThread.predict(features, knnModel);
            }
            else if (modelType.Contains("Naive bayes"))
            {
                // This is for naive bayes
                return NaiveBayesBackThread.predict(features, nbModel);
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


                this.Invoke(new MethodInvoker(delegate () { timeToFinishLabel.Text = "Time to finish: " + Garage.PeriodInSecToString(_remainingTime); }));
            }

            // Run again
            if (_remainingData == 0L)
            {
                // Set final processing time
                long processingTime = (DateTimeOffset.Now.ToUnixTimeMilliseconds() - _startingTime) / 1000;
                this.Invoke(new MethodInvoker(delegate () { timeToFinishLabel.Text = "Processed in: " + Garage.PeriodInSecToString(processingTime); }));
                // Insert it in _validationData
                _aRTHTModels._validationTimeCompelxity = processingTime;

                // Update validation data in models table
                DbStimulator dbStimulator = new DbStimulator();
                dbStimulator.Update("models", new string[] { "the_model" },
                    new Object[] { Garage.ObjectToByteArray(_aRTHTModels.Clone()) }, _modelId, "DetailsForm");
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

        private void optimizeThresholdsButton_Click(object sender, EventArgs e)
        {
            queryFeatures("DetailsFormForThresholds");
        }

        private void queryFeatures(string callingClassName)
        {
            // Qurey for signals features in all selected intervals from dataset
            string selection = "_id>=? and _id<=?";
            int intervalsNum = 1;
            foreach (List<long[]> training in _aRTHTModels.DataIdsIntervalsList)
                intervalsNum += training.Count;
            object[] selectionArgs = new object[intervalsNum * 2];
            intervalsNum = 0;
            selectionArgs[intervalsNum] = 0;
            selectionArgs[intervalsNum + 1] = 0;
            foreach (List<long[]> training in _aRTHTModels.DataIdsIntervalsList)
                foreach (long[] datasetInterval in training)
                {
                    intervalsNum += 2;
                    selection += " or _id>=? and _id<=?";
                    selectionArgs[intervalsNum] = datasetInterval[0];
                    selectionArgs[intervalsNum + 1] = datasetInterval[1];
                }

            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("dataset",
                                        new String[] { "features" },
                                        selection,
                                        selectionArgs,
                                        "", callingClassName));
            dbStimulatorThread.Start();
        }

        private void saveChangesButton_Click(object sender, EventArgs e)
        {
            // Get new thresholds from OutputsThresholdsDic
            foreach (string stepName in OutputsThresholdsDic.Keys)
                _aRTHTModels.ARTHTModelsDic[stepName].OutputsThresholds = (float[])OutputsThresholdsDic[stepName].Clone();

            // Now save _outputThresholds in database
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Update("models",
                                        new String[] { "the_model" },
                                        new object[] { Garage.ObjectToByteArray(_aRTHTModels.Clone()) },
                                        _modelId,
                                        "DetailsForm"));
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
                    long[] updatesDatasetSize = new long[_aRTHTModels.DataIdsIntervalsList.Count];
                    int updateListIndex;
                    for (int i = 0; i < _aRTHTModels.DataIdsIntervalsList.Count; i++)
                    {
                        DatasetFlowLayoutPanelItem2UserControl datasetFlowLayoutPanelItem2UserControl = new DatasetFlowLayoutPanelItem2UserControl();
                        datasetFlowLayoutPanelItem2UserControl.Name = "trainingUpdate" + i;
                        datasetFlowLayoutPanelItem2UserControl.trainingUpdateLabel.Text += " " + (i + 1);
                        this.Invoke(new MethodInvoker(delegate () { signalsFlowLayoutPanel.Controls.Add(datasetFlowLayoutPanelItem2UserControl); }));
                    }

                    // Insert new items from records
                    foreach (DataRow row in dataTable.AsEnumerable())
                    {
                        // Check which update does this signal belong to
                        for (int i = 0; i < _aRTHTModels.DataIdsIntervalsList.Count; i++)
                        {
                            List<long[]> training = _aRTHTModels.DataIdsIntervalsList[i];
                            foreach (long[] datasetInterval in training)
                            {
                                if (row.Field<long>("_id") >= datasetInterval[0] && row.Field<long>("_id") <= datasetInterval[1])
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
                                    if (i < _aRTHTModels.DataIdsIntervalsList.Count - 1)
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
                else if (callingClassName.Equals("DetailsFormForThresholds"))
                {
                    // Count the ones of the outputs for each classification step
                    foreach (string stepName in OutputsThresholdsDic.Keys)
                        if (stepName.Equals(ARTHTNamings.Step2RPeaksSelectionData) || stepName.Equals(ARTHTNamings.Step4PTSelectionData) ||
                            stepName.Equals(ARTHTNamings.Step5ShortPRScanData) || stepName.Equals(ARTHTNamings.Step7DeltaExaminationData))
                        {
                            float[] ones = new float[OutputsThresholdsDic[stepName].Length];
                            float allPoss = 0f;

                            foreach (DataRow row in dataTable.AsEnumerable())
                            {
                                ARTHTFeatures aRTHTFeatures = (ARTHTFeatures)Garage.ByteArrayToObject(row.Field<byte[]>("features"));

                                foreach (Sample sample in aRTHTFeatures.StepsDataDic[stepName].Samples)
                                {
                                    double[] outputs = sample.getOutputs();
                                    for (int i = 0; i < outputs.Length; i++)
                                        ones[i] += (float)outputs[i];
                                    allPoss++;
                                }
                            }

                            // Calculate new threshold
                            for (int i = 0; i < ones.Length; i++)
                                ones[i] = ones[i] / allPoss > 0 ? (ones[i] / allPoss < 1 ? ones[i] / allPoss : 0.5f) : 0.5f;
                            // Update _outputThresholds with the new thresholds
                            OutputsThresholdsDic[stepName] = ones;
                            refreshValidationData(stepName, true);
                        }
                }
                else if (callingClassName.Equals("DetailsFormForFeatures"))
                {
                    // Shuffle all records first
                    Garage.Shuffle(dataTable);

                    // Initialize lists of features for each step
                    List<Sample> trainingSamples;
                    List<Sample> validationSamples;

                    // Separate features to 4 quarters
                    // and take 3 quarters as training data
                    // and the fourth quarter as validation data
                    int parts = 1;
                    int partSamplesNmbr = 1;
                    for (int i = 4; i > 0; i--)
                        if (dataTable.Rows.Count / i > 0)
                        {
                            parts = i;
                            break;
                        }
                    // Check if parts is equal to 1
                    if (parts == 1)
                    {
                        // If yes then data is not sufficient for validation
                        // Show a message aboutit and return
                        MessageBox.Show("Datasize not sufficient", "Warning \"Unexpected data\"", MessageBoxButtons.OK);
                        return;
                    }

                    partSamplesNmbr = dataTable.Rows.Count / parts;

                    // Check if cross validation is activated
                    int validationPart = parts - 1;
                    int crossValidationNumbr = 1;
                    if (enableCrossValidationCheckBox.Checked)
                    {
                        // If yes then
                        validationPart = 0;
                        crossValidationNumbr = parts;
                    }

                    ////////////////////////// Start validation for each step model (7 steps)
                    int fitProgress = 0;
                    // Initialize _validationData
                    foreach (CustomBaseModel model in _aRTHTModels.ARTHTModelsDic.Values)
                        model.ValidationData = new ValidationData();

                    this.Invoke(new MethodInvoker(delegate () { validationFlowLayoutPanel.Controls.Clear(); }));
                    // Set maximum of progress bar
                    this.Invoke(new MethodInvoker(delegate () { validationProgressBar.Maximum = _aRTHTModels.ARTHTModelsDic.Count; }));
                    double accuracy, sensitivity, specificity;
                    int trainingSize, validationSize, possibilities, targetPositives, targetNegatives;
                    // Calculate number of data to be processed
                    _data = 0;
                    for (int x = validationPart; x < parts; x++)
                        for (int j = 0; j < dataTable.Rows.Count; j++)
                            if ((x * partSamplesNmbr <= j && j < (x + 1) * partSamplesNmbr) || (x == parts - 1 && x * partSamplesNmbr <= j))
                            {
                                DataRow row = dataTable.Rows[j];
                                ARTHTFeatures aRTHTFeatures = (ARTHTFeatures)Garage.ByteArrayToObject(row.Field<byte[]>("features"));
                                foreach (Data data in aRTHTFeatures.StepsDataDic.Values)
                                    //_data += data.Samples.Count * data.OutputsLabelsIndx.Count;
                                    _data += data.Samples.Count;
                            }

                    _remainingData = _data;

                    // Start validation process for each step model
                    calculateTimeToFinish();
                    foreach (string stepName in _aRTHTModels.ARTHTModelsDic.Keys)
                    {
                        trainingSize = 0;
                        validationSize = 0;
                        accuracy = 0;
                        sensitivity = 0;
                        specificity = 0;
                        possibilities = 0;
                        targetPositives = 0;
                        targetNegatives = 0;
                        for (int x = validationPart; x < parts; x++)
                        {
                            // Iterate through each signal features
                            trainingSamples = new List<Sample>();
                            validationSamples = new List<Sample>();
                            List<int> selectedBeatLastState = new List<int>();
                            // and split them into 75% for training and 25% for validation in trainingFeatures and validationFeatures respectively
                            for (int j = 0; j < dataTable.Rows.Count; j++)
                            {
                                DataRow row = dataTable.Rows[j];
                                ARTHTFeatures aRTHTFeatures = (ARTHTFeatures)Garage.ByteArrayToObject(row.Field<byte[]>("features"));

                                if ((x * partSamplesNmbr <= j && j < (x + 1) * partSamplesNmbr) || (x == parts - 1 && x * partSamplesNmbr <= j))
                                    foreach (Sample sample in aRTHTFeatures.StepsDataDic[stepName].Samples)
                                        validationSamples.Add(sample);
                                else
                                    foreach (Sample sample in aRTHTFeatures.StepsDataDic[stepName].Samples)
                                        trainingSamples.Add(sample);
                            }

                            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            // Create The model from trainingFeatures
                            // Send features for fitting
                            // Check which model is selected
                            //trainingSize += trainingFeatures.Count;
                            trainingSize = trainingSamples.Count;
                            //validationSize += validationFeatures.Count;
                            validationSize = validationSamples.Count;
                            KNNModel knnModel = new KNNModel { };
                            NaiveBayesModel nbModel = new NaiveBayesModel { };
                            if (_aRTHTModels.Name.Contains("Neural network"))
                            {
                                // This is for neural network
                                _tFBackThread._queue.Enqueue(new QueueSignalInfo()
                                {
                                    TargetFunc = "fit",
                                    CallingClass = this.Name,
                                    DataList = trainingSamples,
                                    ModelsName = _aRTHTModels.Name,
                                    StepName = stepName
                                });
                                _tFBackThread._signal.Set();
                            }
                            else if (_aRTHTModels.Name.Contains("K-Nearest neighbor"))
                            {
                                // This is for knn
                                // Create a KNN model structure with the initial optimum K, which is "3"
                                knnModel = KNNBackThread.createKNNModel(stepName, trainingSamples, _aRTHTModels.ARTHTModelsDic[stepName]._pcaActive);

                                // Fit features
                                knnModel = KNNBackThread.fit(knnModel, trainingSamples);
                            }
                            else if (_aRTHTModels.Name.Contains("Naive bayes"))
                            {
                                // This is for naive bayes
                                nbModel = NaiveBayesBackThread.createNBModel(stepName, trainingSamples, _aRTHTModels.ARTHTModelsDic[stepName]._pcaActive);

                                // Fit features
                                nbModel = NaiveBayesBackThread.fit(nbModel, trainingSamples);
                            }

                            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            // Calculate validation metrics
                            double[] predictedOutput;
                            double[] actualOutput;

                            string currentBeatName = "";
                            string prevBeatName = "";
                            int numberOfStates = 0;
                            (double predicted, double actual) pPrediction = (-1, 0);
                            (double predicted, double actual) tPrediction = (-1, 0);
                            for (int j = 0; j < validationSize; j++)
                            {
                                Sample sample = validationSamples[j];
                                actualOutput = sample.getOutputs();
                                // Get prediction output for current feature
                                predictedOutput = askForPrediction(sample.getFeatures(), _aRTHTModels.Name, knnModel, nbModel, stepName);
                                _remainingData--;

                                // Check which type of validation metrics is this
                                if (stepName.Equals(ARTHTNamings.Step1RPeaksScanData) || stepName.Equals(ARTHTNamings.Step3BeatPeaksScanData) || stepName.Equals(ARTHTNamings.Step6UpstrokesScanData))
                                {
                                    // If yes then this is for regression metrics
                                    for (int i = 0; i < predictedOutput.Length; i++)
                                    {
                                        // Calculate accuracy using Mean Absolute Percentage Error formula
                                        // where accuracy should be (1 - mape)
                                        double mape = Math.Abs((actualOutput[i] - predictedOutput[i]) / actualOutput[i]);
                                        accuracy = 1d - (((1d - accuracy) * possibilities + mape) / (possibilities + 1));
                                        possibilities++;
                                    }
                                }
                                else
                                {
                                    // This is for classification metrics
                                    // Check if this is for P and T selection
                                    if (stepName.Equals(ARTHTNamings.Step4PTSelectionData))
                                    {
                                        // If yes then prediction should be rearanged for each beat states
                                        // Check if beat name is changed
                                        // or if this is the last state in validationSamples
                                        currentBeatName = sample.Name.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0];
                                        if (!currentBeatName.Equals(prevBeatName) || j == validationSize - 1)
                                        {
                                            // Check if this is the last state
                                            if (j == validationSize - 1)
                                            {
                                                // Compute last state's values
                                                numberOfStates++;

                                                // Check if p probability is greater than previous predicted sample
                                                if (predictedOutput[0] > pPrediction.predicted)
                                                    pPrediction = (predictedOutput[0], actualOutput[0]);

                                                // Check if t probability is greater than previous predicted sample
                                                if (predictedOutput[1] > tPrediction.predicted)
                                                    tPrediction = (predictedOutput[1], actualOutput[1]);
                                            }

                                            // Check if there exist predicted states
                                            if (numberOfStates > 0)
                                            {
                                                // Cumpute the accuracy, sensitivity, and specificity of previous beat

                                                // Compute accuracy
                                                double trueVals = numberOfStates * 2;
                                                trueVals -= pPrediction.actual == 1 ? 0 : 2;
                                                trueVals -= tPrediction.actual == 1 ? 0 : 2;
                                                accuracy = (accuracy * possibilities + trueVals) / (possibilities + numberOfStates * 2);
                                                // possibilities of all of the otputs (2) --> predictedOutput.Length
                                                possibilities += numberOfStates * 2;

                                                // Compute sensitivity
                                                double truePositives = pPrediction.actual + tPrediction.actual;
                                                sensitivity = (sensitivity * targetPositives + truePositives) / (targetPositives + 2);
                                                // only 1 p wave and 1 t wave (2 target positive) for each beat
                                                targetPositives += 2;

                                                // Compute specificity
                                                double trueNegatives = trueVals - truePositives;
                                                specificity = (specificity * targetNegatives + trueNegatives) / (targetNegatives + numberOfStates * 2 - 2);
                                                // all states are negative except 1 for p wave and 1 for t wave
                                                targetNegatives += numberOfStates * 2 - 2;
                                            }

                                            // Initialize values for next beat
                                            prevBeatName = currentBeatName;
                                            numberOfStates = 0;
                                            pPrediction = (-1, 0);
                                            tPrediction = (-1, 0);

                                            // Break the for loop here if this is the last state
                                            if (j == validationSize - 1)
                                                break;
                                        }
                                        numberOfStates++;

                                        // Check if p probability is greater than previous predicted sample
                                        if (predictedOutput[0] > pPrediction.predicted)
                                            pPrediction = (predictedOutput[0], actualOutput[0]);

                                        // Check if t probability is greater than previous predicted sample
                                        if (predictedOutput[1] > tPrediction.predicted)
                                            tPrediction = (predictedOutput[1], actualOutput[1]);
                                    }
                                    else
                                    {
                                        // Set validation data for other steps
                                        for (int i = 0; i < predictedOutput.Length; i++)
                                        {
                                            // Get curretn step threshold
                                            float threshold = _aRTHTModels.ARTHTModelsDic[stepName].OutputsThresholds[i];
                                            // Regulate output value
                                            predictedOutput[i] = predictedOutput[i] >= threshold ? 1 : 0;

                                            // Calculate accuracy
                                            if (predictedOutput[i] == actualOutput[i])
                                                accuracy = (accuracy * possibilities + 1) / (possibilities + 1);
                                            else
                                                accuracy = (accuracy * possibilities + 0) / (possibilities + 1);
                                            possibilities++;

                                            // Calculate sensitivity and specificity
                                            if (actualOutput[i] == 1)
                                            {
                                                // This is for sensitivity
                                                if (predictedOutput[i] == 1)
                                                    sensitivity = (sensitivity * targetPositives + 1) / (targetPositives + 1);
                                                else
                                                    sensitivity = (sensitivity * targetPositives + 0) / (targetPositives + 1);
                                                targetPositives++;
                                            }
                                            else
                                            {
                                                // This is for specificity
                                                if (predictedOutput[i] == 0)
                                                    specificity = (specificity * targetNegatives + 1) / (targetNegatives + 1);
                                                else
                                                    specificity = (specificity * targetNegatives + 0) / (targetNegatives + 1);
                                                targetNegatives++;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Update fitProgressBar
                        fitProgress++;
                        this.Invoke(new MethodInvoker(delegate () { validationProgressBar.Value = fitProgress; }));

                        // Insert new validation data in validationFlowLayoutPanel
                        insertValidatoinData(trainingSize, validationSize, accuracy, sensitivity, specificity, stepName);
                        refreshValidationData(stepName, false);
                    }
                }
            }
        }
    }
}

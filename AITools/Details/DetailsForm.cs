using BSP_Using_AI.AITools.DatasetExplorer;
using BSP_Using_AI.Database;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BSP_Using_AI.AITools.Details
{
    public partial class DetailsForm : Form, DbStimulatorReportHolder
    {
        public TFBackThread _tFBackThread;

        public long _modelId;
        public string _modelName;

        public List<List<long[]>> _trainingDetails;

        private object[] _validationData;

        private List<float[]> _outputThresholds;

        public DetailsForm(long modelId, string modelName, List<List<long[]>> trainingDetails, TFBackThread tFBackThread)
        {
            InitializeComponent();

            _modelId = modelId;
            _modelName = modelName;
            _trainingDetails = trainingDetails;
            _tFBackThread = tFBackThread;

            this.Text = modelName + " Details";

            // Qurey for validation data from models
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("models",
                                        new String[] { "validation_data", "outputs_thresholds" },
                                        "_id=?",
                                        new Object[] { _modelId },
                                        "", "DetailsFormForModels"));
            dbStimulatorThread.Start();

            // Query for all signals in dataset table
             dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            dbStimulatorThread = new Thread(() => dbStimulator.Query("dataset",
                                new String[] { "_id", "sginal_name", "starting_index", "sampling_rate" },
                                null,
                                null,
                                " ORDER BY sginal_name ASC", "DetailsFormForDataset"));
            dbStimulatorThread.Start();
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        private void refreshValidationData(int index, bool replace)
        {
            // Insert new vallidation data in validationFlowLayoutPanel
            ValidationFlowLayoutPanelUserControl validationFlowLayoutPanelUserControl = new ValidationFlowLayoutPanelUserControl();
            validationFlowLayoutPanelUserControl.modelTargetLabel.Text = ((string[])_validationData[index])[0];
            validationFlowLayoutPanelUserControl.algorithmTypeLabel.Text = ((string[])_validationData[index])[1];
            validationFlowLayoutPanelUserControl.datasetSizeLabel.Text = ((string[])_validationData[index])[2];
            validationFlowLayoutPanelUserControl.trainingDatasetLabel.Text = ((string[])_validationData[index])[3];
            validationFlowLayoutPanelUserControl.validationDatasetLabel.Text = ((string[])_validationData[index])[4];
            validationFlowLayoutPanelUserControl.accuracyLabel.Text = ((string[])_validationData[index])[5];
            validationFlowLayoutPanelUserControl.sensitivityLabel.Text = ((string[])_validationData[index])[6];
            validationFlowLayoutPanelUserControl.specificityLabel.Text = ((string[])_validationData[index])[7];

            if (index == 0 || index == 2 || index == 5)
                validationFlowLayoutPanelUserControl.thresholdTextBox.Enabled = false;
            else
                validationFlowLayoutPanelUserControl.thresholdTextBox.Text = _outputThresholds[index][0].ToString();

            if (replace)
                this.Invoke(new MethodInvoker(delegate () { validationFlowLayoutPanel.Controls.RemoveAt(index);
                                                            validationFlowLayoutPanel.Controls.Add(validationFlowLayoutPanelUserControl);
                                                            validationFlowLayoutPanel.Controls.SetChildIndex(validationFlowLayoutPanelUserControl, index); }));
            else
                this.Invoke(new MethodInvoker(delegate () { validationFlowLayoutPanel.Controls.Add(validationFlowLayoutPanelUserControl); }));
        }

        private void insertValidatoinData(int trainingSize, int validationSize, double accuracy, double sensitivity, double specificity, int step)
        {
            if (step == 1)
                ((string[])_validationData[step - 1])[0] = "R peaks detection";
            else if (step == 2)
                ((string[])_validationData[step - 1])[0] = "R selection";
            else if (step == 3)
                ((string[])_validationData[step - 1])[0] = "Beat peaks detection";
            else if (step == 4)
                ((string[])_validationData[step - 1])[0] = "P and T detection";
            else if (step == 5)
                ((string[])_validationData[step - 1])[0] = "Short PR detection";
            else if (step == 6)
                ((string[])_validationData[step - 1])[0] = "Delta detection";
            else
                ((string[])_validationData[step - 1])[0] = "WPW syndrome declaration";

            ((string[])_validationData[step - 1])[2] = (trainingSize + validationSize).ToString();
            ((string[])_validationData[step - 1])[3] = trainingSize.ToString();
            ((string[])_validationData[step - 1])[4] = validationSize.ToString();
            ((string[])_validationData[step - 1])[5] = accuracy.ToString();
            ((string[])_validationData[step - 1])[6] = sensitivity.ToString();
            ((string[])_validationData[step - 1])[7] = specificity.ToString();

            if (step == 1 || step == 3 || step == 6)
            {
                ((string[])_validationData[step - 1])[1] = "Regression";
                ((string[])_validationData[step - 1])[6] = "/";
                ((string[])_validationData[step - 1])[7] = "/";
            }
            else
                ((string[])_validationData[step - 1])[1] = "Classification";
        }

        private double[] askForPrediction(double[] input, string modelType, KNNBackThread.KNNModel knnModel, NaiveBayesBackThread.NaiveBayesModel nbModel, int step)
        {
            // Check which model is selected
            if (modelType.Contains("Neural network"))
            {
                // This is for neural network
                AutoResetEvent signal = new AutoResetEvent(false);
                ConcurrentQueue<object[]> queue = new ConcurrentQueue<object[]>();

                _tFBackThread._queue.Enqueue(new object[] { "predict", "DetailsForm", input, modelType, step, signal, queue });
                _tFBackThread._signal.Set();

                // Wait for the answer
                signal.WaitOne();

                object[] item = null;
                while (queue.TryDequeue(out item))
                {
                    // Check which function is selected
                    return (double[])item[0];
                }
            }
            else if (modelType.Contains("K-Nearest neighbor"))
            {
                // This is for knn
                return KNNBackThread.predict(input, knnModel, (List<double[]>)((List<object[]>)_tFBackThread._targetsModelsHashtable[modelType])[step][1]);
            }
            else if (modelType.Contains("Naive bayes"))
            {
                // This is for naive bayes
                return NaiveBayesBackThread.predict(input, nbModel, (List<double[]>)((List<object[]>)_tFBackThread._targetsModelsHashtable[modelType])[step][1]);
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
                _validationData[_validationData.Length - 1] = processingTime;

                // Update validation data in models table
                DbStimulator dbStimulator = new DbStimulator();
                dbStimulator.Update("models", new string[] { "validation_data" },
                    new Object[] { Garage.ObjectToByteArray(_validationData) }, _modelId, "DetailsForm");
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
            foreach (List<long[]> training in _trainingDetails)
                intervalsNum += training.Count;
            object[] selectionArgs = new object[intervalsNum * 2];
            intervalsNum = 0;
            selectionArgs[intervalsNum] = 0;
            selectionArgs[intervalsNum + 1] = 0;
            foreach (List<long[]> training in _trainingDetails)
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
            // Check if there exist values in validationFlowLayoutPanel
            if (validationFlowLayoutPanel.Controls.Count > 0)
                for (int i = 0; i < _outputThresholds.Count; i++)
                    if (i == 1 || i == 3 || i == 4 || i == 6)
                    {
                        // If yes then copy them to _outputThresholds
                        string threshold = ((ValidationFlowLayoutPanelUserControl)validationFlowLayoutPanel.Controls[i]).thresholdTextBox.Text.Replace(" ", "");
                        if (!threshold.Equals(""))
                            _outputThresholds[i][0] = float.Parse(threshold);
                        // Copy that in _targetsModelsHashtable
                        ((List<object[]>)_tFBackThread._targetsModelsHashtable[_modelName])[i][2] = _outputThresholds[i];
                        refreshValidationData(i, true);
                    }
            // Now save _outputThresholds in database
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Update("models",
                                        new String[] { "outputs_thresholds" },
                                        new object[] { Garage.ObjectToByteArray(_outputThresholds) },
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
                // Check if this is for validation data query
                if (callingClassName.Equals("DetailsFormForModels"))
                {
                    // Show validation data
                    try
                    {
                        _outputThresholds = (List<float[]>)Garage.ByteArrayToObject(dataTable.Rows[0].Field<byte[]>("outputs_thresholds"));
                        _validationData = (object[])Garage.ByteArrayToObject(dataTable.Rows[0].Field<byte[]>("validation_data"));

                        for (int i = 0; i < _validationData.Length - 1; i++)
                            refreshValidationData(i, false);

                        this.Invoke(new MethodInvoker(delegate () { timeToFinishLabel.Text = "Processed in: " + Garage.PeriodInSecToString((long)_validationData[_validationData.Length - 1]); }));
                    }
                    catch (Exception e)
                    {
                    }
                }
                else if (callingClassName.Equals("DetailsFormForDataset"))
                {
                    // Show dataset signals and updates values
                    // Set updates iterations
                    // Iterate through each training update
                    long[] updatesDatasetSize = new long[_trainingDetails.Count];
                    int updateListIndex;
                    for (int i = 0; i < _trainingDetails.Count; i++)
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
                        for (int i = 0; i < _trainingDetails.Count; i++)
                        {
                            List<long[]> training = _trainingDetails[i];
                            foreach (long[] datasetInterval in training)
                            {
                                if (row.Field<long>("_id") >= datasetInterval[0] && row.Field<long>("_id") <= datasetInterval[1])
                                {
                                    // If yes then create an item of the model
                                    DatasetFlowLayoutPanelItemUserControl datasetFlowLayoutPanelItemUserControl = new DatasetFlowLayoutPanelItemUserControl();
                                    datasetFlowLayoutPanelItemUserControl.signalNameLabel.Text = row.Field<string>("sginal_name");
                                    datasetFlowLayoutPanelItemUserControl.startingIndexLabel.Text = row.Field<long>("starting_index").ToString();
                                    datasetFlowLayoutPanelItemUserControl.samplingRateLabel.Text = row.Field<long>("sampling_rate").ToString();

                                    datasetFlowLayoutPanelItemUserControl.selectionCheckBox.Visible = false;
                                    datasetFlowLayoutPanelItemUserControl.featuresDetailsButton.Visible = false;

                                    this.Invoke(new MethodInvoker(delegate () { signalsFlowLayoutPanel.Controls.Add(datasetFlowLayoutPanelItemUserControl); }));

                                    // Set the signal to the correct update list
                                    if (i < _trainingDetails.Count - 1)
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
                    for (int i = 0; i < _outputThresholds.Count; i++)
                    {
                        if (i == 1 || i == 3 || i == 4 || i == 6)
                        {
                            float[] ones = new float[_outputThresholds[i].Length];
                            float allPoss = 0f;

                            foreach (DataRow row in dataTable.AsEnumerable())
                            {
                                OrderedDictionary orderedDictionaryBuff = (OrderedDictionary)Garage.ByteArrayToObject(row.Field<byte[]>("features"));
                                object[] stepFeatures = null;
                                if (orderedDictionaryBuff.Count > i + 1)
                                    stepFeatures = (object[])orderedDictionaryBuff[i + 1];
                                else
                                    continue;

                                if (i == 3)
                                {
                                    foreach (List<object[]> beat in stepFeatures)
                                        foreach (object[] feature in beat)
                                        {
                                            for (int j = 0; j < ((double[])feature[1]).Length; j++)
                                                ones[j] += (float)((double[])feature[1])[j];
                                            allPoss++;
                                        }
                                }
                                else
                                    foreach (object[] feature in stepFeatures)
                                        if (feature[1] != null)
                                        {
                                            for (int j = 0; j < ((double[])feature[1]).Length; j++)
                                                ones[j] += (float)((double[])feature[1])[j];
                                            allPoss++;
                                        }
                            }

                            // Calculate new threshold
                            for (int j = 0; j < ones.Length; j++)
                                ones[j] = ones[j] / allPoss > 0 ? (ones[j] / allPoss < 1 ? ones[j] / allPoss : 0.5f) : 0.5f;
                            // Update _outputThresholds with the new thresholds
                            _outputThresholds[i] = ones;
                            refreshValidationData(i, true);
                        }
                    }
                }
                else if (callingClassName.Equals("DetailsFormForFeatures"))
                {
                    // Shuffle all records first
                    Garage.Shuffle(dataTable);

                    // Initialize lists of features for each step
                    List<object[]> trainingFeatures;
                    List<object[]> validationFeatures;

                    // Separate features to 4 quarters
                    // and take 3 quarters as training data
                    // and the fourth quarter as validation data
                    int parts = 1;
                    int partFeaturesNmbr = 1;
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

                    partFeaturesNmbr = dataTable.Rows.Count / parts;

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
                    ///// The first item is just beats states
                    int fitProgress = 0;
                    //int stepsNum = ((OrderedDictionary)Garage.ByteArrayToObject((byte[])records[0][0])).Count - 1;
                    int stepsNum = 7;
                    // Initialize _validationData
                    _validationData = new object[stepsNum + 1];
                    for (int i = 0; i < stepsNum; i++)
                        _validationData[i] = new string[8];
                    this.Invoke(new MethodInvoker(delegate () { validationFlowLayoutPanel.Controls.Clear(); }));
                    // Set maximum of progress bar
                    this.Invoke(new MethodInvoker(delegate () { validationProgressBar.Maximum = stepsNum; }));
                    double accuracy, sensitivity, specificity;
                    int trainingSize, validationSize, possibilities, targetPositives, targetNegatives;
                    // Calculate number of data to be processed
                    _data = 0;
                    for (int i = 1; i < (stepsNum + 1); i++)
                    {
                        for (int x = validationPart; x < parts; x++)
                            for (int j = 0; j < dataTable.Rows.Count; j++)
                            {
                                DataRow row = dataTable.Rows[j];
                                OrderedDictionary orderedDictionaryBuff = (OrderedDictionary)Garage.ByteArrayToObject(row.Field<byte[]>("features"));
                                object[] stepFeatures = null;
                                if (orderedDictionaryBuff.Count > i)
                                    stepFeatures = (object[])orderedDictionaryBuff[i];
                                else
                                    continue;
                                if (i == 1)
                                {
                                    if ((x * partFeaturesNmbr <= j && j < (x + 1) * partFeaturesNmbr) || (x == parts - 1 && x * partFeaturesNmbr <= j))
                                        _data += ((double[])stepFeatures[1]).Length;
                                }
                                else if (i == 4)
                                {
                                    foreach (List<object[]> beat in stepFeatures)
                                        foreach (object[] feature in beat)
                                            if ((x * partFeaturesNmbr <= j && j < (x + 1) * partFeaturesNmbr) || (x == parts - 1 && x * partFeaturesNmbr <= j))
                                                _data += ((double[])feature[1]).Length;
                                }
                                else
                                    foreach (object[] feature in stepFeatures)
                                        if ((x * partFeaturesNmbr <= j && j < (x + 1) * partFeaturesNmbr) || (x == parts - 1 && x * partFeaturesNmbr <= j))
                                            if(feature[1] != null)
                                                _data += ((double[])feature[1]).Length;
                            }
                    }
                    _remainingData = _data;

                    // Start validation process for each step model
                    calculateTimeToFinish();
                    for (int i = 1; i < (stepsNum + 1); i++)
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
                            trainingFeatures = new List<object[]>();
                            validationFeatures = new List<object[]>();
                            List<int> selectedBeatLastState = new List<int>();
                            // and split them into 75% for training and 25% for validation in trainingFeatures and validationFeatures respectively
                            object[] stepFeatures = null;
                            for (int j = 0; j < dataTable.Rows.Count; j++)
                            {
                                DataRow row = dataTable.Rows[j];
                                OrderedDictionary orderedDictionaryBuff = (OrderedDictionary)Garage.ByteArrayToObject(row.Field<byte[]>("features"));
                                List<int[]> signalBeatsSpecs = (List<int[]>)orderedDictionaryBuff[0];
                                if (orderedDictionaryBuff.Count > i)
                                    stepFeatures = (object[])orderedDictionaryBuff[i];
                                else
                                    continue;
                                if (i == 1)
                                {
                                    // Check if this feature is for validation
                                    if ((x * partFeaturesNmbr <= j && j < (x + 1) * partFeaturesNmbr) || (x == parts - 1 && x * partFeaturesNmbr <= j))
                                    {
                                        // This is for validation
                                        // Add the length of the signal in addition to the features for mean squared error metrics
                                        object[] stepFeaturesBuff = new object[3] { stepFeatures[0], stepFeatures[1], (int)signalBeatsSpecs[signalBeatsSpecs.Count - 1][7] };
                                        validationFeatures.Add(stepFeaturesBuff);
                                    }
                                    else
                                        // This is for training
                                        trainingFeatures.Add(stepFeatures);
                                }
                                else if (i == 4)
                                    foreach (List<object[]> beat in stepFeatures)
                                    {
                                        if ((x * partFeaturesNmbr <= j && j < (x + 1) * partFeaturesNmbr) || (x == parts - 1 && x * partFeaturesNmbr <= j))
                                            selectedBeatLastState.Add(beat.Count);

                                        foreach (object[] feature in beat)
                                        {
                                            if ((x * partFeaturesNmbr <= j && j < (x + 1) * partFeaturesNmbr) || (x == parts - 1 && x * partFeaturesNmbr <= j))
                                                validationFeatures.Add(feature);
                                            else
                                                trainingFeatures.Add(feature);
                                        }
                                    }
                                else
                                    for (int k = 0; k < stepFeatures.Length; k++)
                                        if (((object[])stepFeatures[k])[0] != null)
                                        {
                                            if ((x * partFeaturesNmbr <= j && j < (x + 1) * partFeaturesNmbr) || (x == parts - 1 && x * partFeaturesNmbr <= j))
                                            {
                                                object[] stepFeaturesBuff = new object[3] { ((object[])stepFeatures[k])[0], ((object[])stepFeatures[k])[1], 0d };
                                                // Check if this is 3rd step (beat peaks scan)
                                                if (i == 3)
                                                {
                                                    // If yes then add the length of the beat in addition to the features for mean squared error metrics
                                                    stepFeaturesBuff[2] = (int)signalBeatsSpecs[k][7] - (int)signalBeatsSpecs[k][0];
                                                }
                                                validationFeatures.Add(stepFeaturesBuff);
                                            }
                                            else
                                                trainingFeatures.Add((object[])stepFeatures[k]);
                                        }
                            }

                            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            // Create The model from trainingFeatures
                            // Send features for fitting
                            // Check which model is selected
                            //trainingSize += trainingFeatures.Count;
                            trainingSize = trainingFeatures.Count;
                            //validationSize += validationFeatures.Count;
                            validationSize = validationFeatures.Count;
                            KNNBackThread.KNNModel knnModel = new KNNBackThread.KNNModel { };
                            NaiveBayesBackThread.NaiveBayesModel nbModel = new NaiveBayesBackThread.NaiveBayesModel { };
                            if (_modelName.Contains("Neural network"))
                            {
                                // This is for neural network
                                _tFBackThread._queue.Enqueue(new object[] { "fit", this.Name, i, stepsNum, trainingFeatures, _modelName });
                                _tFBackThread._signal.Set();
                            }
                            else if (_modelName.Contains("K-Nearest neighbor"))
                            {
                                // This is for knn
                                // Create a KNN model structure with the initial optimum K, which is "3"
                                knnModel = KNNBackThread.createKNNModel(3);

                                // Fit features
                                knnModel = KNNBackThread.fit(knnModel, trainingFeatures, new int[stepsNum], i - 1, (List<double[]>)((List<object[]>)_tFBackThread._targetsModelsHashtable[_modelName])[i - 1][1]);
                            }
                            else if (_modelName.Contains("Naive bayes"))
                            {
                                // This is for naive bayes
                                nbModel = NaiveBayesBackThread.createNBModel(i);

                                // Fit features
                                nbModel = NaiveBayesBackThread.fit(nbModel, trainingFeatures, (List<double[]>)((List<object[]>)_tFBackThread._targetsModelsHashtable[_modelName])[i - 1][1]);
                            }

                            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            // Calculate validation metrics
                            double[] output;
                            List<(double[], double[])> predictedValidOutputsList = new List<(double[], double[])>();

                            int selectedBeat = 0;
                            int lastBeatState = 0;
                            for (int j = 0; j < validationFeatures.Count; j++)
                            {
                                // Get prediction output for current feature
                                output = askForPrediction((double[])validationFeatures[j][0], _modelName, knnModel, nbModel, i - 1);
                                for (int l = 0; l < output.Length; l++)
                                    _remainingData--;

                                // Check which type of validation metrics is this
                                if (i == 1 || i == 3 || i == 6)
                                {
                                    // If yes then this is for regression metrics
                                    for (int l = 0; l < output.Length; l++)
                                    {
                                        // Create the scaling factor according to each output type
                                        double scalingFactor = 1d;
                                        if ((i == 1 || i == 3) && l == 0)
                                            scalingFactor = 1000d;
                                        else if ((i == 1 || i == 3) && l == 1)
                                            scalingFactor = (int)validationFeatures[j][2] + 9; // 9: signalStatesViewerUserControl.hThresholdScrollBar.LargeChange - 1
                                        else if (i == 6)
                                            scalingFactor = 100000d;

                                        // Calculate accuracy using Mean Absolute Percentage Error formula
                                        // where accuracy should be (1 - mape)
                                        double mape = Math.Abs((((double[])validationFeatures[j][1])[l] * scalingFactor - output[l] * scalingFactor) / (((double[])validationFeatures[j][1])[l] * scalingFactor));
                                        accuracy = 1d - (((1d - accuracy) * possibilities + mape) / (possibilities + 1));
                                        possibilities++;
                                    }
                                }
                                else
                                {
                                    // This is for classification metrics
                                    // Check if this is for P and T selection
                                    if (i == 4)
                                    {
                                        // If yes then prediction should be rearanged for each beat states
                                        // Check if lastBeatState is not selected
                                        if (lastBeatState == 0)
                                            // If yes then select first beat states number
                                            lastBeatState = selectedBeatLastState[selectedBeat] - 1;

                                        // Check if last state of current beat is not reached yet
                                        if (j < lastBeatState - 1)
                                            // If yes then just add current state's outputs in predictedValidOutput
                                            predictedValidOutputsList.Add((output, (double[])validationFeatures[j][1]));
                                        else
                                        {
                                            // If yes then calculate validation metrics after rearanging predicted outputs
                                            predictedValidOutputsList.Add((output, (double[])validationFeatures[j][1]));
                                            // Rearange predicted outputs
                                            (double, int) pProba = (0, -1);
                                            (double, int) tProba = (0, -1);
                                            for (int k = 0; k < predictedValidOutputsList.Count; k++)
                                            {
                                                if (predictedValidOutputsList[k].Item1[0] > pProba.Item1)
                                                {
                                                    pProba.Item1 = predictedValidOutputsList[k].Item1[0];
                                                    predictedValidOutputsList[k].Item1[0] = 1d;
                                                    if (pProba.Item2 >= 0)
                                                        predictedValidOutputsList[pProba.Item2].Item1[0] = 0d;
                                                    pProba.Item2 = k;
                                                } else
                                                    predictedValidOutputsList[k].Item1[0] = 0d;

                                                if (predictedValidOutputsList[k].Item1[1] > tProba.Item1)
                                                {
                                                    tProba.Item1 = predictedValidOutputsList[k].Item1[1];
                                                    predictedValidOutputsList[k].Item1[1] = 1d;
                                                    if (tProba.Item2 >= 0)
                                                        predictedValidOutputsList[tProba.Item2].Item1[1] = 0d;
                                                    tProba.Item2 = k;
                                                }
                                                else
                                                    predictedValidOutputsList[k].Item1[1] = 0d;
                                            }
                                            // Update validation metrics
                                            for (int k = 0; k < predictedValidOutputsList.Count; k++)
                                                for (int l = 0; l < predictedValidOutputsList[k].Item1.Length; l++)
                                                {
                                                    // Calculate accuracy
                                                    if (predictedValidOutputsList[k].Item1[l] == predictedValidOutputsList[k].Item2[l])
                                                        accuracy = (accuracy * possibilities + 1) / (possibilities + 1);
                                                    else
                                                        accuracy = (accuracy * possibilities + 0) / (possibilities + 1);
                                                    possibilities++;

                                                    // Calculate sensitivity and specificity
                                                    if (predictedValidOutputsList[k].Item2[l] == 1)
                                                    {
                                                        // This is for sensitivity
                                                        if (predictedValidOutputsList[k].Item1[l] == 1)
                                                            sensitivity = (sensitivity * targetPositives + 1) / (targetPositives + 1);
                                                        else
                                                            sensitivity = (sensitivity * targetPositives + 0) / (targetPositives + 1);
                                                        targetPositives++;
                                                    } else
                                                    {
                                                        // This is for specificity
                                                        if (predictedValidOutputsList[k].Item1[l] == 0)
                                                            specificity = (specificity * targetNegatives + 1) / (targetNegatives + 1);
                                                        else
                                                            specificity = (specificity * targetNegatives + 0) / (targetNegatives + 1);
                                                        targetNegatives++;
                                                    }
                                                }

                                            // Set the new lastBeatState
                                            selectedBeat++;
                                            if (selectedBeatLastState.Count > selectedBeat)
                                                lastBeatState += selectedBeatLastState[selectedBeat];
                                            predictedValidOutputsList = new List<(double[], double[])>();
                                        }
                                    }
                                    else
                                    {
                                        // Set validation data for other steps
                                        for (int l = 0; l < output.Length; l++)
                                        {
                                            // Get curretn step threshold
                                            float threshold = ((float[])((List<object[]>)_tFBackThread._targetsModelsHashtable[_modelName])[i - 1][2])[l];
                                            // Regulate output value
                                            if (output[l] >= threshold)
                                                output[l] = 1d;
                                            else
                                                output[l] = 0d;
                                            // Calculate accuracy
                                            if (output[l] == ((double[])validationFeatures[j][1])[l])
                                                accuracy = (accuracy * possibilities + 1) / (possibilities + 1);
                                            else
                                                accuracy = (accuracy * possibilities + 0) / (possibilities + 1);
                                            possibilities++;

                                            // Calculate sensitivity and specificity
                                            if (((double[])validationFeatures[j][1])[l] == 1)
                                            {
                                                // This is for sensitivity
                                                if (output[l] == 1)
                                                    sensitivity = (sensitivity * targetPositives + 1) / (targetPositives + 1);
                                                else
                                                    sensitivity = (sensitivity * targetPositives + 0) / (targetPositives + 1);
                                                targetPositives++;
                                            }
                                            else
                                            {
                                                // This is for specificity
                                                if (output[l] == 0)
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
                        insertValidatoinData(trainingSize, validationSize, accuracy, sensitivity, specificity, i);
                        refreshValidationData(i - 1, false);
                    }
                }
            }
        }
    }
}

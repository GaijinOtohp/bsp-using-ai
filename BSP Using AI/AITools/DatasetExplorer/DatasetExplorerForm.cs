using BSP_Using_AI.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools.DatasetExplorer
{
    public partial class DatasetExplorerForm : Form, DbStimulatorReportHolder
    {
        public MainForm _mainForm;

        public long _id;
        public ARTHTModels _aRTHTModels;

        public long _datasetSize;
        public int _updatesNum;
        public AIToolsForm _aIToolsForm;
        public int _lastSelectedItem_shift = -1;
        public bool _shiftClicked = false;

        public DatasetExplorerForm()
        {
            InitializeComponent();
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public void queryForSignals()
        {
            // Query for all signals in dataset table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("dataset",
                                new String[] { "_id", "sginal_name", "starting_index", "sampling_rate", "quantisation_step" },
                                null,
                                null,
                                " ORDER BY sginal_name ASC", "DatasetExplorerFormForDataset"));
            dbStimulatorThread.Start();
        }

        public void queryForTrainingDataset()
        {
            // Check if there was any added intervals
            List<List<long[]>> dataIdsIntervalsList = _aRTHTModels.DataIdsIntervalsList;
            if (!(_aRTHTModels.DataIdsIntervalsList.Count > _updatesNum))
                return;

            // Qurey for signals features in all last selected intervals from dataset
            string selection = "_id>=? and _id<=?";
            int intervalsNum = 1;
            if (dataIdsIntervalsList.Count > 0)
                intervalsNum += dataIdsIntervalsList[dataIdsIntervalsList.Count - 1].Count;
            object[] selectionArgs = new object[intervalsNum * 2];
            intervalsNum = 0;
            selectionArgs[intervalsNum] = 0;
            selectionArgs[intervalsNum + 1] = 0;
            foreach (long[] datasetInterval in dataIdsIntervalsList[dataIdsIntervalsList.Count - 1])
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
                                        "", "DatasetExplorerFormForFeatures"));
            dbStimulatorThread.Start();
        }

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void removeSelectionButton_Click(object sender, EventArgs e)
        {
            // Show message for confirming the action
            DialogResult dialogResult = MessageBox.Show("Are you sure about deleting all selected signals?", "Action confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                // Iterate through all signals in signalsFlowLayoutPanel
                foreach (DatasetFlowLayoutPanelItemUserControl item in signalsFlowLayoutPanel.Controls)
                    // Check if item is selected to be removed
                    if (item.selectionCheckBox.Checked)
                    {
                        // If selected then remove it from table
                        // Remove it from models table
                        DbStimulator dbStimulator = new DbStimulator();
                        dbStimulator.Delete("dataset",
                                                    "_id=?",
                                                    new Object[] { item._id },
                                                    "DatasetExplorerForm");
                    }
                // Reset badge number of MainForm
                _mainForm.resetBadge();

                // Refresh modelsFlowLayoutPanel
                ((DatasetExplorerForm)this.FindForm()).queryForSignals();
            }
        }

        private void uselectAllButton_Click(object sender, EventArgs e)
        {
            // Iterate through all signals in signalsFlowLayoutPanel
            foreach (DatasetFlowLayoutPanelItemUserControl item in signalsFlowLayoutPanel.Controls)
                // Unselect item
                item.selectionCheckBox.Checked = false;

        }

        private void fitSelectionButton_Click(object sender, EventArgs e)
        {
            // Create a list from signalsFlowLayoutPanel and sort it according to item._id
            List<(bool enabled, bool selected, long id)> signalsList = new List<(bool enabled, bool selected, long id)>();
            foreach (DatasetFlowLayoutPanelItemUserControl item in signalsFlowLayoutPanel.Controls)
                signalsList.Add((item.Enabled, item.selectionCheckBox.Checked, item._id));
            // Sort the list according to _id
            signalsList.Sort((e1, e2) => { return e1.id.CompareTo(e2.id); });

            // Set selected signals in _trainingDetails
            List<long[]> datasetIntervalsList = new List<long[]>();
            datasetIntervalsList.Add(new long[] { -1, -1 });
            // Iterate through all signals in signalsFlowLayoutPanel
            foreach ((bool enabled, bool selected, long id) item in signalsList)
                // Check if current item is enabled
                if (item.enabled)
                {
                    // If yes then check if it's selected for training
                    if (item.selected)
                    {
                        // If yes then check if this should be in the start of the interval or end
                        if (datasetIntervalsList[datasetIntervalsList.Count - 1][0] == -1)
                        {
                            // Then start and end of the interval
                            datasetIntervalsList[datasetIntervalsList.Count - 1][0] = item.id;
                            datasetIntervalsList[datasetIntervalsList.Count - 1][1] = item.id;
                        }
                        else
                            // Update the end of the interval
                            datasetIntervalsList[datasetIntervalsList.Count - 1][1] = item.id;
                    }
                    else
                    {
                        // If yes then check if last interval is set
                        if (datasetIntervalsList[datasetIntervalsList.Count - 1][1] != -1)
                            // If yes then start new interval
                            datasetIntervalsList.Add(new long[] { -1, -1 });
                    }
                }
                else
                {
                    // If yes then check if last interval is set
                    if (datasetIntervalsList[datasetIntervalsList.Count - 1][1] != -1)
                        // If yes then start new interval
                        datasetIntervalsList.Add(new long[] { -1, -1 });
                }

            // Remove last interval if it's not defined
            if (datasetIntervalsList[datasetIntervalsList.Count - 1][1] == -1)
                datasetIntervalsList.RemoveAt(datasetIntervalsList.Count - 1);

            // Insert new intervals in _trainingDetails if there exist any interval
            if (datasetIntervalsList.Count > 0)
                _aRTHTModels.DataIdsIntervalsList.Add(datasetIntervalsList);

            // Start training
            queryForTrainingDataset();

            // Close form
            this.Close();
        }

        private void signalsFlowLayoutPanel_SizeChanged(object sender, EventArgs e)
        {
            EventHandlers.signalFlowLayout_SizeChanged(sender, 565);
        }

        private void DatasetExplorerForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the pressed key is "Shift"
            if (e.Shift)
                _shiftClicked = true;
        }

        private void DatasetExplorerForm_KeyUp(object sender, KeyEventArgs e)
        {
            _shiftClicked = false;
        }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //:::::::::::::::::::::::::::CROSS PROCESS FORM FUNCTIONS (INTERFACES)::::::::::::::::::::::://
        public void holdRecordReport(DataTable dataTable, string callingClassName)
        {
            if (!callingClassName.Contains("DatasetExplorerForm"))
                return;

            // Set dataset in signalsFlowLayoutPanel
            if (callingClassName.Equals("DatasetExplorerFormForDataset"))
            {
                // Clear modelsFlowLayoutPanel
                if (signalsFlowLayoutPanel.Controls.Count > 0)
                    if (IsHandleCreated) this.Invoke(new MethodInvoker(delegate () { signalsFlowLayoutPanel.Controls.Clear(); }));

                // Order datatable by name
                List<DataRow> rowsList = new List<DataRow>(dataTable.AsEnumerable());
                List<string> namesList = new List<string>();
                foreach (DataRow row in rowsList)
                    namesList.Add(row.Field<string>("sginal_name"));
                rowsList = Garage.OrderByTextWithNumbers(rowsList, namesList);

                // Insert new items from records
                foreach (DataRow row in rowsList)
                {
                    // Create an item of the model
                    DatasetFlowLayoutPanelItemUserControl datasetFlowLayoutPanelItemUserControl = new DatasetFlowLayoutPanelItemUserControl();
                    datasetFlowLayoutPanelItemUserControl.signalNameLabel.Text = row.Field<string>("sginal_name");
                    datasetFlowLayoutPanelItemUserControl.startingIndexLabel.Text = row.Field<long>("starting_index").ToString();
                    datasetFlowLayoutPanelItemUserControl.samplingRateLabel.Text = row.Field<long>("sampling_rate").ToString();
                    datasetFlowLayoutPanelItemUserControl.quantizationStepLabel.Text = row.Field<long>("quantisation_step").ToString();
                    datasetFlowLayoutPanelItemUserControl._id = row.Field<long>("_id");

                    // Check if this form is for "Training dataset explorer"
                    if (this.Text.Equals("Training dataset explorer"))
                    {
                        // If yes then check the selection of the signal and disable featuresDetailsButton
                        datasetFlowLayoutPanelItemUserControl.selectionCheckBox.Checked = true;
                        datasetFlowLayoutPanelItemUserControl.featuresDetailsButton.Enabled = false;

                        // Check if its _id is included in _trainingDetails
                        foreach (List<long[]> training in _aRTHTModels.DataIdsIntervalsList)
                            foreach (long[] datasetInterval in training)
                                if (row.Field<long>("_id") >= datasetInterval[0] && row.Field<long>("_id") <= datasetInterval[1])
                                    // If yes then disable the UserControl 
                                    datasetFlowLayoutPanelItemUserControl.Enabled = false;
                    }

                    if (IsHandleCreated) this.Invoke(new MethodInvoker(delegate () { signalsFlowLayoutPanel.Controls.Add(datasetFlowLayoutPanelItemUserControl); }));
                }
            }
            // Set features for training
            else if (callingClassName.Equals("DatasetExplorerFormForFeatures"))
            {
                // Initialize lists of features for each step
                Dictionary<string, List<Sample>> dataLists = new Dictionary<string, List<Sample>>(7);

                // Iterate through each signal samples and sort them in dataLists
                ARTHTFeatures aRTHTFeatures = null;
                foreach (DataRow row in dataTable.AsEnumerable())
                {
                    aRTHTFeatures = Garage.ByteArrayToObject<ARTHTFeatures>(row.Field<byte[]>("features"));
                    foreach (string stepName in aRTHTFeatures.StepsDataDic.Keys)
                    {
                        if (!dataLists.ContainsKey(stepName))
                            dataLists.Add(stepName, new List<Sample>());

                        foreach (Sample sample in aRTHTFeatures.StepsDataDic[stepName].Samples)
                            dataLists[stepName].Add(sample);
                    }
                }
                // Send features for fitting
                // Check which model is selected
                long datasetSize = _datasetSize + dataTable.Rows.Count;
                if (_aRTHTModels.Name.Contains("Neural network"))
                {
                    // This is for neural network
                    _aIToolsForm._tFBackThread._queue.Enqueue(new QueueSignalInfo()
                    {
                        TargetFunc = "fit",
                        CallingClass = "DatasetExplorerForm",
                        ModelsName = _aRTHTModels.Name,
                        DataLists = dataLists,
                        _datasetSize = datasetSize,
                        _modelId = _id,
                        StepName = ""
                    });
                    _aIToolsForm._tFBackThread._signal.Set();
                }
                else if (_aRTHTModels.Name.Contains("K-Nearest neighbor"))
                {
                    // This is for knn
                    KNNBackThread kNNBackThread = new KNNBackThread(_aIToolsForm._arthtModelsDic, _aIToolsForm);
                    Thread knnThread = new Thread(() => kNNBackThread.fit(_aRTHTModels.Name, dataLists, datasetSize, _id, ""));
                    knnThread.Start();
                }
                else if (_aRTHTModels.Name.Contains("Naive bayes"))
                {
                    // This is for naive bayes
                    NaiveBayesBackThread naiveBayesBackThread = new NaiveBayesBackThread(_aIToolsForm._arthtModelsDic, _aIToolsForm);
                    Thread nbThread = new Thread(() => naiveBayesBackThread.fit(_aRTHTModels.Name, dataLists, datasetSize, _id, ""));
                    nbThread.Start();
                }
            }
        }
    }
}

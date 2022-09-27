using BSP_Using_AI.Database;
using System;
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

namespace BSP_Using_AI.AITools.DatasetExplorer
{
    public partial class DatasetExplorerForm : Form, DbStimulatorReportHolder
    {
        public MainForm _mainForm;

        public long _id;
        public string _modelPath;
        public long _datasetSize;
        public int _updatesNum;
        public List<List<long[]>> _trainingDetails;
        public AIToolsForm _aIToolsForm;
        public string _modelName;
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
            dbStimulator.initialize("dataset",
                                new String[] { "_id", "sginal_name", "starting_index", "sampling_rate" },
                                null,
                                null,
                                " ORDER BY sginal_name ASC", "DatasetExplorerFormForDataset");
            Thread dbStimulatorThread = new Thread(new ThreadStart(dbStimulator.run));
            dbStimulatorThread.Start();
        }

        public void queryForTrainingDataset()
        {
            // Check if there was any added intervals
            if (!(_trainingDetails.Count > _updatesNum))
                return;

            // Qurey for signals features in all last selected intervals from dataset
            string selection = "_id>=? and _id<=?";
            int intervalsNum = 1;
            if (_trainingDetails.Count > 0)
                intervalsNum += _trainingDetails[_trainingDetails.Count - 1].Count;
            object[] selectionArgs = new object[intervalsNum * 2];
            intervalsNum = 0;
            selectionArgs[intervalsNum] = 0;
            selectionArgs[intervalsNum + 1] = 0;
            foreach (long[] datasetInterval in _trainingDetails[_trainingDetails.Count - 1])
            {
                intervalsNum += 2;
                selection += " or _id>=? and _id<=?";
                selectionArgs[intervalsNum] = datasetInterval[0];
                selectionArgs[intervalsNum + 1] = datasetInterval[1];
            }

            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            dbStimulator.initialize("dataset",
                                        new String[] { "features" },
                                        selection,
                                        selectionArgs,
                                        "", "DatasetExplorerFormForFeatures");
            Thread dbStimulatorThread = new Thread(new ThreadStart(dbStimulator.run));
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
                        dbStimulator.initialize("dataset",
                                                    "_id=?",
                                                    new Object[] { item._id },
                                                    "DatasetExplorerForm");
                        dbStimulator.run();
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
                _trainingDetails.Add(datasetIntervalsList);

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
        public void holdRecordReport(List<object[]> records, string callingClassName)
        {
            if (!callingClassName.Contains("DatasetExplorerForm"))
                return;

            // Set dataset in signalsFlowLayoutPanel
            if (callingClassName.Equals("DatasetExplorerFormForDataset"))
            {
                // Clear modelsFlowLayoutPanel
                if (signalsFlowLayoutPanel.Controls.Count > 0)
                    this.Invoke(new MethodInvoker(delegate () { signalsFlowLayoutPanel.Controls.Clear(); }));

                // Insert new items from records
                foreach (object[] record in records)
                {
                    // Create an item of the model
                    DatasetFlowLayoutPanelItemUserControl datasetFlowLayoutPanelItemUserControl = new DatasetFlowLayoutPanelItemUserControl();
                    datasetFlowLayoutPanelItemUserControl.signalNameLabel.Text = (string)record[1];
                    datasetFlowLayoutPanelItemUserControl.startingIndexLabel.Text = record[2].ToString();
                    datasetFlowLayoutPanelItemUserControl.samplingRateLabel.Text = ((int)record[3]).ToString();
                    datasetFlowLayoutPanelItemUserControl._id = (long)record[0];

                    // Check if this form is for "Training dataset explorer"
                    if (this.Text.Equals("Training dataset explorer"))
                    {
                        // If yes then check the selection of the signal and disable featuresDetailsButton
                        datasetFlowLayoutPanelItemUserControl.selectionCheckBox.Checked = true;
                        datasetFlowLayoutPanelItemUserControl.featuresDetailsButton.Enabled = false;

                        // Check if its _id is included in _trainingDetails
                        foreach (List<long[]> training in _trainingDetails)
                            foreach (long[] datasetInterval in training)
                                if ((long)record[0] >= datasetInterval[0] && (long)record[0] <= datasetInterval[1])
                                    // If yes then disable the UserControl 
                                    datasetFlowLayoutPanelItemUserControl.Enabled = false;
                    }

                    this.Invoke(new MethodInvoker(delegate () { signalsFlowLayoutPanel.Controls.Add(datasetFlowLayoutPanelItemUserControl); }));
                }
            }
            // Set features for training
            else if (callingClassName.Equals("DatasetExplorerFormForFeatures"))
            {
                // Initialize lists of features for each step
                List<object[]>[] featuresLists = new List<object[]>[7];
                for (int i = 0; i < featuresLists.Length; i++)
                    featuresLists[i] = new List<object[]>();

                // Iterate through each signal features and sort them in featuresLists
                OrderedDictionary signalFeatures = null;
                foreach (object[] record in records)
                {
                    signalFeatures = (OrderedDictionary)Garage.ByteArrayToObject((byte[])record[0]);
                    // The first item is just beats states
                    for (int i = 1; i < signalFeatures.Count; i++)
                    {
                        if (i == 1)
                            featuresLists[i - 1].Add((object[])signalFeatures[i]);
                        else if (i == 4)
                            foreach (List<object[]> beat in (object[])signalFeatures[i])
                                foreach (object[] feature in beat)
                                    featuresLists[i - 1].Add(feature);
                        else
                            foreach (object[] feature in (object[])signalFeatures[i])
                                if (feature[0] != null)
                                    featuresLists[i - 1].Add(feature);
                    }
                }
                // Send features for fitting
                // Check which model is selected
                long datasetSize = _datasetSize + records.Count;
                if (_modelName.Contains("Neural network"))
                {
                    // This is for neural network
                    _aIToolsForm._tFBackThread._queue.Enqueue(new object[] { "fit", _modelName, featuresLists, _modelPath, datasetSize, _id, _trainingDetails, -1 });
                    _aIToolsForm._tFBackThread._signal.Set();
                }
                else if (_modelName.Contains("K-Nearest neighbor"))
                {
                    // This is for knn
                    KNNBackThread kNNBackThread = new KNNBackThread(_aIToolsForm._targetsModelsHashtable, _aIToolsForm);
                    Thread knnThread = new Thread(() => kNNBackThread.fit(_modelName, featuresLists, datasetSize, _id, _trainingDetails, -1));
                    knnThread.Start();
                }
                else if (_modelName.Contains("Naive bayes"))
                {
                    // This is for naive bayes
                    NaiveBayesBackThread naiveBayesBackThread = new NaiveBayesBackThread(_aIToolsForm._targetsModelsHashtable, _aIToolsForm);
                    Thread nbThread = new Thread(() => naiveBayesBackThread.fit(_modelName, featuresLists, datasetSize, _id, _trainingDetails, -1));
                    nbThread.Start();
                }
            }
        }
    }
}

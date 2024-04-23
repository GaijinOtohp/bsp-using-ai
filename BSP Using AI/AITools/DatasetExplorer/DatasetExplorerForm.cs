using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.AITools.Keras_NET_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.KNN_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.NaiveBayes_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools.DatasetExplorer
{
    public partial class DatasetExplorerForm : Form, DbStimulatorReportHolder
    {
        public MainForm _mainForm;

        public long _id;

        public int _lastSelectedItem_shift = -1;
        public bool _shiftClicked = false;

        private bool _ignoreEvent = false;

        public DatasetExplorerForm(string title)
        {
            InitializeComponent();
            this.Text = title;

            // Check if this is for dataset exploring
            if (title.Equals("Dataset explorer"))
            {
                // Include the available objectives in aiGoalComboBox
                _ignoreEvent = true;
                string[] objectives = typeof(AIModels_ObjectivesArchitectures).GetNestedTypes().Where(type => type.GetField("ObjectiveName") != null).
                                                                                                Select(type => (string)type.GetField("ObjectiveName").GetValue(null)).ToArray();
                aiGoalComboBox.DataSource = objectives;
                _ignoreEvent = false;
            }
            else
                // Hide aiGoalComboBox
                aiGoalComboBox.Visible = false;
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
                                " ORDER BY sginal_name ASC", "DatasetExplorerFormForDataset"));
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
                                " ORDER BY sginal_name ASC", "DatasetExplorerFormForDataset"));
            dbStimulatorThread.Start();
        }

        public void queryForSignals_CWD()
        {
            queryForSignals_Anno(CharacteristicWavesDelineation.ObjectiveName);
        }

        public void queryForSignals_ArrhyCla()
        {
            queryForSignals_Anno(ArrhythmiaClassification.ObjectiveName);
        }

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        public void aiGoalComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ignoreEvent)
                return;

            if (aiGoalComboBox.Text.Equals(CharacteristicWavesDelineation.ObjectiveName))
                queryForSignals_CWD();
            else if (aiGoalComboBox.Text.Equals(ArrhythmiaClassification.ObjectiveName))
                queryForSignals_ArrhyCla();
            else
                queryForSignals_ARTHT();
        }

        public void DeleteDataById(long id, string callingClassName)
        {
            // Set table name
            string tableName;
            if (aiGoalComboBox.Text.Equals(CharacteristicWavesDelineation.ObjectiveName) ||
                aiGoalComboBox.Text.Equals(ArrhythmiaClassification.ObjectiveName))
                tableName = "anno_ds";
            else
                tableName = "dataset";

            // Remove it from the table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.Delete(tableName,
                                        "_id=?",
                                        new Object[] { id },
                                        callingClassName);
        }

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
                        // If selected then remove it from table
                        DeleteDataById(item._id, "DatasetExplorerForm");

                // Reset badge number of MainForm
                _mainForm.resetBadge();

                // Refresh modelsFlowLayoutPanel
                aiGoalComboBox_SelectedIndexChanged(null, null);
            }
        }

        private void uselectAllButton_Click(object sender, EventArgs e)
        {
            // Iterate through all signals in signalsFlowLayoutPanel
            foreach (DatasetFlowLayoutPanelItemUserControl item in signalsFlowLayoutPanel.Controls)
                // Unselect item
                item.selectionCheckBox.Checked = false;

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

        private void fitSelectionButton_Click(object sender, EventArgs e)
        {
            // Send event to Training_DatasetExplorerForm
            fitSelectionButton_Click_Training(sender, e);
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
                rowsList = GeneralTools.OrderByTextWithNumbers(rowsList, namesList);

                // Insert new items from records
                foreach (DataRow row in rowsList)
                {
                    // Create an item of the model
                    DatasetFlowLayoutPanelItemUserControl datasetFlowLayoutPanelItemUserControl = new DatasetFlowLayoutPanelItemUserControl();
                    datasetFlowLayoutPanelItemUserControl.signalNameLabel.Text = row.Field<string>("sginal_name");
                    datasetFlowLayoutPanelItemUserControl.startingIndexLabel.Text = row.Field<long>("starting_index").ToString();
                    datasetFlowLayoutPanelItemUserControl.samplingRateLabel.Text = row.Field<long>("sampling_rate").ToString();
                    datasetFlowLayoutPanelItemUserControl.quantizationStepLabel.Text = row.Field<long>("quantisation_step").ToString();
                    datasetFlowLayoutPanelItemUserControl._Table = dataTable.TableName;
                    datasetFlowLayoutPanelItemUserControl._id = row.Field<long>("_id");

                    // Check if this form is for "Training dataset explorer"
                    if (this.Text.Equals("Training dataset explorer"))
                        holdRecordReport_Training_Item(/*dataTable, */row, datasetFlowLayoutPanelItemUserControl);

                    if (IsHandleCreated) this.Invoke(new MethodInvoker(delegate () { signalsFlowLayoutPanel.Controls.Add(datasetFlowLayoutPanelItemUserControl); }));
                }
            }
            // Set features for training
            else if (callingClassName.Contains("DatasetExplorerFormForTraining"))
                holdRecordReport_Training(dataTable, callingClassName);
        }
    }
}

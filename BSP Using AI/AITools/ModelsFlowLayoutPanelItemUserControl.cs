using BSP_Using_AI.AITools.DatasetExplorer;
using BSP_Using_AI.AITools.Details;
using BSP_Using_AI.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;

namespace BSP_Using_AI.AITools
{
    public partial class ModelsFlowLayoutPanelItemUserControl : UserControl, DbStimulatorReportHolder
    {
        public long _id;
        public ObjectiveBaseModel _objectiveModel;

        Dictionary<string, ObjectiveBaseModel> _objectivesModelsDic;

        public ModelsFlowLayoutPanelItemUserControl(ObjectiveBaseModel objectiveBaseModel, Dictionary<string, ObjectiveBaseModel> objectivesModelsDic, Form invokerForm)
        {
            InitializeComponent();

            _objectiveModel = objectiveBaseModel;
            _objectivesModelsDic = objectivesModelsDic;

            /// Look for available ufitted signals
            string tableName = "dataset";
            string selection = null;
            object[] selectionArgs = null;
            if (objectiveBaseModel is CWDReinforcementL || objectiveBaseModel is CWDLSTM)
            {
                tableName = "anno_ds";
                selection = "anno_objective=?";
                selectionArgs = new object[] { CharacteristicWavesDelineation.ObjectiveName };
            }
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query(tableName,
                                        new String[] { "_id" },
                                        selection,
                                        selectionArgs,
                                        "", "ModelsFlowLayoutPanelItemUserControl"));
            dbStimulatorThread.Start();

            Thread waitForModelToLeadThread = new Thread(() => WaitForModelToLoad(invokerForm));
            waitForModelToLeadThread.Start();
        }

        private void WaitForModelToLoad(Form invokerForm)
        {
            invokerForm.Invoke(new MethodInvoker(delegate () {
                fitButton.Enabled = false;
                detailsButton.Enabled = false;
                Cursor = Cursors.WaitCursor;
            }));

            for (int i = 0; i < 240; i++)
                if (!_objectivesModelsDic.ContainsKey(_objectiveModel.ModelName + _objectiveModel.ObjectiveName) || !IsHandleCreated)
                    Thread.Sleep(500);
                else
                    break;
            _objectiveModel = _objectivesModelsDic[_objectiveModel.ModelName + _objectiveModel.ObjectiveName];

            invokerForm.Invoke(new MethodInvoker(delegate () {
                fitButton.Enabled = true;
                detailsButton.Enabled = true;
                Cursor = Cursors.Default;
            }));
        }

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void detailsButton_Click(object sender, EventArgs e)
        {
            // Show details form and insert update details in trainingsDetailsListBox
            DetailsForm detailsForm = new DetailsForm(_objectivesModelsDic, _id, _objectiveModel, ((AIToolsForm)this.FindForm())._tFBackThread);
            detailsForm.Show();
            detailsForm.initializeForm();
        }

        private void fitButton_Click(object sender, EventArgs e)
        {
            // Open DatasetExplorerForm for selecting training dataset
            DatasetExplorerForm datasetExplorerForm = new DatasetExplorerForm("Training dataset explorer");
            datasetExplorerForm._id = _id;
            datasetExplorerForm._objectiveModel = _objectiveModel;
            datasetExplorerForm._datasetSize = long.Parse(datasetSizeLabel.Text);
            datasetExplorerForm._updatesNum = int.Parse(updatesLabel.Text);
            datasetExplorerForm._aIToolsForm = (AIToolsForm)this.FindForm();

            datasetExplorerForm.deleteSelectionButton.Visible = false;
            datasetExplorerForm.unselectAllButton.Visible = false;
            datasetExplorerForm.fitSelectionButton.Visible = true;
            datasetExplorerForm.instrucitonLabel.Visible = true;
            datasetExplorerForm.Show();

            if (_objectiveModel is ARTHTModels)
                datasetExplorerForm.queryForSignals_ARTHT();
            else if (_objectiveModel is CWDReinforcementL || _objectiveModel is CWDLSTM)
                datasetExplorerForm.queryForSignals_CWD();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show message for confirming the action
            DialogResult dialogResult = MessageBox.Show("Are you sure about deleting the model \"" + this.Name + "\"?", "Action confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                // Remove model from _targetsModelsHashtable
                ((AIToolsForm)this.FindForm())._objectivesModelsDic.Remove(this.Name);

                // Remove it from models table
                DbStimulator dbStimulator = new DbStimulator();
                dbStimulator.Delete("models",
                                            "_id=?",
                                            new Object[] { _id },
                                            "ModelsFlowLayoutPanelItemUserControl");

                if (_objectiveModel is ARTHTModels arthtModels)
                    // Check if this is Neural Network model
                    foreach (CustomArchiBaseModel model in arthtModels.ARTHTModelsDic.Values)
                        if (model is KerasNETNeuralNetworkModel)
                        {
                            // Get the folder of the collected steps models
                            string modelsPath = (model as KerasNETNeuralNetworkModel).ModelPath.Substring(0, (model as KerasNETNeuralNetworkModel).ModelPath.LastIndexOf("/"));
                            // Remove the folder
                            if (Directory.Exists(modelsPath))
                                Directory.Delete(modelsPath, true);
                        }

                // Refresh modelsFlowLayoutPanel
                ((AIToolsForm)this.FindForm()).queryForModels();
            }
        }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //:::::::::::::::::::::::::::CROSS PROCESS FORM FUNCTIONS (INTERFACES)::::::::::::::::::::::://
        public void holdRecordReport(DataTable dataTable, string callingClassName)
        {
            if (!callingClassName.Equals("ModelsFlowLayoutPanelItemUserControl"))
                return;

            if (dataTable.Rows.Count > 0)
            {
                // Update unfittedDataLabel
                while (true)
                {
                    try
                    {
                        this.Invoke(new MethodInvoker(delegate () { unfittedDataLabel.Text = (dataTable.Rows.Count - int.Parse(datasetSizeLabel.Text)).ToString(); }));
                        break;
                    }
                    catch (Exception e)
                    {
                        Thread.Sleep(200);
                    }
                }
            }
        }
    }
}

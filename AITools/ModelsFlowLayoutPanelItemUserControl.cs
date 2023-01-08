using BSP_Using_AI.AITools.DatasetExplorer;
using BSP_Using_AI.AITools.Details;
using BSP_Using_AI.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BSP_Using_AI.AITools
{
    public partial class ModelsFlowLayoutPanelItemUserControl : UserControl, DbStimulatorReportHolder
    {
        public long _id;
        public string _modelPath;
        public List<List<long[]>> _trainingDetails;

        public ModelsFlowLayoutPanelItemUserControl()
        {
            InitializeComponent();

            /// Look for available ufitted signals
            // Query for all signals in dataset table
            // and remove datasetSizeLabel from it
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("dataset",
                                        new String[] { "_id" },
                                        null,
                                        null,
                                        "", "ModelsFlowLayoutPanelItemUserControl"));
            dbStimulatorThread.Start();
        }

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void detailsButton_Click(object sender, EventArgs e)
        {
            // Show details form and insert update details in trainingsDetailsListBox
            DetailsForm detailsForm = new DetailsForm(_id, this.Name, _trainingDetails, ((AIToolsForm)this.FindForm())._tFBackThread);
            detailsForm.Show();
        }

        private void fitButton_Click(object sender, EventArgs e)
        {
            // Open DatasetExplorerForm for selecting training dataset
            DatasetExplorerForm datasetExplorerForm = new DatasetExplorerForm();
            datasetExplorerForm._id = _id;
            datasetExplorerForm._modelPath = _modelPath;
            datasetExplorerForm._datasetSize = long.Parse(datasetSizeLabel.Text);
            datasetExplorerForm._updatesNum = int.Parse(updatesLabel.Text);
            datasetExplorerForm._trainingDetails = _trainingDetails;
            datasetExplorerForm._modelName = this.Name;
            datasetExplorerForm._aIToolsForm = (AIToolsForm)this.FindForm();

            datasetExplorerForm.Text = "Training dataset explorer";
            datasetExplorerForm.deleteSelectionButton.Visible = false;
            datasetExplorerForm.unselectAllButton.Visible = false;
            datasetExplorerForm.fitSelectionButton.Visible = true;
            datasetExplorerForm.instrucitonLabel.Visible = true;
            datasetExplorerForm.Show();

            datasetExplorerForm.queryForSignals();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show message for confirming the action
            DialogResult dialogResult = MessageBox.Show("Are you sure about deleting the model \"" + this.Name + "\"?", "Action confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                // Remove model from _targetsModelsHashtable
                ((AIToolsForm)this.FindForm())._targetsModelsHashtable.Remove(this.Name);

                // Remove it from models table
                DbStimulator dbStimulator = new DbStimulator();
                dbStimulator.Delete("models",
                                            "_id=?",
                                            new Object[] { _id },
                                            "ModelsFlowLayoutPanelItemUserControl");

                // Remove it from the folder
                if (Directory.Exists(_modelPath))
                {
                    Directory.Delete(_modelPath, true);
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

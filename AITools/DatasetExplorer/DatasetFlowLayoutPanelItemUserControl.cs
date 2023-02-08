using BSP_Using_AI.Database;
using BSP_Using_AI.DetailsModify;
using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace BSP_Using_AI.AITools.DatasetExplorer
{
    public partial class DatasetFlowLayoutPanelItemUserControl : UserControl, DbStimulatorReportHolder
    {
        public long _id;

        public DatasetFlowLayoutPanelItemUserControl()
        {
            InitializeComponent();
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        private void showSignalDetails(FilteringTools filteringTools, ARTHTFeatures artHTFeatures)
        {
            // Open a new form
            FormDetailsModify formDetailsModify = new FormDetailsModify(filteringTools, signalNameLabel.Text + "\\Features details");
            formDetailsModify._arthtFeatures = artHTFeatures;

            formDetailsModify.initializeAITools();
            formDetailsModify.finish();

            formDetailsModify.signalFusionButton.Enabled = false;
            formDetailsModify.signalsPickerComboBox.Enabled = false;
            formDetailsModify.applyButton.Enabled = false;
            formDetailsModify.samplingRateTextBox.Enabled = false;
            formDetailsModify.filtersComboBox.Enabled = false;
            formDetailsModify.discardButton.Enabled = false;
            formDetailsModify.setFeaturesLabelsButton.Enabled = false;
            formDetailsModify.aiGoalComboBox.Enabled = false;
            formDetailsModify.modelTypeComboBox.Enabled = false;
            formDetailsModify.predictButton.Enabled = false;

            formDetailsModify.signalsPickerComboBox.SelectedIndex = 1;

            formDetailsModify.Text = "Features details";
            formDetailsModify.Show();
        }

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void featuresDetailsButton_Click(object sender, EventArgs e)
        {
            // Query for the signal and its features
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("dataset",
                                new String[] { "starting_index", "signal", "sampling_rate", "quantisation_step", "features" },
                                "_id=?",
                                new Object[] { _id },
                                "", "DatasetFlowLayoutPanelItemUserControl"));
            dbStimulatorThread.Start();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if parent form is not "Dataset explorer"
            if (!this.FindForm().Text.Equals("Dataset explorer"))
            {
                // If yes then show a message that signal cannot be deleted from here and return
                MessageBox.Show("Cannot delete signal from \"Training dataset explorer\"", "Access not allowed", MessageBoxButtons.OK);
                return;
            }

            // Show message for confirming the action
            DialogResult dialogResult = MessageBox.Show("Are you sure about deleting the signal \"" + this.signalNameLabel.Text + "(" + this.startingIndexLabel.Text + ")" + "\"?", "Action confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                // Remove it from models table
                DbStimulator dbStimulator = new DbStimulator();
                dbStimulator.Delete("dataset",
                                            "_id=?",
                                            new Object[] { _id },
                                            "DatasetFlowLayoutPanelItemUserControl");

                // Reset badge number of MainForm
                ((DatasetExplorerForm)this.FindForm())._mainForm.resetBadge();

                // Refresh modelsFlowLayoutPanel
                ((DatasetExplorerForm)this.FindForm()).queryForSignals();
            }
        }

        private void selectionCheckBox_Click(object sender, EventArgs e)
        {
            if (this.FindForm().Text.Equals("Dataset explorer"))
                return;
            // Get current item's index in signalsFlowLayoutPanel
            int currentItemIndx = ((FlowLayoutPanel)this.Parent).Controls.GetChildIndex(this);
            // Check if "Shift" button is clicked and a previous selected item
            if (((DatasetExplorerForm)this.FindForm())._shiftClicked && ((DatasetExplorerForm)this.FindForm())._lastSelectedItem_shift != -1)
            {
                // If yes then alter the selection status of each item in the shifted interval
                int start = ((DatasetExplorerForm)this.FindForm())._lastSelectedItem_shift + 1;
                int end = currentItemIndx;
                if (start > end)
                {
                    start = currentItemIndx + 1;
                    end = ((DatasetExplorerForm)this.FindForm())._lastSelectedItem_shift;
                }
                for (int i = start; i < end; i++)
                    if (((FlowLayoutPanel)this.Parent).Controls[i].Enabled)
                        ((DatasetFlowLayoutPanelItemUserControl)((FlowLayoutPanel)this.Parent).Controls[i]).selectionCheckBox.Checked = !((DatasetFlowLayoutPanelItemUserControl)((FlowLayoutPanel)this.Parent).Controls[i]).selectionCheckBox.Checked;
            }

            // Update _lastSelectedItem_shift
            ((DatasetExplorerForm)this.FindForm())._lastSelectedItem_shift = currentItemIndx;
        }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //:::::::::::::::::::::::::::CROSS PROCESS FORM FUNCTIONS (INTERFACES)::::::::::::::::::::::://
        public void holdRecordReport(DataTable dataTable, string callingClassName)
        {
            if (!callingClassName.Equals("DatasetFlowLayoutPanelItemUserControl"))
                return;

            // Get signal and features
            foreach (DataRow row in dataTable.Rows)
            {
                double[] signal = (double[])Garage.ByteArrayToObject(row.Field<byte[]>("signal"));
                ARTHTFeatures artHTFeatures = (ARTHTFeatures)Garage.ByteArrayToObject(row.Field<byte[]>("features"));

                FilteringTools filteringTools = new FilteringTools((int)row.Field<long>("sampling_rate"), row.Field<long>("quantisation_step"), null);
                filteringTools.SetStartingInSecond(row.Field<long>("starting_index"));
                filteringTools.SetOriginalSamples(signal);
                // Show signal with features
                this.Invoke(new MethodInvoker(delegate () { showSignalDetails(filteringTools, artHTFeatures); }));
            }
        }
    }
}

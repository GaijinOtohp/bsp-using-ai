using BSP_Using_AI.Database;
using BSP_Using_AI.DetailsModify;
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
    public partial class DatasetFlowLayoutPanelItemUserControl : UserControl, DbStimulatorReportHolder
    {
        public long _id;

        public DatasetFlowLayoutPanelItemUserControl()
        {
            InitializeComponent();
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        private void showSignalDetails(double[] signal, OrderedDictionary featuresOrderedDictionary)
        {
            // Open a new form
            FormDetailsModify formDetailsModify = new FormDetailsModify(signal, double.Parse(samplingRateLabel.Text), signalNameLabel.Text + "\\Features details", double.Parse(startingIndexLabel.Text));
            formDetailsModify._featuresOrderedDictionary = featuresOrderedDictionary;

            formDetailsModify.initializeAITools();
            formDetailsModify.finish();

            formDetailsModify.signalFusionButton.Enabled = false;
            formDetailsModify.signalsPickerComboBox.Enabled = false;
            formDetailsModify.applyButton.Enabled = false;
            formDetailsModify.samplingRateTextBox.Enabled = false;
            formDetailsModify.filtersComboBox.Enabled = false;
            formDetailsModify.fftButton.Enabled = false;
            formDetailsModify.dwtButton.Enabled = false;
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
            dbStimulator.initialize("dataset",
                                new String[] { "signal", "features" },
                                "_id=?",
                                new Object[] { _id },
                                "", "DatasetFlowLayoutPanelItemUserControl");
            Thread dbStimulatorThread = new Thread(new ThreadStart(dbStimulator.run));
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
            DialogResult dialogResult = MessageBox.Show("Are you sure about deleting the signal \"" + this.signalNameLabel.Text + "(" + this.startingIndexLabel.Text     + ")" + "\"?", "Action confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                // Remove it from models table
                DbStimulator dbStimulator = new DbStimulator();
                dbStimulator.initialize("dataset",
                                            "_id=?",
                                            new Object[] { _id },
                                            "DatasetFlowLayoutPanelItemUserControl");
                dbStimulator.run();

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
        public void holdRecordReport(List<object[]> records, string callingClassName)
        {
            if (!callingClassName.Equals("DatasetFlowLayoutPanelItemUserControl"))
                return;

            // Get signal and features
            double[] signal = (double[])Garage.ByteArrayToObject((byte[])records[0][0]);
            OrderedDictionary featuresOrderedDictionary = (OrderedDictionary)Garage.ByteArrayToObject((byte[])records[0][1]);

            // Show signal with features
            this.Invoke(new MethodInvoker(delegate () { showSignalDetails(signal, featuresOrderedDictionary); }));
        }
    }
}

using BSP_Using_AI.AITools;
using BSP_Using_AI.AITools.DatasetExplorer;
using BSP_Using_AI.Database;
using Keras;
using Keras.Layers;
using Keras.Models;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BSP_Using_AI
{
    public partial class AIToolsForm : Form, DbStimulatorReportHolder, AIBackThreadReportHolder
    {
        public MainForm _mainForm;

        public Hashtable _targetsModelsHashtable = null;

        public TFBackThread _tFBackThread;

        public AIToolsForm()
        {
            InitializeComponent();

            /// Look for available models
            queryForModels();
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public void queryForModels()
        {
            // Query for all models in models table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("models",
                                new String[] { "_id", "type_name", "model_target", "model_path", "dataset_size", "model_updates", "trainings_details" },
                                null,
                                null,
                                "", "AIToolsForm"));
            dbStimulatorThread.Start();
        }

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void createNewModelButton_Click(object sender, EventArgs e)
        {
            // Check if there is a missing combo box not chosed from
            // show a message if not
            String message = "You must chose the following:\n";
            bool showMessage = false;

            if (modelTypeComboBox.SelectedIndex < 0) { message += ". AI model type\n"; showMessage = true; }

            if (aiGoalComboBox.SelectedIndex < 0) { message += ". AI model target\n"; showMessage = true; }

            if (showMessage) { MessageBox.Show(message, "Fields missing", MessageBoxButtons.OK); return; }

            // Set selected_variables and thresholds for the 7 steps
            List<double[]>[] pcLoadingScores = new List<double[]>[] { new List<double[]>(), new List<double[]>(), new List<double[]>(),
                                                                  new List<double[]>(), new List<double[]>(), new List<double[]>(), new List<double[]>() };
            List<float[]> outputsThresholds = new List<float[]> { null, new float[] { 0.5f }, null, new float[] { 0.5f, 0.5f }, new float[] { 0.5f }, null, new float[] { 0.5f } };
            // Check which model for which terget is selected
            if (modelTypeComboBox.SelectedIndex == 0 && aiGoalComboBox.SelectedIndex == 0)
            {
                // If yes then this is neural network for WPW syndrome detection
                _tFBackThread._queue.Enqueue(new object[] { "createNeuralNetworkModelForWPW", "AIToolsForm", this, pcLoadingScores, outputsThresholds });
                _tFBackThread._signal.Set();
            } else if (modelTypeComboBox.SelectedIndex == 1 && aiGoalComboBox.SelectedIndex == 0)
            {
                // If yes then this is for KNN models
                KNNBackThread kNNBackThread = new KNNBackThread(_targetsModelsHashtable, this);
                Thread knnThread = new Thread(() => kNNBackThread.createKNNkModelForWPW(pcLoadingScores, outputsThresholds));
                knnThread.Start();
            }
            else if (modelTypeComboBox.SelectedIndex == 2 && aiGoalComboBox.SelectedIndex == 0)
            {
                // If yes then this is for KNN models
                NaiveBayesBackThread naiveBayesBackThread = new NaiveBayesBackThread(_targetsModelsHashtable, this);
                Thread nbThread = new Thread(() => naiveBayesBackThread.createNBModelForWPW(pcLoadingScores, outputsThresholds));
                nbThread.Start();
            }
        }

        private void DatasetExplorerButton_Click(object sender, EventArgs e)
        {
            // If yes then open the signals form
            // Check if the form is already opened, and wlose it if so
            if (Application.OpenForms.OfType<DatasetExplorerForm>().Count() > 0)
                try
                {
                    foreach (DatasetExplorerForm form in Application.OpenForms.OfType<DatasetExplorerForm>())
                        if (form.Text.Equals("Dataset explorer"))
                            form.Close();
                }
                catch (Exception ec)
                {

                }

            // Open a new form
            DatasetExplorerForm datasetExplorerForm = new DatasetExplorerForm();
            datasetExplorerForm._mainForm = _mainForm;

            datasetExplorerForm.Text = "Dataset explorer";
            datasetExplorerForm.Show();

            datasetExplorerForm.queryForSignals();
        }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //:::::::::::::::::::::::::::CROSS PROCESS FORM FUNCTIONS (INTERFACES)::::::::::::::::::::::://
        public void holdRecordReport(DataTable dataTable, string callingClassName)
        {
            if (!callingClassName.Equals("AIToolsForm"))
                return;

            // Clear modelsFlowLayoutPanel
            if (modelsFlowLayoutPanel.Controls.Count > 0)
                this.Invoke(new MethodInvoker(delegate () { modelsFlowLayoutPanel.Controls.Clear(); }));

            // Insert new items from records
            foreach (DataRow row in dataTable.AsEnumerable())
            {
                // Create an item of the model
                ModelsFlowLayoutPanelItemUserControl modelsFlowLayoutPanelItemUserControl = new ModelsFlowLayoutPanelItemUserControl();
                int modelIndx = 0;
                while (modelsFlowLayoutPanel.Controls.ContainsKey(row.Field<string>("type_name") + " for " + row.Field<string>("model_target") + modelIndx))
                    modelIndx++;
                modelsFlowLayoutPanelItemUserControl.Name = row.Field<string>("type_name") + " for " + row.Field<string>("model_target") + modelIndx;
                modelsFlowLayoutPanelItemUserControl.modelNameLabel.Text = modelsFlowLayoutPanelItemUserControl.Name;
                modelsFlowLayoutPanelItemUserControl.datasetSizeLabel.Text = row.Field<long>("dataset_size").ToString();
                modelsFlowLayoutPanelItemUserControl.updatesLabel.Text = row.Field<long>("model_updates").ToString();
                modelsFlowLayoutPanelItemUserControl.unfittedDataLabel.Text = "0";
                modelsFlowLayoutPanelItemUserControl._id = row.Field<long>("_id");
                modelsFlowLayoutPanelItemUserControl._modelPath = row.Field<string>("model_path");
                modelsFlowLayoutPanelItemUserControl._trainingDetails = (List<List<long[]>>)Garage.ByteArrayToObject(row.Field<byte[]>("trainings_details"));

                this.Invoke(new MethodInvoker(delegate () { modelsFlowLayoutPanel.Controls.Add(modelsFlowLayoutPanelItemUserControl); }));
            }
        }

        public void holdAIReport(object[] paramms, string callingClassName)
        {
            if (!callingClassName.Equals("AIToolsForm"))
                return;

            if (paramms[0].Equals("createModel") && this.Controls.Find(modelsFlowLayoutPanel.Name, false).Length > 0)
                // Refresh modelsFlowLayoutPanel
                this.Invoke(new MethodInvoker(delegate () { queryForModels(); }));
            // Check if this is fitting progress report
            else if (paramms[0].Equals("progress") && this.Controls.Find(modelsFlowLayoutPanel.Name, false).Length > 0)
            {
                // If yes then refresh progress bar of the selected model
                this.Invoke(new MethodInvoker(delegate () { ((ModelsFlowLayoutPanelItemUserControl)modelsFlowLayoutPanel.Controls[(string)paramms[1]]).fitProgressBar.Maximum = (int)paramms[3]; }));
                this.Invoke(new MethodInvoker(delegate () { ((ModelsFlowLayoutPanelItemUserControl)modelsFlowLayoutPanel.Controls[(string)paramms[1]]).fitProgressBar.Value = (int)paramms[2]; }));
            }
            else if (paramms[0].Equals("fitting_complete") && (long)paramms[2] == -1)
            {
                // If yes then this is from PCA analysis, then just set the progress bar to its maximum
                this.Invoke(new MethodInvoker(delegate () { ((ModelsFlowLayoutPanelItemUserControl)modelsFlowLayoutPanel.Controls[(string)paramms[1]]).fitProgressBar.Maximum = 1; }));
                this.Invoke(new MethodInvoker(delegate () { ((ModelsFlowLayoutPanelItemUserControl)modelsFlowLayoutPanel.Controls[(string)paramms[1]]).fitProgressBar.Value = 1; }));
            }
            else if (paramms[0].Equals("fitting_complete") && this.Controls.Find(modelsFlowLayoutPanel.Name, false).Length > 0)
            {
                // Refresh model details in modelsFlowLayoutPanel
                int unfittedDatasetSize = int.Parse(((ModelsFlowLayoutPanelItemUserControl)modelsFlowLayoutPanel.Controls[(string)paramms[1]]).unfittedDataLabel.Text);
                int fittedDatasetSize = int.Parse(((ModelsFlowLayoutPanelItemUserControl)modelsFlowLayoutPanel.Controls[(string)paramms[1]]).datasetSizeLabel.Text);
                int pudates = int.Parse(((ModelsFlowLayoutPanelItemUserControl)modelsFlowLayoutPanel.Controls[(string)paramms[1]]).updatesLabel.Text);
                pudates++;

                this.Invoke(new MethodInvoker(delegate () { ((ModelsFlowLayoutPanelItemUserControl)modelsFlowLayoutPanel.Controls[(string)paramms[1]]).datasetSizeLabel.Text = ((long)paramms[2]).ToString(); }));
                this.Invoke(new MethodInvoker(delegate () { ((ModelsFlowLayoutPanelItemUserControl)modelsFlowLayoutPanel.Controls[(string)paramms[1]]).updatesLabel.Text = pudates.ToString(); }));
                this.Invoke(new MethodInvoker(delegate () { ((ModelsFlowLayoutPanelItemUserControl)modelsFlowLayoutPanel.Controls[(string)paramms[1]]).unfittedDataLabel.Text = (unfittedDatasetSize - ((long)paramms[2] - fittedDatasetSize)).ToString(); }));

                // Check if current dataset is the largest
                if ((long)paramms[2] > _mainForm._largestDatasetSize)
                {
                    // If yes then set the new largest dataset number
                    _mainForm._largestDatasetSize = (long)paramms[2];
                    // Reset unfitted data number
                    this.Invoke(new MethodInvoker(delegate () { _mainForm.resetBadge(); }));
                }
            }
        }

        private void modelsFlowLayoutPanel_SizeChanged(object sender, EventArgs e)
        {
            EventHandlers.signalFlowLayout_SizeChanged(sender, 1074);
        }
    }
}

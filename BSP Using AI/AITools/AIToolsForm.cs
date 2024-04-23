using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.AITools.Keras_NET_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.KNN_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.NaiveBayes_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.RL_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.AITools;
using BSP_Using_AI.AITools.DatasetExplorer;
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

namespace BSP_Using_AI
{
    public partial class AIToolsForm : Form, DbStimulatorReportHolder, AIBackThreadReportHolder
    {
        public MainForm _mainForm;

        public Dictionary<string, ObjectiveBaseModel> _objectivesModelsDic = null;

        public ARTHT_Keras_NET_NN _tFBackThread;

        public AIToolsForm()
        {
            InitializeComponent();

            // Include the available AI models types in modelTypeComboBox
            string[] modelsTypes = typeof(CustomArchiBaseModel).Assembly.GetExportedTypes().Where(type => type.IsSubclassOf(typeof(CustomArchiBaseModel)) && type.GetField("ModelName") != null).
                                                                                       Select(type => (string)type.GetField("ModelName").GetValue(null)).ToArray();
            modelTypeComboBox.DataSource = modelsTypes;
            // Include the available objectives in aiGoalComboBox
            string[] objectives = typeof(AIModels_ObjectivesArchitectures).GetNestedTypes().Where(type => type.GetField("ObjectiveName") != null).
                                                                                            Select(type => (string)type.GetField("ObjectiveName").GetValue(null)).ToArray();
            aiGoalComboBox.DataSource = objectives;

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
                                new String[] { "_id", "the_model", "dataset_size" },
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

            // Check which objective is selected
            if (aiGoalComboBox.SelectedItem.Equals(WPWSyndromeDetection.ObjectiveName))
            {
                // If yes then this is for WPW syndrome detection
                // Check which model is selected
                if (modelTypeComboBox.SelectedItem.Equals(KerasNETNeuralNetworkModel.ModelName))
                {
                    // If yes then this is neural network for WPW syndrome detection
                    _tFBackThread._queue.Enqueue(new QueueSignalInfo() { TargetFunc = "createNeuralNetworkModelForWPW", CallingClass = "AIToolsForm", _aIToolsForm = this });
                    _tFBackThread._signal.Set();
                }
                else if (modelTypeComboBox.SelectedItem.Equals(KNNModel.ModelName))
                {
                    // If yes then this is for KNN models
                    ARTHT_KNN kNNBackThread = new ARTHT_KNN(_objectivesModelsDic, this);
                    Thread knnThread = new Thread(() => kNNBackThread.createKNNkModelForWPW());
                    knnThread.Start();
                }
                else if (modelTypeComboBox.SelectedItem.Equals(NaiveBayesModel.ModelName))
                {
                    // If yes then this is for Naive Bayes models
                    ARTHT_NaiveBayes naiveBayesBackThread = new ARTHT_NaiveBayes(_objectivesModelsDic, this);
                    Thread nbThread = new Thread(() => naiveBayesBackThread.createNBModelForWPW());
                    nbThread.Start();
                }
                else if (modelTypeComboBox.SelectedItem.Equals(TFNETNeuralNetworkModel.ModelName))
                {
                    // If yes then this is for Tensorflow.Net Neural Networks models
                    ARTHT_TF_NET_NN tf_NET_NN = new ARTHT_TF_NET_NN(_objectivesModelsDic, this);
                    Thread tfNetThread = new Thread(() => tf_NET_NN.createTFNETNeuralNetworkModelForWPW());
                    tfNetThread.Start();
                }
                else if (modelTypeComboBox.SelectedItem.Equals(TFKerasNeuralNetworkModel.ModelName))
                {
                    // If yes then this is for Tensorflow.Keras Neural Networks models
                    TF_KERAS_NN tf_Keras_NN = new TF_KERAS_NN(_objectivesModelsDic, this);
                    Thread tfKerasThread = new Thread(() => tf_Keras_NN.createTFKerasNeuralNetworkModelForWPW());
                    tfKerasThread.Start();
                }
            }
            else if (aiGoalComboBox.SelectedItem.Equals(CharacteristicWavesDelineation.ObjectiveName))
            {
                // This is for characteristic waves dlineation
                // Check which model is selected
                if (modelTypeComboBox.SelectedItem.Equals(TFNETReinforcementL.ModelName))
                {
                    // If yes then this is for Reinforcement learning models
                    CWD_RL_TFNET cwdRLTFNET = new CWD_RL_TFNET(_objectivesModelsDic, this);
                    Thread cwdRLTFNETThread = new Thread(() => cwdRLTFNET.createTFNETRLModelForCWD());
                    cwdRLTFNETThread.Start();
                }
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
            DatasetExplorerForm datasetExplorerForm = new DatasetExplorerForm("Dataset explorer");
            datasetExplorerForm._mainForm = _mainForm;
            datasetExplorerForm.Show();

            datasetExplorerForm.queryForSignals_ARTHT();
        }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //:::::::::::::::::::::::::::CROSS PROCESS FORM FUNCTIONS (INTERFACES)::::::::::::::::::::::://
        public void holdRecordReport(DataTable dataTable, string callingClassName)
        {
            if (!callingClassName.Equals("AIToolsForm"))
                return;

            // Clear modelsFlowLayoutPanel
            if (modelsFlowLayoutPanel.Controls.Count > 0)
                if (IsHandleCreated) this.Invoke(new MethodInvoker(delegate () { modelsFlowLayoutPanel.Controls.Clear(); }));

            // Order datatable by name
            List<DataRow> rowsList = new List<DataRow>(dataTable.AsEnumerable());
            List<string> namesList = new List<string>();
            foreach (DataRow row in rowsList)
            {
                ObjectiveBaseModel objectiveBaseModel = GeneralTools.ByteArrayToObject<ObjectiveBaseModel>(row.Field<byte[]>("the_model"));
                namesList.Add(objectiveBaseModel.ModelName + objectiveBaseModel.ObjectiveName);
            }
            rowsList = GeneralTools.OrderByTextWithNumbers(rowsList, namesList);

            // Insert new items from records
            foreach (DataRow row in rowsList)
            {
                // Create an item of the model
                ModelsFlowLayoutPanelItemUserControl modelsFlowLayoutPanelItemUserControl = new ModelsFlowLayoutPanelItemUserControl();

                ObjectiveBaseModel objectiveBaseModel = GeneralTools.ByteArrayToObject<ObjectiveBaseModel>(row.Field<byte[]>("the_model"));

                modelsFlowLayoutPanelItemUserControl.Name = objectiveBaseModel.ModelName + objectiveBaseModel.ObjectiveName;
                modelsFlowLayoutPanelItemUserControl.modelNameLabel.Text = objectiveBaseModel.ModelName + objectiveBaseModel.ObjectiveName;
                modelsFlowLayoutPanelItemUserControl.datasetSizeLabel.Text = row.Field<long>("dataset_size").ToString();
                modelsFlowLayoutPanelItemUserControl.updatesLabel.Text = objectiveBaseModel.DataIdsIntervalsList.Count().ToString();
                modelsFlowLayoutPanelItemUserControl.unfittedDataLabel.Text = "0";
                modelsFlowLayoutPanelItemUserControl._id = row.Field<long>("_id");
                for (int i = 0; i < 240; i++)
                    if (!_objectivesModelsDic.ContainsKey(objectiveBaseModel.ModelName + objectiveBaseModel.ObjectiveName))
                        Thread.Sleep(500);
                    else
                        break;
                modelsFlowLayoutPanelItemUserControl._objectiveModel = _objectivesModelsDic[objectiveBaseModel.ModelName + objectiveBaseModel.ObjectiveName];

                if (IsHandleCreated) this.Invoke(new MethodInvoker(delegate () { modelsFlowLayoutPanel.Controls.Add(modelsFlowLayoutPanelItemUserControl); }));
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

using Biological_Signal_Processing_Using_AI;
using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.AITools.Keras_NET_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.KNN_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.NaiveBayes_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.RL_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.TF_NET_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.Database;
using BSP_Using_AI.SignalHolderFolder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;

namespace BSP_Using_AI
{
    public partial class MainForm : Form, DbStimulatorReportHolder
    {
        public long _largestDatasetSize = 0;

        public Dictionary<string, ObjectiveBaseModel> _objectivesModelsDic = null;

        public bool scrollAllowed = true;

        public ARTHT_Keras_NET_NN _tFBackThread;

        public MainForm()
        {
            InitializeComponent();

            // Set models reader to ready
            _objectivesModelsDic = new Dictionary<string, ObjectiveBaseModel>();

            // Set the notification badge over "aiToolsButton" ready
            MainFormFolder.BadgeControl.AddBadgeTo(aiToolsButton, "0");

            _tFBackThread = new ARTHT_Keras_NET_NN();
            _tFBackThread._objectivesModelsDic = _objectivesModelsDic;
            Thread TFThread = new Thread(() => _tFBackThread.ThreadServer());
            TFThread.IsBackground = true;
            TFThread.Start();

            /// Look for unfitted signals
            // Query for last signal id from the most trained model
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("models",
                                new String[] { "type_name", "model_target", "the_model", "dataset_size" },
                                null,
                                null,
                                //" ORDER BY dataset_size DESC LIMIT 1", "MainFormForModels");
                                "", "MainFormForModels"));
            dbStimulatorThread.Start();

            SignalHolder SignalHolder = new SignalHolder();

            signalsFlowLayout.Controls.Add(SignalHolder);

            if (SignalHolder.Height * signalsFlowLayout.Controls.Count > signalsFlowLayout.Height)
            {
                vScrollBar.LargeChange = signalsFlowLayout.Height;
                vScrollBar.Maximum = SignalHolder.Height * signalsFlowLayout.Controls.Count;

                signalsFlowLayout.VerticalScroll.LargeChange = signalsFlowLayout.Height;
                signalsFlowLayout.VerticalScroll.Maximum = SignalHolder.Height * signalsFlowLayout.Controls.Count;
            }
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public void resetBadge()
        {
            // Query for last signal id from the most trained model
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("dataset",
                                new String[] { "_id" },
                                null,
                                null,
                                "", "MainFormForDataset"));
            dbStimulatorThread.Start();
        }


        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void signalsFlowLayout_SizeChanged(object sender, EventArgs e)
        {
            EventHandlers.signalFlowLayout_SizeChanged(sender, 900);
        }

        private void ChoseRecapButton_Click(object sender, EventArgs e)
        {
            EventHandlers.learnedSignalsButton_Click(sender, e);
        }

        private void signalsComparatorButton_Click(object sender, EventArgs e)
        {
            EventHandlers.signalsComparatorButton_Click();
        }

        private void signalsCollectorButton_Click(object sender, EventArgs e)
        {
            EventHandlers.signalsCollectorButton_Click();
        }

        public void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            signalsFlowLayout.VerticalScroll.Value = vScrollBar.Value;
        }

        private void signalExhibitor_MouseWheelScroll(object sender, MouseEventArgs e)
        {
            // Check if scroll is allowed (mouse is not in a signal exhibitor)
            if (scrollAllowed)
            {
                int diff;
                if (e.Delta < 0)
                {
                    diff = vScrollBar.Value + vScrollBar.SmallChange;
                    if (diff < vScrollBar.Maximum - vScrollBar.LargeChange + 1)
                        vScrollBar.Value = diff;
                    else
                        vScrollBar.Value = vScrollBar.Maximum - vScrollBar.LargeChange + 1;
                }
                else
                {
                    diff = vScrollBar.Value - vScrollBar.SmallChange;
                    if (diff > 0)
                        vScrollBar.Value = diff;
                    else
                        vScrollBar.Value = 1;
                }

                vScrollBar1_Scroll(null, null);
            }
        }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //:::::::::::::::::::::::::::CROSS PROCESS FORM FUNCTIONS (INTERFACES)::::::::::::::::::::::://
        public void holdRecordReport(DataTable dataTable, string callingClassName)
        {
            if (!callingClassName.Equals("MainFormForModels") && !callingClassName.Equals("MainFormForDataset"))
                return;

            if (dataTable.Rows.Count > 0)
            {
                // Check if this report is from model table
                if (callingClassName.Equals("MainFormForModels"))
                {
                    // Order datatable by name
                    List<DataRow> rowsList = new List<DataRow>(dataTable.AsEnumerable());
                    List<string> namesList = new List<string>();
                    foreach (DataRow row in rowsList)
                    {
                        ObjectiveBaseModel objectiveBaseModel = GeneralTools.ByteArrayToObject<ObjectiveBaseModel>(row.Field<byte[]>("the_model"));
                        namesList.Add(objectiveBaseModel.ModelName + objectiveBaseModel.ObjectiveName);
                    }
                    rowsList = GeneralTools.OrderByTextWithNumbers(rowsList, namesList);

                    // Set models ready and get last_signal_id of the highest model size
                    foreach (DataRow row in rowsList)
                    {
                        if (row.Field<long>("dataset_size") > _largestDatasetSize)
                            _largestDatasetSize = row.Field<long>("dataset_size");
                        // Check which objective is selected
                        if (row.Field<string>("model_target").Equals(WPWSyndromeDetection.ObjectiveName))
                        {
                            // If yes then this is for WPW syndrome detection
                            ARTHTModels arthtModels = GeneralTools.ByteArrayToObject<ARTHTModels>(row.Field<byte[]>("the_model"));
                            // Check which model is selected
                            if (row.Field<string>("type_name").Equals(KerasNETNeuralNetworkModel.ModelName))
                            {
                                _tFBackThread._queue.Enqueue(new QueueSignalInfo()
                                {
                                    TargetFunc = "initializeNeuralNetworkModelsForWPW",
                                    CallingClass = "MainForm",
                                    aRTHTModels = GeneralTools.ByteArrayToObject<ARTHTModels>(row.Field<byte[]>("the_model"))
                                });
                                _tFBackThread._signal.Set();
                            }
                            else if (row.Field<string>("type_name").Equals(KNNModel.ModelName))
                            {
                                // Create models for KNN
                                ARTHT_KNN kNNBackThread = new ARTHT_KNN(_objectivesModelsDic, null);
                                Thread knnThread = new Thread(() => kNNBackThread.initializeNeuralNetworkModelsForWPW(arthtModels));
                                knnThread.Start();
                            }
                            else if (row.Field<string>("type_name").Equals(NaiveBayesModel.ModelName))
                            {
                                // Create models for naive bayes
                                ARTHT_NaiveBayes naiveBayesBackThread = new ARTHT_NaiveBayes(_objectivesModelsDic, null);
                                Thread nbThread = new Thread(() => naiveBayesBackThread.initializeNeuralNetworkModelsForWPW(arthtModels));
                                nbThread.Start();
                            }
                            else if (row.Field<string>("type_name").Equals(TFNETNeuralNetworkModel.ModelName))
                            {
                                // Create models for Tensorflow.Net Neural Networks models
                                ARTHT_TF_NET_NN tf_NET_NN = new ARTHT_TF_NET_NN(_objectivesModelsDic, null);
                                Thread tfNetThread = new Thread(() => tf_NET_NN.initializeTFNETNeuralNetworkModelsForWPW(arthtModels));
                                tfNetThread.Start();
                            }
                            else if (row.Field<string>("type_name").Equals(TFKerasNeuralNetworkModel.ModelName))
                            {
                                // Create models for Tensorflow.Keras Neural Networks models
                                TF_KERAS_NN tf_Keras_NN = new TF_KERAS_NN(_objectivesModelsDic, null);
                                Thread tfKerasThread = new Thread(() => tf_Keras_NN.initializeTFKerasNeuralNetworkModelsForWPW(arthtModels));
                                tfKerasThread.Start();
                            }
                        }
                        else if (row.Field<string>("model_target").Equals(CharacteristicWavesDelineation.ObjectiveName))
                        {
                            // This is for characteristic waves dlineation
                            // Check which model is selected
                            if (row.Field<string>("type_name").Equals(TFNETReinforcementL.ModelName))
                            {
                                // Create models for Tensorflow.Keras Neural Networks models
                                CWD_RL_TFNET cwd_RLTFNET = new CWD_RL_TFNET(_objectivesModelsDic, null);
                                Thread cwd_RLTFNETThread = new Thread(() => cwd_RLTFNET.initializeRLModelForCWD(GeneralTools.ByteArrayToObject<CWDReinforcementL>(row.Field<byte[]>("the_model"))));
                                cwd_RLTFNETThread.Start();
                            }
                            else if (row.Field<string>("type_name").Equals(TFNETLSTMModel.ModelName))
                            {
                                // Create models for Tensorflow.Keras Neural Networks models
                                CWD_TF_NET_LSTM cwdLSTMTFNET = new CWD_TF_NET_LSTM(_objectivesModelsDic, null);
                                Thread cwdLSTMTFNETThread = new Thread(() => cwdLSTMTFNET.initializeRLModelForCWD(GeneralTools.ByteArrayToObject<CWDLSTM>(row.Field<byte[]>("the_model"))));
                                cwdLSTMTFNETThread.Start();
                            }
                        }
                    }
                    // If yes then query for last signal id from the most trained model
                    resetBadge();
                }
                else
                {
                    // If yes then this report is from dataset
                    // Update the notification badge for unfitted signals
                    Control badge = MainFormFolder.BadgeControl.GetBadge(this);
                    while (true)
                    {
                        try
                        {
                            long unfittedData = dataTable.Rows.Count - _largestDatasetSize;
                            if (unfittedData == 0)
                                this.Invoke(new MethodInvoker(delegate () { badge.Visible = false; }));
                            else
                            {
                                this.Invoke(new MethodInvoker(delegate () { badge.Text = unfittedData.ToString(); }));
                                this.Invoke(new MethodInvoker(delegate () { badge.Visible = true; }));
                            }
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
}

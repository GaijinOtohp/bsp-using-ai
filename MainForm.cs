using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using BSP_Using_AI.AITools;
using BSP_Using_AI.Database;
using BSP_Using_AI.SignalHolderFolder;
using Keras;
using Keras.Layers;
using Keras.Models;
using Numpy;

namespace BSP_Using_AI
{
    public partial class MainForm : Form, DbStimulatorReportHolder
    {
        public long _largestDatasetSize = 0;

        public Hashtable _targetsModelsHashtable = null;

        public bool scrollAllowed = true;

        public TFBackThread _tFBackThread;

        public MainForm()
        {
            InitializeComponent();

            // Set models reader to ready
            _targetsModelsHashtable = new Hashtable();

            // Set the notification badge over "aiToolsButton" ready
            MainFormFolder.BadgeControl.AddBadgeTo(aiToolsButton, "0");

            _tFBackThread = new TFBackThread();
            _tFBackThread._targetsModelsHashtable = _targetsModelsHashtable;
            Thread TFThread = new Thread(() => _tFBackThread.ThreadServer());
            TFThread.IsBackground = true;
            TFThread.Start();

            /// Look for unfitted signals
            // Query for last signal id from the most trained model
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("models",
                                new String[] { "type_name", "model_target", "the_model", "selected_variables", "outputs_thresholds", "model_path", "dataset_size" },
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
                    // Set models ready and get last_signal_id of the highest model size
                    foreach (DataRow row in dataTable.AsEnumerable())
                    {
                        // Check which model for which terget is this record "type_name", "model_target", "the_model", "selected_variables", "outputs_thresholds", "model_path", "dataset_size"
                        if (row.Field<long>("dataset_size") > _largestDatasetSize)
                            _largestDatasetSize = row.Field<long>("dataset_size");
                        // Add modelsList in _targetsModelsHashtable
                        if (row.Field<string>("type_name").Equals("Neural network") && row.Field<string>("model_target").Equals("WPW syndrome detection"))
                        {
                            _tFBackThread._queue.Enqueue(new object[] { "initializeNeuralNetworkModelsForWPW", "MainForm", row.Field<string>("type_name") + " for " + row.Field<string>("model_target"), row.Field<string>("model_path"),
                                                                        (List<double[]>[])Garage.ByteArrayToObject(row.Field<byte[]>("selected_variables")), (List<float[]>)Garage.ByteArrayToObject(row.Field<byte[]>("outputs_thresholds")) });
                            _tFBackThread._signal.Set();
                        } else if (row.Field<string>("type_name").Equals("K-Nearest neighbor") && row.Field<string>("model_target").Equals("WPW syndrome detection"))
                        {
                            // Create models for KNN
                            KNNBackThread kNNBackThread = new KNNBackThread(_targetsModelsHashtable, null);
                            Thread knnThread = new Thread(() => kNNBackThread.initializeNeuralNetworkModelsForWPW(row.Field<long>("dataset_size"), (int[])Garage.ByteArrayToObject(row.Field<byte[]>("the_model")),
                                                                              (List<double[]>[])Garage.ByteArrayToObject(row.Field<byte[]>("selected_variables")), (List<float[]>)Garage.ByteArrayToObject(row.Field<byte[]>("outputs_thresholds"))));
                            knnThread.Start();
                        }
                        else if (row.Field<string>("type_name").Equals("Naive bayes") && row.Field<string>("model_target").Equals("WPW syndrome detection"))
                        {
                            // Create models for naive bayes
                            NaiveBayesBackThread naiveBayesBackThread = new NaiveBayesBackThread(_targetsModelsHashtable, null);
                            Thread nbThread = new Thread(() => naiveBayesBackThread.initializeNeuralNetworkModelsForWPW(row.Field<byte[]>("the_model"),
                                                                                    (List<double[]>[])Garage.ByteArrayToObject(row.Field<byte[]>("selected_variables")), (List<float[]>)Garage.ByteArrayToObject(row.Field<byte[]>("outputs_thresholds"))));
                            nbThread.Start();
                        }
                    }
                    // If yes then query for last signal id from the most trained model
                    resetBadge();
                } else
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

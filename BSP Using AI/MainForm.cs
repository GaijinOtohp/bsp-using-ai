using Biological_Signal_Processing_Using_AI;
using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.AITools.Keras_NET_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.KNN_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.NaiveBayes_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.Database;
using BSP_Using_AI.SignalHolderFolder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;

namespace BSP_Using_AI
{
    public partial class MainForm : Form, DbStimulatorReportHolder
    {
        public long _largestDatasetSize = 0;

        public Dictionary<string, ARTHTModels> _arthtModelsDic = null;

        public bool scrollAllowed = true;

        public ARTHT_Keras_NET_NN _tFBackThread;





        public class staton
        {
            public int _index;
            public double _value;

            public double _prevMeanTan;
            public double _nextMeanTan;
            public double _deviationAngle; // Argument

            public double _prevMeanMag;
            public double _nextMeanMag;
        }

        private void TangentDeviationPoints(double[] signal, double sampleRate)
        {
            double amplitudeInterval = GeneralTools.amplitudeInterval(signal);
            double magThreshold = 0.02d;
            double angThreshold = 70d;

            staton[] states = new staton[signal.Length];
            staton latestSta = null;

            for (int i = 0; i < signal.Length; i++)
            {
                // Set current sample
                states[i] = new staton { _index = i, _value = signal[i] };

                if (i == 0)
                    latestSta = states[0];

                int latStaShiftIndx = latestSta._index;

                // Compute _prevMeanMag and _prevMeanTan of the current sample
                if (i - latStaShiftIndx > 0)
                {
                    double xPrevDiff = (states[i]._index - latestSta._index) / sampleRate;
                    double yPrevDiff = signal[states[i]._index] - signal[latestSta._index];

                    double prevMag = Math.Sqrt(Math.Pow(xPrevDiff, 2) + Math.Pow(yPrevDiff, 2));
                    states[i]._prevMeanMag = (states[i - 1]._prevMeanMag * (i - 1 - latStaShiftIndx) + prevMag) / (i - latStaShiftIndx);

                    double prevTan = yPrevDiff / xPrevDiff;
                    states[i]._prevMeanTan = (states[i - 1]._prevMeanTan * (i - 1 - latStaShiftIndx) + prevTan) / (i - latStaShiftIndx);
                }

                // Check if prevMeanMag is twice larger than amplitudeInterval * magThreshold
                if (i - latStaShiftIndx > 1)
                {
                    // Update _nextMeanMag, _nextMeanTan, and _deviationAngle of the samples between the latest state and the current sample
                    for (int j = latStaShiftIndx + 1; j < i; j++)
                    {
                        double xNextDiff = (states[i]._index - states[j]._index) / sampleRate;
                        double yNextDiff = signal[states[i]._index] - signal[states[j]._index];

                        double nextMag = Math.Sqrt(Math.Pow(xNextDiff, 2) + Math.Pow(yNextDiff, 2));
                        states[j]._nextMeanMag = (states[j]._nextMeanMag * (i - 1 - j) + nextMag) / (i - j);

                        double nextTan = yNextDiff / xNextDiff;
                        states[j]._nextMeanTan = (states[j]._nextMeanTan * (i - 1 - j) + nextTan) / (i - j);

                        states[j]._deviationAngle = (Math.Atan(states[j]._nextMeanTan) - Math.Atan(states[j]._prevMeanTan)) * 180 / Math.PI;
                    }

                    // Get the sample with the largest angle that exceeds angThreshold
                    // and both of _prevMeanMag and _nextMeanMag exceeds amplitudeInterval * magThreshold
                    staton[] tempLargest = states.Select((state, index) => (state, index)).Where(tuple => tuple.index > latStaShiftIndx && tuple.index < i).
                                                                                            Where(tuple => tuple.state._nextMeanMag > amplitudeInterval * magThreshold
                                                                                            && tuple.state._prevMeanMag > amplitudeInterval * magThreshold
                                                                                            && Math.Abs(tuple.state._deviationAngle) > angThreshold).
                                                                                            Select(tuple => tuple.state).ToArray();

                    if (tempLargest.Length > 0)
                    {
                        latestSta = tempLargest.Select(state => (Math.Abs(state._deviationAngle), state)).Max().state;
                        Debug.WriteLine(latestSta._index);
                        Debug.WriteLine(latestSta._deviationAngle);
                        // If new corner is created
                        // then update all previousMeanMag and _prevMeanTan of the new corner's next samples
                        latStaShiftIndx = latestSta._index;
                        double[] prevMeanMags = new double[i - latStaShiftIndx + 1];
                        double[] prevMeanTans = new double[i - latStaShiftIndx + 1];
                        for (int j = 1; j <= i - latStaShiftIndx; j++)
                        {
                            double xPrevDiff = (states[j + latStaShiftIndx]._index - latestSta._index) / sampleRate;
                            double yPrevDiff = signal[states[j + latStaShiftIndx]._index] - signal[latestSta._index];

                            double prevMag = Math.Sqrt(Math.Pow(xPrevDiff, 2) + Math.Pow(yPrevDiff, 2));
                            prevMeanMags[j] = prevMeanMags[j - 1] * (j - 1) + prevMag / j;
                            states[j + latStaShiftIndx]._prevMeanMag = prevMeanMags[j];

                            double prevTan = yPrevDiff / xPrevDiff;
                            prevMeanTans[j] = prevMeanTans[j - 1] * (j - 1) + prevTan / j;
                            states[j + latStaShiftIndx]._prevMeanTan = prevMeanTans[j];
                        }
                    }
                }
            }
        }




        public MainForm()
        {

            double[] aa = new double[1000];
            for (int i = 0; i < 1000; i++)
                if (i / 100 == 0)
                    aa[i] = 1;
                else
                    aa[i] = 4;

            //TangentDeviationPoints(aa, 500);




            InitializeComponent();

            // Set models reader to ready
            _arthtModelsDic = new Dictionary<string, ARTHTModels>();

            // Set the notification badge over "aiToolsButton" ready
            MainFormFolder.BadgeControl.AddBadgeTo(aiToolsButton, "0");

            _tFBackThread = new ARTHT_Keras_NET_NN();
            _tFBackThread._arthtModelsDic = _arthtModelsDic;
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
                        ARTHTModels aRTHTModels = GeneralTools.ByteArrayToObject<ARTHTModels>(row.Field<byte[]>("the_model"));
                        namesList.Add(aRTHTModels.ModelName + aRTHTModels.ProblemName);
                    }
                    rowsList = GeneralTools.OrderByTextWithNumbers(rowsList, namesList);

                    // Set models ready and get last_signal_id of the highest model size
                    foreach (DataRow row in rowsList)
                    {
                        // Check which model for which terget is this record "type_name", "model_target", "the_model", "selected_variables", "outputs_thresholds", "model_path", "dataset_size"
                        if (row.Field<long>("dataset_size") > _largestDatasetSize)
                            _largestDatasetSize = row.Field<long>("dataset_size");
                        // Add modelsList in _targetsModelsHashtable
                        if (row.Field<string>("type_name").Equals("Neural network") && row.Field<string>("model_target").Equals("WPW syndrome detection"))
                        {
                            _tFBackThread._queue.Enqueue(new QueueSignalInfo()
                            {
                                TargetFunc = "initializeNeuralNetworkModelsForWPW",
                                CallingClass = "MainForm",
                                aRTHTModels = GeneralTools.ByteArrayToObject<ARTHTModels>(row.Field<byte[]>("the_model"))
                            });
                            _tFBackThread._signal.Set();
                        }
                        else if (row.Field<string>("type_name").Equals("K-Nearest neighbor") && row.Field<string>("model_target").Equals("WPW syndrome detection"))
                        {
                            // Create models for KNN
                            ARTHT_KNN kNNBackThread = new ARTHT_KNN(_arthtModelsDic, null);
                            Thread knnThread = new Thread(() => kNNBackThread.initializeNeuralNetworkModelsForWPW(GeneralTools.ByteArrayToObject<ARTHTModels>(row.Field<byte[]>("the_model"))));
                            knnThread.Start();
                        }
                        else if (row.Field<string>("type_name").Equals("Naive bayes") && row.Field<string>("model_target").Equals("WPW syndrome detection"))
                        {
                            // Create models for naive bayes
                            ARTHT_NaiveBayes naiveBayesBackThread = new ARTHT_NaiveBayes(_arthtModelsDic, null);
                            Thread nbThread = new Thread(() => naiveBayesBackThread.initializeNeuralNetworkModelsForWPW(GeneralTools.ByteArrayToObject<ARTHTModels>(row.Field<byte[]>("the_model"))));
                            nbThread.Start();
                        }
                        else if (row.Field<string>("type_name").Equals(TFNETNeuralNetworkModel.ModelName) && row.Field<string>("model_target").Equals("WPW syndrome detection"))
                        {
                            // Create models for Tensorflow.Net Neural Networks models
                            ARTHT_TF_NET_NN tf_NET_NN = new ARTHT_TF_NET_NN(_arthtModelsDic, null);
                            Thread tfNetThread = new Thread(() => tf_NET_NN.initializeTFNETNeuralNetworkModelsForWPW(GeneralTools.ByteArrayToObject<ARTHTModels>(row.Field<byte[]>("the_model"))));
                            tfNetThread.Start();
                        }
                        else if (row.Field<string>("type_name").Equals(TFKerasNeuralNetworkModel.ModelName) && row.Field<string>("model_target").Equals("WPW syndrome detection"))
                        {
                            // Create models for Tensorflow.Keras Neural Networks models
                            TF_KERAS_NN tf_Keras_NN = new TF_KERAS_NN(_arthtModelsDic, null);
                            Thread tfKerasThread = new Thread(() => tf_Keras_NN.initializeTFKerasNeuralNetworkModelsForWPW(GeneralTools.ByteArrayToObject<ARTHTModels>(row.Field<byte[]>("the_model"))));
                            tfKerasThread.Start();
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

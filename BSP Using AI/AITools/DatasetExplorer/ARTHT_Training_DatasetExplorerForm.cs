using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.AITools.Keras_NET_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.KNN_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.NaiveBayes_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools.DatasetExplorer
{
    public partial class DatasetExplorerForm
    {
        public void queryForTrainingDataset_ARTHT()
        {
            // Check if there was any added intervals
            if (!(_objectiveModel.DataIdsIntervalsList.Count > _updatesNum))
                return;

            // Qurey for signals features in all last selected intervals from dataset
            (string selection, object[] selectionArgs) = SelectDataFromIntervals(null, null);

            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("dataset",
                                        new String[] { "features" },
                                        selection,
                                        selectionArgs,
                                        "", "DatasetExplorerFormForTraining_ARTHT"));
            dbStimulatorThread.Start();
        }

        private void fitSelectionButton_Click_ARTHT(object sender, EventArgs e)
        {
            // Start training
            queryForTrainingDataset_ARTHT();
        }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //:::::::::::::::::::::::::::CROSS PROCESS FORM FUNCTIONS (INTERFACES)::::::::::::::::::::::://
        public void holdRecordReport_ARTHT(DataTable dataTable, string callingClassName)
        {
            // Initialize lists of features for each step
            Dictionary<string, List<Sample>> dataLists = new Dictionary<string, List<Sample>>(7);

            // Iterate through each signal samples and sort them in dataLists
            ARTHTFeatures aRTHTFeatures = null;
            foreach (DataRow row in dataTable.AsEnumerable())
            {
                aRTHTFeatures = GeneralTools.ByteArrayToObject<ARTHTFeatures>(row.Field<byte[]>("features"));
                foreach (string stepName in aRTHTFeatures.StepsDataDic.Keys)
                {
                    if (!dataLists.ContainsKey(stepName))
                        dataLists.Add(stepName, new List<Sample>());

                    foreach (Sample sample in aRTHTFeatures.StepsDataDic[stepName].Samples)
                        dataLists[stepName].Add(sample);
                }
            }
            // Send features for fitting
            // Check which model is selected
            long datasetSize = _datasetSize + dataTable.Rows.Count;
            if (_objectiveModel.ModelName.Equals(KerasNETNeuralNetworkModel.ModelName))
            {
                // This is for neural network
                _aIToolsForm._tFBackThread._queue.Enqueue(new QueueSignalInfo()
                {
                    TargetFunc = "fit",
                    CallingClass = "DatasetExplorerForm",
                    ModelsName = _objectiveModel.ModelName + _objectiveModel.ObjectiveName,
                    DataLists = dataLists,
                    _datasetSize = datasetSize,
                    _modelId = _id,
                    StepName = ""
                });
                _aIToolsForm._tFBackThread._signal.Set();
            }
            else if (_objectiveModel.ModelName.Equals(KNNModel.ModelName))
            {
                // This is for knn
                ARTHT_KNN kNNBackThread = new ARTHT_KNN(_aIToolsForm._objectivesModelsDic, _aIToolsForm);
                Thread knnThread = new Thread(() => kNNBackThread.fit(_objectiveModel.ModelName + _objectiveModel.ObjectiveName, dataLists, datasetSize, _id, ""));
                knnThread.Start();
            }
            else if (_objectiveModel.ModelName.Equals(NaiveBayesModel.ModelName))
            {
                // This is for naive bayes
                ARTHT_NaiveBayes naiveBayesBackThread = new ARTHT_NaiveBayes(_aIToolsForm._objectivesModelsDic, _aIToolsForm);
                Thread nbThread = new Thread(() => naiveBayesBackThread.fit(_objectiveModel.ModelName + _objectiveModel.ObjectiveName, dataLists, datasetSize, _id, ""));
                nbThread.Start();
            }
            else if (_objectiveModel.ModelName.Equals(TFNETNeuralNetworkModel.ModelName))
            {
                // This is for Tensorflow.Net Neural Networks models
                ARTHT_TF_NET_NN tf_NET_NN = new ARTHT_TF_NET_NN(_aIToolsForm._objectivesModelsDic, _aIToolsForm);
                Thread tfNetThread = new Thread(() => tf_NET_NN.fit(_objectiveModel.ModelName + _objectiveModel.ObjectiveName, dataLists, datasetSize, _id, ""));
                tfNetThread.Start();
            }
            else if (_objectiveModel.ModelName.Equals(TFKerasNeuralNetworkModel.ModelName))
            {
                // This is for Tensorflow.Keras Neural Networks models
                TF_KERAS_NN tf_Keras_NN = new TF_KERAS_NN(_aIToolsForm._objectivesModelsDic, _aIToolsForm);
                Thread tfKerasThread = new Thread(() => tf_Keras_NN.fit(_objectiveModel.ModelName + _objectiveModel.ObjectiveName, dataLists, datasetSize, _id, ""));
                tfKerasThread.Start();
            }
        }
    }
}

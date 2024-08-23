using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.AITools.Keras_NET_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.KNN_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.NaiveBayes_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.RL_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.TF_NET_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.AITools.DatasetExplorer;
using BSP_Using_AI.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation
{
    partial class DataVisualisationForm : DbStimulatorReportHolder
    {
        public void queryForSelectedDataset_ARTHT(List<IdInterval> allDataIdsIntervalsList)
        {
            (string selection, object[] selectionArgs) = DatasetExplorerForm.SelectDataFromIntervals(allDataIdsIntervalsList, null, null);

            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("dataset",
                                        new String[] { "features" },
                                        selection,
                                        selectionArgs,
                                        "", "DataVisualisationForm_ARTHT"));
            dbStimulatorThread.Start();
        }

        public void queryForSelectedDataset_CWD(List<IdInterval> allDataIdsIntervalsList, string modelType)
        {
            (string selection, object[] selectionArgs) = DatasetExplorerForm.SelectDataFromIntervals(allDataIdsIntervalsList,
                                                                                     "and anno_objective=?", new object[] { CharacteristicWavesDelineation.ObjectiveName });

            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("anno_ds",
                                        new string[] { "sginal_name", "starting_index", "signal_data", "sampling_rate", "anno_data" },
                                        selection,
                                        selectionArgs,
                                        "", "DataVisualisationForm_" + modelType));
            dbStimulatorThread.Start();
        }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //:::::::::::::::::::::::::::CROSS PROCESS FORM FUNCTIONS (INTERFACES)::::::::::::::::::::::://

        private List<Sample> SortDatasetSamples_ARTHT(DataTable dataTable)
        {
            List<Sample> dataList = new List<Sample>();

            ARTHTFeatures aRTHTFeatures = null;
            foreach (DataRow row in dataTable.AsEnumerable())
            {
                aRTHTFeatures = GeneralTools.ByteArrayToObject<ARTHTFeatures>(row.Field<byte[]>("features"));

                dataList.AddRange(aRTHTFeatures.StepsDataDic[_InnerObjectiveModel.Name].Samples);
            }

            return dataList;
        }

        private List<Sample> SortDatasetSamples_CWDReinforcementL(DataTable dataTable)
        {
            Dictionary<string, List<Sample>> trainingSamplesListsDict = DatasetExplorerForm.GetPreviousTrainingSamples(dataTable.AsEnumerable().ToList());

            List<Sample> dataList = trainingSamplesListsDict.SelectMany(dictPair => dictPair.Value).ToList();

            return dataList;
        }

        private List<List<Sample>> SortDatasetSamples_CWDLSTM(DataTable dataTable)
        {
            List<List<Sample>> trainingDataListSequences = DatasetExplorerForm.BuildLSTMTrainingSequences(dataTable.AsEnumerable().ToList(), ((CWDLSTM)_objectiveModel).CWDReinforcementLModel);

            return trainingDataListSequences;
        }

        private CustomArchiBaseModel CreateTheNewModel(DataTable dataTable, string callingClassName)
        {
            if (dataTable.Rows.Count == 0)
                return _InnerObjectiveModel;

            // Check which objective is selected
            if (_objectiveModel.ObjectiveName.Contains(WPWSyndromeDetection.ObjectiveName))
            {
                // If yes then this is for WPW syndrome detection

                List<Sample> dataList = SortDatasetSamples_ARTHT(dataTable);

                // Get the dimension of the input and the output
                int inputDimension = PCA.Where(pcaItem => pcaItem._selected).Count();
                if (inputDimension == 0)
                    inputDimension = dataList[0].getFeatures().Length;
                int outputDimension = dataList[0].getOutputs().Length;

                // Check which model is selected
                if (_InnerObjectiveModel is KNNModel)
                    // If yes then this is for KNN models
                    _InnerObjectiveModel = ARTHT_KNN.createKNNModel(_InnerObjectiveModel.Name, 3, inputDimension, outputDimension);
                else if (_InnerObjectiveModel is NaiveBayesModel)
                    // If yes then this is for Naive Bayes models
                    _InnerObjectiveModel = ARTHT_NaiveBayes.createNBModel(_InnerObjectiveModel.Name, inputDimension, outputDimension);
                else if (_InnerObjectiveModel is TFNETNeuralNetworkModel)
                    // If yes then this is for Tensorflow.Net Neural Networks models
                    _InnerObjectiveModel = ARTHT_TF_NET_NN.createTFNETNeuralNetModel(_InnerObjectiveModel.Name, ((TFNETNeuralNetworkModel)_InnerObjectiveModel).BaseModel.ModelPath, inputDimension, outputDimension);

                ((ARTHTModels)_objectiveModel).ARTHTModelsDic[_InnerObjectiveModel.Name] = _InnerObjectiveModel;
            }
            else if (_objectiveModel.ObjectiveName.Contains(CharacteristicWavesDelineation.ObjectiveName))
            {
                // This is for characteristic waves dlineation
                // Check which model is selected
                if (_InnerObjectiveModel is TFNETReinforcementL)
                {
                    // If yes then this is for Reinforcement learning models

                    List<Sample> dataList = SortDatasetSamples_CWDReinforcementL(dataTable);

                    // Get the dimension of the input and the output
                    int inputDimension = PCA.Where(pcaItem => pcaItem._selected).Count();
                    if (inputDimension == 0)
                        inputDimension = dataList[0].getFeatures().Length;
                    int outputDimension = dataList[0].getOutputs().Length;

                    _InnerObjectiveModel = CWD_RL_TFNET.createTFNETRLModel(_InnerObjectiveModel.Name, ((TFNETReinforcementL)_InnerObjectiveModel).BaseModel.ModelPath, inputDimension, outputDimension);
                    
                    if (_objectiveModel is CWDReinforcementL cwdRLModel)
                        cwdRLModel.CWDReinforcementLModel = (TFNETReinforcementL)_InnerObjectiveModel;
                    else if (_objectiveModel is CWDLSTM cwdLSTMModel)
                        cwdLSTMModel.CWDReinforcementLModel = (TFNETReinforcementL)_InnerObjectiveModel;
                }
                if (_InnerObjectiveModel is TFNETLSTMModel)
                {
                    // If yes then this is for LSTM models

                    List<List<Sample>> dataListSequences = SortDatasetSamples_CWDLSTM(dataTable);

                    if (dataListSequences[0].Count == 0)
                        return _InnerObjectiveModel;

                    // Get the dimension of the input and the output
                    int inputDimension = PCA.Where(pcaItem => pcaItem._selected).Count();
                    if (inputDimension == 0)
                        inputDimension = dataListSequences[0][0].getFeatures().Length;
                    int outputDimension = dataListSequences[0][0].getOutputs().Length;

                    _InnerObjectiveModel = CWD_TF_NET_LSTM.createTFNETLSTMModel(_InnerObjectiveModel.Name, ((TFNETLSTMModel)_InnerObjectiveModel).BaseModel.ModelPath, inputDimension, outputDimension);
                    
                    ((CWDLSTM)_objectiveModel).CWDLSTMModel = (TFNETLSTMModel)_InnerObjectiveModel;
                }
            }

            return _InnerObjectiveModel;
        }

        private void TrainTheModel(DataTable dataTable, string callingClassName)
        {
            // Check which objective is selected
            if (_objectiveModel.ObjectiveName.Contains(WPWSyndromeDetection.ObjectiveName))
            {
                // If yes then this is for WPW syndrome detection

                List<Sample> dataList = SortDatasetSamples_ARTHT(dataTable);
                Dictionary<string, List<Sample>> dataListsDict = new Dictionary<string, List<Sample>>();
                dataListsDict.Add(_InnerObjectiveModel.Name, dataList);

                // Check which model is selected
                if (_InnerObjectiveModel is KNNModel)
                {
                    // If yes then this is for KNN models
                    ARTHT_KNN ARTHTKNNTools = new ARTHT_KNN(_objectivesModelsDic, _ValidationItemUserControl);
                    ARTHTKNNTools.fit(_objectiveModel.ModelName + _objectiveModel.ObjectiveName, dataListsDict, _datasetSize, _modelId, "");
                }
                else if (_InnerObjectiveModel is NaiveBayesModel)
                {
                    // If yes then this is for Naive Bayes models
                    ARTHT_NaiveBayes ARTHTNaiveBayesTools = new ARTHT_NaiveBayes(_objectivesModelsDic, _ValidationItemUserControl);
                    ARTHTNaiveBayesTools.fit(_objectiveModel.ModelName + _objectiveModel.ObjectiveName, dataListsDict, _datasetSize, _modelId, "");
                }
                else if (_InnerObjectiveModel is TFNETNeuralNetworkModel)
                {
                    // If yes then this is for Tensorflow.Net Neural Networks models
                    ARTHT_TF_NET_NN ARTHTTFNETTools = new ARTHT_TF_NET_NN(_objectivesModelsDic, _ValidationItemUserControl);
                    ARTHTTFNETTools.fit(_objectiveModel.ModelName + _objectiveModel.ObjectiveName, dataListsDict, _datasetSize, _modelId, "");
                }
            }
            else if (_objectiveModel.ObjectiveName.Contains(CharacteristicWavesDelineation.ObjectiveName))
            {
                // This is for characteristic waves dlineation
                // Check which model is selected
                if (_InnerObjectiveModel is TFNETReinforcementL)
                {
                    // If yes then this is for Reinforcement learning models

                    List<Sample> dataList = SortDatasetSamples_CWDReinforcementL(dataTable);

                    if (_objectiveModel is CWDReinforcementL)
                    {
                        CWD_RL_TFNET CWDRLTFNETTools = new CWD_RL_TFNET(_objectivesModelsDic, _ValidationItemUserControl);
                        CWDRLTFNETTools.Fit(_objectiveModel.ModelName + _objectiveModel.ObjectiveName, dataList, _datasetSize, _modelId);
                    }
                    else if (_objectiveModel is CWDLSTM)
                    {
                        CWD_TF_NET_LSTM CWDLSTMTFNETTools = new CWD_TF_NET_LSTM(_objectivesModelsDic, _ValidationItemUserControl);
                        CWDLSTMTFNETTools.FitOnRLModel(_objectiveModel.ModelName + _objectiveModel.ObjectiveName, dataList);

                        List<List<Sample>> dataListSequences = new List<List<Sample>>();
                        CWDLSTMTFNETTools.FitOnLSTMModel(_objectiveModel.ModelName + _objectiveModel.ObjectiveName, dataListSequences, _datasetSize, _modelId);
                    }
                }
                if (_InnerObjectiveModel is TFNETLSTMModel)
                {
                    // If yes then this is for LSTM models

                    List<List<Sample>> dataListSequences = SortDatasetSamples_CWDLSTM(dataTable);

                    CWD_TF_NET_LSTM CWDLSTMTFNETTools = new CWD_TF_NET_LSTM(_objectivesModelsDic, _ValidationItemUserControl);
                    CWDLSTMTFNETTools.FitOnLSTMModel(_objectiveModel.ModelName + _objectiveModel.ObjectiveName, dataListSequences, _datasetSize, _modelId);
                }
            }
        }

        private void CreateTheNewModelAndTrain(DataTable dataTable, string callingClassName)
        {
            // ------------------Create the new inner model with the new input dimension
            _InnerObjectiveModel = CreateTheNewModel(dataTable, callingClassName);

            // ------------------Update the PCA values
            //Remove previous selection of eigenvectors
            _InnerObjectiveModel.PCA.Clear();
            // Save selected eigenvectors in _pcLoadingScoresArray
            foreach (PCAitem pcaItem in PCA)
                _InnerObjectiveModel.PCA.Add(pcaItem.Clone());
            // Set if PCA is activated or not
            int selectedPCCount = PCA.Where(pcaItem => pcaItem._selected).Count();
            _InnerObjectiveModel._pcaActive = (selectedPCCount > 0);

            // ------------------Fit the data to the new inner model
            // Initialize list of features for selected step
            TrainTheModel(dataTable, callingClassName);
        }

        public void holdRecordReport(DataTable dataTable, string callingClassName)
        {
            if (!callingClassName.Contains("DataVisualisationForm"))
                return;

            // Update the inner model in a different thread
            Thread modelUpdateThread = new Thread(() => CreateTheNewModelAndTrain(dataTable, callingClassName));
            modelUpdateThread.Start();
        }
    }
}

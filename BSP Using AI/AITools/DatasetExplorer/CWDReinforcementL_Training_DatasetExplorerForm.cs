using Biological_Signal_Processing_Using_AI.AITools.RL_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.AITools.AIBackThreadReportHolder;

namespace BSP_Using_AI.AITools.DatasetExplorer
{
    public partial class DatasetExplorerForm
    {
        public static Dictionary<string, List<Sample>> GetPreviousTrainingSamples(DataTable dataTable)
        {
            DbStimulator dbStimulator = new DbStimulator();
            DataTable previousDataDataTable = dbStimulator.Query("cwd_rl_dataset",
                                new string[] { "sginal_data_key", "training_data" },
                                null,
                                null,
                                "", "DatasetExplorerFormForTraining_CWDReinforcementL");
            // Convert them to a dicitonary
            Dictionary<string, Data> previousDataDict = new Dictionary<string, Data>(previousDataDataTable.Rows.Count);
            foreach (DataRow row in previousDataDataTable.AsEnumerable())
                previousDataDict.Add(row.Field<string>("sginal_data_key"), GeneralTools.ByteArrayToObject<Data>(row.Field<byte[]>("training_data")));

            // Create the training samples list
            Dictionary<string, List<Sample>> trainingSamplesListsDict = new Dictionary<string, List<Sample>>(dataTable.Rows.Count * 10);

            foreach (DataRow row in dataTable.AsEnumerable())
            {
                string signalDataKey = row.Field<string>("sginal_name") + row.Field<long>("starting_index");
                if (previousDataDict.ContainsKey(signalDataKey))
                    trainingSamplesListsDict.Add(signalDataKey, previousDataDict[signalDataKey].Samples);
            }

            return trainingSamplesListsDict;
        }

        public List<Sample> GetTrainingSamples(DataTable dataTable, TFNETReinforcementL CWDCrazyReinforcementLModel)
        {
            int fitProgress = 0;
            int tolatFitProgress = dataTable.Rows.Count;
            string modelName = _objectiveModel.ModelName + _objectiveModel.ObjectiveName;

            // Initialize the reinforcement learning environment
            CWD_RL cwdRL = new CWD_RL(CWDCrazyReinforcementLModel._DimensionsList);

            // Get previously learned data
            Dictionary<string, List<Sample>> trainingSamplesListsDict = GetPreviousTrainingSamples(dataTable);

            foreach (DataRow row in dataTable.AsEnumerable())
            {
                // Update fitProgressBar
                fitProgress++;
                if (_aIToolsForm != null)
                    _aIToolsForm.holdAIReport(new FittingProgAIReport()
                    {
                        ReportType = AIReportType.FittingProgress,
                        ModelName = modelName,
                        fitProgress = fitProgress,
                        fitMaxProgress = tolatFitProgress
                    }, "AIToolsForm");
                // Check if current signal is already learned before
                string signalDataKey = row.Field<string>("sginal_name") + row.Field<long>("starting_index");
                if (trainingSamplesListsDict.ContainsKey(signalDataKey))
                    continue;

                // Learn the new signal
                int samplingRate = (int)(row.Field<long>("sampling_rate"));
                int startingIndex = (int)(row.Field<long>("starting_index") * samplingRate);
                double[] signalSamples = GeneralTools.ByteArrayToObject<double[]>(row.Field<byte[]>("signal_data"));
                AnnotationData annoData = GeneralTools.ByteArrayToObject<AnnotationData>(row.Field<byte[]>("anno_data"));

                Data newSignalData = cwdRL.DeepFitRLData(signalSamples, samplingRate, annoData, CWDCrazyReinforcementLModel);
                // Append the new siganl data into trainingSamplesList
                trainingSamplesListsDict.Add(signalDataKey, newSignalData.Samples);

                // Save the new signal data into cwd_rl_dataset
                DbStimulator dbStimulator = new DbStimulator();
                Thread dbStimulatorThread = new Thread(() => dbStimulator.Insert("cwd_rl_dataset",
                                        new string[] { "sginal_data_key", "training_data" },
                                        new object[] { signalDataKey, GeneralTools.ObjectToByteArray(newSignalData) },
                                        "DatasetExplorerFormForTraining_CWDReinforcementL"));
                dbStimulatorThread.Start();
            }

            // Save the crazy model
            //TF_NET_NN.SaveModelVariables(CWDCrazyReinforcementLModel.BaseModel.Session, CWDCrazyReinforcementLModel.BaseModel.ModelPath, new string[] { "output" });

            return trainingSamplesListsDict.SelectMany(dictPair => dictPair.Value).ToList();
        }

        public void holdRecordReport_CWDReinforcementL(DataTable dataTable, string callingClassName)
        {
            string modelName = _objectiveModel.ModelName + _objectiveModel.ObjectiveName;
            List<Sample> trainingSamplesList = GetTrainingSamples(dataTable, ((CWDReinforcementL)_objectiveModel).CWDCrazyReinforcementLModel);

            // Now fit all of trainingSamplesList into the reinforcement learning neural network model
            long datasetSize = _datasetSize + dataTable.Rows.Count;
            CWD_RL_TFNET cwdRLTFNET = new CWD_RL_TFNET(_aIToolsForm._objectivesModelsDic, _aIToolsForm);
            Thread cwdRLTFNETThread = new Thread(() => cwdRLTFNET.Fit(modelName, trainingSamplesList, datasetSize, _id));
            cwdRLTFNETThread.Start();
        }
    }
}

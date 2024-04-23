using Biological_Signal_Processing_Using_AI.AITools.RL_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools.DatasetExplorer
{
    public partial class DatasetExplorerForm
    {
        public void queryForTrainingDataset_CWDReinforcementL()
        {
            // Check if there was any added intervals
            if (!(_objectiveModel.DataIdsIntervalsList.Count > _updatesNum))
                return;

            // Qurey for signals features in all last selected intervals from dataset
            (string selection, object[] selectionArgs) = SelectDataFromIntervals("and anno_objective=?", new object[] { CharacteristicWavesDelineation.ObjectiveName });

            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("anno_ds",
                                        new string[] { "sginal_name", "starting_index", "signal_data", "sampling_rate", "anno_data" },
                                        selection,
                                        selectionArgs,
                                        "", "DatasetExplorerFormForTraining_CWDReinforcementL"));
            dbStimulatorThread.Start();
        }

        public void holdRecordReport_CWDReinforcementL(DataTable dataTable, string callingClassName)
        {
            int fitProgress = 0;
            int tolatFitProgress = dataTable.Rows.Count;
            string modelName = _objectiveModel.ModelName + _objectiveModel.ObjectiveName;

            // Initialize the reinforcement learning environment
            CWD_RL cwdRL = new CWD_RL();

            // Get previously learned data
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
            List<Sample> trainingSamplesList = new List<Sample>(previousDataDataTable.Rows.Count * 10);
            foreach (Data trainingSamples in previousDataDict.Values)
                trainingSamplesList.AddRange(trainingSamples.Samples);

            foreach (DataRow row in dataTable.AsEnumerable())
            {
                // Update fitProgressBar
                fitProgress++;
                if (_aIToolsForm != null)
                    _aIToolsForm.holdAIReport(new object[] { "progress", modelName, fitProgress, tolatFitProgress }, "AIToolsForm");
                // Check if current signal is already learned before
                string signalDataKey = row.Field<string>("sginal_name") + row.Field<long>("starting_index");
                if (previousDataDict.ContainsKey(signalDataKey))
                    continue;

                // Learn the new signal
                int samplingRate = (int)(row.Field<long>("sampling_rate"));
                int startingIndex = (int)(row.Field<long>("starting_index") * samplingRate);
                double[] signalSamples = GeneralTools.ByteArrayToObject<double[]>(row.Field<byte[]>("signal_data"));
                AnnotationData annoData = GeneralTools.ByteArrayToObject<AnnotationData>(row.Field<byte[]>("anno_data"));
                int[] trueCornersIndex = annoData.GetAnnotations().Where(ecgAnno => !ecgAnno.Name.Equals("Delta")).Select(anno => anno.GetIndexes().starting).ToArray();

                Data newSignalData = cwdRL.FitRLData(signalSamples, samplingRate, trueCornersIndex);
                // Append the new siganl data into trainingSamplesList
                trainingSamplesList.AddRange(newSignalData.Samples);

                // Save the new signal data into cwd_rl_dataset
                Thread dbStimulatorThread = new Thread(() => dbStimulator.Insert("cwd_rl_dataset",
                                        new string[] { "sginal_data_key", "training_data" },
                                        new object[] { signalDataKey, GeneralTools.ObjectToByteArray(newSignalData) },
                                        "DatasetExplorerFormForTraining_CWDReinforcementL"));
                dbStimulatorThread.Start();
            }

            // Now fit all of trainingSamplesList into the reinforcement learning neural network model
            long datasetSize = _datasetSize + dataTable.Rows.Count;
            CWD_RL_TFNET cwdRLTFNET = new CWD_RL_TFNET(_aIToolsForm._objectivesModelsDic, _aIToolsForm);
            Thread cwdRLTFNETThread = new Thread(() => cwdRLTFNET.Fit(modelName, trainingSamplesList, datasetSize, _id));
            cwdRLTFNETThread.Start();
        }

        private void fitSelectionButton_Click_CWDReinforcementL(object sender, EventArgs e)
        {
            // Start training
            queryForTrainingDataset_CWDReinforcementL();
        }
    }
}

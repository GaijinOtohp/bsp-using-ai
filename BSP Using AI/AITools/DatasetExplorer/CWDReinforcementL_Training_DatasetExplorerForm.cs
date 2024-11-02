using Biological_Signal_Processing_Using_AI.AITools.RL_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.AITools.AIBackThreadReportHolder;

namespace BSP_Using_AI.AITools.DatasetExplorer
{
    public partial class DatasetExplorerForm
    {
        public static Dictionary<string, List<Sample>> GetPreviousTrainingSamples(List<DataRow> rowsList)
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
            Dictionary<string, List<Sample>> trainingSamplesListsDict = new Dictionary<string, List<Sample>>(rowsList.Count * 10);

            foreach (DataRow row in rowsList)
            {
                string signalDataKey = row.Field<string>("sginal_name") + row.Field<long>("starting_index");
                if (previousDataDict.ContainsKey(signalDataKey))
                    trainingSamplesListsDict.Add(signalDataKey, previousDataDict[signalDataKey].Samples);
            }

            return trainingSamplesListsDict;
        }

        private static void UpdateFitProgressBar(AIToolsForm aiToolsForm, string modelName, ref int fitProgress, int totalFitProgress)
        {
            fitProgress++;
            if (aiToolsForm != null)
                aiToolsForm.holdAIReport(new FittingProgAIReport()
                {
                    ReportType = AIReportType.FittingProgress,
                    ModelName = modelName,
                    fitProgress = fitProgress,
                    fitMaxProgress = totalFitProgress
                }, "AIToolsForm");
        }

        private static List<(double[] segmentSamples, AnnotationData segmentAnnos)> GetRepresentanteSegments(double[] signalSamples, double samplingRate, AnnotationData annoData)
        {
            // Proposing that for each 5 min (300 seconds) of the signal should have a representante segment
            // Get the number of possible segments
            double repRegionLenSec = 300d;
            double signalTimeLenSec = signalSamples.Length / samplingRate;
            int segments = (int)(signalTimeLenSec / repRegionLenSec + (signalTimeLenSec % repRegionLenSec > 0 ? 1 : 0));

            // Create the segments by taking random 10 seconds segments from each part of the original signal
            List<(double[] segmentSamples, AnnotationData segmentAnnos)> representanteSegments = new List<(double[] segmentSamples, AnnotationData segmentAnnos)>(segments + 1);
            Random random = new Random();
            for (int i = 0; i < segments; i++)
            {
                // Get the possible limits of the segment
                int segMaxEndSec = (int)Math.Min(i * repRegionLenSec + (repRegionLenSec - 10), signalTimeLenSec - 10);
                int segMinStartSec = (int)Math.Max(segMaxEndSec - (repRegionLenSec - 10), 0);
                int segmentStartIndex = (int)(random.Next(segMinStartSec, segMaxEndSec) * samplingRate);
                int segmentEndIndex = (int)(segmentStartIndex + 10 * samplingRate);
                // Get the samples of the segment
                double[] segmentSamples = signalSamples.Where((value, index) => segmentStartIndex <= index && index < segmentEndIndex).ToArray();
                // Translate the annotation indecies of the selected segment
                AnnotationData segmentAnnos = new AnnotationData(annoData.Name);
                foreach (AnnotationECG ecgAnno in annoData.GetAnnotations().Where(anno => segmentStartIndex <= anno.GetIndexes().starting && anno.GetIndexes().starting < segmentEndIndex))
                    segmentAnnos.InsertAnnotation(ecgAnno.Name, ecgAnno.GetAnnotationType(), ecgAnno.GetIndexes().starting - segmentStartIndex, 0);

                representanteSegments.Add((segmentSamples, segmentAnnos));
            }

            return representanteSegments;
        }

        public static List<Sample> GetTrainingSamples(List<DataRow> rowsList, TFNETReinforcementL CWDCrazyReinforcementLModel, AIToolsForm aiToolsForm, ObjectiveBaseModel objectiveModel)
        {
            int fitProgress = 0;
            int totalFitProgress = rowsList.Count;
            string modelName = objectiveModel.ModelName + objectiveModel.ObjectiveName;

            // Get previously learned data
            Dictionary<string, List<Sample>> trainingSamplesListsDict = GetPreviousTrainingSamples(rowsList);

            List<Thread> segmentsFitThreads = new List<Thread>();
            foreach (DataRow row in rowsList)
            {
                // Check if current signal is already learned before
                string signalDataKey = row.Field<string>("sginal_name") + row.Field<long>("starting_index");
                if (trainingSamplesListsDict.ContainsKey(signalDataKey))
                    UpdateFitProgressBar(aiToolsForm, modelName, ref fitProgress, totalFitProgress);
                else
                {
                    // Learn the new signal
                    int samplingRate = (int)(row.Field<long>("sampling_rate"));
                    int startingIndex = (int)(row.Field<long>("starting_index") * samplingRate);
                    double[] signalSamples = GeneralTools.ByteArrayToObject<double[]>(row.Field<byte[]>("signal_data"));
                    AnnotationData annoData = GeneralTools.ByteArrayToObject<AnnotationData>(row.Field<byte[]>("anno_data"));

                    // Train on 10 seconds segments of each 5 minutes from the original signal
                    List<(double[] segmentSamples, AnnotationData segmentAnnos)> representanteSegments = GetRepresentanteSegments(signalSamples, samplingRate, annoData);

                    // Thread for the selected signal
                    Thread segmentFitThread = new Thread(() =>
                    {
                        // Combine all the training samples of each segment of the signal in newSignalData
                        Data newSignalData = null;
                        List<Sample> totalTrainSamples = new List<Sample>();

                        // Fit each segment from representanteSegments in a different thread
                        List<Thread> innerFitThreads = new List<Thread>();
                        foreach ((double[] segmentSamples, AnnotationData segmentAnnos) in representanteSegments)
                        {
                            // Thread for the selected segment from the signal
                            Thread innerFitThread = new Thread(() =>
                            {
                                // Initialize the reinforcement learning environment
                                CWD_RL cwdRL = new CWD_RL(CWDCrazyReinforcementLModel._DimensionsList);
                                newSignalData = cwdRL.DeepFitRLData(segmentSamples, samplingRate, segmentAnnos, CWDCrazyReinforcementLModel);
                                totalTrainSamples.AddRange(newSignalData.Samples);
                            });
                            innerFitThread.Start();
                            innerFitThreads.Add(innerFitThread);
                        }
                        foreach (Thread t in innerFitThreads)
                            t.Join();

                        newSignalData.Samples = totalTrainSamples;
                        // Append the new siganl data into trainingSamplesList
                        trainingSamplesListsDict.Add(signalDataKey, newSignalData.Samples);

                        // Save the new signal data into cwd_rl_dataset
                        DbStimulator dbStimulator = new DbStimulator();
                        dbStimulator.Insert("cwd_rl_dataset",
                                                new string[] { "sginal_data_key", "training_data" },
                                                new object[] { signalDataKey, GeneralTools.ObjectToByteArray(newSignalData) },
                                                "DatasetExplorerFormForTraining_CWDReinforcementL");

                        UpdateFitProgressBar(aiToolsForm, modelName, ref fitProgress, totalFitProgress);
                    });
                    segmentFitThread.Start();
                    segmentsFitThreads.Add(segmentFitThread);
                }
            }
            foreach (Thread t in segmentsFitThreads)
                t.Join();

            // Save the crazy model
            //TF_NET_NN.SaveModelVariables(CWDCrazyReinforcementLModel.BaseModel.Session, CWDCrazyReinforcementLModel.BaseModel.ModelPath, new string[] { "output" });

            return trainingSamplesListsDict.SelectMany(dictPair => dictPair.Value).ToList();
        }

        public void holdRecordReport_CWDReinforcementL(DataTable dataTable, string callingClassName)
        {
            string modelName = _objectiveModel.ModelName + _objectiveModel.ObjectiveName;
            List<Sample> trainingSamplesList = GetTrainingSamples(dataTable.AsEnumerable().ToList(), ((CWDReinforcementL)_objectiveModel).CWDCrazyReinforcementLModel, _aIToolsForm, _objectiveModel);

            // Now fit all of trainingSamplesList into the reinforcement learning neural network model
            long datasetSize = _datasetSize + dataTable.Rows.Count;
            CWD_RL_TFNET cwdRLTFNET = new CWD_RL_TFNET(_aIToolsForm._objectivesModelsDic, _aIToolsForm);
            Thread cwdRLTFNETThread = new Thread(() => cwdRLTFNET.Fit(modelName, trainingSamplesList, datasetSize, _id));
            cwdRLTFNETThread.Start();
        }
    }
}

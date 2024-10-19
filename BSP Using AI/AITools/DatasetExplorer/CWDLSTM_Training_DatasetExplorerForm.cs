using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.RL_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.TF_NET_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using BSP_Using_AI.DetailsModify;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.ReinforcementLearning.Environment;
using static Biological_Signal_Processing_Using_AI.AITools.RL_Objectives.CWD_RL;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Filters.CornersScanner;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools.DatasetExplorer
{
    public partial class DatasetExplorerForm
    {
        public class LSTMDataBuilderMemory
        {
            public string[] FeaturesLabels = null;
            public string[] OutputsLabels = null;

            public int samplingRate;

            public double globalAmpInterval;

            public double argTanNormalizer = Math.PI;

            public int LatestClassifiedPeakIndex = 0;
            public int LatestPPeakIndex = 0;
            public int LatestRPeakIndex = 0;
            public int LatestTPeakIndex = 0;

            public int PsCount = 0;
            public int RsCount = 0;
            public int TsCount = 0;

            //_____________________________________________________________________//
            public int[] ProbingIntervals = new int[4];
        }

        public static (double opposite, double adjacent, double hypotenuse) ExtractRightTriangleBranches(double[] samples, double samplingRate)
        {
            double opposite = 0;
            double adjacent = 0;
            for (int i = 1; i < samples.Length; i++)
            {
                opposite += (samples[i] - samples[0]) / (double)(samples.Length - 1);
                adjacent += i / (double)(samples.Length - 1);
            }
            adjacent = adjacent / samplingRate;
            double hypotenuse = Math.Sqrt(Math.Pow(opposite, 2) + Math.Pow(adjacent, 2));

            return (opposite, adjacent, hypotenuse);
        }
        
        private static double[] GetSurroundingRangeFeatures(int longRangeIndex, int shortRangeIndex, int cornerIndex, LSTMDataBuilderMemory dataBuilderMemory, double[] rescaledSignalTotalSamples)
        {
            double[] xPeakPreSamples = rescaledSignalTotalSamples.Where((value, index) => (cornerIndex - longRangeIndex) <= index &&
                                                                                                      index <= cornerIndex).
                                                                  ToArray();
            xPeakPreSamples = xPeakPreSamples.Reverse().ToArray();
            double[] xPeakPostSamples = rescaledSignalTotalSamples.Where((value, index) => cornerIndex <= index &&
                                                                                                      index <= cornerIndex + longRangeIndex).
                                                                   ToArray();
            double[] xPeakPreFeatures = new double[5];
            double[] xPeakPostFeatures = new double[5];
            if (xPeakPreSamples.Length > 0)
                xPeakPreFeatures = GetSideRangeFeatures(shortRangeIndex, xPeakPreSamples, dataBuilderMemory);
            if (xPeakPostSamples.Length > 0)
                xPeakPostFeatures = GetSideRangeFeatures(shortRangeIndex, xPeakPostSamples, dataBuilderMemory);

            double[] xPeakAllFeatures = xPeakPreFeatures.Concat(xPeakPostFeatures).ToArray();

            return xPeakAllFeatures;
        }
        public static (int maxAmpIndex, int minAmpInde) GetMaxMinAmpFromTanIndecies(int shortRangeIndex, double meanTan, double[] spanSamples, double samplingRate)
        {
            int maxAmpIndex = 0;
            double maxAmpFromTan = double.MinValue;
            int minAmpIndex = 0;
            double minAmpFromTan = double.MaxValue;

            for (int i = 1; i < shortRangeIndex && i < spanSamples.Length; i++)
            {
                double valInTan = spanSamples[0] + meanTan * (i / samplingRate);
                if (spanSamples[i] - valInTan > maxAmpFromTan)
                {
                    maxAmpFromTan = spanSamples[i] - valInTan;
                    maxAmpIndex = i;
                }
                if (spanSamples[i] - valInTan < minAmpFromTan)
                {
                    minAmpFromTan = spanSamples[i] - valInTan;
                    minAmpIndex = i;
                }
            }

            return (maxAmpIndex, minAmpIndex);
        }
        private static double[] GetSideRangeFeatures(int shortRangeIndex, double[] spanSamples, LSTMDataBuilderMemory dataBuilderMemory)
        {
            (double spanOps, double spanAdja, _) = ExtractRightTriangleBranches(spanSamples, dataBuilderMemory.samplingRate);

            double spanTan = 0;
            if (spanAdja != 0)
                spanTan = spanOps / spanAdja;

            (int spanMaxAmpIndex, int spanMinAmpIndex) = GetMaxMinAmpFromTanIndecies(shortRangeIndex, spanTan, spanSamples, dataBuilderMemory.samplingRate);

            double spanMaxAmpTan = (spanSamples[spanMaxAmpIndex] - spanSamples[0]) / (spanMaxAmpIndex / (double)dataBuilderMemory.samplingRate);
            double spanMinAmpTan = (spanSamples[spanMinAmpIndex] - spanSamples[0]) / (spanMinAmpIndex / (double)dataBuilderMemory.samplingRate);
            double spanAmpIntervToGlob = (spanSamples[spanMaxAmpIndex] - spanSamples[spanMinAmpIndex]) / dataBuilderMemory.globalAmpInterval;
            double spanMaxAmpBranToGlob = Math.Sqrt(Math.Pow(spanSamples[spanMaxAmpIndex] - spanSamples[0], 2) +
                                             Math.Pow(spanMaxAmpIndex / (double)dataBuilderMemory.samplingRate, 2)) /
                                             dataBuilderMemory.globalAmpInterval;
            double spanMinAmpBranToGlob = Math.Sqrt(Math.Pow(spanSamples[spanMinAmpIndex] - spanSamples[0], 2) +
                                             Math.Pow(spanMinAmpIndex / (double)dataBuilderMemory.samplingRate, 2)) /
                                             dataBuilderMemory.globalAmpInterval;

            double spanMaxAmpAtanDiv = 0;
            double spanMinAmpAtanDiv = 0;
            if (spanTan != 0)
            {
                spanMaxAmpAtanDiv = (Math.Atan(spanMaxAmpTan) - Math.Atan(spanTan)) / dataBuilderMemory.argTanNormalizer;
                spanMinAmpAtanDiv = (Math.Atan(spanMinAmpTan) - Math.Atan(spanTan)) / dataBuilderMemory.argTanNormalizer;
            }

            return new double[] { spanMaxAmpAtanDiv, spanMinAmpAtanDiv, spanAmpIntervToGlob, spanMaxAmpBranToGlob, spanMinAmpBranToGlob };
        }

        public static double[] GetCornerFeatures(Sample sample, LSTMDataBuilderMemory dataBuilderMemory, int cornerIndex, double[] rescaledSignalTotalSamples, List<SignalSegment> segmentsList)
        {
            List<double> features = new List<double>(106);

            double[] intervalFeatures;
            //for (int iProbingInterval = 0; iProbingInterval < dataBuilderMemory.ProbingIntervals.Length; iProbingInterval++)
            for (int iLongIntervalSegm = 2; iLongIntervalSegm <= 5; iLongIntervalSegm++)
            {
                for (int iShortIntervalSegm = 1; iShortIntervalSegm < iLongIntervalSegm; iShortIntervalSegm++)
                {
                    intervalFeatures = GetSurroundingRangeFeatures(dataBuilderMemory.ProbingIntervals[0] * iLongIntervalSegm / 10,
                                                         dataBuilderMemory.ProbingIntervals[0] * iShortIntervalSegm / 10,
                                                         cornerIndex,
                                                         dataBuilderMemory,
                                                         rescaledSignalTotalSamples);
                    for (int iIntervalFeature = 0; iIntervalFeature < intervalFeatures.Length; iIntervalFeature++)
                        features.Add(intervalFeatures[iIntervalFeature]);
                }
            }

            SignalSegment cornerSegment = segmentsList.Where(segment => segment.startingIndex <= cornerIndex && cornerIndex <= segment.endingIndex).ToArray()[0];
            double segmentInterval = cornerSegment.segmentMax - cornerSegment.segmentMin;

            features.AddRange(new double[] { 0, 0, 0 });
            if (dataBuilderMemory.ProbingIntervals[1] != 0)
                features[100] = (double)(cornerIndex - dataBuilderMemory.LatestPPeakIndex) / dataBuilderMemory.ProbingIntervals[1];
            if (dataBuilderMemory.ProbingIntervals[2] != 0)
                features[101] = (double)(cornerIndex - dataBuilderMemory.LatestRPeakIndex) / dataBuilderMemory.ProbingIntervals[2];
            if (dataBuilderMemory.ProbingIntervals[3] != 0)
                features[102] = (double)(cornerIndex - dataBuilderMemory.LatestTPeakIndex) / dataBuilderMemory.ProbingIntervals[3];

            features.Add((rescaledSignalTotalSamples[cornerIndex] - cornerSegment.segmentMin) / segmentInterval);
            features.Add(rescaledSignalTotalSamples[cornerIndex] / dataBuilderMemory.globalAmpInterval);
            features.Add(segmentInterval / dataBuilderMemory.globalAmpInterval);

            if (sample != null)
                for (int iFeature = 0; iFeature < dataBuilderMemory.FeaturesLabels.Length; iFeature++)
                    sample.insertFeature(iFeature, dataBuilderMemory.FeaturesLabels[iFeature], features[iFeature]);

            return features.ToArray();
        }

        private static void UpdatePeakMemoryParams(ref int latestXPeakIndex, ref int xxIntervalAv, ref int XsCount, int newXPeakIndex)
        {
            double latestXPeakInterval = newXPeakIndex - latestXPeakIndex;
            xxIntervalAv = (int)((xxIntervalAv * XsCount + latestXPeakInterval) / (XsCount + 1));

            XsCount++;
            latestXPeakIndex = newXPeakIndex;
        }

        public static void GetOutputsOfTheSample(Sample sample, LSTMDataBuilderMemory dataBuilderMemory, List<CornerInterval> matchedIntrvsList)
        {
            foreach (CornerInterval cornerIntv in matchedIntrvsList)
            {
                for (int iOutput = 0; iOutput < dataBuilderMemory.OutputsLabels.Length; iOutput++)
                    if (dataBuilderMemory.OutputsLabels[iOutput].Equals(cornerIntv.Name))
                    {
                        sample?.insertOutput(iOutput, cornerIntv.Name, 1);

                        if (cornerIntv.Name.Equals(CWDNamigs.PeaksLabelsOutputs.PPeak))
                            UpdatePeakMemoryParams(ref dataBuilderMemory.LatestPPeakIndex, ref dataBuilderMemory.ProbingIntervals[1], ref dataBuilderMemory.PsCount, cornerIntv.cornerIndex);
                        else if (cornerIntv.Name.Equals(CWDNamigs.PeaksLabelsOutputs.RPeak))
                            UpdatePeakMemoryParams(ref dataBuilderMemory.LatestRPeakIndex, ref dataBuilderMemory.ProbingIntervals[2], ref dataBuilderMemory.RsCount, cornerIntv.cornerIndex);
                        else if (cornerIntv.Name.Equals(CWDNamigs.PeaksLabelsOutputs.TPeak))
                            UpdatePeakMemoryParams(ref dataBuilderMemory.LatestTPeakIndex, ref dataBuilderMemory.ProbingIntervals[3], ref dataBuilderMemory.TsCount, cornerIntv.cornerIndex);
                    }

                if (!cornerIntv.Name.Equals(CWDNamigs.PeaksLabelsOutputs.Other))
                    dataBuilderMemory.LatestClassifiedPeakIndex = cornerIntv.cornerIndex;
            }
        }

        public static List<List<Sample>> BuildLSTMTrainingSequences(List<DataRow> rowsList, TFNETReinforcementL rlModel)
        {
            List<List<Sample>> dataListSequences = new List<List<Sample>>(rowsList.Count);

            foreach (DataRow row in rowsList)
            {
                // Get the signal and segment it using the CWD_RL segmentation method
                int samplingRate = (int)(row.Field<long>("sampling_rate"));
                int startingIndex = (int)(row.Field<long>("starting_index") * samplingRate);
                double[] signalSamples = GeneralTools.ByteArrayToObject<double[]>(row.Field<byte[]>("signal_data"));
                AnnotationData annoData = GeneralTools.ByteArrayToObject<AnnotationData>(row.Field<byte[]>("anno_data"));

                // Rescale samples to be in an amplitude interval of 4
                double globalAmpInterval = 4d;
                double[] RescaledSamples = GeneralTools.rescaleSignal(signalSamples, globalAmpInterval);

                // Scan the corners of each segment using the corners scanner in FormDetailsModifyFilters
                (List<CornerSample> ScannedCorners, List<SignalSegment> SegmentsList) = FormDetailsModify.RLAutoCornersScanner(rlModel, signalSamples, samplingRate);
                // The parameters of the scanner are extracted using the RL model in cwdLSTM
                List<int> totalScannedCornersIndecies = ScannedCorners.Select(corner => corner._index).ToList();

                // Combine each scanned sample in totalScannedCornersIndecies to its closest annotation in approxIntervList
                AnnotationECG[] trueCorners = CWD_RL.GetCornersWithException(annoData, new string[] { CWDNamigs.Delta, CWDNamigs.Normal, CWDNamigs.Abnormal });
                // Create the intervals covering 40% from the corners in both sides
                List<CornerInterval> approxIntervList = ApproximateIndexesToIntervals(trueCorners, 40, RescaledSamples, samplingRate);
                // Sort the intervals in a dictionary with the indecies of the scanned corners
                Dictionary<int, List<CornerInterval>> sortedIntervDictBuff = approxIntervList.Select(cornerIntv => (coveredIndicies: totalScannedCornersIndecies.Where(scannedCornIndx => cornerIntv.starting <= scannedCornIndx &&
                                                                                                                                                  scannedCornIndx <= cornerIntv.ending).ToList(),
                                                                                                                cornerIntv)).
                                                                                          Select(tuple => (coveredIndiciesDist: tuple.coveredIndicies.Select(scannedCornIndx => (
                                                                                                                                                                                    Math.Sqrt(
                                                                                                                                                                                                Math.Pow((double)(scannedCornIndx - tuple.cornerIntv.cornerIndex) / samplingRate, 2) +
                                                                                                                                                                                                Math.Pow(RescaledSamples[scannedCornIndx] - RescaledSamples[tuple.cornerIntv.cornerIndex], 2)
                                                                                                                                                                                             ),
                                                                                                                                                                                    scannedCornIndx
                                                                                                                                                                                )).
                                                                                                                                                      ToList(),
                                                                                                           tuple.cornerIntv)
                                                                                                ).
                                                                                          Select(tuple => (closestIndex: tuple.coveredIndiciesDist.Count > 0 ? tuple.coveredIndiciesDist.Min().scannedCornIndx : tuple.cornerIntv.cornerIndex,
                                                                                                           tuple.cornerIntv)).
                                                                                          Select(tuple => (tuple.closestIndex, cornerIntv: new CornerInterval()
                                                                                          {
                                                                                              cornerIndex = tuple.closestIndex,
                                                                                              Name = tuple.cornerIntv.Name
                                                                                          })).
                                                                                          GroupBy(tuple => tuple.closestIndex).
                                                                                          Select(group => (group.Key, cornerIntvGroup: group.ToList().Select(group => group.cornerIntv).ToList())).
                                                                                          ToDictionary(tuple => tuple.Key, tuple => tuple.cornerIntvGroup);

                Dictionary<int, List<CornerInterval>> sortedIntervDict = totalScannedCornersIndecies.Select(scannedCornIndx => new List<CornerInterval>() { new CornerInterval() { cornerIndex = scannedCornIndx,
                                                                                                                                                                                   Name = CWDNamigs.PeaksLabelsOutputs.Other } }).
                                                                                                     ToDictionary(cornerIntvList => cornerIntvList[0].cornerIndex);

                // Combine other corners with the true ones
                foreach (int intervIndex in sortedIntervDictBuff.Keys)
                    if (sortedIntervDict.ContainsKey(intervIndex))
                        sortedIntervDict[intervIndex] = sortedIntervDictBuff[intervIndex];
                    else
                        sortedIntervDict.Add(intervIndex, sortedIntervDictBuff[intervIndex]);

                sortedIntervDict = sortedIntervDict.OrderBy(keyVal => keyVal.Key).ToDictionary(keyVal => keyVal.Key, keyVal => keyVal.Value);

                // Create the training samples

                // Create the data list sequence for the current signal
                LSTMDataBuilderMemory dataBuilderMemory = new LSTMDataBuilderMemory();
                Data CornersScanData = new Data(CWDNamigs.LSTMPeaksClassificationData);

                dataBuilderMemory.OutputsLabels = CWDNamigs.PeaksLabelsOutputs.GetNames();
                //dataBuilderMemory.FeaturesLabels = CWDNamigs.PeaksLabelsFeatures.GetNames();
                dataBuilderMemory.FeaturesLabels = new string[106];
                for (int i = 0; i < 106; i++)
                    dataBuilderMemory.FeaturesLabels[i] = "Feature " + i;
                dataBuilderMemory.samplingRate = samplingRate;
                dataBuilderMemory.ProbingIntervals[0] = samplingRate;
                dataBuilderMemory.globalAmpInterval = globalAmpInterval;

                foreach (int cornerIndex in sortedIntervDict.Keys)
                {
                    // Create a new sample
                    Sample sample = new Sample("Peak" + cornerIndex, dataBuilderMemory.FeaturesLabels.Length, dataBuilderMemory.OutputsLabels.Length, CornersScanData);

                    sample.AdditionalInfo = new Dictionary<object, object>(3);
                    sample.AdditionalInfo.Add(CWDNamigs.peakIndex, cornerIndex);
                    sample.AdditionalInfo.Add(CWDNamigs.samplingRate, samplingRate);

                    // Set features of the corner before the outputs (the outputs updates dataBuilderMemory for the next corner)
                    GetCornerFeatures(sample, dataBuilderMemory, cornerIndex, RescaledSamples, SegmentsList);

                    // Set the output
                    GetOutputsOfTheSample(sample, dataBuilderMemory, sortedIntervDict[cornerIndex]);
                }

                //_______________________________________________________________________________________________________//
                // Append the data list sequence for the current signal into dataListSequences
                dataListSequences.Add(CornersScanData.Samples);
            }

            return dataListSequences;
        }

        public void holdRecordReport_CWDLSTM(DataTable dataTable, string callingClassName)
        {
            string modelName = _objectiveModel.ModelName + _objectiveModel.ObjectiveName;

            CWD_TF_NET_LSTM cwdLSTM = new CWD_TF_NET_LSTM(_aIToolsForm._objectivesModelsDic, _aIToolsForm);

            // Check if there is any other deep Q-learning "CWDReinforcementLModel" model with the same training data as the current one
            bool rlModelCopied = false;
            foreach (ObjectiveBaseModel objectiveBaseModel in _aIToolsForm._objectivesModelsDic.Values)
                // The model exists only in CWDReinforcementL or CWDLSTM objects and should not be the model being trained
                if ((objectiveBaseModel is CWDReinforcementL || objectiveBaseModel is CWDLSTM) && objectiveBaseModel != _objectiveModel)
                    // The training data could be checked using the samples ids in DataIdsIntervalsList
                    if (objectiveBaseModel.DataIdsIntervalsList.Count == _objectiveModel.DataIdsIntervalsList.Count)
                    {
                        bool copyModel = true;
                        for (int iList = 0; iList < objectiveBaseModel.DataIdsIntervalsList.Count; iList++)
                        {
                            // Sort the samples ids intervals
                            List<IdInterval> intervalsList = objectiveBaseModel.DataIdsIntervalsList[iList];
                            List<IdInterval> trainingIntervalsList = _objectiveModel.DataIdsIntervalsList[iList];

                            // Check if there is any non similarities between the training datasets
                            if (intervalsList.Count == trainingIntervalsList.Count)
                            {
                                intervalsList.Sort((interval1, interval2) => { return interval1.starting.CompareTo(interval2.starting); });
                                trainingIntervalsList.Sort((interval1, interval2) => { return interval1.starting.CompareTo(interval2.starting); });
                                for (int iInterval = 0; iInterval < intervalsList.Count; iInterval++)
                                    if (intervalsList[iInterval] != trainingIntervalsList[iInterval])
                                    {
                                        copyModel = false;
                                        break;
                                    }
                            }
                            else
                            {
                                copyModel = false;
                                break;
                            }
                            if (!copyModel)
                                break;
                        }

                        if (copyModel)
                        {
                            // Copy the model to the current model being trained
                            TFNETReinforcementL CWDReinforcementLModel = null;
                            TFNETReinforcementL TariningCWDReinforcementLModel = ((CWDLSTM)_objectiveModel).CWDReinforcementLModel;
                            if (objectiveBaseModel is CWDReinforcementL rlBaseModel)
                                CWDReinforcementLModel = rlBaseModel.CWDReinforcementLModel;
                            else if (objectiveBaseModel is CWDLSTM lstmBaseModel)
                                CWDReinforcementLModel = lstmBaseModel.CWDReinforcementLModel;

                            TFNETBaseModel baseModel = CWDReinforcementLModel.BaseModel;
                            TariningCWDReinforcementLModel.BaseModel.Session = TF_NET_NN.LoadModelVariables(baseModel, CWD_RL_TFNET.createTFNETNeuralNetModelSession);
                            TF_NET_NN.SaveModelVariables(TariningCWDReinforcementLModel.BaseModel.Session, TariningCWDReinforcementLModel.BaseModel.ModelPath, new string[] { "output" });

                            rlModelCopied = true;
                            break;
                        }
                    }

            // Get deep Q-learning model's training dataset if available
            List<Sample> rlTrainingSamplesList = new List<Sample>();
            if (!rlModelCopied)
                rlTrainingSamplesList = GetTrainingSamples(dataTable.AsEnumerable().ToList(), ((CWDLSTM)_objectiveModel).CWDCrazyReinforcementLModel, _aIToolsForm, _objectiveModel);

            // Train the deep Q-learning model
            cwdLSTM.FitOnRLModel(modelName, rlTrainingSamplesList);
            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
            //**************************************************************************************************************//

            // Build LSTM training sequences
            List<List<Sample>> dataListSequences = BuildLSTMTrainingSequences(dataTable.AsEnumerable().ToList(), ((CWDLSTM)_objectiveModel).CWDReinforcementLModel);

            // Train the LSTM model
            long datasetSize = _datasetSize + dataTable.Rows.Count;
            cwdLSTM.FitOnLSTMModel(modelName, dataListSequences, datasetSize, _id);
        }
    }
}

using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.AITools.RL_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.TF_NET_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.RL_Objectives.CWD_RL;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.DetailsModify.FormDetailsModify.CornersScanner;

namespace BSP_Using_AI.AITools.DatasetExplorer
{
    public partial class DatasetExplorerForm
    {
        public class LSTMDataBuilderMemory
        {
            public int samplingRate;

            public CornerSample LatestPeak = null;
            public CornerSample LatestClassifiedPeak = null;
            public CornerSample LatestPPeak = null;
            public CornerSample LatestRPeak = null;

            public int latestPeakToClassifyIndx = -1;
            public string latestPeakToClassifyName;
            public double[] latestPeakToClassifyProba;

            public bool currentPeakIsP = false;
            public bool currentPeakIsR = false;

            public int PsCount = 0;
            public int RsCount = 0;

            public double latestPPInterval;
            public double latestRRInterval;

            public double ppIntervalAv;
            public double rrIntervalAv;

            public double globalMin;
            public double segmentMin;

            public double globalAmpInterval;
            public double segmentAmpInterval;
        }

        public double[] GetLabelsOfTheSample(LSTMDataBuilderMemory dataBuilderMemory, List<CornerInterval> matchedIntervalsList)
        {
            // There are 12 classes as output from the LSTM model
            double[] labels = new double[12];

            if (matchedIntervalsList.Count > 0)
            {
                foreach (CornerInterval cornInterval in matchedIntervalsList)
                    if (cornInterval.Name.Equals(CWDNamigs.POnset))
                        labels[0] = 1;
                    else if (cornInterval.Name.Equals(CWDNamigs.PPeak))
                    {
                        labels[1] = 1;
                        dataBuilderMemory.currentPeakIsP = true;
                    }
                    else if (cornInterval.Name.Equals(CWDNamigs.PEnd))
                        labels[2] = 1;
                    else if (cornInterval.Name.Equals(CWDNamigs.QPeak))
                        labels[3] = 1;
                    else if (cornInterval.Name.Equals(CWDNamigs.RPeak))
                    {
                        labels[4] = 1;
                        dataBuilderMemory.currentPeakIsR = true;
                    }
                    else if (cornInterval.Name.Equals(CWDNamigs.SPeak))
                        labels[5] = 1;
                    else if (cornInterval.Name.Equals(CWDNamigs.TOnset))
                        labels[6] = 1;
                    else if (cornInterval.Name.Equals(CWDNamigs.TPeak))
                        labels[7] = 1;
                    else if (cornInterval.Name.Equals(CWDNamigs.TEnd))
                        labels[8] = 1;
                    else if (cornInterval.Name.Equals(CWDNamigs.Other))
                        labels[9] = 1;
                    else if (cornInterval.Name.Equals(CWDNamigs.Normal))
                        labels[10] = 1;
                    else if (cornInterval.Name.Equals(CWDNamigs.Abnormal))
                        labels[11] = 1;
            }
            else
            {
                // Set the peak as "other" and "normal"
                labels[9] = 1;
                labels[10] = 1;
            }

            return labels;
        }

        public static double[] GetFeaturesOfTheSample(Sample sample, LSTMDataBuilderMemory dataBuilderMemory, CornerSample scannedCorner)
        {
            double[] features = new double[7];

            double tanLastPeak = 0;
            double tanLastClassPeak = 0;
            double ppIntervRatio = 0;
            double rrIntervRatio = 0;
            double segmPeakAmpRatio = 0;
            double globPeakAmpRatio = 0;
            double segmGlobAmpRatio = 0;

            if (dataBuilderMemory.LatestPeak != null)
                tanLastPeak = (scannedCorner._value - dataBuilderMemory.LatestPeak._value) / ((scannedCorner._index - dataBuilderMemory.LatestPeak._index) / (double)dataBuilderMemory.samplingRate);
            if (dataBuilderMemory.LatestClassifiedPeak != null)
                tanLastClassPeak = (scannedCorner._value - dataBuilderMemory.LatestClassifiedPeak._value) / ((scannedCorner._index - dataBuilderMemory.LatestClassifiedPeak._index) / (double)dataBuilderMemory.samplingRate);

            if (dataBuilderMemory.ppIntervalAv > 0)
                ppIntervRatio = (double)(scannedCorner._index - dataBuilderMemory.LatestPPeak._index) / dataBuilderMemory.ppIntervalAv;
            if (dataBuilderMemory.rrIntervalAv > 0)
                rrIntervRatio = (double)(scannedCorner._index - dataBuilderMemory.LatestRPeak._index) / dataBuilderMemory.rrIntervalAv;

            segmPeakAmpRatio = (scannedCorner._value - dataBuilderMemory.segmentMin) / dataBuilderMemory.segmentAmpInterval;
            globPeakAmpRatio = (scannedCorner._value - dataBuilderMemory.globalMin) / dataBuilderMemory.globalAmpInterval;
            segmGlobAmpRatio = dataBuilderMemory.segmentAmpInterval / dataBuilderMemory.globalAmpInterval;

            features[0] = Math.Atan(tanLastPeak) / (Math.PI * 2);
            features[1] = Math.Atan(tanLastClassPeak) / (Math.PI * 2);
            features[2] = ppIntervRatio;
            features[3] = rrIntervRatio;
            features[4] = segmPeakAmpRatio;
            features[5] = globPeakAmpRatio;
            features[6] = segmGlobAmpRatio;

            sample?.insertFeature(0, CWDNamigs.ArgTanLastPeak, features[0]);
            sample?.insertFeature(1, CWDNamigs.ArgTanLastClassPeak, features[1]);
            sample?.insertFeature(2, CWDNamigs.PPIntervRatio, features[2]);
            sample?.insertFeature(3, CWDNamigs.RRIntervRatio, features[3]);
            sample?.insertFeature(4, CWDNamigs.SegmPeakAmpRatio, features[4]);
            sample?.insertFeature(5, CWDNamigs.GlobPeakAmpRatio, features[5]);
            sample?.insertFeature(6, CWDNamigs.SegmGlobAmpRatio, features[6]);

            return features;
        }

        private void UpdateDatasetSample(ref Sample sample, Data CornersScanData, LSTMDataBuilderMemory dataBuilderMemory, CornerSample scannedCorner, List<CornerInterval> matchedIntervalsList, bool updateTheSample)
        {
            if (!updateTheSample)
            {
                sample = new Sample("Peak" + scannedCorner._index, 7, 12, CornersScanData);
            }

            // Get the label of the selected scanned corner
            double[] labels = GetLabelsOfTheSample(dataBuilderMemory, matchedIntervalsList);
            string[] labelsNamings = new string[] { CWDNamigs.POnset, CWDNamigs.PPeak, CWDNamigs.PEnd, CWDNamigs.QPeak, CWDNamigs.RPeak, CWDNamigs.SPeak,
                                                    CWDNamigs.TOnset, CWDNamigs.TPeak, CWDNamigs.TEnd, CWDNamigs.Other, CWDNamigs.Normal, CWDNamigs.Abnormal };

            GetFeaturesOfTheSample(sample, dataBuilderMemory, scannedCorner);
            sample.insertOutputArray(labelsNamings, labels);
        }

        public void holdRecordReport_CWDLSTM(DataTable dataTable, string callingClassName)
        {
            string modelName = _objectiveModel.ModelName + _objectiveModel.ObjectiveName;

            CWD_TF_NET_LSTM cwdLSTM = new CWD_TF_NET_LSTM(_aIToolsForm._objectivesModelsDic, _aIToolsForm);

            // Check if there is any other deep Q-learning "CWDReinforcementLModel" model with the same training data as the current one
            bool rlModelCopied = false;
            foreach(ObjectiveBaseModel objectiveBaseModel in _aIToolsForm._objectivesModelsDic.Values)
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

                            TariningCWDReinforcementLModel.BaseModel.Session = TF_NET_NN.LoadModelVariables(CWDReinforcementLModel.BaseModel.ModelPath, "input_place_holder:0", "output:0", CWD_RL_TFNET.createTFNETNeuralNetModelSession);
                            TF_NET_NN.SaveModelVariables(TariningCWDReinforcementLModel.BaseModel.Session, TariningCWDReinforcementLModel.BaseModel.ModelPath, new string[] { "output" });

                            rlModelCopied = true;
                            break;
                        }
                    }

            // Get deep Q-learning model's training dataset if available
            List<Sample> rlTrainingSamplesList = new List<Sample>();
            if (!rlModelCopied)
                rlTrainingSamplesList = GetTrainingSamples(dataTable);

            // Train the deep Q-learning model
            cwdLSTM.FitOnRLModel(modelName, rlTrainingSamplesList);
            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
            //**************************************************************************************************************//

            // Build LSTM training sequences
            List<List<Sample>> dataListSequences = new List<List<Sample>>(dataTable.Rows.Count);

            foreach (DataRow row in dataTable.AsEnumerable())
            {
                // Get the signal and segment it using the CWD_RL segmentation method
                int samplingRate = (int)(row.Field<long>("sampling_rate"));
                int startingIndex = (int)(row.Field<long>("starting_index") * samplingRate);
                double[] signalSamples = GeneralTools.ByteArrayToObject<double[]>(row.Field<byte[]>("signal_data"));
                AnnotationData annoData = GeneralTools.ByteArrayToObject<AnnotationData>(row.Field<byte[]>("anno_data"));

                // Rescale samples to be in an amplitude interval of 4
                double globalAmpInterval = 4d;
                double[] RescaledSamples = GeneralTools.rescaleSignal(signalSamples, globalAmpInterval);

                List<SignalSegment> segmentsList = CWD_RL.SegmentTheMainSamples(RescaledSamples, samplingRate, 0.002d, 0.5d);

                // Scan the corners of each segment using the corners scanner in FormDetailsModifyFilters
                // The parameters of the scanner are extracted using the RL model in cwdLSTM
                Dictionary<int, (CornerSample corner, Sample dataSample)> scannedCornersDict = new Dictionary<int, (CornerSample, Sample)>(annoData.GetAnnotations().Count);
                AnnotationECG[] trueCorners = CWD_RL.GetCornersExceptDleta(annoData);
                // Create the intervals covering 40% from the corners in both sides
                List<CornerInterval> approxIntervList = ApproximateIndexesToIntervals(trueCorners, 40, RescaledSamples, samplingRate);

                // Create the data list sequence for the current signal
                LSTMDataBuilderMemory dataBuilderMemory = new LSTMDataBuilderMemory();
                List<Sample> cornersSequence = new List<Sample>(scannedCornersDict.Count);
                Data CornersScanData = new Data(CWDNamigs.LSTMPeaksClassificationData);
                Sample sample = null;

                // Update dataBuilderMemory with the global samples
                dataBuilderMemory.globalMin = GeneralTools.MeanMinMax(RescaledSamples).min;
                dataBuilderMemory.globalAmpInterval = globalAmpInterval;
                dataBuilderMemory.samplingRate = samplingRate;

                foreach (SignalSegment segment in segmentsList)
                {
                    (double globalMean, double globalStdDev, double globalIQR, double segmentMean,
                     double segmentMin, double segmentMax, double segmentStdDev, double segmentIQR) = GetFeaturesValues(RescaledSamples, segment);
                    double[] features = new double[] { globalMean, globalStdDev, globalIQR, segmentMean, segmentMin, segmentMax, segmentStdDev, segmentIQR };

                    double[] atARTOutput = TF_NET_NN.predict(features, ((CWDLSTM)_objectiveModel).CWDReinforcementLModel, ((CWDLSTM)_objectiveModel).CWDReinforcementLModel.BaseModel.Session);

                    List<CornerSample> tempCornersList = ScanCorners(segment.SegmentSamples, segment.startingIndex, samplingRate, atARTOutput[1], atARTOutput[0] * 360d);

                    // Update dataBuilderMemory with the segment samples
                    (_, double rescSegmentMin, double rescSegmentMax) = GeneralTools.MeanMinMax(segment.SegmentSamples);
                    dataBuilderMemory.segmentMin = rescSegmentMin;
                    dataBuilderMemory.segmentAmpInterval = rescSegmentMax - rescSegmentMin;

                    // There should be no double true corners
                    foreach (CornerSample scannedCorner in tempCornersList)
                    {
                        // The new corner should be after the latest peak
                        if (dataBuilderMemory.LatestPeak != null)
                            if (scannedCorner._index <= dataBuilderMemory.LatestPeak._index)
                                continue;

                        // Get the interval where this scannedCornIndx is belonging to
                        List<CornerInterval> matchedIntervalsList = approxIntervList.Where(interval => interval.starting <= scannedCorner._index && scannedCorner._index <= interval.ending).ToList();

                        // Check if we have a true corner close to the current scanned one
                        if (matchedIntervalsList.Count > 0)
                        {
                            CornerInterval cornInterval = matchedIntervalsList[0];
                            // If this corner is already included in scannedCornersDict
                            // then check if this scanned corner is closer to the true one
                            if (scannedCornersDict.ContainsKey(cornInterval.cornerIndex))
                            {
                                // Compute the amplitude between the real corner and the new scanned one vs the real corner and the previous selected one
                                double newDistAamp = Math.Sqrt(Math.Pow((cornInterval.cornerIndex - scannedCorner._index) / (double)samplingRate, 2) + Math.Pow(RescaledSamples[cornInterval.cornerIndex] - scannedCorner._value, 2));
                                double prevDistAamp = Math.Sqrt(Math.Pow((cornInterval.cornerIndex - scannedCornersDict[cornInterval.cornerIndex].corner._index) / (double)samplingRate, 2) + Math.Pow(RescaledSamples[cornInterval.cornerIndex] - scannedCornersDict[cornInterval.cornerIndex].corner._value, 2));
                                if (newDistAamp < prevDistAamp)
                                {
                                    // If yes then set the previous corner as "Other"
                                    for (int iOutput = 0; iOutput < 9; iOutput++) // 3 for P, 3 for QRS, and 3 for T
                                        scannedCornersDict[cornInterval.cornerIndex].dataSample.UpdateOutput(iOutput, 0);
                                    scannedCornersDict[cornInterval.cornerIndex].dataSample.UpdateOutput(9, 1); // The 9th index is for "Other" peaks
                                    // Create a dataset sequence sample for the current scanned corner
                                    UpdateDatasetSample(ref sample, CornersScanData, dataBuilderMemory, scannedCorner, matchedIntervalsList, false);
                                    // Swap the previous corner in the dictionary with the new one
                                    scannedCornersDict[cornInterval.cornerIndex] = (scannedCorner, sample);
                                }
                                else
                                {
                                    // Add the new scanned corner as "Other"
                                    // Create a dataset sequence sample for the current scanned corner
                                    matchedIntervalsList = matchedIntervalsList.Where(interval => interval.Name == CWDNamigs.Normal || interval.Name == CWDNamigs.Abnormal).ToList();
                                    matchedIntervalsList.Add(new CornerInterval() { Name = CWDNamigs.Other });
                                    UpdateDatasetSample(ref sample, CornersScanData, dataBuilderMemory, scannedCorner, matchedIntervalsList, false);
                                }
                            }
                            else
                            {
                                // Update the latest classified peak
                                if (dataBuilderMemory.latestPeakToClassifyIndx >= 0)
                                    dataBuilderMemory.LatestClassifiedPeak = scannedCornersDict[dataBuilderMemory.latestPeakToClassifyIndx].corner.Clone();
                                dataBuilderMemory.latestPeakToClassifyIndx = cornInterval.cornerIndex;
                                // Update LatestPPeak and LatestRPeak with their averaged interval
                                if (dataBuilderMemory.currentPeakIsP)
                                {
                                    if (dataBuilderMemory.PsCount > 0)
                                    {
                                        dataBuilderMemory.latestPPInterval = dataBuilderMemory.LatestClassifiedPeak._index - dataBuilderMemory.LatestPPeak._index;
                                        dataBuilderMemory.ppIntervalAv = ((dataBuilderMemory.ppIntervalAv * dataBuilderMemory.PsCount) + dataBuilderMemory.latestPPInterval)
                                                                         / (double)(dataBuilderMemory.PsCount + 1);
                                    }
                                    dataBuilderMemory.PsCount++;

                                    dataBuilderMemory.LatestPPeak = dataBuilderMemory.LatestClassifiedPeak.Clone();
                                    dataBuilderMemory.currentPeakIsP = false;
                                }
                                if (dataBuilderMemory.currentPeakIsR)
                                {
                                    if (dataBuilderMemory.RsCount > 0)
                                    {
                                        dataBuilderMemory.latestRRInterval = dataBuilderMemory.LatestClassifiedPeak._index - dataBuilderMemory.LatestRPeak._index;
                                        dataBuilderMemory.rrIntervalAv = ((dataBuilderMemory.rrIntervalAv * dataBuilderMemory.RsCount) + dataBuilderMemory.latestRRInterval)
                                                                         / (double)(dataBuilderMemory.RsCount + 1);
                                    }
                                    dataBuilderMemory.RsCount++;

                                    dataBuilderMemory.LatestRPeak = dataBuilderMemory.LatestClassifiedPeak.Clone();
                                    dataBuilderMemory.currentPeakIsR = false;
                                }
                                // Create a dataset sequence sample for the current scanned corner
                                UpdateDatasetSample(ref sample, CornersScanData, dataBuilderMemory, scannedCorner, matchedIntervalsList, false);
                                // Add the new true corner
                                scannedCornersDict.Add(cornInterval.cornerIndex, (scannedCorner, sample));
                            }
                        }
                        else
                        {
                            // Create a dataset sequence sample for the current scanned corner
                            UpdateDatasetSample(ref sample, CornersScanData, dataBuilderMemory, scannedCorner, matchedIntervalsList, false);
                            // Otherwise just add the corner as a regular one
                            scannedCornersDict.Add(scannedCorner._index, (scannedCorner, sample));
                        }

                        dataBuilderMemory.LatestPeak = scannedCorner.Clone();
                    }
                }

                //_______________________________________________________________________________________________________//
                // Append the data list sequence for the current signal into dataListSequences
                dataListSequences.Add(CornersScanData.Samples);
            }

            // Train the LSTM model
            long datasetSize = _datasetSize + dataTable.Rows.Count;
            cwdLSTM.FitOnLSTMModel(modelName, dataListSequences, datasetSize, _id);
        }
    }
}

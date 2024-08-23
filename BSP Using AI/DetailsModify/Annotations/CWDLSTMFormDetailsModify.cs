using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.AITools.DatasetExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.RL_Objectives.CWD_RL;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Filters.CornersScanner;
using static BSP_Using_AI.AITools.DatasetExplorer.DatasetExplorerForm;

namespace BSP_Using_AI.DetailsModify
{
    public partial class FormDetailsModify
    {
        private string GetAnnoNameByIndex(int annoIndex)
        {
            string annoName = "";

            if (annoIndex == 0)
                annoName = CWDNamigs.POnset;
            else if (annoIndex == 1)
                annoName = CWDNamigs.PPeak;
            else if (annoIndex == 2)
                annoName = CWDNamigs.PEnd;
            else if (annoIndex == 3)
                annoName = CWDNamigs.QPeak;
            else if (annoIndex == 4)
                annoName = CWDNamigs.RPeak;
            else if (annoIndex == 5)
                annoName = CWDNamigs.SPeak;
            else if (annoIndex == 6)
                annoName = CWDNamigs.TOnset;
            else if (annoIndex == 7)
                annoName = CWDNamigs.TPeak;
            else if (annoIndex == 8)
                annoName = CWDNamigs.TEnd;

            return annoName;
        }

        private (string cornerName, string regularity, List<int> selectedLabelsIndecies) GetLabelsOfPrediction(double[] predictionOutput, LSTMDataBuilderMemory DataBuilderMemory, OutputThresholdItem[] outputThresholds)
        {
            // Get the annotation of the prediction (the prediction should be at least higher than 0.5 for a valid annotaion)
            string cornerName = "";
            string regularity = "";
            List<int> selectedLabelsIndecies = predictionOutput.Select((proba, index) => (proba, index))
                                                               .Where(tuple => tuple.proba >= outputThresholds[tuple.index]._threshold && tuple.index < 9)
                                                               .Select(tuple => tuple.index).ToList();

            if (predictionOutput[0] >= outputThresholds[0]._threshold)
                cornerName += CWDNamigs.POnset + ", ";
            if (predictionOutput[1] >= outputThresholds[1]._threshold)
            {
                cornerName += CWDNamigs.PPeak + ", ";
                DataBuilderMemory.currentPeakIsP = true;
            }
            if (predictionOutput[2] >= outputThresholds[2]._threshold)
                cornerName += CWDNamigs.PEnd + ", ";
            if (predictionOutput[3] >= outputThresholds[3]._threshold)
                cornerName += CWDNamigs.QPeak + ", ";
            if (predictionOutput[4] >= outputThresholds[4]._threshold)
            {
                cornerName += CWDNamigs.RPeak + ", ";
                DataBuilderMemory.currentPeakIsR = true;
            }
            if (predictionOutput[5] >= outputThresholds[5]._threshold)
                cornerName += CWDNamigs.SPeak + ", ";
            if (predictionOutput[6] >= outputThresholds[6]._threshold)
                cornerName += CWDNamigs.TOnset + ", ";
            if (predictionOutput[7] >= outputThresholds[7]._threshold)
                cornerName += CWDNamigs.TPeak + ", ";
            if (predictionOutput[8] >= outputThresholds[8]._threshold)
                cornerName += CWDNamigs.TEnd + ", ";
            if (predictionOutput[9] >= outputThresholds[9]._threshold)
                cornerName += CWDNamigs.Other + ", ";

            if (predictionOutput[10] >= outputThresholds[10]._threshold)
                regularity = CWDNamigs.Normal + ", ";
            if (predictionOutput[11] >= outputThresholds[11]._threshold)
                regularity += CWDNamigs.Abnormal + ", ";

            return (cornerName, regularity, selectedLabelsIndecies);
        }

        private void predictButton_Click_CWDLSTM(string modelNameProblem)
        {
            // Get the selected model
            CWDLSTM cwdLSTM = (CWDLSTM)_objectivesModelsDic[modelNameProblem];
            TFNETReinforcementL CWDReinforcementLModel = cwdLSTM.CWDReinforcementLModel;
            TFNETLSTMModel CWDLSTMModel = cwdLSTM.CWDLSTMModel;

            // Scan for the corners using the selected deep Q-learning model with the rescaled segments list
            (List<CornerSample> scannedCorners, List<SignalSegment> rescSegmentsList) = RLAutoCornersScanner(CWDReinforcementLModel);

            // Rescale samples to be in an amplitude interval of 4
            double globalAmpInterval = 4d;
            double[] RescaledSamples = GeneralTools.rescaleSignal(_FilteringTools._FilteredSamples, globalAmpInterval);

            // Create the data builder memory for generating the input data
            LSTMDataBuilderMemory DataBuilderMemory = new LSTMDataBuilderMemory();
            // Update dataBuilderMemory with the global samples
            DataBuilderMemory.globalMin = GeneralTools.MeanMinMax(RescaledSamples).min;
            DataBuilderMemory.globalAmpInterval = globalAmpInterval;
            DataBuilderMemory.samplingRate = _FilteringTools._samplingRate;
            DataBuilderMemory.latestPeakToClassifyProba = new double[CWDLSTMModel._outputDim];

            // Create the queue for building the input sequence of the LSTM model
            Queue<double[]> InputSeqQueue = new Queue<double[]>(CWDLSTMModel._modelSequenceLength + 1);

            // Create the annotation data
            _AnnotationData = new AnnotationData(CharacteristicWavesDelineation.ObjectiveName);
            List<CornerSample> AnnotatedCornsList = new List<CornerSample>();

            foreach (SignalSegment segment in rescSegmentsList)
            {
                List<CornerSample> segScannedCornsList = scannedCorners.Where(corn => segment.startingIndex <= corn._index && corn._index <= segment.endingIndex).ToList();

                // Update dataBuilderMemory with the segment samples
                (_, double rescSegmentMin, double rescSegmentMax) = GeneralTools.MeanMinMax(segment.SegmentSamples);
                DataBuilderMemory.segmentMin = rescSegmentMin;
                DataBuilderMemory.segmentAmpInterval = rescSegmentMax - rescSegmentMin;

                // Feed each corner features into the LSTM model
                foreach (CornerSample scannedCorner in segScannedCornsList)
                {
                    // The new corner should be after the latest peak
                    if (DataBuilderMemory.LatestPeak != null)
                        if (scannedCorner._index <= DataBuilderMemory.LatestPeak._index)
                            continue;

                    // Get the next and previous corners that are in the range of 0.2 sec froward and backward
                    double nearbyCornersTempIntervalRange = 0.3d;
                    DataBuilderMemory.nextCorners = segScannedCornsList.Where(corner => Math.Abs(corner._index - scannedCorner._index) / (double)DataBuilderMemory.samplingRate <= nearbyCornersTempIntervalRange && corner._index > scannedCorner._index).ToList();
                    DataBuilderMemory.prevCorners = segScannedCornsList.Where(corner => Math.Abs(corner._index - scannedCorner._index) / (double)DataBuilderMemory.samplingRate <= nearbyCornersTempIntervalRange && corner._index < scannedCorner._index).ToList();

                    double nearbySamplesTempIntervalRange = 0.3d;
                    DataBuilderMemory.nextRescSamples = RescaledSamples.Where((value, index) => scannedCorner._index <= index && index <= scannedCorner._index + (nearbySamplesTempIntervalRange * DataBuilderMemory.samplingRate)).ToArray();
                    DataBuilderMemory.prevRescSamplesReversed = RescaledSamples.Where((value, index) => scannedCorner._index - (nearbySamplesTempIntervalRange * DataBuilderMemory.samplingRate) <= index && index <= scannedCorner._index).ToArray();
                    DataBuilderMemory.prevRescSamplesReversed = DataBuilderMemory.prevRescSamplesReversed.Reverse().ToArray();

                    // Get the features of the selected corner
                    double[] features = DatasetExplorerForm.GetFeaturesOfTheSample(null, DataBuilderMemory, scannedCorner);
                    // Enqueue the new features in InputSeqQueue
                    InputSeqQueue.Enqueue(features);
                    // Check if the queue has exceeded the sequence length of the model
                    if (InputSeqQueue.Count > CWDLSTMModel._modelSequenceLength)
                        InputSeqQueue.Dequeue();

                    // Feed the queue sequence into the LSTM model for prediction
                    List<double[]> output = TF_NET_LSTM.Predict(InputSeqQueue.ToList(), CWDLSTMModel);

                    if (output.Count == 0)
                        continue;

                    (string cornerName, string regularity, List<int> selectedLabelsIndecies) = GetLabelsOfPrediction(output[output.Count - 1], DataBuilderMemory, CWDLSTMModel.OutputsThresholds);

                    // Set the probabilities of the non selected labels at DataBuilderMemory.latestPeakToClassifyProba to 0
                    for (int iLabelIndex = 0; iLabelIndex < 9; iLabelIndex++)
                        if (!selectedLabelsIndecies.Contains(iLabelIndex))
                            DataBuilderMemory.latestPeakToClassifyProba[iLabelIndex] = 0;

                    // Iterate through each selected label in selectedLabelsIndecies
                    // and set the name of the new corner
                    string cornerNewName = "";
                    foreach (int iLabelIndex in selectedLabelsIndecies)
                    {
                        // Check if the selected label was already selected previously
                        if (DataBuilderMemory.latestPeakToClassifyProba[iLabelIndex] > 0)
                        {
                            // If yes then check if the new selection is more certain than the previous one
                            if (output[output.Count - 1][iLabelIndex] >= DataBuilderMemory.latestPeakToClassifyProba[iLabelIndex])
                            {
                                // If yes then swap the previous selection with the new one
                                // I mean like remove the label from the previous peak and add it to the new one
                                List<AnnotationECG> annosList = _AnnotationData.GetAnnotations();
                                string prevAnnoName = annosList[annosList.Count - 1].Name;
                                (int prevAnnoIndex, _) = annosList[annosList.Count - 1].GetIndexes();
                                string prevNewAnnoName = prevAnnoName.Replace(GetAnnoNameByIndex(iLabelIndex) + ", ", "");

                                if (prevNewAnnoName.Length > 0)
                                    annosList[annosList.Count - 1].SetNewVals(prevNewAnnoName, prevAnnoIndex, 0);
                                else
                                    annosList[annosList.Count - 1].Remove();

                                // Set its label into cornerNewName
                                cornerNewName += GetAnnoNameByIndex(iLabelIndex) + ", ";

                                // Update LatestPPeak and LatestRPeak with their averaged interval
                                if (DataBuilderMemory.currentPeakIsP)
                                {
                                    /*if (DataBuilderMemory.PsCount > 0)
                                        DataBuilderMemory.ppIntervalAv = ((DataBuilderMemory.ppIntervalAv * DataBuilderMemory.PsCount) + (corner._index - DataBuilderMemory.LatestPPeak._index))
                                                                         / (double)(DataBuilderMemory.PsCount + 1);
                                    DataBuilderMemory.PsCount++;*/

                                    //DataBuilderMemory.ppIntervalAv = 

                                    DataBuilderMemory.LatestPPeak = scannedCorner.Clone();
                                    DataBuilderMemory.currentPeakIsP = false;
                                }
                                if (DataBuilderMemory.currentPeakIsR)
                                {
                                    /*if (DataBuilderMemory.RsCount > 0)
                                        DataBuilderMemory.rrIntervalAv = ((DataBuilderMemory.rrIntervalAv * DataBuilderMemory.RsCount) + (corner._index - DataBuilderMemory.LatestRPeak._index))
                                                                         / (double)(DataBuilderMemory.RsCount + 1);
                                    DataBuilderMemory.RsCount++;*/

                                    DataBuilderMemory.LatestRPeak = scannedCorner.Clone();
                                    DataBuilderMemory.currentPeakIsR = false;
                                }

                                // Update its probability in DataBuilderMemory.latestPeakToClassifyProba
                                DataBuilderMemory.latestPeakToClassifyProba[iLabelIndex] = output[output.Count - 1][iLabelIndex];
                            }
                            //------------------> Maybe otherwise, just set its probability to 0 at DataBuilderMemory.latestPeakToClassifyProba
                        }
                        else
                        {
                            // Add the new label to the new peak
                            // Update LatestPPeak and LatestRPeak with their averaged interval
                            if (DataBuilderMemory.currentPeakIsP)
                            {
                                if (DataBuilderMemory.PsCount > 0)
                                    DataBuilderMemory.ppIntervalAv = ((DataBuilderMemory.ppIntervalAv * DataBuilderMemory.PsCount) + (scannedCorner._index - DataBuilderMemory.LatestPPeak._index))
                                                                     / (double)(DataBuilderMemory.PsCount + 1);
                                DataBuilderMemory.PsCount++;

                                DataBuilderMemory.LatestPPeak = scannedCorner.Clone();
                                DataBuilderMemory.currentPeakIsP = false;
                            }
                            if (DataBuilderMemory.currentPeakIsR)
                            {
                                if (DataBuilderMemory.RsCount > 0)
                                    DataBuilderMemory.rrIntervalAv = ((DataBuilderMemory.rrIntervalAv * DataBuilderMemory.RsCount) + (scannedCorner._index - DataBuilderMemory.LatestRPeak._index))
                                                                     / (double)(DataBuilderMemory.RsCount + 1);
                                DataBuilderMemory.RsCount++;

                                DataBuilderMemory.LatestRPeak = scannedCorner.Clone();
                                DataBuilderMemory.currentPeakIsR = false;
                            }

                            // Set its label into cornerNewName
                            cornerNewName += GetAnnoNameByIndex(iLabelIndex) + ", ";

                            // Add its probability in DataBuilderMemory.latestPeakToClassifyProba
                            DataBuilderMemory.latestPeakToClassifyProba[iLabelIndex] = output[output.Count - 1][iLabelIndex];
                        }
                    }

                    // Update the latest classified peak and add its annotation if exists
                    if (selectedLabelsIndecies.Count > 0 && cornerNewName.Length > 0)
                    {
                        DataBuilderMemory.LatestClassifiedPeak = scannedCorner.Clone();

                        _AnnotationData.InsertAnnotation(cornerNewName, AnnotationType.Point, scannedCorner._index, 0);
                    }                        

                    DataBuilderMemory.LatestPeak = scannedCorner.Clone();
                }
            }

            // Show the annotation in the chart
            UpdatePointsAnnoPlot();

            predictionEnd();
        }
    }
}

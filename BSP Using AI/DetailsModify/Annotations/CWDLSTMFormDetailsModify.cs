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
using static BSP_Using_AI.AITools.DatasetExplorer.DatasetExplorerForm;
using static BSP_Using_AI.DetailsModify.FormDetailsModify.CornersScanner;

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

        private (string cornerName, string regularity, List<int> selectedLabelsIndecies) GetLabelsOfPrediction(double[] predictionOutput, LSTMDataBuilderMemory DataBuilderMemory)
        {
            // Get the annotation of the prediction (the prediction should be at least higher than 0.5 for a valid annotaion)
            string cornerName = "";
            string regularity = "";
            List<int> selectedLabelsIndecies = predictionOutput.Select((proba, index) => (proba, index))
                                                               .Where(tuple => tuple.proba >= 0.5d && tuple.index < 9)
                                                               .Select(tuple => tuple.index).ToList();

            if (predictionOutput[0] >= 0.5d)
                cornerName += CWDNamigs.POnset + ", ";
            if (predictionOutput[1] >= 0.5d)
            {
                cornerName = CWDNamigs.PPeak + ", ";
                DataBuilderMemory.currentPeakIsP = true;
            }
            if (predictionOutput[2] >= 0.5d)
                cornerName = CWDNamigs.PEnd + ", ";
            if (predictionOutput[3] >= 0.5d)
                cornerName = CWDNamigs.QPeak + ", ";
            if (predictionOutput[4] >= 0.5d)
            {
                cornerName = CWDNamigs.RPeak + ", ";
                DataBuilderMemory.currentPeakIsR = true;
            }
            if (predictionOutput[5] >= 0.5d)
                cornerName = CWDNamigs.SPeak + ", ";
            if (predictionOutput[6] >= 0.5d)
                cornerName = CWDNamigs.TOnset + ", ";
            if (predictionOutput[7] >= 0.5d)
                cornerName = CWDNamigs.TPeak + ", ";
            if (predictionOutput[8] >= 0.5d)
                cornerName = CWDNamigs.TEnd + ", ";
            if (predictionOutput[9] >= 0.5d)
                cornerName = CWDNamigs.Other + ", ";

            if (predictionOutput[10] >= 0.5d)
                regularity = CWDNamigs.Normal + ", ";
            if (predictionOutput[11] >= 0.5d)
                regularity = CWDNamigs.Abnormal + ", ";

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

            // Create the queue for building the input sequence of the LSTM model
            Queue<double[]> InputSeqQueue = new Queue<double[]>(CWDLSTMModel._modelSequenceLength + 1);

            // Create the annotation data
            _AnnotationData = new AnnotationData(CharacteristicWavesDelineation.ObjectiveName);
            List<CornerSample> AnnotatedCornsList = new List<CornerSample>();

            foreach (SignalSegment segment in rescSegmentsList)
            {
                List<CornerSample> segScannedCornsList = scannedCorners.Where(corn => segment.startingIndex <= corn._index && corn._index <= segment.endingIndex).ToList();

                // Feed each corner features into the LSTM model
                foreach (CornerSample corner in segScannedCornsList)
                {
                    // Update dataBuilderMemory with the segment samples
                    (_, double rescSegmentMin, double rescSegmentMax) = GeneralTools.MeanMinMax(segment.SegmentSamples);
                    DataBuilderMemory.segmentMin = rescSegmentMin;
                    DataBuilderMemory.segmentAmpInterval = rescSegmentMax - rescSegmentMin;

                    // Get the features of the selected corner
                    double[] features = DatasetExplorerForm.GetFeaturesOfTheSample(null, DataBuilderMemory, corner);
                    // Enqueue the new features in InputSeqQueue
                    InputSeqQueue.Enqueue(features);
                    // Check if the queue has exceeded the sequence length of the model
                    if (InputSeqQueue.Count > CWDLSTMModel._modelSequenceLength)
                        InputSeqQueue.Dequeue();

                    // Feed the queue sequence into the LSTM model for prediction
                    List<double[]> output = TF_NET_LSTM.Predict(InputSeqQueue.ToList(), CWDLSTMModel);

                    if (output.Count == 0)
                        continue;

                    (string cornerName, string regularity, List<int> selectedLabelsIndecies) = GetLabelsOfPrediction(output[output.Count - 1], DataBuilderMemory);

                    // Check if the corner is not classified as Other
                    if (!cornerName.Contains(CWDNamigs.Other))
                    {
                        // Check if current predicted corner is different than the latestPeakToClassify
                        if (!cornerName.Equals(DataBuilderMemory.latestPeakToClassifyName))
                        {
                            // Update the latest classified peak
                            if (AnnotatedCornsList.Count > 0)
                                DataBuilderMemory.LatestClassifiedPeak = AnnotatedCornsList[AnnotatedCornsList.Count - 1].Clone();
                            // Update LatestPPeak and LatestRPeak with their averaged interval
                            if (DataBuilderMemory.currentPeakIsP)
                            {
                                if (DataBuilderMemory.PsCount > 0)
                                    DataBuilderMemory.ppIntervalAv = ((DataBuilderMemory.ppIntervalAv * DataBuilderMemory.PsCount) + (corner._index - DataBuilderMemory.LatestPPeak._index))
                                                                     / (double)(DataBuilderMemory.PsCount + 1);
                                DataBuilderMemory.PsCount++;

                                DataBuilderMemory.LatestPPeak = corner.Clone();
                                DataBuilderMemory.currentPeakIsP = false;
                            }
                            if (DataBuilderMemory.currentPeakIsR)
                            {
                                if (DataBuilderMemory.RsCount > 0)
                                    DataBuilderMemory.rrIntervalAv = ((DataBuilderMemory.rrIntervalAv * DataBuilderMemory.RsCount) + (corner._index - DataBuilderMemory.LatestRPeak._index))
                                                                     / (double)(DataBuilderMemory.RsCount + 1);
                                DataBuilderMemory.RsCount++;

                                DataBuilderMemory.LatestRPeak = corner.Clone();
                                DataBuilderMemory.currentPeakIsR = false;
                            }
                            // Update the memory data
                            AnnotatedCornsList.Add(corner);
                            DataBuilderMemory.latestPeakToClassifyName = cornerName;
                            DataBuilderMemory.latestPeakToClassifyProba = output[output.Count - 1];

                            // Add the new annotation
                            _AnnotationData.InsertAnnotation(cornerName, AnnotationType.Point, corner._index, 0);
                        }
                        // Check if the classification probability of the current corner is higher than the previous one
                        else
                        {
                            // Also check if we should split the annotation or not
                            bool keepAnno = false;
                            bool updateAnno = false;
                            string updatePrevAnnoName = "";
                            string newAnnoName = "";
                            // Iterat through the selected labels indecies
                            foreach (int labelIndex in selectedLabelsIndecies)
                            {
                                // Check if currrent prediction probability is higher than the previous one
                                if (output[output.Count - 1][labelIndex] >= DataBuilderMemory.latestPeakToClassifyProba[labelIndex])
                                {
                                    updateAnno = true;
                                    newAnnoName += GetAnnoNameByIndex(labelIndex) + ", ";

                                    // Update LatestPPeak and LatestRPeak with their averaged interval
                                    if (DataBuilderMemory.currentPeakIsP)
                                    {
                                        /*if (DataBuilderMemory.PsCount > 0)
                                            DataBuilderMemory.ppIntervalAv = ((DataBuilderMemory.ppIntervalAv * DataBuilderMemory.PsCount) + (corner._index - DataBuilderMemory.LatestPPeak._index))
                                                                             / (double)(DataBuilderMemory.PsCount + 1);
                                        DataBuilderMemory.PsCount++;*/

                                        DataBuilderMemory.LatestPPeak = corner.Clone();
                                        DataBuilderMemory.currentPeakIsP = false;
                                    }
                                    if (DataBuilderMemory.currentPeakIsR)
                                    {
                                        /*if (DataBuilderMemory.RsCount > 0)
                                            DataBuilderMemory.rrIntervalAv = ((DataBuilderMemory.rrIntervalAv * DataBuilderMemory.RsCount) + (corner._index - DataBuilderMemory.LatestRPeak._index))
                                                                             / (double)(DataBuilderMemory.RsCount + 1);
                                        DataBuilderMemory.RsCount++;*/

                                        DataBuilderMemory.LatestRPeak = corner.Clone();
                                        DataBuilderMemory.currentPeakIsR = false;
                                    }

                                    // Update the probability of the label
                                    DataBuilderMemory.latestPeakToClassifyProba[labelIndex] = output[output.Count - 1][labelIndex];
                                }
                                else
                                {
                                    keepAnno = true;
                                    updatePrevAnnoName += GetAnnoNameByIndex(labelIndex) + ", ";
                                }
                            }

                            // If we should keep some annotaions and update others then we should split the annotaions
                            List<AnnotationECG> annosList = _AnnotationData.GetAnnotations();
                            if (keepAnno && updateAnno)
                            {
                                // Update the annotation that should be kept
                                annosList[annosList.Count - 1].SetNewVals(updatePrevAnnoName, AnnotatedCornsList[AnnotatedCornsList.Count - 1]._index, 0);

                                // Add the new annotation
                                AnnotatedCornsList.Add(corner);
                                _AnnotationData.InsertAnnotation(newAnnoName, AnnotationType.Point, corner._index, 0);
                            }
                            else if (updateAnno)
                            {
                                // Update the latest peak to classify
                                AnnotatedCornsList[AnnotatedCornsList.Count - 1] = corner;
                                // Update the position of the annotation as well
                                annosList[annosList.Count - 1].SetNewVals(cornerName, corner._index, 0);
                            }
                        }
                    }
                    else
                    {
                        // This is an other peak (no annotation) then set the latest peak to classify as a classified one
                        if (AnnotatedCornsList.Count > 0)
                            DataBuilderMemory.LatestClassifiedPeak = AnnotatedCornsList[AnnotatedCornsList.Count - 1].Clone();

                        // Update the memory data
                        DataBuilderMemory.latestPeakToClassifyName = cornerName;
                        DataBuilderMemory.latestPeakToClassifyProba = output[output.Count - 1];
                    }
                        

                    DataBuilderMemory.LatestPeak = corner.Clone();
                }
            }

            // Show the annotation in the chart
            UpdatePointsAnnoPlot();

            predictionEnd();
        }
    }
}

using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.AITools.DatasetExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tensorflow;
using Tensorflow.NumPy;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.RL_Objectives.CWD_RL;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Filters.CornersScanner;
using static BSP_Using_AI.AITools.DatasetExplorer.DatasetExplorerForm;
using static BSP_Using_AI.AITools.Details.DetailsForm;

namespace BSP_Using_AI.DetailsModify
{
    public partial class FormDetailsModify
    {
        private void predictButton_Click_CWDLSTM(string modelNameProblem)
        {
            // Get the selected model
            CWDLSTM cwdLSTM = (CWDLSTM)_objectivesModelsDic[modelNameProblem];
            TFNETReinforcementL CWDReinforcementLModel = cwdLSTM.CWDReinforcementLModel;
            TFNETLSTMModel CWDLSTMModel = cwdLSTM.CWDLSTMModel;

            // Scan for the corners using the selected deep Q-learning model with the rescaled segments list
            (List<CornerSample> scannedCorners, List<SignalSegment> rescSegmentsList) = RLAutoCornersScanner(CWDReinforcementLModel, _FilteringTools._FilteredSamples, _FilteringTools._samplingRate);

            // Rescale samples to be in an amplitude interval of 4
            double globalAmpInterval = 4d;
            double[] RescaledSamples = GeneralTools.rescaleSignal(_FilteringTools._FilteredSamples, globalAmpInterval);

            // Create the data builder memory for generating the input data
            LSTMDataBuilderMemory dataBuilderMemory = new LSTMDataBuilderMemory();

            dataBuilderMemory.OutputsLabels = CWDNamigs.PeaksLabelsOutputs.GetNames();
            //dataBuilderMemory.FeaturesLabels = CWDNamigs.PeaksLabelsFeatures.GetNames();
            dataBuilderMemory.FeaturesLabels = new string[106];
            for (int i = 0; i < 106; i++)
                dataBuilderMemory.FeaturesLabels[i] = "Feature " + i;
            dataBuilderMemory.samplingRate = _FilteringTools._samplingRate;
            dataBuilderMemory.ProbingIntervals[0] = _FilteringTools._samplingRate;
            dataBuilderMemory.globalAmpInterval = globalAmpInterval;

            double[] predictedOutput;
            // The following is used for rectifying predicted outputs
            RefDouble[] predictedRefOutput;
            (RefDouble[] refOutput, int[] indecies) latestClassified = (null, new int[dataBuilderMemory.OutputsLabels.Length]);
            List<RefDouble[]> predictedRefOutputList = new List<RefDouble[]>(scannedCorners.Count);
            List<CornerInterval> matchedIntrvsList;

            // Create the queue for building the input sequence of the LSTM model
            Queue<double[]> InputSeqQueue = new Queue<double[]>(CWDLSTMModel._modelSequenceLength + 1);

            List<double[]> predictedSequenceList;
            List<NDArray> layersLatestOutputsVals = null, layersLatestStatesVals = null;
            (List<Tensor> sequenceCellsInputsPlaceHolders, List<Tensor> sequenceCellsOutputs) = TF_NET_LSTM_Recur.GetInputOutputPlaceHolders(CWDLSTMModel, CWDLSTMModel.BaseModel.Session);
            (List<Tensor> layersSartingOutputs, List<Tensor> layersStartingStates) = TF_NET_LSTM_Recur.GetLayersStartingOutputsAndStates(CWDLSTMModel);
            (List<Tensor> layersLatestOutputs, List<Tensor> layersLatestStates) = TF_NET_LSTM_Recur.GetLayersLatestOutputsAndStates(CWDLSTMModel);

            foreach (CornerSample scannedCorner in scannedCorners)
            {
                // Get the features of the selected corner
                double[] features = DatasetExplorerForm.GetCornerFeatures(null, dataBuilderMemory, scannedCorner._index, RescaledSamples, rescSegmentsList);
                // Enqueue the new features in InputSeqQueue
                InputSeqQueue.Enqueue(features);
                // Check if the queue has exceeded the sequence length of the model
                if (InputSeqQueue.Count > CWDLSTMModel._modelSequenceLength)
                    InputSeqQueue.Dequeue();

                // Feed the queue sequence into the LSTM model for prediction
                (predictedSequenceList, layersLatestOutputsVals, layersLatestStatesVals) = TF_NET_LSTM_Recur.PredictSequenciallyFast(InputSeqQueue.ToList(), CWDLSTMModel,
                                                                                  layersLatestOutputsVals, layersLatestStatesVals,
                                                                                  sequenceCellsInputsPlaceHolders, sequenceCellsOutputs,
                                                                                  layersSartingOutputs, layersStartingStates,
                                                                                  layersLatestOutputs, layersLatestStates);

                if (predictedSequenceList.Count == 0)
                    continue;
                predictedOutput = predictedSequenceList[0];

                predictedRefOutput = new RefDouble[predictedOutput.Length];
                for (int iDouble = 0; iDouble < predictedOutput.Length; iDouble++)
                    predictedRefOutput[iDouble] = new RefDouble(predictedOutput[iDouble]);

                predictedRefOutputList.Add(predictedRefOutput);

                matchedIntrvsList = new List<CornerInterval>(predictedOutput.Length);

                // Update latestClassifiedRefOutput
                if (latestClassified.refOutput == null)
                    latestClassified.refOutput = predictedRefOutput;

                for (int iPredOutput = 0; iPredOutput < predictedRefOutput.Length; iPredOutput++)
                    if (predictedRefOutput[iPredOutput].value >= CWDLSTMModel.OutputsThresholds[iPredOutput]._threshold)
                    {
                        // If this is a new peak label then set the data to update dataBuilderMemory
                        if (latestClassified.refOutput[iPredOutput].value == 0)
                            matchedIntrvsList.Add(new CornerInterval()
                            {
                                Name = dataBuilderMemory.OutputsLabels[iPredOutput],
                                cornerIndex = scannedCorner._index
                            });

                        if (predictedRefOutput[iPredOutput].value >= latestClassified.refOutput[iPredOutput].value)
                        {
                            latestClassified.refOutput[iPredOutput].value = 0;
                            latestClassified.refOutput[iPredOutput] = predictedRefOutput[iPredOutput];
                            latestClassified.indecies[iPredOutput] = scannedCorner._index;
                        }
                        else
                            predictedRefOutput[iPredOutput].value = 0;
                    }
                    else
                        latestClassified.refOutput[iPredOutput] = new RefDouble(0);

                // Update dataBuilderMemory
                DatasetExplorerForm.GetOutputsOfTheSample(null, dataBuilderMemory, matchedIntrvsList);
            }

            // Create the annotation data
            _AnnotationData = new AnnotationData(CharacteristicWavesDelineation.ObjectiveName);
            string cornerNewName;

            for (int iScannedCorner = 0; iScannedCorner < scannedCorners.Count; iScannedCorner++)
            {
                predictedRefOutput = predictedRefOutputList[iScannedCorner];
                for (int iOutput = 0; iOutput < predictedRefOutput.Length; iOutput++)
                    if (predictedRefOutput[iOutput].value >= CWDLSTMModel.OutputsThresholds[iOutput]._threshold)
                    {
                        cornerNewName = dataBuilderMemory.OutputsLabels[iOutput];
                        if (!cornerNewName.Equals(CWDNamigs.PeaksLabelsOutputs.Other))
                            _AnnotationData.InsertAnnotation(cornerNewName, AnnotationType.Point, scannedCorners[iScannedCorner]._index, 0);
                    }
            }

            // Show the annotation in the chart
            UpdatePointsAnnoPlot();

            predictionEnd();
        }
    }
}

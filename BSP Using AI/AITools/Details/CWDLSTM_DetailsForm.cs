using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.AITools.TF_NET_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.AITools.DatasetExplorer;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tensorflow;
using Tensorflow.NumPy;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.Details.ValidationDataSelection.ValDataSelectionForm;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools.Details
{
    public partial class DetailsForm
    {
        public class RefDouble
        {
            public double value;

            public RefDouble(double initVal) => value = initVal;
        }

        public delegate void ValidationReportDelegate();

        private void CWDLSTM_ValidateModel(List<ModelData> valModelsDataFolds, CWDLSTM cwdLSTM)
        {
            // Convert the annotation data samples to the deep reinforcement learning model samples
            (List<ModelSamples> valModelSamplesFolds, int totalValidationProgress) = CWDReinforcementL_GetModelSamplesFolds(valModelsDataFolds, cwdLSTM.CWDCrazyReinforcementLModel, cwdLSTM);
            // Set maximum of progress bar
            this.Invoke(new MethodInvoker(delegate () { validationProgressBar.Maximum = totalValidationProgress; }));

            // Initialize the lists of samples for the deep reinforcement learning model
            List<Sample> rlTrainingSamples;
            List<Sample> rlValidationSamples;
            // and the LSTM model
            List<List<Sample>> lstmTrainingSequences;
            List<List<Sample>> lstmValidationSequences;

            ////////////////////////// Start validation for each model

            // Calculate number of data to be processed
            _totalSamples = totalValidationProgress;
            _remainingSamples = _totalSamples;

            // Start the validation process for both models
            calculateTimeToFinish();

            double rlTotalTrainingSize = 0;
            double rlTotalValidationSize = 0;

            double lstmTotalTrainingSize = 0;
            double lstmTotalValidationSize = 0;

            for (int iModelDataFold = 0; iModelDataFold < valModelSamplesFolds.Count; iModelDataFold++)
            {
                // Iterate through each signal features
                //====================================================================================//
                //:::::::::::::::::::::::::::Reinforcement learning model::::::::::::::::::::::::::::://
                // Set vallidation samples for the CWDReinforcementLModel
                rlTrainingSamples = valModelSamplesFolds[iModelDataFold].TrainingData;
                // and training samples for the CWDReinforcementLModel
                rlValidationSamples = valModelSamplesFolds[iModelDataFold].ValidationData;

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                CustomArchiBaseModel rlTempModel = null;
                // Check if there are any training samples for the temporary model
                if (rlTrainingSamples.Count > 0)
                    // This is for training and validating
                    // Create The model from trainingFeatures
                    rlTempModel = CWDReinforcementL_CreateTempModel(cwdLSTM.CWDReinforcementLModel, rlTrainingSamples);
                else
                    // This is for fast validation
                    // Get the selected model
                    rlTempModel = cwdLSTM.CWDReinforcementLModel;


                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                rlTotalTrainingSize += rlTrainingSamples.Count / (double)valModelsDataFolds.Count;
                rlTotalValidationSize += rlValidationSamples.Count / (double)valModelsDataFolds.Count;

                // Add one to _remainingSamples to keep the timer waiting for the LSTM model validation
                _remainingSamples += 1;
                // Update validation values with the new folding 
                CWDReinforcementL_UpdateModelValidation(cwdLSTM.CWDReinforcementLModel, rlValidationSamples, rlTempModel);

                //====================================================================================//
                //::::::::::::::::::::::::::::::::::::LSTM model:::::::::::::::::::::::::::::::::::::://
                // Build LSTM training and validation sequences
                lstmTrainingSequences = DatasetExplorerForm.BuildLSTMTrainingSequences(valModelsDataFolds[iModelDataFold].TrainingData, (TFNETReinforcementL)rlTempModel);
                lstmValidationSequences = DatasetExplorerForm.BuildLSTMTrainingSequences(valModelsDataFolds[iModelDataFold].ValidationData, (TFNETReinforcementL)rlTempModel);

                int lstmFoldTrainingSize = 0;
                int lstmFoldValidationSize = 0;
                foreach (List<Sample> samplesSeq in lstmTrainingSequences)
                    lstmFoldTrainingSize += samplesSeq.Count;
                foreach (List<Sample> samplesSeq in lstmValidationSequences)
                    lstmFoldValidationSize += samplesSeq.Count;

                // Update the maximum of progress bar
                totalValidationProgress += lstmFoldValidationSize;
                this.Invoke(new MethodInvoker(delegate () { validationProgressBar.Maximum = totalValidationProgress; }));
                // Update the remaining time computing values
                _totalSamples += lstmFoldValidationSize;
                _remainingSamples += lstmFoldValidationSize;
                // Remove the previously added one
                _remainingSamples -= 1;

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                CustomArchiBaseModel lstmTempModel = null;
                // Check if there are any training samples for the temporary model
                if (lstmTrainingSequences.Count > 0)
                    // This is for training and validating
                    // Create The model from trainingFeatures
                    lstmTempModel = LSTM_CreateTempModel(cwdLSTM.CWDLSTMModel, lstmTrainingSequences);
                else
                    // This is for fast validation
                    // Get the selected model
                    lstmTempModel = cwdLSTM.CWDLSTMModel;


                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                lstmTotalTrainingSize += lstmFoldTrainingSize / (double)valModelsDataFolds.Count;
                lstmTotalValidationSize += lstmFoldValidationSize / (double)valModelsDataFolds.Count;

                // Update validation values with the new folding 
                LSTM_UpdateModelValidation(cwdLSTM.CWDLSTMModel, lstmValidationSequences, (TFNETLSTMModel)lstmTempModel);
            }

            // Set the new dataset size measurements to the selected model
            cwdLSTM.CWDReinforcementLModel.ValidationData._datasetSize = (int)(rlTotalTrainingSize + rlTotalValidationSize);
            cwdLSTM.CWDReinforcementLModel.ValidationData._trainingDatasetSize = rlTotalTrainingSize;
            cwdLSTM.CWDReinforcementLModel.ValidationData._validationDatasetSize = rlTotalValidationSize;

            cwdLSTM.CWDLSTMModel.ValidationData._datasetSize = (int)(lstmTotalTrainingSize + lstmTotalValidationSize);
            cwdLSTM.CWDLSTMModel.ValidationData._trainingDatasetSize = lstmTotalTrainingSize;
            cwdLSTM.CWDLSTMModel.ValidationData._validationDatasetSize = lstmTotalValidationSize;

            // Insert new validation data in validationFlowLayoutPanel
            refreshValidationData();
        }

        private CustomArchiBaseModel LSTM_CreateTempModel(TFNETLSTMModel cwdLSTMModel, List<List<Sample>> trainingSequences)
        {
            CustomArchiBaseModel tempModel = CWD_TF_NET_LSTM.createTFNETLSTMModel(cwdLSTMModel.Name, "", cwdLSTMModel._inputDim, cwdLSTMModel._outputDim, cwdLSTMModel.OutputsNames);
            tempModel._pcaActive = cwdLSTMModel._pcaActive;
            if (tempModel._pcaActive)
                tempModel.PCA = DataVisualisationForm.getPCA(trainingSequences.SelectMany(seqSamples => seqSamples).ToList());

            // Fit features
            tempModel = TF_NET_LSTM_Recur.Fit((TFNETLSTMModel)tempModel, trainingSequences, null);
            CWD_TF_NET_LSTM.UpdateThresholds((TFNETLSTMModel)tempModel, trainingSequences);

            return tempModel;
        }

        private void ValidationReportUpdate()
        {
            _remainingSamples--;
            // Update fitProgressBar
            this.Invoke(new MethodInvoker(delegate () { validationProgressBar.Value++; }));
        }

        private void LSTM_UpdateModelValidation(TFNETLSTMModel selectedModel, List<List<Sample>> validationSequences, TFNETLSTMModel tempModel)
        {
            selectedModel.ValidationData = LSTM_ValidateTheModel(selectedModel, validationSequences, tempModel, ValidationReportUpdate).validationData;
        }

        public static (ValidationData validationData, OutputThresholdItem[] outThresholds) LSTM_ValidateTheModel(TFNETLSTMModel selectedModel, List<List<Sample>> validationSequences, TFNETLSTMModel tempModel, ValidationReportDelegate validationReportDelegate )
        {
            // Get current model's output metrics' resutls and thresholds
            ValidationData validationDataClone = selectedModel.ValidationData.Clone();
            OutputMetrics[] outputMetrics = validationDataClone._ModelOutputsValidMetrics;
            OutputThresholdItem[] outThresholdsClone = selectedModel.OutputsThresholds.Select(thresholdVals => thresholdVals.Clone()).ToArray();

            // Initialize the validation values
            for (int col = 0; col < selectedModel._outputDim; col++)
            {
                outputMetrics[col]._truePositive = 0;
                outputMetrics[col]._trueNegative = 0;
                outputMetrics[col]._falsePositive = 0;
                outputMetrics[col]._falseNegative = 0;

                validationDataClone._ConfusionMatrix[col] = new double[selectedModel._outputDim];

                outputMetrics[col]._iSamples = 0;

                validationDataClone._ModelOutputsValidMetrics[col]._classDeviationMean = 0;
                validationDataClone._ModelOutputsValidMetrics[col]._classDeviationStd = 0;
            }

            // Thhese two are for averaging the predicted outputs
            // for computing the mean threshold
            int[] highOutputsCount = new int[selectedModel._outputDim];
            int[] lowOutputsCount = new int[selectedModel._outputDim];

            // Calculate validation metrics
            double[] predictedOutput;
            double[] actualOutput;
            int sampleIndex;
            int samplingRate;
            List<double>[] predictionDeviations = new List<double>[selectedModel._outputDim];
            for (int i = 0; i < selectedModel._outputDim; i++)
                predictionDeviations[i] = new List<double>();

            // The following is used for rectifying predicted outputs
            RefDouble[] predictedRefOutput;
            RefDouble[] latestClassifiedRefOutput = null;
            List<(RefDouble[] predicted, int peakIndex)> prediOutputsIndexPairList;

            Queue<double[]> InputSeqQueue;

            List<double[]> predictedSequenceList;
            List<NDArray> layersLatestOutputsVals = null, layersLatestStatesVals = null;
            (List<Tensor> sequenceCellsInputsPlaceHolders, List<Tensor> sequenceCellsOutputs) = TF_NET_LSTM_Recur.GetInputOutputPlaceHolders(tempModel, tempModel.BaseModel.Session);
            (List<Tensor> layersSartingOutputs, List<Tensor> layersStartingStates) = TF_NET_LSTM_Recur.GetLayersStartingOutputsAndStates(tempModel);
            (List<Tensor> layersLatestOutputs, List<Tensor> layersLatestStates) = TF_NET_LSTM_Recur.GetLayersLatestOutputsAndStates(tempModel);

            foreach (List<Sample> samplesSeq in validationSequences)
            {
                prediOutputsIndexPairList = new List<(RefDouble[], int)>(samplesSeq.Count);

                InputSeqQueue = new Queue<double[]>(tempModel._modelSequenceLength + 1);

                layersLatestOutputsVals = null;
                layersLatestStatesVals = null;
                foreach (Sample sample in samplesSeq)
                {
                    actualOutput = sample.getOutputs();

                    // Enqueue the new features in InputSeqQueue
                    InputSeqQueue.Enqueue(sample.getFeatures());
                    // Check if the queue has exceeded the sequence length of the model
                    if (InputSeqQueue.Count > tempModel._modelSequenceLength)
                        InputSeqQueue.Dequeue();

                    // Feed the queue sequence into the LSTM model for prediction
                    (predictedSequenceList, layersLatestOutputsVals, layersLatestStatesVals) = TF_NET_LSTM_Recur.PredictSequenciallyFast(InputSeqQueue.ToList(), tempModel,
                                                                                      layersLatestOutputsVals, layersLatestStatesVals,
                                                                                      sequenceCellsInputsPlaceHolders, sequenceCellsOutputs,
                                                                                      layersSartingOutputs, layersStartingStates,
                                                                                      layersLatestOutputs, layersLatestStates);

                    if (predictedSequenceList.Count == 0)
                        continue;
                    predictedOutput = predictedSequenceList[0];

                    // Update the threshold _highOutputAv and _lowOutputAv
                    for (int iOutput = 0; iOutput < sample.getOutputs().Length; iOutput++)
                    {
                        double outputaVal = sample.getOutputs()[iOutput];

                        if (outputaVal > 0)
                        {
                            outThresholdsClone[iOutput]._highOutputAv = (outThresholdsClone[iOutput]._highOutputAv * highOutputsCount[iOutput] +
                                                                                 predictedOutput[iOutput]) / (highOutputsCount[iOutput] + 1);
                            highOutputsCount[iOutput] += 1;
                        }
                        else
                        {
                            outThresholdsClone[iOutput]._lowOutputAv = (outThresholdsClone[iOutput]._lowOutputAv * lowOutputsCount[iOutput] +
                                                                                 predictedOutput[iOutput]) / (lowOutputsCount[iOutput] + 1);
                            lowOutputsCount[iOutput] += 1;
                        }
                    }

                    // Set each predicted value from the output as a reference
                    predictedRefOutput = new RefDouble[predictedOutput.Length];
                    for (int iDouble = 0; iDouble < predictedOutput.Length; iDouble++)
                        predictedRefOutput[iDouble] = new RefDouble(predictedOutput[iDouble]);

                    // Pair the output with its index
                    prediOutputsIndexPairList.Add((predictedRefOutput, (int)sample.AdditionalInfo[CWDNamigs.peakIndex]));

                    // Update latestClassifiedRefOutput
                    if (latestClassifiedRefOutput == null)
                        latestClassifiedRefOutput = predictedRefOutput;

                    for (int iPredOutput = 0; iPredOutput < predictedRefOutput.Length; iPredOutput++)
                        if (predictedRefOutput[iPredOutput].value >= outThresholdsClone[iPredOutput]._threshold)
                        {
                            if (predictedRefOutput[iPredOutput].value >= latestClassifiedRefOutput[iPredOutput].value)
                            {
                                latestClassifiedRefOutput[iPredOutput].value = 0;
                                latestClassifiedRefOutput[iPredOutput] = predictedRefOutput[iPredOutput];
                            }
                            else
                                predictedRefOutput[iPredOutput].value = 0;
                        }
                        else
                            latestClassifiedRefOutput[iPredOutput] = new RefDouble(0);

                    // Update validation progress report
                    if (validationReportDelegate != null)
                        validationReportDelegate();
                }

                // Set the validation measurements of the prediction for the current sequence
                for (int iSample = 0; iSample < samplesSeq.Count; iSample++)
                {
                    actualOutput = samplesSeq[iSample].getOutputs();
                    sampleIndex = (int)samplesSeq[iSample].AdditionalInfo[CWDNamigs.peakIndex];
                    samplingRate = (int)samplesSeq[iSample].AdditionalInfo[CWDNamigs.samplingRate];
                    for (int i = 0; i < actualOutput.Length; i++)
                    {
                        // Check if the output should be positive or negative
                        if (actualOutput[i] == 1)
                        {
                            // Get nearby positively predicted samples according to the selected label's tolerance
                            List<(RefDouble[] prediction, double deviation)> nearbyPositivePeaks = prediOutputsIndexPairList.Where(predIndexPair =>
                                                                        (Math.Abs(predIndexPair.peakIndex - sampleIndex) / (double)samplingRate * 1000d) <= validationDataClone._ModelOutputsValidMetrics[i]._classDeviationTolerance // 1000d to convert the deviation from seconds to milliseconds
                                                                        && predIndexPair.predicted[i].value >= outThresholdsClone[i]._threshold)
                                                                    .Select(predIndexPair => (predIndexPair.predicted,
                                                                                              Math.Abs(predIndexPair.peakIndex - sampleIndex) / (double)samplingRate * 1000d))
                                                                    .ToList();

                            // Compute the deviations of the prediction
                            foreach ((_, double deviation) in nearbyPositivePeaks)
                                predictionDeviations[i].Add(deviation);

                            // Check if there is any positive forcasted output
                            if (nearbyPositivePeaks.Count > 0)
                                outputMetrics[i]._truePositive++;
                            else
                                outputMetrics[i]._falseNegative++;
                        }
                        else
                        {
                            if (prediOutputsIndexPairList[iSample].predicted[i].value < outThresholdsClone[i]._threshold)
                                outputMetrics[i]._trueNegative++;
                            else
                                outputMetrics[i]._falsePositive++;
                        }
                    }
                }

                // Set the confusion matrix
                foreach (Sample sample in samplesSeq)
                {
                    actualOutput = sample.getOutputs();
                    sampleIndex = (int)sample.AdditionalInfo[CWDNamigs.peakIndex];
                    samplingRate = (int)sample.AdditionalInfo[CWDNamigs.samplingRate];
                    for (int col = 0; col < actualOutput.Length; col++)
                        if (actualOutput[col] == 1)
                            for (int row = 0; row < actualOutput.Length; row++)
                            {
                                List<RefDouble[]> nearbyPositivePeaks = prediOutputsIndexPairList.Where(predIndexPair =>
                                                                            (Math.Abs(predIndexPair.peakIndex - sampleIndex) / (double)samplingRate * 1000d) <= validationDataClone._ModelOutputsValidMetrics[row]._classDeviationTolerance // 1000d to convert the deviation from seconds to milliseconds
                                                                            && predIndexPair.predicted[row].value >= outThresholdsClone[row]._threshold)
                                                                        .Select(predIndexPair => predIndexPair.predicted)
                                                                        .ToList();
                                if (nearbyPositivePeaks.Count > 0)
                                    validationDataClone._ConfusionMatrix[col][row]++;
                            }
                }
            }

            // Compute te deviation mean and standard deviation of the predictions
            for (int iOutput = 0; iOutput < selectedModel._outputDim; iOutput++)
            {
                double[] deviations = predictionDeviations[iOutput].ToArray();
                validationDataClone._ModelOutputsValidMetrics[iOutput]._classDeviationMean = GeneralTools.MeanMinMax(deviations).mean;
                validationDataClone._ModelOutputsValidMetrics[iOutput]._classDeviationStd = GeneralTools.stdDevCalc(deviations, validationDataClone._ModelOutputsValidMetrics[iOutput]._classDeviationMean);
            }

            return (validationDataClone, outThresholdsClone);
        }
    }
}

using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.AITools.TF_NET_Objectives;
using BSP_Using_AI.AITools.DatasetExplorer;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
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
            // Initialize _validationData for both models
            cwdLSTM.CWDReinforcementLModel.ValidationData = new ValidationData(cwdLSTM.CWDReinforcementLModel._outputDim);
            cwdLSTM.CWDLSTMModel.ValidationData = new ValidationData(cwdLSTM.CWDLSTMModel._outputDim);

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
            CustomArchiBaseModel tempModel = CWD_TF_NET_LSTM.createTFNETLSTMModel(cwdLSTMModel.Name, "", cwdLSTMModel._inputDim, cwdLSTMModel._outputDim);
            tempModel._pcaActive = cwdLSTMModel._pcaActive;
            if (tempModel._pcaActive)
                tempModel.PCA = DataVisualisationForm.getPCA(trainingSequences.SelectMany(seqSamples => seqSamples).ToList());

            // Fit features
            tempModel = TF_NET_LSTM.Fit((TFNETLSTMModel)tempModel, trainingSequences, null);
            CWD_TF_NET_LSTM.UpdateThresholds((TFNETLSTMModel)tempModel, trainingSequences);

            return tempModel;
        }

        private void LSTM_UpdateModelValidation(TFNETLSTMModel selectedModel, List<List<Sample>> validationSequences, TFNETLSTMModel tempModel)
        {
            // Get current model's output metrics' resutls and thresholds
            OutputMetrics[] outputMetrics = selectedModel.ValidationData._ModelOutputsValidMetrics;
            OutputThresholdItem[] outThresholds = selectedModel.OutputsThresholds;
            // Calculate validation metrics
            double[] predictedOutput;
            double[] actualOutput;

            // The following is used for rectifying predicted outputs
            RefDouble[] predictedRefOutput;
            RefDouble[] latestClassifiedRefOutput = null;
            List<(RefDouble[] predictedRefOutput, double[] actualOutput)> prediActualOutputsPairsList;

            Queue<double[]> InputSeqQueue;

            foreach (List<Sample> samplesSeq in validationSequences)
            {
                prediActualOutputsPairsList = new List<(RefDouble[] predictedRefOutput, double[] actualOutput)>(samplesSeq.Count);

                InputSeqQueue = new Queue<double[]>(tempModel._modelSequenceLength + 1);
                foreach (Sample sample in samplesSeq)
                {
                    actualOutput = sample.getOutputs();

                    // Enqueue the new features in InputSeqQueue
                    InputSeqQueue.Enqueue(sample.getFeatures());
                    // Check if the queue has exceeded the sequence length of the model
                    if (InputSeqQueue.Count > tempModel._modelSequenceLength)
                        InputSeqQueue.Dequeue();

                    // Feed the queue sequence into the LSTM model for prediction
                    List<double[]> output = TF_NET_LSTM.Predict(InputSeqQueue.ToList(), tempModel);
                    _remainingSamples--;

                    if (output.Count == 0)
                        continue;
                    predictedOutput = output[output.Count - 1];

                    predictedRefOutput = new RefDouble[predictedOutput.Length];
                    for (int iDouble = 0; iDouble < predictedOutput.Length; iDouble++)
                        predictedRefOutput[iDouble] = new RefDouble(predictedOutput[iDouble]);

                    // Insert the paris in prediActualOutputsPairsList
                    prediActualOutputsPairsList.Add((predictedRefOutput, actualOutput));

                    // Update latestClassifiedRefOutput
                    if (latestClassifiedRefOutput == null)
                        latestClassifiedRefOutput = predictedRefOutput;

                    for (int iPredOutput = 0; iPredOutput < predictedRefOutput.Length; iPredOutput++)
                        if (predictedRefOutput[iPredOutput].value >= outThresholds[iPredOutput]._threshold)
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

                    // Update fitProgressBar
                    this.Invoke(new MethodInvoker(delegate () { validationProgressBar.Value++; }));
                }

                // Set the validation measurements of the prediction for the current sequence
                foreach ((RefDouble[] predicted, double[] actual) predicPair in prediActualOutputsPairsList)
                    for (int i = 0; i < predicPair.predicted.Length; i++)
                    {
                        // Check if the output should be positive or negative
                        if (predicPair.actual[i] == 1)
                        {
                            // If yes then check if the forcasted output is true or false
                            if (predicPair.predicted[i].value >= outThresholds[i]._threshold)
                                outputMetrics[i]._truePositive++;
                            else
                                outputMetrics[i]._falseNegative++;
                        }
                        else
                        {
                            if (predicPair.predicted[i].value < outThresholds[i]._threshold)
                                outputMetrics[i]._trueNegative++;
                            else
                                outputMetrics[i]._falsePositive++;
                        }
                    }
            }
        }
    }
}

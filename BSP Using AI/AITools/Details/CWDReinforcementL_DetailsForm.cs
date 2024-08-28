using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.AITools.RL_Objectives;
using BSP_Using_AI.AITools.DatasetExplorer;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.Details.ValidationDataSelection.ValDataSelectionForm;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools.Details
{
    public partial class DetailsForm
    {
        private (List<ModelSamples> valModelSamplesFolds, int totalValidationProgress) CWDReinforcementL_GetModelSamplesFolds(List<ModelData> valModelsDataFolds, TFNETReinforcementL crazyReinforcementLModel, ObjectiveBaseModel objectiveModel)
        {
            int totalValidationProgress = 0;
            List<ModelSamples> valModelSamplesFolds = new List<ModelSamples>(valModelsDataFolds.Count);
            for (int iDataFold = 0; iDataFold < valModelsDataFolds.Count; iDataFold++)
            {
                ModelSamples newSamplesFold = new ModelSamples();
                newSamplesFold.TrainingData = DatasetExplorerForm.GetTrainingSamples(valModelsDataFolds[iDataFold].TrainingData, crazyReinforcementLModel, null, objectiveModel);
                newSamplesFold.ValidationData = DatasetExplorerForm.GetTrainingSamples(valModelsDataFolds[iDataFold].ValidationData, crazyReinforcementLModel, null, objectiveModel);

                valModelSamplesFolds.Add(newSamplesFold);

                totalValidationProgress += newSamplesFold.ValidationData.Count;
            }

            return (valModelSamplesFolds, totalValidationProgress);
        }

        private void CWDReinforcementL_ValidateModel(List<ModelData> valModelsDataFolds, CWDReinforcementL cwdReinforcementL)
        {
            // Convert the annotation data samples to the deep reinforcement learning model samples
            (List<ModelSamples> valModelSamplesFolds, int totalValidationProgress) = CWDReinforcementL_GetModelSamplesFolds(valModelsDataFolds, cwdReinforcementL.CWDCrazyReinforcementLModel, cwdReinforcementL);
            // Set maximum of progress bar
            this.Invoke(new MethodInvoker(delegate () { validationProgressBar.Maximum = totalValidationProgress; }));

            // Initialize the lists of samples for the deep reinforcement learning model
            List<Sample> trainingSamples;
            List<Sample> validationSamples;

            ////////////////////////// Start validation for the deep rl model
            // Initialize _validationData
            cwdReinforcementL.CWDReinforcementLModel.ValidationData = new ValidationData(cwdReinforcementL.CWDReinforcementLModel._outputDim);

            // Calculate number of data to be processed
            _totalSamples = totalValidationProgress;
            _remainingSamples = _totalSamples;

            // Start the validation process for the CWDReinforcementLModel model
            calculateTimeToFinish();

            double totalTrainingSize = 0;
            double totalValidationSize = 0;

            foreach (ModelSamples modelSamples in valModelSamplesFolds)
            {
                // Iterate through each signal features
                // Set vallidation samples for the CWDReinforcementLModel
                trainingSamples = modelSamples.TrainingData;
                // and training samples for the CWDReinforcementLModel
                validationSamples = modelSamples.ValidationData;

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                CustomArchiBaseModel tempModel = null;
                // Check if there are any training samples for the temporary model
                if (trainingSamples.Count > 0)
                    // This is for training and validating
                    // Create The model from trainingFeatures
                    tempModel = CWDReinforcementL_CreateTempModel(cwdReinforcementL.CWDReinforcementLModel, trainingSamples);
                else
                    // This is for fast validation
                    // Get the selected model
                    tempModel = cwdReinforcementL.CWDReinforcementLModel;


                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                totalTrainingSize += trainingSamples.Count / (double)valModelsDataFolds.Count;
                totalValidationSize += validationSamples.Count / (double)valModelsDataFolds.Count;

                // Update validation values with the new folding 
                CWDReinforcementL_UpdateModelValidation(cwdReinforcementL.CWDReinforcementLModel, validationSamples, tempModel);
            }

            // Set the new dataset size measurements to the selected model
            cwdReinforcementL.CWDReinforcementLModel.ValidationData._datasetSize = (int)(totalTrainingSize + totalValidationSize);
            cwdReinforcementL.CWDReinforcementLModel.ValidationData._trainingDatasetSize = totalTrainingSize;
            cwdReinforcementL.CWDReinforcementLModel.ValidationData._validationDatasetSize = totalValidationSize;

            // Insert new validation data in validationFlowLayoutPanel
            refreshValidationData();
        }

        private CustomArchiBaseModel CWDReinforcementL_CreateTempModel(TFNETReinforcementL cwdReinforcementLModel, List<Sample> trainingSamples)
        {
            CustomArchiBaseModel tempModel = CWD_RL_TFNET.createTFNETRLModel(cwdReinforcementLModel.Name, "", cwdReinforcementLModel._inputDim, cwdReinforcementLModel._outputDim, cwdReinforcementLModel.OutputsNames);
            tempModel._pcaActive = cwdReinforcementLModel._pcaActive;
            if (tempModel._pcaActive)
                tempModel.PCA = DataVisualisationForm.getPCA(trainingSamples); ;

            // Fit features
            tempModel = TF_NET_NN.fit(tempModel, ((TFNETNeuralNetworkModel)tempModel).BaseModel, trainingSamples, null);

            return tempModel;
        }

        private void CWDReinforcementL_UpdateModelValidation(TFNETReinforcementL selectedModel, List<Sample> validationSamples, CustomArchiBaseModel tempModel)
        {
            // Get current model's output metrics' resutls
            OutputMetrics[] outputMetrics = selectedModel.ValidationData._ModelOutputsValidMetrics;
            // Calculate validation metrics
            double[] predictedOutput;
            double[] actualOutput;
            // the following is used for computing MASE
            double[] previousActualOutput = null;

            for (int iValSample = 0; iValSample < validationSamples.Count; iValSample++)
            {
                Sample sample = validationSamples[iValSample];
                actualOutput = sample.getOutputs();
                // Get prediction output for current feature
                predictedOutput = TF_NET_NN.predict(sample.getFeatures(), tempModel, ((TFNETReinforcementL)tempModel).BaseModel.Session);
                _remainingSamples--;

                // Check if there is previousActualOutput
                if (iValSample > 0)
                    for (int i = 0; i < predictedOutput.Length; i++)
                    {
                        // Compute accuracy using Mean Absolute Scaled Error
                        outputMetrics[i]._mae = (outputMetrics[i]._mae * outputMetrics[i]._iSamples + Math.Abs(actualOutput[i] - predictedOutput[i])) / (outputMetrics[i]._iSamples + 1);
                        outputMetrics[i]._maeNaive = (outputMetrics[i]._maeNaive * outputMetrics[i]._iSamples + Math.Abs(actualOutput[i] - previousActualOutput[i])) / (outputMetrics[i]._iSamples + 1);
                        double mase = outputMetrics[i]._mae / outputMetrics[i]._maeNaive;
                        if (!double.IsInfinity(mase))
                            outputMetrics[i]._mase = mase;
                        outputMetrics[i]._iSamples++;
                    }
                previousActualOutput = actualOutput;

                // Update fitProgressBar
                this.Invoke(new MethodInvoker(delegate () { validationProgressBar.Value++; }));
            }
        }
    }
}

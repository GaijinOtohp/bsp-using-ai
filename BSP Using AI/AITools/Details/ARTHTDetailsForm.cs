using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.AITools.Keras_NET_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.KNN_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.NaiveBayes_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.Database;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.AITools.Details.ValidationDataSelection.ValDataSelectionForm;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools.Details
{
    public partial class DetailsForm
    {
        public ARTHT_Keras_NET_NN _tFBackThread;

        private void ARTHRValidateModel(List<ModelData> valModelsDataFolds, ARTHTModels arthtModels)
        {
            // Initialize lists of features for each step
            List<Sample> trainingSamples;
            List<Sample> validationSamples;

            ////////////////////////// Start validation for each step model (7 steps)
            // Set maximum of progress bar
            this.Invoke(new MethodInvoker(delegate () { validationProgressBar.Maximum = arthtModels.ARTHTModelsDic.Count * valModelsDataFolds.Count; }));
            // Initialize _validationData
            foreach (CustomArchiBaseModel model in arthtModels.ARTHTModelsDic.Values)
                model.ValidationData = new ValidationData(model._outputDim);

            // Calculate number of data to be processed
            _totalSamples = 0;
            foreach (ModelData modelData in valModelsDataFolds)
                foreach (DataRow row in modelData.ValidationData)
                {
                    ARTHTFeatures arthtFeatures = GeneralTools.ByteArrayToObject<ARTHTFeatures>(row.Field<byte[]>("features"));
                    foreach (Data data in arthtFeatures.StepsDataDic.Values)
                        _totalSamples += data.Samples.Count;
                }
            _remainingSamples = _totalSamples;

            // Start validation process for each step model
            calculateTimeToFinish();
            foreach (string stepName in arthtModels.ARTHTModelsDic.Keys)
            {
                double totalTrainingSize = 0;
                double totalValidationSize = 0;

                foreach (ModelData modelData in valModelsDataFolds)
                {
                    // Iterate through each signal features
                    trainingSamples = new List<Sample>();
                    validationSamples = new List<Sample>();
                    // Set vallidation samples
                    foreach (DataRow row in modelData.ValidationData)
                    {
                        ARTHTFeatures arthtFeatures = GeneralTools.ByteArrayToObject<ARTHTFeatures>(row.Field<byte[]>("features"));
                        validationSamples.AddRange(arthtFeatures.StepsDataDic[stepName].Samples);
                    }
                    // and training samples
                    foreach (DataRow row in modelData.TrainingData)
                    {
                        ARTHTFeatures arthtFeatures = GeneralTools.ByteArrayToObject<ARTHTFeatures>(row.Field<byte[]>("features"));
                        trainingSamples.AddRange(arthtFeatures.StepsDataDic[stepName].Samples);
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    CustomArchiBaseModel tempModel = null;
                    // Check if there are any training samples for the temporary model
                    if (trainingSamples.Count > 0)
                        // This is for training and validating
                        // Create The model from trainingFeatures
                        tempModel = ARTHT_CreateTempModel(arthtModels, trainingSamples, stepName);
                    else
                        // This is for fast validation
                        // Get the selected model
                        tempModel = arthtModels.ARTHTModelsDic[stepName];


                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    totalTrainingSize += trainingSamples.Count / (double)valModelsDataFolds.Count;
                    totalValidationSize += validationSamples.Count / (double)valModelsDataFolds.Count;

                    // Update validation values with the new folding 
                    ARTHT_UpdateModelValidation(arthtModels, validationSamples, stepName, tempModel);

                    // Update fitProgressBar
                    this.Invoke(new MethodInvoker(delegate () { validationProgressBar.Value++; }));
                }

                // Set the new dataset size measurements to the selected model
                arthtModels.ARTHTModelsDic[stepName].ValidationData._datasetSize = (int)(totalTrainingSize + totalValidationSize);
                arthtModels.ARTHTModelsDic[stepName].ValidationData._trainingDatasetSize = totalTrainingSize;
                arthtModels.ARTHTModelsDic[stepName].ValidationData._validationDatasetSize = totalValidationSize;

                // Insert new validation data in validationFlowLayoutPanel
                refreshValidationData();
            }
        }

        private CustomArchiBaseModel ARTHT_CreateTempModel(ARTHTModels arthtModels, List<Sample> trainingSamples, string stepName)
        {
            // Check which model is selected
            CustomArchiBaseModel tempModel = null;
            if (arthtModels.ModelName.Equals(KerasNETNeuralNetworkModel.ModelName))
            {
                // This is for neural network
                _tFBackThread._queue.Enqueue(new QueueSignalInfo()
                {
                    TargetFunc = "fit",
                    CallingClass = this.Name,
                    DataList = trainingSamples,
                    ModelsName = arthtModels.ModelName + arthtModels.ObjectiveName,
                    StepName = stepName
                });
                _tFBackThread._signal.Set();
            }
            else if (arthtModels.ModelName.Equals(KNNModel.ModelName))
            {
                // This is for knn
                // Create a KNN model structure with the initial optimum K, which is "3"
                tempModel = ARTHT_KNN.createKNNModel(stepName, trainingSamples, arthtModels.ARTHTModelsDic[stepName]._pcaActive);

                // Fit features
                tempModel = KNN.fit((KNNModel)tempModel, trainingSamples, null);
            }
            else if (arthtModels.ModelName.Equals(NaiveBayesModel.ModelName))
            {
                // This is for naive bayes
                tempModel = ARTHT_NaiveBayes.createNBModel(stepName, trainingSamples, arthtModels.ARTHTModelsDic[stepName]._pcaActive);

                // Fit features
                tempModel = NaiveBayes.fit((NaiveBayesModel)tempModel, trainingSamples);
            }
            else if (arthtModels.ModelName.Equals(TFNETNeuralNetworkModel.ModelName))
            {
                // This is for Tensorflow.Net Neural Networks
                tempModel = ARTHT_TF_NET_NN.createTFNETNeuralNetModel(stepName, trainingSamples, arthtModels.ARTHTModelsDic[stepName]._pcaActive, "");

                // Fit features
                tempModel = TF_NET_NN.fit(tempModel, ((TFNETNeuralNetworkModel)tempModel).BaseModel, trainingSamples, null);
            }
            else if (arthtModels.ModelName.Equals(TFKerasNeuralNetworkModel.ModelName))
            {
                // This is for Tensorflow.Keras Neural Networks
                tempModel = TF_KERAS_NN.createTFKerasNeuralNetModel(stepName, trainingSamples, arthtModels.ARTHTModelsDic[stepName]._pcaActive, "");

                // Fit features
                tempModel = TF_KERAS_NN.fit((TFKerasNeuralNetworkModel)tempModel, trainingSamples);
            }

            return tempModel;
        }

        private void ARTHT_UpdateModelValidation(ARTHTModels arthtModels, List<Sample> validationSamples, string stepName, CustomArchiBaseModel tempModel)
        {
            int validationSize = validationSamples.Count;
            // Get curretn step threshold
            CustomArchiBaseModel selectedModel = arthtModels.ARTHTModelsDic[stepName];
            OutputThresholdItem[] outThresholds = selectedModel.OutputsThresholds;
            OutputMetrics[] outputMetrics = selectedModel.ValidationData._ModelOutputsValidMetrics;
            // Calculate validation metrics
            double[] predictedOutput;
            double[] actualOutput;
            // the following is used for computing MASE
            double[] previousActualOutput = null;

            string currentBeatName = "";
            string prevBeatName = "";
            int currentBeatSamples = 0;
            (double predicted, double actual)[] ptPrediction = new (double predicted, double actual)[2] { (-1, 0), (-1, 0) };
            for (int iValSample = 0; iValSample < validationSize; iValSample++)
            {
                Sample sample = validationSamples[iValSample];
                actualOutput = sample.getOutputs();
                // Get prediction output for current feature
                predictedOutput = ARTHT_AskForPrediction(sample.getFeatures(), arthtModels.ModelName, tempModel, stepName);
                _remainingSamples--;

                // Check which type of validation metrics is this
                if (stepName.Equals(ARTHTNamings.Step1RPeaksScanData) || stepName.Equals(ARTHTNamings.Step3BeatPeaksScanData) || stepName.Equals(ARTHTNamings.Step6UpstrokesScanData))
                {
                    // If yes then this is for regression metrics
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
                }
                else
                {
                    // This is for classification metrics
                    // Check if this is for P and T selection
                    if (stepName.Equals(ARTHTNamings.Step4PTSelectionData))
                    {
                        // If yes then prediction should be rearanged for each beat states
                        // Check if beat name is changed
                        // or if this is the last state in validationSamples
                        currentBeatName = sample.Name.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0];
                        if (!currentBeatName.Equals(prevBeatName) || iValSample == validationSize - 1)
                        {
                            // Check if this is the last state
                            if (iValSample == validationSize - 1)
                            {
                                // Compute last state's values
                                currentBeatSamples++;

                                // Check if p and t probabilities are greater than previous predicted samples
                                for (int i = 0; i < predictedOutput.Length; i++)
                                    if (predictedOutput[i] > ptPrediction[i].predicted)
                                        ptPrediction[i] = (predictedOutput[i], actualOutput[i]);
                            }

                            // Check if there exist predicted states
                            if (currentBeatSamples > 0)
                            {
                                // Since P and T peak selection's task takes only the two peaks with the highest probability of being P or T
                                // then those two peaks would be either only one true positive (and there will be 0 false positives and 0 false negatives),
                                // or only one false positive (and there will be 0 true positives and 1 false negative)

                                // Check if the output of the selected peak should be positive or negative
                                for (int i = 0; i < ptPrediction.Length; i++)
                                    // Check if the selected peak is true
                                    if (ptPrediction[i].actual == 1)
                                    {
                                        outputMetrics[i]._truePositive++;
                                        outputMetrics[i]._trueNegative += (currentBeatSamples - 1);
                                    }
                                    else
                                    {
                                        outputMetrics[i]._falsePositive++; // Since this peak is selected falesly then the right one was not selected (so it would have got false positive)
                                        outputMetrics[i]._falseNegative++; // Since this selected peak is not the right one and should be negative (so false negative)
                                        outputMetrics[i]._trueNegative += (currentBeatSamples - 2);
                                    }
                            }

                            // Initialize values for next beat
                            prevBeatName = currentBeatName;
                            currentBeatSamples = 0;
                            for (int i = 0; i < ptPrediction.Length; i++)
                                ptPrediction[i] = (-1, 0);

                            // Break the for loop here if this is the last state
                            if (iValSample == validationSize - 1)
                                break;
                        }
                        currentBeatSamples++;

                        // Check if p and t probabilities are greater than previous predicted samples
                        for (int i = 0; i < predictedOutput.Length; i++)
                            if (predictedOutput[i] > ptPrediction[i].predicted)
                                ptPrediction[i] = (predictedOutput[i], actualOutput[i]);
                    }
                    else
                    {
                        // Set validation data for other steps
                        for (int i = 0; i < predictedOutput.Length; i++)
                        {
                            // Check if the output should be positive or negative
                            if (actualOutput[i] == 1)
                            {
                                // If yes then check if the forcasted output is true or false
                                if (predictedOutput[i] >= outThresholds[i]._threshold)
                                    outputMetrics[i]._truePositive++;
                                else
                                    outputMetrics[i]._falseNegative++;
                            }
                            else
                            {
                                if (predictedOutput[i] < outThresholds[i]._threshold)
                                    outputMetrics[i]._trueNegative++;
                                else
                                    outputMetrics[i]._falsePositive++;
                            }
                        }
                    }
                }
            }
        }

        private double[] ARTHT_AskForPrediction(double[] features, string modelName, CustomArchiBaseModel model, string stepName)
        {
            // Check which model is selected
            if (modelName.Equals(KerasNETNeuralNetworkModel.ModelName))
            {
                // This is for neural network
                AutoResetEvent signal = new AutoResetEvent(false);
                ConcurrentQueue<QueueSignalInfo> queue = new ConcurrentQueue<QueueSignalInfo>();
                _tFBackThread._queue.Enqueue(new QueueSignalInfo()
                {
                    TargetFunc = "predict",
                    CallingClass = "DetailsForm",
                    Features = features,
                    Signal = signal,
                    Queue = queue
                });
                _tFBackThread._signal.Set();

                // Wait for the answer
                signal.WaitOne();

                QueueSignalInfo item = null;
                while (queue.TryDequeue(out item))
                    // Check which function is selected
                    return item.Outputs;
            }
            else if (modelName.Equals(KNNModel.ModelName))
            {
                // This is for knn
                return KNN.predict(features, (KNNModel)model);
            }
            else if (modelName.Equals(NaiveBayesModel.ModelName))
            {
                // This is for naive bayes
                return NaiveBayes.predict(features, (NaiveBayesModel)model);
            }
            else if (modelName.Equals(TFNETNeuralNetworkModel.ModelName))
            {
                // This is for Tensorflow.Net Neural Networks
                return TF_NET_NN.predict(features, model, ((TFNETNeuralNetworkModel)model).BaseModel.Session);
            }
            else if (modelName.Equals(TFKerasNeuralNetworkModel.ModelName))
            {
                // This is for Tensorflow.Keras Neural Networks
                return TF_KERAS_NN.predict(features, (TFKerasNeuralNetworkModel)model);
            }

            return null;
        }
    }
}

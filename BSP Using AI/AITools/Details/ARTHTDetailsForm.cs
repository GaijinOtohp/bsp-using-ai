using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.AITools.Keras_NET_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.KNN_Objectives;
using Biological_Signal_Processing_Using_AI.AITools.NaiveBayes_Objectives;
using BSP_Using_AI.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.AITools.Details.ValidationDataSelection.ValDataSelectionForm;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools.Details
{
    public partial class DetailsForm : Form, DbStimulatorReportHolder
    {
        private void ARTHRValidateModel(List<ModelData> valModelsData, ARTHTModels arthtModels)
        {
            // Initialize lists of features for each step
            List<Sample> trainingSamples;
            List<Sample> validationSamples;

            ////////////////////////// Start validation for each step model (7 steps)
            int fitProgress = 0;
            // Set maximum of progress bar
            this.Invoke(new MethodInvoker(delegate () { validationProgressBar.Maximum = arthtModels.ARTHTModelsDic.Count * valModelsData.Count; }));
            // Initialize _validationData
            foreach (CustomArchiBaseModel model in arthtModels.ARTHTModelsDic.Values)
                model.ValidationData = new ValidationData();

            double accuracy, sensitivity, specificity;
            int trainingSize, validationSize, possibilities, targetPositives, targetNegatives;
            double totalTrainingSize, totalValidationSize;
            // Calculate number of data to be processed
            _data = 0;
            foreach (ModelData modelData in valModelsData)
                foreach (ARTHTFeatures aRTHTFeatures in modelData.ValidationData)
                    foreach (Data data in aRTHTFeatures.StepsDataDic.Values)
                        _data += data.Samples.Count;
            _remainingData = _data;

            // Start validation process for each step model
            calculateTimeToFinish();
            foreach (string stepName in arthtModels.ARTHTModelsDic.Keys)
            {
                trainingSize = 0;
                totalTrainingSize = 0;
                validationSize = 0;
                totalValidationSize = 0;
                accuracy = 0;
                sensitivity = 0;
                specificity = 0;
                possibilities = 0;
                targetPositives = 0;
                targetNegatives = 0;

                foreach (ModelData modelData in valModelsData)
                {
                    // Iterate through each signal features
                    trainingSamples = new List<Sample>();
                    validationSamples = new List<Sample>();
                    List<int> selectedBeatLastState = new List<int>();
                    // Set vallidation samples
                    foreach (ARTHTFeatures aRTHTFeatures in modelData.ValidationData)
                        foreach (Sample sample in aRTHTFeatures.StepsDataDic[stepName].Samples)
                            validationSamples.Add(sample);
                    // and training samples
                    foreach (ARTHTFeatures aRTHTFeatures in modelData.TrainingData)
                        foreach (Sample sample in aRTHTFeatures.StepsDataDic[stepName].Samples)
                            trainingSamples.Add(sample);

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // Create The model from trainingFeatures
                    // Send features for fitting
                    // Check which model is selected
                    //trainingSize += trainingFeatures.Count;
                    trainingSize = trainingSamples.Count;
                    totalTrainingSize += trainingSamples.Count / (double)valModelsData.Count;
                    //validationSize += validationFeatures.Count;
                    validationSize = validationSamples.Count;
                    totalValidationSize += validationSamples.Count / (double)valModelsData.Count;
                    CustomArchiBaseModel model = null;
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
                        model = ARTHT_KNN.createKNNModel(stepName, trainingSamples, arthtModels.ARTHTModelsDic[stepName]._pcaActive);

                        // Fit features
                        model = KNN.fit((KNNModel)model, trainingSamples, null);
                    }
                    else if (arthtModels.ModelName.Equals(NaiveBayesModel.ModelName))
                    {
                        // This is for naive bayes
                        model = ARTHT_NaiveBayes.createNBModel(stepName, trainingSamples, arthtModels.ARTHTModelsDic[stepName]._pcaActive);

                        // Fit features
                        model = NaiveBayes.fit((NaiveBayesModel)model, trainingSamples);
                    }
                    else if (arthtModels.ModelName.Equals(TFNETNeuralNetworkModel.ModelName))
                    {
                        // This is for Tensorflow.Net Neural Networks
                        model = ARTHT_TF_NET_NN.createTFNETNeuralNetModel(stepName, trainingSamples, arthtModels.ARTHTModelsDic[stepName]._pcaActive, "");

                        // Fit features
                        model = TF_NET_NN.fit(model, ((TFNETNeuralNetworkModel)model).BaseModel, trainingSamples, null);
                    }
                    else if (arthtModels.ModelName.Equals(TFKerasNeuralNetworkModel.ModelName))
                    {
                        // This is for Tensorflow.Keras Neural Networks
                        model = TF_KERAS_NN.createTFKerasNeuralNetModel(stepName, trainingSamples, arthtModels.ARTHTModelsDic[stepName]._pcaActive, "");

                        // Fit features
                        model = TF_KERAS_NN.fit((TFKerasNeuralNetworkModel)model, trainingSamples);
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // Calculate validation metrics
                    double[] predictedOutput;
                    double[] actualOutput;
                    // the follosing is used for computing MASE
                    double[] previousActualOutput = null;
                    double mae = 0;
                    double maeNaive = 0;

                    string currentBeatName = "";
                    string prevBeatName = "";
                    int numberOfStates = 0;
                    (double predicted, double actual) pPrediction = (-1, 0);
                    (double predicted, double actual) tPrediction = (-1, 0);
                    for (int j = 0; j < validationSize; j++)
                    {
                        Sample sample = validationSamples[j];
                        actualOutput = sample.getOutputs();
                        // Get prediction output for current feature
                        predictedOutput = askForPrediction(sample.getFeatures(), arthtModels.ModelName, model, stepName);
                        _remainingData--;

                        // Check which type of validation metrics is this
                        if (stepName.Equals(ARTHTNamings.Step1RPeaksScanData) || stepName.Equals(ARTHTNamings.Step3BeatPeaksScanData) || stepName.Equals(ARTHTNamings.Step6UpstrokesScanData))
                        {
                            // If yes then this is for regression metrics
                            // Check if there is previousActualOutput
                            if (j > 0)
                                for (int i = 0; i < predictedOutput.Length; i++)
                                {
                                    // Compute accuracy using Mean Absolute Scaled Error
                                    mae = (mae * possibilities + Math.Abs(actualOutput[i] - predictedOutput[i])) / (possibilities + 1);
                                    maeNaive = (maeNaive * possibilities + Math.Abs(actualOutput[i] - previousActualOutput[i])) / (possibilities + 1);
                                    double mase = mae / maeNaive;
                                    if (!double.IsInfinity(mase))
                                        accuracy = mase;
                                    possibilities++;
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
                                if (!currentBeatName.Equals(prevBeatName) || j == validationSize - 1)
                                {
                                    // Check if this is the last state
                                    if (j == validationSize - 1)
                                    {
                                        // Compute last state's values
                                        numberOfStates++;

                                        // Check if p probability is greater than previous predicted sample
                                        if (predictedOutput[0] > pPrediction.predicted)
                                            pPrediction = (predictedOutput[0], actualOutput[0]);

                                        // Check if t probability is greater than previous predicted sample
                                        if (predictedOutput[1] > tPrediction.predicted)
                                            tPrediction = (predictedOutput[1], actualOutput[1]);
                                    }

                                    // Check if there exist predicted states
                                    if (numberOfStates > 0)
                                    {
                                        // Cumpute the accuracy, sensitivity, and specificity of previous beat

                                        // Compute accuracy
                                        double trueVals = numberOfStates * 2;
                                        trueVals -= pPrediction.actual == 1 ? 0 : 2;
                                        trueVals -= tPrediction.actual == 1 ? 0 : 2;
                                        accuracy = (accuracy * possibilities + trueVals) / (possibilities + numberOfStates * 2);
                                        // possibilities of all of the otputs (2) --> predictedOutput.Length
                                        possibilities += numberOfStates * 2;

                                        // Compute sensitivity
                                        double truePositives = pPrediction.actual + tPrediction.actual;
                                        sensitivity = (sensitivity * targetPositives + truePositives) / (targetPositives + 2);
                                        // only 1 p wave and 1 t wave (2 target positive) for each beat
                                        targetPositives += 2;

                                        // Compute specificity
                                        double trueNegatives = trueVals - truePositives;
                                        specificity = (specificity * targetNegatives + trueNegatives) / (targetNegatives + numberOfStates * 2 - 2);
                                        // all states are negative except 1 for p wave and 1 for t wave
                                        targetNegatives += numberOfStates * 2 - 2;
                                    }

                                    // Initialize values for next beat
                                    prevBeatName = currentBeatName;
                                    numberOfStates = 0;
                                    pPrediction = (-1, 0);
                                    tPrediction = (-1, 0);

                                    // Break the for loop here if this is the last state
                                    if (j == validationSize - 1)
                                        break;
                                }
                                numberOfStates++;

                                // Check if p probability is greater than previous predicted sample
                                if (predictedOutput[0] > pPrediction.predicted)
                                    pPrediction = (predictedOutput[0], actualOutput[0]);

                                // Check if t probability is greater than previous predicted sample
                                if (predictedOutput[1] > tPrediction.predicted)
                                    tPrediction = (predictedOutput[1], actualOutput[1]);
                            }
                            else
                            {
                                // Set validation data for other steps
                                for (int i = 0; i < predictedOutput.Length; i++)
                                {
                                    // Get curretn step threshold
                                    OutputThresholdItem threshold = arthtModels.ARTHTModelsDic[stepName].OutputsThresholds[i];
                                    // Regulate output value
                                    predictedOutput[i] = predictedOutput[i] >= threshold._threshold ? 1 : 0;

                                    // Calculate accuracy
                                    if (predictedOutput[i] == actualOutput[i])
                                        accuracy = (accuracy * possibilities + 1) / (possibilities + 1);
                                    else
                                        accuracy = (accuracy * possibilities + 0) / (possibilities + 1);
                                    possibilities++;

                                    // Calculate sensitivity and specificity
                                    if (actualOutput[i] == 1)
                                    {
                                        // This is for sensitivity
                                        if (predictedOutput[i] == 1)
                                            sensitivity = (sensitivity * targetPositives + 1) / (targetPositives + 1);
                                        else
                                            sensitivity = (sensitivity * targetPositives + 0) / (targetPositives + 1);
                                        targetPositives++;
                                    }
                                    else
                                    {
                                        // This is for specificity
                                        if (predictedOutput[i] == 0)
                                            specificity = (specificity * targetNegatives + 1) / (targetNegatives + 1);
                                        else
                                            specificity = (specificity * targetNegatives + 0) / (targetNegatives + 1);
                                        targetNegatives++;
                                    }
                                }
                            }
                        }
                    }

                    // Update fitProgressBar
                    fitProgress++;
                    this.Invoke(new MethodInvoker(delegate () { validationProgressBar.Value = fitProgress; }));
                }

                // Insert new validation data in validationFlowLayoutPanel
                UpdateModelValidatoinData(totalTrainingSize, totalValidationSize, accuracy, sensitivity, specificity, stepName);
            }
        }
    }
}

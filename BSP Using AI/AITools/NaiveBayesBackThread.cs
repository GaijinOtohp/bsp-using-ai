using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using System;
using System.Collections.Generic;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools
{
    class NaiveBayesBackThread
    {
        private AIBackThreadReportHolder _aiBackThreadReportHolderForAIToolsForm;

        private Dictionary<string, ARTHTModels> _arthtModelsDic = null;

        private class outputProbaGivenInput
        {
            public double proba;
            public double output;
        }

        public NaiveBayesBackThread(Dictionary<string, ARTHTModels> arthtModelsDic, AIBackThreadReportHolder aiBackThreadReportHolderForAIToolsForm)
        {
            _arthtModelsDic = arthtModelsDic;
            _aiBackThreadReportHolderForAIToolsForm = aiBackThreadReportHolderForAIToolsForm;
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public void fit(string modelsName, Dictionary<string, List<Sample>> dataLists, long datasetSize, long modelId, string stepName)
        {
            // Iterate through models from the selected ones in _targetsModelsHashtable
            int fitProgress = 0;
            int tolatFitProgress = dataLists.Count;

            // Iterate through models from the selected ones in _arthtModelsDic
            ARTHTModels arthtModels = _arthtModelsDic[modelsName];

            if (!stepName.Equals(""))
                fit((NaiveBayesModel)arthtModels.ARTHTModelsDic[stepName],
                                                      dataLists[stepName]);
            else
                foreach (string stepNa in arthtModels.ARTHTModelsDic.Keys)
                {
                    // Set the new features probabilities in the model
                    fit((NaiveBayesModel)arthtModels.ARTHTModelsDic[stepNa],
                                                          dataLists[stepNa]);

                    // Update fitProgressBar
                    fitProgress++;
                    if (_aiBackThreadReportHolderForAIToolsForm != null)
                        _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "progress", modelsName, fitProgress, tolatFitProgress }, "AIToolsForm");
                }

            // Update model in models table
            DbStimulator dbStimulator = new DbStimulator();
            if (arthtModels.DataIdsIntervalsList.Count > 0)
                dbStimulator.Update("models", new string[] { "the_model", "dataset_size" },
                    new Object[] { Garage.ObjectToByteArray(arthtModels.Clone()), datasetSize }, modelId, "TFBackThread");
            else
                dbStimulator.Update("models", new string[] { "the_model" },
                    new Object[] { Garage.ObjectToByteArray(arthtModels.Clone()) }, modelId, "TFBackThread");

            // Send report about fitting is finished and models table should be updated
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "fitting_complete", modelsName, datasetSize }, "AIToolsForm");
        }

        public static NaiveBayesModel fit(NaiveBayesModel model, List<Sample> dataList)
        {
            if (model._pcaActive)
                dataList = Garage.rearrangeFeaturesInput(dataList, model.PCA);

            // Set the new features probabilities in the model
            model.OutputsProbaList = partitionOutputs(dataList, model._regression, model.OutputsProbaList);

            return model;
        }

        public static double[] predict(double[] features, NaiveBayesModel naiveBayesModel)
        {
            // Initialize input
            if (naiveBayesModel._pcaActive)
                features = Garage.rearrangeInput(features, naiveBayesModel.PCA);
            // Predict the input
            // Get GausParamsInputsGivenOutput for each class
            // and calculate the probabilities for each val of input
            List<Partition[]> outputsProbaList = naiveBayesModel.OutputsProbaList;
            object[] outputProbaGivenInput = new object[outputsProbaList.Count];
            for (int i = 0; i < outputsProbaList.Count; i++)
            {
                Partition[] partitions = outputsProbaList[i];
                outputProbaGivenInput[i] = new List<outputProbaGivenInput>();
                for (int j = 0; j < partitions.Length; j++)
                {
                    Partition partition = partitions[j];
                    double proba = partition._proba;
                    for (int k = 0; k < partition.GausParamsInputsGivenOutput.Length; k++)
                        proba *= gaussian(partition.GausParamsInputsGivenOutput[k]._mean, partition.GausParamsInputsGivenOutput[k]._variance, features[k]);

                    ((List<outputProbaGivenInput>)outputProbaGivenInput[i]).Add(new outputProbaGivenInput { proba = proba, output = partition._value });
                }
            }
            // Sort outputs according to probabilities
            for (int i = 0; i < outputsProbaList.Count; i++)
                ((List<outputProbaGivenInput>)outputProbaGivenInput[i]).Sort((e1, e2) => { return e1.proba.CompareTo(e2.proba); });

            // Get the values of highest probability as outputs
            double[] output = new double[outputsProbaList.Count];
            for (int i = 0; i < outputsProbaList.Count; i++)
                output[i] = ((List<outputProbaGivenInput>)outputProbaGivenInput[i])[((List<outputProbaGivenInput>)outputProbaGivenInput[i]).Count - 1].output;

            // Return result to main user interface
            return output;
        }

        public void createNBModelForWPW()
        {
            // Create neural network models for WPW syndrome detection
            // Create 7 models for { 2 for QRS detection (Threshold_ratio & Hor_threshold, remove_miss_selected_R),
            // 2 for P_T detection (Threshold_ratio & Hor_threshold, P & T states),
            // 1 for short PR detection,
            // 2 for delta deteciton (Acceleration threshold, delta existence) }
            ARTHTModels arthtModels = new ARTHTModels();
            // Create a KNN models structure with the initial optimum K, which is "3"
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step1RPeaksScanData] = createNBModel(ARTHTNamings.Step1RPeaksScanData, true, 2); // (15, 2) For R peaks detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step2RPeaksSelectionData] = createNBModel(ARTHTNamings.Step2RPeaksSelectionData, false, 1); // (2, 1) For R selection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step3BeatPeaksScanData] = createNBModel(ARTHTNamings.Step3BeatPeaksScanData, true, 2); // (5, 2) For beat peaks detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step4PTSelectionData] = createNBModel(ARTHTNamings.Step4PTSelectionData, false, 2); // (3, 2) For P and T detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step5ShortPRScanData] = createNBModel(ARTHTNamings.Step5ShortPRScanData, false, 1); // (1, 1) For short PR detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step6UpstrokesScanData] = createNBModel(ARTHTNamings.Step6UpstrokesScanData, true, 1); // (6, 1) For delta detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step7DeltaExaminationData] = createNBModel(ARTHTNamings.Step7DeltaExaminationData, false, 1); // (6, 1) For WPW syndrome declaration

            // Insert models in _targetsModelsHashtable
            int modelIndx = 0;
            while (_arthtModelsDic.ContainsKey("Naive bayes for WPW syndrome detection" + modelIndx))
                modelIndx++;
            arthtModels.Name = "Naive bayes for WPW syndrome detection" + modelIndx;
            _arthtModelsDic.Add(arthtModels.Name, arthtModels);

            // Save models in models table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.Insert("models", new string[] { "type_name", "model_target", "the_model", "dataset_size" },
                new Object[] { "Naive bayes", "WPW syndrome detection", Garage.ObjectToByteArray(arthtModels.Clone()), 0 }, "KNNBackThread");

            // Refresh modelsFlowLayoutPanel
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "createModel" }, "AIToolsForm");
        }

        public static NaiveBayesModel createNBModel(string stepName, List<Sample> dataList, bool pcaActive)
        {
            // Compute PCA loading scores if PCA is active
            List<PCAitem> pca = new List<PCAitem>();
            if (pcaActive)
                // If yes then compute PCA loading scores
                pca = DataVisualisationForm.getPCA(dataList);
            bool regression = false;
            int output;

            if (stepName.Equals(ARTHTNamings.Step1RPeaksScanData) || stepName.Equals(ARTHTNamings.Step3BeatPeaksScanData) || stepName.Equals(ARTHTNamings.Step6UpstrokesScanData))
                regression = true;

            if (stepName.Equals(ARTHTNamings.Step1RPeaksScanData) || stepName.Equals(ARTHTNamings.Step3BeatPeaksScanData) || stepName.Equals(ARTHTNamings.Step4PTSelectionData))
                output = 2;
            else
                output = 1;

            NaiveBayesModel tempModel = createNBModel(stepName, regression, output);
            tempModel._pcaActive = pcaActive;
            tempModel.PCA = pca;

            return tempModel;
        }

        public static NaiveBayesModel createNBModel(string name, bool regression, int outputNumber)
        {
            // Create a KNN model structure with the initial optimum K
            NaiveBayesModel model = new NaiveBayesModel { Name = name, _regression = regression };
            // Set initial thresholds for output decisions
            model.OutputsThresholds = new float[outputNumber];
            for (int i = 0; i < outputNumber; i++)
                model.OutputsThresholds[i] = 0.5f;

            return model;
        }

        public void initializeNeuralNetworkModelsForWPW(ARTHTModels arthtModels)
        {
            // Insert models in _arthtModelsDic
            _arthtModelsDic.Add(arthtModels.Name, arthtModels);
        }

        //*******************************************************************************************************//
        //*******************************************NAIVE BAYES TOOLS*******************************************//
        private static List<Partition[]> partitionOutputs(List<Sample> dataList, bool forRegression, List<Partition[]> outputsProbaList)
        {
            if (dataList.Count == 0)
                return new List<Partition[]>();

            int numberOfFeatures = dataList[0].getFeatures().Length;
            int numberOfOutputs = dataList[0].getOutputs().Length;

            if (outputsProbaList.Count == 0)
            {
                outputsProbaList = new List<Partition[]>();
                // Check if this partitionning is for regression
                if (forRegression)
                {
                    // This is for regression partitionning
                    // Create output arrays from featuresList
                    object[] outputsVals = new object[numberOfOutputs];
                    for (int j = 0; j < numberOfOutputs; j++)
                        outputsVals[j] = new double[dataList.Count];
                    for (int i = 0; i < dataList.Count; i++)
                    {
                        for (int j = 0; j < numberOfOutputs; j++)
                            ((double[])outputsVals[j])[i] = dataList[i].getOutputs()[j];
                    }
                    // Create partitions for each output according to if the model is for regression
                    foreach (double[] outputVals in outputsVals)
                    {
                        double min = double.PositiveInfinity;
                        double max = double.NegativeInfinity;
                        foreach (double val in outputVals)
                        {
                            if (val < min)
                                min = val;
                            if (val > max)
                                max = val;
                        }
                        outputsProbaList.Add(partitionContinuedVals(min, max, outputVals.Length, numberOfFeatures));
                    }
                }
                else
                    // This is for classification
                    for (int i = 0; i < numberOfOutputs; i++)
                    {
                        outputsProbaList.Add(new Partition[] { createPartition(numberOfFeatures, 0, 1), createPartition(numberOfFeatures, 1, 1) });
                    }
            }

            // Set proba properties
            int classIndx;
            foreach (Sample sample in dataList)
            {
                double[] features = sample.getFeatures();
                double[] outputs = sample.getOutputs();
                // Iterate through each output from current feature
                for (int i = 0; i < numberOfOutputs; i++)
                {
                    // Get corresponding class of this feature output
                    // classInd = (output_val - first_partition_val) / partitions_size
                    classIndx = (int)((outputs[i] - outputsProbaList[i][0]._value) / outputsProbaList[i][0]._partitionSize);
                    if ((outputs[i] - outputsProbaList[i][0]._value == outputsProbaList[i][0]._partitionSize) && forRegression)
                        classIndx--;
                    classIndx = classIndx >= 0 ? classIndx : 0;
                    classIndx = classIndx < outputsProbaList[i].Length ? classIndx : outputsProbaList[i].Length - 1;
                    // Update frequency and proba of the correspoding class of current feature
                    outputsProbaList[i][classIndx]._frequency++;
                    outputsProbaList[i][classIndx]._proba = outputsProbaList[i][classIndx]._frequency / dataList.Count;
                    // Add input values in this class partition
                    for (int j = 0; j < numberOfFeatures; j++)
                        outputsProbaList[i][classIndx].GausParamsInputsGivenOutput[j].ValuesList.Add(features[j]);
                }
            }
            // Set GausParamsInputsGivenOutput for each class
            for (int i = 0; i < numberOfOutputs; i++)
            {
                Partition[] partitions = outputsProbaList[i];
                for (int j = 0; j < partitions.Length; j++)
                {
                    Partition partition = partitions[j];
                    for (int k = 0; k < partition.GausParamsInputsGivenOutput.Length; k++)
                    {
                        gausParamsInputGivenOutput gausParamsInputsGivenOutput = partition.GausParamsInputsGivenOutput[k];
                        double[] inputVals = new double[gausParamsInputsGivenOutput.ValuesList.Count];
                        for (int l = 0; l < inputVals.Length; l++)
                            inputVals[l] = gausParamsInputsGivenOutput.ValuesList[l];
                        outputsProbaList[i][j].GausParamsInputsGivenOutput[k]._mean = (gausParamsInputsGivenOutput._mean + mean(inputVals)) / 2;
                        outputsProbaList[i][j].GausParamsInputsGivenOutput[k]._variance = (gausParamsInputsGivenOutput._variance + variance(gausParamsInputsGivenOutput._mean, inputVals)) / 2;

                        outputsProbaList[i][j].GausParamsInputsGivenOutput[k].ValuesList = null;
                    }
                }
            }

            return outputsProbaList;
        }

        private static Partition[] partitionContinuedVals(double min, double max, int collectionSize, int inputSize)
        {
            // Get partitions number starting from 10 partitions
            int partitions = 1;
            for (int i = 10; i > 0; i--)
            {
                partitions = collectionSize / i;
                if (partitions > 0)
                    break;
            }
            // Create partitions according to min and max
            double partitionSize = (max - min) / partitions;
            Partition[] partition = new Partition[partitions];
            for (int i = 0; i < partitions; i++)
                partition[i] = createPartition(inputSize, min + (partitionSize * i), partitionSize);
            return partition;
        }

        private static Partition createPartition(int inputSize, double partitionValue, double partitionSize)
        {
            Partition partition = new Partition { _value = partitionValue, _partitionSize = partitionSize };
            partition.GausParamsInputsGivenOutput = new gausParamsInputGivenOutput[inputSize];
            for (int i = 0; i < partition.GausParamsInputsGivenOutput.Length; i++)
                partition.GausParamsInputsGivenOutput[i] = new gausParamsInputGivenOutput() { ValuesList = new List<double>() };
            return partition;
        }

        private static double mean(double[] variables)
        {
            double mean = 0;
            foreach (double var in variables)
                mean += var / variables.Length;
            return mean;
        }

        private static double variance(double mean, double[] variables)
        {
            double variance = 0;
            foreach (double var in variables)
                variance += Math.Pow(var - mean, 2) / variables.Length;
            return variance;
        }

        private static double gaussian(double mean, double variance, double x)
        {
            double y = -Math.Pow(x - mean, 2) / (2 * variance);
            y = Math.Exp(y);
            y = (1 / Math.Sqrt(2 * Math.PI * variance)) * y;
            return y;
        }
    }
}

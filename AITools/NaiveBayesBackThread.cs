using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP_Using_AI.AITools
{
    class NaiveBayesBackThread
    {
        private AIBackThreadReportHolder _aiBackThreadReportHolderForAIToolsForm;

        private Hashtable _targetsModelsHashtable = null;

        [Serializable]
        public struct NaiveBayesModel
        {
            public bool regression;
            public List<Partition[]> outputsProbaList; // ( one_partition { value, partition_size, frequency, proba, gausParamsInputGivenOutput[] } )
        }

        [Serializable]
        public struct Partition
        {
            public double value, partitionSize, frequency, proba;
            public gausParamsInputGivenOutput[] gausParamsInputsGivenOutput;
        }

        [Serializable]
        public struct gausParamsInputGivenOutput
        {
            public double mean, variance;
            public List<double> valuesList;
        }

        private struct outputProbaGivenInput
        {
            public double proba;
            public double output;
        }

        public NaiveBayesBackThread(Hashtable targetsModelsHashtable, AIBackThreadReportHolder aiBackThreadReportHolderForAIToolsForm)
        {
            _targetsModelsHashtable = targetsModelsHashtable;
            _aiBackThreadReportHolderForAIToolsForm = aiBackThreadReportHolderForAIToolsForm;
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public void fit(string modelsName, List<object[]>[] featuresLists, long datasetSize, long modelId, List<List<long[]>> trainingDetails, int stepIndx)
        {
            // Iterate through models from the selected ones in _targetsModelsHashtable
            int fitProgress = 0;
            int tolatFitProgress = featuresLists.Length;

            if (stepIndx > -1)
            {
                ((List<object[]>)_targetsModelsHashtable[modelsName])[stepIndx][0] = fit((NaiveBayesModel)((List<object[]>)_targetsModelsHashtable[modelsName])[stepIndx][0], featuresLists[stepIndx],
                                                                                      (List<double[]>)((List<object[]>)_targetsModelsHashtable[modelsName])[stepIndx][1]);
            }
            else
                for (int i = 0; i < ((List<object[]>)_targetsModelsHashtable[modelsName]).Count; i++)
                {
                    // Set the new features probabilities in the model
                    ((List<object[]>)_targetsModelsHashtable[modelsName])[i][0] = fit((NaiveBayesModel)((List<object[]>)_targetsModelsHashtable[modelsName])[i][0], featuresLists[i],
                                                                                      (List<double[]>)((List<object[]>)_targetsModelsHashtable[modelsName])[i][1]);

                    // Update fitProgressBar
                    fitProgress++;
                    if (_aiBackThreadReportHolderForAIToolsForm != null)
                        _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "progress", modelsName, fitProgress, tolatFitProgress }, "AIToolsForm");
                }
            // Update model in models table
            List<NaiveBayesModel> modelsOnlyList = new List<NaiveBayesModel>();
            foreach (object[] modelBuff in (List<object[]>)_targetsModelsHashtable[modelsName])
                modelsOnlyList.Add((NaiveBayesModel)modelBuff[0]);
            DbStimulator dbStimulator = new DbStimulator();
            if (trainingDetails.Count > 0)
                dbStimulator.initialize("models", new string[] { "the_model", "dataset_size", "model_updates", "trainings_details" },
                    new Object[] { Garage.ObjectToByteArray(modelsOnlyList), datasetSize, trainingDetails.Count, Garage.ObjectToByteArray(trainingDetails) }, modelId, "TFBackThread");
            else
                dbStimulator.initialize("models", new string[] { "the_model" },
                    new Object[] { Garage.ObjectToByteArray(modelsOnlyList) }, modelId, "TFBackThread");
            dbStimulator.run();

            // Send report about fitting is finished and models table should be updated
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "fitting_complete", modelsName, datasetSize }, "AIToolsForm");
        }

        public static NaiveBayesModel fit(NaiveBayesModel model, List<object[]> featuresList, List<double[]> pcLoadingScores)
        {
            featuresList = Garage.rearrangeFeaturesInput(featuresList, pcLoadingScores);
            // Set the new features probabilities in the model
            bool forRegression = model.regression;
            model = new NaiveBayesModel { regression = forRegression, outputsProbaList = partitionOutputs(featuresList, forRegression, model.outputsProbaList) };

            return model;
        }

        public static double[] predict(double[] input, NaiveBayesModel naiveBayesModel, List<double[]> pcLoadingScores)
        {
            // Initialize input
            input = Garage.rearrangeInput(input, pcLoadingScores);
            // Predict the input
            // Get gausParamsInputsGivenOutput for each class
            // and calculate the probabilities for each val of input
            List<Partition[]> outputsProbaList = naiveBayesModel.outputsProbaList;
            object[] outputProbaGivenInput = new object[outputsProbaList.Count];
            for (int i = 0; i < outputsProbaList.Count; i++)
            {
                Partition[] partitions = outputsProbaList[i];
                outputProbaGivenInput[i] = new List<outputProbaGivenInput>();
                for (int j = 0; j < partitions.Length; j++)
                {
                    Partition partition = partitions[j];
                    double proba = partition.proba;
                    for (int k = 0; k < partition.gausParamsInputsGivenOutput.Length; k++)
                        proba *= gaussian(partition.gausParamsInputsGivenOutput[k].mean, partition.gausParamsInputsGivenOutput[k].variance, input[k]);

                    ((List<outputProbaGivenInput>)outputProbaGivenInput[i]).Add(new outputProbaGivenInput { proba = proba, output = partition.value });
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

        public void createNBModelForWPW(List<double[]>[] pcLoadingScores, List<float[]> outputsThresholds)
        {
            // Create neural network models for WPW syndrome detection
            // Create 7 models for { 2 for QRS detection (Threshold_ratio & Hor_threshold, remove_miss_selected_R),
            // 2 for P_T detection (Threshold_ratio & Hor_threshold, P & T states),
            // 1 for short PR detection,
            // 2 for delta deteciton (Acceleration threshold, delta existence) }
            List<object[]> modelsList = new List<object[]> ();
            // Create a KNN models structure with the initial optimum K, which is "3"
            modelsList.Add(new object[] { new NaiveBayesModel { regression = true, outputsProbaList = new List<Partition[]>() }, pcLoadingScores[0], outputsThresholds[0] }); // (15, 2) For R peaks detection
            modelsList.Add(new object[] { new NaiveBayesModel { regression = false, outputsProbaList = new List<Partition[]>() }, pcLoadingScores[1], outputsThresholds[1] }); // (2, 1) For R selection
            modelsList.Add(new object[] { new NaiveBayesModel { regression = true, outputsProbaList = new List<Partition[]>() }, pcLoadingScores[2], outputsThresholds[2] }); // (5, 2) For beat peaks detection
            modelsList.Add(new object[] { new NaiveBayesModel { regression = false, outputsProbaList = new List<Partition[]>() }, pcLoadingScores[3], outputsThresholds[3] }); // (3, 2) For P and T detection
            modelsList.Add(new object[] { new NaiveBayesModel { regression = false, outputsProbaList = new List<Partition[]>() }, pcLoadingScores[4], outputsThresholds[4] }); // (1, 1) For short PR detection
            modelsList.Add(new object[] { new NaiveBayesModel { regression = true, outputsProbaList = new List<Partition[]>() }, pcLoadingScores[5], outputsThresholds[5] }); // (6, 1) For delta detection
            modelsList.Add(new object[] { new NaiveBayesModel { regression = false, outputsProbaList = new List<Partition[]>() }, pcLoadingScores[6], outputsThresholds[6] }); // (6, 1) For WPW syndrome declaration

            // Insert models in _targetsModelsHashtable
            int modelIndx = 0;
            while (_targetsModelsHashtable.ContainsKey("Naive bayes for WPW syndrome detection" + modelIndx))
                modelIndx++;
            _targetsModelsHashtable.Add("Naive bayes for WPW syndrome detection" + modelIndx, modelsList);

            // Save models in models table
            List<NaiveBayesModel> modelsOnlyList = new List<NaiveBayesModel>();
            foreach (object[] modelBuff in modelsList)
                modelsOnlyList.Add((NaiveBayesModel)modelBuff[0]);
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.initialize("models", new string[] { "type_name", "model_target", "the_model", "selected_variables", "outputs_thresholds", "model_path", "dataset_size", "model_updates", "trainings_details" },
                new Object[] { "Naive bayes", "WPW syndrome detection", Garage.ObjectToByteArray(modelsOnlyList), Garage.ObjectToByteArray(pcLoadingScores), Garage.ObjectToByteArray(outputsThresholds), "", 0, 0, Garage.ObjectToByteArray(new List<List<long[]>>()) }, "NBBackThread");
            dbStimulator.run();

            // Refresh modelsFlowLayoutPanel
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "createModel" }, "AIToolsForm");
        }

        public static NaiveBayesModel createNBModel(int step)
        {
            if (step == 1)
                return new NaiveBayesModel { regression = true, outputsProbaList = new List<Partition[]>() }; // (15, 2) For R peaks detection
            else if (step == 2)
                return new NaiveBayesModel { regression = false, outputsProbaList = new List<Partition[]>() }; // (2, 1) For R selection
            else if (step == 3)
                return new NaiveBayesModel { regression = true, outputsProbaList = new List<Partition[]>() }; // (5, 2) For beat peaks detection
            else if (step == 4)
                return new NaiveBayesModel { regression = false, outputsProbaList = new List<Partition[]>() }; // (3, 2) For P and T selection
            else if (step == 5)
                return new NaiveBayesModel { regression = false, outputsProbaList = new List<Partition[]>() }; // (1, 1) For short PR detection
            else if (step == 6)
                return new NaiveBayesModel { regression = true, outputsProbaList = new List<Partition[]>() }; // (6, 1) For delta detection
            else
                return new NaiveBayesModel { regression = false, outputsProbaList = new List<Partition[]>() }; // (6, 1) For WPW syndrome declaration
        }

        public void initializeNeuralNetworkModelsForWPW(byte[] modelsListBytes, List<double[]>[] pcLoadingScores, List<float[]> outputsThresholds)
        {
            // Insert models in _targetsModelsHashtable
            List<NaiveBayesModel> modelsOnlyList = (List<NaiveBayesModel>)Garage.ByteArrayToObject(modelsListBytes);
            if (modelsOnlyList == null) return;

            List<object[]> modelsList = new List<object[]>();
            for (int i = 0; i < modelsOnlyList.Count; i++)
                modelsList.Add(new object[] { modelsOnlyList[i], pcLoadingScores[i], outputsThresholds[i] });
            int modelIndx = 0;
            while (_targetsModelsHashtable.ContainsKey("Naive bayes for WPW syndrome detection" + modelIndx))
                modelIndx++;
            _targetsModelsHashtable.Add("Naive bayes for WPW syndrome detection" + modelIndx, modelsList);
        }

        //*******************************************************************************************************//
        //*******************************************NAIVE BAYES TOOLS*******************************************//
        private static List<Partition[]> partitionOutputs(List<object[]> featuresList, bool forRegression, List<Partition[]> outputsProbaList)
        {
            if (featuresList.Count == 0)
                return new List<Partition[]>();

            if (outputsProbaList.Count == 0)
            {
                outputsProbaList = new List<Partition[]>();
                // Check if this partitionning is for regression
                if (forRegression)
                {
                    // This is for regression partitionning
                    // Create output arrays from featuresList
                    object[] outputsVals = new object[((double[])featuresList[0][1]).Length];
                    for (int i = 0; i < outputsVals.Length; i++)
                        outputsVals[i] = new double[featuresList.Count];
                    for (int i = 0; i < featuresList.Count; i++)
                    {
                        for (int j = 0; j < outputsVals.Length; j++)
                            ((double[])outputsVals[j])[i] = ((double[])featuresList[i][1])[j];
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
                        outputsProbaList.Add(partitionContinuedVals(min, max, outputVals.Length, ((double[])featuresList[0][0]).Length));
                    }
                }
                else
                    // This is for classification
                    for (int i = 0; i < ((double[])featuresList[0][1]).Length; i++)
                    {
                        outputsProbaList.Add(new Partition[] { createPartition(((double[])featuresList[0][0]).Length, 0, 1), createPartition(((double[])featuresList[0][0]).Length, 1, 1) });
                    }
            }

            // Set proba properties
            int classIndx;
            foreach (object[] feature in featuresList)
            {
                // Iterate through each output from current feature
                for (int i = 0; i < ((double[])feature[1]).Length; i++)
                {
                    // Get corresponding class of this feature output
                    // classInd = (output_val - first_partition_val) / partitions_size
                    classIndx = (int)((((double[])feature[1])[i] - outputsProbaList[i][0].value) / outputsProbaList[i][0].partitionSize);
                    if ((((double[])feature[1])[i] - outputsProbaList[i][0].value == outputsProbaList[i][0].partitionSize) && forRegression)
                        classIndx--;
                    classIndx = classIndx >= 0 ? classIndx : 0;
                    classIndx = classIndx < outputsProbaList[i].Length ? classIndx : outputsProbaList[i].Length - 1;
                    // Update frequency and proba of the correspoding class of current feature
                    outputsProbaList[i][classIndx].frequency++;
                    outputsProbaList[i][classIndx].proba = outputsProbaList[i][classIndx].frequency / featuresList.Count;
                    // Add input values in this class partition
                    for (int j = 0; j < ((double[])feature[0]).Length; j++)
                        outputsProbaList[i][classIndx].gausParamsInputsGivenOutput[j].valuesList.Add(((double[])feature[0])[j]);
                }
            }
            // Set gausParamsInputsGivenOutput for each class
            for (int i = 0; i < outputsProbaList.Count; i++)
            {
                Partition[] partitions = outputsProbaList[i];
                for (int j = 0; j < partitions.Length; j++)
                {
                    Partition partition = partitions[j];
                    for (int k = 0; k < partition.gausParamsInputsGivenOutput.Length; k++)
                    {
                        gausParamsInputGivenOutput gausParamsInputsGivenOutput = partition.gausParamsInputsGivenOutput[k];
                        double[] inputVals = new double[gausParamsInputsGivenOutput.valuesList.Count];
                        for (int l = 0; l < inputVals.Length; l++)
                            inputVals[l] = gausParamsInputsGivenOutput.valuesList[l];
                        outputsProbaList[i][j].gausParamsInputsGivenOutput[k].mean = (gausParamsInputsGivenOutput.mean + mean(inputVals)) / 2;
                        outputsProbaList[i][j].gausParamsInputsGivenOutput[k].variance = (gausParamsInputsGivenOutput.variance + variance(gausParamsInputsGivenOutput.mean, inputVals)) / 2;

                        outputsProbaList[i][j].gausParamsInputsGivenOutput[k].valuesList = null;
                    }
                }
            }

            return outputsProbaList;
        }

        private static Partition[] partitionContinuedVals(double min, double max, int collectionSize, int inputSize)
        {
            // Get partitions number starting from 10 partitions
            int partitions = 1;
            for(int i = 10; i > 0; i--)
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
            Partition partition = new Partition { value = partitionValue, partitionSize = partitionSize };
            partition.gausParamsInputsGivenOutput = new gausParamsInputGivenOutput[inputSize];
            for (int i = 0; i < partition.gausParamsInputsGivenOutput.Length; i++)
                partition.gausParamsInputsGivenOutput[i].valuesList = new List<double>();
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

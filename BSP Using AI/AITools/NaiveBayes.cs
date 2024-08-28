using Biological_Signal_Processing_Using_AI.Garage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace Biological_Signal_Processing_Using_AI.AITools
{
    public class NaiveBayes
    {
        private class outputProbaGivenInput
        {
            public double proba;
            public double output;
        }

        public static NaiveBayesModel fit(NaiveBayesModel model, List<Sample> dataList)
        {
            if (model._pcaActive)
                dataList = GeneralTools.rearrangeFeaturesInput(dataList, model.PCA);

            // Set the new features probabilities in the model
            model.OutputsProbaList = partitionOutputs(dataList, model._regression, model.OutputsProbaList);

            return model;
        }

        public static double[] predict(double[] features, NaiveBayesModel naiveBayesModel)
        {
            // Initialize input
            if (naiveBayesModel._pcaActive)
                features = GeneralTools.rearrangeInput(features, naiveBayesModel.PCA);
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
                    if (outputs[i] - outputsProbaList[i][0]._value == outputsProbaList[i][0]._partitionSize && forRegression)
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
                partition[i] = createPartition(inputSize, min + partitionSize * i, partitionSize);
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
            y = 1 / Math.Sqrt(2 * Math.PI * variance) * y;
            return y;
        }
    }
}

using BSP_Using_AI;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using Keras.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace Biological_Signal_Processing_Using_AI.AITools
{
    public class AIModels
    {
        //_______________________________________________________//
        //::::::::::::::::::::::ARTHT models::::::::::::::::::::://
        [Serializable]
        public class ARTHTModels
        {
            public string Name { get; set; }
            /// <summary>
            /// Training Details:
            /// Each train update creates a list of intervals (List<long[]>) of the _ids of the selected data
            /// </summary>
            public List<List<long[]>> DataIdsIntervalsList { get; set; } = new List<List<long[]>>();

            public Dictionary<string, CustomBaseModel> ARTHTModelsDic = new Dictionary<string, CustomBaseModel>(7)
            {
                { ARTHTNamings.Step1RPeaksScanData, new CustomBaseModel() },
                { ARTHTNamings.Step2RPeaksSelectionData, new CustomBaseModel() },
                { ARTHTNamings.Step3BeatPeaksScanData, new CustomBaseModel() },
                { ARTHTNamings.Step4PTSelectionData, new CustomBaseModel() },
                { ARTHTNamings.Step5ShortPRScanData, new CustomBaseModel() },
                { ARTHTNamings.Step6UpstrokesScanData, new CustomBaseModel() },
                { ARTHTNamings.Step7DeltaExaminationData, new CustomBaseModel() },
            };

            public long _validationTimeCompelxity { get; set; }

            public ARTHTModels Clone()
            {
                ARTHTModels aRTHTModels = new ARTHTModels();
                aRTHTModels.Name = Name;
                aRTHTModels._validationTimeCompelxity = _validationTimeCompelxity;

                aRTHTModels.DataIdsIntervalsList = new List<List<long[]>>();
                foreach (List<long[]> clonedUpdateIntervals in DataIdsIntervalsList)
                {
                    List<long[]> updateIntervals = new List<long[]>();
                    for (int i = 0; i < clonedUpdateIntervals.Count; i++)
                        updateIntervals.Add((long[])clonedUpdateIntervals[i].Clone());
                    aRTHTModels.DataIdsIntervalsList.Add(updateIntervals);
                }

                foreach (string stepName in ARTHTModelsDic.Keys)
                    if (ARTHTModelsDic[stepName].GetType().Name.Equals("KNNModel"))
                        aRTHTModels.ARTHTModelsDic[stepName] = ((KNNModel)ARTHTModelsDic[stepName]).Clone();
                    else if (ARTHTModelsDic[stepName].GetType().Name.Equals("NeuralNetworkModel"))
                        aRTHTModels.ARTHTModelsDic[stepName] = ((NeuralNetworkModel)ARTHTModelsDic[stepName]).Clone();
                    else if (ARTHTModelsDic[stepName].GetType().Name.Equals("NaiveBayesModel"))
                        aRTHTModels.ARTHTModelsDic[stepName] = ((NaiveBayesModel)ARTHTModelsDic[stepName]).Clone();

                return aRTHTModels;
            }
        }
        //_______________________________________________________//
        //::::::::::::::::::::::ARTHT models::::::::::::::::::::://
        [Serializable]
        public class ValidationData
        {
            public string AlgorithmType { get; set; }
            public int _datasetSize { get; set; }
            public int _trainingDatasetSize { get; set; }
            public int _validationDatasetSize { get; set; }
            public double _accuracy { get; set; }
            public double _sensitivity { get; set; }
            public double _specificity { get; set; }

            public ValidationData Clone()
            {
                ValidationData validationData = new ValidationData();
                validationData.AlgorithmType = AlgorithmType;
                validationData._datasetSize = _datasetSize;
                validationData._trainingDatasetSize = _trainingDatasetSize;
                validationData._validationDatasetSize = _validationDatasetSize;
                validationData._accuracy = _accuracy;
                validationData._sensitivity = _sensitivity;
                validationData._specificity = _specificity;
                return validationData;
            }
        }
        //_______________________________________________________//
        //::::::::::::::::::::::Base model::::::::::::::::::::::://
        [Serializable]
        public class CustomBaseModel
        {
            public string Name { get; set; }
            public bool _pcaActive { get; set; } = false;
            public List<PCAitem> PCA { get; set; } = new List<PCAitem>();
            public float[] OutputsThresholds { get; set; }
            public ValidationData ValidationData = new ValidationData();

            public object CloneBase()
            {
                object model = null;
                if (this.GetType().Name.Equals("KNNModel"))
                    model = new KNNModel();
                else if (this.GetType().Name.Equals("NeuralNetworkModel"))
                    model = new ModelLessNeuralNetwork();
                else if (this.GetType().Name.Equals("ModelLessNeuralNetwork"))
                    model = new NeuralNetworkModel();
                else if (this.GetType().Name.Equals("NaiveBayesModel"))
                    model = new NaiveBayesModel();
                ((CustomBaseModel)model).Name = Name;
                ((CustomBaseModel)model)._pcaActive = _pcaActive;

                foreach (PCAitem pcLoadingScores in PCA)
                    ((CustomBaseModel)model).PCA.Add((PCAitem)pcLoadingScores.Clone());

                ((CustomBaseModel)model).OutputsThresholds = (float[])OutputsThresholds.Clone();
                ((CustomBaseModel)model).ValidationData = ValidationData.Clone();

                return model;
            }
        }
        //_______________________________________________________//
        //:::::::::::::::::::::::::::KNN::::::::::::::::::::::::://
        [Serializable]
        public class KNNModel : CustomBaseModel
        {
            public int k;
            public List<Sample> DataList = new List<Sample>();

            public KNNModel Clone()
            {
                KNNModel knnModel = (KNNModel)CloneBase();
                knnModel.k = k;

                return knnModel;
            }
        }

        public class distanteOutput
        {
            public double distance;
            public double[] output;
        }

        public class kError
        {
            public double error;
            public int k;
        }
        //_______________________________________________________//
        //::::::::::::::::::::Neural Network::::::::::::::::::::://
        [Serializable]
        public class NeuralNetworkModel : CustomBaseModel
        {
            public string ModelPath;
            public BaseModel Model = new Sequential();

            public ModelLessNeuralNetwork Clone()
            {
                ModelLessNeuralNetwork neuralNetworkModel = (ModelLessNeuralNetwork)CloneBase();
                neuralNetworkModel.ModelPath = ModelPath;

                return neuralNetworkModel;
            }
        }
        [Serializable]
        public class ModelLessNeuralNetwork : CustomBaseModel
        {
            public string ModelPath;

            public NeuralNetworkModel Clone()
            {
                NeuralNetworkModel neuralNetworkModel = (NeuralNetworkModel)CloneBase();
                neuralNetworkModel.ModelPath = ModelPath;

                return neuralNetworkModel;
            }
        }

        public class QueueSignalInfo
        {
            public string CallingClass;
            public string TargetFunc;

            public string ModelsName;
            public string StepName;

            public List<Sample> DataList;

            public Dictionary<string, List<Sample>> DataLists;
            public long _datasetSize;
            public long _modelId;

            public AutoResetEvent Signal;
            public ConcurrentQueue<QueueSignalInfo> Queue;

            public double[] Features;
            public double[] Outputs;

            public AIToolsForm _aIToolsForm;

            public ARTHTModels aRTHTModels;
        }
        //_______________________________________________________//
        //:::::::::::::::::::::::Naive Bayes::::::::::::::::::::://
        [Serializable]
        public class NaiveBayesModel : CustomBaseModel
        {
            public bool _regression;
            public List<Partition[]> OutputsProbaList = new List<Partition[]>();

            public NaiveBayesModel Clone()
            {
                NaiveBayesModel naiveBayesModel = (NaiveBayesModel)CloneBase();
                naiveBayesModel._regression = _regression;

                naiveBayesModel.OutputsProbaList = new List<Partition[]>();
                foreach (Partition[] clonedPartitions in OutputsProbaList)
                {
                    Partition[] partitions = new Partition[clonedPartitions.Length];
                    for (int i = 0; i < clonedPartitions.Length; i++)
                        partitions[i] = clonedPartitions[i].Clone();
                    naiveBayesModel.OutputsProbaList.Add(partitions);
                }

                return naiveBayesModel;
            }
        }

        [Serializable]
        public class Partition
        {
            public double _value, _partitionSize, _frequency, _proba;
            public gausParamsInputGivenOutput[] GausParamsInputsGivenOutput;

            public Partition Clone()
            {
                Partition partition = new Partition();

                partition._value = _value;
                partition._partitionSize = _partitionSize;
                partition._frequency = _frequency;
                partition._proba = _proba;

                partition.GausParamsInputsGivenOutput = new gausParamsInputGivenOutput[GausParamsInputsGivenOutput.Length];
                for (int i = 0; i < GausParamsInputsGivenOutput.Length; i++)
                    partition.GausParamsInputsGivenOutput[i] = GausParamsInputsGivenOutput[i].Clone();

                return partition;
            }
        }

        [Serializable]
        public class gausParamsInputGivenOutput
        {
            public double _mean, _variance;
            public List<double> ValuesList;

            public gausParamsInputGivenOutput Clone()
            {
                gausParamsInputGivenOutput gausParamsInputGivenOutput = new gausParamsInputGivenOutput();

                gausParamsInputGivenOutput._mean = _mean;
                gausParamsInputGivenOutput._variance = _variance;

                if (ValuesList != null)
                    foreach (double value in ValuesList)
                        gausParamsInputGivenOutput.ValuesList.Add(value);

                return gausParamsInputGivenOutput;
            }
        }
    }
}

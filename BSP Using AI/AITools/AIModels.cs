using BSP_Using_AI;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using Keras.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using Tensorflow;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace Biological_Signal_Processing_Using_AI.AITools
{
    public class AIModels
    {
        //_______________________________________________________//
        //:::::::::::::::::::::ValidationData:::::::::::::::::::://
        [Serializable]
        [DataContract(IsReference = true)]
        public class ValidationData
        {
            [DataMember]
            public string AlgorithmType { get; set; }
            [DataMember]
            public int _datasetSize { get; set; }
            [DataMember]
            public double _trainingDatasetSize { get; set; }
            [DataMember]
            public double _validationDatasetSize { get; set; }
            [DataMember]
            public double _accuracy { get; set; }
            [DataMember]
            public double _sensitivity { get; set; }
            [DataMember]
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
        [DataContract(IsReference = true)]
        public class CustomBaseModel
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public bool _pcaActive { get; set; } = false;
            [DataMember]
            public List<PCAitem> PCA { get; set; } = new List<PCAitem>();
            [DataMember]
            public float[] OutputsThresholds { get; set; }
            [DataMember]
            public ValidationData ValidationData = new ValidationData();

            public object CloneBase()
            {
                object model = null;
                if (this.GetType().Name.Equals("KNNModel"))
                    model = new KNNModel();
                else if (this.GetType().Name.Equals("NeuralNetworkModel"))
                    model = new KerasNETModelLessNeuralNetwork();
                else if (this.GetType().Name.Equals("ModelLessNeuralNetwork"))
                    model = new KerasNETNeuralNetworkModel();
                else if (this.GetType().Name.Equals("NaiveBayesModel"))
                    model = new NaiveBayesModel();
                else if (this.GetType().Name.Equals("TFNETNeuralNetworkModel"))
                    model = new TFNETModelLessNeuralNetwork();
                else if (this.GetType().Name.Equals("TFNETModelLessNeuralNetwork"))
                    model = new TFNETNeuralNetworkModel();
                else if (this.GetType().Name.Equals("TFKerasNeuralNetworkModel"))
                    model = new TFKerasModelLessNeuralNetwork();
                else if (this.GetType().Name.Equals("TFKerasModelLessNeuralNetwork"))
                    model = new TFKerasNeuralNetworkModel();
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
        [DataContract(IsReference = true)]
        public class KNNModel : CustomBaseModel
        {
            [DataMember]
            public static string ModelName = "K-Nearest neighbors";
            [DataMember]
            public int k;
            [DataMember]
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
        [DataContract(IsReference = true)]
        public class KerasNETNeuralNetworkModel : CustomBaseModel
        {
            [DataMember]
            public static string ModelName = "Keras.NET Neural network";
            [DataMember]
            public string ModelPath;
            [DataMember]
            public BaseModel Model = new Sequential();

            public KerasNETModelLessNeuralNetwork Clone()
            {
                KerasNETModelLessNeuralNetwork neuralNetworkModel = (KerasNETModelLessNeuralNetwork)CloneBase();
                neuralNetworkModel.ModelPath = ModelPath;

                return neuralNetworkModel;
            }
        }
        [Serializable]
        [DataContract(IsReference = true)]
        public class KerasNETModelLessNeuralNetwork : CustomBaseModel
        {
            [DataMember]
            public string ModelPath;

            public KerasNETNeuralNetworkModel Clone()
            {
                KerasNETNeuralNetworkModel neuralNetworkModel = (KerasNETNeuralNetworkModel)CloneBase();
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

            public ARTHTModels aRTHTModels { get; set; }
        }
        //_______________________________________________________//
        //:::::::::::::::::::::::Naive Bayes::::::::::::::::::::://
        [Serializable]
        [DataContract(IsReference = true)]
        public class NaiveBayesModel : CustomBaseModel
        {
            [DataMember]
            public static string ModelName = "Naive bayes";
            [DataMember]
            public bool _regression;
            [DataMember]
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
        [DataContract(IsReference = true)]
        public class Partition
        {
            [DataMember]
            public double _value, _partitionSize, _frequency, _proba;
            [DataMember]
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
        [DataContract(IsReference = true)]
        public class gausParamsInputGivenOutput
        {
            [DataMember]
            public double _mean, _variance;
            [DataMember]
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
        //_______________________________________________________//
        //::::::::::::Tensorflow.Net Neural Network:::::::::::::://
        [Serializable]
        [DataContract(IsReference = true)]
        public class TFNETNeuralNetworkModel : CustomBaseModel
        {
            [DataMember]
            public static string ModelName = "TF.NET Neural network";
            [DataMember]
            public string ModelPath;
            [DataMember]
            public Session Session;

            public TFNETModelLessNeuralNetwork Clone()
            {
                TFNETModelLessNeuralNetwork tfNETNeuralNetworkModel = (TFNETModelLessNeuralNetwork)CloneBase();
                tfNETNeuralNetworkModel.ModelPath = ModelPath;

                return tfNETNeuralNetworkModel;
            }
        }
        [Serializable]
        [DataContract(IsReference = true)]
        public class TFNETModelLessNeuralNetwork : CustomBaseModel
        {
            [DataMember]
            public string ModelPath;

            public TFNETNeuralNetworkModel Clone()
            {
                TFNETNeuralNetworkModel tfNETNeuralNetworkModel = (TFNETNeuralNetworkModel)CloneBase();
                tfNETNeuralNetworkModel.ModelPath = ModelPath;

                return tfNETNeuralNetworkModel;
            }
        }
        //_______________________________________________________//
        //::::::::::::Tensorflow.Keras Neural Network:::::::::::://
        [Serializable]
        [DataContract(IsReference = true)]
        public class TFKerasNeuralNetworkModel : CustomBaseModel
        {
            [DataMember]
            public static string ModelName = "TF.Keras Neural network";
            [DataMember]
            public string ModelPath;
            [DataMember]
            public Tensorflow.Keras.Engine.IModel Model;

            public TFKerasModelLessNeuralNetwork Clone()
            {
                TFKerasModelLessNeuralNetwork tfKerasNeuralNetworkModel = (TFKerasModelLessNeuralNetwork)CloneBase();
                tfKerasNeuralNetworkModel.ModelPath = ModelPath;

                return tfKerasNeuralNetworkModel;
            }
        }
        [Serializable]
        [DataContract(IsReference = true)]
        public class TFKerasModelLessNeuralNetwork : CustomBaseModel
        {
            [DataMember]
            public string ModelPath;

            public TFKerasNeuralNetworkModel Clone()
            {
                TFKerasNeuralNetworkModel tfKerasNeuralNetworkModel = (TFKerasNeuralNetworkModel)CloneBase();
                tfKerasNeuralNetworkModel.ModelPath = ModelPath;

                return tfKerasNeuralNetworkModel;
            }
        }
        //_______________________________________________________//
        //:::::::::::::::::Reinforcement learning:::::::::::::::://
        [Serializable]
        [DataContract(IsReference = true)]
        public class ReinforcementL : CustomBaseModel
        {
            [DataMember]
            public static string ModelName = "Reinforcement learning";
        }
    }
}

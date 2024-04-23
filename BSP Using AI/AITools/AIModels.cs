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
        public class CustomArchiBaseModel
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

            protected virtual CustomArchiBaseModel CreateCloneInstance()
            {
                return new CustomArchiBaseModel();
            }
            public virtual CustomArchiBaseModel Clone()
            {
                CustomArchiBaseModel baseArchiModelClone = CreateCloneInstance();

                baseArchiModelClone.Name = Name;
                baseArchiModelClone._pcaActive = _pcaActive;

                foreach (PCAitem pcLoadingScores in PCA)
                    baseArchiModelClone.PCA.Add((PCAitem)pcLoadingScores.Clone());

                if (OutputsThresholds != null)
                    baseArchiModelClone.OutputsThresholds = (float[])OutputsThresholds.Clone();
                baseArchiModelClone.ValidationData = ValidationData.Clone();

                return baseArchiModelClone;
            }
        }
        //_______________________________________________________//
        //:::::::::::::::::::::::::::KNN::::::::::::::::::::::::://
        [Serializable]
        [DataContract(IsReference = true)]
        public class KNNModel : CustomArchiBaseModel
        {
            [DataMember]
            public static string ModelName = "K-Nearest neighbors";
            [DataMember]
            public int k;
            [DataMember]
            public List<Sample> DataList = new List<Sample>();

            protected override CustomArchiBaseModel CreateCloneInstance()
            {
                return new KNNModel();
            }
            public override CustomArchiBaseModel Clone()
            {
                KNNModel knnModel = (KNNModel)base.Clone();
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
        public class KerasNETNeuralNetworkModel : CustomArchiBaseModel
        {
            [DataMember]
            public static string ModelName = "Keras.NET Neural network";
            [DataMember]
            public string ModelPath;
            [IgnoreDataMemberAttribute]
            public BaseModel Model = new Sequential();

            protected override CustomArchiBaseModel CreateCloneInstance()
            {
                return new KerasNETNeuralNetworkModel();
            }
            public override CustomArchiBaseModel Clone()
            {
                KerasNETNeuralNetworkModel neuralNetworkModel = (KerasNETNeuralNetworkModel)base.Clone();
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
        public class NaiveBayesModel : CustomArchiBaseModel
        {
            [DataMember]
            public static string ModelName = "Naive bayes";
            [DataMember]
            public bool _regression;
            [DataMember]
            public List<Partition[]> OutputsProbaList = new List<Partition[]>();

            protected override CustomArchiBaseModel CreateCloneInstance()
            {
                return new NaiveBayesModel();
            }
            public override CustomArchiBaseModel Clone()
            {
                NaiveBayesModel naiveBayesModel = (NaiveBayesModel)base.Clone();
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
        [DataContract(IsReference = true)]
        public class TFNETBaseModel
        {
            [DataMember]
            public string ModelPath;
            [IgnoreDataMemberAttribute]
            public Session Session;

            public TFNETBaseModel(string modelpath)
            {
                ModelPath = modelpath;
            }
        }
        [Serializable]
        [DataContract(IsReference = true)]
        public class TFNETNeuralNetworkModel : CustomArchiBaseModel
        {
            [DataMember]
            public static string ModelName = "TF.NET Neural network";
            [DataMember]
            public TFNETBaseModel BaseModel;

            protected override CustomArchiBaseModel CreateCloneInstance()
            {
                return new TFNETNeuralNetworkModel();
            }
            public override CustomArchiBaseModel Clone()
            {
                TFNETNeuralNetworkModel tfNETNeuralNetworkModel = (TFNETNeuralNetworkModel)base.Clone();
                tfNETNeuralNetworkModel.BaseModel.ModelPath = BaseModel.ModelPath;

                return tfNETNeuralNetworkModel;
            }
        }
        //_______________________________________________________//
        //:::::::::::::::::Reinforcement learning:::::::::::::::://
        [Serializable]
        [DataContract(IsReference = true)]
        public class TFNETReinforcementL : CustomArchiBaseModel
        {
            [DataMember]
            public static string ModelName = "Reinforcement learning";
            [DataMember]
            public TFNETBaseModel BaseModel;

            protected override CustomArchiBaseModel CreateCloneInstance()
            {
                return new TFNETReinforcementL();
            }
            public override CustomArchiBaseModel Clone()
            {
                TFNETReinforcementL tfNETReinforcementLModel = (TFNETReinforcementL)base.Clone();
                tfNETReinforcementLModel.BaseModel = new TFNETBaseModel(BaseModel.ModelPath);

                return tfNETReinforcementLModel;
            }
        }
        //_______________________________________________________//
        //::::::::::::Tensorflow.Keras Neural Network:::::::::::://
        [Serializable]
        [DataContract(IsReference = true)]
        public class TFKerasNeuralNetworkModel : CustomArchiBaseModel
        {
            [DataMember]
            public static string ModelName = "TF.Keras Neural network";
            [DataMember]
            public string ModelPath;
            [IgnoreDataMemberAttribute]
            public Tensorflow.Keras.Engine.IModel Model;

            protected override CustomArchiBaseModel CreateCloneInstance()
            {
                return new TFKerasNeuralNetworkModel();
            }
            public override CustomArchiBaseModel Clone()
            {
                TFKerasNeuralNetworkModel tfKerasNeuralNetworkModel = (TFKerasNeuralNetworkModel)base.Clone();
                tfKerasNeuralNetworkModel.ModelPath = ModelPath;

                return tfKerasNeuralNetworkModel;
            }
        }
    }
}

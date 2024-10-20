using BSP_Using_AI;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using Keras.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Tensorflow;
using Tensorflow.NumPy;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.AITools.ReinforcementLearning.Environment;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives
{
    public class AIModels
    {
        //_______________________________________________________//
        //:::::::::::::::::::::ValidationData:::::::::::::::::::://
        [DataContract(IsReference = true)]
        public class OutputMetrics
        {
            [DataMember]
            public int _truePositive;
            [DataMember]
            public int _trueNegative;
            [DataMember]
            public int _falsePositive;
            [DataMember]
            public int _falseNegative;

            [DataMember]
            public double _classDeviationTolerance;
            [DataMember]
            public double _classDeviationMean;
            [DataMember]
            public double _classDeviationStd;

            [DataMember]
            public double _mase; // Mean absolute scaled error
            [DataMember]
            public double _mae; // Mean absolute error
            [DataMember]
            public double _maeNaive; // Mean absolute error for naive forcast
            [DataMember]
            public int _iSamples;

            public OutputMetrics Clone()
            {
                OutputMetrics clonedOutputMetrics = new OutputMetrics();
                clonedOutputMetrics._truePositive = _truePositive;
                clonedOutputMetrics._trueNegative = _trueNegative;
                clonedOutputMetrics._falsePositive = _falsePositive;
                clonedOutputMetrics._falseNegative = _falseNegative;
                clonedOutputMetrics._classDeviationTolerance = _classDeviationTolerance;
                clonedOutputMetrics._classDeviationMean = _classDeviationMean;
                clonedOutputMetrics._classDeviationStd = _classDeviationStd;
                clonedOutputMetrics._mase = _mase;
                clonedOutputMetrics._mae = _mae;
                clonedOutputMetrics._maeNaive = _maeNaive;
                clonedOutputMetrics._iSamples = _iSamples;
                return clonedOutputMetrics;
            }
        }

        [Serializable]
        [DataContract(IsReference = true)]
        public class ValidationData
        {
            [DataMember]
            public int _datasetSize { get; set; }
            [DataMember]
            public double _trainingDatasetSize { get; set; }
            [DataMember]
            public double _validationDatasetSize { get; set; }
            [DataMember]
            public OutputMetrics[] _ModelOutputsValidMetrics { get; set; }
            [DataMember]
            public double[][] _ConfusionMatrix { get; set; }

            public ValidationData Clone()
            {
                ValidationData validationData = new ValidationData(_ModelOutputsValidMetrics.Length);
                validationData._datasetSize = _datasetSize;
                validationData._trainingDatasetSize = _trainingDatasetSize;
                validationData._validationDatasetSize = _validationDatasetSize;
                validationData._ModelOutputsValidMetrics = _ModelOutputsValidMetrics.Select(outputMetrics => outputMetrics.Clone()).ToArray();
                validationData._ConfusionMatrix = new double[_ConfusionMatrix.GetLength(0)][];
                for (int col = 0; col < _ConfusionMatrix.GetLength(0); col++)
                {
                    validationData._ConfusionMatrix[col] = (double[])_ConfusionMatrix[col].Clone();
                }

                return validationData;
            }

            public ValidationData(int outputDim)
            {
                _ModelOutputsValidMetrics = new OutputMetrics[outputDim];
                for (int i = 0; i < outputDim; i++)
                    _ModelOutputsValidMetrics[i] = new OutputMetrics();
                _ConfusionMatrix = new double[outputDim][];
                for (int col = 0; col < outputDim; col++)
                    _ConfusionMatrix[col] = new double[outputDim];
            }
        }
        //_______________________________________________________//
        //::::::::::::::::::::::Base model::::::::::::::::::::::://

        public class OutputThresholdItem
        {
            public double _highOutputAv = 1;
            public double _lowOutputAv = 0;
            public double _threshold = 0.5d;
            public Dictionary<double, (int _truePositives, int _falsePositives)> _ROC;

            public OutputThresholdItem Clone()
            {
                OutputThresholdItem outputThresholdItem = new OutputThresholdItem();
                outputThresholdItem._highOutputAv = _highOutputAv;
                outputThresholdItem._lowOutputAv = _lowOutputAv;
                outputThresholdItem._threshold = _threshold;

                return outputThresholdItem;
            }
        }

        public enum ObjectiveType
        {
            Classification,
            Regression
        }

        [Serializable]
        [DataContract(IsReference = true)]
        public class CustomArchiBaseModel
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public ObjectiveType Type { get; set; }
            [DataMember]
            public int _inputDim;
            [DataMember]
            public int _outputDim;
            [DataMember]
            public bool _pcaActive { get; set; } = false;
            [DataMember]
            public List<PCAitem> PCA { get; set; } = new List<PCAitem>();
            [DataMember]
            public OutputThresholdItem[] OutputsThresholds { get; set; }
            [DataMember]
            public string[] OutputsNames { get; set; }
            [DataMember]
            public ValidationData ValidationData { get; set; }

            protected virtual CustomArchiBaseModel CreateCloneInstance()
            {
                return new CustomArchiBaseModel(_inputDim, _outputDim, OutputsNames);
            }
            public virtual CustomArchiBaseModel Clone()
            {
                CustomArchiBaseModel baseArchiModelClone = CreateCloneInstance();

                baseArchiModelClone.Name = Name;
                baseArchiModelClone.Type = Type;
                baseArchiModelClone._pcaActive = _pcaActive;

                foreach (PCAitem pcLoadingScores in PCA)
                    baseArchiModelClone.PCA.Add(pcLoadingScores.Clone());

                if (OutputsThresholds != null)
                {
                    baseArchiModelClone.OutputsThresholds = new OutputThresholdItem[OutputsThresholds.Length];
                    for (int i = 0; i < OutputsThresholds.Length; i++)
                        baseArchiModelClone.OutputsThresholds[i] = OutputsThresholds[i].Clone();
                }
                baseArchiModelClone.ValidationData = ValidationData.Clone();

                return baseArchiModelClone;
            }

            public CustomArchiBaseModel(int inputDim, int outputDim, string[] outputNames)
            {
                _inputDim = inputDim;
                _outputDim = outputDim;
                OutputsNames = outputNames;
                ValidationData = new ValidationData(outputDim);
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
                return new KNNModel(_inputDim, _outputDim, OutputsNames);
            }
            public override CustomArchiBaseModel Clone()
            {
                KNNModel knnModel = (KNNModel)base.Clone();
                knnModel.k = k;

                return knnModel;
            }

            public KNNModel(int inputDim, int outputDim, string[] outputNames) : base(inputDim, outputDim, outputNames)
            {

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
            [IgnoreDataMember]
            public BaseModel Model = new Sequential();

            protected override CustomArchiBaseModel CreateCloneInstance()
            {
                return new KerasNETNeuralNetworkModel(_inputDim, _outputDim, OutputsNames);
            }
            public override CustomArchiBaseModel Clone()
            {
                KerasNETNeuralNetworkModel neuralNetworkModel = (KerasNETNeuralNetworkModel)base.Clone();
                neuralNetworkModel.ModelPath = ModelPath;

                return neuralNetworkModel;
            }

            public KerasNETNeuralNetworkModel(int inputDim, int outputDim, string[] outputNames) : base(inputDim, outputDim, outputNames)
            {

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
                return new NaiveBayesModel(_inputDim, _outputDim, OutputsNames);
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

            public NaiveBayesModel(int inputDim, int outputDim, string[] outputNames) : base(inputDim, outputDim, outputNames)
            {

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
            [DataMember]
            public int _inputDim { get; set; }
            [DataMember]
            public int _outputDim { get; set; }
            [IgnoreDataMember]
            public Dictionary<string, NDArray> _AssignedValsDict;
            [IgnoreDataMember]
            public Dictionary<string, Operation> _InitAssignmentsOpDict;
            [IgnoreDataMember]
            public Session Session;

            public TFNETBaseModel(string modelpath, ref int inputDim, ref int outputDim)
            {
                ModelPath = modelpath;
                _inputDim = inputDim;
                _outputDim = outputDim;
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

            public TFNETNeuralNetworkModel(string modelpath, int inputDim, int outputDim, string[] outputNames) : base(inputDim, outputDim, outputNames)
            {
                BaseModel = new TFNETBaseModel(modelpath, ref _inputDim, ref _outputDim);
            }

            protected override CustomArchiBaseModel CreateCloneInstance()
            {
                return new TFNETNeuralNetworkModel(BaseModel.ModelPath, _inputDim, _outputDim, OutputsNames);
            }
            public override CustomArchiBaseModel Clone()
            {
                TFNETNeuralNetworkModel tfNETNeuralNetworkModel = (TFNETNeuralNetworkModel)base.Clone();
                tfNETNeuralNetworkModel.BaseModel = new TFNETBaseModel(BaseModel.ModelPath, ref _inputDim, ref _outputDim);

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
            [DataMember]
            public List<RLDimension> _DimensionsList;

            public TFNETReinforcementL(string modelpath, int inputDim, int outputDim, string[] outputNames, List<RLDimension> dimensionsList) : base(inputDim, outputDim, outputNames)
            {
                BaseModel = new TFNETBaseModel(modelpath, ref _inputDim, ref _outputDim);
                _DimensionsList = dimensionsList;
            }

            protected override CustomArchiBaseModel CreateCloneInstance()
            {
                return new TFNETReinforcementL(BaseModel.ModelPath, _inputDim, _outputDim, OutputsNames, _DimensionsList);
            }
            public override CustomArchiBaseModel Clone()
            {
                TFNETReinforcementL tfNETReinforcementLModel = (TFNETReinforcementL)base.Clone();
                tfNETReinforcementLModel.BaseModel = new TFNETBaseModel(BaseModel.ModelPath, ref _inputDim, ref _outputDim);
                if (_DimensionsList != null)
                    for (int iDimension = 0; iDimension < _DimensionsList.Count; iDimension++)
                        tfNETReinforcementLModel._DimensionsList[iDimension] = _DimensionsList[iDimension].Clone();

                return tfNETReinforcementLModel;
            }
        }
        //_______________________________________________________//
        //:::::::::::::::::Reinforcement learning:::::::::::::::://
        [Serializable]
        [DataContract(IsReference = true)]
        public class TFNETLSTMModel : CustomArchiBaseModel
        {
            [DataMember]
            public static string ModelName = "LSTM";
            [DataMember]
            public TFNETBaseModel BaseModel;
            [DataMember]
            public int _modelSequenceLength = 1;
            [DataMember]
            public bool _bidirectional = false;
            [DataMember]
            public int _layers = 1;

            public TFNETLSTMModel(string modelpath, int inputDim, int outputDim, string[] outputNames, int modelSequenceLength, bool bidirectional = false, int layers = 1) : base(inputDim, outputDim, outputNames)
            {
                BaseModel = new TFNETBaseModel(modelpath, ref _inputDim, ref _outputDim);
                _modelSequenceLength = modelSequenceLength;
                _bidirectional = bidirectional;
                _layers = layers;
            }

            protected override CustomArchiBaseModel CreateCloneInstance()
            {
                return new TFNETLSTMModel(BaseModel.ModelPath, _inputDim, _outputDim, OutputsNames, _modelSequenceLength, _bidirectional, _layers);
            }
            public override CustomArchiBaseModel Clone()
            {
                TFNETLSTMModel tfNETLSTMModel = (TFNETLSTMModel)base.Clone();
                tfNETLSTMModel.BaseModel = new TFNETBaseModel(BaseModel.ModelPath, ref _inputDim, ref _outputDim);
                tfNETLSTMModel._modelSequenceLength = _modelSequenceLength;
                tfNETLSTMModel._bidirectional = _bidirectional;
                tfNETLSTMModel._layers = _layers;

                return tfNETLSTMModel;
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
            [IgnoreDataMember]
            public Tensorflow.Keras.Engine.IModel Model;

            protected override CustomArchiBaseModel CreateCloneInstance()
            {
                return new TFKerasNeuralNetworkModel(_inputDim, _outputDim, OutputsNames);
            }
            public override CustomArchiBaseModel Clone()
            {
                TFKerasNeuralNetworkModel tfKerasNeuralNetworkModel = (TFKerasNeuralNetworkModel)base.Clone();
                tfKerasNeuralNetworkModel.ModelPath = ModelPath;

                return tfKerasNeuralNetworkModel;
            }

            public TFKerasNeuralNetworkModel(int inputDim, int outputDim, string[] outputNames) : base(inputDim, outputDim, outputNames)
            {

            }
        }
    }
}

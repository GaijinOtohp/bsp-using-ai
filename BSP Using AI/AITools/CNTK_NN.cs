/*
 using BSP_Using_AI;
using BSP_Using_AI.AITools;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using CNTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace Biological_Signal_Processing_Using_AI.AITools
{
    internal class CNTK_NN
    {
        private AIBackThreadReportHolder _aiBackThreadReportHolderForAIToolsForm;

        private Dictionary<string, ARTHTModels> _arthtModelsDic = null;

        public CNTK_NN(Dictionary<string, ARTHTModels> arthtModelsDic, AIBackThreadReportHolder aiBackThreadReportHolderForAIToolsForm)
        {
            _arthtModelsDic = arthtModelsDic;
            _aiBackThreadReportHolderForAIToolsForm = aiBackThreadReportHolderForAIToolsForm;
        }

        public void fit(string modelsName, Dictionary<string, List<Sample>> dataLists, long datasetSize, long modelId, string stepName)
        {
            int fitProgress = 0;
            int tolatFitProgress = dataLists.Count;
            // Iterate through models from the selected ones in _targetsModelsHashtable

            // Fit features
            if (!stepName.Equals(""))
            {
                _arthtModelsDic[modelsName].ARTHTModelsDic[stepName] = createCNTKNeuralNetModel(stepName, dataLists[stepName], _arthtModelsDic[modelsName].ARTHTModelsDic[stepName]._pcaActive,
                                                        ((CNTKNeuralNetworkModel)_arthtModelsDic[modelsName].ARTHTModelsDic[stepName]).ModelPath, DataType.Double);
                // Fit features in model
                fit((CNTKNeuralNetworkModel)_arthtModelsDic[modelsName].ARTHTModelsDic[stepName], dataLists[stepName], true);
            }
            else
                foreach (string stepNa in _arthtModelsDic[modelsName].ARTHTModelsDic.Keys)
                {
                    // Fit features in model
                    fit((CNTKNeuralNetworkModel)_arthtModelsDic[modelsName].ARTHTModelsDic[stepNa],
                                                                             dataLists[stepNa], true);

                    // Update fitProgressBar
                    fitProgress++;
                    if (_aiBackThreadReportHolderForAIToolsForm != null)
                        _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "progress", modelsName, fitProgress, tolatFitProgress }, "AIToolsForm");
                }
            // Update model in models table
            DbStimulator dbStimulator = new DbStimulator();
            if (_arthtModelsDic[modelsName].DataIdsIntervalsList.Count > 0)
            {
                Thread dbStimulatorThread = new Thread(() => dbStimulator.Update("models", new string[] { "the_model", "dataset_size" },
                    new Object[] { Garage.ObjectToByteArray(_arthtModelsDic[modelsName].Clone()), datasetSize }, modelId, "TFBackThread"));
                dbStimulatorThread.Start();
            }

            // Send report about fitting is finished and models table should be updated
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "fitting_complete", modelsName, datasetSize }, "AIToolsForm");
        }

        public static CNTKNeuralNetworkModel fit(CNTKNeuralNetworkModel model, List<Sample> dataList, bool saveModel)
        {
            if (model._pcaActive)
                dataList = Garage.rearrangeFeaturesInput(dataList, model.PCA);

            if (dataList.Count > 0)
            {
                // Sort features as inputs (x) and outputs (y)
                double[][] allFeatures = new double[dataList.Count][];
                double[][] allOutputs = new double[dataList.Count][];
                for (int j = 0; j < dataList.Count; j++)
                {
                    allFeatures[j] = dataList[j].getFeatures();
                    allOutputs[j] = dataList[j].getOutputs();
                }

                // Define the loss and evaluation functions
                Function lossFunc = CNTKLib.CrossEntropyWithSoftmax(model.Model, model.Model.Output);
                Function evalFunc = CNTKLib.ClassificationError(model.Model, model.Model.Output);

                // Create a learner and a trainer
                double learningRate = 0.001d;
                List<Learner> learner = new List<Learner>() { Learner.SGDLearner(model.Model.Parameters(), new TrainingParameterScheduleDouble(learningRate)) };
                Trainer trainer = Trainer.CreateTrainer(model.Model, lossFunc, evalFunc, learner);

                // Train the model
                int numEpochs = 100;

                // Prepare input data
                Variable inputVariable = model.Model.Arguments[0];
                Variable outputVariable = model.Model.Output;

                for (int epoch = 0; epoch < numEpochs; epoch++)
                {
                    Value inputBatchValue = Value.CreateBatch(inputVariable.Shape, allFeatures.SelectMany(innerArray => innerArray), model.ComputingDevice);
                    Value outputBatchValue = Value.CreateBatch(outputVariable.Shape, allOutputs.SelectMany(innerArray => innerArray), model.ComputingDevice);

                    Dictionary<Variable, Value> arguments = new Dictionary<Variable, Value>()
                    {
                        { inputVariable, inputBatchValue },
                        { outputVariable, outputBatchValue }
                    };

                    trainer.TrainMinibatch(arguments, true, model.ComputingDevice);
                    if ((epoch % 20) == 0 && trainer.PreviousMinibatchSampleCount() != 0)
                    {
                        float trainLossValue = (float)trainer.PreviousMinibatchLossAverage();
                        float evaluationValue = (float)trainer.PreviousMinibatchEvaluationAverage();
                        Console.WriteLine($"Minibatch: {epoch} CrossEntropyLoss = {trainLossValue}, EvaluationCriterion = {evaluationValue}");
                    }
                }
                // Save model
                if (saveModel)
                    model.Model.Save(model.ModelPath);
            }

            return model;
        }

        public static double[] predict(double[] features, CNTKNeuralNetworkModel model)
        {
            // Initialize input
            if (model._pcaActive)
                features = Garage.rearrangeInput(features, model.PCA);

            // Define the input and output variables
            Variable inputVariable = model.Model.Arguments.Single();
            Variable outputVariable = model.Model.Output;

            // Create a new computation graph for the prediction function
            Function predictionFunction = model.Model.Clone(ParameterCloningMethod.Freeze);

            // Prepare input data
            Value inputVal = Value.CreateBatch(inputVariable.Shape, features, model.ComputingDevice);

            // Evaluate the model to obtain the output
            Dictionary<Variable, Value> inputDataMap = new Dictionary<Variable, Value>() { { inputVariable, inputVal } };
            Dictionary<Variable, Value> outputDataMap = new Dictionary<Variable, Value>() { { outputVariable, null } };
            predictionFunction.Evaluate(inputDataMap, outputDataMap, model.ComputingDevice);
            double[] outputData = outputDataMap[outputVariable].GetDenseData<double>(outputVariable)[0].ToArray();

            // Return result to main user interface
            return outputData;
        }

        public void createCNTKNeuralNetworkModelForWPW()
        {
            // Set models in name and path
            int modelIndx = 0;
            while (_arthtModelsDic.ContainsKey("CNTK Neural network for WPW syndrome detection" + modelIndx))
                modelIndx++;
            string modelPath = System.IO.Directory.GetCurrentDirectory() + @"/AIModels/CNTKModels/NN/WPW" + modelIndx + "/";

            // Create neural network models for WPW syndrome detection
            // Create 7 models for { 2 for QRS detection (Threshold_ratio & Hor_threshold, remove_miss_selected_R),
            // 2 for P_T detection (Threshold_ratio & Hor_threshold, P & T states),
            // 1 for short PR detection,
            // 2 for delta deteciton (Acceleration threshold, delta existence) }
            ARTHTModels arthtModels = new ARTHTModels();
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step1RPeaksScanData] = createCNTKNeuralNetModel(ARTHTNamings.Step1RPeaksScanData, modelPath + 0, 15, 2, DataType.Double); // For R peaks detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step2RPeaksSelectionData] = createCNTKNeuralNetModel(ARTHTNamings.Step2RPeaksSelectionData, modelPath + 1, 2, 1, DataType.Double); // For R selection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step3BeatPeaksScanData] = createCNTKNeuralNetModel(ARTHTNamings.Step3BeatPeaksScanData, modelPath + 2, 5, 2, DataType.Double); // For beat peaks detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step4PTSelectionData] = createCNTKNeuralNetModel(ARTHTNamings.Step4PTSelectionData, modelPath + 3, 3, 2, DataType.Double); // For P and T detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step5ShortPRScanData] = createCNTKNeuralNetModel(ARTHTNamings.Step5ShortPRScanData, modelPath + 4, 1, 1, DataType.Double); // For short PR detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step6UpstrokesScanData] = createCNTKNeuralNetModel(ARTHTNamings.Step6UpstrokesScanData, modelPath + 5, 6, 1, DataType.Double); // For delta detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step7DeltaExaminationData] = createCNTKNeuralNetModel(ARTHTNamings.Step7DeltaExaminationData, modelPath + 6, 6, 1, DataType.Double); // For WPW syndrome declaration

            arthtModels.Name = "CNTK Neural network for WPW syndrome detection" + modelIndx;
            _arthtModelsDic.Add(arthtModels.Name, arthtModels);

            // Save path in models table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.Insert("models", new string[] { "type_name", "model_target", "the_model", "dataset_size" },
                new Object[] { "CNTK Neural network", "WPW syndrome detection", Garage.ObjectToByteArray(arthtModels.Clone()), 0 }, "AIToolsForm");

            // Refresh modelsFlowLayoutPanel
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "createModel" }, "AIToolsForm");
        }

        public static CNTKNeuralNetworkModel createCNTKNeuralNetModel(string stepName, List<Sample> dataList, bool pcaActive, string modelPath, DataType dataType)
        {
            (int inputDim, int outputDim, List<PCAitem> pca) = GetStepDimensions(stepName, dataList, pcaActive);

            CNTKNeuralNetworkModel tempModel = createCNTKNeuralNetModel(stepName, modelPath, inputDim, outputDim, dataType);
            tempModel._pcaActive = pcaActive;
            tempModel.PCA = pca;

            return tempModel;
        }

        public static CNTKNeuralNetworkModel createCNTKNeuralNetModel(string name, string path, int inputDim, int outputDim, DataType dataType)
        {
            CNTKNeuralNetworkModel model = new CNTKNeuralNetworkModel() { Name = name, ModelPath = path };
            // Define the input and output variables
            Variable input = Variable.InputVariable(new int[] { inputDim }, dataType);
            Variable output = Variable.InputVariable(new int[] { outputDim }, dataType);
            // Start from the first hidden layer, since the input is not actually a layer   
            // but inform the shape of the input, with "input" elements.
            int hidLayerDim = (int)((double)inputDim * (2d / 3d) + outputDim);
            // Define the model function
            Function hiddenLayer1 = CNTKLib.Tanh(Layer(input, hidLayerDim, dataType, model.ComputingDevice, "Layer1"), "Layer1"); // The input layer connected to the 1st hidden layer. The 1st layer has "Tanh" as activation function
            Function hiddenLayer2 = Layer(hiddenLayer1, hidLayerDim, dataType, model.ComputingDevice, "Layer2"); // The 1st hidden layer connected to the 2nd hidden layer. The 2nd layer has "linear" as activation function (default)
            model.Model = CNTKLib.HardSigmoid(Layer(hiddenLayer2, outputDim, dataType, model.ComputingDevice, "Layer3"), 0.2f, 0.5f, "Layer3"); // The 2nd hidden layer connected to the output layer. The output layer has "hard sigmoid" as activation function

            //model.Model = Layer(input, outputDim, dataType, model.ComputingDevice, "Layer1");

            // Save the model
            if (path.Length > 0)
                model.Model.Save(path);

            // Set initial thresholds for output decisions
            model.OutputsThresholds = new float[outputDim];
            for (int i = 0; i < outputDim; i++)
                model.OutputsThresholds[i] = 0.5f;

            // Get the parameters
            return model;
        }

        public void initializeCNTKNeuralNetworkModelsForWPW(ARTHTModels arthtModels)
        {
            string[] stepsNames = arthtModels.ARTHTModelsDic.Keys.ToArray();
            foreach (string stepName in stepsNames)
            {
                CNTKNeuralNetworkModel model = ((ModelLessCNTKNeuralNetwork)arthtModels.ARTHTModelsDic[stepName]).Clone();
                model.Model = Function.Load(model.ModelPath, model.ComputingDevice);
                model.ComputingDevice = DeviceDescriptor.CPUDevice;
                arthtModels.ARTHTModelsDic[stepName] = model;
            }

            // Insert models in _arthtModelsDic
            _arthtModelsDic.Add(arthtModels.Name, arthtModels);
        }

        public static Function Layer(Variable input, int outputDim, DataType dataType)
        {
            int inputDim = input.Shape[0];
            // Define the shape of the weights and biases
            int[] weightsShape = new int[] { outputDim, inputDim }; // Shape of the weights: (inputSize, hiddenSize)
            int[] biasesShape = new int[] { outputDim }; // Shape of the biases: (hiddenSize)

            // Create the weights and biases parameters
            Parameter weights = new Parameter(weightsShape, dataType, CNTKLib.GlorotUniformInitializer(CNTKLib.DefaultParamInitScale, CNTKLib.SentinelValueForInferParamInitRank, CNTKLib.SentinelValueForInferParamInitRank, 1));
            Parameter biases = new Parameter(biasesShape, dataType, CNTKLib.ConstantInitializer(0.0));

            // Perform the Times operation with the weights and biases
            Function layer = CNTKLib.Plus(biases, CNTKLib.Times(weights, input));
            return layer;
        }

        public static Function Layer(Variable input, int outputDim, DataType dataType, DeviceDescriptor computingDevice, string name) // DeviceDescriptor can be the CPU or one of the connected GPUs
        {
            int inputDim = input.Shape[0];
            // Define the shape of the weights and biases
            int[] weightsShape = new int[] { outputDim, inputDim }; // Shape of the weights: (inputSize, hiddenSize)
            int[] biasesShape = new int[] { outputDim }; // Shape of the biases: (hiddenSize)

            // Create the weights and biases parameters
            Parameter weights = new Parameter(weightsShape, dataType, 1d, computingDevice, name + " Weights");
            Parameter biases = new Parameter(biasesShape, dataType, 0.0d, computingDevice, name + " Biases");

            // Perform the Times operation with the weights and biases
            Function layer = CNTKLib.Plus(biases, CNTKLib.Times(weights, input), name);
            return layer;
        }
    }
}

 */
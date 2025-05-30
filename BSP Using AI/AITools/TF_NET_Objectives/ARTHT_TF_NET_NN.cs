﻿using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI;
using BSP_Using_AI.AITools;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using Google.Protobuf;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Tensorflow;
using Tensorflow.NumPy;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.AITools.TF_NET_NN;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.AITools.AIBackThreadReportHolder;
using static Tensorflow.Binding;

namespace Biological_Signal_Processing_Using_AI.AITools.Keras_NET_Objectives
{
    public class ARTHT_TF_NET_NN
    {
        private AIBackThreadReportHolder _aiBackThreadReportHolderForAIToolsForm;

        private Dictionary<string, ObjectiveBaseModel> _objectivesModelsDic = null;

        string _SelectedModelName;
        int _currentFitStep;
        int _maxFitSteps;

        public ARTHT_TF_NET_NN(Dictionary<string, ObjectiveBaseModel> objectivesModelsDic, AIBackThreadReportHolder aiBackThreadReportHolderForAIToolsForm)
        {
            _objectivesModelsDic = objectivesModelsDic;
            _aiBackThreadReportHolderForAIToolsForm = aiBackThreadReportHolderForAIToolsForm;
        }

        private void UpdateFittingProgress(int currentProgress, int maxProgress)
        {
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new FittingProgAIReport()
                {
                    ReportType = AIReportType.FittingProgress,
                    ModelName = _SelectedModelName,
                    fitProgress = _currentFitStep * maxProgress + currentProgress,
                    fitMaxProgress = _maxFitSteps * maxProgress
                }, "AIToolsForm");
        }

        public void fit(string modelName, Dictionary<string, List<Sample>> dataLists, long datasetSize, long modelId, string stepName)
        {
            _SelectedModelName = modelName;
            _currentFitStep = 0;
            _maxFitSteps = dataLists.Count;

            // Iterate through models from the selected ones in _targetsModelsHashtable
            ARTHTModels arthtModels = (ARTHTModels)_objectivesModelsDic[modelName];

            // Fit features
            if (!stepName.Equals(""))
            {
                arthtModels.ARTHTModelsDic[stepName] = createTFNETNeuralNetModel(stepName, dataLists[stepName], arthtModels.ARTHTModelsDic[stepName]._pcaActive,
                                                        ((TFNETNeuralNetworkModel)arthtModels.ARTHTModelsDic[stepName]).BaseModel.ModelPath);
                // Fit features in model
                TF_NET_NN.fit(arthtModels.ARTHTModelsDic[stepName], ((TFNETNeuralNetworkModel)arthtModels.ARTHTModelsDic[stepName]).BaseModel, dataLists[stepName], null, saveModel: true);
            }
            else
                foreach (string stepNa in dataLists.Keys)
                {
                    // Fit features in model
                    TF_NET_NN.fit(arthtModels.ARTHTModelsDic[stepNa], ((TFNETNeuralNetworkModel)arthtModels.ARTHTModelsDic[stepNa]).BaseModel,
                                                                             dataLists[stepNa], UpdateFittingProgress, saveModel: true);
                    _currentFitStep++;
                }
            // Update model in models table
            DbStimulator dbStimulator = new DbStimulator();
            if (arthtModels.DataIdsIntervalsList.Count > 0)
            {
                Thread dbStimulatorThread = new Thread(() => dbStimulator.Update("models", new string[] { "the_model", "dataset_size" },
                    new object[] { GeneralTools.ObjectToByteArray(arthtModels.Clone()), datasetSize }, modelId, "TF_NET_NN"));
                dbStimulatorThread.Start();
            }

            // Send report about fitting is finished and models table should be updated
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new FittingCompAIReport()
                                                                    {
                                                                        ReportType = AIReportType.FittingComplete,
                                                                        ModelName = modelName,
                                                                        datasetSize = datasetSize,
                                                                    }, "AIToolsForm");
        }

        public void createTFNETNeuralNetworkModelForWPW()
        {
            // Set models in name and path
            int modelIndx = 0;
            while (_objectivesModelsDic.ContainsKey(TFNETNeuralNetworkModel.ModelName + " for WPW syndrome detection" + modelIndx))
                modelIndx++;
            //string modelPath = System.IO.Directory.GetCurrentDirectory() + @"/AIModels/WPW/TFNETModels/NN" + modelIndx + "/";
            string modelPath = @"./AIModels/WPW/TFNETModels/NN" + modelIndx + "/";

            // Create neural network models for WPW syndrome detection
            // Create 7 models for { 2 for QRS detection (Threshold_ratio & Hor_threshold, remove_miss_selected_R),
            // 2 for P_T detection (Threshold_ratio & Hor_threshold, P & T states),
            // 1 for short PR detection,
            // 2 for delta deteciton (Acceleration threshold, delta existence) }
            ARTHTModels arthtModels = new ARTHTModels();
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step1RPeaksScanData] = createTFNETNeuralNetModel(ARTHTNamings.Step1RPeaksScanData, modelPath + 0, 15, 2, ARTHTNamings.PeaksScannerOutputs.GetNames()); // For R peaks detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step2RPeaksSelectionData] = createTFNETNeuralNetModel(ARTHTNamings.Step2RPeaksSelectionData, modelPath + 1, 2, 1, ARTHTNamings.RSelectionOutputs.GetNames()); // For R selection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step3BeatPeaksScanData] = createTFNETNeuralNetModel(ARTHTNamings.Step3BeatPeaksScanData, modelPath + 2, 5, 2, ARTHTNamings.PeaksScannerOutputs.GetNames()); // For beat peaks detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step4PTSelectionData] = createTFNETNeuralNetModel(ARTHTNamings.Step4PTSelectionData, modelPath + 3, 3, 2, ARTHTNamings.PTSelectionOutputs.GetNames()); // For P and T detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step5ShortPRScanData] = createTFNETNeuralNetModel(ARTHTNamings.Step5ShortPRScanData, modelPath + 4, 1, 1, ARTHTNamings.ShortPROutputs.GetNames()); // For short PR detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step6UpstrokesScanData] = createTFNETNeuralNetModel(ARTHTNamings.Step6UpstrokesScanData, modelPath + 5, 6, 1, ARTHTNamings.UpStrokeOutputs.GetNames()); // For delta detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step7DeltaExaminationData] = createTFNETNeuralNetModel(ARTHTNamings.Step7DeltaExaminationData, modelPath + 6, 6, 1, ARTHTNamings.DeltaExamOutputs.GetNames()); // For WPW syndrome declaration

            arthtModels.ModelName = TFNETNeuralNetworkModel.ModelName;// + " for WPW syndrome detection" + modelIndx;
            arthtModels.ObjectiveName = " for WPW syndrome detection" + modelIndx;
            _objectivesModelsDic.Add(arthtModels.ModelName + arthtModels.ObjectiveName, arthtModels);

            // Save path in models table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.Insert("models", new string[] { "type_name", "model_target", "the_model", "dataset_size" },
                new object[] { TFNETNeuralNetworkModel.ModelName, WPWSyndromeDetection.ObjectiveName, GeneralTools.ObjectToByteArray(arthtModels.Clone()), 0 }, "AIToolsForm");

            // Refresh modelsFlowLayoutPanel
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new AIReport() { ReportType = AIReportType.CreateModel }, "AIToolsForm");
        }

        public static TFNETNeuralNetworkModel createTFNETNeuralNetModel(string stepName, List<Sample> dataList, bool pcaActive, string modelPath)
        {
            (int inputDim, int outputDim, string[] outputNames, List<PCAitem> pca) = GetStepDimensions(stepName, dataList, pcaActive);

            TFNETNeuralNetworkModel tempModel = createTFNETNeuralNetModel(stepName, modelPath, inputDim, outputDim, outputNames);
            tempModel._pcaActive = pcaActive;
            tempModel.PCA = pca;

            return tempModel;
        }

        public static TFNETNeuralNetworkModel createTFNETNeuralNetModel(string name, string path, int inputDim, int outputDim, string[] outputNames)
        {
            TFNETNeuralNetworkModel model = new TFNETNeuralNetworkModel(path, inputDim, outputDim, outputNames) { Name = name };
            if (name.Equals(ARTHTNamings.Step1RPeaksScanData) || name.Equals(ARTHTNamings.Step3BeatPeaksScanData) || name.Equals(ARTHTNamings.Step6UpstrokesScanData))
                model.Type = ObjectiveType.Regression;
            else
                model.Type = ObjectiveType.Classification;

            model.BaseModel.Session = createTFNETNeuralNetModelSession(model.BaseModel);

            // Save the model
            if (path.Length > 0)
                TF_NET_NN.SaveModelVariables(model.BaseModel.Session, path, new string[] { "output" });

            // Set initial thresholds for output decisions
            model.OutputsThresholds = new OutputThresholdItem[outputDim];
            for (int i = 0; i < outputDim; i++)
                model.OutputsThresholds[i] = new OutputThresholdItem();

            // Get the parameters
            return model;
        }
        public static Session createTFNETNeuralNetModelSession(TFNETBaseModel baseModel, Dictionary<string, NDArray> initVarsVals = null)
        {
            // Disable eager mode to enable storing new nodes to the default graph automatically
            tf.compat.v1.disable_eager_execution();

            // The model operations and variables are organized in a graph
            // The graph is automatically built in the default graph
            Graph graph = new Graph().as_default();
            // Define the input and output variables
            int inputDim = baseModel._inputDim;
            int outputDim = baseModel._outputDim;
            Tensor input = tf.placeholder(tf.float32, (-1, inputDim), name: "input_place_holder");
            Tensor outputPlaceholder = tf.placeholder(tf.float32, (-1, outputDim), name: "output_place_holder");
            // Start from the first hidden layer, since the input is not actually a layer   
            // but inform the shape of the input, with "input" elements.
            int hidLayerDim = (int)((float)inputDim * (2f / 3f) + outputDim);
            // Define the model function
            Tensor hiddenLayer1 = tf.nn.tanh(TF_NET_NN.Layer(input, hidLayerDim)); // The input layer connected to the 1st hidden layer. The 1st layer has "Tanh" as activation function
            Tensor hiddenLayer2 = TF_NET_NN.Layer(hiddenLayer1, hidLayerDim); // The 1st hidden layer connected to the 2nd hidden layer. The 2nd layer has "linear" as activation function (default)
            Tensor output = TF_NET_NN.HardSigmoid(TF_NET_NN.Layer(hiddenLayer2, outputDim), "output"); // The 2nd hidden layer connected to the output layer. The output layer has "hard sigmoid" as activation function
            Session session = new Session(graph);
            session.as_default();
            // Assign values to the graph variables
            (Dictionary<string, Operation> initAssignmentsDict, initVarsVals) = TF_NET_NN.AssignValsToVars(session, initVarsVals);
            // Store initVarsVals and their assignments in baseModel
            baseModel._AssignedValsDict = initVarsVals;
            baseModel._InitAssignmentsOpDict = initAssignmentsDict;
            // Insert the cost function operation to the graph of the model
            Tensor costFunc = tf.reduce_mean(tf.square(tf.sub(output, outputPlaceholder)), name: "cost_function");
            // Insert the optimizer operation to the graph of the model with a default learning rate of 0.01
            Tensor learning_rate = tf.placeholder(tf.float32, Shape.Scalar, name: "learning_rate");
            Operation optimizer = tf.train.GradientDescentOptimizer(learning_rate).minimize(costFunc, name: "optimizer");

            return session;
        }

        public void initializeTFNETNeuralNetworkModelsForWPW(ARTHTModels arthtModels)
        {
            string[] stepsNames = arthtModels.ARTHTModelsDic.Keys.ToArray();
            foreach (string stepName in stepsNames)
            {
                TFNETNeuralNetworkModel model = (TFNETNeuralNetworkModel)arthtModels.ARTHTModelsDic[stepName].Clone();
                model.BaseModel.Session = LoadModelVariables(model.BaseModel, createTFNETNeuralNetModelSession);
                arthtModels.ARTHTModelsDic[stepName] = model;
            }

            // Insert models in _arthtModelsDic
            _objectivesModelsDic.Add(arthtModels.ModelName + arthtModels.ObjectiveName, arthtModels);
        }
    }
}

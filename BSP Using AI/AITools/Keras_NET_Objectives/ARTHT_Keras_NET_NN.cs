using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI;
using BSP_Using_AI.AITools;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using Keras;
using Keras.Layers;
using Keras.Models;
using Numpy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.AITools.AIBackThreadReportHolder;

namespace Biological_Signal_Processing_Using_AI.AITools.Keras_NET_Objectives
{
    public class ARTHT_Keras_NET_NN
    {
        public readonly AutoResetEvent _signal = new AutoResetEvent(false);
        public readonly ConcurrentQueue<QueueSignalInfo> _queue = new ConcurrentQueue<QueueSignalInfo>();
        public object PCADataVis { get; private set; }

        public AIBackThreadReportHolder _tFBackThreadReportHolderForAIToolsForm;
        public AIBackThreadReportHolder _tFBackThreadReportHolderForDetailsForm;

        public Dictionary<string, ObjectiveBaseModel> _objectivesModelsDic = null;

        private string _callingClass = null;

        private KerasNETNeuralNetworkModel _tempModel;
        public ARTHT_Keras_NET_NN()
        {
        }

        public void ThreadServer()
        {
            while (true)
            {
                _signal.WaitOne();

                QueueSignalInfo item = null;
                while (_queue.TryDequeue(out item))
                {
                    // Check which function is selected
                    _callingClass = item.CallingClass;
                    switch (item.TargetFunc)
                    {
                        case "fit":
                            // Check if this fit call is from DetailsForm (validation process)
                            if (_callingClass.Equals("DetailsForm"))
                            {
                                // If yes then create new model in _tempModelsList, and fit data inside it
                                _tempModel = createTempNeuralNetworkModelForWPW(item.StepName, item.DataList, ((ARTHTModels)_objectivesModelsDic[item.ModelsName]).ARTHTModelsDic[item.StepName]._pcaActive, "");
                                // Fit features in model
                                _tempModel = Keras_NET_NN.fit(_tempModel, item.DataList, false);
                            }
                            else
                                fit(item.ModelsName, item.DataLists, item._datasetSize, item._modelId, item.StepName);
                            break;
                        case "predict":
                            // Predict input and send it back to caller
                            AutoResetEvent signal = item.Signal;
                            ConcurrentQueue<QueueSignalInfo> queue = item.Queue;
                            // Check if this fit call is from DetailsForm (validation process)
                            if (_callingClass.Equals("DetailsForm"))
                                queue.Enqueue(new QueueSignalInfo { Outputs = Keras_NET_NN.predict(item.Features, _tempModel, true) });
                            else
                                queue.Enqueue(new QueueSignalInfo { Outputs = Keras_NET_NN.predict(item.Features, (KerasNETNeuralNetworkModel)((ARTHTModels)_objectivesModelsDic[item.ModelsName]).ARTHTModelsDic[item.StepName], false) });
                            signal.Set();
                            break;
                        case "createNeuralNetworkModelForWPW":
                            _tFBackThreadReportHolderForAIToolsForm = item._aIToolsForm;
                            createNeuralNetworkModelForWPW();
                            break;
                        case "initializeNeuralNetworkModelsForWPW":
                            initializeNeuralNetworkModelsForWPW(item.aRTHTModels);
                            break;
                        case "Close":
                            List<ObjectiveBaseModel> arthtModels = _objectivesModelsDic.Values.ToList();
                            for (int i = 0; i < arthtModels.Count; i++)
                                if (((ARTHTModels)arthtModels[i]).ARTHTModelsDic.ElementAt(0).Value is KerasNETNeuralNetworkModel)
                                {
                                    List<CustomArchiBaseModel> neuralNetworkModels = ((ARTHTModels)arthtModels[i]).ARTHTModelsDic.Values.ToList();
                                    for (int j = 0; j < neuralNetworkModels.Count; j++)
                                        ((KerasNETNeuralNetworkModel)neuralNetworkModels[j]).Model.Dispose();
                                }
                            return;
                    }
                }
            }
        }

        private void fit(string modelName, Dictionary<string, List<Sample>> dataLists, long datasetSize, long modelId, string stepName)
        {
            int fitProgress = 0;
            int tolatFitProgress = dataLists.Count;
            // Iterate through models from the selected ones in _targetsModelsHashtable

            Dictionary<string, CustomArchiBaseModel> arthtModelsDic = ((ARTHTModels)_objectivesModelsDic[modelName]).ARTHTModelsDic;
            // Fit features
            if (!stepName.Equals(""))
            {
                arthtModelsDic[stepName] = createTempNeuralNetworkModelForWPW(stepName, dataLists[stepName], arthtModelsDic[stepName]._pcaActive,
                                                        ((KerasNETNeuralNetworkModel)arthtModelsDic[stepName]).ModelPath);
                // Fit features in model
                Keras_NET_NN.fit((KerasNETNeuralNetworkModel)arthtModelsDic[stepName], dataLists[stepName], true);
            }
            else
                foreach (string stepNa in arthtModelsDic.Keys)
                {
                    // Fit features in model
                    Keras_NET_NN.fit((KerasNETNeuralNetworkModel)arthtModelsDic[stepNa],
                                                                             dataLists[stepNa], true);

                    // Update fitProgressBar
                    fitProgress++;
                    if (_tFBackThreadReportHolderForAIToolsForm != null)
                        _tFBackThreadReportHolderForAIToolsForm.holdAIReport(new FittingProgAIReport()
                                                                            {
                                                                                ReportType = AIReportType.FittingProgress,
                                                                                ModelName = modelName,
                                                                                fitProgress = fitProgress,
                                                                                fitMaxProgress = tolatFitProgress
                                                                            }, "AIToolsForm");
                }
            // Update model in models table
            DbStimulator dbStimulator = new DbStimulator();
            if (_objectivesModelsDic[modelName].DataIdsIntervalsList.Count > 0)
            {
                Thread dbStimulatorThread = new Thread(() => dbStimulator.Update("models", new string[] { "the_model", "dataset_size" },
                    new Object[] { GeneralTools.ObjectToByteArray(_objectivesModelsDic[modelName].Clone()), datasetSize }, modelId, "TFBackThread"));
                dbStimulatorThread.Start();
            }

            // Send report about fitting is finished and models table should be updated
            if (_tFBackThreadReportHolderForAIToolsForm != null)
                _tFBackThreadReportHolderForAIToolsForm.holdAIReport(new FittingCompAIReport()
                                                                    {
                                                                        ReportType = AIReportType.FittingComplete,
                                                                        ModelName = modelName,
                                                                        datasetSize = datasetSize,
                                                                    }, "AIToolsForm");
        }

        private void createNeuralNetworkModelForWPW()
        {
            // Set models in name and path
            int modelIndx = 0;
            while (_objectivesModelsDic.ContainsKey("Neural network for WPW syndrome detection" + modelIndx))
                modelIndx++;
            //string modelPath = System.IO.Directory.GetCurrentDirectory() + @"/AIModels/TFModels/NN/WPW" + modelIndx + "/";
            string modelPath = "C:/Users/SMURF/Desktop/Tens/AIModels/TFModels/NN/WPW" + modelIndx + "/";

            // Create neural network models for WPW syndrome detection
            // Create 7 models for { 2 for QRS detection (Threshold_ratio & Hor_threshold, remove_miss_selected_R),
            // 2 for P_T detection (Threshold_ratio & Hor_threshold, P & T states),
            // 1 for short PR detection,
            // 2 for delta deteciton (Acceleration threshold, delta existence) }
            ARTHTModels arthtModels = new ARTHTModels();
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step1RPeaksScanData] = createNeuralNetModel(ARTHTNamings.Step1RPeaksScanData, modelPath + 0, 15, 2); // For R peaks detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step2RPeaksSelectionData] = createNeuralNetModel(ARTHTNamings.Step2RPeaksSelectionData, modelPath + 1, 2, 1); // For R selection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step3BeatPeaksScanData] = createNeuralNetModel(ARTHTNamings.Step3BeatPeaksScanData, modelPath + 2, 5, 2); // For beat peaks detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step4PTSelectionData] = createNeuralNetModel(ARTHTNamings.Step4PTSelectionData, modelPath + 3, 3, 2); // For P and T detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step5ShortPRScanData] = createNeuralNetModel(ARTHTNamings.Step5ShortPRScanData, modelPath + 4, 1, 1); // For short PR detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step6UpstrokesScanData] = createNeuralNetModel(ARTHTNamings.Step6UpstrokesScanData, modelPath + 5, 6, 1); // For delta detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step7DeltaExaminationData] = createNeuralNetModel(ARTHTNamings.Step7DeltaExaminationData, modelPath + 6, 6, 1); // For WPW syndrome declaration

            arthtModels.ModelName = KerasNETNeuralNetworkModel.ModelName;
            arthtModels.ObjectiveName = " for WPW syndrome detection" + modelIndx;
            _objectivesModelsDic.Add(arthtModels.ModelName + arthtModels.ObjectiveName, arthtModels);

            // Save path in models table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.Insert("models", new string[] { "type_name", "model_target", "the_model", "dataset_size" },
                new Object[] { "Neural network", "WPW syndrome detection", GeneralTools.ObjectToByteArray(arthtModels.Clone()), 0 }, "AIToolsForm");

            // Refresh modelsFlowLayoutPanel
            if (_tFBackThreadReportHolderForAIToolsForm != null)
                _tFBackThreadReportHolderForAIToolsForm.holdAIReport(new AIReport() { ReportType = AIReportType.CreateModel }, "AIToolsForm");
        }

        private KerasNETNeuralNetworkModel createTempNeuralNetworkModelForWPW(string stepName, List<Sample> dataList, bool pcaActive, string modelPath)
        {
            (int inputDim, int outputDim, List<PCAitem> pca) = GetStepDimensions(stepName, dataList, pcaActive);

            KerasNETNeuralNetworkModel tempModel = createNeuralNetModel(stepName, modelPath, inputDim, outputDim);
            tempModel._pcaActive = pcaActive;
            tempModel.PCA = pca;

            return tempModel;
        }

        private KerasNETNeuralNetworkModel createNeuralNetModel(string name, string path, int input, int output)
        {
            KerasNETNeuralNetworkModel model = new KerasNETNeuralNetworkModel() { Name = name, ModelPath = path };
            Sequential sequential = new Sequential();
            // Start from the first hidden layer, since the input is not actually a layer   
            // but inform the shape of the input, with "input" elements.
            int hidLayer = (int)((double)input * (2d / 3d) + output);
            sequential.Add(new Dense(hidLayer, activation: "tanh", input_shape: new Shape(input))); // Hidden layer 1 with input
            sequential.Add(new Dense(hidLayer, activation: "linear")); // Hidden layer 2
            sequential.Add(new Dense(output, activation: "hard_sigmoid")); // Output layer
            sequential.Compile(optimizer: "sgd", loss: "mean_squared_error");

            // Save the model
            model.Model = sequential;
            if (path.Length > 0)
                model.Model.Save(path);

            // Set initial thresholds for output decisions
            model.OutputsThresholds = new float[output];
            for (int i = 0; i < output; i++)
                model.OutputsThresholds[i] = 0.5f;

            return model;
        }

        private void initializeNeuralNetworkModelsForWPW(ARTHTModels arthtModels)
        {
            string[] stepsNames = arthtModels.ARTHTModelsDic.Keys.ToArray();
            foreach (string stepName in stepsNames)
            {
                KerasNETNeuralNetworkModel model = (KerasNETNeuralNetworkModel)arthtModels.ARTHTModelsDic[stepName].Clone();
                model.Model = Sequential.LoadModel(model.ModelPath);
                arthtModels.ARTHTModelsDic[stepName] = model;
            }

            // Insert models in _arthtModelsDic
            _objectivesModelsDic.Add(arthtModels.ModelName + arthtModels.ObjectiveName, arthtModels);
        }
    }
}

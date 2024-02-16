using Biological_Signal_Processing_Using_AI.Garage;
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
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools
{
    public class TFBackThread
    {
        public readonly AutoResetEvent _signal = new AutoResetEvent(false);
        public readonly ConcurrentQueue<QueueSignalInfo> _queue = new ConcurrentQueue<QueueSignalInfo>();
        public object PCADataVis { get; private set; }

        public AIBackThreadReportHolder _tFBackThreadReportHolderForAIToolsForm;
        public AIBackThreadReportHolder _tFBackThreadReportHolderForDetailsForm;

        public Dictionary<string, ARTHTModels> _arthtModelsDic = null;

        private string _callingClass = null;

        private NeuralNetworkModel _tempModel;
        public TFBackThread()
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
                                _tempModel = createTempNeuralNetworkModelForWPW(item.StepName, item.DataList, _arthtModelsDic[item.ModelsName].ARTHTModelsDic[item.StepName]._pcaActive, "");
                                // Fit features in model
                                _tempModel = fit(_tempModel, item.DataList, false);
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
                                queue.Enqueue(new QueueSignalInfo { Outputs = predict(item.Features, _tempModel, true) });
                            else
                                queue.Enqueue(new QueueSignalInfo { Outputs = predict(item.Features, (NeuralNetworkModel)_arthtModelsDic[item.ModelsName].ARTHTModelsDic[item.StepName], false) });
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
                            List<ARTHTModels> arthtModels = _arthtModelsDic.Values.ToList();
                            for (int i = 0; i < arthtModels.Count; i++)
                                if (arthtModels[i].ARTHTModelsDic.ElementAt(0).Value is NeuralNetworkModel)
                                {
                                    List<CustomBaseModel> neuralNetworkModels = arthtModels[i].ARTHTModelsDic.Values.ToList();
                                    for (int j = 0; j < neuralNetworkModels.Count; j++)
                                        ((NeuralNetworkModel)neuralNetworkModels[j]).Model.Dispose();
                                }
                            return;
                    }
                }
            }
        }

        private void fit(string modelsName, Dictionary<string, List<Sample>> dataLists, long datasetSize, long modelId, string stepName)
        {
            int fitProgress = 0;
            int tolatFitProgress = dataLists.Count;
            // Iterate through models from the selected ones in _targetsModelsHashtable

            // Fit features
            if (!stepName.Equals(""))
            {
                _arthtModelsDic[modelsName].ARTHTModelsDic[stepName] = createTempNeuralNetworkModelForWPW(stepName, dataLists[stepName], _arthtModelsDic[modelsName].ARTHTModelsDic[stepName]._pcaActive,
                                                        ((NeuralNetworkModel)_arthtModelsDic[modelsName].ARTHTModelsDic[stepName]).ModelPath);
                // Fit features in model
                fit((NeuralNetworkModel)_arthtModelsDic[modelsName].ARTHTModelsDic[stepName], dataLists[stepName], true);
            }
            else
                foreach (string stepNa in _arthtModelsDic[modelsName].ARTHTModelsDic.Keys)
                {
                    // Fit features in model
                    fit((NeuralNetworkModel)_arthtModelsDic[modelsName].ARTHTModelsDic[stepNa],
                                                                             dataLists[stepNa], true);

                    // Update fitProgressBar
                    fitProgress++;
                    if (_tFBackThreadReportHolderForAIToolsForm != null)
                        _tFBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "progress", modelsName, fitProgress, tolatFitProgress }, "AIToolsForm");
                }
            // Update model in models table
            DbStimulator dbStimulator = new DbStimulator();
            if (_arthtModelsDic[modelsName].DataIdsIntervalsList.Count > 0)
            {
                Thread dbStimulatorThread = new Thread(() => dbStimulator.Update("models", new string[] { "the_model", "dataset_size" },
                    new Object[] { GeneralTools.ObjectToByteArray(_arthtModelsDic[modelsName].Clone()), datasetSize }, modelId, "TFBackThread"));
                dbStimulatorThread.Start();
            }

            // Send report about fitting is finished and models table should be updated
            if (_tFBackThreadReportHolderForAIToolsForm != null)
                _tFBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "fitting_complete", modelsName, datasetSize }, "AIToolsForm");
        }

        private NeuralNetworkModel fit(NeuralNetworkModel model, List<Sample> dataList, bool saveModel)
        {
            if (model._pcaActive)
                dataList = GeneralTools.rearrangeFeaturesInput(dataList, model.PCA);

            if (dataList.Count > 0)
            {
                // Sort features as inputs (x) and outputs (y)
                double[] features = dataList[0].getFeatures();
                double[] outputs = dataList[0].getOutputs();
                double[,] x = new double[dataList.Count, features.Length];
                double[,] y = new double[dataList.Count, outputs.Length];
                for (int j = 0; j < dataList.Count; j++)
                {
                    features = dataList[j].getFeatures();
                    outputs = dataList[j].getOutputs();
                    for (int k = 0; k < features.Length; k++)
                        x[j, k] = features[k];
                    for (int k = 0; k < outputs.Length; k++)
                        y[j, k] = outputs[k];
                }
                // Now fit data in the model 50 times, each for 10 epochs
                model.Model.Fit(np.array(x), np.array(y), epochs: 1000, verbose: 0);
                // Save model
                if (saveModel)
                    model.Model.Save(model.ModelPath);
            }

            return model;
        }

        private double[] predict(double[] features, NeuralNetworkModel model, bool fromTempModel)
        {
            // Initialize input
            if (model._pcaActive)
                features = GeneralTools.rearrangeInput(features, model.PCA);
            double[,] x = new double[1, features.Length];
            for (int i = 0; i < features.Length; i++)
                x[0, i] = features[i];
            // Predict with the selected model
            NDarray y = model.Model.Predict(x, verbose: 0);
            float[] floatOutput = y.GetData<float>();
            double[] output = new double[floatOutput.Length];
            for (int i = 0; i < output.Length; i++)
                output[i] = floatOutput[i];
            // Return result to main user interface
            return output;
        }

        private void createNeuralNetworkModelForWPW()
        {
            // Set models in name and path
            int modelIndx = 0;
            while (_arthtModelsDic.ContainsKey("Neural network for WPW syndrome detection" + modelIndx))
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

            arthtModels.ModelName = NeuralNetworkModel.ModelName;
            arthtModels.ProblemName = " for WPW syndrome detection" + modelIndx;
            _arthtModelsDic.Add(arthtModels.ModelName + arthtModels.ProblemName, arthtModels);

            // Save path in models table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.Insert("models", new string[] { "type_name", "model_target", "the_model", "dataset_size" },
                new Object[] { "Neural network", "WPW syndrome detection", GeneralTools.ObjectToByteArray(arthtModels.Clone()), 0 }, "AIToolsForm");

            // Refresh modelsFlowLayoutPanel
            if (_tFBackThreadReportHolderForAIToolsForm != null)
                _tFBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "createModel" }, "AIToolsForm");
        }

        private NeuralNetworkModel createTempNeuralNetworkModelForWPW(string stepName, List<Sample> dataList, bool pcaActive, string modelPath)
        {
            (int inputDim, int outputDim, List<PCAitem> pca) = GetStepDimensions(stepName, dataList, pcaActive);

            NeuralNetworkModel tempModel = createNeuralNetModel(stepName, modelPath, inputDim, outputDim);
            tempModel._pcaActive = pcaActive;
            tempModel.PCA = pca;

            return tempModel;
        }

        private NeuralNetworkModel createNeuralNetModel(string name, string path, int input, int output)
        {
            NeuralNetworkModel model = new NeuralNetworkModel() { Name = name, ModelPath = path };
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
                NeuralNetworkModel model = ((ModelLessNeuralNetwork)arthtModels.ARTHTModelsDic[stepName]).Clone();
                model.Model = Sequential.LoadModel(model.ModelPath);
                arthtModels.ARTHTModelsDic[stepName] = model;
            }

            // Insert models in _arthtModelsDic
            _arthtModelsDic.Add(arthtModels.ModelName + arthtModels.ProblemName, arthtModels);
        }
    }
}

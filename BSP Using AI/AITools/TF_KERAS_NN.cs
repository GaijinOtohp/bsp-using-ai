using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI;
using BSP_Using_AI.AITools;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tensorflow;
using Tensorflow.Keras.Engine;
using Tensorflow.NumPy;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.AITools.AIBackThreadReportHolder;
using static Tensorflow.Binding;

namespace Biological_Signal_Processing_Using_AI.AITools
{
    internal class TF_KERAS_NN
    {
        private AIBackThreadReportHolder _aiBackThreadReportHolderForAIToolsForm;

        private Dictionary<string, ObjectiveBaseModel> _objectivesModelsDic = null;

        public TF_KERAS_NN(Dictionary<string, ObjectiveBaseModel> objectivesModelsDic, AIBackThreadReportHolder aiBackThreadReportHolderForAIToolsForm)
        {
            _objectivesModelsDic = objectivesModelsDic;
            _aiBackThreadReportHolderForAIToolsForm = aiBackThreadReportHolderForAIToolsForm;
        }

        public void fit(string modelName, Dictionary<string, List<Sample>> dataLists, long datasetSize, long modelId, string stepName)
        {
            int fitProgress = 0;
            int tolatFitProgress = dataLists.Count;
            // Iterate through models from the selected ones in _targetsModelsHashtable
            ARTHTModels arthtModels = (ARTHTModels)_objectivesModelsDic[modelName];

            // Fit features
            if (!stepName.Equals(""))
            {
                arthtModels.ARTHTModelsDic[stepName] = createTFKerasNeuralNetModel(stepName, dataLists[stepName], arthtModels.ARTHTModelsDic[stepName]._pcaActive,
                                                        ((TFKerasNeuralNetworkModel)arthtModels.ARTHTModelsDic[stepName]).ModelPath);
                // Fit features in model
                fit((TFKerasNeuralNetworkModel)arthtModels.ARTHTModelsDic[stepName], dataLists[stepName], true);
            }
            else
                foreach (string stepNa in arthtModels.ARTHTModelsDic.Keys)
                {
                    // Fit features in model
                    fit((TFKerasNeuralNetworkModel)arthtModels.ARTHTModelsDic[stepNa],
                                                                             dataLists[stepNa], true);

                    // Update fitProgressBar
                    fitProgress++;
                    if (_aiBackThreadReportHolderForAIToolsForm != null)
                        _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new FittingProgAIReport()
                                                                            {
                                                                                ReportType = AIReportType.FittingProgress,
                                                                                ModelName = modelName,
                                                                                fitProgress = fitProgress,
                                                                                fitMaxProgress = tolatFitProgress
                                                                            }, "AIToolsForm");
                }
            // Update model in models table
            DbStimulator dbStimulator = new DbStimulator();
            if (arthtModels.DataIdsIntervalsList.Count > 0)
            {
                Thread dbStimulatorThread = new Thread(() => dbStimulator.Update("models", new string[] { "the_model", "dataset_size" },
                    new object[] { GeneralTools.ObjectToByteArray(arthtModels.Clone()), datasetSize }, modelId, "TF_NET_KERAS_NN"));
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

        public static TFKerasNeuralNetworkModel fit(TFKerasNeuralNetworkModel model, List<Sample> dataList, bool saveModel = false, int suggestedBatchSize = 4)
        {
            if (model._pcaActive)
                dataList = GeneralTools.rearrangeFeaturesInput(dataList, model.PCA);

            if (dataList.Count > 0)
            {
                // Sort data into inputs and outputs
                int featuresDim = dataList[0].getFeatures().Length;
                int outputsDim = dataList[0].getOutputs().Length;
                float[,] dataInputs = new float[dataList.Count, featuresDim];
                float[,] dataOutputs = new float[dataList.Count, outputsDim];
                for (int j = 0; j < dataList.Count; j++)
                {
                    double[] features = dataList[j].getFeatures();
                    double[] outputs = dataList[j].getOutputs();
                    for (int k = 0; k < features.Length; k++)
                        dataInputs[j, k] = (float)features[k];
                    for (int k = 0; k < outputs.Length; k++)
                        dataOutputs[j, k] = (float)outputs[k];
                }

                // Start training
                float improvementThreshold = 0.0001f;

                // Save weights for restoring the model if the training was bad
                List<NDArray> weights = model.Model.get_weights();

                // Create the early stopping callback (stops training if loss value wasn't improving by at least improvementThreshold for 15 epochs)
                var callbackPars = new Tensorflow.Keras.Callbacks.CallbackParams() { Model = model.Model };
                var earlystoppingCB = new Tensorflow.Keras.Callbacks.EarlyStopping(parameters: callbackPars,
                                                                        monitor: "loss",
                                                                        patience: 15,
                                                                        mode: "min",
                                                                        restore_best_weights: true,
                                                                        //baseline: 0.001f,
                                                                        min_delta: improvementThreshold);

                ICallback historyCB;
                float learningRate = 0.1f;
                List<float> batchesLossList;
                // Give it 7 trials for chosing the best learning rate
                for (int i = 0; i < 7; i++)
                {
                    // Set the learning rate
                    model.Model.compile(optimizer: tf.keras.optimizers.SGD(learningRate), loss: tf.keras.losses.MeanSquaredError());
                    // Fit data
                    historyCB = model.Model.fit(dataInputs, dataOutputs, epochs: 1000, batch_size: suggestedBatchSize, callbacks: new List<Tensorflow.Keras.Engine.ICallback>() { earlystoppingCB });
                    // Check if loss was approaching 0
                    batchesLossList = historyCB.history["loss"];
                    if (batchesLossList[batchesLossList.Count - 1] < batchesLossList[batchesLossList.Count - 2])
                        // If yes then training was OK
                        break;

                    // Reset learning rate for the next trial
                    model.Model.set_weights(weights);
                    learningRate /= 10f;
                }

                // Save model
                if (saveModel)
                    model.Model.save(model.ModelPath);
            }

            return model;
        }

        public static double[] predict(double[] features, TFKerasNeuralNetworkModel model)
        {
            // Initialize input
            if (model._pcaActive)
                features = GeneralTools.rearrangeInput(features, model.PCA);

            // Arrange the features in a float[,]
            float[,] featuresFloat = new float[1, features.Length];
            for (int i = 0; i < features.Length; i++) featuresFloat[0, i] = (float)features[i];
            // Predict the input
            Tensor prediction = model.Model.predict(tf.constant(featuresFloat));

            // Return result to main user interface
            return prediction.numpy().ToMultiDimArray<float>().OfType<float>().Select(val => (double)val).ToArray();
        }

        public void createTFKerasNeuralNetworkModelForWPW()
        {
            // Set models in name and path
            int modelIndx = 0;
            while (_objectivesModelsDic.ContainsKey(TFKerasNeuralNetworkModel.ModelName + " for WPW syndrome detection" + modelIndx))
                modelIndx++;
            //string modelPath = System.IO.Directory.GetCurrentDirectory() + @"/AIModels/TFKerasModels/NN/WPW" + modelIndx + "/";
            string modelPath = @"./AIModels/TFKerasModels/NN/WPW" + modelIndx + "/";

            // Create neural network models for WPW syndrome detection
            // Create 7 models for { 2 for QRS detection (Threshold_ratio & Hor_threshold, remove_miss_selected_R),
            // 2 for P_T detection (Threshold_ratio & Hor_threshold, P & T states),
            // 1 for short PR detection,
            // 2 for delta deteciton (Acceleration threshold, delta existence) }
            ARTHTModels arthtModels = new ARTHTModels();
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step1RPeaksScanData] = createTFKerasNeuralNetModel(ARTHTNamings.Step1RPeaksScanData, modelPath + 0, 15, 2, ARTHTNamings.PeaksScannerOutputs.GetNames()); // For R peaks detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step2RPeaksSelectionData] = createTFKerasNeuralNetModel(ARTHTNamings.Step2RPeaksSelectionData, modelPath + 1, 2, 1, ARTHTNamings.RSelectionOutputs.GetNames()); // For R selection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step3BeatPeaksScanData] = createTFKerasNeuralNetModel(ARTHTNamings.Step3BeatPeaksScanData, modelPath + 2, 5, 2, ARTHTNamings.PeaksScannerOutputs.GetNames()); // For beat peaks detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step4PTSelectionData] = createTFKerasNeuralNetModel(ARTHTNamings.Step4PTSelectionData, modelPath + 3, 3, 2, ARTHTNamings.PTSelectionOutputs.GetNames()); // For P and T detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step5ShortPRScanData] = createTFKerasNeuralNetModel(ARTHTNamings.Step5ShortPRScanData, modelPath + 4, 1, 1, ARTHTNamings.ShortPROutputs.GetNames()); // For short PR detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step6UpstrokesScanData] = createTFKerasNeuralNetModel(ARTHTNamings.Step6UpstrokesScanData, modelPath + 5, 6, 1, ARTHTNamings.UpStrokeOutputs.GetNames()); // For delta detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step7DeltaExaminationData] = createTFKerasNeuralNetModel(ARTHTNamings.Step7DeltaExaminationData, modelPath + 6, 6, 1, ARTHTNamings.DeltaExamOutputs.GetNames()); // For WPW syndrome declaration

            arthtModels.ModelName = TFKerasNeuralNetworkModel.ModelName;// + " for WPW syndrome detection" + modelIndx;
            arthtModels.ObjectiveName = " for WPW syndrome detection";
            while (_objectivesModelsDic.ContainsKey(arthtModels.ModelName + arthtModels.ObjectiveName + modelIndx))
                modelIndx++;
            arthtModels.ObjectiveName = " for WPW syndrome detection" + modelIndx;
            _objectivesModelsDic.Add(arthtModels.ModelName + arthtModels.ObjectiveName, arthtModels);

            // Save path in models table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.Insert("models", new string[] { "type_name", "model_target", "the_model", "dataset_size" },
                new object[] { TFKerasNeuralNetworkModel.ModelName, "WPW syndrome detection", GeneralTools.ObjectToByteArray(arthtModels.Clone()), 0 }, "AIToolsForm");

            // Refresh modelsFlowLayoutPanel
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new AIReport() { ReportType = AIReportType.CreateModel }, "AIToolsForm");
        }

        public static TFKerasNeuralNetworkModel createTFKerasNeuralNetModel(string stepName, List<Sample> dataList, bool pcaActive, string modelPath)
        {
            (int inputDim, int outputDim, string[] outputsNames, List<PCAitem> pca) = GetStepDimensions(stepName, dataList, pcaActive);

            TFKerasNeuralNetworkModel tempModel = createTFKerasNeuralNetModel(stepName, modelPath, inputDim, outputDim, outputsNames);
            tempModel._pcaActive = pcaActive;
            tempModel.PCA = pca;

            return tempModel;
        }

        public static TFKerasNeuralNetworkModel createTFKerasNeuralNetModel(string name, string path, int inputDim, int outputDim, string[] outputsNames)
        {
            TFKerasNeuralNetworkModel model = new TFKerasNeuralNetworkModel(inputDim, outputDim, outputsNames) { Name = name, ModelPath = path };

            model.Model = createTFKerasNeuralNetModel(inputDim, outputDim);

            // Save the model
            if (path.Length > 0)
                model.Model.save(path);

            // Set initial thresholds for output decisions
            model.OutputsThresholds = new OutputThresholdItem[outputDim];
            for (int i = 0; i < outputDim; i++)
                model.OutputsThresholds[i] = new OutputThresholdItem();

            // Get the parameters
            return model;
        }
        public static Tensorflow.Keras.Engine.IModel createTFKerasNeuralNetModel(int inputDim, int outputDim)
        {
            var layers = tf.keras.layers;
            // Define the input variable
            Tensor input = tf.keras.Input((-1, inputDim), name: "input_place_holder");
            // Start from the first hidden layer, since the input is not actually a layer   
            // but inform the shape of the input, with "input" elements.
            int hidLayerDim = (int)((float)inputDim * (2f / 3f) + outputDim);
            // Define the model function
            Tensor hiddenLayer1 = layers.Dense(hidLayerDim, activation: "tanh").Apply(input); // The input layer connected to the 1st hidden layer. The 1st layer has "Tanh" as activation function
            Tensor hiddenLayer2 = layers.Dense(hidLayerDim, activation: "linear").Apply(hiddenLayer1); // The 1st hidden layer connected to the 2nd hidden layer. The 2nd layer has "linear" as activation function (default)
            Tensor output = TF_NET_NN.HardSigmoid(layers.Dense(outputDim, activation: "linear").Apply(hiddenLayer2)); // The 2nd hidden layer connected to the output layer. The output layer has "hard sigmoid" as activation function
            var model = tf.keras.Model(input, output);
            // compile keras model in tensorflow static graph
            float learningRate = 0.1f;
            model.compile(optimizer: tf.keras.optimizers.SGD(learningRate), loss: tf.keras.losses.MeanSquaredError());

            return model;
        }

        public void initializeTFKerasNeuralNetworkModelsForWPW(ARTHTModels arthtModels)
        {
            string[] stepsNames = arthtModels.ARTHTModelsDic.Keys.ToArray();
            foreach (string stepName in stepsNames)
            {
                TFKerasNeuralNetworkModel model = (TFKerasNeuralNetworkModel)arthtModels.ARTHTModelsDic[stepName].Clone();
                model.Model = tf.keras.models.load_model(model.ModelPath);
                arthtModels.ARTHTModelsDic[stepName] = model;
            }

            // Insert models in _arthtModelsDic
            _objectivesModelsDic.Add(arthtModels.ModelName + arthtModels.ObjectiveName, arthtModels);
        }
    }
}

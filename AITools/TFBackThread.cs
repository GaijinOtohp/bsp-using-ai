using BSP_Using_AI.AITools.Details;
using Keras;
using Keras.Layers;
using Keras.Models;
using Numpy;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BSP_Using_AI.AITools
{
    public class TFBackThread
    {
        public readonly AutoResetEvent _signal = new AutoResetEvent(false);
        public readonly ConcurrentQueue<object[]> _queue = new ConcurrentQueue<object[]>();

        public AIBackThreadReportHolder _tFBackThreadReportHolderForAIToolsForm;
        public AIBackThreadReportHolder _tFBackThreadReportHolderForDetailsForm;

        public Hashtable _targetsModelsHashtable = null;

        private string _callingClass = null;

        private BaseModel _tempModel;
        public TFBackThread()
        {
        }

        public void ThreadServer()
        {
            while (true)
            {
                _signal.WaitOne();

                object[] item = null;
                while (_queue.TryDequeue(out item))
                {
                    // Check which function is selected
                    _callingClass = (string)item[1];
                    switch ((string)item[0])
                    {
                        case "fit":
                            // Check if this fit call is from DetailsForm (validation process)
                            if (_callingClass.Equals("DetailsForm"))
                            {
                                // If yes then create new model in _tempModelsList, and fit data inside it
                                _tempModel = createNeuralNetworkModel((int)item[2], (string)item[5]);
                                // Fit features in model
                                _tempModel = fit( _tempModel, (List<object[]>)item[4], (List<double[]>)((List<object[]>)_targetsModelsHashtable[(string)item[5]])[(int)item[2] - 1][1], null, false);
                            }
                            else
                                fit((string)item[1], (List<object[]>[])item[2], (string)item[3], (long)item[4], (long)item[5], (List<List<long[]>>)item[6], (int)item[7]);
                            break;
                        case "predict":
                            // Predict input and send it back to caller
                            AutoResetEvent signal = (AutoResetEvent)item[5];
                            ConcurrentQueue<object[]> queue = (ConcurrentQueue<object[]>)item[6];
                            // Check if this fit call is from DetailsForm (validation process)
                            if (_callingClass.Equals("DetailsForm"))
                                queue.Enqueue(new object[] { predict((double[])item[2], (string)item[3], (int)item[4], true) });
                            else
                                queue.Enqueue(new object[] { predict((double[])item[2], (string)item[3], (int)item[4], false) });
                            signal.Set();
                            break;
                        case "createNeuralNetworkModelForWPW":
                            _tFBackThreadReportHolderForAIToolsForm = (AIToolsForm)item[2];
                            createNeuralNetworkModelForWPW((List<double[]>[])item[3], (List<float[]>)item[4]);
                            break;
                        case "initializeNeuralNetworkModelsForWPW":
                            int modelIndx = 0;
                            while (_targetsModelsHashtable.ContainsKey((string)item[2] + modelIndx))
                                modelIndx++;
                            _targetsModelsHashtable.Add((string)item[2] + modelIndx, initializeNeuralNetworkModelsForWPW((string)item[3], (List<double[]>[])item[4], (List<float[]>)item[5]));
                            break;
                    }
                }
            }
        }

        private void fit(string modelsName, List<object[]>[] featuresLists, string modelPath, long datasetSize, long modelId, List<List<long[]>> trainingDetails, int stepIndx)
        {
            int fitProgress = 0;
            int tolatFitProgress = featuresLists.Length;
            // Iterate through models from the selected ones in _targetsModelsHashtable
            BaseModel model = null;

            // Fit features
            if (stepIndx > -1)
            {
                model = createNeuralNetworkModel(stepIndx + 1, modelsName);
                // Fit features in model
                ((List<object[]>)_targetsModelsHashtable[modelsName])[stepIndx][0] = fit(model, featuresLists[stepIndx], (List<double[]>)((List<object[]>)_targetsModelsHashtable[modelsName])[stepIndx][1],
                                                                                  modelPath + stepIndx, true);
            }
            else
                for (int i = 0; i < ((List<object[]>)_targetsModelsHashtable[modelsName]).Count; i++)
                {
                    // Get model and its selected features
                    // ((List<object[]>)_targetsModelsHashtable[modelsName])[i] ====> { model, selectedVars, outputsThresholds }
                    model = (BaseModel)((List<object[]>)_targetsModelsHashtable[modelsName])[i][0];
                    // Fit features in model
                    ((List<object[]>)_targetsModelsHashtable[modelsName])[i][0] = fit(model, featuresLists[i], (List<double[]>)((List<object[]>)_targetsModelsHashtable[modelsName])[i][1],
                                                                                      modelPath + i, true);

                    // Update fitProgressBar
                    fitProgress++;
                    if (_tFBackThreadReportHolderForAIToolsForm != null)
                        _tFBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "progress", modelsName, fitProgress, tolatFitProgress }, "AIToolsForm");
                }
            // Update model in models table
            DbStimulator dbStimulator = new DbStimulator();
            if (trainingDetails.Count > 0)
                dbStimulator.initialize("models", new string[] { "dataset_size", "model_updates", "trainings_details" },
                    new Object[] { datasetSize, trainingDetails.Count, Garage.ObjectToByteArray(trainingDetails) }, modelId, "TFBackThread");
            Thread dbStimulatorThread = new Thread(new ThreadStart(dbStimulator.run));
            dbStimulatorThread.Start();

            // Send report about fitting is finished and models table should be updated
            if (_tFBackThreadReportHolderForAIToolsForm != null)
                _tFBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "fitting_complete", modelsName, datasetSize }, "AIToolsForm");
        }

        private BaseModel fit(BaseModel model, List<object[]> featuresList, List<double[]> pcLoadingScores, string modelPath, bool saveModel)
        {
            featuresList = Garage.rearrangeFeaturesInput(featuresList, pcLoadingScores);
            if (featuresList.Count > 0)
            {
                // Sort features as inputs (x) and outputs (y)
                double[,] x = new double[featuresList.Count, ((double[])featuresList[0][0]).Length];
                double[,] y = new double[featuresList.Count, ((double[])featuresList[0][1]).Length];
                for (int j = 0; j < featuresList.Count; j++)
                {
                    for (int k = 0; k < ((double[])featuresList[j][0]).Length; k++)
                        x[j, k] = ((double[])featuresList[j][0])[k];
                    for (int k = 0; k < ((double[])featuresList[j][1]).Length; k++)
                        y[j, k] = ((double[])featuresList[j][1])[k];
                }
                // Now fit data in the model 50 times, each for 10 epochs
                model.Fit(np.array(x), np.array(y), epochs: 1000, verbose: 0);
                // Save model
                if (saveModel)
                    model.Save(modelPath);
            }

            return model;
        }

        private double[] predict(double[] input, string modelType, int step, bool fromTempModel)
        {
            // Initialize input
            input = Garage.rearrangeInput(input, (List<double[]>)((List<object[]>)_targetsModelsHashtable[modelType])[step][1]);
            double[,] x = new double[1, input.Length];
            for (int i = 0; i < input.Length; i++)
                x[0, i] = input[i];
            // Predict with the selected model
            NDarray y = null;
            if (fromTempModel)
                y = _tempModel.Predict(x, verbose: 0);
            else
                // ((List<object[]>)_targetsModelsHashtable[modelsName])[step] ====> { model, selectedVars, outputsThresholds }
                y = ((BaseModel)((List<object[]>)_targetsModelsHashtable[modelType])[step][0]).Predict(x, verbose: 0);
            float[] floatOutput = y.GetData<float>();
            double[] output = new double[floatOutput.Length];
            for (int i = 0; i < output.Length; i++)
                output[i] = floatOutput[i];
            // Return result to main user interface
            return output;
        }

        private void createNeuralNetworkModelForWPW(List<double[]>[] pcLoadingScores, List<float[]> outputsThresholds)
        {
            // Create neural network models for WPW syndrome detection
            // Create 7 models for { 2 for QRS detection (Threshold_ratio & Hor_threshold, remove_miss_selected_R),
            // 2 for P_T detection (Threshold_ratio & Hor_threshold, P & T states),
            // 1 for short PR detection,
            // 2 for delta deteciton (Acceleration threshold, delta existence) }
            List<object[]> modelsList = new List<object[]>();
            modelsList.Add(new object[] { createNeuralNetModel(15, 2), pcLoadingScores[0], outputsThresholds[0] }); // For R peaks detection
            modelsList.Add(new object[] { createNeuralNetModel(2, 1), pcLoadingScores[1], outputsThresholds[1] }); // For R selection
            modelsList.Add(new object[] { createNeuralNetModel(5, 2), pcLoadingScores[2], outputsThresholds[2] }); // For beat peaks detection
            modelsList.Add(new object[] { createNeuralNetModel(3, 2), pcLoadingScores[3], outputsThresholds[3] }); // For P and T detection
            modelsList.Add(new object[] { createNeuralNetModel(1, 1), pcLoadingScores[4], outputsThresholds[4] }); // For short PR detection
            modelsList.Add(new object[] { createNeuralNetModel(6, 1), pcLoadingScores[5], outputsThresholds[5] }); // For delta detection
            modelsList.Add(new object[] { createNeuralNetModel(6, 1), pcLoadingScores[6], outputsThresholds[6] }); // For WPW syndrome declaration

            // Insert models in _targetsModelsHashtable
            int modelIndx = 0;
            while (_targetsModelsHashtable.ContainsKey("Neural network for WPW syndrome detection" + modelIndx))
                modelIndx++;
            _targetsModelsHashtable.Add("Neural network for WPW syndrome detection" + modelIndx, modelsList);

            // Save models
            String modelPath = System.IO.Directory.GetCurrentDirectory() + @"/AIModels/TFModels/NN/WPW" + modelIndx + "/";
            for (int i = 0; i < modelsList.Count; i++)
                ((BaseModel)modelsList[i][0]).Save(modelPath + i);

            // Save path in models table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.initialize("models", new string[] { "type_name", "model_target", "selected_variables", "outputs_thresholds", "model_path", "dataset_size", "model_updates", "trainings_details" },
                new Object[] { "Neural network", "WPW syndrome detection", Garage.ObjectToByteArray(pcLoadingScores), Garage.ObjectToByteArray(outputsThresholds), modelPath, 0, 0, Garage.ObjectToByteArray(new List<List<long[]>>()) }, "AIToolsForm");
            dbStimulator.run();

            // Refresh modelsFlowLayoutPanel
            if (_tFBackThreadReportHolderForAIToolsForm != null)
                _tFBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "createModel" }, "AIToolsForm");
        }

        private BaseModel createNeuralNetworkModel(int step, string modelsName)
        {
            int input = step == 1 ? 15 : step == 2 ? 2 : step == 3 ? 5 : step == 4 ? 3 : step == 5 ? 1 : 6;
            if (((List<double[]>)((List<object[]>)_targetsModelsHashtable[modelsName])[step - 1][1]).Count > 0)
                input = ((List<double[]>)((List<object[]>)_targetsModelsHashtable[modelsName])[step - 1][1]).Count;
            if (step == 1)
                return createNeuralNetModel(input, 2); // For R peaks detection
            else if (step == 2)
                return createNeuralNetModel(input, 1); // For R selection
            else if (step == 3)
                return createNeuralNetModel(input, 2); // For beat peaks detection
            else if (step == 4)
                return createNeuralNetModel(input, 2); // For P and T detection
            else if (step == 5)
                return createNeuralNetModel(input, 1); // For short PR detection
            else if (step == 6)
                return createNeuralNetModel(input, 1); // For delta detection
            else 
                return createNeuralNetModel(input, 1); // For WPW syndrome declaration
        }

        private BaseModel createNeuralNetModel(int input, int output)
        {
            Sequential model = new Sequential();
            // Start from the first hidden layer, since the input is not actually a layer   
            // but inform the shape of the input, with "input" elements.
            int hidLayer = (int)((double)input * (2d / 3d) + output);
            model.Add(new Dense(hidLayer, activation: "tanh", input_shape: new Shape(input))); // Hidden layer 1 with input
            model.Add(new Dense(hidLayer, activation: "linear")); // Hidden layer 2
            model.Add(new Dense(output, activation: "hard_sigmoid")); // Output layer
            model.Compile(optimizer: "sgd", loss: "mean_squared_error");

            return model;
        }

        private List<object[]> initializeNeuralNetworkModelsForWPW(string modelsPath, List<double[]>[] pcLoadingScores, List<float[]> outputsThresholds)
        {
            // Create 7 models for { 2 for QRS detection (Threshold_ratio & Hor_threshold, remove_miss_selected_R),
            // 2 for P_T detection (Threshold_ratio & Hor_threshold, P & T states),
            // 1 for short PR detection,
            // 2 for delta deteciton (Acceleration threshold, delta existence) }
            List<object[]> modelsList = new List<object[]>();

            try
            {
                for (int i = 0; i < 7; i++)
                {
                    BaseModel model = Sequential.LoadModel(modelsPath + i);
                    modelsList.Add(new object[] { model, pcLoadingScores[i], outputsThresholds[i] });
                }
            }
            catch (Exception e)
            {
                return null;
            }

            return modelsList;
        }
    }
}

using BSP_Using_AI.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP_Using_AI.AITools
{
    class KNNBackThread: DbStimulatorReportHolder
    {
        private AIBackThreadReportHolder _aiBackThreadReportHolderForAIToolsForm;

        private Hashtable _targetsModelsHashtable = null;
        private string _selectedModel;

        public struct KNNModel
        {
            public int k;
            public List<object[]> featuresList;
        }

        private struct distanteOutput
        {
            public double distance;
            public double[] output;
        }

        private struct kError
        {
            public double error;
            public int k;
        }

        public KNNBackThread(Hashtable targetsModelsHashtable, AIBackThreadReportHolder aiBackThreadReportHolderForAIToolsForm)
        {
            _targetsModelsHashtable = targetsModelsHashtable;
            _aiBackThreadReportHolderForAIToolsForm = aiBackThreadReportHolderForAIToolsForm;
        }

        public void fit(string modelsName, List<object[]>[] featuresLists, long datasetSize, long modelId, List<List<long[]>> trainingDetails, int stepIndx)
        {
            int fitProgress = 0;
            int tolatFitProgress = featuresLists.Length;

            // Iterate through models from the selected ones in _targetsModelsHashtable
            int[] ks = new int[featuresLists.Length];

            // Fit features
            if (stepIndx > -1)
            {
                ((List<object[]>)_targetsModelsHashtable[modelsName])[stepIndx][0] = fit((KNNModel)((List<object[]>)_targetsModelsHashtable[modelsName])[stepIndx][0],
                                                                                      featuresLists[stepIndx], ks, stepIndx,
                                                                                      (List<double[]>)((List<object[]>)_targetsModelsHashtable[modelsName])[stepIndx][1]);
            }
            else
                for (int i = 0; i < ((List<object[]>)_targetsModelsHashtable[modelsName]).Count; i++)
                {
                    ((List<object[]>)_targetsModelsHashtable[modelsName])[i][0] = fit((KNNModel)((List<object[]>)_targetsModelsHashtable[modelsName])[i][0],
                                                                                      featuresLists[i], ks, i,
                                                                                      (List<double[]>)((List<object[]>)_targetsModelsHashtable[modelsName])[i][1]);

                    // Update fitProgressBar
                    fitProgress++;
                    if (_aiBackThreadReportHolderForAIToolsForm != null)
                        _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "progress", modelsName, fitProgress, tolatFitProgress }, "AIToolsForm");
                }

            // Update model in models table
            DbStimulator dbStimulator = new DbStimulator();
            if (trainingDetails.Count > 0)
                dbStimulator.initialize("models", new string[] { "the_model", "dataset_size", "model_updates", "trainings_details" },
                    new Object[] { Garage.ObjectToByteArray(ks), datasetSize, trainingDetails.Count, Garage.ObjectToByteArray(trainingDetails) }, modelId, "TFBackThread");
            else
                dbStimulator.initialize("models", new string[] { "the_model" },
                    new Object[] { Garage.ObjectToByteArray(ks) }, modelId, "TFBackThread");
            dbStimulator.run();

            // Send report about fitting is finished and models table should be updated
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "fitting_complete", modelsName, datasetSize }, "AIToolsForm");
        }

        public static KNNModel fit(KNNModel model, List<object[]> featuresList, int[] ks, int i, List<double[]> pcLoadingScores)
        {
            featuresList = Garage.rearrangeFeaturesInput(featuresList, pcLoadingScores);
            // Set the new optimal k for this model
            if (featuresList.Count > 0)
                ks[i] = getOptimalK(featuresList.Select((x, y) => new { Index = y, Value = x })
                                                        .GroupBy(x => x.Index / 300)
                                                        .Select(x => x.Select(v => v.Value).ToList())
                                                        .ToList()[0],
                                                        pcLoadingScores);
            else
                ks[i] = 3;
            // Set the new features in the model
            model = new KNNModel { k = ks[i], featuresList = new List<object[]>() };
            foreach (object[] feature in featuresList)
                model.featuresList.Add(feature);

            return model;
        }

        public static double[] predict(double[] input, KNNModel kNNModel, List<double[]> pcLoadingScores)
        {
            // Initialize input
            input = Garage.rearrangeInput(input, pcLoadingScores);
            // Create list for calculating distances between input and saved dataset
            List<distanteOutput> distances = new List<distanteOutput>();
            // Iterate through all saved features and calucalte distance between the input and the saved feature
            double distance;
            foreach (object[] feature in kNNModel.featuresList)
            {
                distance = 0;
                for (int i = 0; i < input.Length; i++)
                    distance += Math.Pow(input[i] - ((double[])feature[0])[i], 2);
                distance = Math.Sqrt(distance);
                // Insert distance and its output in distances
                distances.Add(new distanteOutput { distance = distance, output = (double[])feature[1] });
            }
            // Sort distances in distances
            distances.Sort((e1, e2) => { return e1.distance.CompareTo(e2.distance); });

            // Calculate the average of the first "k" outputs
            double[] output = null;
            if (distances.Count > 0)
                output = new double[distances[0].output.Length];
            int k = kNNModel.k < distances.Count ? kNNModel.k : distances.Count;
            for (int i = 0; i < k; i++)
                for (int j = 0; j < output.Length; j++)
                    output[j] += distances[i].output[j] / k;

            // Return result to main user interface
            return output;
        }

        public void createKNNkModelForWPW(List<double[]>[] pcLoadingScores, List<float[]> outputsThresholds)
        {
            // Create neural network models for WPW syndrome detection
            // Create 7 models for { 2 for QRS detection (Threshold_ratio & Hor_threshold, remove_miss_selected_R),
            // 2 for P_T detection (Threshold_ratio & Hor_threshold, P & T states),
            // 1 for short PR detection,
            // 2 for delta deteciton (Acceleration threshold, delta existence) }
            List<object[]> modelsList = new List<object[]>();
            // Create a KNN models structure with the initial optimum K, which is "3"
            modelsList.Add(new object[] { createKNNModel(3), pcLoadingScores[0], outputsThresholds[0] }); // (15, 2) For R peaks detection
            modelsList.Add(new object[] { createKNNModel(3), pcLoadingScores[1], outputsThresholds[1] }); // (2, 1) For R selection
            modelsList.Add(new object[] { createKNNModel(3), pcLoadingScores[2], outputsThresholds[2] }); // (5, 2) For beat peaks detection
            modelsList.Add(new object[] { createKNNModel(3), pcLoadingScores[3], outputsThresholds[3] }); // (3, 2) For P and T detection
            modelsList.Add(new object[] { createKNNModel(3), pcLoadingScores[4], outputsThresholds[4] }); // (1, 1) For short PR detection
            modelsList.Add(new object[] { createKNNModel(3), pcLoadingScores[5], outputsThresholds[5] }); // (6, 1) For delta detection
            modelsList.Add(new object[] { createKNNModel(3), pcLoadingScores[6], outputsThresholds[6] }); // (6, 1) For WPW syndrome declaration

            // Insert models in _targetsModelsHashtable
            int modelIndx = 0;
            while (_targetsModelsHashtable.ContainsKey("K-Nearest neighbor for WPW syndrome detection" + modelIndx))
                modelIndx++;
            _targetsModelsHashtable.Add("K-Nearest neighbor for WPW syndrome detection" + modelIndx, modelsList);

            // Create array of ints initialized with models optimum k values
            int[] ks = new int[] { 3, 3, 3, 3, 3, 3, 3 };
            // Save models in models table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.initialize("models", new string[] { "type_name", "model_target", "the_model", "selected_variables", "outputs_thresholds", "model_path", "dataset_size", "model_updates", "trainings_details" },
                new Object[] { "K-Nearest neighbor", "WPW syndrome detection", Garage.ObjectToByteArray(ks), Garage.ObjectToByteArray(pcLoadingScores), Garage.ObjectToByteArray(outputsThresholds), "", 0, 0, Garage.ObjectToByteArray(new List<List<long[]>>()) }, "KNNBackThread");
            dbStimulator.run();

            // Refresh modelsFlowLayoutPanel
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "createModel" }, "AIToolsForm");
        }

        public static KNNModel createKNNModel(int k)
        {
            // Create a KNN model structure with the initial optimum K
            return new KNNModel { k = k, featuresList = new List<object[]>() };
        }

        public void initializeNeuralNetworkModelsForWPW(long lastSignalId, int[] ks, List<double[]>[] pcLoadingScores, List<float[]> outputsThresholds)
        {
            // Create model according their k in ks
            List<object[]> modelsList = new List<object[]>();
            for (int i = 0; i < pcLoadingScores.Length; i++)
                modelsList.Add(new object[] { createKNNModel(ks[i]), pcLoadingScores[i], outputsThresholds[i] });
            // Insert models in _targetsModelsHashtable
            int modelIndx = 0;
            while (_targetsModelsHashtable.ContainsKey("K-Nearest neighbor for WPW syndrome detection" + modelIndx))
                modelIndx++;
            _targetsModelsHashtable.Add("K-Nearest neighbor for WPW syndrome detection" + modelIndx, modelsList);
            // Set the selected model name
            _selectedModel = "K-Nearest neighbor for WPW syndrome detection" + modelIndx;

            // Query for all fitted signals in this model in dataset table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            dbStimulator.initialize("dataset",
                                new String[] { "features" },
                                "_id<=?",
                                new Object[] { lastSignalId },
                                "", "KNNBackThread");
            dbStimulator.run();
        }

        private static int getOptimalK(List<object[]> features, List<double[]> pcLoadingScores)
        {
            // Iterate through all possible k values
            List<kError> kErrors = new List<kError>();
            for (int k = 1; k <= 101 && k <= features.Count; k += 2)
            {
                // Separate features to 4 quarters
                // and take 3 quarters as KNNModel features
                // and the fourth quarter as validation data
                int parts = 1;
                int partFeaturesNmbr = 1;
                for (int i = 4; i > 0; i--)
                    if (features.Count / i > 0)
                    {
                        parts = i;
                        break;
                    }
                partFeaturesNmbr = features.Count / parts;
                double modelError = 0;
                double predError;
                for (int i = 0; i < parts; i++)
                {
                    // Create KNNModel and validation list
                    KNNModel kNNModel = new KNNModel { k = k, featuresList = new List<object[]>() };
                    List<object[]> validationList = new List<object[]>();
                    if (parts == 1)
                        foreach (object[] feature in features)
                        {
                            kNNModel.featuresList.Add(feature);
                            validationList.Add(feature);
                        }
                    else
                        for (int j = 0; j < features.Count; j++)
                        {
                            // Check if this feature is for validation
                            if ((i * partFeaturesNmbr <= j && j < (i + 1) * partFeaturesNmbr) || (i == parts - 1 && i * partFeaturesNmbr <= j))
                                // This is for validation
                                validationList.Add(features[j]);
                            else
                                // This is for KNNModel
                                kNNModel.featuresList.Add(features[j]);
                        }
                    // Calculate error for this parition
                    double[] predictedOutput;
                    foreach (object[] feature in validationList)
                    {
                        // Calculate prediction error
                        predError = 0;
                        predictedOutput = predict((double[])feature[0], kNNModel, pcLoadingScores);
                        for (int j = 0; j < predictedOutput.Length; j++)
                            predError += Math.Pow(predictedOutput[j] - ((double[])feature[1])[j], 2);
                        predError = Math.Sqrt(predError);
                        // Add this error in modelError
                        modelError += predError / features.Count;
                    }
                }

                // Add current k error in kErrors
                kErrors.Add(new kError { error = modelError, k = k });
            }
            // Sort errors in kErrors
            kErrors.Sort((e1, e2) => { return e1.error.CompareTo(e2.error); });
            // Return the least error k
            return kErrors[0].k;
        }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //:::::::::::::::::::::::::::CROSS PROCESS FORM FUNCTIONS (INTERFACES)::::::::::::::::::::::://
        public void holdRecordReport(List<object[]> records, string callingClassName)
        {
            if (!callingClassName.Contains("KNNBackThread"))
                return;

            // Iterate through each signal features and sort them in featuresLists
            OrderedDictionary signalFeatures = null;
            foreach (object[] record in records)
            {
                signalFeatures = (OrderedDictionary)Garage.ByteArrayToObject((byte[])record[0]);
                // The first item is just beats states
                for (int i = 1; i < signalFeatures.Count; i++)
                {
                    if (i == 1)
                        ((KNNModel)((List<object[]>)_targetsModelsHashtable[_selectedModel])[i - 1][0]).featuresList.Add((object[])signalFeatures[i]);
                    else if (i == 4)
                        foreach (List<object[]> beat in (object[])signalFeatures[i])
                            foreach (object[] feature in beat)
                                ((KNNModel)((List<object[]>)_targetsModelsHashtable[_selectedModel])[i - 1][0]).featuresList.Add(feature);
                    else
                        foreach (object[] feature in (object[])signalFeatures[i])
                            if (feature[0] != null)
                                ((KNNModel)((List<object[]>)_targetsModelsHashtable[_selectedModel])[i - 1][0]).featuresList.Add(feature);
                }
            }
        }
    }
}

using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.AITools.Details;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using BSP_Using_AI.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools
{
    class KNNBackThread : DbStimulatorReportHolder
    {
        private AIBackThreadReportHolder _aiBackThreadReportHolderForAIToolsForm;

        private Dictionary<string, ARTHTModels> _arthtModelsDic = null;
        ARTHTModels _aRTHTModels = null;

        public KNNBackThread(Dictionary<string, ARTHTModels> arthtModelsDic, AIBackThreadReportHolder aiBackThreadReportHolderForAIToolsForm)
        {
            _arthtModelsDic = arthtModelsDic;
            _aiBackThreadReportHolderForAIToolsForm = aiBackThreadReportHolderForAIToolsForm;
        }

        public void fit(string modelsName, Dictionary<string, List<Sample>> dataLists, long datasetSize, long modelId, string stepName)
        {
            int fitProgress = 0;
            int tolatFitProgress = dataLists.Count;

            // Iterate through models from the selected ones in _arthtModelsDic
            ARTHTModels arthtModels = _arthtModelsDic[modelsName];

            // Fit features
            if (!stepName.Equals(""))
            {
                _arthtModelsDic[modelsName].ARTHTModelsDic[stepName] = createKNNModel(stepName, dataLists[stepName], _arthtModelsDic[modelsName].ARTHTModelsDic[stepName]._pcaActive);
                // Fit features in model
                fit((KNNModel)arthtModels.ARTHTModelsDic[stepName],
                                                      dataLists[stepName]);
            }
            else
                foreach (string stepNa in arthtModels.ARTHTModelsDic.Keys)
                {
                    fit((KNNModel)arthtModels.ARTHTModelsDic[stepNa],
                                                   dataLists[stepNa]);

                    // Update fitProgressBar
                    fitProgress++;
                    if (_aiBackThreadReportHolderForAIToolsForm != null)
                        _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "progress", modelsName, fitProgress, tolatFitProgress }, "AIToolsForm");
                }

            // Update model in models table
            DbStimulator dbStimulator = new DbStimulator();
            if (arthtModels.DataIdsIntervalsList.Count > 0)
                dbStimulator.Update("models", new string[] { "the_model", "dataset_size" },
                    new Object[] { GeneralTools.ObjectToByteArray(arthtModels.Clone()), datasetSize }, modelId, "TFBackThread");
            else
                dbStimulator.Update("models", new string[] { "the_model" },
                    new Object[] { GeneralTools.ObjectToByteArray(arthtModels.Clone()) }, modelId, "TFBackThread");

            // Send report about fitting is finished and models table should be updated
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "fitting_complete", modelsName, datasetSize }, "AIToolsForm");
        }

        public static KNNModel fit(KNNModel model, List<Sample> dataList)
        {
            if (model._pcaActive)
                dataList = GeneralTools.rearrangeFeaturesInput(dataList, model.PCA);
            // Set the new optimal k for this model
            if (dataList.Count > 0)
                model.k = getOptimalK(dataList.Select((x, y) => new { Value = x, Index = y })
                                                        .GroupBy(x => x.Index / 300)
                                                        .Select(x => x.Select(v => v.Value).ToList())
                                                        .ToList()[0], model);
            else
                model.k = 3;
            // Set the new features in the model
            foreach (Sample feature in dataList)
                model.DataList.Add(feature);

            return model;
        }

        public static double[] predict(double[] features, KNNModel kNNModel)
        {
            // Initialize input
            if (kNNModel._pcaActive)
                features = GeneralTools.rearrangeInput(features, kNNModel.PCA);
            // Create list for calculating distances between input and saved dataset
            List<distanteOutput> distances = new List<distanteOutput>();
            // Iterate through all saved features and calucalte distance between the input and the saved feature
            double[] savedFeatures;
            double distance;
            foreach (Sample samp in kNNModel.DataList)
            {
                distance = 0;
                savedFeatures = samp.getFeatures();
                for (int i = 0; i < features.Length; i++)
                    distance += Math.Pow(features[i] - savedFeatures[i], 2);
                distance = Math.Sqrt(distance);
                // Insert distance and its output in distances
                distances.Add(new distanteOutput { distance = distance, output = samp.getOutputs() });
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

        public void createKNNkModelForWPW()
        {
            // Create neural network models for WPW syndrome detection
            // Create 7 models for { 2 for QRS detection (Threshold_ratio & Hor_threshold, remove_miss_selected_R),
            // 2 for P_T detection (Threshold_ratio & Hor_threshold, P & T states),
            // 1 for short PR detection,
            // 2 for delta deteciton (Acceleration threshold, delta existence) }
            ARTHTModels arthtModels = new ARTHTModels();
            // Create a KNN models structure with the initial optimum K, which is "3"
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step1RPeaksScanData] = createKNNModel(ARTHTNamings.Step1RPeaksScanData, 3, 2); // (15, 2) For R peaks detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step2RPeaksSelectionData] = createKNNModel(ARTHTNamings.Step2RPeaksSelectionData, 3, 1); // (2, 1) For R selection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step3BeatPeaksScanData] = createKNNModel(ARTHTNamings.Step3BeatPeaksScanData, 3, 2); // (5, 2) For beat peaks detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step4PTSelectionData] = createKNNModel(ARTHTNamings.Step4PTSelectionData, 3, 2); // (3, 2) For P and T detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step5ShortPRScanData] = createKNNModel(ARTHTNamings.Step5ShortPRScanData, 3, 1); // (1, 1) For short PR detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step6UpstrokesScanData] = createKNNModel(ARTHTNamings.Step6UpstrokesScanData, 3, 1); // (6, 1) For delta detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step7DeltaExaminationData] = createKNNModel(ARTHTNamings.Step7DeltaExaminationData, 3, 1); // (6, 1) For WPW syndrome declaration

            // Insert models in _arthtModelsDic
            int modelIndx = 0;
            arthtModels.ModelName = KNNModel.ModelName;
            arthtModels.ProblemName = " for WPW syndrome detection";
            while (_arthtModelsDic.ContainsKey(arthtModels.ModelName + arthtModels.ProblemName + modelIndx))
                modelIndx++;
            arthtModels.ProblemName = " for WPW syndrome detection" + modelIndx;
            _arthtModelsDic.Add(arthtModels.ModelName + arthtModels.ProblemName, arthtModels);

            // Save models in models table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.Insert("models", new string[] { "type_name", "model_target", "the_model", "dataset_size" },
                new Object[] { "K-Nearest neighbor", "WPW syndrome detection", GeneralTools.ObjectToByteArray(arthtModels.Clone()), 0 }, "KNNBackThread");

            // Refresh modelsFlowLayoutPanel
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "createModel" }, "AIToolsForm");
        }

        public static KNNModel createKNNModel(string stepName, List<Sample> dataList, bool pcaActive)
        {
            // Compute PCA loading scores if PCA is active
            List<PCAitem> pca = new List<PCAitem>();
            if (pcaActive)
                // If yes then compute PCA loading scores
                pca = DataVisualisationForm.getPCA(dataList);
            int output;

            if (stepName.Equals(ARTHTNamings.Step1RPeaksScanData) || stepName.Equals(ARTHTNamings.Step3BeatPeaksScanData) || stepName.Equals(ARTHTNamings.Step4PTSelectionData))
                output = 2;
            else
                output = 1;

            KNNModel tempModel = createKNNModel(stepName, 3, output);
            tempModel._pcaActive = pcaActive;
            tempModel.PCA = pca;

            return tempModel;
        }

        public static KNNModel createKNNModel(string name, int k, int outputNumber)
        {
            // Create a KNN model structure with the initial optimum K
            KNNModel model = new KNNModel { Name = name, k = k };
            // Set initial thresholds for output decisions
            model.OutputsThresholds = new float[outputNumber];
            for (int i = 0; i < outputNumber; i++)
                model.OutputsThresholds[i] = 0.5f;

            return model;
        }

        public void initializeNeuralNetworkModelsForWPW(ARTHTModels arthtModels)
        {
            // Insert models in _arthtModelsDic
            _arthtModelsDic.Add(arthtModels.ModelName + arthtModels.ProblemName, arthtModels);
            _aRTHTModels = arthtModels;

            // Query for the selected dataset
            ValidationFlowLayoutPanelUserControl.queryForSelectedDataset(arthtModels.DataIdsIntervalsList, this);
        }

        private static int getOptimalK(List<Sample> data, KNNModel model)
        {
            // Iterate through all possible k values
            List<kError> kErrors = new List<kError>();
            for (int k = 1; k <= 41 && k <= data.Count; k += 2)
            {
                // Separate features to 4 quarters
                // and take 3 quarters as KNNModel features
                // and the fourth quarter as validation data
                int parts = 1;
                int partFeaturesNmbr = 1;
                for (int i = 4; i > 0; i--)
                    if (data.Count / i > 0)
                    {
                        parts = i;
                        break;
                    }
                partFeaturesNmbr = data.Count / parts;
                double modelError = 0;
                double predError;
                for (int i = 0; i < parts; i++)
                {
                    // Create KNNModel and validation list
                    KNNModel kNNModel = model.Clone();
                    List<Sample> validationList = new List<Sample>();
                    if (parts == 1)
                        foreach (Sample sample in data)
                        {
                            kNNModel.DataList.Add(sample);
                            validationList.Add(sample);
                        }
                    else
                        for (int j = 0; j < data.Count; j++)
                        {
                            // Check if this feature is for validation
                            if ((i * partFeaturesNmbr <= j && j < (i + 1) * partFeaturesNmbr) || (i == parts - 1 && i * partFeaturesNmbr <= j))
                                // This is for validation
                                validationList.Add(data[j]);
                            else
                                // This is for KNNModel
                                kNNModel.DataList.Add(data[j]);
                        }
                    // Calculate error for this parition
                    double[] predictedOutput;
                    double[] actualOutput;
                    foreach (Sample sample in validationList)
                    {
                        // Calculate prediction error
                        predError = 0;
                        predictedOutput = predict(sample.getFeatures(), kNNModel);
                        actualOutput = sample.getOutputs();
                        for (int j = 0; j < predictedOutput.Length; j++)
                            predError += Math.Pow(predictedOutput[j] - actualOutput[j], 2);
                        predError = Math.Sqrt(predError);
                        // Add this error in modelError
                        modelError += predError / data.Count;
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
        public void holdRecordReport(DataTable dataTable, string callingClassName)
        {
            // Iterate through each signal samples and sort them in dataLists
            ARTHTFeatures aRTHTFeatures;
            foreach (DataRow row in dataTable.AsEnumerable())
            {
                aRTHTFeatures = GeneralTools.ByteArrayToObject<ARTHTFeatures>(row.Field<byte[]>("features"));
                foreach (string stepName in aRTHTFeatures.StepsDataDic.Keys)
                {
                    foreach (Sample sample in aRTHTFeatures.StepsDataDic[stepName].Samples)
                        ((KNNModel)_aRTHTModels.ARTHTModelsDic[stepName]).DataList.Add(sample);
                }
            }
        }
    }
}

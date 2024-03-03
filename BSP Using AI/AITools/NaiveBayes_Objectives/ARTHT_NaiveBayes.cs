using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI;
using BSP_Using_AI.AITools;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using System;
using System.Collections.Generic;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace Biological_Signal_Processing_Using_AI.AITools.NaiveBayes_Objectives
{
    class ARTHT_NaiveBayes
    {
        private AIBackThreadReportHolder _aiBackThreadReportHolderForAIToolsForm;

        private Dictionary<string, ARTHTModels> _arthtModelsDic = null;

        public ARTHT_NaiveBayes(Dictionary<string, ARTHTModels> arthtModelsDic, AIBackThreadReportHolder aiBackThreadReportHolderForAIToolsForm)
        {
            _arthtModelsDic = arthtModelsDic;
            _aiBackThreadReportHolderForAIToolsForm = aiBackThreadReportHolderForAIToolsForm;
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public void fit(string modelsName, Dictionary<string, List<Sample>> dataLists, long datasetSize, long modelId, string stepName)
        {
            // Iterate through models from the selected ones in _targetsModelsHashtable
            int fitProgress = 0;
            int tolatFitProgress = dataLists.Count;

            // Iterate through models from the selected ones in _arthtModelsDic
            ARTHTModels arthtModels = _arthtModelsDic[modelsName];

            if (!stepName.Equals(""))
            {
                _arthtModelsDic[modelsName].ARTHTModelsDic[stepName] = createNBModel(stepName, dataLists[stepName], _arthtModelsDic[modelsName].ARTHTModelsDic[stepName]._pcaActive);
                // Fit features in model
                NaiveBayes.fit((NaiveBayesModel)arthtModels.ARTHTModelsDic[stepName],
                                                      dataLists[stepName]);
            }
            else
                foreach (string stepNa in arthtModels.ARTHTModelsDic.Keys)
                {
                    // Set the new features probabilities in the model
                    NaiveBayes.fit((NaiveBayesModel)arthtModels.ARTHTModelsDic[stepNa],
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
                    new object[] { GeneralTools.ObjectToByteArray(arthtModels.Clone()), datasetSize }, modelId, "TFBackThread");
            else
                dbStimulator.Update("models", new string[] { "the_model" },
                    new object[] { GeneralTools.ObjectToByteArray(arthtModels.Clone()) }, modelId, "TFBackThread");

            // Send report about fitting is finished and models table should be updated
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "fitting_complete", modelsName, datasetSize }, "AIToolsForm");
        }

        public void createNBModelForWPW()
        {
            // Create neural network models for WPW syndrome detection
            // Create 7 models for { 2 for QRS detection (Threshold_ratio & Hor_threshold, remove_miss_selected_R),
            // 2 for P_T detection (Threshold_ratio & Hor_threshold, P & T states),
            // 1 for short PR detection,
            // 2 for delta deteciton (Acceleration threshold, delta existence) }
            ARTHTModels arthtModels = new ARTHTModels();
            // Create a KNN models structure with the initial optimum K, which is "3"
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step1RPeaksScanData] = createNBModel(ARTHTNamings.Step1RPeaksScanData, true, 2); // (15, 2) For R peaks detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step2RPeaksSelectionData] = createNBModel(ARTHTNamings.Step2RPeaksSelectionData, false, 1); // (2, 1) For R selection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step3BeatPeaksScanData] = createNBModel(ARTHTNamings.Step3BeatPeaksScanData, true, 2); // (5, 2) For beat peaks detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step4PTSelectionData] = createNBModel(ARTHTNamings.Step4PTSelectionData, false, 2); // (3, 2) For P and T detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step5ShortPRScanData] = createNBModel(ARTHTNamings.Step5ShortPRScanData, false, 1); // (1, 1) For short PR detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step6UpstrokesScanData] = createNBModel(ARTHTNamings.Step6UpstrokesScanData, true, 1); // (6, 1) For delta detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step7DeltaExaminationData] = createNBModel(ARTHTNamings.Step7DeltaExaminationData, false, 1); // (6, 1) For WPW syndrome declaration

            // Insert models in _targetsModelsHashtable
            int modelIndx = 0;
            arthtModels.ModelName = NaiveBayesModel.ModelName;
            arthtModels.ProblemName = " for WPW syndrome detection";
            while (_arthtModelsDic.ContainsKey(arthtModels.ModelName + arthtModels.ProblemName + modelIndx))
                modelIndx++;
            arthtModels.ProblemName = " for WPW syndrome detection" + modelIndx;
            _arthtModelsDic.Add(arthtModels.ModelName + arthtModels.ProblemName, arthtModels);

            // Save models in models table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.Insert("models", new string[] { "type_name", "model_target", "the_model", "dataset_size" },
                new object[] { "Naive bayes", "WPW syndrome detection", GeneralTools.ObjectToByteArray(arthtModels.Clone()), 0 }, "KNNBackThread");

            // Refresh modelsFlowLayoutPanel
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new object[] { "createModel" }, "AIToolsForm");
        }

        public static NaiveBayesModel createNBModel(string stepName, List<Sample> dataList, bool pcaActive)
        {
            // Compute PCA loading scores if PCA is active
            List<PCAitem> pca = new List<PCAitem>();
            if (pcaActive)
                // If yes then compute PCA loading scores
                pca = DataVisualisationForm.getPCA(dataList);
            bool regression = false;
            int output;

            if (stepName.Equals(ARTHTNamings.Step1RPeaksScanData) || stepName.Equals(ARTHTNamings.Step3BeatPeaksScanData) || stepName.Equals(ARTHTNamings.Step6UpstrokesScanData))
                regression = true;

            if (stepName.Equals(ARTHTNamings.Step1RPeaksScanData) || stepName.Equals(ARTHTNamings.Step3BeatPeaksScanData) || stepName.Equals(ARTHTNamings.Step4PTSelectionData))
                output = 2;
            else
                output = 1;

            NaiveBayesModel tempModel = createNBModel(stepName, regression, output);
            tempModel._pcaActive = pcaActive;
            tempModel.PCA = pca;

            return tempModel;
        }

        public static NaiveBayesModel createNBModel(string name, bool regression, int outputNumber)
        {
            // Create a KNN model structure with the initial optimum K
            NaiveBayesModel model = new NaiveBayesModel { Name = name, _regression = regression };
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
        }
    }
}

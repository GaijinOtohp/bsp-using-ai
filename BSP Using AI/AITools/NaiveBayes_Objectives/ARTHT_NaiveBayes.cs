using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI;
using BSP_Using_AI.AITools;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using System;
using System.Collections.Generic;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.AITools.AIBackThreadReportHolder;

namespace Biological_Signal_Processing_Using_AI.AITools.NaiveBayes_Objectives
{
    class ARTHT_NaiveBayes
    {
        private AIBackThreadReportHolder _aiBackThreadReportHolderForAIToolsForm;

        private Dictionary<string, ObjectiveBaseModel> _objectivesModelsDic = null;

        public ARTHT_NaiveBayes(Dictionary<string, ObjectiveBaseModel> objectivesModelsDic, AIBackThreadReportHolder aiBackThreadReportHolderForAIToolsForm)
        {
            _objectivesModelsDic = objectivesModelsDic;
            _aiBackThreadReportHolderForAIToolsForm = aiBackThreadReportHolderForAIToolsForm;
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public void fit(string modelName, Dictionary<string, List<Sample>> dataLists, long datasetSize, long modelId, string stepName)
        {
            // Iterate through models from the selected ones in _targetsModelsHashtable
            int fitProgress = 0;
            int tolatFitProgress = dataLists.Count;

            // Iterate through models from the selected ones in _arthtModelsDic
            ARTHTModels arthtModels = (ARTHTModels)_objectivesModelsDic[modelName];

            if (!stepName.Equals(""))
            {
                arthtModels.ARTHTModelsDic[stepName] = createNBModel(stepName, dataLists[stepName], arthtModels.ARTHTModelsDic[stepName]._pcaActive);
                // Fit features in model
                NaiveBayes.fit((NaiveBayesModel)arthtModels.ARTHTModelsDic[stepName],
                                                      dataLists[stepName]);
            }
            else
                foreach (string stepNa in dataLists.Keys)
                {
                    // Set the new features probabilities in the model
                    NaiveBayes.fit((NaiveBayesModel)arthtModels.ARTHTModelsDic[stepNa],
                                                          dataLists[stepNa]);

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
                dbStimulator.Update("models", new string[] { "the_model", "dataset_size" },
                    new object[] { GeneralTools.ObjectToByteArray(arthtModels.Clone()), datasetSize }, modelId, "TFBackThread");
            else
                dbStimulator.Update("models", new string[] { "the_model" },
                    new object[] { GeneralTools.ObjectToByteArray(arthtModels.Clone()) }, modelId, "TFBackThread");

            // Send report about fitting is finished and models table should be updated
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new FittingCompAIReport()
                                                                    {
                                                                        ReportType = AIReportType.FittingComplete,
                                                                        ModelName = modelName,
                                                                        datasetSize = datasetSize,
                                                                    }, "AIToolsForm");
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
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step1RPeaksScanData] = createNBModel(ARTHTNamings.Step1RPeaksScanData, 15, 2, ARTHTNamings.PeaksScannerOutputs.GetNames()); // (15, 2) For R peaks detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step2RPeaksSelectionData] = createNBModel(ARTHTNamings.Step2RPeaksSelectionData, 2, 1, ARTHTNamings.RSelectionOutputs.GetNames()); // (2, 1) For R selection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step3BeatPeaksScanData] = createNBModel(ARTHTNamings.Step3BeatPeaksScanData, 5, 2, ARTHTNamings.PeaksScannerOutputs.GetNames()); // (5, 2) For beat peaks detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step4PTSelectionData] = createNBModel(ARTHTNamings.Step4PTSelectionData, 3, 2, ARTHTNamings.PTSelectionOutputs.GetNames()); // (3, 2) For P and T detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step5ShortPRScanData] = createNBModel(ARTHTNamings.Step5ShortPRScanData, 1, 1, ARTHTNamings.ShortPROutputs.GetNames()); // (1, 1) For short PR detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step6UpstrokesScanData] = createNBModel(ARTHTNamings.Step6UpstrokesScanData, 6, 1, ARTHTNamings.UpStrokeOutputs.GetNames()); // (6, 1) For delta detection
            arthtModels.ARTHTModelsDic[ARTHTNamings.Step7DeltaExaminationData] = createNBModel(ARTHTNamings.Step7DeltaExaminationData, 6, 1, ARTHTNamings.DeltaExamOutputs.GetNames()); // (6, 1) For WPW syndrome declaration

            // Insert models in _targetsModelsHashtable
            int modelIndx = 0;
            arthtModels.ModelName = NaiveBayesModel.ModelName;
            arthtModels.ObjectiveName = " for WPW syndrome detection";
            while (_objectivesModelsDic.ContainsKey(arthtModels.ModelName + arthtModels.ObjectiveName + modelIndx))
                modelIndx++;
            arthtModels.ObjectiveName = " for WPW syndrome detection" + modelIndx;
            _objectivesModelsDic.Add(arthtModels.ModelName + arthtModels.ObjectiveName, arthtModels);

            // Save models in models table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.Insert("models", new string[] { "type_name", "model_target", "the_model", "dataset_size" },
                new object[] { NaiveBayesModel.ModelName, WPWSyndromeDetection.ObjectiveName, GeneralTools.ObjectToByteArray(arthtModels.Clone()), 0 }, "KNNBackThread");

            // Refresh modelsFlowLayoutPanel
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new AIReport() { ReportType = AIReportType.CreateModel }, "AIToolsForm");
        }

        public static NaiveBayesModel createNBModel(string stepName, List<Sample> dataList, bool pcaActive)
        {
            // Compute PCA loading scores if PCA is active
            List<PCAitem> pca = new List<PCAitem>();
            if (pcaActive)
                // If yes then compute PCA loading scores
                pca = DataVisualisationForm.getPCA(dataList);
            int input = pca.Count > 0 ? pca.Count : 0;
            int output;
            string[] outputNames;

            if (stepName.Equals(ARTHTNamings.Step1RPeaksScanData))
            {
                if (input == 0) input = 15;
                outputNames = ARTHTNamings.PeaksScannerOutputs.GetNames();
            }
            else if (stepName.Equals(ARTHTNamings.Step2RPeaksSelectionData))
            {
                if (input == 0) input = 2;
                outputNames = ARTHTNamings.RSelectionOutputs.GetNames();
            }
            else if (stepName.Equals(ARTHTNamings.Step3BeatPeaksScanData))
            {
                if (input == 0) input = 5;
                outputNames = ARTHTNamings.PeaksScannerOutputs.GetNames();
            }
            else if (stepName.Equals(ARTHTNamings.Step4PTSelectionData))
            {
                if (input == 0) input = 3;
                outputNames = ARTHTNamings.PTSelectionOutputs.GetNames();
            }
            else if (stepName.Equals(ARTHTNamings.Step5ShortPRScanData))
            {
                if (input == 0) input = 1;
                outputNames = ARTHTNamings.ShortPROutputs.GetNames();
            }
            else if (stepName.Equals(ARTHTNamings.Step6UpstrokesScanData))
            {
                if (input == 0) input = 6;
                outputNames = ARTHTNamings.UpStrokeOutputs.GetNames();
            }
            else
            {
                if (input == 0) input = 6;
                outputNames = ARTHTNamings.DeltaExamOutputs.GetNames();
            }

            if (stepName.Equals(ARTHTNamings.Step1RPeaksScanData) || stepName.Equals(ARTHTNamings.Step3BeatPeaksScanData) || stepName.Equals(ARTHTNamings.Step4PTSelectionData))
                output = 2;
            else
                output = 1;

            NaiveBayesModel tempModel = createNBModel(stepName, input, output, outputNames);
            tempModel._pcaActive = pcaActive;
            tempModel.PCA = pca;

            return tempModel;
        }

        public static NaiveBayesModel createNBModel(string name, int inputDim, int outputDim, string[] outputNames)
        {
            // Create a KNN model structure with the initial optimum K
            NaiveBayesModel model = new NaiveBayesModel(inputDim, outputDim, outputNames) { Name = name};
            if (name.Equals(ARTHTNamings.Step1RPeaksScanData) || name.Equals(ARTHTNamings.Step3BeatPeaksScanData) || name.Equals(ARTHTNamings.Step6UpstrokesScanData))
                model.Type = ObjectiveType.Regression;
            else
                model.Type = ObjectiveType.Classification;
            model._regression = (model.Type == ObjectiveType.Regression);
            // Set initial thresholds for output decisions
            model.OutputsThresholds = new OutputThresholdItem[outputDim];
            for (int i = 0; i < outputDim; i++)
                model.OutputsThresholds[i] = new OutputThresholdItem();

            return model;
        }

        public void initializeNeuralNetworkModelsForWPW(ARTHTModels arthtModels)
        {
            // Insert models in _arthtModelsDic
            _objectivesModelsDic.Add(arthtModels.ModelName + arthtModels.ObjectiveName, arthtModels);
        }
    }
}

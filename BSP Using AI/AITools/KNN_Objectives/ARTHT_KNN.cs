using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI;
using BSP_Using_AI.AITools;
using BSP_Using_AI.AITools.Details;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using BSP_Using_AI.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.AITools.AIBackThreadReportHolder;

namespace Biological_Signal_Processing_Using_AI.AITools.KNN_Objectives
{
    class ARTHT_KNN : DbStimulatorReportHolder
    {
        private AIBackThreadReportHolder _aiBackThreadReportHolderForAIToolsForm;

        private Dictionary<string, ObjectiveBaseModel> _objectivesModelsDic = null;
        ARTHTModels _aRTHTModels = null;

        string _SelectedModelName;
        int _currentFitProgress;
        int _maxFitProgress = 0;

        public ARTHT_KNN(Dictionary<string, ObjectiveBaseModel> objectivesModelsDic, AIBackThreadReportHolder aiBackThreadReportHolderForAIToolsForm)
        {
            _objectivesModelsDic = objectivesModelsDic;
            _aiBackThreadReportHolderForAIToolsForm = aiBackThreadReportHolderForAIToolsForm;
        }

        private void UpdateFittingProgress(int currentProgress, int maxProgress)
        {
            _currentFitProgress++;
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new FittingProgAIReport()
                {
                    ReportType = AIReportType.FittingProgress,
                    ModelName = _SelectedModelName,
                    fitProgress = _currentFitProgress,
                    fitMaxProgress = _maxFitProgress
                }, "AIToolsForm");
        }

        public void fit(string modelName, Dictionary<string, List<Sample>> dataLists, long datasetSize, long modelId, string stepName)
        {
            _SelectedModelName = modelName;
            _currentFitProgress = 0;
            foreach (List<Sample> dataList in dataLists.Values)
                _maxFitProgress += dataList.Count;

            // Iterate through models from the selected ones in _arthtModelsDic
            ARTHTModels arthtModels = (ARTHTModels)_objectivesModelsDic[modelName];

            // Fit features
            if (!stepName.Equals(""))
            {
                arthtModels.ARTHTModelsDic[stepName] = createKNNModel(stepName, dataLists[stepName], arthtModels.ARTHTModelsDic[stepName]._pcaActive);
                // Fit features in model
                KNN.fit((KNNModel)arthtModels.ARTHTModelsDic[stepName],
                                                      dataLists[stepName], null);
            }
            else
                foreach (string stepNa in arthtModels.ARTHTModelsDic.Keys)
                    KNN.fit((KNNModel)arthtModels.ARTHTModelsDic[stepNa], dataLists[stepNa], UpdateFittingProgress);

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
            arthtModels.ObjectiveName = " for WPW syndrome detection";
            while (_objectivesModelsDic.ContainsKey(arthtModels.ModelName + arthtModels.ObjectiveName + modelIndx))
                modelIndx++;
            arthtModels.ObjectiveName = " for WPW syndrome detection" + modelIndx;
            _objectivesModelsDic.Add(arthtModels.ModelName + arthtModels.ObjectiveName, arthtModels);

            // Save models in models table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.Insert("models", new string[] { "type_name", "model_target", "the_model", "dataset_size" },
                new object[] { KNNModel.ModelName, WPWSyndromeDetection.ObjectiveName, GeneralTools.ObjectToByteArray(arthtModels.Clone()), 0 }, "KNNBackThread");

            // Refresh modelsFlowLayoutPanel
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new AIReport() { ReportType = AIReportType.CreateModel }, "AIToolsForm");
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
            _objectivesModelsDic.Add(arthtModels.ModelName + arthtModels.ObjectiveName, arthtModels);
            _aRTHTModels = arthtModels;

            // Query for the selected dataset
            ValidationFlowLayoutPanelUserControl.queryForSelectedDataset(arthtModels.DataIdsIntervalsList, this);
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

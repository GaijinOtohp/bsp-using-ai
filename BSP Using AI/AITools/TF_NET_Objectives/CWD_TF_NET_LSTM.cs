using Biological_Signal_Processing_Using_AI.AITools.RL_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI;
using BSP_Using_AI.AITools;
using BSP_Using_AI.AITools.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tensorflow;
using Tensorflow.NumPy;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.AITools.AIBackThreadReportHolder;

namespace Biological_Signal_Processing_Using_AI.AITools.TF_NET_Objectives
{
    public class CWD_TF_NET_LSTM
    {
        private AIBackThreadReportHolder _aiBackThreadReportHolderForAIToolsForm;

        private Dictionary<string, ObjectiveBaseModel> _objectivesModelsDic = null;

        string _SelectedModelName;
        int _currentFitStep;
        int _maxFitSteps;

        public CWD_TF_NET_LSTM(Dictionary<string, ObjectiveBaseModel> objectivesModelsDic, AIBackThreadReportHolder aiBackThreadReportHolderForAIToolsForm)
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

        public void FitOnRLModel(string modelName, List<Sample> rlTrainingSamplesList)
        {
            _SelectedModelName = modelName;
            _currentFitStep = 0;
            _maxFitSteps = 1;

            CWDLSTM cwdLSTM = (CWDLSTM)_objectivesModelsDic[modelName];

            // Fit features in model
            if (rlTrainingSamplesList.Count > 0)
                TF_NET_NN.fit(cwdLSTM.CWDReinforcementLModel, cwdLSTM.CWDReinforcementLModel.BaseModel, rlTrainingSamplesList, UpdateFittingProgress, saveModel: true);
        }

        public void FitOnLSTMModel(string modelName, List<List<Sample>> dataListSequences, long datasetSize, long modelId)
        {
            _SelectedModelName = modelName;
            _currentFitStep = 0;
            _maxFitSteps = 1;

            CWDLSTM cwdLSTM = (CWDLSTM)_objectivesModelsDic[modelName];

            // Fit features in model
            TF_NET_LSTM_Recur.Fit(cwdLSTM.CWDLSTMModel, dataListSequences, UpdateFittingProgress, true);
            UpdateThresholds(cwdLSTM.CWDLSTMModel, dataListSequences, UpdateFittingProgress);

            // Update model in models table
            DbStimulator dbStimulator = new DbStimulator();
            if (cwdLSTM.DataIdsIntervalsList.Count > 0)
            {
                Thread dbStimulatorThread = new Thread(() => dbStimulator.Update("models", new string[] { "the_model", "dataset_size" },
                    new object[] { GeneralTools.ObjectToByteArray(cwdLSTM.Clone()), datasetSize }, modelId, "CWD_TF_NET_LSTM"));
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

        public void createTFNETLSTMModelForCWD()
        {
            // Set models in name and path
            int rlModelIndx = 0;
            int lstmModelIndx = 0;
            while (_objectivesModelsDic.ContainsKey(TFNETReinforcementL.ModelName + " for " + CharacteristicWavesDelineation.ObjectiveName + rlModelIndx))
                rlModelIndx++;
            while (_objectivesModelsDic.ContainsKey(TFNETLSTMModel.ModelName + " for " + CharacteristicWavesDelineation.ObjectiveName + lstmModelIndx))
                lstmModelIndx++;
            /*string rlModelPath = System.IO.Directory.GetCurrentDirectory() + @"/AIModels/CWD/TFNETModels/RL" + rlModelIndx;
            string crazyRLModelPath = System.IO.Directory.GetCurrentDirectory() + @"/AIModels/CWD/TFNETModels/CrazyRL" + rlModelIndx;
            string lstmModelPath = System.IO.Directory.GetCurrentDirectory() + @"/AIModels/CWD/TFNETModels/LSTM" + lstmModelIndx;*/
            string rlModelPath = @"./AIModels/CWD/TFNETModels/RL" + rlModelIndx;
            string crazyRLModelPath = @"./AIModels/CWD/TFNETModels/CrazyRL" + rlModelIndx;
            string lstmModelPath = @"./AIModels/CWD/TFNETModels/LSTM" + lstmModelIndx;

            // Create the model object
            CWDLSTM cwdLSTM = new CWDLSTM();
            // One for tuning the peaks analyzer using deep Q learning
            cwdLSTM.CWDReinforcementLModel = CWD_RL_TFNET.createTFNETRLModel(CWDNamigs.RLCornersScanData, rlModelPath, inputDim: 10, outputDim: 2, CWDNamigs.CornersScanOutputs.GetNames());
            cwdLSTM.CWDCrazyReinforcementLModel = CWD_RL_TFNET.createTFNETRLModel(CWDNamigs.RLCornersScanData, crazyRLModelPath, inputDim: 10, outputDim: 2, CWDNamigs.CornersScanOutputs.GetNames());
            // One for classifying the peaks
            cwdLSTM.CWDLSTMModel = createTFNETLSTMModel(CWDNamigs.LSTMPeaksClassificationData, lstmModelPath, inputDim: 106, outputDim: 10, CWDNamigs.PeaksLabelsOutputs.GetNames());

            cwdLSTM.ModelName = TFNETLSTMModel.ModelName;
            cwdLSTM.ObjectiveName = " for " + CharacteristicWavesDelineation.ObjectiveName + lstmModelIndx;

            _objectivesModelsDic.Add(cwdLSTM.ModelName + cwdLSTM.ObjectiveName, cwdLSTM);

            // Save models in models table
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.Insert("models", new string[] { "type_name", "model_target", "the_model", "dataset_size" },
                new object[] { TFNETLSTMModel.ModelName, CharacteristicWavesDelineation.ObjectiveName, GeneralTools.ObjectToByteArray(cwdLSTM.Clone()), 0 }, "CWD_TF_NET_LSTM");

            // Refresh modelsFlowLayoutPanel
            if (_aiBackThreadReportHolderForAIToolsForm != null)
                _aiBackThreadReportHolderForAIToolsForm.holdAIReport(new AIReport() { ReportType = AIReportType.CreateModel }, "AIToolsForm");
        }

        public static TFNETLSTMModel createTFNETLSTMModel(string name, string path, int inputDim, int outputDim, string[] outputNames)
        {
            int modelSequenceLength = 2;
            int layers = 1;
            TFNETLSTMModel model = new TFNETLSTMModel(path, inputDim, outputDim, outputNames, modelSequenceLength, bidirectional: true, layers: layers) { Name = name, Type = ObjectiveType.Classification };

            model.BaseModel.Session = TF_NET_LSTM_Recur.LSTMSession(model);

            // Save the model
            if (path.Length > 0)
                TF_NET_NN.SaveModelVariables(model.BaseModel.Session, path, TF_NET_LSTM_Recur.GetOutputCellsNames(model));

            // Set initial thresholds for output decisions
            model.OutputsThresholds = new OutputThresholdItem[outputDim];
            for (int i = 0; i < outputDim; i++)
                model.OutputsThresholds[i] = new OutputThresholdItem();

            // Set the prediction tolerance
            double[] peaksPredictionTolerance = PeaksPredictionTolerance.GetValues();
            for (int i = 0; i < peaksPredictionTolerance.Length; i++)
                model.ValidationData._ModelOutputsValidMetrics[i]._classDeviationTolerance = peaksPredictionTolerance[i];

            // Get the parameters
            return model;
        }

        public void initializeRLModelForCWD(CWDLSTM cwdLSTM)
        {
            TFNETReinforcementL rfModel = (TFNETReinforcementL)cwdLSTM.CWDReinforcementLModel.Clone();
            rfModel.BaseModel.Session = TF_NET_NN.LoadModelVariables(rfModel.BaseModel, CWD_RL_TFNET.createTFNETNeuralNetModelSession);
            cwdLSTM.CWDReinforcementLModel = rfModel;

            TFNETReinforcementL crazyRLModel = (TFNETReinforcementL)cwdLSTM.CWDCrazyReinforcementLModel.Clone();
            crazyRLModel.BaseModel.Session = TF_NET_NN.LoadModelVariables(crazyRLModel.BaseModel, CWD_RL_TFNET.createTFNETNeuralNetModelSession);
            cwdLSTM.CWDCrazyReinforcementLModel = crazyRLModel;

            TFNETLSTMModel lstmModel = (TFNETLSTMModel)cwdLSTM.CWDLSTMModel.Clone();
            lstmModel.BaseModel.Session = TF_NET_LSTM_Recur.LoadLSTMModelVariables(lstmModel);
            cwdLSTM.CWDLSTMModel = lstmModel;

            _objectivesModelsDic.Add(cwdLSTM.ModelName + cwdLSTM.ObjectiveName, cwdLSTM);
        }

        public static void UpdateThresholds(TFNETLSTMModel lstmModel, List<List<Sample>> dataListSequences, FittingProgAIReportDelegate fittingProgAIReportDelegate)
        {
            if (dataListSequences.Count == 0)
                return;

            // Set the high and low outputs averaged to zeros
            // and initialize the ROC
            for (int iOutput = 0; iOutput < lstmModel._outputDim; iOutput++)
            {
                lstmModel.OutputsThresholds[iOutput]._highOutputAv = 0;
                lstmModel.OutputsThresholds[iOutput]._lowOutputAv = 0;
                lstmModel.OutputsThresholds[iOutput]._ROC = new Dictionary<double, (int _truePositives, int _falsePositives)>(101);
            }

            // Iterate through 100 possibilities of thresholds
            int[] rocBestGap = new int[lstmModel._outputDim];
            double[] rocBestThreshold = new double[lstmModel._outputDim];
            // Create the general thresholds with additional last threshold "0.99"
            Queue<double> selectedThresholds = new Queue<double>(new double[] { 0, 0.1d, 0.2d, 0.3d, 0.4d, 0.5d, 0.6d, 0.7d, 0.8d, 0.9d, 0.99d });
            int totalThresholds = selectedThresholds.Count;
            int iThreshold = 0;
            while (selectedThresholds.Any())
            {
                iThreshold++;
                // Modify the model's outputs' thresholds
                double newThreshold = selectedThresholds.Dequeue();
                for (int iOutput = 0; iOutput < lstmModel._outputDim; iOutput++)
                    lstmModel.OutputsThresholds[iOutput]._threshold = newThreshold;

                // Get validation data using the selected threshod
                (ValidationData validationData, OutputThresholdItem[] outThresholds) = DetailsForm.LSTM_ValidateTheModel(lstmModel, dataListSequences, lstmModel, null);

                // Insert the new evaluation to the ROC
                for (int iOutput = 0; iOutput < lstmModel._outputDim; iOutput++)
                {
                    int truePositives = validationData._ModelOutputsValidMetrics[iOutput]._truePositive;
                    int falsePositives = validationData._ModelOutputsValidMetrics[iOutput]._falsePositive;

                    lstmModel.OutputsThresholds[iOutput]._ROC.Add(newThreshold, (truePositives, falsePositives));

                    // Check if the current threshold gave better results
                    if (truePositives - falsePositives > rocBestGap[iOutput])
                    {
                        rocBestGap[iOutput] = truePositives - falsePositives;
                        rocBestThreshold[iOutput] = newThreshold;

                        lstmModel.OutputsThresholds[iOutput]._highOutputAv = outThresholds[iOutput]._highOutputAv;
                        lstmModel.OutputsThresholds[iOutput]._lowOutputAv = outThresholds[iOutput]._lowOutputAv;
                    }
                    // Otherwise the previous threshold was better
                    else
                    {
                        // Check if the selected threshold is one of the general ones
                        if (newThreshold * 10 % 1 == 0 || newThreshold == 0.99d)
                        {
                            double bestThresh = rocBestThreshold[iOutput];
                            // If yes then add new details thresholds around the previous threshold
                            for (double addThresh = bestThresh - 0.09d; addThresh <= bestThresh + 0.09d; addThresh += 0.01d)
                            {
                                addThresh = Math.Round(addThresh, 2);
                                if (!selectedThresholds.Contains(addThresh) && addThresh > 0 && addThresh < 1 && addThresh != bestThresh)
                                {
                                    selectedThresholds.Enqueue(addThresh);
                                    totalThresholds++;
                                }
                            }
                        }
                    }
                }

                // Update fitProgressBar
                if (fittingProgAIReportDelegate != null)
                    fittingProgAIReportDelegate(iThreshold, totalThresholds);
            }

            // Set the best thresholds
            for (int iOutput = 0; iOutput < lstmModel._outputDim; iOutput++)
                lstmModel.OutputsThresholds[iOutput]._threshold = rocBestThreshold[iOutput];
        }
    }
}

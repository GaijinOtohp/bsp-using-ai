using Biological_Signal_Processing_Using_AI.AITools.RL_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI;
using BSP_Using_AI.AITools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
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
                TF_NET_NN.fit(cwdLSTM.CWDReinforcementLModel, cwdLSTM.CWDReinforcementLModel.BaseModel, rlTrainingSamplesList, UpdateFittingProgress, saveModel: true, earlyStopThreshold: 0.00001f);
        }

        public void FitOnLSTMModel(string modelName, List<List<Sample>> dataListSequences, long datasetSize, long modelId)
        {
            _SelectedModelName = modelName;
            _currentFitStep = 0;
            _maxFitSteps = 1;

            CWDLSTM cwdLSTM = (CWDLSTM)_objectivesModelsDic[modelName];

            // Fit features in model
            TF_NET_LSTM.Fit(cwdLSTM.CWDLSTMModel, dataListSequences, UpdateFittingProgress, true);
            UpdateThresholds(cwdLSTM.CWDLSTMModel, dataListSequences);

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
            string rlModelPath = System.IO.Directory.GetCurrentDirectory() + @"/AIModels/CWD/TFNETModels/RL" + rlModelIndx;
            string lstmModelPath = System.IO.Directory.GetCurrentDirectory() + @"/AIModels/CWD/TFNETModels/LSTM" + lstmModelIndx;

            // Create the model object
            CWDLSTM cwdLSTM = new CWDLSTM();
            // One for tuning the peaks analyzer using deep Q learning
            cwdLSTM.CWDReinforcementLModel = CWD_RL_TFNET.createTFNETRLModel(CWDNamigs.RLCornersScanData, rlModelPath, inputDim: 8, outputDim: 2);
            // One for classifying the peaks
            cwdLSTM.CWDLSTMModel = createTFNETLSTMModel(CWDNamigs.LSTMPeaksClassificationData, lstmModelPath, inputDim: 21, outputDim: 12);

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

        public static TFNETLSTMModel createTFNETLSTMModel(string name, string path, int inputDim, int outputDim)
        {
            int modelSequenceLength = 10;
            int layers = 1;
            TFNETLSTMModel model = new TFNETLSTMModel(path, inputDim, outputDim, modelSequenceLength, layers: layers) { Name = name, Type = ObjectiveType.Classification };

            model.BaseModel.Session = TF_NET_LSTM.LSTMSession(inputDim, outputDim, modelSequenceLength, false, layers);

            // Save the model
            if (path.Length > 0)
                TF_NET_NN.SaveModelVariables(model.BaseModel.Session, path, TF_NET_LSTM.GetOutputCellsNames(model));

            // Set initial thresholds for output decisions
            model.OutputsThresholds = new OutputThresholdItem[outputDim];
            for (int i = 0; i < outputDim; i++)
                model.OutputsThresholds[i] = new OutputThresholdItem();

            // Get the parameters
            return model;
        }

        public void initializeRLModelForCWD(CWDLSTM cwdLSTM)
        {
            TFNETReinforcementL rfModel = (TFNETReinforcementL)cwdLSTM.CWDReinforcementLModel.Clone();
            rfModel.BaseModel.Session = TF_NET_NN.LoadModelVariables(rfModel.BaseModel.ModelPath, "input_place_holder:0", "output:0", CWD_RL_TFNET.createTFNETNeuralNetModelSession);
            cwdLSTM.CWDReinforcementLModel = rfModel;

            TFNETLSTMModel lstmModel = (TFNETLSTMModel)cwdLSTM.CWDLSTMModel.Clone();
            lstmModel.BaseModel.Session = TF_NET_LSTM.LoadLSTMModelVariables(lstmModel);
            cwdLSTM.CWDLSTMModel = lstmModel;

            _objectivesModelsDic.Add(cwdLSTM.ModelName + cwdLSTM.ObjectiveName, cwdLSTM);
        }

        private void UpdateThresholds(TFNETLSTMModel lstmModel, List<List<Sample>> dataListSequences)
        {
            if (dataListSequences.Count == 0)
                return;

            // Set the high and low outputs averaged to zeros
            for (int iOutput = 0; iOutput < lstmModel.BaseModel._outputDim; iOutput++)
            {
                lstmModel.OutputsThresholds[iOutput]._highOutputAv = 0;
                lstmModel.OutputsThresholds[iOutput]._lowOutputAv = 0;
            }

            int[] highOutputsCount = new int[lstmModel.BaseModel._outputDim];
            int[] lowOutputsCount = new int[lstmModel.BaseModel._outputDim];

            Queue<double[]> InputSeqQueue;

            foreach (List<Sample> samplesSeq in dataListSequences)
            {
                InputSeqQueue = new Queue<double[]>(lstmModel._modelSequenceLength + 1);
                foreach (Sample sample in samplesSeq)
                {
                    // Enqueue the new features in InputSeqQueue
                    InputSeqQueue.Enqueue(sample.getFeatures());
                    // Check if the queue has exceeded the sequence length of the model
                    if (InputSeqQueue.Count > lstmModel._modelSequenceLength)
                        InputSeqQueue.Dequeue();

                    // Feed the queue sequence into the LSTM model for prediction
                    List<double[]> output = TF_NET_LSTM.Predict(InputSeqQueue.ToList(), lstmModel);

                    if (output.Count == 0)
                        continue;

                    // Update the threshold _highOutputAv and _lowOutputAv
                    for (int iOutput = 0; iOutput < sample.getOutputs().Length; iOutput++)
                    {
                        double outputaVal = sample.getOutputs()[iOutput];

                        if (outputaVal > 0)
                        {
                            lstmModel.OutputsThresholds[iOutput]._highOutputAv = (lstmModel.OutputsThresholds[iOutput]._highOutputAv * highOutputsCount[iOutput] +
                                                                                 output[output.Count - 1][iOutput]) / (highOutputsCount[iOutput] + 1);
                            highOutputsCount[iOutput] += 1;
                        }
                        else
                        {
                            lstmModel.OutputsThresholds[iOutput]._lowOutputAv = (lstmModel.OutputsThresholds[iOutput]._lowOutputAv * lowOutputsCount[iOutput] +
                                                                                 output[output.Count - 1][iOutput]) / (lowOutputsCount[iOutput] + 1);
                            lowOutputsCount[iOutput] += 1;
                        }
                    }
                }
            }

            // Update the outputs thresholds according to the new _highOutputAv and _lowOutputAv
            for (int iOutput = 0; iOutput < lstmModel.BaseModel._outputDim; iOutput++)
            {
                lstmModel.OutputsThresholds[iOutput]._threshold = (lstmModel.OutputsThresholds[iOutput]._highOutputAv + lstmModel.OutputsThresholds[iOutput]._lowOutputAv) / 2;
            }
        }
    }
}

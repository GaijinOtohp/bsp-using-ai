using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.AITools.DatasetExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tensorflow;
using Tensorflow.NumPy;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.RL_Objectives.CWD_RL;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Filters.CornersScanner;
using static BSP_Using_AI.AITools.DatasetExplorer.DatasetExplorerForm;
using static BSP_Using_AI.AITools.Details.DetailsForm;

namespace BSP_Using_AI.DetailsModify
{
    public partial class FormDetailsModify
    {
        private class LSTMAutoClassifierVars
        {
            public double[] RescaledSamples;

            public List<SignalSegment> RescSegmentsList = new List<SignalSegment>();

            public Dictionary<int, CornerSample> TotalScannedCorners = new Dictionary<int, CornerSample>();

            public LSTMDataBuilderMemory DataBuilderMemory;

            public double[] PredictedOutput;

            public RefDouble[] PredictedRefOutput;
            public (RefDouble[] refOutput, int[] indecies) LatestClassified;
            public AnnotationECG[] LatestClassifiedAnno;
            public List<CornerInterval> MatchedIntrvsList;

            public Queue<double[]> InputSeqQueue;

            public List<double[]> PredictedSequenceList;
            public List<NDArray> LayersLatestOutputsVals = null; public List<NDArray> LayersLatestStatesVals = null;
            public List<Tensor> SequenceCellsInputsPlaceHolders; public List<Tensor> SequenceCellsOutputs;
            public List<Tensor> LayersSartingOutputs; public List<Tensor> LayersStartingStates;
            public List<Tensor> LayersLatestOutputs; public List<Tensor> LayersLatestStates;
        }

        private LSTMAutoClassifierVars _LSTMClasGlobVars;

        private void PlotAnnotation()
        {
            try
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    // Show the annotation in the chart
                    UpdatePointsAnnoPlot();
                }));
            }
            catch (Exception e) { }
        }

        private void LSTMAutoCornersClassifier_Classify(List<CornerSample> cornersToClassify, TFNETLSTMModel CWDLSTMModel)
        {
            foreach (CornerSample scannedCorner in cornersToClassify)
            {
                // Get the features of the selected corner
                double[] features = DatasetExplorerForm.GetCornerFeatures(null, _LSTMClasGlobVars.DataBuilderMemory, scannedCorner._index, _LSTMClasGlobVars.RescaledSamples, _LSTMClasGlobVars.RescSegmentsList);
                // Enqueue the new features in InputSeqQueue
                _LSTMClasGlobVars.InputSeqQueue.Enqueue(features);
                // Check if the queue has exceeded the sequence length of the model
                if (_LSTMClasGlobVars.InputSeqQueue.Count > CWDLSTMModel._modelSequenceLength)
                    _LSTMClasGlobVars.InputSeqQueue.Dequeue();

                // Feed the queue sequence into the LSTM model for prediction
                (_LSTMClasGlobVars.PredictedSequenceList, _LSTMClasGlobVars.LayersLatestOutputsVals, _LSTMClasGlobVars.LayersLatestStatesVals) = TF_NET_LSTM_Recur.PredictSequenciallyFast(_LSTMClasGlobVars.InputSeqQueue.ToList(), CWDLSTMModel,
                                                                                  _LSTMClasGlobVars.LayersLatestOutputsVals, _LSTMClasGlobVars.LayersLatestStatesVals,
                                                                                  _LSTMClasGlobVars.SequenceCellsInputsPlaceHolders, _LSTMClasGlobVars.SequenceCellsOutputs,
                                                                                  _LSTMClasGlobVars.LayersSartingOutputs, _LSTMClasGlobVars.LayersStartingStates,
                                                                                  _LSTMClasGlobVars.LayersLatestOutputs, _LSTMClasGlobVars.LayersLatestStates);

                if (_LSTMClasGlobVars.PredictedSequenceList.Count == 0)
                    continue;
                _LSTMClasGlobVars.PredictedOutput = _LSTMClasGlobVars.PredictedSequenceList[0];

                _LSTMClasGlobVars.PredictedRefOutput = new RefDouble[_LSTMClasGlobVars.PredictedOutput.Length];
                for (int iDouble = 0; iDouble < _LSTMClasGlobVars.PredictedOutput.Length; iDouble++)
                    _LSTMClasGlobVars.PredictedRefOutput[iDouble] = new RefDouble(_LSTMClasGlobVars.PredictedOutput[iDouble]);

                _LSTMClasGlobVars.MatchedIntrvsList = new List<CornerInterval>(_LSTMClasGlobVars.PredictedOutput.Length);

                // Update latestClassifiedRefOutput
                if (_LSTMClasGlobVars.LatestClassified.refOutput == null)
                    _LSTMClasGlobVars.LatestClassified.refOutput = new RefDouble[_LSTMClasGlobVars.PredictedOutput.Length].Select(refDouble => new RefDouble(0)).ToArray();

                for (int iPredOutput = 0; iPredOutput < _LSTMClasGlobVars.PredictedRefOutput.Length; iPredOutput++)
                    if (_LSTMClasGlobVars.PredictedRefOutput[iPredOutput].value >= CWDLSTMModel.OutputsThresholds[iPredOutput]._threshold)
                    {
                        string cornerName = _LSTMClasGlobVars.DataBuilderMemory.OutputsLabels[iPredOutput];
                        // If this is a new peak label then set the data to update dataBuilderMemory
                        if (_LSTMClasGlobVars.LatestClassified.refOutput[iPredOutput].value == 0)
                        {
                            _LSTMClasGlobVars.MatchedIntrvsList.Add(new CornerInterval()
                            {
                                Name = cornerName,
                                cornerIndex = scannedCorner._index
                            });

                            // Create the new annotation to be plotted
                            if (!cornerName.Equals(CWDNamigs.PeaksLabelsOutputs.Other))
                                //lock (_AnnotationData)
                                _LSTMClasGlobVars.LatestClassifiedAnno[iPredOutput] = _AnnotationData.InsertAnnotation(cornerName, AnnotationType.Point, scannedCorner._index, 0);
                        }

                        if (_LSTMClasGlobVars.PredictedRefOutput[iPredOutput].value >= _LSTMClasGlobVars.LatestClassified.refOutput[iPredOutput].value)
                        {
                            _LSTMClasGlobVars.LatestClassified.refOutput[iPredOutput].value = 0;
                            _LSTMClasGlobVars.LatestClassified.refOutput[iPredOutput] = _LSTMClasGlobVars.PredictedRefOutput[iPredOutput];
                            _LSTMClasGlobVars.LatestClassified.indecies[iPredOutput] = scannedCorner._index;

                            // Update the annotation with the new index to be plotted
                            if (!cornerName.Equals(CWDNamigs.PeaksLabelsOutputs.Other))
                            {
                                _LSTMClasGlobVars.LatestClassifiedAnno[iPredOutput].SetNewVals(cornerName, scannedCorner._index, 0);
                                // Show the annotation in the chart
                                PlotAnnotation();
                            }
                        }
                        else
                            _LSTMClasGlobVars.PredictedRefOutput[iPredOutput].value = 0;
                    }
                    else
                        _LSTMClasGlobVars.LatestClassified.refOutput[iPredOutput] = new RefDouble(0);

                // Update dataBuilderMemory
                DatasetExplorerForm.GetOutputsOfTheSample(null, _LSTMClasGlobVars.DataBuilderMemory, _LSTMClasGlobVars.MatchedIntrvsList);
            }
        }

        private void LSTMAutoCornersClassifier_SegmentDelegate(SignalSegment segment, int segmentCount, bool lastSegment, ObjectiveBaseModel baseModel)
        {
            // Get the selected model
            TFNETReinforcementL CWDReinforcementLModel = ((CWDLSTM)baseModel).CWDReinforcementLModel;
            TFNETLSTMModel CWDLSTMModel = ((CWDLSTM)baseModel).CWDLSTMModel;

            // Scan the corners of the segment
            List<CornerSample> scannedCorners = ScanSegmentCorners(segment, _FilteringTools._samplingRate, CWDReinforcementLModel);

            _LSTMClasGlobVars.RescSegmentsList.Add(segment);

            // Copy the new scanned corners to TotalScannedCorners
            foreach (CornerSample cornSample in scannedCorners)
                if (!_LSTMClasGlobVars.TotalScannedCorners.ContainsKey(cornSample._index))
                    _LSTMClasGlobVars.TotalScannedCorners.Add(cornSample._index, cornSample);

            // Classify the corners that are only in the previous segment
            // because the current segment might include more corners from the next segment
            List<CornerSample> cornersToClassify = new List<CornerSample>();
            if (segmentCount > 1 && !lastSegment)
                cornersToClassify = _LSTMClasGlobVars.TotalScannedCorners.Where(corn => _LSTMClasGlobVars.RescSegmentsList[segmentCount - 2].startingIndex <= corn.Key &&
                                                                                        corn.Key <= _LSTMClasGlobVars.RescSegmentsList[segmentCount - 1].startingIndex).
                                                                            Select(corn => corn.Value).ToList();
            else if (lastSegment)
                cornersToClassify = _LSTMClasGlobVars.TotalScannedCorners.Where(corn => _LSTMClasGlobVars.RescSegmentsList[Math.Max(segmentCount - 2, 0)].startingIndex <= corn.Key &&
                                                                                        corn.Key <= _LSTMClasGlobVars.RescSegmentsList[segmentCount - 1].endingIndex).
                                                                            Select(corn => corn.Value).ToList();

            // Sort them according to their indecies
            cornersToClassify = cornersToClassify.OrderBy(corn => corn._index).ToList();

            LSTMAutoCornersClassifier_Classify(cornersToClassify, CWDLSTMModel);

            if (lastSegment)
                try
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        predictionEnd();
                    }));
                }
                catch (Exception e) { }
        }

        private void predictButton_Click_CWDLSTM(string modelNameProblem)
        {
            Thread lstmPredictionThread = new Thread(() =>
            {
                // Get the selected model
                CWDLSTM cwdLSTM = (CWDLSTM)_objectivesModelsDic[modelNameProblem];
                TFNETLSTMModel CWDLSTMModel = cwdLSTM.CWDLSTMModel;

                // Create the annotation data
                _AnnotationData = new AnnotationData(CharacteristicWavesDelineation.ObjectiveName);

                // Initialize the global variables
                _LSTMClasGlobVars = new LSTMAutoClassifierVars();

                // Rescale samples to be in an amplitude interval of 4
                double globalAmpInterval = 4d;
                _LSTMClasGlobVars.RescaledSamples = GeneralTools.rescaleSignal(_FilteringTools._FilteredSamples, globalAmpInterval);

                // Create the data builder memory for generating the input data
                LSTMDataBuilderMemory dataBuilderMemory = new LSTMDataBuilderMemory();

                dataBuilderMemory.OutputsLabels = CWDNamigs.PeaksLabelsOutputs.GetNames();
                //dataBuilderMemory.FeaturesLabels = CWDNamigs.PeaksLabelsFeatures.GetNames();
                dataBuilderMemory.FeaturesLabels = new string[106];
                for (int i = 0; i < 106; i++)
                    dataBuilderMemory.FeaturesLabels[i] = "Feature " + i;
                dataBuilderMemory.samplingRate = _FilteringTools._samplingRate;
                dataBuilderMemory.ProbingIntervals[0] = _FilteringTools._samplingRate;
                dataBuilderMemory.globalAmpInterval = globalAmpInterval;
                _LSTMClasGlobVars.DataBuilderMemory = dataBuilderMemory;

                // The following is used for rectifying predicted outputs
                _LSTMClasGlobVars.LatestClassified = (null, new int[dataBuilderMemory.OutputsLabels.Length]);
                _LSTMClasGlobVars.LatestClassifiedAnno = new AnnotationECG[dataBuilderMemory.OutputsLabels.Length];

                // Create the queue for building the input sequence of the LSTM model
                _LSTMClasGlobVars.InputSeqQueue = new Queue<double[]>(CWDLSTMModel._modelSequenceLength + 1);

                (_LSTMClasGlobVars.SequenceCellsInputsPlaceHolders, _LSTMClasGlobVars.SequenceCellsOutputs) = TF_NET_LSTM_Recur.GetInputOutputPlaceHolders(CWDLSTMModel, CWDLSTMModel.BaseModel.Session);
                (_LSTMClasGlobVars.LayersSartingOutputs, _LSTMClasGlobVars.LayersStartingStates) = TF_NET_LSTM_Recur.GetLayersStartingOutputsAndStates(CWDLSTMModel);
                (_LSTMClasGlobVars.LayersLatestOutputs, _LSTMClasGlobVars.LayersLatestStates) = TF_NET_LSTM_Recur.GetLayersLatestOutputsAndStates(CWDLSTMModel);

                // Scan for the corners using the selected deep Q-learning model with the rescaled segments list
                RLAutoCornersScanner_Prediction(_FilteringTools._FilteredSamples, _FilteringTools._samplingRate, cwdLSTM, LSTMAutoCornersClassifier_SegmentDelegate);
            });
            lstmPredictionThread.Start();
        }
    }
}

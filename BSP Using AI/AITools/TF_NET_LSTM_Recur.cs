using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tensorflow;
using Tensorflow.NumPy;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.AITools.AIBackThreadReportHolder;
using static Tensorflow.Binding;
using static Biological_Signal_Processing_Using_AI.Structures;
using static Tensorflow.Binding;
using Biological_Signal_Processing_Using_AI.Garage;

namespace Biological_Signal_Processing_Using_AI.AITools
{
    public class TF_NET_LSTM_Recur
    {
        public class SequenceBatches
        {
            public int _SequenceLength;

            public List<float[,]> inputBatches;
            public List<float[,]> outputBatches;

            public SequenceBatches(int sequenceLength)
            {
                _SequenceLength = sequenceLength;

                inputBatches = new List<float[,]>(sequenceLength);
                outputBatches = new List<float[,]>(sequenceLength);
            }
        }

        public delegate void PredictionAutoFeedback(List<double[]> predictedSequenceList, List<NDArray> layersLatestOutputsVals, List<NDArray> layersLatestStatesVals);

        private static List<SequenceBatches> GetSeqBatches(List<List<Sample>> dataListSequences, int suggestedBatchSize)
        {
            // Count the number of batches
            int batchesCount = (dataListSequences.Count / suggestedBatchSize) + (dataListSequences.Count % suggestedBatchSize > 0 ? 1 : 0);
            List<SequenceBatches> lstmSequenceBatchesList = new List<SequenceBatches>(batchesCount);

            // Build sequences of batches
            int featuresCount = dataListSequences[0][0].getFeatures().Length;
            int outputsCount = dataListSequences[0][0].getOutputs().Length;
            for (int iBatch1stSeq = 0; iBatch1stSeq < dataListSequences.Count; iBatch1stSeq += suggestedBatchSize)
            {
                // Compute the right batch size
                int batchSize = iBatch1stSeq + suggestedBatchSize <= dataListSequences.Count ? suggestedBatchSize : dataListSequences.Count - iBatch1stSeq;

                // Get the longest sequence in the selected batch
                int batchLongestSequence = dataListSequences.Where((dataList, index) => iBatch1stSeq <= index && index < iBatch1stSeq + batchSize).Max(dataList => dataList.Count);

                // Create the sequence of batches
                SequenceBatches sequenceBatches = new SequenceBatches(batchLongestSequence);
                // Create the batches for the sequence
                for (int iTimeStep = 0; iTimeStep < sequenceBatches._SequenceLength; iTimeStep++)
                {
                    // Create the new batche of the current time step
                    float[,] inputBatch = new float[batchSize, featuresCount];
                    float[,] outputBatch = new float[batchSize, outputsCount];
                    
                    for (int iBatchSequence = iBatch1stSeq; iBatchSequence - iBatch1stSeq < batchSize; iBatchSequence++)
                    {
                        // Get iBatchSequence sample inouts and outputs
                        if (dataListSequences[iBatchSequence].Count > iTimeStep)
                        {
                            double[] features = dataListSequences[iBatchSequence][iTimeStep].getFeatures();
                            double[] outputs = dataListSequences[iBatchSequence][iTimeStep].getOutputs();
                            // Copy the inputs to inputBatch
                            for (int a = 0; a < featuresCount; a++)
                                inputBatch[iBatchSequence - iBatch1stSeq, a] = (float)features[a];
                            // Copy the outputs to outputBatch
                            for (int a = 0; a < outputsCount; a++)
                                outputBatch[iBatchSequence - iBatch1stSeq, a] = (float)outputs[a];
                        }
                    }

                    sequenceBatches.inputBatches.Add(inputBatch);
                    sequenceBatches.outputBatches.Add(outputBatch);
                }

                // Insert the newly created sequences of batches to their lists
                lstmSequenceBatchesList.Add(sequenceBatches);
            }

            return lstmSequenceBatchesList;
        }

        private static List<FeedItem> CreateFitForwardFeedItems(List<Tensor> inputsPlaceHolders, List<Tensor> outputsPlaceHolders, TFNETLSTMModel lstmModel, SequenceBatches sequenceBatches, int iBatch)
        {
            float[,] emptyInputBatch = new float[sequenceBatches.inputBatches[0].GetLength(0), sequenceBatches.inputBatches[0].GetLength(1)];
            float[,] emptyOutputBatch = new float[sequenceBatches.outputBatches[0].GetLength(0), sequenceBatches.outputBatches[0].GetLength(1)];

            List<FeedItem> inOutFeedItemsList = new List<FeedItem>(lstmModel._modelSequenceLength * 2);
            for (int iCell = 0; iCell < lstmModel._modelSequenceLength; iCell++)
            {
                if (iBatch - iCell >= 0)
                {
                    inOutFeedItemsList.Add(new FeedItem(inputsPlaceHolders[iCell], sequenceBatches.inputBatches[iBatch - iCell]));
                    inOutFeedItemsList.Add(new FeedItem(outputsPlaceHolders[iCell], sequenceBatches.outputBatches[iBatch - iCell]));
                }
                else
                {
                    inOutFeedItemsList.Add(new FeedItem(inputsPlaceHolders[iCell], emptyInputBatch));
                    inOutFeedItemsList.Add(new FeedItem(outputsPlaceHolders[iCell], emptyOutputBatch));
                }
            }

            return inOutFeedItemsList;
        }

        public static TFNETLSTMModel Fit(TFNETLSTMModel lstmModel, List<List<Sample>> dataListSequences, FittingProgAIReportDelegate fittingProgAIReportDelegate, bool saveModel = false, int suggestedBatchSize = 4)
        {
            if (lstmModel._pcaActive)
                for (int iSequence = 0; iSequence < dataListSequences.Count; iSequence++)
                    dataListSequences[iSequence] = GeneralTools.rearrangeFeaturesInput(dataListSequences[iSequence], lstmModel.PCA);

            if (dataListSequences.Count > 0)
            {
                // Get the session from the model
                Session session = lstmModel.BaseModel.Session;
                session.as_default();
                //________________________________________________________________________________________________________________________________________//
                //________________________________________________________________________________________________________________________________________//
                // Sort data into batches
                List<SequenceBatches> lstmSequenceBatchesList = GetSeqBatches(dataListSequences, suggestedBatchSize);

                //________________________________________________________________________________________________________________________________________//
                //________________________________________________________________________________________________________________________________________//

                // Start training
                float improvementThreshold = 0.0001f;
                // Get the necessary nodes for training from the model graph
                List<Tensor> sequenceCellsInputsPlaceHolders = new List<Tensor>(lstmModel._modelSequenceLength);
                List<Tensor> sequenceCellsOutputsPlaceHolders = new List<Tensor>(lstmModel._modelSequenceLength);
                for (int iTimeStep = 0; iTimeStep < lstmModel._modelSequenceLength; iTimeStep++)
                {
                    sequenceCellsInputsPlaceHolders.add(session.graph.OperationByName("input_place_holder_cell" + iTimeStep));
                    sequenceCellsOutputsPlaceHolders.add(session.graph.OperationByName("output_place_holder_cell" + iTimeStep));
                }

                Tensor costFunc = session.graph.OperationByName("cost_function");
                Tensor learningRate = session.graph.OperationByName("learning_rate");
                float lRate = 10f;
                Operation optimizer = session.graph.OperationByName("optimizer");

                // The following tensors are used for feed the output of the latest cell to the first cell of the sequence
                // Get the starting output and state of the sequences of all layers
                (List<Tensor> layersSartingOutputs, List<Tensor> layersStartingStates) = GetLayersStartingOutputsAndStates(lstmModel);
                // Get the latest output and state of the sequences of all layers
                (List<Tensor> layersLatestOutputs, List<Tensor> layersLatestStates) = GetLayersLatestOutputsAndStates(lstmModel);
                List<NDArray> layersLatestOutputsVals;
                List<NDArray> layersLatestStatesVals;

                List<ITensorOrOperation> fetchingTensorsList = new List<ITensorOrOperation>(layersLatestOutputs.Count + layersLatestStates.Count);
                fetchingTensorsList.AddRange(layersLatestOutputs);
                fetchingTensorsList.AddRange(layersLatestStates);
                fetchingTensorsList.Add(optimizer);
                fetchingTensorsList.Add(costFunc);

                // Save the initiale values of the variables for restoring the graph if the learning isn't going well
                // Save them as assignment operations to the
                session.graph.as_default();
                IVariableV1[] currentVars = tf.global_variables();
                Dictionary<string, NDArray> currentVarsValsDict = new Dictionary<string, NDArray>();
                foreach (IVariableV1 iVar in currentVars)
                    currentVarsValsDict.Add(iVar.Name, session.run(iVar.AsTensor()));

                // Create an error queue for storing the mean training error of each epoch
                CircularQueue<float> costCirQueue = new CircularQueue<float>(15);
                float meanCost = 0;

                // Start training
                // Max 1000 epochs
                int longestSequence = dataListSequences.Max(dataList => dataList.Count);
                List<FeedItem> inOutFeedItemsList = new List<FeedItem>(lstmModel._modelSequenceLength * 2 + 1 + layersSartingOutputs.Count  * 2); // + 1 for the learning rate
                NDArray[] predictedSequence = null;
                float[,] emptyInputBatch;
                float[,] emptyOutputBatch;
                int epochsMax = 200;
                for (int epoch = 0; epoch < epochsMax; epoch++)
                {
                    // Iterate through the sequences of batches
                    for (int iSequencesBatch = 0; iSequencesBatch < lstmSequenceBatchesList.Count; iSequencesBatch++)
                    {
                        // Create the empty data for the current LSTM model sequence batch
                        emptyInputBatch = new float[lstmSequenceBatchesList[iSequencesBatch].inputBatches[0].GetLength(0), lstmSequenceBatchesList[iSequencesBatch].inputBatches[0].GetLength(1)];
                        emptyOutputBatch = new float[lstmSequenceBatchesList[iSequencesBatch].outputBatches[0].GetLength(0), layersSartingOutputs[0].shape[1]];

                        // Initialize the starting placeholders of the lstm sequence for the selected data sequence
                        layersLatestOutputsVals = new List<NDArray>(layersLatestOutputs.Count);
                        layersLatestStatesVals = new List<NDArray>(layersLatestOutputs.Count);
                        for (int i = 0; i < layersLatestOutputs.Count; i++)
                        {
                            layersLatestOutputsVals.Add(new NDArray(emptyOutputBatch));
                            layersLatestStatesVals.Add(new NDArray(emptyOutputBatch));
                        }

                        // Iterate through the batches of the selected sequence
                        for (int iBatch = 0; iBatch < lstmSequenceBatchesList[iSequencesBatch]._SequenceLength; iBatch++)
                        {
                            // Build the feed items for the current LSTM model sequence batch
                            // Take the last lstmModel._modelSequenceLength batches that are before iBatch
                            // The new batch always goes to the last cell of the LSTM model and not the Cell0
                            // Any cell that has no input and output must be filled with the emptyBatch

                            IncludeNewFeedItems(layersSartingOutputs, layersLatestOutputsVals, inOutFeedItemsList);
                            IncludeNewFeedItems(layersStartingStates, layersLatestStatesVals, inOutFeedItemsList);

                            inOutFeedItemsList.AddRange(CreateFitForwardFeedItems(sequenceCellsInputsPlaceHolders, sequenceCellsOutputsPlaceHolders, lstmModel, lstmSequenceBatchesList[iSequencesBatch], iBatch));
                            
                            inOutFeedItemsList.Add(new FeedItem(learningRate, lRate));

                            // Train the model with the selected batch
                            predictedSequence = session.run(fetchingTensorsList.ToArray(),
                                                            inOutFeedItemsList.ToArray());

                            float cost = predictedSequence[predictedSequence.Length - 1];

                            CopyFetchedValsFromPrediction(predictedSequence, 0, layersLatestOutputsVals);
                            CopyFetchedValsFromPrediction(predictedSequence, layersLatestOutputs.Count, layersLatestStatesVals);

                            inOutFeedItemsList = new List<FeedItem>(lstmModel._modelSequenceLength * 1 + layersSartingOutputs.Count * 2);

                            // Check if the cost went to infinity
                            if (float.IsNaN(cost) || float.IsInfinity(cost))
                            {
                                // If yes then decrease the learning rate
                                lRate /= 10f;
                                // Restore the initial values of the variables (weights, and biases)
                                TF_NET_NN.RefreshModelAndUpdateInitVals(lstmModel.BaseModel, currentVars, currentVarsValsDict);
                            }

                            // Update the last and the mean cost value
                            meanCost += cost / (longestSequence * lstmSequenceBatchesList.Count);
                        }
                    }

                    // Update fitProgressBar
                    if (fittingProgAIReportDelegate != null)
                        fittingProgAIReportDelegate(epoch, epochsMax);

                    // Check if the learning is not improving according to improvementThreshold
                    // in the last 15 epochs
                    costCirQueue.Enqueue(meanCost);
                    meanCost = 0;

                    if (costCirQueue._count == 15)
                    {
                        (float mean, float min, float max) = GeneralTools.MeanMinMax(costCirQueue.ToArray());
                        if ((max - min) < improvementThreshold * lstmModel._modelSequenceLength)
                            // If yes then there is no greate improvement. We can stop learning here
                            break;
                    }
                }

                // Update fitProgressBar
                if (fittingProgAIReportDelegate != null)
                    fittingProgAIReportDelegate(epochsMax, epochsMax);

                // Save model
                if (saveModel)
                    TF_NET_NN.SaveModelVariables(session, lstmModel.BaseModel.ModelPath, GetOutputCellsNames(lstmModel));
            }

            return lstmModel;
        }


        private static List<double[]> PCARearrange(List<double[]> featuresSequence, TFNETLSTMModel model)
        {
            if (model._pcaActive)
                for (int i = 0; i < featuresSequence.Count; i++)
                    featuresSequence[i] = GeneralTools.rearrangeInput(featuresSequence[i], model.PCA);

            return featuresSequence;
        }

        public static (List<Tensor> inputPlaceHolders, List<Tensor> outputs) GetInputOutputPlaceHolders(TFNETLSTMModel model, Session session)
        {
            string lastLayerName = "layer" + (model._layers - 1); // Layers count starts from 0
            if (model._bidirectional)
                lastLayerName = "bi_layer" + (model._layers - 1); // Layers count starts from 0
            List<Tensor> sequenceCellsInputsPlaceHolders = new List<Tensor>(model._modelSequenceLength);
            List<Tensor> sequenceCellsOutputs = new List<Tensor>(model._modelSequenceLength);
            for (int iTimeStep = 0; iTimeStep < model._modelSequenceLength; iTimeStep++)
            {
                sequenceCellsInputsPlaceHolders.add(session.graph.OperationByName("input_place_holder_cell" + iTimeStep));
                sequenceCellsOutputs.add(session.graph.OperationByName(lastLayerName + "_cell" + iTimeStep + "_output"));
            }

            return (sequenceCellsInputsPlaceHolders, sequenceCellsOutputs);
        }

        public static string[] GetOutputCellsNames(TFNETLSTMModel lstmModel)
        {
            string[] outputsNames = new string[lstmModel._modelSequenceLength];
            string lastLayerName = "layer" + (lstmModel._layers - 1); // Layers count starts from 0
            if (lstmModel._bidirectional)
                lastLayerName = "bi_layer" + (lstmModel._layers - 1); // Layers count starts from 0
            for (int i = 0; i < outputsNames.Length; i++)
                outputsNames[i] = lastLayerName + "_cell" + i + "_output";

            return outputsNames;
        }

        public static (List<Tensor> layersStartingOutputs, List<Tensor> layersStartingStates) GetLayersStartingOutputsAndStates(TFNETLSTMModel model)
        {
            // Get the session from the model
            Session session = model.BaseModel.Session;

            string layerBaseName = "layer";
            int sequencesCount = model._layers;
            if (model._bidirectional)
            {
                layerBaseName = "bi_layer";
                sequencesCount *= 2;
            }

            List<Tensor> layersStartingOutputs = new List<Tensor>(sequencesCount);
            List<Tensor> layersStartingStates = new List<Tensor>(sequencesCount);

            for (int iLayer = 0; iLayer < model._layers; iLayer++)
            {
                if (model._bidirectional)
                {
                    // Get the tensors of the direct sequence of the current selected layer
                    layersStartingOutputs.Add(session.graph.OperationByName(layerBaseName + iLayer + "_direct_cells_startingOutput_place_holder"));
                    layersStartingStates.Add(session.graph.OperationByName(layerBaseName + iLayer + "_direct_cells_startingState_place_holder"));
                    // and of the reversed sequence
                    layersStartingOutputs.Add(session.graph.OperationByName(layerBaseName + iLayer + "_reverse_cells_startingOutput_place_holder"));
                    layersStartingStates.Add(session.graph.OperationByName(layerBaseName + iLayer + "_reverse_cells_startingState_place_holder"));
                }
                else
                {
                    layersStartingOutputs.Add(session.graph.OperationByName(layerBaseName + iLayer + "_cells_startingOutput_place_holder"));
                    layersStartingStates.Add(session.graph.OperationByName(layerBaseName + iLayer + "_cells_startingState_place_holder"));
                }
            }

            return (layersStartingOutputs, layersStartingStates);
        }

        public static (List<Tensor> layersLatestOutputs, List<Tensor> layersLatestStates) GetLayersLatestOutputsAndStates(TFNETLSTMModel model)
        {
            // Get the session from the model
            Session session = model.BaseModel.Session;

            string layerBaseName = "layer";
            int sequencesCount = model._layers;
            if (model._bidirectional)
            {
                layerBaseName = "bi_layer";
                sequencesCount *= 2;
            }

            List<Tensor> layersLatestOutputs = new List<Tensor>(sequencesCount);
            List<Tensor> layersLatestStates = new List<Tensor>(sequencesCount);

            int latestCellIndex = model._modelSequenceLength - 1;
            for (int iLayer = 0; iLayer < model._layers; iLayer++)
            {
                if (model._bidirectional)
                {
                    // Get the tensors of the direct sequence of the current selected layer
                    layersLatestOutputs.Add(session.graph.OperationByName(layerBaseName + iLayer + "_direct_cell" + latestCellIndex + "_output"));
                    layersLatestStates.Add(session.graph.OperationByName(layerBaseName + iLayer + "_direct_cell" + latestCellIndex + "_new_state"));
                    // and of the reversed sequence
                    layersLatestOutputs.Add(session.graph.OperationByName(layerBaseName + iLayer + "_reverse_cell" + latestCellIndex + "_output"));
                    layersLatestStates.Add(session.graph.OperationByName(layerBaseName + iLayer + "_reverse_cell" + latestCellIndex + "_new_state"));
                }
                else
                {
                    layersLatestOutputs.Add(session.graph.OperationByName(layerBaseName + iLayer + "_cell" + latestCellIndex + "_output"));
                    layersLatestStates.Add(session.graph.OperationByName(layerBaseName + iLayer + "_cell" + latestCellIndex + "_new_state"));
                }
            }

            return (layersLatestOutputs, layersLatestStates);
        }

        private static List<FeedItem> CreatePredictionForwardFeedItems(List<Tensor> sequenceCellsInputsPlaceHolders, TFNETLSTMModel lstmModel, List<double[]> featuresSequence, int iSequenceTimeStep)
        {
            List<FeedItem> inputFeedItemsList = new List<FeedItem>(lstmModel._modelSequenceLength);

            float[,] featuresFloat;
            for (int iCell = 0; iCell < lstmModel._modelSequenceLength; iCell++)
            {
                featuresFloat = new float[1, featuresSequence[iSequenceTimeStep].Length];
                if (iSequenceTimeStep - iCell >= 0)
                    for (int iFeature = 0; iFeature < featuresSequence[iSequenceTimeStep - iCell].Length; iFeature++)
                        featuresFloat[0, iFeature] = (float)featuresSequence[iSequenceTimeStep - iCell][iFeature];
                inputFeedItemsList.Add(new FeedItem(sequenceCellsInputsPlaceHolders[iCell], featuresFloat));
            }

            return inputFeedItemsList;
        }

        private static void IncludeNewFeedItems(List<Tensor> keys, List<NDArray> vals, List<FeedItem> collectingFeedItemsList)
        {
            for (int iFeedItem = 0; iFeedItem < keys.Count; iFeedItem++)
                collectingFeedItemsList.Add(new FeedItem(keys[iFeedItem], vals[iFeedItem]));
        }

        private static void CopyFetchedValsFromPrediction(NDArray[] prediction, int valsStartingIndex, List<NDArray> collectingList)
        {
            collectingList = new List<NDArray>(collectingList.Count);
            for (int iPredictedVal = valsStartingIndex; iPredictedVal < valsStartingIndex + collectingList.Count; iPredictedVal++)
                collectingList.Add(prediction[iPredictedVal]);
        }

        public static (List<double[]> predictedSequenceList, List<NDArray> layersLatestOutputsVals, List<NDArray> layersLatestStatesVals)
            PredictSequenciallyFast(List<double[]> featuresSequence, TFNETLSTMModel model,
            List<NDArray> layersLatestOutputsVals, List<NDArray> layersLatestStatesVals,
            List<Tensor> sequenceCellsInputsPlaceHolders = null, List<Tensor> sequenceCellsOutputs = null,
            List<Tensor> layersSartingOutputs = null, List<Tensor> layersStartingStates = null,
            List<Tensor> layersLatestOutputs = null, List<Tensor> layersLatestStates = null)
        {
            // Get the cells input and output tensors
            if (sequenceCellsInputsPlaceHolders == null || sequenceCellsOutputs == null)
                (sequenceCellsInputsPlaceHolders, sequenceCellsOutputs) = GetInputOutputPlaceHolders(model, model.BaseModel.Session);
            // Get the starting output and state of the sequences of all layers
            if (layersSartingOutputs == null || layersStartingStates == null)
                (layersSartingOutputs, layersStartingStates) = GetLayersStartingOutputsAndStates(model);
            // Get the latest output and state of the sequences of all layers
            if (layersLatestOutputs == null || layersLatestStates == null)
                (layersLatestOutputs, layersLatestStates) = GetLayersLatestOutputsAndStates(model);

            int iSeqStart = featuresSequence.Count > model._modelSequenceLength ? model._modelSequenceLength - 1 : featuresSequence.Count - 1;

            return PredictSequencially(featuresSequence, model,
                                       sequenceCellsInputsPlaceHolders, sequenceCellsOutputs,
                                       layersSartingOutputs, layersStartingStates,
                                       layersLatestOutputs, layersLatestStates,
                                       layersLatestOutputsVals, layersLatestStatesVals,
                                       iSeqStart);
        }

        public static List<double[]> PredictSequencially(List<double[]> featuresSequence, TFNETLSTMModel model)
        {
            // Get the cells input and output tensors
            (List<Tensor> sequenceCellsInputsPlaceHolders, List<Tensor> sequenceCellsOutputs) = GetInputOutputPlaceHolders(model, model.BaseModel.Session);
            // Get the starting output and state of the sequences of all layers
            (List<Tensor> layersSartingOutputs, List<Tensor> layersStartingStates) = GetLayersStartingOutputsAndStates(model);
            // Get the latest output and state of the sequences of all layers
            (List<Tensor> layersLatestOutputs, List<Tensor> layersLatestStates) = GetLayersLatestOutputsAndStates(model);

            return PredictSequencially(featuresSequence, model,
                                       sequenceCellsInputsPlaceHolders, sequenceCellsOutputs,
                                       layersSartingOutputs, layersStartingStates,
                                       layersLatestOutputs, layersLatestStates).predictedSequenceList;
        }

        public static void PredictSequenciallyAutoFeedback(List<double[]> totalFeaturesSequence, TFNETLSTMModel model, PredictionAutoFeedback feedbackDelegate)
        {
            // Get the cells input and output tensors
            (List<Tensor> sequenceCellsInputsPlaceHolders, List<Tensor> sequenceCellsOutputs) = GetInputOutputPlaceHolders(model, model.BaseModel.Session);
            // Get the starting output and state of the sequences of all layers
            (List<Tensor> layersSartingOutputs, List<Tensor> layersStartingStates) = GetLayersStartingOutputsAndStates(model);
            // Get the latest output and state of the sequences of all layers
            (List<Tensor> layersLatestOutputs, List<Tensor> layersLatestStates) = GetLayersLatestOutputsAndStates(model);

            Queue<double[]> InputSeqQueue = new Queue<double[]>(model._modelSequenceLength + 1);

            List<double[]> predictedSequenceList;
            List<NDArray> layersLatestOutputsVals = null, layersLatestStatesVals = null;

            foreach (double[] iFeatures in totalFeaturesSequence)
            {
                // Enqueue the new features in InputSeqQueue
                InputSeqQueue.Enqueue(iFeatures);
                // Check if the queue has exceeded the sequence length of the model
                if (InputSeqQueue.Count > model._modelSequenceLength)
                    InputSeqQueue.Dequeue();

                // Predict the new sequence
                (predictedSequenceList, layersLatestOutputsVals, layersLatestStatesVals) = PredictSequencially(InputSeqQueue.ToList(), model,
                                                                                                               sequenceCellsInputsPlaceHolders, sequenceCellsOutputs,
                                                                                                               layersSartingOutputs, layersStartingStates,
                                                                                                               layersLatestOutputs, layersLatestStates,
                                                                                                               layersLatestOutputsVals, layersLatestStatesVals,
                                                                                                               InputSeqQueue.Count);

                // Return the predicted values via the delegate
                feedbackDelegate(predictedSequenceList, layersLatestOutputsVals, layersLatestStatesVals);
            }
        }

        /// <summary>
        /// Gives the same results as Predict and PredictSequenciallyFast but much slower
        /// This prediction function takes the input sequencally one by one
        /// and feed it to the LSTM cells one by one from the last cell to the first.
        /// At first, the first cells will receive empty values.
        /// </summary>
        /// <param name="featuresSequence"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private static (List<double[]> predictedSequenceList, List<NDArray> layersLatestOutputsVals, List<NDArray> layersLatestStatesVals)
            PredictSequencially(List<double[]> featuresSequence, TFNETLSTMModel model,
            List<Tensor> sequenceCellsInputsPlaceHolders, List<Tensor> sequenceCellsOutputs,
            List<Tensor> layersSartingOutputs, List<Tensor> layersStartingStates,
            List<Tensor> layersLatestOutputs, List<Tensor> layersLatestStates,
            List<NDArray> layersLatestOutputsVals = null, List<NDArray> layersLatestStatesVals = null,
            int iSeqStart = 0)
        {
            // Initialize input
            featuresSequence = PCARearrange(featuresSequence, model);

            // Get the session from the model
            Session session = model.BaseModel.Session;

            session.graph.as_default();
            session.as_default();

            if (layersLatestOutputsVals == null || layersLatestStatesVals == null)
            {
                layersLatestOutputsVals = new List<NDArray>(layersLatestOutputs.Count);
                layersLatestStatesVals = new List<NDArray>(layersLatestOutputs.Count);
            }
            if (layersLatestOutputsVals.Count == 0 || layersLatestStatesVals.Count == 0)
            {
                for (int i = 0; i < layersLatestOutputs.Count; i++)
                {
                    layersLatestOutputsVals.Add(new NDArray(new float[1, layersSartingOutputs[0].shape[1]]));
                    layersLatestStatesVals.Add(new NDArray(new float[1, layersStartingStates[0].shape[1]]));
                }
            }

            List<Tensor> fetchingTensorsList = new List<Tensor>(sequenceCellsOutputs.Count + layersLatestOutputs.Count + layersLatestStates.Count);
            fetchingTensorsList.AddRange(layersLatestOutputs);
            fetchingTensorsList.AddRange(layersLatestStates);
            fetchingTensorsList.AddRange(sequenceCellsOutputs);

            // Arrange the features in a float[,]
            List<FeedItem> inputFeedItemsList = new List<FeedItem>(model._modelSequenceLength + layersLatestOutputs.Count + layersLatestStates.Count);
            Tensorflow.NumPy.NDArray[] predictedSequence = null;
            for (int iSequenceTimeStep = iSeqStart; iSequenceTimeStep < featuresSequence.Count; iSequenceTimeStep++)
            {
                IncludeNewFeedItems(layersSartingOutputs, layersLatestOutputsVals, inputFeedItemsList);
                IncludeNewFeedItems(layersStartingStates, layersLatestStatesVals, inputFeedItemsList);

                inputFeedItemsList.AddRange(CreatePredictionForwardFeedItems(sequenceCellsInputsPlaceHolders, model, featuresSequence, iSequenceTimeStep));

                // Predict the input
                predictedSequence = session.run(fetchingTensorsList.ToArray(), inputFeedItemsList.ToArray());

                CopyFetchedValsFromPrediction(predictedSequence, 0, layersLatestOutputsVals);
                CopyFetchedValsFromPrediction(predictedSequence, layersLatestOutputs.Count, layersLatestStatesVals);

                inputFeedItemsList = new List<FeedItem>(model._modelSequenceLength + layersLatestOutputs.Count + layersLatestStates.Count);
            }

            // Return result to main user interface
            List<double[]> predictedSequenceList = new List<double[]>(predictedSequence.Length - 2);
            for (int i = layersLatestOutputs.Count + layersLatestStates.Count; i < predictedSequence.Length; i++)
                predictedSequenceList.Add(predictedSequence[i].ToMultiDimArray<float>().OfType<float>().Select(val => (double)val).ToArray());

            return (predictedSequenceList, layersLatestOutputsVals, layersLatestStatesVals);
        }

        public static Session LSTMSession(TFNETLSTMModel lstmModel, Dictionary<string, NDArray> initVarsVals = null)
        {
            // Disable eager mode to enable storing new nodes to the default graph automatically
            tf.compat.v1.disable_eager_execution();

            // The model operations and variables are organized in a graph
            // The graph is automatically built in the default graph
            Graph graph = new Graph().as_default();

            // Define the input and output placeholders
            int inputDim = lstmModel._inputDim;
            int outputDim = lstmModel._outputDim;
            int timeSteps = lstmModel._modelSequenceLength;
            bool bidirectional = lstmModel._bidirectional;
            int hiddenLayers = lstmModel._layers;

            List<Tensor> sequenceCellsInputs = new List<Tensor>(timeSteps);
            List<Tensor> sequenceCellsOutputsPlaceHolders = new List<Tensor>(timeSteps);
            for (int iTimeStep = 0; iTimeStep < timeSteps; iTimeStep++)
            {
                sequenceCellsInputs.add(tf.placeholder(tf.float32, (-1, inputDim), name: "input_place_holder_cell" + iTimeStep));
                sequenceCellsOutputsPlaceHolders.add(tf.placeholder(tf.float32, (-1, outputDim), name: "output_place_holder_cell" + iTimeStep));
            }

            List<Tensor> sequenceCellsOutputs;

            // Build the LSTM model
            // Check if the model is bidirectional
            if (bidirectional)
                sequenceCellsOutputs = MultiLayerBiLSTMSequence(sequenceCellsInputs, outputDim, hiddenLayers, name: "bi_layer");
            else
                sequenceCellsOutputs = MultiLayerLSTMSequence(sequenceCellsInputs, outputDim, hiddenLayers, name: "layer");

            Session session = new Session(graph);
            session.as_default();
            // Assign values to the graph variables
            (Dictionary<string, Operation> initAssignmentsDict, initVarsVals) = TF_NET_NN.AssignValsToVars(session, initVarsVals);
            // Store initVarsVals and their assignments in baseModel
            lstmModel.BaseModel._AssignedValsDict = initVarsVals;
            lstmModel.BaseModel._InitAssignmentsOpDict = initAssignmentsDict;
            // Insert the cost function operation to the graph of the model
            Tensor costFunc = null;
            if (timeSteps > 1)
            {
                costFunc = tf.reduce_mean(tf.square(tf.sub(sequenceCellsOutputs[0], sequenceCellsOutputsPlaceHolders[0])));
                for (int iTimeStep = 1; iTimeStep < timeSteps; iTimeStep++)
                    if (iTimeStep == timeSteps - 1)
                        costFunc = tf.add(costFunc, tf.reduce_mean(tf.square(tf.sub(sequenceCellsOutputs[iTimeStep], sequenceCellsOutputsPlaceHolders[iTimeStep]))), name: "cost_function");
                    else
                        costFunc = tf.add(costFunc, tf.reduce_mean(tf.square(tf.sub(sequenceCellsOutputs[iTimeStep], sequenceCellsOutputsPlaceHolders[iTimeStep]))));
            }
            else
                costFunc = tf.reduce_mean(tf.square(tf.sub(sequenceCellsOutputs[0], sequenceCellsOutputsPlaceHolders[0])), name: "cost_function");
            // Insert the optimizer operation to the graph of the model with a default learning rate of 0.01
            Tensor learning_rate = tf.placeholder(tf.float32, Shape.Scalar, name: "learning_rate");
            Operation optimizer = tf.train.GradientDescentOptimizer(learning_rate).minimize(costFunc, name: "optimizer");

            return session;
        }

        private static (Tensor output, Tensor state) LSTMCell(Tensor input, Tensor prevOutput, Tensor prevState, string name = "_cell0")
        {
            // Get the shape of the output
            int outputDim = (int)prevOutput.shape[1];

            // Join the previous output with the input
            Tensor outputInputJoin = tf.concat(new Tensor[] { prevOutput, input }, axis: 1, name + "_prevOutput_input_concat");

            // Build the gates of the LSTM cell
            Tensor forgetGate = tf.nn.sigmoid(TF_NET_NN.Layer(outputInputJoin, outputDim, name + "_forget_gate"));
            Tensor inputGate = tf.nn.sigmoid(TF_NET_NN.Layer(outputInputJoin, outputDim, name + "_input_gate"));
            Tensor inputNode = tf.nn.tanh(TF_NET_NN.Layer(outputInputJoin, outputDim, name + "_input_node"));
            Tensor outputGate = tf.nn.sigmoid(TF_NET_NN.Layer(outputInputJoin, outputDim, name + "_output_gate"));

            // Perform the operations of the LSTM cell
            Tensor forgetState = tf.multiply(prevState, forgetGate, name + "_forget_state");
            Tensor newState = tf.add(forgetState, tf.multiply(inputGate, inputNode), name + "_new_state");
            Tensor output = tf.multiply(tf.nn.tanh(newState), outputGate, name + "_output");

            return (output, newState);
        }

        private static (List<Tensor> sequenceCellsOutputs, Tensor latestCellState) LSTMSequence(List<Tensor> sequenceCellsInputs, int outputDim, string name = "layer0")
        {
            // Create the starting output and state of the sequence
            Tensor prevOutput = tf.placeholder(tf.float32, (-1, outputDim), name: name + "_cells_startingOutput_place_holder");
            Tensor prevState = tf.placeholder(tf.float32, (-1, outputDim), name: name + "_cells_startingState_place_holder");

            // Create the lstm sequence's chain tensors
            List<Tensor> sequenceCellsOutputs = new List<Tensor>(sequenceCellsInputs.Count);

            // Iterate through the time steps
            for (int iTimeStep = 0; iTimeStep < sequenceCellsInputs.Count; iTimeStep++)
            {
                (prevOutput, prevState) = LSTMCell(sequenceCellsInputs[iTimeStep], prevOutput, prevState, name + "_cell" + iTimeStep);

                sequenceCellsOutputs.Add(prevOutput);
            }

            return (sequenceCellsOutputs, prevState);
        }

        private static List<Tensor> MultiLayerLSTMSequence(List<Tensor> sequenceCellsInputs, int outputDim, int hiddenLayers, string name = "layer0")
        {
            // Create the lstm sequence's chain tensors
            List<Tensor> sequenceCellsOutputs = new List<Tensor>(sequenceCellsInputs.Count);

            // Build the layers sequences
            for (int iLayer = 0; iLayer < hiddenLayers; iLayer++)
            {
                sequenceCellsOutputs = LSTMSequence(sequenceCellsInputs, outputDim, name: name + iLayer).sequenceCellsOutputs;
                sequenceCellsInputs = sequenceCellsOutputs;
            }

            return sequenceCellsOutputs;
        }

        private static List<Tensor> MultiLayerBiLSTMSequence(List<Tensor> directSequenceCellsInputs, int outputDim, int hiddenLayers, string name = "bi_layer0")
        {
            // Reverse the input sequence
            List<Tensor> reverseSequenceCellsInputs = directSequenceCellsInputs.Select(tensor => tensor).ToList();
            reverseSequenceCellsInputs.Reverse();

            // Create the output lstm sequence's chain tensors
            List<Tensor> directSequenceCellsOutputs = new List<Tensor>(directSequenceCellsInputs.Count);
            List<Tensor> reverseSequenceCellsOutputs = new List<Tensor>(reverseSequenceCellsInputs.Count);

            // Build the layers' direct and reversed LSTM sequences
            for (int iLayer = 0; iLayer < hiddenLayers; iLayer++)
            {
                directSequenceCellsOutputs = LSTMSequence(directSequenceCellsInputs, outputDim, name: name + iLayer + "_direct").sequenceCellsOutputs;
                reverseSequenceCellsOutputs = LSTMSequence(reverseSequenceCellsInputs, outputDim, name: name + iLayer + "_reverse").sequenceCellsOutputs;
                directSequenceCellsInputs = directSequenceCellsOutputs;
                reverseSequenceCellsInputs = reverseSequenceCellsOutputs;
            }

            // Reverse the direction of the cells flow of the reverseSequenceCellsOutputs
            // So that the last cell of the reversed sequences should be connected with the first cell of the direct sequences
            reverseSequenceCellsOutputs.Reverse();
            // Merge the outputs of the two final sequences as the merged output tensors
            // By averaging each two cells outputs
            List<Tensor> mergedSequenceCellsOutputs = new List<Tensor>(directSequenceCellsOutputs.Count);
            for (int iCellOutput = 0; iCellOutput < directSequenceCellsOutputs.Count; iCellOutput++)
                mergedSequenceCellsOutputs.Add(tf.multiply(tf.add(directSequenceCellsOutputs[iCellOutput], reverseSequenceCellsOutputs[iCellOutput]), 0.5f, name: name + (hiddenLayers - 1) + "_cell" + iCellOutput + "_output"));

            return mergedSequenceCellsOutputs;
        }

        public static Session LoadLSTMModelVariables(TFNETLSTMModel lstmModel)
        {
            // Load the learned graph variables values
            Graph valsGraph = tf.train.load_graph(lstmModel.BaseModel.ModelPath + "/model_variables.pb");
            // and create a temporal session for reading the variables tensors values
            Session tempSess = tf.Session();

            // Activate restore mode to enable both eager mode (run operations immediately without the need of a graph)
            // and graph mode (stores the new nodes to the default graph)
            tf.Context.restore_mode();

            // Get valsGraph Variables' values
            Dictionary<string, NDArray> initVarsVals = new Dictionary<string, Tensorflow.NumPy.NDArray>();
            string[] varsTensorsNames = valsGraph.get_operations().Where(iTensOp => iTensOp.op.OpType == "Identity").Select(iTensOp => iTensOp.name.Split("/")[0] + ":0").ToArray();
            foreach (string varTensName in varsTensorsNames)
            {
                Tensor valsTensor = valsGraph.get_tensor_by_name(varTensName);
                Tensorflow.NumPy.NDArray val = tempSess.run(valsTensor);
                initVarsVals.Add(varTensName, val);
            }

            // Create a new session for the model
            Session session = LSTMSession(lstmModel, initVarsVals);

            return session;
        }
    }
}

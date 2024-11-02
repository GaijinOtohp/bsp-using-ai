using Biological_Signal_Processing_Using_AI.DetailsModify.Filters;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI;
using BSP_Using_AI.AITools;
using BSP_Using_AI.AITools.DatasetExplorer;
using NWaves.Filters.Base;
using NWaves.Signals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.ReinforcementLearning.Environment;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Filters.CornersScanner;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace Biological_Signal_Processing_Using_AI.AITools.RL_Objectives
{
    public class CWD_RL
    {
        public class SignalSegment
        {
            public int startingIndex;
            public int endingIndex;
            public double[] SegmentSamples;
            public double segmentMean;
            public double segmentMax;
            public double segmentMin;
        }

        ReinforcementLearning.Environment _Env;

        List<CornerInterval> _ApproxIntervList;

        double[] _RescaledSamples;
        double _samplingRate;

        double _penalty_movement = 1; // steps

        double _penalty_false_negative = 5; // not detecting the thing
        double _penalty_false_positive = 1; // detecting other thing

        double _reward_true_positive = 1;
        double _reward_detecting_all_corners = 5;

        CircularQueue<int> _atCircularQueue;
        CircularQueue<int> _artCircularQueue;

        List<SignalSegment> _SignalSegmentsList;

        int _selectedSegment;

        bool _done = false;
        double _overPeaksRatio = 20d;
        double _ratioIncrement = 0.5d;
        double _overPeaksRatioReset = 20d;
        int _holdRatio = 50;
        int _holdRatioReset = 50;

        public delegate void SegmentDelegate(SignalSegment segment, int segmentCount);

        public CWD_RL(List<RLDimension> dimensionsList)
        {
            // Set the environment
            double learningRate = 0.1d;
            double discount = 0.95d;
            _Env = new ReinforcementLearning.Environment(dimensionsList, learningRate, discount, ComputeReward, CheckIfDone);
        }

        private (double reward, bool badState) ComputeReward(int[] state, ReinforcementLearning.Environment env)
        {
            double reward = 0;
            bool badState = false;
            // Get selected window samples
            double[] windowSamples = _RescaledSamples.Where((val, index) => _SignalSegmentsList[_selectedSegment].startingIndex <= index && index <= _SignalSegmentsList[_selectedSegment].endingIndex).ToArray();

            // Scan corners of the selected window
            List<RLDimension> dimList = env._DimensionsList;
            double at = dimList[0]._min + (state[0] * dimList[0]._step);
            double art = dimList[1]._min + (state[1] * dimList[1]._step);
            List<CornerSample> tempCornersList = ScanCorners(windowSamples, _SignalSegmentsList[_selectedSegment].startingIndex, _samplingRate, art, at);
            // Take only the indexes of the corners
            int[] tempCornersIndex = tempCornersList.Select(corner => corner._index).ToArray();
            // Create a list of the intervals holding the corners in tempCornersIndex
            List<CornerInterval> tempIntervalsList = new List<CornerInterval>();

            // Compute the reward of current step
            // Get the true corners of only the selected window
            //int[] windowTrueCornersIndex = _TrueCornersIndex.Where(cornerIndex => _chunk * _chunkStep <= cornerIndex && cornerIndex < (_chunk * _chunkStep) + _samplingRate).ToArray();
            // Get the intervals of the true corners of the selected window only
            CornerInterval[] windowapproxIntervals = _ApproxIntervList.Where(interval => _SignalSegmentsList[_selectedSegment].startingIndex < interval.starting && interval.ending < _SignalSegmentsList[_selectedSegment].endingIndex).ToArray();

            // Compute reward value of the current step
            reward -= _penalty_movement;
            foreach (int cornerIndex in tempCornersIndex)
            {
                // Check if the scanned corner is included in true corner interval
                List<CornerInterval> chosenInterval = windowapproxIntervals.Where(interval => interval.starting <= cornerIndex && cornerIndex <= interval.ending).ToList();
                if (chosenInterval.Count() > 0)
                {
                    // If yes then check if this interval has not included a previous corner
                    if (!tempIntervalsList.Contains(chosenInterval[0]))
                    {
                        //if (windowTrueCornersIndex.Contains(cornerIndex))
                        reward += _reward_true_positive;
                        tempIntervalsList.Add(chosenInterval[0]);
                    }
                    else
                        reward -= _penalty_false_positive;
                }
                else
                    reward -= _penalty_false_positive;
            }
            // Compute the reward of the false negatives
            foreach (CornerInterval interval in windowapproxIntervals)
                if (tempCornersIndex.Where(cornerIndex => interval.starting <= cornerIndex && cornerIndex <= interval.ending).Count() == 0)
                //foreach (int cornerIndex in windowTrueCornersIndex)
                //    if (!tempCornersIndex.Contains(cornerIndex))
                {
                    reward -= _penalty_false_negative;
                    badState = true;
                }

            if (!badState && (((double)tempCornersIndex.Length / windowapproxIntervals.Length) < _overPeaksRatio || windowapproxIntervals.Length == 0))
            {
                reward += _reward_detecting_all_corners;
                _done = true;
            }

            return (reward, badState);
        }

        private bool CheckIfDone(int[] state)
        {
            bool done = false;
            // ----Done when emprovement was almost stable----
            _atCircularQueue.Enqueue(state[0]);
            _artCircularQueue.Enqueue(state[1]);

            if (GeneralTools.amplitudeInterval(_atCircularQueue.ToArray().Select(at => (double)at).ToArray()) < 2 && _atCircularQueue._count > 4 &&
                GeneralTools.amplitudeInterval(_artCircularQueue.ToArray().Select(art => (double)art).ToArray()) < 2 && _artCircularQueue._count > 4 &&
                !_done)
            {
                _Env.AgentReset();
                _atCircularQueue = new CircularQueue<int>(5);
                _artCircularQueue = new CircularQueue<int>(5);
            }

            if (_done)
            {
                done = true;
                _done = false;
                _holdRatio = _holdRatioReset;
            }
            else if (_holdRatio > 0)
                _holdRatio--;
            else
            {
                _overPeaksRatio += _ratioIncrement;
                _holdRatio = _holdRatioReset;
            }

            return done;
        }

        public static AnnotationECG[] GetCornersExceptDleta(AnnotationData annoData)
        {
            // Remove the peaks of delta and any annotation that has the same index
            AnnotationECG[] trueCorners = annoData.GetAnnotations().GroupBy(anno => anno.GetIndexes().starting).Where(group => !group.ToList().Select(ecgAnno => ecgAnno.Name).
                                                                                                                                               Contains(CWDNamigs.Delta)).
                                                                                                                SelectMany(group => group.ToList()).
                                                                                                                ToArray();

            return trueCorners;
        }

        public static AnnotationECG[] GetCornersWithExceptionAndRelation(AnnotationData annoData, string[] exceptions)
        {
            // Group the annotation according to their indecies
            List<List<AnnotationECG>> annotationIndexGroups = annoData.GetAnnotations().GroupBy(anno => anno.GetIndexes().starting).Select(group => group.ToList()).ToList();

            // Remove any group that contains the exception
            foreach (string exception in exceptions)
                annotationIndexGroups = annotationIndexGroups.Where(group => !group.Select(ecgAnno => ecgAnno.Name).
                                                                                   Contains(exception)).
                                                              ToList();

            // Concatenate the groups in one list
            AnnotationECG[] trueCorners = annotationIndexGroups.SelectMany(group => group).ToArray();

            return trueCorners;
        }

        public static AnnotationECG[] GetCornersWithException(AnnotationData annoData, string[] exceptions)
        {
            AnnotationECG[] trueCorners = annoData.GetAnnotations().ToArray();

            foreach (string exception in exceptions)
                trueCorners = trueCorners.Where(ecgAnno => !ecgAnno.Name.Equals(exception)).ToArray();

            return trueCorners;
        }

        public Data DeepFitRLData(double[] samples, int samplingRate, AnnotationData annoData, TFNETReinforcementL CWDCrazyReinforcementLModel)
        {
            RLDimension atDim = _Env._DimensionsList.Where(dim => dim._Name.Equals(CWDNamigs.CornersScanOutputs.AT)).ToList()[0];
            RLDimension artDim = _Env._DimensionsList.Where(dim => dim._Name.Equals(CWDNamigs.CornersScanOutputs.ART)).ToList()[0];
            // Include the signal infos
            // Rescale samples to be in an amplitude interval of 4
            _RescaledSamples = GeneralTools.rescaleSignal(samples, 4);
            _samplingRate = samplingRate;

            AnnotationECG[] trueCorners = CWD_RL.GetCornersWithException(annoData, new string[] { CWDNamigs.Delta, CWDNamigs.Normal, CWDNamigs.Abnormal });

            // Create the intervals covering 20% from the corners in both sides
            _ApproxIntervList = ApproximateIndexesToIntervals(trueCorners, 40, _RescaledSamples, samplingRate);

            // Initialize the conditions of finishing the episodes
            _atCircularQueue = new CircularQueue<int>(5);
            _artCircularQueue = new CircularQueue<int>(5);

            // Create the global features and outputs data object
            Data GlobalCornersScanData = new Data(CWDNamigs.RLCornersScanData);

            // Segment the samples of signal according to the chunks' distribution
            _SignalSegmentsList = SegmentTheMainSamples(_RescaledSamples, (int)_samplingRate, 0.5d, null);
            int maxEpisodes = 3;
            for (_selectedSegment = 0; _selectedSegment < _SignalSegmentsList.Count; _selectedSegment++)
            {
                // ---------------------------------------------Try reset then new environment's q-table
                _Env.QTableReset();
                _overPeaksRatio = _overPeaksRatioReset;

                // Iterate through episodes
                for (int iEpisode = 0; iEpisode < maxEpisodes; iEpisode++)
                {
                    // Create the features and outputs data object
                    Data EpisodeCornersScanData = new Data(CWDNamigs.RLCornersScanData);
                    // Get the features of the selected segment
                    Sample EpisodeSample = GetFeaturesOfTheSamples(EpisodeCornersScanData);
                    // Predict the initial state of the selected segment
                    double[] features = EpisodeSample.getFeatures();

                    double[] atARTOutput = null;
                    lock (CWDCrazyReinforcementLModel)
                        atARTOutput = TF_NET_NN.predict(features, CWDCrazyReinforcementLModel, CWDCrazyReinforcementLModel.BaseModel.Session);

                    // Start training with the new initial state in the new episode
                    (int[] episodeBestState, bool badState) = _Env.DeepTrain(atARTOutput);

                    // Check if the predicted state of the crazy model is greater than the improvementThreshold
                    double improvementThreshold = 0.001d;
                    double bestAT = (episodeBestState[0] * atDim._step) / (atDim._max - atDim._min);
                    double bestART = (episodeBestState[1] * artDim._step) / (artDim._max - artDim._min);
                    if ((Math.Abs(atARTOutput[0] - bestAT) + Math.Abs(atARTOutput[1] - bestART)) > improvementThreshold)
                    {
                        // Then train the crazy model with the new sample
                        // Include the best state as output to the sample
                        EpisodeSample.insertOutput(0, CWDNamigs.CornersScanOutputs.AT, bestAT);
                        EpisodeSample.insertOutput(1, CWDNamigs.CornersScanOutputs.ART, bestART);

                        List<Sample> trainingSamplesList = new List<Sample>() { EpisodeSample };
                        lock (CWDCrazyReinforcementLModel)
                            TF_NET_NN.fit(CWDCrazyReinforcementLModel, CWDCrazyReinforcementLModel.BaseModel, trainingSamplesList, null, saveModel: false, epochsMax: 1);
                    }
                }

                // Get the features of the selected segment
                Sample SegmentSample = GetFeaturesOfTheSamples(GlobalCornersScanData);
                // Get the best state of the environment
                Dictionary<string, (double reward, bool badState)> generalQTable = _Env.GetQTableDict();
                List<(double reward, string state)> acceptableStates = generalQTable.Where(state => state.Value.badState == false).Select(state => (state.Value.reward, state.Key)).ToList();
                // Check if there was no best state
                if (acceptableStates.Count == 0)
                    // Then just take the whole Q table
                    acceptableStates = generalQTable.Select(state => (state.Value.reward, state.Key)).ToList();
                // Take the state with the highest reward
                int[] segmentBestState = ReinforcementLearning.Environment.String2IntArray(acceptableStates.Max().state);

                SegmentSample.insertOutput(0, CWDNamigs.CornersScanOutputs.AT, (segmentBestState[0] * atDim._step) / (atDim._max - atDim._min));
                SegmentSample.insertOutput(1, CWDNamigs.CornersScanOutputs.ART, (segmentBestState[1] * artDim._step) / (artDim._max - artDim._min));
            }

            return GlobalCornersScanData;
        }

        public static List<SignalSegment> SegmentTheMainSamples(double[] globalSamples, int samplingRate, double distributionBarThreshold, SegmentDelegate segmentDelegate)
        {
            List<SignalSegment> signalSegmentsList = new List<SignalSegment>();
            // The segment should be at least 0.2 seconds long
            int segmentInitialLen = (int)(0.2d * samplingRate) + ((0.2d * samplingRate) % 1 > 0 ? 1 : 0);
            // The segment could extend to the next and previous segments up to 0.1 seconds
            int segmentExtension = (int)(0.1d * samplingRate) + ((0.1d * samplingRate) % 1 > 0 ? 1 : 0);

            int bufferEndingIndexBeforeExtension;
            double[] haarDWTLevel1 = GeneralTools.calculateDWT(globalSamples, "haar", 2)[0];
            double[] absHaarDWTLevel1 = GeneralTools.absoluteSignal(haarDWTLevel1);
            int dwtDownScale = 2;
            List<double> dwtSegmentBuffer;
            int dwtSegmentInitialLen = segmentInitialLen / dwtDownScale;
            int dwtSegmentExtension = segmentExtension / dwtDownScale;
            int dwtSamplingRate = samplingRate / dwtDownScale;
            double[] distribution;

            bool segmentExceededLimit;
            double dwtMin, dwtMax;
            double[] dwtExtended;
            int dwtMaxPrefExt;
            int dwtMaxSuffExt;
            int dwtPrefExtension;
            int dwtSuffExtension;

            for (int iDWTGlobal = 0; iDWTGlobal < absHaarDWTLevel1.Length; iDWTGlobal++)
            {
                SignalSegment signalSegment = new SignalSegment();
                dwtPrefExtension = 0;
                dwtSuffExtension = 0;
                dwtSegmentBuffer = new List<double>(dwtSamplingRate + 2 * dwtSegmentExtension);
                // The segment should be up to 1 second
                for (int iDWTSegment = 0; iDWTSegment < samplingRate && iDWTGlobal + iDWTSegment < absHaarDWTLevel1.Length; iDWTSegment++)
                {
                    dwtSegmentBuffer.Add(absHaarDWTLevel1[iDWTGlobal + iDWTSegment]);
                    // The segment should be at least of length dwtSegmentInitialLen
                    if (dwtSegmentBuffer.Count < dwtSegmentInitialLen)
                        continue;
                    // Compute the distribution of the derivative
                    distribution = DistributionDisplay.CoputeDistribution(dwtSegmentBuffer.ToArray(), 10).distribution;
                    // Check if the distribution of the derivative is not equiprobable
                    segmentExceededLimit = false;
                    foreach (double bar in distribution)
                        if (bar >= distributionBarThreshold)
                        {
                            // Remove the sample that caused the distortion on the distribution
                            dwtSegmentBuffer.RemoveAt(dwtSegmentBuffer.Count - 1);
                            segmentExceededLimit = true;
                            break;
                        }
                    if (segmentExceededLimit)
                        break;
                }
                bufferEndingIndexBeforeExtension = iDWTGlobal + (dwtSegmentBuffer.Count > 0 ? dwtSegmentBuffer.Count : 0);

                // Extend the segment
                // The extension should be in the limits of the derivative's interval
                (_, dwtMin, dwtMax) = GeneralTools.MeanMinMax(dwtSegmentBuffer.ToArray());
                dwtMaxPrefExt = iDWTGlobal - dwtSegmentExtension >= 0 ? dwtSegmentExtension : iDWTGlobal;
                dwtMaxSuffExt = bufferEndingIndexBeforeExtension + dwtSegmentExtension < absHaarDWTLevel1.Length ? dwtSegmentExtension : (absHaarDWTLevel1.Length - bufferEndingIndexBeforeExtension) - 1;

                for (int i = 1; i <= dwtMaxPrefExt; i++)
                {
                    if (dwtMin <= absHaarDWTLevel1[iDWTGlobal - i] && absHaarDWTLevel1[iDWTGlobal - i] <= dwtMax)
                        dwtPrefExtension = i;
                    else
                        break;
                }
                for (int i = 1; i <= dwtMaxSuffExt; i++)
                {
                    if (dwtMin <= absHaarDWTLevel1[bufferEndingIndexBeforeExtension + i] && absHaarDWTLevel1[bufferEndingIndexBeforeExtension + i] <= dwtMax)
                        dwtSuffExtension = i;
                    else
                        break;
                }

                // Include the new segment in signalSegmentsList
                SignalSegment newSegment = new SignalSegment()
                {
                    startingIndex = (iDWTGlobal - dwtPrefExtension) * dwtDownScale,
                    endingIndex = (bufferEndingIndexBeforeExtension + dwtSuffExtension + 1) * dwtDownScale - 1 // Add the samples of the gape between two segments [ (.. + 1 * dwtDownScale) - 1 ]
                };
                newSegment.SegmentSamples = globalSamples.Where((val, index) => newSegment.startingIndex <= index && index <= newSegment.endingIndex).ToArray();
                (newSegment.segmentMean, newSegment.segmentMin, newSegment.segmentMax) = GeneralTools.MeanMinMax(newSegment.SegmentSamples);

                signalSegmentsList.Add(newSegment);

                // Send the segment with the delegate
                if (segmentDelegate != null)
                    segmentDelegate(newSegment, signalSegmentsList.Count);

                // Move iGlobal according to the new segment
                iDWTGlobal = bufferEndingIndexBeforeExtension;
            }

            return signalSegmentsList;
        }

        public static double[] GetFeaturesValues(SignalSegment segment)
        {
            double[] distribution = DistributionDisplay.CoputeDistribution(segment.SegmentSamples, 10).distribution;

            return distribution;
        }

        private Sample GetFeaturesOfTheSamples(Data dataParent)
        {
            double[] features = GetFeaturesValues(_SignalSegmentsList[_selectedSegment]);

            Sample sample = new Sample("segment" + _selectedSegment, features.Length, 2, dataParent);

            for (int i = 0; i < features.Length; i++)
                sample.insertFeature(i, "Feature " + i, features[i]);

            return sample;
        }
    }
}

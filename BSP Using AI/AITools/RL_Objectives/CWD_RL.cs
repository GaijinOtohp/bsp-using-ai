using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI;
using BSP_Using_AI.AITools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tensorflow;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.ReinforcementLearning.Environment;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;
using static BSP_Using_AI.DetailsModify.FormDetailsModify.CornersScanner;
using static Tensorflow.Binding;

namespace Biological_Signal_Processing_Using_AI.AITools.RL_Objectives
{
    public class CWD_RL
    {
        public class SignalSegment
        {
            public int startingIndex;
            public int endingIndex;
            public double[] SegmentSamples;
        }

        ReinforcementLearning.Environment _Env;

        List<CornerInterval> _ApproxIntervList;

        double[] _RescaledSamples;
        double _samplingRate;

        double _penalty_movement = 0.5f; // steps

        double _penalty_false_negative = 10; // not detecting the thing
        double _penalty_false_positive = 1; // detecting other thing

        double _reward_true_positive = 1;
        double _reward_detecting_all_corners = 5;

        CircularQueue<int> _atCircularQueue;
        CircularQueue<int> _artCircularQueue;

        List<SignalSegment> _SignalSegmentsList;

        int _selectedSegment;

        public CWD_RL()
        {
            // Set the environment
            List<ReinforcementLearning.Environment.Dimension> dimensionsList = new List<ReinforcementLearning.Environment.Dimension>(2);
            dimensionsList.Add(new ReinforcementLearning.Environment.Dimension(name: "at", size: 60, min: 1, max: 40));
            dimensionsList.Add(new ReinforcementLearning.Environment.Dimension(name: "art", size: 60, min: 0, max: 0.3d));

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
            ReinforcementLearning.Environment.Dimension atDim = env._DimensionsList.Where(dim => dim._Name.Equals("at")).ToList()[0];
            ReinforcementLearning.Environment.Dimension artDim = env._DimensionsList.Where(dim => dim._Name.Equals("art")).ToList()[0];
            List<CornerSample> tempCornersList = ScanCorners(windowSamples, _SignalSegmentsList[_selectedSegment].startingIndex, _samplingRate, state[1] * artDim._step, state[0] * atDim._step);
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
            //reward -= penalty_movement;
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

            if (!badState)
                reward += _reward_detecting_all_corners;

            return (reward, badState);
        }

        private bool CheckIfDone(int[] state)
        {
            bool done = false;
            // ----Done when emprovement was almost stable----
            _atCircularQueue.Enqueue(state[0]);
            _artCircularQueue.Enqueue(state[1]);

            if (GeneralTools.amplitudeInterval(_atCircularQueue.ToArray().Select(at => (double)at).ToArray()) < 2 && _atCircularQueue._count > 7 &&
                GeneralTools.amplitudeInterval(_artCircularQueue.ToArray().Select(art => (double)art).ToArray()) < 2 && _artCircularQueue._count > 7)
            {
                _atCircularQueue = new CircularQueue<int>(8);
                _artCircularQueue = new CircularQueue<int>(8);
                done = true;
            }

            return done;
        }

        public static AnnotationECG[] GetCornersExceptDleta(AnnotationData annoData)
        {
            // Remove the peaks of delta and any annotation that has the same index
            int[] deltaIndexes = annoData.GetAnnotations().Where(ecgAnno => ecgAnno.Name.Equals(CWDNamigs.Delta)).Select(anno => anno.GetIndexes().starting).ToArray();
            AnnotationECG[] trueCorners = annoData.GetAnnotations().Select(ecgAnno => {
                int annoIndex = ecgAnno.GetIndexes().starting;
                foreach (int deltaIndex in deltaIndexes)
                    if (deltaIndex == annoIndex)
                    {
                        ecgAnno.SetNewVals(ecgAnno.Name, int.MinValue, 0);
                        break;
                    }
                return ecgAnno;
            }).
                                                              Where(ecgAnno => ecgAnno.GetIndexes().starting != int.MinValue).ToArray();

            return trueCorners;
        }

        public Data FitRLData(double[] samples, int samplingRate, AnnotationData annoData)
        {
            // Include the signal infos
            // Rescale samples to be in an amplitude interval of 4
            _RescaledSamples = GeneralTools.rescaleSignal(samples, 4);
            _samplingRate = samplingRate;

            AnnotationECG[] trueCorners = GetCornersExceptDleta(annoData);

            // Create the intervals covering 20% from the corners in both sides
            _ApproxIntervList = ApproximateIndexesToIntervals(trueCorners, 40, samples, samplingRate);

            // Initialize the conditions of finishing the episodes
            _atCircularQueue = new CircularQueue<int>(8);
            _artCircularQueue = new CircularQueue<int>(8);

            // Create the features and outputs data object
            Data CornersScanData = new Data(CWDNamigs.RLCornersScanData);

            // Segment the samples of signal according to the chunks' distribution
            _SignalSegmentsList = SegmentTheMainSamples(samples, (int)_samplingRate, 0.002d, 0.5d);
            int maxEpisodes = 15;
            for (_selectedSegment = 0; _selectedSegment < _SignalSegmentsList.Count; _selectedSegment++)
            {
                // Get the Q tables of the current chunk
                (Dictionary<string, (double reward, bool badState)> generalQTable, List<(double episodeReward, Dictionary<string, (double reward, bool badState)> episodeQTable)> episodesQTables) = _Env.Train(maxEpisodes);
                // Get the best state of the environment
                List<(double reward, string state)> acceptableStates = generalQTable.Where(state => state.Value.badState == false).Select(state => (state.Value.reward, state.Key)).ToList();
                // Check if there was no best state
                if (acceptableStates.Count == 0)
                    // Then just take the whole Q table
                    acceptableStates = generalQTable.Select(state => (state.Value.reward, state.Key)).ToList();
                // Take the state with the highest reward
                int[] bestState = ReinforcementLearning.Environment.String2IntArray(acceptableStates.Max().state);

                // Include the best state for the corners scan as outputs
                Sample chunkSample = GetFeaturesOfTheSamples(CornersScanData);

                ReinforcementLearning.Environment.Dimension atDim = _Env._DimensionsList.Where(dim => dim._Name.Equals("at")).ToList()[0];
                ReinforcementLearning.Environment.Dimension artDim = _Env._DimensionsList.Where(dim => dim._Name.Equals("art")).ToList()[0];

                chunkSample.insertOutput(0, CWDNamigs.AT, (bestState[0] * atDim._step) / 360d); // Normalize the value to be from 0 to 1 by dividing with 360 (the max angle)
                chunkSample.insertOutput(1, CWDNamigs.ART, (bestState[1] * artDim._step)); // art is already between 0 and 1
            }

            return CornersScanData;
        }

        public static List<SignalSegment> SegmentTheMainSamples3(double[] globalSamples, int samplingRate, double derivativeDelta, double distributionBarThreshold)
        {
            List<SignalSegment> signalSegmentsList = new List<SignalSegment>();
            // The segment should be at least 0.05 seconds long
            int segmentInitialLen = (int)(0.05d * samplingRate) + ((0.05d * samplingRate) % 1 > 0 ? 1 : 0);
            // The segment could extend to the next and previous segments up to 0.1 seconds
            int segmentExtension = (int)(0.1d * samplingRate) + ((0.1d * samplingRate) % 1 > 0 ? 1 : 0);

            int bufferEndingIndexBeforeExtension;
            double[] derivative = GeneralTools.Derivative(globalSamples, samplingRate, derivativeDelta);
            List<double> derivSegmentBuffer;

            double[] distribution = null;
            double distXOffset = 0;
            double distStep = 1;

            bool segmentExceededLimit;
            double dwtMin, dwtMax;
            int dwtMaxPrefExt;
            int dwtMaxSuffExt;
            int dwtPrefExtension;
            int dwtSuffExtension;

            for (int iDWTGlobal = 0; iDWTGlobal < derivative.Length; iDWTGlobal++)
            {
                SignalSegment signalSegment = new SignalSegment();
                dwtPrefExtension = 0;
                dwtSuffExtension = 0;
                derivSegmentBuffer = new List<double>(samplingRate + 2 * segmentExtension);
                // The segment should be up to 1 second
                for (int iDWTSegment = 0; iDWTSegment < samplingRate && iDWTGlobal + iDWTSegment < derivative.Length; iDWTSegment++)
                {
                    derivSegmentBuffer.Add(derivative[iDWTGlobal + iDWTSegment]);
                    // The segment should be at least of length dwtSegmentInitialLen
                    if (derivSegmentBuffer.Count < segmentInitialLen)
                        continue;
                    // Compute the distribution of the derivative
                    (distribution, distXOffset, distStep) = DistributionDisplay.CoputeDistribution(derivSegmentBuffer.ToArray(), 10);
                    // Check if the distribution of the derivative is not equiprobable
                    segmentExceededLimit = false;
                    foreach (double bar in distribution)
                        if (bar >= distributionBarThreshold)
                        {
                            segmentExceededLimit = true;
                            break;
                        }
                    if (segmentExceededLimit)
                        break;
                }
                bufferEndingIndexBeforeExtension = iDWTGlobal + (derivSegmentBuffer.Count > 0 ? derivSegmentBuffer.Count - 1 : 0);

                // Extend the segment
                // The extension should be in the limits of the derivative's interval
                (_, dwtMin, dwtMax) = GeneralTools.MeanMinMax(derivSegmentBuffer.ToArray());
                dwtMaxPrefExt = iDWTGlobal - segmentExtension >= 0 ? segmentExtension : iDWTGlobal;
                dwtMaxSuffExt = bufferEndingIndexBeforeExtension + segmentExtension < derivative.Length ? segmentExtension : (derivative.Length - bufferEndingIndexBeforeExtension) - 1;

                for (int i = 1; i <= dwtMaxPrefExt; i++)
                {
                    int barIndex = (int)((derivative[iDWTGlobal - i] - distXOffset) / distStep);
                    if (barIndex == distribution.Length)
                        barIndex--;
                    else if (barIndex > distribution.Length || barIndex < 0)
                        break;
                    //if (dwtMin <= derivative[iDWTGlobal - i] && derivative[iDWTGlobal - i] <= dwtMax)
                    if (distribution[barIndex] >= 0.1d)
                        dwtPrefExtension = i;
                    else
                        break;
                }
                for (int i = 1; i <= dwtMaxSuffExt; i++)
                {
                    int barIndex = (int)((derivative[bufferEndingIndexBeforeExtension + i] - distXOffset) / distStep);
                    if (barIndex == distribution.Length)
                        barIndex--;
                    else if (barIndex > distribution.Length || barIndex < 0)
                        break;
                    //if (dwtMin <= derivative[bufferEndingIndexBeforeExtension + i] && derivative[bufferEndingIndexBeforeExtension + i] <= dwtMax)
                    if (distribution[barIndex] >= 0.1d)
                        dwtSuffExtension = i;
                    else
                        break;
                }

                // Include the new segment in signalSegmentsList
                SignalSegment newSegment = new SignalSegment() { startingIndex = iDWTGlobal - dwtPrefExtension, endingIndex = bufferEndingIndexBeforeExtension + dwtSuffExtension };
                newSegment.SegmentSamples = globalSamples.Where((val, index) => newSegment.startingIndex <= index && index <= newSegment.endingIndex).ToArray();

                signalSegmentsList.Add(newSegment);

                // Move iGlobal according to the new segment
                iDWTGlobal = bufferEndingIndexBeforeExtension;
            }

            return signalSegmentsList;
        }

        public static List<SignalSegment> SegmentTheMainSamples(double[] globalSamples, int samplingRate, double derivativeDelta, double distributionBarThreshold)
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
                bufferEndingIndexBeforeExtension = iDWTGlobal + (dwtSegmentBuffer.Count > 0 ? dwtSegmentBuffer.Count - 1 : 0);

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
                SignalSegment newSegment = new SignalSegment() { startingIndex = (iDWTGlobal - dwtPrefExtension) * dwtDownScale, endingIndex = (bufferEndingIndexBeforeExtension + dwtSuffExtension) * dwtDownScale };
                newSegment.SegmentSamples = globalSamples.Where((val, index) => newSegment.startingIndex <= index && index <= newSegment.endingIndex).ToArray();

                signalSegmentsList.Add(newSegment);

                // Move iGlobal according to the new segment
                iDWTGlobal = bufferEndingIndexBeforeExtension;
            }

            return signalSegmentsList;
        }

        public static List<SignalSegment> SegmentTheMainSamples0(double[] globalSamples, int samplingRate, double derivativeDelta, double distributionBarThreshold)
        {
            List<SignalSegment> signalSegmentsList = new List<SignalSegment>();
            // The segment should be at least 0.05 seconds long
            int segmentInitialLen = (int)(0.05d * samplingRate) + ((0.05d * samplingRate) % 1 > 0 ? 1 : 0);
            // The segment could extend to the next and previous segments up to 0.1 seconds
            int segmentExtension = (int)(0.1d * samplingRate) + ((0.1d * samplingRate) % 1 > 0 ? 1 : 0);

            List<double> segmentBuffer;
            int bufferEndingIndexBeforeExtension;
            double[] derivative = null;
            double[] distribution;

            bool segmentExceededLimit;
            double derMin, derMax;
            double[] derExtended;
            int derPrefExt;
            int derSuffExt;
            int prefExtension;
            int suffExtension;

            for (int iGlobal = 0; iGlobal < globalSamples.Length; iGlobal++)
            {
                SignalSegment signalSegment = new SignalSegment();
                prefExtension = 0;
                suffExtension = 0;
                segmentBuffer = new List<double>(samplingRate + 2 * segmentExtension);
                for (int iSegment = 0; iSegment < samplingRate && iGlobal + iSegment < globalSamples.Length; iSegment++)
                {
                    segmentBuffer.Add(globalSamples[iGlobal + iSegment]);
                    // Compute the derivative of the segment
                    derivative = GeneralTools.Derivative(segmentBuffer.ToArray(), samplingRate, derivativeDelta);
                    // The segment should be at least of length segmentInitialLen
                    if (segmentBuffer.Count < segmentInitialLen)
                        continue;
                    // Compute the distribution of the derivative
                    distribution = DistributionDisplay.CoputeDistribution(derivative, 10).distribution;
                    // Check if the distribution of the derivative is not equiprobable
                    segmentExceededLimit = false;
                    foreach (double bar in distribution)
                        if (bar >= distributionBarThreshold)
                        {
                            segmentExceededLimit = true;
                            break;
                        }
                    if (segmentExceededLimit)
                        break;
                }
                bufferEndingIndexBeforeExtension = iGlobal + (segmentBuffer.Count > 0 ? segmentBuffer.Count - 1 : 0);

                // Extend the segment
                // The extension should be in the limits of the derivative's interval
                (_, derMin, derMax) = GeneralTools.MeanMinMax(derivative);
                derPrefExt = iGlobal - segmentExtension >= 0 ? segmentExtension : iGlobal;
                derSuffExt = bufferEndingIndexBeforeExtension + segmentExtension < globalSamples.Length ? segmentExtension : (globalSamples.Length - bufferEndingIndexBeforeExtension) - 1;
                derExtended = GeneralTools.Derivative(globalSamples.Where((sample, index) => iGlobal - derPrefExt <= index && index <= bufferEndingIndexBeforeExtension + derSuffExt).ToArray(),
                                                      samplingRate,
                                                      derivativeDelta);
                for (int i = 1; i <= derPrefExt; i++)
                {
                    if (derMin <= derExtended[derPrefExt - i] && derExtended[derPrefExt - i] <= derMax)
                    {
                        segmentBuffer.Insert(0, globalSamples[iGlobal - i]);
                        prefExtension = i;
                    }
                    else
                        break;
                }
                for (int i = 1; i <= derSuffExt; i++)
                {
                    if (derMin <= derExtended[(derExtended.Length - 1) - derSuffExt + i] && derExtended[(derExtended.Length - 1) - derSuffExt + i] <= derMax)
                    {
                        segmentBuffer.Add(globalSamples[bufferEndingIndexBeforeExtension + i]);
                        suffExtension = i;
                    }
                    else
                        break;
                }

                // Include the new segment in signalSegmentsList
                SignalSegment newSegment = new SignalSegment() { startingIndex = iGlobal - prefExtension, endingIndex = bufferEndingIndexBeforeExtension + suffExtension };
                newSegment.SegmentSamples = segmentBuffer.ToArray();

                signalSegmentsList.Add(newSegment);

                // Move iGlobal according to the new segment
                iGlobal = bufferEndingIndexBeforeExtension;
            }

            return signalSegmentsList;
        }

        public static (double globalMean, double globalStdDev, double globalIQR, double segmentMean,
                       double segmentMin, double segmentMax, double segmentStdDev, double segmentIQR) GetFeaturesValues(double[] mainSamples, SignalSegment segment)
        {
            // Normalize samples
            double[] normSamples = GeneralTools.normalizeSignal(mainSamples);
            double[] normSegmentSamples = normSamples.Where((val, index) => segment.startingIndex <= index && index <= segment.endingIndex).ToArray();

            double globalMean = GeneralTools.MeanMinMax(normSamples).mean;
            double globalStdDev = GeneralTools.stdDevCalc(normSamples, globalMean);
            double globalIQR = GeneralTools.signalIQR(normSamples);
            (double segmentMean, double segmentMin, double segmentMax) = GeneralTools.MeanMinMax(normSegmentSamples);
            double segmentStdDev = GeneralTools.stdDevCalc(normSegmentSamples, segmentMean);
            double segmentIQR = GeneralTools.signalIQR(normSegmentSamples);

            return (globalMean, globalStdDev, globalIQR, segmentMean,
                    segmentMin, segmentMax, segmentStdDev, segmentIQR);
        }

        private Sample GetFeaturesOfTheSamples(Data dataParent)
        {
            Sample sample = new Sample("segment" + _selectedSegment, 8, 2, dataParent);

            (double globalMean, double globalStdDev, double globalIQR, double segmentMean,
             double segmentMin, double segmentMax, double segmentStdDev, double segmentIQR) = GetFeaturesValues(_RescaledSamples, _SignalSegmentsList[_selectedSegment]);

            sample.insertFeature(0, CWDNamigs.GlobalMean, globalMean);
            sample.insertFeature(1, CWDNamigs.GlobalStdDev, globalStdDev);
            sample.insertFeature(2, CWDNamigs.GlobalIQR, globalIQR);
            sample.insertFeature(3, CWDNamigs.SegmentMean, segmentMean);
            sample.insertFeature(4, CWDNamigs.SegmentMin, segmentMin);
            sample.insertFeature(5, CWDNamigs.SegmentMax, segmentMax);
            sample.insertFeature(6, CWDNamigs.SegmentStdDev, segmentStdDev);
            sample.insertFeature(7, CWDNamigs.SegmentIQR, segmentIQR);

            return sample;
        }
    }
}

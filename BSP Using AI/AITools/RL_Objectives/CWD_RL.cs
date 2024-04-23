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
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.DetailsModify.FormDetailsModify.CornersScanner;
using static Tensorflow.Binding;

namespace Biological_Signal_Processing_Using_AI.AITools.RL_Objectives
{
    public class CWD_RL
    {
        ReinforcementLearning.Environment _Env;

        List<Interval> _ApproxIntervList;

        double[] _Samples;
        double _samplingRate;
        int[] _TrueCornersIndex;

        double penalty_movement = 0.5f; // steps

        double penalty_false_negative = 2; // not detecting the thing
        double penalty_false_positive = 1; // detecting other thing

        double reward_true_positive = 1;

        CircularQueue<int> _atCircularQueue;
        CircularQueue<int> _artCircularQueue;

        int _signalChunks;
        int _chunkStep;
        int _chunk;

        public CWD_RL()
        {
            // Set the environment
            List<ReinforcementLearning.Environment.Dimension> dimensionsList = new List<ReinforcementLearning.Environment.Dimension>(2);
            dimensionsList.Add(new ReinforcementLearning.Environment.Dimension(name: "at", size: 60, min: 1, max: 10));
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
            double[] windowSamples = _Samples.Where((val, index) => _chunk * _chunkStep <= index && index < (_chunk * _chunkStep) + _samplingRate).ToArray();

            // Scan corners of the selected window
            ReinforcementLearning.Environment.Dimension atDim = env._DimensionsList.Where(dim => dim._Name.Equals("at")).ToList()[0];
            ReinforcementLearning.Environment.Dimension artDim = env._DimensionsList.Where(dim => dim._Name.Equals("art")).ToList()[0];
            List<CornerSample> tempCornersList = ScanCorners(windowSamples, _chunk * _chunkStep, _samplingRate, state[1] * artDim._step, state[0] * atDim._step);
            // Take only the indexes of the corners
            int[] tempCornersIndex = tempCornersList.Select(corner => corner._index).ToArray();
            // Create a list of the intervals holding the corners in tempCornersIndex
            List<Interval> tempIntervalsList = new List<Interval>();

            // Compute the reward of current step
            // Get the true corners of only the selected window
            //int[] windowTrueCornersIndex = _TrueCornersIndex.Where(cornerIndex => _chunk * _chunkStep <= cornerIndex && cornerIndex < (_chunk * _chunkStep) + _samplingRate).ToArray();
            // Get the intervals of the true corners of the selected window only
            Interval[] windowapproxIntervals = _ApproxIntervList.Where(interval => _chunk * _chunkStep <= interval.starting && interval.ending < (_chunk * _chunkStep) + _samplingRate).ToArray();

            // Compute reward value of the current step
            //reward -= penalty_movement;
            foreach (int cornerIndex in tempCornersIndex)
            {
                // Check if the scanned corner is included in true corner interval
                List<Interval> chosenInterval = windowapproxIntervals.Where(interval => interval.starting <= cornerIndex && cornerIndex <= interval.ending).ToList();
                if (chosenInterval.Count() > 0)
                {
                    // If yes then check if this interval has not included a previous corner
                    if (!tempIntervalsList.Contains(chosenInterval[0]))
                    {
                        //if (windowTrueCornersIndex.Contains(cornerIndex))
                        reward += reward_true_positive;
                        tempIntervalsList.Add(chosenInterval[0]);
                    }
                }
                else
                    reward -= penalty_false_positive;
            }
            // Compute the reward of the false negatives
            foreach (Interval interval in windowapproxIntervals)
                if (tempCornersIndex.Where(cornerIndex => interval.starting <= cornerIndex && cornerIndex <= interval.ending).Count() == 0)
                //foreach (int cornerIndex in windowTrueCornersIndex)
                //    if (!tempCornersIndex.Contains(cornerIndex))
                {
                    reward -= penalty_false_negative;
                    badState = true;
                }

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

        public Data FitRLData(double[] samples, int samplingRate, int[] trueCornersIndex)
        {
            // Include the signal infos
            _Samples = samples;
            _samplingRate = samplingRate;
            _TrueCornersIndex = trueCornersIndex;

            // Create the intervals covering 20% from the corners in both sides
            _ApproxIntervList = ApproximateIndexesToIntervals(trueCornersIndex, 20, samples);

            // Initialize the conditions of finishing the episodes
            _atCircularQueue = new CircularQueue<int>(8);
            _artCircularQueue = new CircularQueue<int>(8);

            // Create the features and outputs data object
            Data CornersScanData = new Data(CWDNamigs.RLCornersScanData);

            // Truncate the samples of signal to truncations of 1 second of length, and half a second of step
            _signalChunks = ((int)(samples.Length / samplingRate) + (int)(samples.Length % samplingRate) % 2) * 2;
            _chunkStep = (int)samplingRate / 2;
            int maxEpisodes = 15;
            for (_chunk = 0; _chunk < _signalChunks; _chunk++)
            {
                // Get the Q tables of the current chunk
                (Dictionary<string, (double reward, bool badState)> generalQTable, List<(double episodeReward, Dictionary<string, (double reward, bool badState)> episodeQTable)> episodesQTables) = _Env.Train(maxEpisodes);
                // Get the best state of the environment
                List<(double reward, string state)> acceptableStates = generalQTable.Where(state => state.Value.badState == false).Select(state => (state.Value.reward, state.Key)).ToList();
                // Check if there was no best state
                if (acceptableStates.Count == 0)
                    // Then just take the whole Q table
                    generalQTable.Select(state => (state.Value.reward, state.Key)).ToList();
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

        private Sample GetFeaturesOfTheSamples(Data dataParent)
        {
            // Normalize samples
            double[] normSamples = GeneralTools.normalizeSignal(_Samples);
            double[] normChunkSamples = normSamples.Where((val, index) => _chunk * _chunkStep <= index && index < (_chunk * _chunkStep) + _samplingRate).ToArray();

            Sample sample = new Sample("chunk" + _chunk, 8, 2, dataParent);

            double majorMean = GeneralTools.MeanMinMax(normSamples).mean;
            double majorStdDev = GeneralTools.stdDevCalc(normSamples, majorMean);
            double majorIQR = GeneralTools.signalIQR(normSamples);
            (double chunkMean, double chunkMin, double chunkMax) = GeneralTools.MeanMinMax(normChunkSamples);
            double chunkStdDev = GeneralTools.stdDevCalc(normChunkSamples, chunkMean);
            double chunkIQR = GeneralTools.signalIQR(normChunkSamples);

            sample.insertFeature(0, CWDNamigs.MajorMean, majorMean);
            sample.insertFeature(1, CWDNamigs.MajorStdDev, majorStdDev);
            sample.insertFeature(2, CWDNamigs.MajorIQR, majorIQR);
            sample.insertFeature(3, CWDNamigs.ChunkMean, chunkMean);
            sample.insertFeature(4, CWDNamigs.ChunkMin, chunkMin);
            sample.insertFeature(5, CWDNamigs.ChunkMax, chunkMax);
            sample.insertFeature(6, CWDNamigs.ChunkStdDev, chunkStdDev);
            sample.insertFeature(7, CWDNamigs.ChunkIQR, chunkIQR);

            return sample;
        }
    }
}

using Biological_Signal_Processing_Using_AI.Garage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.DetailsModify.FormDetailsModify.CornersScanner;

namespace Biological_Signal_Processing_Using_AI.AITools
{
    public class ReinforcementLearning
    {
        public class Agent
        {
            private Environment _Environment;

            private int[] _State;

            public Agent(Environment environment)
            {
                _Environment = environment;
                _State = new int[environment._DimensionsList.Count];
            }

            public int[] GetState()
            {
                return (int[])_State.Clone();
            }

            public virtual (int dimensionIndex, int movement) GetMovementDetails(int choice)
            {
                // Each dimension should have two actions of movements
                int dimensionIndex = choice / 2;
                int movement = choice % 2 == 0 ? +1 : -1;
                return (dimensionIndex, movement);
            }
            public virtual int[] Observe(int choice)
            {
                // Return the the expected state of the chosen action
                (int dimensionIndex, int movement) = GetMovementDetails(choice);

                return Move(dimensionIndex, movement);
            }
            public virtual void Action(int choice)
            {
                // Change the state of the agnet according to the chosen action
                (int dimensionIndex, int movement) = GetMovementDetails(choice);

                _State = Move(dimensionIndex, movement);
            }

            private int[] Move(int dimensionIndex, int movement)
            {
                int[] newState = (int[])_State.Clone();
                if (dimensionIndex >= _State.Length)
                    return null;

                newState[dimensionIndex] += movement;
                // The new state should be in the right borders
                if (newState[dimensionIndex] < 0)
                    newState[dimensionIndex] = 0;
                else if (newState[dimensionIndex] >= _Environment._DimensionsList[dimensionIndex]._size)
                    newState[dimensionIndex] = _Environment._DimensionsList[dimensionIndex]._size - 1;

                return newState;
            }
        }

        public class Environment
        {
            public class Dimension
            {
                public string _Name;
                public int _size;
                public double _min;
                public double _max;
                public double _step;

                public Dimension(string name, int size, double min, double max)
                {
                    _Name = name;
                    _size = size;
                    _min = min;
                    _max = max;
                    _step = (max - min) / size;
                }
            }

            public List<Dimension> _DimensionsList;

            private Dictionary<string, (double reward, bool badState)> _QTableDict = new Dictionary<string, (double, bool)>();

            private Agent _Agent;

            double _learningRate = 0.1d;
            double _discount = 0.95d;

            public delegate (double reward, bool badState) ComputeReward(int[] state, ReinforcementLearning.Environment env);
            public delegate bool CheckIfDone(int[] state);

            private ComputeReward _ComputeRewardDelegate;
            private CheckIfDone _CheckIfDoneDelegate;

            private double _randomnessEpsi = 0.95d;
            private double _EpsiDecay = 0.2d;
            private double _EpsiMin = 0.001d;

            public Environment(List<Dimension> dimensionsList, double learningRate, double discount, ComputeReward computeRewardDelegate, CheckIfDone checkIfDoneDelegate)
            {
                _DimensionsList = dimensionsList;
                _learningRate = learningRate;
                _discount = discount;
                _ComputeRewardDelegate = computeRewardDelegate;
                _CheckIfDoneDelegate = checkIfDoneDelegate;
            }

            public virtual void Reset()
            {
                // Just recreate the agents, which will reset their states
                /*for (int i = 0; i < _AgentsList.Count; i++)
                    _AgentsList[i] = new Agent(this);*/
                _Agent = new Agent(this);
                // You can try setting the starting states of the agents randomly
            }

            /// <summary>
            /// Take a step without actually chaging the state of the agent
            /// </summary>
            /// <param name="agent"></param>
            /// <param name="action"></param>
            /// <returns></returns>
            public virtual (int[] newState, double reward, bool badState) Step(Agent agent, int action)
            {
                // Try moving the agent with the corresponding action
                int[] newState = agent.Observe(action);
                (double reward, bool badState) = _ComputeRewardDelegate(newState, this);

                return (newState, reward, badState);
            }

            /// <summary>
            /// Append the chosen action to the given state
            /// </summary>
            /// <param name="state"></param>
            /// <param name="action"></param>
            /// <returns></returns>
            private int[] SetActionToState(int[] state, int action)
            {
                int[] stateWithAction = new int[state.Length + 1];
                for (int i = 0; i < state.Length; i++)
                    stateWithAction[i] = state[i];
                stateWithAction[state.Length] = action;
                return stateWithAction;
            }

            /// <summary>
            /// Convert the content of the array of interegs to a string
            /// </summary>
            /// <param name="array"></param>
            /// <returns></returns>
            private static string IntArray2String(int[] array)
            {
                string result = "";

                foreach (int i in array)
                    result += i.ToString() + ", ";

                return result;
            }
            public static int[] String2IntArray(string stringArray)
            {
                string[] intsArray = stringArray.Split(", ");
                int[] result = intsArray.Where(intString => intString.Length > 0).Select(intString => int.Parse(intString)).ToArray();

                return result;
            }

            public virtual (Dictionary<string, (double reward, bool badState)> generalQTable, List<(double episodeReward, Dictionary<string, (double reward, bool badState)> episodeQTable)> episodesQTables) Train(int maxEpisodes)
            {
                List<(double, Dictionary<string, (double reward, bool badState)>)> episodesQTables = new List<(double, Dictionary<string, (double, bool)>)>(maxEpisodes);
                _QTableDict = new Dictionary<string, (double, bool)>();
                Random rd = new Random();

                for (int epoisode = 0; epoisode < maxEpisodes; epoisode++)
                {
                    // Reset the new episode's stuff
                    _randomnessEpsi = 1;
                    Dictionary<string, (double reward, bool badState)> episodeQTable = new Dictionary<string, (double, bool)>();
                    Reset();
                    int[] currentStateWithAction = null;
                    bool done = false;
                    double episodeReward = 0;

                    while (!done)
                    {
                        int newAction = 0;
                        int[] newState = null;
                        int[] newStateWithAction = null;
                        double reward = double.NegativeInfinity;
                        bool badState = true;
                        // Check if the epsilon of chosing a random action is less than a random value
                        if (rd.NextDouble() > _randomnessEpsi)
                        {
                            // Take the action with the highest reward value
                            for (int action = 0; action < _DimensionsList.Count * 2; action++)
                            {
                                currentStateWithAction = SetActionToState(_Agent.GetState(), action);
                                if (_QTableDict.ContainsKey(IntArray2String(currentStateWithAction)))
                                {
                                    if (_QTableDict[IntArray2String(currentStateWithAction)].reward > reward)
                                    {
                                        newAction = action;
                                        (newState, reward, badState) = Step(_Agent, action);
                                    }
                                }
                                else if (-rd.NextDouble() > reward)
                                {
                                    newAction = action;
                                    (newState, reward, badState) = Step(_Agent, action);
                                }
                            }
                        }
                        else
                        {
                            // Take a random action
                            newAction = rd.Next(_DimensionsList.Count * 2);
                            (newState, reward, badState) = Step(_Agent, newAction);
                            _randomnessEpsi -= _EpsiDecay;
                        }

                        // Set the new reward in qTable by computing the new Q value
                        double maxFutureQ = double.NegativeInfinity;
                        for (int action = 0; action < _DimensionsList.Count * 2; action++)
                        {
                            newStateWithAction = SetActionToState(newState, action);
                            if (_QTableDict.ContainsKey(IntArray2String(newStateWithAction)))
                            {
                                if (_QTableDict[IntArray2String(newStateWithAction)].reward > maxFutureQ)
                                    maxFutureQ = _QTableDict[IntArray2String(newStateWithAction)].reward;
                            }
                            else if (-rd.NextDouble() > maxFutureQ)
                                maxFutureQ = -rd.NextDouble();
                        }

                        double currentQ = -rd.NextDouble();
                        currentStateWithAction = SetActionToState(_Agent.GetState(), newAction);
                        if (_QTableDict.ContainsKey(IntArray2String(currentStateWithAction)))
                            currentQ = _QTableDict[IntArray2String(currentStateWithAction)].reward;
                        double newQ = (1 - _learningRate) * currentQ + _learningRate * (reward + _discount * maxFutureQ);

                        // Update the _QTableDict and episodeQTable with the new Q value
                        if (_QTableDict.ContainsKey(IntArray2String(currentStateWithAction)))
                            _QTableDict[IntArray2String(currentStateWithAction)] = (newQ, badState);
                        else
                            _QTableDict.Add(IntArray2String(currentStateWithAction), (newQ, badState));

                        if (episodeQTable.ContainsKey(IntArray2String(currentStateWithAction)))
                            episodeQTable[IntArray2String(currentStateWithAction)] = (newQ, badState);
                        else
                            episodeQTable.Add(IntArray2String(currentStateWithAction), (newQ, badState));

                        // Move the agent with the new action
                        _Agent.Action(newAction);

                        episodeReward += reward;

                        // Check if this episode is done
                        done = _CheckIfDoneDelegate(newState);

                        /*if (_randomnessEpsi > _EpsiMin)
                        {
                            _randomnessEpsi -= _EpsiDecay;
                            _randomnessEpsi = Math.Max(_EpsiMin, _randomnessEpsi);
                        }*/
                    }
                    episodesQTables.Add((episodeReward, episodeQTable));
                }
                return (_QTableDict, episodesQTables);
            }
            /*public virtual (Dictionary<string, double> generalQTable, List<(double, Dictionary<string, double>)> episodesQTables) Train(int maxEpisodes)
            {
                List<(double, Dictionary<string, double>)> episodesQTables = new List<(double, Dictionary<string, double>)>(maxEpisodes);
                _QTableDict = new Dictionary<string, double>();
                Random rd = new Random();

                for (int epoisode = 0; epoisode < maxEpisodes; epoisode++)
                {
                    // Reset the new episode's stuff
                    _randomnessEpsi = 1;
                    Dictionary<string, double> episodeQTable = new Dictionary<string, double>();
                    Reset();
                    int[] currentStateWithAction = null;
                    bool done = false;
                    double episodeReward = 0;

                    while (!done)
                    {
                        int newAction = 0;
                        int[] newState = null;
                        int[] newStateWithAction = null;
                        double reward = double.NegativeInfinity;
                        // Check if the epsilon of chosing a random action is less than a random value
                        if (rd.NextDouble() > _randomnessEpsi)
                        {
                            // Take the action with the highest reward value
                            for (int action = 0; action < _DimensionsList.Count * 2; action++)
                            {
                                (int[] iNewState, double iReward) = Step(_Agent, action);
                                if (iReward >= reward)
                                {
                                    newAction = action;
                                    newState = iNewState;
                                    reward = iReward;
                                }
                            }
                        }
                        else
                        {
                            // Take a random action
                            newAction = rd.Next(_DimensionsList.Count * 2);
                            (newState, reward) = Step(_Agent, newAction);
                            _randomnessEpsi -= _EpsiDecay;
                        }

                        // Set the new reward in qTable by computing the new Q value
                        double maxFutureQ = double.NegativeInfinity;
                        for (int action = 0; action < _DimensionsList.Count * 2; action++)
                        {
                            newStateWithAction = SetActionToState(newState, action);
                            if (_QTableDict.ContainsKey(IntArray2String(newStateWithAction)))
                            {
                                if (_QTableDict[IntArray2String(newStateWithAction)] > maxFutureQ)
                                    maxFutureQ = _QTableDict[IntArray2String(newStateWithAction)];
                            }
                            else if (-rd.NextDouble() > maxFutureQ)
                                maxFutureQ = -rd.NextDouble();
                        }
                            
                        double currentQ = -rd.NextDouble();
                        currentStateWithAction = SetActionToState(_Agent.GetState(), newAction);
                        if (_QTableDict.ContainsKey(IntArray2String(currentStateWithAction)))
                            currentQ = _QTableDict[IntArray2String(currentStateWithAction)];
                        double newQ = (1 - _learningRate) * currentQ + _learningRate * (reward + _discount * maxFutureQ);

                        // Update the _QTableDict and episodeQTable with the new Q value
                        if (_QTableDict.ContainsKey(IntArray2String(currentStateWithAction)))
                            _QTableDict[IntArray2String(currentStateWithAction)] = newQ;
                        else
                            _QTableDict.Add(IntArray2String(currentStateWithAction), newQ);

                        if (episodeQTable.ContainsKey(IntArray2String(currentStateWithAction)))
                            episodeQTable[IntArray2String(currentStateWithAction)] = newQ;
                        else
                            episodeQTable.Add(IntArray2String(currentStateWithAction), newQ);

                        // Move the agent with the new action
                        _Agent.Action(newAction);

                        episodeReward += reward;

                        // Check if this episode is done
                        done = _CheckIfDoneDelegate(newState);
                    }
                    episodesQTables.Add((episodeReward, episodeQTable));
                }
                return (_QTableDict, episodesQTables);
            }*/
        }
















        double[] _Samples;
        double _samplingRate;
        int[] _TrueCornersIndex;

        int atd = 60; // angle threshold choices
        int artd = 60; // amplitude ratio threshold choices

        int wsMax;
        int wsMin;
        int wsStep;

        int wpMax;
        int wpMin;
        int wpStep;

        double atMax;
        double atMin;
        double atStep;

        double artMax;
        double artMin;
        double artStep;

        List<int> _AgentCornersIndex;
        Dictionary<int, int> _AgentCornersIndexDict;
        List<(float, int[])> _AgentEpisodeCornersIndex;

        float penalty_movement = 0.5f; // steps
        float penalty_window_increment = 0.6f; // steps
        float penalty_window_decrement = 1; // steps
        float penalty_window_forward = 0.8f; // steps
        float penalty_window_backward = 1; // steps

        float penalty_false_negative = 50; // not detecting the thing
        float penalty_false_positive = 3; // detecting other thing

        float reward_true_positive = 3;

        float _learningRate = 0.1f;
        float _discount = 0.95f;

        int _signalChunks;
        int _chunkStep;
        int _chunk;

        List<Interval> approxIntervList;

        CircularQueue<int> atCircularQueue;
        CircularQueue<int> artCircularQueue;

        class Interval
        {
            public int starting;
            public int ending;
        }

        class State
        {
            public int at = 0;
            public int art = 0;

            public State Clone()
            {
                State newState = new State();
                newState.at = at;
                newState.art = art;

                return newState;
            }
        }

        public ReinforcementLearning(double[] samples, double samplingRate, int[] cornersIndex)
        {
            _Samples = samples;
            _samplingRate = samplingRate;
            _TrueCornersIndex = cornersIndex;

            _AgentEpisodeCornersIndex = new List<(float, int[])>(100);
            _AgentCornersIndexDict = new Dictionary<int, int>();
        }

        public void Learn()
        {
            approxIntervList = ApproximateIndexesToIntervals(_TrueCornersIndex, 20);
            // Window's size from 0 to 2 seconds
            // Wondow's position always starts from the end of the previous window
            // Window's position maybe it should be the division of the signal by the shortest window size
            // Angle threshold from 0 to 360 (maybe frmo 0 to 50)
            // ART from 0 to 1

            atMax = 10;
            atMin = 1;
            atStep = (atMax - atMin) / atd;

            artMax = 0.3f;
            artMin = 0;
            artStep = (artMax - artMin) / artd;

            Random rd = new Random();

            float[,,] qTable = new float[atd, artd, 4];
            // Initialze the Q table
            for (int at = 0; at < atd; at++)
                for (int art = 0; art < artd; art++)
                    for (int action = 0; action < 4; action++)
                        qTable[at, art, action] = (float)-rd.NextDouble();

            _signalChunks = ((int)(_Samples.Length / _samplingRate) + (int)(_Samples.Length % _samplingRate) % 2) * 2;
            _chunkStep = (int)_samplingRate / 2;
            _AgentCornersIndex = new List<int>(_TrueCornersIndex.Length);
            for (_chunk = 0; _chunk < _signalChunks; _chunk++)
            {
                List<int> agentChunkCornersIndex = new List<int>(_TrueCornersIndex.Length);
                // Iterate through episodes
                for (int epoisode = 1; epoisode < 10; epoisode++)
                {
                    atCircularQueue = new CircularQueue<int>(5);
                    artCircularQueue = new CircularQueue<int>(5);
                    State currentState = new State();
                    bool done = false;
                    float episodeReward = 0;

                    while (!done)
                    {
                        float highestQ = float.NegativeInfinity;
                        int newAction = 0;
                        int[] cornersIndx = null;
                        State newState = null;
                        float reward = float.NegativeInfinity;
                        for (int action = 0; action < 4; action++)
                        {
                            (int[] iCornersIndx, State iNewState, float iReward, bool iDone) = Step(currentState, action);
                            done |= iDone;
                            if (iReward >= reward)
                            {
                                newAction = action;
                                cornersIndx = iCornersIndx;
                                newState = iNewState;
                                reward = iReward;
                                highestQ = qTable[currentState.at, currentState.art, action];
                            }
                        }

                        // Replace _AgentCorners with the new selected ones in the selected window
                        //_AgentCornersIndex = _AgentCornersIndex.Where(cornerIndex => _chunk * _chunkStep > cornerIndex || cornerIndex >= (_chunk * _chunkStep) + _samplingRate).ToList();
                        //_AgentCornersIndex.AddRange(cornersIndx);
                        agentChunkCornersIndex.Clear();
                        agentChunkCornersIndex.AddRange(cornersIndx);

                        // Set the new reward in qTable
                        float maxFutureQ = float.NegativeInfinity;
                        for (int action = 0; action < 4; action++)
                            if (qTable[newState.at, newState.art, action] > maxFutureQ)
                                maxFutureQ = qTable[newState.at, newState.art, action];
                        float currentQ = qTable[currentState.at, currentState.art, newAction];
                        float newQ = (1 - _learningRate) * currentQ + _learningRate * (reward + _discount * maxFutureQ);
                        qTable[currentState.at, currentState.art, newAction] = newQ;

                        // Set the new window size and position
                        currentState = newState.Clone();

                        episodeReward += reward;
                    }

                    //_AgentEpisodeCornersIndex.Add((episodeReward, _AgentCornersIndex.ToArray()));
                }
                _AgentEpisodeCornersIndex.Add((_chunk, agentChunkCornersIndex.ToArray()));
                foreach (int index in agentChunkCornersIndex)
                    _AgentCornersIndexDict.TryAdd(index, index);
                artMin = 0;
            }
            _AgentCornersIndex = _AgentCornersIndexDict.ToList().Select(item => item.Key).ToList();
            foreach (Interval interval in approxIntervList)
                if (_AgentCornersIndex.Where(cornerIndex => interval.starting <= cornerIndex && cornerIndex <= interval.ending).Count() == 0)
                    artMin = 0;
        }
        private (int[] cornersIndx, State newState, float reward, bool done) Step(State currentState, int action)
        {
            State newState = currentState.Clone();
            float reward = 0;
            bool done = false;
            // Move or change the size of the window if needed
            newState = Move(currentState, action);
            // Get selected window samples
            double[] windowSamples = _Samples.Where((val, index) => _chunk * _chunkStep <= index && index < (_chunk * _chunkStep) + _samplingRate).ToArray();

            // Scan corners
            List<CornerSample> tempCornersList = ScanCorners(windowSamples, _chunk * _chunkStep, _samplingRate, newState.art * artStep, newState.at * atStep);
            int[] tempCornersIndex = tempCornersList.Select(corner => corner._index).ToArray();

            // Compute the reward of current step
            // Get corners of only the selected window
            int[] windowTrueCornersIndex = _TrueCornersIndex.Where(cornerIndex => _chunk * _chunkStep <= cornerIndex && cornerIndex < (_chunk * _chunkStep) + _samplingRate).ToArray();
            Interval[] windowapproxIntervals = approxIntervList.Where(interval => _chunk * _chunkStep <= interval.starting && interval.ending < (_chunk * _chunkStep) + _samplingRate).ToArray();
            // Compute reward of the false positives and true positives
            reward -= penalty_movement;
            foreach (int cornerIndex in tempCornersIndex)
                //if (windowapproxIntervals.Where(interval => interval.starting <= cornerIndex && cornerIndex <= interval.ending).Count() > 0)
                if (windowTrueCornersIndex.Contains(cornerIndex))
                    reward += reward_true_positive;
                else
                    reward -= penalty_false_positive;
            // Compute the reward of the false negatives
            //foreach (Interval interval in windowapproxIntervals)
            //    if (tempCornersIndex.Where(cornerIndex => interval.starting <= cornerIndex && cornerIndex <= interval.ending).Count() == 0)
            foreach (int cornerIndex in windowTrueCornersIndex)
                if (!tempCornersIndex.Contains(cornerIndex))
                    reward -= penalty_false_negative;

            // ----Done when detecting all of the things and emprovement was almost stable----
            atCircularQueue.Enqueue(newState.at);
            artCircularQueue.Enqueue(newState.art);

            // Done when finding all true corners
            //if (reward >= -10)
            //if (currentState.art == artd && currentState.at == atd)
            if (GeneralTools.amplitudeInterval(atCircularQueue.ToArray().Select(at => (double)at).ToArray()) < 3 && atCircularQueue._count > 4 &&
                GeneralTools.amplitudeInterval(artCircularQueue.ToArray().Select(art => (double)art).ToArray()) < 3 && artCircularQueue._count > 4)
                done = true;

            return (tempCornersIndex, newState, reward, done);
        }

        private State Move(State currentState, int action)
        {
            State newState = currentState.Clone();

            if (action == 0)
                newState.at = (newState.at + 1) % atd;
            else if (action == 1)
                newState.at = Math.Abs(newState.at - 1) % atd;
            else if (action == 2)
                newState.art = (newState.art + 1) % artd;
            else if (action == 3)
                newState.art = Math.Abs(newState.art - 1) % artd;

            return newState;
        }

        private List<Interval> ApproximateIndexesToIntervals(int[] indexes, double tolerance)
        {
            List<Interval> intervals = new List<Interval>();

            for (int i = 0; i < indexes.Length; i++)
            {
                Interval indexInterval = new Interval();
                if (i - 1 >= 0)
                    indexInterval.starting = indexes[i] - (int)(tolerance * (indexes[i] - indexes[i - 1]) / 100f);
                else
                    indexInterval.starting = indexes[i] - (int)(tolerance * (indexes[i] - 0) / 100f);

                if (i + 1 < indexes.Length)
                    indexInterval.ending = indexes[i] + (int)(tolerance * (indexes[i + 1] - indexes[i]) / 100f);
                else
                    indexInterval.ending = indexes[i] + (int)(tolerance * ((_Samples.Length - 1) - indexes[i]) / 100f);

                intervals.Add(indexInterval);
            }


            return intervals;
        }
    }
}

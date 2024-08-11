using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                // Set a random state to the agent
                Random rd = new Random();
                for (int dimIndex = 0; dimIndex < environment._DimensionsList.Count; dimIndex++)
                    _State[dimIndex] = rd.Next(environment._DimensionsList[dimIndex]._size);
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
            
            public virtual (Dictionary<string, (double reward, bool badState)> generalQTable, List<(double episodeReward, Dictionary<string, (double reward, bool badState)> episodeQTable)> episodesQTables) TrainFast(int maxEpisodes)
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
                                (int[] iNewState, double iReward, badState) = Step(_Agent, action);
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
                    }
                    episodesQTables.Add((episodeReward, episodeQTable));
                }
                return (_QTableDict, episodesQTables);
            }
        }
    }
}

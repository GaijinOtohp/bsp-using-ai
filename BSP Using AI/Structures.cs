using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Biological_Signal_Processing_Using_AI
{
    public class Structures
    {
        public class State
        {
            public string Name { get; set; }
            public int _index { get; set; }
            public double _value { get; set; }
            public double _firstApearanceValue { get; set; }
            public double _tangentFromLastState { get; set; } = double.NegativeInfinity;
            public double _meanTangentFromLastState { get; set; } = double.NegativeInfinity;
            public double _deviantionAngle { get; set; }

            public State Clone()
            {
                State state = new State();
                state.Name = Name;
                state._index = _index;
                state._value = _value;
                state._firstApearanceValue = _firstApearanceValue;
                state._tangentFromLastState = _tangentFromLastState;
                state._meanTangentFromLastState = _meanTangentFromLastState;
                state._deviantionAngle = _deviantionAngle;
                return state;
            }
        }

        public class TempState
        {
            public string Name { get; set; }
            public int _edgeIndex { get; set; }
            public double _edgeValue { get; set; }
        }

        /// <summary>
        /// Signal analysis namings
        /// </summary>
        public class SANamings
        {
            public static class ScatterPlotsNames
            {
                public static string UpPeaks { get; } = "Up peaks";
                public static string DownPeaks { get; } = "Down peaks";
                public static string StableStates { get; } = "Stable";
                public static string Labels { get; } = "Labels";
                public static string SpanAnnotations { get; } = "Span annotations";
            }
            public static string Signal { get; } = "Signal";
            public static string AllPeaks { get; } = "All peaks";
            public static string Selection { get; } = "Selection";
            public static string ShiftSelection { get; } = "Shift selection";
            public static string PointHorizSpan { get; } = "Pointing horizontal span";
            public static string IntervalHorizSpan { get; } = "Interval horizontal span";

            public static string Up { get; } = "up";
            public static string Down { get; } = "down";
            public static string Stable { get; } = "stable";

            public static string P { get; } = "P";
            public static string Q { get; } = "Q";
            public static string R { get; } = "R";
            public static string S { get; } = "S";
            public static string T { get; } = "T";
            public static string Delta { get; } = "Delta";
            public static string WPW { get; } = "WPW";
        }
        //____________________________________________________________________________________//
        [Serializable]
        [DataContract(IsReference = true)]
        public class Beat
        {
            [DataMember]
            public int _startingIndex { get; set; } = int.MinValue;
            [DataMember]
            public int _pIndex { get; set; } = int.MinValue;
            [DataMember]
            public int _qIndex { get; set; } = int.MinValue;
            [DataMember]
            public int _slurredUpstrokeIndex { get; set; } = int.MinValue;
            [DataMember]
            public int _rIndex { get; set; } = int.MinValue;
            [DataMember]
            public int _sIndex { get; set; } = int.MinValue;
            [DataMember]
            public int _tIndex { get; set; } = int.MinValue;
            [DataMember]
            public int _endingIndex { get; set; } = int.MinValue;
            [DataMember]
            public bool _deltaDetected { get; set; } = false;
            [DataMember]
            public bool _wpwDetected { get; set; } = false;
        }

        [Serializable]
        [DataContract(IsReference = true)]
        public class Data
        {
            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public Dictionary<string, int> FeaturesLabelsIndx { get; set; } = new Dictionary<string, int>();
            [DataMember]
            public Dictionary<string, int> OutputsLabelsIndx { get; set; } = new Dictionary<string, int>();

            [DataMember]
            public List<Sample> Samples { get; set; } = new List<Sample>();

            public Data(string name) { Name = name; }

            public double getFeatureByLabel(int sampleIndex, string featureLabel)
            {
                return Samples[sampleIndex].getFeatures()[FeaturesLabelsIndx[featureLabel]];
            }

            public double getOutputByLabel(int sampleIndex, string featureLabel)
            {
                return Samples[sampleIndex].getOutputs()[OutputsLabelsIndx[featureLabel]];
            }

            public void removeLastSample() { Samples.RemoveAt(Samples.Count - 1); }

            public void Clear()
            {
                FeaturesLabelsIndx.Clear();
                OutputsLabelsIndx.Clear();
                Samples.Clear();
            }
        }
        [Serializable]
        [DataContract(IsReference = true)]
        public class Sample
        {
            [DataMember]
            public Data DataParent { get; set; }
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            private double[] Features { get; set; }
            [DataMember]
            private double[] Outputs { get; set; }
            [DataMember]
            public Dictionary<object, object> AdditionalInfo { get; set; }

            public Sample(string name, int numberOfFeatures, int numberOfOutputs, Data dataParent = null)
            {
                Name = name;
                Features = new double[numberOfFeatures];
                Outputs = new double[numberOfOutputs];
                DataParent = dataParent;
                DataParent?.Samples.Add(this);
            }

            public void insertFeature(int index, string label, double value)
            {
                Features[index] = value;
                // Check if dataParent does not have the index of current label
                if (!DataParent.FeaturesLabelsIndx.ContainsKey(label))
                    DataParent.FeaturesLabelsIndx.Add(label, index);
            }

            public void insertOutput(int index, string label, double value)
            {
                Outputs[index] = value;
                // Check if dataParent does not have the index of current label
                if (!DataParent.OutputsLabelsIndx.ContainsKey(label))
                    DataParent.OutputsLabelsIndx.Add(label, index);
            }

            public void insertOutputArray(string[] labels, double[] values)
            {
                Outputs = values;
                // Check if dataParent does not have the index of current label
                for (int i = 0; i < labels.Length; i++)
                    if (!DataParent.OutputsLabelsIndx.ContainsKey(labels[i]))
                        DataParent.OutputsLabelsIndx.Add(labels[i], i);
            }

            public void insertFeaturesArray(double[] values) { Features = values; }

            public void UpdateFeature(int index, double value)
            {
                Features[index] = value;
            }

            public void UpdateOutput(int index, double value)
            {
                Outputs[index] = value;
            }

            public double[] getFeatures() { return Features; }

            public double[] getOutputs() { return Outputs; }

            public double getFeatureByLabel(string featureLabel)
            {
                if (featureLabel == null) return 0;
                return Features[DataParent.FeaturesLabelsIndx[featureLabel]];
            }

            public double getOutputByLabel(string featureLabel)
            {
                if (featureLabel == null) return 0;
                return Outputs[DataParent.OutputsLabelsIndx[featureLabel]];
            }
        }
        //____________________________________________________________________________________//

        public class StatParam
        {
            public string Name { get; set; }
            public double _value { get; set; }
        }
        //____________________________________________________________________________________//

        public class CircularQueue<T>
        {
            public int _capacity { get; private set; } = 1;
            public int _count { get; private set; } = 0;
            public int _lastNodeIndex { get; private set; } = -1;

            public CircularQueueNode<T> FirstNode { get; private set; } = null;
            public CircularQueueNode<T> LastNode { get; private set; } = null;

            private List<T> NodesList = new List<T>();

            public CircularQueue(int capacity)
            {
                if (capacity > 0)
                {
                    _capacity = capacity;
                    NodesList = new List<T>(capacity);
                }
            }

            public T Enqueue(T newNodeVal)
            {
                // Update last node index
                _lastNodeIndex = (_lastNodeIndex + 1) % _capacity;

                // Insert the new value of the last node
                if (NodesList.Count < _capacity)
                    // To the list of values
                    NodesList.Add(newNodeVal);
                else
                {
                    // To the list of values
                    NodesList[_lastNodeIndex] = newNodeVal;
                    // Change the first node to be the next
                    FirstNode = FirstNode.NextNode;
                }

                // Create the new node to be the last node
                LastNode = new CircularQueueNode<T>(this, newNodeVal, NodesList, _lastNodeIndex);

                // Check if the first node is null
                if (FirstNode == null)
                    // If yes then set the last node to be the first node as well
                    FirstNode = LastNode;

                // Update the count value
                _count = NodesList.Count;

                return newNodeVal;
            }

            public T GetFirst()
            {
                if (NodesList.Count != 0)
                    return NodesList[(_lastNodeIndex + 1) % _count];
                else
                    return default(T);
            }

            public T GetLast()
            {
                if (NodesList.Count != 0)
                    return NodesList[_lastNodeIndex];
                else
                    return default(T);
            }

            public T[] ToArray()
            {
                return NodesList.ToArray();
            }
        }

        public class CircularQueueNode<T>
        {
            public T Value { get; private set; }
            private int _nodeIndex;

            public CircularQueueNode<T> PreviousNode { get; private set; } = null;
            public CircularQueueNode<T> NextNode { get; private set; } = null;

            private CircularQueue<T> ParentCircularQueue { get; set; }
            private List<T> NodesList;

            public CircularQueueNode(CircularQueue<T> parentCircularQueue, T currentNodeVal, List<T> nodesList, int nodeIndex)
            {
                // Set the values to this node
                ParentCircularQueue = parentCircularQueue;
                NodesList = nodesList;
                Value = currentNodeVal;
                _nodeIndex = nodeIndex;
                // Move the loop of the circular queue
                parentCircularQueue.LastNode?.SetNextNode(this);
                PreviousNode = parentCircularQueue.LastNode;
                NextNode = parentCircularQueue.FirstNode;
                NextNode?.SetPreviousNode(this);
            }

            public void SetValue(T newValue)
            {
                Value = newValue;
                NodesList[_nodeIndex] = newValue;
            }

            public void SetPreviousNode(CircularQueueNode<T> previousNode)
            {
                PreviousNode = previousNode;
            }

            public void SetNextNode(CircularQueueNode<T> nextNode)
            {
                NextNode = nextNode;
            }
        }
    }
}

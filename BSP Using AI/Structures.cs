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
            public static string Signal { get; } = "Signal";
            public static string AllPeaks { get; } = "All peaks";
            public static string UpPeaks { get; } = "Up peaks";
            public static string DownPeaks { get; } = "Down peaks";
            public static string StableStates { get; } = "Stable";
            public static string Selection { get; } = "Selection";
            public static string Labels { get; } = "Labels";
            public static string PointHorizSpan { get; } = "Pointing horizontal span";
            public static string IntervalHorizSpan { get; } = "Interval horizontal span";
            public static string SpanAnnotations { get; } = "Span annotations";

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
            public double[] Features { get; set; }
            [DataMember]
            public double[] Outputs { get; set; }

            public Sample(string name, int numberOfFeatures, int numberOfOutputs, Data dataParent)
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

        [Serializable]
        [DataContract(IsReference = true)]
        public class ARTHTFeatures
        {
            [DataMember]
            public int _processedStep { get; set; } = 0;

            [DataMember]
            public List<Beat> SignalBeats { get; set; } = new List<Beat>();

            [DataMember]
            public Dictionary<string, Data> StepsDataDic { get; set; } = new Dictionary<string, Data>(7)
            {
                {ARTHTNamings.Step1RPeaksScanData, new Data(ARTHTNamings.Step1RPeaksScanData) },
                {ARTHTNamings.Step2RPeaksSelectionData, new Data(ARTHTNamings.Step2RPeaksSelectionData) },
                {ARTHTNamings.Step3BeatPeaksScanData, new Data(ARTHTNamings.Step3BeatPeaksScanData) },
                {ARTHTNamings.Step4PTSelectionData, new Data(ARTHTNamings.Step4PTSelectionData) },
                {ARTHTNamings.Step5ShortPRScanData, new Data(ARTHTNamings.Step5ShortPRScanData) },
                {ARTHTNamings.Step6UpstrokesScanData, new Data(ARTHTNamings.Step6UpstrokesScanData) },
                {ARTHTNamings.Step7DeltaExaminationData, new Data(ARTHTNamings.Step7DeltaExaminationData) }
            };

            public void Clear()
            {
                _processedStep = 0;
                SignalBeats.Clear();
                StepsDataDic[ARTHTNamings.Step1RPeaksScanData].Clear();
                StepsDataDic[ARTHTNamings.Step2RPeaksSelectionData].Clear();
                StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].Clear();
                StepsDataDic[ARTHTNamings.Step4PTSelectionData].Clear();
                StepsDataDic[ARTHTNamings.Step5ShortPRScanData].Clear();
                StepsDataDic[ARTHTNamings.Step6UpstrokesScanData].Clear();
                StepsDataDic[ARTHTNamings.Step7DeltaExaminationData].Clear();
            }
        }
        public class ARTHTNamings
        {
            public static string Features = "Features";
            public static string Outputs = "Outputs";

            public static string Step1RPeaksScanData { get; } = "R-peaks scan";
            public static string Step2RPeaksSelectionData { get; } = "R-peaks selection";
            public static string Step3BeatPeaksScanData { get; } = "Beat peaks scan";
            public static string Step4PTSelectionData { get; } = "P and T selection";
            public static string Step5ShortPRScanData { get; } = "Short PR scan";
            public static string Step6UpstrokesScanData { get; } = "Upstrokes scan";
            public static string Step7DeltaExaminationData { get; } = "Delta examination";

            public static string Mean = "mean";
            public static string Min = "min";
            public static string Max = "max";
            public static string StdDev = "STD_DEV";
            public static string IQR = "IQR";

            /// <summary>
            /// Amplitude ratio threshold
            /// </summary>
            public static string ART = "ART"; // Amplitude Ratio Threshold
            /// <summary>
            /// Horizontal threshold
            /// </summary>
            public static string HT = "HT"; // Horizontal Threshold


            public static string RIntrvl = "RpRcur/RRav";
            public static string RAmp = "ampRcur/ampRp";

            public static string RemoveR = "Remove R";


            public static string Beat = "Beat";


            public static string State = "State";

            public static string Stx = "Stx";
            public static string StRIntrvl = "(Stx - Rk) / (Rk - Rk-1)";
            public static string StAmp = "((ampStx - ampStx-1) + (ampStx - ampStx+1)) / 2";

            public static string PWave = "P wave";
            public static string TWave = "T wave";


            public static string PQIntrvl = "(Q - P) / (R - P)";

            public static string ShortPR = "Short PR";


            /// <summary>
            /// Tangent deviation threshold
            /// </summary>
            public static string TDT = "TDT"; // Tangent Deviation Threshold


            public static string DeltaAmp = "(ampDelta - ampQ) / (ampR - ampQ)";

            public static string WPWPattern = "WPW pattern";
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

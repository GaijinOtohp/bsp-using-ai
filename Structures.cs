using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biological_Signal_Processing_Using_AI
{
    internal class Structures
    {
        public class State
        {
            public string Name { get; set; }
            public int _startingIndex { get; set; }
            public int _index { get; set; }
            public double _startingValue { get; set; }
            public double _value { get; set; }
            public double _meanFromLastState { get; set; }
            public double _tangentFromLastState { get; set; }
            public double _meanTangentFromLastState { get; set; }
            public double _deviantionAngle { get; set; }
        }
    }
}

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
            public string Name;
            public int _index;
            public double _value;
            public double _meanFromLastState;
            public double _tangentFromLastState;
            public double _meanTangentFromLastState;
            public double _deviantionAngle;
        }
    }
}

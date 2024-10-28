using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.WFDB.Annotations.AnnotationsDefinitions;
using static Biological_Signal_Processing_Using_AI.WFDB.Samples.HeaderDefinitions;
using static Biological_Signal_Processing_Using_AI.WFDB.Samples.SamplesDefinitions;

namespace Biological_Signal_Processing_Using_AI.WFDB
{
    public class WFBDDefinitions
    {
        public class WFDBScope
        {
            public Header Header;
            public Dictionary<string, Signal> SignalsDict;
            public Dictionary<string, Annotation[]> AnnotationsDict;

            public WFDBScope() { }
        }
    }
}

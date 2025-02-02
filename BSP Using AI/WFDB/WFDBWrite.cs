using Biological_Signal_Processing_Using_AI.WFDB.Annotations;
using Biological_Signal_Processing_Using_AI.WFDB.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.WFDB.WFBDDefinitions;

namespace Biological_Signal_Processing_Using_AI.WFDB
{
    public class WFDBWrite
    {
        public static void WriteWFDBData(WFDBScope wfdbScope, string directoryPath, string annoFormat)
        {
            // Write the header file
            HeaderWrite.WriteHeaderFile(wfdbScope, directoryPath);

            // Write the samples file
            SamplesWrite.WriteSamplesFiles(wfdbScope, directoryPath);

            // Write the annotations file
            if (annoFormat.Equals("MIT"))
                AnnotationsWrite.MITFormat(wfdbScope, directoryPath);
            else if (annoFormat.Equals("AHA"))
                AnnotationsWrite.AHAFormat(wfdbScope, directoryPath);
        }
    }
}

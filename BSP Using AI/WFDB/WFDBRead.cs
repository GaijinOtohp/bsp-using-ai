using Biological_Signal_Processing_Using_AI.WFDB.Annotations;
using Biological_Signal_Processing_Using_AI.WFDB.Samples;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.WFDB.Annotations.AnnotationsDefinitions;
using static Biological_Signal_Processing_Using_AI.WFDB.Samples.SamplesDefinitions;
using static Biological_Signal_Processing_Using_AI.WFDB.WFBDDefinitions;

namespace Biological_Signal_Processing_Using_AI.WFDB
{
    public class WFDBRead
    {
        public static WFDBScope ReadWFDBInfo(string filePath)
        {
            WFDBScope wfdbScope = new WFDBScope();

            // Get the directory path and file name without extension
            string directoryPath = Path.GetDirectoryName(filePath);
            string fileNameNoExtension = Path.GetFileNameWithoutExtension(filePath);

            // Get all files with the same name
            Dictionary<string, string> similarFilePathsDict = System.IO.Directory.EnumerateFiles(directoryPath, fileNameNoExtension + ".*").ToArray().ToDictionary(path => Path.GetExtension(path), path => path);

            // Check if the header does not exist
            if (!similarFilePathsDict.ContainsKey(".hea"))
                return wfdbScope;

            // Check the format the signal is encoded with
            bool mitFormat = false;
            bool ahaFormat = false;
            if (similarFilePathsDict.ContainsKey(".dat"))
                mitFormat = true;
            else if (similarFilePathsDict.ContainsKey(".edf"))
                ahaFormat = true;

            // Read the header
            wfdbScope.Header = HeaderRead.ReadHeaderFile(similarFilePathsDict[".hea"]);

            // Get the signals
            Signal[] signals = SamplesRead.ReadSamplesFiles(filePath, wfdbScope.Header);
            wfdbScope.SignalsDict = signals.Select((signal, index) => (signal, index)).ToDictionary(tuple => tuple.signal.FileName + @"\" + tuple.index + @"\" + tuple.signal.description, tuple => tuple.signal);

            // Get annotations
            wfdbScope.AnnotationsDict = new Dictionary<string, Annotation[]>();
            foreach (string extension in similarFilePathsDict.Keys)
                if (!extension.Equals(".hea") && !extension.Equals(".dat") && !extension.Equals(".edf"))
                    if (mitFormat)
                        wfdbScope.AnnotationsDict.Add(extension, AnnotationsRead.MITFormat(similarFilePathsDict[extension]));
                    else if (ahaFormat)
                        wfdbScope.AnnotationsDict.Add(extension, AnnotationsRead.AHAFormat(similarFilePathsDict[extension]));

            return wfdbScope;
        }
    }
}

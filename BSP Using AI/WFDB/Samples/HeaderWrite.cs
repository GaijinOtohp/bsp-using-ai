using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.WFDB.Samples.HeaderDefinitions;
using static Biological_Signal_Processing_Using_AI.WFDB.Samples.SamplesDefinitions;
using static Biological_Signal_Processing_Using_AI.WFDB.WFBDDefinitions;

namespace Biological_Signal_Processing_Using_AI.WFDB.Samples
{
    //::::::::::::::::::::::::::::::More information source:::::::::::::::::::::::::::::://
    //___________________________________________________________________________________//
    // For more information visite the following website:
    // https://physionet.org/physiotools/wag/header-5.htm

    public class HeaderWrite
    {
        private static string WriteRecordLine(RecordLine recordLine)
        {
            return string.Join(" ", new string[] { recordLine.RecordName, recordLine.signalsNum.ToString(), ((int)recordLine.samplingFreq).ToString(), recordLine.samplesNum.ToString() });
        }

        private static int ComputeChecksum(int[] samples)
        {
            short checksum = 0;

            foreach (short sample in samples)
                checksum += sample;

            return checksum;
        }

        private static string[] WriteSignalSpecificationLines(List<SignalSpecsLine> signalLinesList, Dictionary<string, Signal> signalsDict)
        {
            string[] sinalLines = new string[signalLinesList.Count];
            for (int iSignal = 0; iSignal < signalLinesList.Count; iSignal++)
            {
                SignalSpecsLine signalLine = signalLinesList[iSignal];
                string signalKey = signalLine.FileName + @"\" + iSignal + @"\" + signalLine.description;
                Signal signal = signalsDict[signalKey];

                int checksum = ComputeChecksum(signal.Samples);

                sinalLines[iSignal] = string.Join(" ", new string[] { signal.FileName, signalLine.format.ToString(), signal.adcGain.ToString(), signalLine.resolution.ToString(), signalLine.adcZero.ToString(),
                                                   signal.Samples[0].ToString(), checksum.ToString(), signalLine.blockSize.ToString(), signal.description});
            }

            return sinalLines;
        }

        public static void WriteHeaderFile(WFDBScope wfdbScope, string directoryPath)
        {
            List<string> HeaderLines = new List<string>(3);
            HeaderLines.Add(WriteRecordLine(wfdbScope.Header.RecordLineSpecs));
            HeaderLines.AddRange(WriteSignalSpecificationLines(wfdbScope.Header.SignalLinesList, wfdbScope.SignalsDict));

            string filePath = directoryPath + @"\" + wfdbScope.Header.RecordLineSpecs.RecordName + ".hea";
            File.WriteAllLines(filePath, HeaderLines.ToArray(), Encoding.ASCII);
        }
    }
}

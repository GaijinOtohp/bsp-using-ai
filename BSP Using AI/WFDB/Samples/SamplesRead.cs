using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.WFDB.Samples.HeaderDefinitions;
using static Biological_Signal_Processing_Using_AI.WFDB.Samples.SamplesDefinitions;

namespace Biological_Signal_Processing_Using_AI.WFDB.Samples
{
    public class SamplesRead
    {
        //::::::::::::::::::::::::::::::More information source:::::::::::::::::::::::::::::://
        //___________________________________________________________________________________//
        // For more information visite the following website:
        // https://physionet.org/physiotools/wag/signal-5.htm

        private static int[] SamplesReadFormat8(BitArray samplesBits)
        {
            List<int> samplesList = new List<int>(samplesBits.Length / 8 + 1);

            for (int i = 0; i < samplesBits.Length; i += 8)
            {
                BitArray sampleBits = new BitArray(8);
                for (int j = 0; j < 8; j++)
                    sampleBits.Set(j, samplesBits.Get(i + j));
                int[] newSample = new int[1];
                sampleBits.CopyTo(newSample, 0);

                samplesList.Add(newSample[0]);
            }

            return samplesList.ToArray();
        }

        private static int[] SamplesReadFormat212(BitArray samplesBits)
        {
            List<int> samplesList = new List<int>(samplesBits.Length / 12 + 1);

            // The final samples would be stored in a 32 bit int
            // while the original samples occupies 12 bits
            int sampleOriginalBitLen = 12;
            int sampleFinalBitLen = 32;
            int bitExtension = sampleFinalBitLen - sampleOriginalBitLen;

            // Each three successive bytes contains two 12 bits samples
            for (int i = 0; i < samplesBits.Length; i += 24)
            {
                BitArray evenSampleBits = new BitArray(12);
                BitArray oddSampleBits = new BitArray(12);
                for (int j = 0; j < 12; j++)
                {
                    evenSampleBits.Set(j, samplesBits.Get(i + j));
                    oddSampleBits.Set(j, samplesBits.Get(i + 12 + (j + 4) % 12)); // The 4 high significant bits of second sample are stored before the 8 least significant bits
                }
                int[] newSample = new int[2]; // Short is 16 bit signed integer, while int is a 32 bit signed integer
                evenSampleBits.CopyTo(newSample, 0);
                oddSampleBits.CopyTo(newSample, 1);

                // Extend the 12 bits copied signed values to 32 bits int signed values
                // Shift the 12 bits that are occupying the right side of the 32 bit int to the left
                // then shift them back to the right to their original positions so they get the 32 bit format
                newSample[0] = (newSample[0] << bitExtension) >> bitExtension;
                newSample[1] = (newSample[1] << bitExtension) >> bitExtension;

                samplesList.Add(newSample[0]);
                samplesList.Add(newSample[1]);
            }

            return samplesList.ToArray();
        }

        private static int[] GetSignalSamples(BitArray samplesBits, int format)
        {
            int[] samples = null;
            if (format == FormatCodes.format8)
                samples = SamplesReadFormat8(samplesBits);
            else if (format == FormatCodes.format212)
                samples = SamplesReadFormat212(samplesBits);

            return samples;
        }

        private static List<(string fileName, int format)> GetSignalsFilesNamesFormat(Header header)
        {
            List<(string fileName, int format)> filesNamesFormat = null;
            if (header.SegmentLinesList != null)
                filesNamesFormat = header.SegmentLinesList.Select(segmentSpecs => (segmentSpecs.RecordName, FormatCodes.format8)).Distinct().ToList();
            else if (header.SignalLinesList != null)
                filesNamesFormat = header.SignalLinesList.Select(signalSpecs => (signalSpecs.FileName, signalSpecs.format)).Distinct().ToList();

            return filesNamesFormat;
        }

        private static Dictionary<string, int[]> GetSignalsFilesSamples(string directoryPath, Header header)
        {
            // Get the files names and their formats
            List<(string fileName, int format)> filesNamesFormat = GetSignalsFilesNamesFormat(header);

            Dictionary<string, int[]> signalsSamplesDict = new Dictionary<string, int[]>(filesNamesFormat.Count + 1);
            foreach ((string fileName, int format) in filesNamesFormat)
            {
                // Create the full path of the file
                string signalFilePath = null;
                foreach (string extension in new string[] { "", ".edf", ".dat" })
                    if (File.Exists(directoryPath + @"\" + fileName + extension))
                    {
                        signalFilePath = directoryPath + @"\" + fileName + extension;
                        break;
                    }
                // Convert the file to bit array
                byte[] signalBytes = File.ReadAllBytes(signalFilePath);
                // Convert the bytes to bits
                BitArray signalBits = new BitArray(signalBytes);

                // Get the samples of the signal from the bit array according its coding format
                signalsSamplesDict.Add(fileName, GetSignalSamples(signalBits, format));
            }

            return signalsSamplesDict;
        }

        public static Signal[] ReadSamplesFiles(string filePath, Header header)
        {
            // The signals specifications are declared in the header as either "signal specifications" or "segment specifications"
            List<Signal> signalsList = new List<Signal>(Math.Max(header.RecordLineSpecs.segmentsNum, header.RecordLineSpecs.signalsNum) + 1);

            // Create the signals with the initial information
            if (header.SegmentLinesList != null)
            {
                // From segments lines
                foreach (SegmentSpecsLine segmentSpecs in header.SegmentLinesList)
                {
                    Signal signal = new Signal();
                    signal.FileName = segmentSpecs.RecordName;
                    signal.samplingFreq = header.RecordLineSpecs.samplingFreq;
                    signal.Samples = new int[segmentSpecs.samplesCount];

                    signalsList.Add(signal);
                }
            }
            else if (header.SignalLinesList != null)
            {
                // From signals lines
                foreach (SignalSpecsLine signalSpecs in header.SignalLinesList)
                {
                    Signal signal = new Signal();
                    signal.FileName = signalSpecs.FileName;
                    signal.samplingFreq = signalSpecs.samplesPerFrame * header.RecordLineSpecs.samplingFreq;
                    signal.adcGain = signalSpecs.adcGain;
                    signal.Samples = new int[header.RecordLineSpecs.samplesNum];
                    signal.description = signalSpecs.description;

                    signalsList.Add(signal);
                }
            }

            // Copy the signals multiplexed samples to theri corresponding signal in signalsList

            // Get the directory path
            string directoryPath = Path.GetDirectoryName(filePath);

            // Get signals samples
            Dictionary<string, int[]> signalsSamples = GetSignalsFilesSamples(directoryPath, header);

            // Group the signals to their file names
            Dictionary<string, List<Signal>> groupedSignals = signalsList.GroupBy(signal => signal.FileName).ToDictionary(group => group.Key, group => group.ToList());

            // Distribute each signal samples to its multiplexed groups
            foreach (string fileName in signalsSamples.Keys)
            {
                int[] signalSamples = signalsSamples[fileName];
                List<Signal> signalGroup = groupedSignals[fileName];

                int totalMultiplexedSamples = signalGroup.Select(signal => signal.Samples.Length).Sum();
                for (int iSample = 0; iSample < totalMultiplexedSamples; iSample++)
                    signalGroup[iSample % signalGroup.Count].Samples[iSample / signalGroup.Count] = signalSamples[iSample];
            }

            return signalsList.ToArray();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.WFDB.Samples.SamplesDefinitions;
using static Biological_Signal_Processing_Using_AI.WFDB.WFBDDefinitions;

namespace Biological_Signal_Processing_Using_AI.WFDB.Samples
{
    public class SamplesWrite
    {
        //::::::::::::::::::::::::::::::More information source:::::::::::::::::::::::::::::://
        //___________________________________________________________________________________//
        // For more information visite the following website:
        // https://physionet.org/physiotools/wag/signal-5.htm

        private static byte[] BytesWriteFormat8(byte[] multiplexedBytes)
        {
            List<byte> compressedMultiplexedBytes = new List<byte>(multiplexedBytes.Length / 4);

            for (int i = 0; i < multiplexedBytes.Length; i += 4)
                compressedMultiplexedBytes.Add(multiplexedBytes[i]);

            return compressedMultiplexedBytes.ToArray();
        }

        private static byte[] BytesWriteFormat212(byte[] multiplexedBytes)
        {
            List<byte> compressedMultiplexedBytes = new List<byte>(multiplexedBytes.Length * 12 / 32);

            // The final samples would be stored in 12 bits
            // while the original samples occupies 32 bits (4 bytes)

            // Each two ints (8 bytes) should make 3 successive bytes
            for (int i = 0; i < multiplexedBytes.Length; i += 8)
            {
                BitArray multiplexedBits = new BitArray(24);
                BitArray evenSampleBits = i < multiplexedBytes.Length ? new BitArray(new byte[] { multiplexedBytes[i], multiplexedBytes[i + 1] }) : null;
                BitArray oddSampleBits = i + 4 < multiplexedBytes.Length ? new BitArray(new byte[] { multiplexedBytes[i + 4], multiplexedBytes[i + 5] }) : null;
                for (int j = 0; j < 12; j++)
                {
                    if (evenSampleBits != null)
                        multiplexedBits.Set(j, evenSampleBits[j]);
                    if (oddSampleBits != null)
                        multiplexedBits.Set(j + 12, oddSampleBits[(j + 8) % 12]); // The 4 high significant bits of second sample are stored before the 8 least significant bits
                }

                // Copy the new compressed bits to the total compressed bytes
                byte[] compressedBytes = new byte[multiplexedBits.Length / 8];
                multiplexedBits.CopyTo(compressedBytes, 0);
                compressedMultiplexedBytes.AddRange(compressedBytes);
            }

            return compressedMultiplexedBytes.ToArray();
        }

        private static byte[] GetMultiplexedByteArray(int[] multiplexedSamples, int format)
        {
            // Convert the multiplexed samples to byte array
            byte[] multiplexedBytes = new byte[multiplexedSamples.Length * sizeof(int)];
            Buffer.BlockCopy(multiplexedSamples, 0, multiplexedBytes, 0, multiplexedBytes.Length);
            // Convert the multiplexedBytes to the compressed WFDB format
            byte[] compressedMultiplexedBytes = null;
            if (format == FormatCodes.format8)
                compressedMultiplexedBytes = BytesWriteFormat8(multiplexedBytes);
            else if (format == FormatCodes.format212)
                compressedMultiplexedBytes = BytesWriteFormat212(multiplexedBytes);

            return compressedMultiplexedBytes;
        }

        private static void WriteBinaryFiles(int[] multiplexedSamples, int format, string filePath)
        {
            // Get the compressed byte array
            byte[] compressedMultiplexedBytes = GetMultiplexedByteArray(multiplexedSamples, format);
            // Write the compressed bytes to a file
            File.WriteAllBytes(filePath, compressedMultiplexedBytes);
        }

        public static void WriteSamplesFiles(WFDBScope wfdbScope, string directoryPath)
        {
            // Combine all samples in one array according to their fileNames
            Dictionary<string, List<Signal>> signalsGroups = wfdbScope.SignalsDict.Values.GroupBy(signal => signal.FileName).
                                                                                         ToDictionary(group => group.Key, group => group.ToList());

            // Create dictionary of fileNames with formats
            Dictionary<string, int> formatGroups = wfdbScope.Header.SignalLinesList.GroupBy(signalLine => signalLine.FileName).
                                                                                    ToDictionary(group => group.Key, group => group.ToList()[0].format);

            // Create the multiplexes signal for each group
            foreach (string fileName in signalsGroups.Keys)
            {
                // Create the multiplexed samples of the group
                int[] multiplexedSamples = new int[signalsGroups[fileName].Count * wfdbScope.Header.RecordLineSpecs.samplesNum];

                // The operation (iGlobSamp % signalsGroups[fileName].Count) finds the signal index in the group
                // The operation (iGlobSamp / signalsGroups[fileName].Count) finds the sample index in the signal
                for (int iGlobSamp = 0; iGlobSamp < multiplexedSamples.Length; iGlobSamp++)
                    multiplexedSamples[iGlobSamp] = signalsGroups[fileName][iGlobSamp % signalsGroups[fileName].Count].Samples[iGlobSamp / signalsGroups[fileName].Count];

                // Write the samples file
                string filePath = directoryPath + @"\" + fileName;
                WriteBinaryFiles(multiplexedSamples, formatGroups[fileName], filePath);
            }
        }
    }
}

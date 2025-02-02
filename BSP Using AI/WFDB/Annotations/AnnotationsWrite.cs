using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.WFDB.Annotations.AnnotationsDefinitions;
using static Biological_Signal_Processing_Using_AI.WFDB.WFBDDefinitions;

namespace Biological_Signal_Processing_Using_AI.WFDB.Annotations
{
    public class AnnotationsWrite
    {
        //::::::::::::::::::::::::::::::More information source:::::::::::::::::::::::::::::://
        //___________________________________________________________________________________//
        // For more information visite the following website:
        // https://physionet.org/physiotools/wag/annot-5.htm

        private static byte[] GetAnnoBits(int first10Bits, int last6Bits)
        {
            // Copy infos to compressedAnnoBytes
            BitArray intervalBits = new BitArray(new int[] { first10Bits });
            BitArray codeBits = new BitArray(new int[] { last6Bits });

            BitArray annoBits = new BitArray(16);
            for (int i = 0; i < 10; i++)
                annoBits.Set(i, intervalBits.Get(i));
            for (int i = 0; i < 6; i++)
                annoBits.Set(10 + i, codeBits.Get(i));

            byte[] annoBytes = new byte[2];
            annoBits.CopyTo(annoBytes, 0);

            return annoBytes;
        }

        public static void MITFormat(WFDBScope wfdbScope, string directoryPath)
        {
            // Itirate trough each annotation array
            foreach (string annoExten in wfdbScope.AnnotationsDict.Keys)
            {
                List<byte> compressedAnnoBytes = new List<byte>(wfdbScope.AnnotationsDict[annoExten].Length * 3);

                int latestAnnoIndex = 0;
                int number = 0;
                int channel = 0;

                // Itirate through each annotation in the array
                foreach (Annotation anno in wfdbScope.AnnotationsDict[annoExten])
                {
                    // Get anno info
                    int intervalFromLastAnno = anno.index - latestAnnoIndex;
                    int annoCode = anno.codeValue;
                    latestAnnoIndex = anno.index;

                    compressedAnnoBytes.AddRange(GetAnnoBits(intervalFromLastAnno, annoCode));

                    // Check if channel is changed
                    if (anno.channel != channel)
                    {
                        compressedAnnoBytes.AddRange(GetAnnoBits(anno.channel, AnnotationsDefinitions.AnnotationCodes.Control.CHN));
                        channel = anno.channel;
                    }

                    // Check if number is changed
                    if (anno.number != number)
                    {
                        compressedAnnoBytes.AddRange(GetAnnoBits(anno.number, AnnotationsDefinitions.AnnotationCodes.Control.NUM));
                        number = anno.number;
                    }
                }

                // Write the compressed bytes to a file
                string filePath = directoryPath + @"\" + wfdbScope.Header.RecordLineSpecs.RecordName + annoExten;
                File.WriteAllBytes(filePath, compressedAnnoBytes.ToArray());
            }
        }

        public static void AHAFormat(WFDBScope wfdbScope, string directoryPath)
        {
            ///:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::///
            ///:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::///
            ///:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::///
            ///:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::///
            ///----------------------------------------------TODO---------------------------------------------///

        }
    }
}

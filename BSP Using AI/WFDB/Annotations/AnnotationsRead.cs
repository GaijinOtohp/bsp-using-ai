using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.WFDB.Annotations.AnnotationsDefinitions;

namespace Biological_Signal_Processing_Using_AI.WFDB.Annotations
{
    public class AnnotationsRead
    {
        //::::::::::::::::::::::::::::::More information source:::::::::::::::::::::::::::::://
        //___________________________________________________________________________________//
        // For more information visite the following website:
        // https://physionet.org/physiotools/wag/annot-5.htm

        public static Annotation[] MITFormat(string filePath)
        {
            // Get the annotations file as bytes
            byte[] fileBytes = File.ReadAllBytes(filePath);
            // Convert the bytes to bits
            BitArray fileBits = new BitArray(fileBytes);

            // Each information is stored in an even number of bytes
            // and each annotation holds two bytes
            // The "skip" annotation takes its next four bytes (32 bits) as a PDP-11 long integer defining the interval in the signal to skip (because of noise)
            int annoBitDistance = 16;
            int latestAnnoIndex = 0;
            int skipInterval = 0;       // "int" in C# takes 32 bits in memory
            int selectedNum = 0;
            int selectedChannel = 0;

            List<Annotation> annotations = new List<Annotation>(fileBytes.Length / 2 + 1);
            int[] annoInfo = new int[2]; // Each annotation occupies two bytes (16 bits) (the first 10 bits contains the samples interval and the rest 6 bits holds the annotation code)

            for (int i = 0; i < fileBits.Length; i += annoBitDistance)
            {
                // Build the annotation information from the next 16 bits
                BitArray intervalFromLastAnnoBits = new BitArray(10);       // [I] int the documentation
                BitArray annoCodeBits = new BitArray(6);                    // [A] int the documentation
                for (int j = 0; j < 10; j++)
                    if (i + j < fileBits.Length)
                        intervalFromLastAnnoBits.Set(j, fileBits.Get(i + j));
                for (int j = 0; j < 6; j++)
                    if (10 + i + j < fileBits.Length)
                        annoCodeBits.Set(j, fileBits.Get(10 + i + j));
                // Copy the interval and the code of the annotation to annoInfo
                intervalFromLastAnnoBits.CopyTo(annoInfo, 0);
                annoCodeBits.CopyTo(annoInfo, 1);

                // Create the new annotation
                if (AnnotationsDefinitions.AnnotationCodes.AnnotationCodesDict.ContainsKey(annoInfo[1]))
                {
                    int intervalFromLastAnno = annoInfo[0];
                    int annoCode = annoInfo[1];

                    Annotation newAnnotation = AnnotationsDefinitions.AnnotationCodes.AnnotationCodesDict[annoCode].Clone();
                    newAnnotation.index = latestAnnoIndex + skipInterval + intervalFromLastAnno;
                    newAnnotation.number = selectedNum;
                    newAnnotation.channel = selectedChannel;

                    annotations.Add(newAnnotation);

                    latestAnnoIndex = newAnnotation.index;
                    skipInterval = 0;
                }
                else if (annoInfo[1] == AnnotationsDefinitions.AnnotationCodes.Control.SKIP)
                {
                    // I which is annoInfo[0] should equal "0" and has no information
                    // Get the next four bytes (32 bits) as a long integer representing the interval to skip
                    // The the interval is in PDP-11 long integer format (the high 16 bits first, then the low 16 bits, with the low byte first in each pair)
                    BitArray skipIntervalBits = new BitArray(32);
                    // Take the low 16 bits
                    for (int j = 0; j < 16; j++)
                        if (16 + i + 16 + j < fileBits.Length)
                            skipIntervalBits.Set(j, fileBits.Get(16 + i + 16 + j));
                    // Take the high 16 bits
                    for (int j = 0; j < 16; j++)
                        if (16 + i + j < fileBits.Length)
                            skipIntervalBits.Set(16 + j, fileBits.Get(16 + i + j));

                    // Convert the long bits to a long variable
                    int[] skipIntervalArray = new int[1];
                    skipIntervalBits.CopyTo(skipIntervalArray, 0);          // "int" in C# takes 32 bits in memory

                    skipInterval = (int)skipIntervalArray[0];

                    // Forward i by 32 bits additional to the annotation 16 bits
                    i += 32;
                }
                else if (annoInfo[1] == AnnotationsDefinitions.AnnotationCodes.Control.NUM)
                {
                    // Change the num of the next annotations being P[0], QRS[1], or T[2]
                    selectedNum = annoInfo[0];
                    // additional to the previous one
                    annotations[annotations.Count - 1].number = selectedNum;
                }
                else if (annoInfo[1] == AnnotationsDefinitions.AnnotationCodes.Control.SUB)
                {
                    ///:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::///
                    ///:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::///
                    ///:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::///
                    ///:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::///
                    ///----------------------------------------------TODO---------------------------------------------///
                }
                else if (annoInfo[1] == AnnotationsDefinitions.AnnotationCodes.Control.CHN)
                {
                    // Change the channel of the next annotations
                    selectedChannel = annoInfo[0];
                    // additional to the previous one
                    annotations[annotations.Count - 1].channel = selectedChannel;
                }
                else if (annoInfo[1] == AnnotationsDefinitions.AnnotationCodes.Control.AUX)
                {
                    // I which is annoInfo[0] contains the number of bytes of auxiliary information
                    // that starts from the next 16 bits
                    int auxBytesCount = annoInfo[0];

                    BitArray auxBits = new BitArray(auxBytesCount * 8);
                    for (int j = 0; j < auxBytesCount * 8; j++)
                        if (16 + i + j < fileBits.Length)
                            auxBits.Set(j, fileBits.Get(16 + i + j));
                }
            }

            return annotations.ToArray();
        }

        public static Annotation[] AHAFormat(string filePath)
        {
            ///:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::///
            ///:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::///
            ///:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::///
            ///:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::///
            ///----------------------------------------------TODO---------------------------------------------///
            
            return new List<Annotation>().ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biological_Signal_Processing_Using_AI.WFDB.Samples
{
    public class SamplesDefinitions
    {
        //::::::::::::::::::::::::::::::More information source:::::::::::::::::::::::::::::://
        //___________________________________________________________________________________//
        // For more information visite the following website:
        // https://physionet.org/physiotools/wag/signal-5.htm

        public class Signal
        {
            public string FileName;
            public double samplingFreq;
            public double adcGain = 200; // 200 is the default value defined in WFDB
            public int[] Samples;
            public string description = "";

            public Signal() { }
        }

        public static class FormatCodes
        {
            public static int format8 = 8; // Each sample is represented as an 8-bit first difference; i.e., to get the value of sample n, sum the first n bytes of the sample data file together with the initial value from the header file
            public static int format16 = 16; // Each sample is represented by a 16-bit two’s complement amplitude stored least significant byte first
            public static int format24 = 24; // Each sample is represented by a 24-bit two’s complement amplitude stored least significant byte first.
            public static int format32 = 32; // Each sample is represented by a 32-bit two’s complement amplitude stored least significant byte first.
            public static int format61 = 61; // Each sample is represented by a 16-bit two’s complement amplitude stored most significant byte first.
            public static int format80 = 80; // Each sample is represented by an 8-bit amplitude in offset binary form (i.e., 128 must be subtracted from each unsigned byte to obtain a signed 8-bit amplitude).
            public static int format160 = 160; // Each sample is represented by a 16-bit amplitude in offset binary form (i.e., 32,768 must be subtracted from each unsigned byte pair to obtain a signed 16-bit amplitude). As for format 16, the least significant byte of each pair is first.
            public static int format212 = 212; // Each sample is represented by a 12-bit two’s complement amplitude.
            public static int format310 = 310; // Each sample is represented by a 10-bit two’s-complement amplitude.
            public static int format311 = 311; // Each sample is represented by a 10-bit two’s-complement amplitude.
            public static int format508 = 508; // Signals are compressed using the FLAC (Free Lossless Audio Codec) format, using 8 bits per sample.
            public static int format516 = 516; // Signals are compressed using the FLAC (Free Lossless Audio Codec) format, using 16 bits per sample.
            public static int format524 = 524; // Signals are compressed using the FLAC (Free Lossless Audio Codec) format, using 24 bits per sample.
        }
    }
}

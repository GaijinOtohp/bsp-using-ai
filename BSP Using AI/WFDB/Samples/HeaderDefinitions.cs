using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biological_Signal_Processing_Using_AI.WFDB.Samples
{
    public class HeaderDefinitions
    {
        //::::::::::::::::::::::::::::::More information source:::::::::::::::::::::::::::::://
        //___________________________________________________________________________________//
        // For more information visite the following website:
        // https://physionet.org/physiotools/wag/header-5.htm

        public class RecordLine
        {
            public string RecordName;
            public int segmentsNum = 1;                 // optional
            public int signalsNum;
            public double samplingFreq = 250;           // optional
            public double counterFreq = 250;            // optional
            public double baseCounter = 0;              // optional
            public int samplesNum;                      // optional
            public string BaseTime = "0:0:0";           // optional
            public string BaseDate = "DD/MM/YYYY";      // optional

            public RecordLine() { }
        }

        public class SignalSpecsLine
        {
            public string FileName;
            public int format;
            public int samplesPerFrame = 1;             // optional
            public int skew;                            // optional
            public int byteOffset;                      // optional
            public double adcGain = 200;                // optional (ADC units per physical unit)
            public int baseline;                        // optional (by ADC units)
            public string units = "mV";                 // optional (default: one millivolt)
            public int resolution = 12;                 // optional (the quantization resolution in bits)
            public int adcZero = 0;                     // optional (the amplitude of the middle of the signal range)
            public int initialValue = 0;                // optional
            public Int16 checksum = 0;                  // optional
            public int blockSize;                       // optional (specifies the block size in bytes)
            public string description = "";             // optional

            public SignalSpecsLine() { }
        }

        public class SegmentSpecsLine
        {
            public string RecordName;
            public int samplesCount;

            public SegmentSpecsLine() { }
        }

        public class Header
        {
            // Record line
            public RecordLine RecordLineSpecs;

            // Signal specification lines
            public List<SignalSpecsLine> SignalLinesList;

            // Segment specification lines
            public List<SegmentSpecsLine> SegmentLinesList;

            public Header() { }
        }
    }
}

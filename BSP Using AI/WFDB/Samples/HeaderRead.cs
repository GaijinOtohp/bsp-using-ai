using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.WFDB.Samples.HeaderDefinitions;

namespace Biological_Signal_Processing_Using_AI.WFDB.Samples
{
    public class HeaderRead
    {
        //::::::::::::::::::::::::::::::More information source:::::::::::::::::::::::::::::://
        //___________________________________________________________________________________//
        // For more information visite the following website:
        // https://physionet.org/physiotools/wag/header-5.htm

        private static RecordLine ReadRecordLine(string[] whiteSpacedParts)
        {
            RecordLine recordLineSpecs = new RecordLine();

            for (int iPart = 0; iPart < whiteSpacedParts.Length; iPart++)
                switch (iPart)
                {
                    case 0:
                        // The record_name and number_of_segments part
                        // the record name and the number of segments are separated by '/'
                        string[] recordNameParts = whiteSpacedParts[iPart].Split("/", StringSplitOptions.RemoveEmptyEntries);
                        for (int iRecPaert = 0; iRecPaert < recordNameParts.Length; iRecPaert++)
                            if (iRecPaert == 0)
                                recordLineSpecs.RecordName = recordNameParts[0];
                            // the number of segments is not always present
                            else if (iRecPaert == 1)
                                recordLineSpecs.segmentsNum = int.Parse(recordNameParts[1]);
                        break;
                    case 1:
                        // The number of signals part
                        recordLineSpecs.signalsNum = int.Parse(whiteSpacedParts[iPart]);
                        break;
                    case 2:
                        // The sampling frequency, counter frequency, and base counter value part
                        // the sampling frequency and the counter frequency are separated by '/'
                        string[] samplingFreqParts = whiteSpacedParts[iPart].Split(new string[] { "/", "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int iSamPart = 0; iSamPart < samplingFreqParts.Length; iSamPart++)
                            if (iSamPart == 0)
                                recordLineSpecs.samplingFreq = double.Parse(samplingFreqParts[0]);
                            else if (iSamPart == 1)
                                // The counter frequency exists only if the sampling frequency also exists
                                recordLineSpecs.counterFreq = double.Parse(samplingFreqParts[1]);
                            else if (iSamPart == 2)
                                // The base counter exists only if the counter frequency also exists
                                recordLineSpecs.baseCounter = double.Parse(samplingFreqParts[2]);
                        // Otherwise, it would be the same as the sampling frequency
                        if (samplingFreqParts.Length == 1)
                            recordLineSpecs.counterFreq = recordLineSpecs.samplingFreq;
                        break;
                    case 3:
                        // The number of samples per signal part
                        recordLineSpecs.samplesNum = int.Parse(whiteSpacedParts[iPart]);
                        break;
                    case 4:
                        recordLineSpecs.BaseTime = whiteSpacedParts[iPart];
                        break;
                    case 5:
                        recordLineSpecs.BaseDate = whiteSpacedParts[iPart];
                        break;
                }

            return recordLineSpecs;
        }

        private static SignalSpecsLine ReadSignalSpecificationLine(string[] whiteSpacedParts)
        {
            SignalSpecsLine signalSpecsLine = new SignalSpecsLine();

            for (int iPart = 0; iPart < whiteSpacedParts.Length; iPart++)
                switch (iPart)
                {
                    case 0:
                        // The file name part
                        signalSpecsLine.FileName = whiteSpacedParts[iPart];
                        break;
                    case 1:
                        // The format, samples per frame, skew, and byte offset parts
                        string[] formatParts = whiteSpacedParts[iPart].Split(new string[] { "x", ":", "+" }, StringSplitOptions.RemoveEmptyEntries);
                        signalSpecsLine.format = int.Parse(formatParts[0]);
                        for (int iFormPart = 1; iFormPart < formatParts.Length; iFormPart++)
                            if (whiteSpacedParts[iPart].Contains("x") && iFormPart <= 1) // It could be only after the format
                                signalSpecsLine.samplesPerFrame = int.Parse(formatParts[iFormPart]);
                            else if (whiteSpacedParts[iPart].Contains(":") && iFormPart <= 2) // It could be after the samplesPerFrame or after format if samplesPerFrame is not declared
                                signalSpecsLine.skew = int.Parse(formatParts[iFormPart]);
                            else if (whiteSpacedParts[iPart].Contains("+") && iFormPart <= 3) // It could be after the skew, samplesPerFrame, or format
                                signalSpecsLine.byteOffset = int.Parse(formatParts[iFormPart]);
                        break;
                    case 2:
                        // The ADC gain (ADC units per physical unit), base line (ADC units), and units parts
                        string[] adcGainParts = whiteSpacedParts[iPart].Split(new string[] { "(", ")", "/" }, StringSplitOptions.RemoveEmptyEntries);
                        signalSpecsLine.adcGain = double.Parse(adcGainParts[0]);
                        for (int iADCPart = 1; iADCPart < adcGainParts.Length; iADCPart++)
                            if (whiteSpacedParts[iPart].Contains("(") && iADCPart <= 1) // It could be only after the format
                                signalSpecsLine.baseline = int.Parse(adcGainParts[iADCPart]);
                            else if (whiteSpacedParts[iPart].Contains("/") && iADCPart <= 2) // It could be after the samplesPerFrame or after format if samplesPerFrame is not declared
                                signalSpecsLine.units = adcGainParts[iADCPart];
                        break;
                    case 3:
                        // The ADC resolution part
                        signalSpecsLine.resolution = int.Parse(whiteSpacedParts[iPart]);
                        // If ADC resolution is 0 then the default value is 12 bits for amplitude-format signals, or 10 bits for difference-format signals
                        if (signalSpecsLine.resolution == 0)
                            signalSpecsLine.resolution = 12;
                        break;
                    case 4:
                        // The ADC zero part
                        signalSpecsLine.adcZero = int.Parse(whiteSpacedParts[iPart]);
                        // If the base line was not previously declared on the header then it should be set as ADCzero
                        if (signalSpecsLine.baseline == 0)
                            signalSpecsLine.baseline = signalSpecsLine.adcZero;
                        // The same as with the initial value
                        if (whiteSpacedParts.Length < 5)
                            signalSpecsLine.initialValue = signalSpecsLine.adcZero;
                        break;
                    case 5:
                        // The initial value part
                        signalSpecsLine.initialValue = int.Parse(whiteSpacedParts[iPart]);
                        break;
                    case 6:
                        // The checksum part
                        signalSpecsLine.checksum = Int16.Parse(whiteSpacedParts[iPart]);
                        break;
                    case 7:
                        // The block size part
                        signalSpecsLine.blockSize = int.Parse(whiteSpacedParts[iPart]);
                        break;
                    case 8:
                        // The rest of the parts builds the description part
                        signalSpecsLine.description = string.Join(" ", whiteSpacedParts, iPart, whiteSpacedParts.Length - iPart);
                        // End up the loop
                        iPart = whiteSpacedParts.Length;
                        break;
                }

            return signalSpecsLine;
        }

        private static SegmentSpecsLine ReadSegmentSpecificationLine(string[] whiteSpacedParts)
        {
            SegmentSpecsLine segmentSpecsLine = new SegmentSpecsLine();

            for (int iPart = 0; iPart < whiteSpacedParts.Length; iPart++)
                switch (iPart)
                {
                    case 0:
                        // The record name part
                        segmentSpecsLine.RecordName = whiteSpacedParts[iPart];
                        break;
                    case 1:
                        // The number of samples per signal part
                        segmentSpecsLine.samplesCount = int.Parse(whiteSpacedParts[iPart]);
                        break;
                }

            return segmentSpecsLine;
        }

        public static Header ReadHeaderFile(string filePath)
        {
            // Create the header reader
            StreamReader headerStreamReader = new StreamReader(filePath, Encoding.ASCII);

            // Start reading the header line by line
            Header header = new Header();
            while (!headerStreamReader.EndOfStream)
            {
                string line = headerStreamReader.ReadLine();
                // Check if this is a comment line
                if (line[0].Equals("#") || line.Equals(string.Empty))
                    // Ignore the line
                    continue;

                // Get the parts separated by white spaces
                string[] whiteSpacedParts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                // The first non-empty, non-comment line is the record line
                if (header.RecordLineSpecs == null)
                {
                    header.RecordLineSpecs = ReadRecordLine(whiteSpacedParts);

                    // If the number of segments are greater than one
                    // then the rest of the lines are segment specification lines
                    if (header.RecordLineSpecs.segmentsNum > 1)
                        header.SegmentLinesList = new List<SegmentSpecsLine>(header.RecordLineSpecs.segmentsNum + 1);
                    // Otherwise, the rest are signal specification lines
                    else
                        header.SignalLinesList = new List<SignalSpecsLine>(header.RecordLineSpecs.signalsNum + 1);
                }
                // The rest are either signal specification lines
                else if (header.SignalLinesList != null)
                {
                    // Any extra signal line other than the indicated number in the record line is ignored
                    if (header.SignalLinesList.Count < header.RecordLineSpecs.signalsNum)
                        header.SignalLinesList.Add(ReadSignalSpecificationLine(whiteSpacedParts));
                }
                // or segment specification lines
                else if (header.SegmentLinesList != null)
                {
                    // Any extra segment line other than the indicated number in the record line is ignored
                    if (header.SegmentLinesList.Count < header.RecordLineSpecs.segmentsNum)
                        header.SegmentLinesList.Add(ReadSegmentSpecificationLine(whiteSpacedParts));
                }
            }
            headerStreamReader.Close();

            return header;
        }
    }
}

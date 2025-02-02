using Biological_Signal_Processing_Using_AI.WFDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation.CWDNamigs;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.WFDB.Annotations.AnnotationsDefinitions;
using static Biological_Signal_Processing_Using_AI.WFDB.Samples.HeaderDefinitions;
using static Biological_Signal_Processing_Using_AI.WFDB.Samples.SamplesDefinitions;
using static Biological_Signal_Processing_Using_AI.WFDB.WFBDDefinitions;

namespace Biological_Signal_Processing_Using_AI.DetailsModify.ExportToWFDB
{
    public partial class ExportToWFDBForm : Form
    {
        int[] _Samples;
        int _samplingRate;
        int _qunatizationStep;

        AnnotationData _AnnotationData;

        public ExportToWFDBForm(double[] samples, int samplingRate, int quantizationStep, AnnotationData annotationData)
        {
            InitializeComponent();

            _Samples = samples.Select(sample => (int)(sample * quantizationStep)).ToArray();
            _samplingRate = samplingRate;
            _qunatizationStep = quantizationStep;
            _AnnotationData = annotationData;

            signalFormatsComboBox.SelectedIndex = 0;
            annotationFormatsComboBox.SelectedIndex = 0;
            samplingRateTextBox.Text = samplingRate.ToString();
            quantizationStepTextBox.Text = quantizationStep.ToString();
        }

        private string GetValidRecordFileName(string filePath)
        {
            // Get the directory path and file name without extension
            string directoryPath = Path.GetDirectoryName(filePath);
            string fileNameNoExtension = Path.GetFileNameWithoutExtension(filePath);

            // Generate a name for the record
            string recordName = fileNameNoExtension;
            int iNameMod = 1;

            // Check if any file already exists with the given file name
            while (true)
            {
                // Get all files with the same name
                Dictionary<string, string> similarFilePathsDict = System.IO.Directory.EnumerateFiles(directoryPath, recordName + ".*").ToArray().ToDictionary(path => Path.GetExtension(path), path => path);

                if (similarFilePathsDict.Count == 0)
                    break;
                else
                    // If yes then modify on the new file name
                    recordName = fileNameNoExtension + "(" + iNameMod + ")";
                iNameMod++;
            }

            return fileNameNoExtension;
        }

        private static Annotation[] ConvertAnnotationToWFDB(AnnotationData annotationData, int channel)
        {
            // Get point annotation data
            AnnotationECG[] annotationECGArray = annotationData.GetAnnotations().Where(ECGanno => ECGanno.GetAnnotationType() == AnnotationType.Point).ToArray();
            // Build the wfdb annotaion data
            List<Annotation> wfdbAnnotationArray = new List<Annotation>(annotationECGArray.Length);

            foreach (AnnotationECG annoECG in annotationECGArray)
            {
                // Create the info variables of the new anno
                int annoCode = 0;
                int index = annoECG.GetIndexes().starting;
                int number = 0;

                // Check the type of the fiducial point
                if (annoECG.Name.Equals(PeaksLabelsOutputs.POnset))
                    annoCode = AnnotationCodes.NonBeat.WFON;
                else if (annoECG.Name.Equals(PeaksLabelsOutputs.PPeak))
                    annoCode = AnnotationCodes.NonBeat.PWAVE;
                else if (annoECG.Name.Equals(PeaksLabelsOutputs.PEnd))
                    annoCode = AnnotationCodes.NonBeat.WFOFF;
                else if (annoECG.Name.Equals(PeaksLabelsOutputs.QPeak))
                {
                    annoCode = AnnotationCodes.NonBeat.WFON;
                    number = 1;
                }
                else if (annoECG.Name.Equals(PeaksLabelsOutputs.RPeak))
                    annoCode = AnnotationCodes.Beat.NORMAL;
                else if (annoECG.Name.Equals(PeaksLabelsOutputs.SPeak))
                {
                    annoCode = AnnotationCodes.NonBeat.WFOFF;
                    number = 1;
                }
                else if (annoECG.Name.Equals(PeaksLabelsOutputs.TOnset))
                {
                    annoCode = AnnotationCodes.NonBeat.WFON;
                    number = 2;
                }
                else if (annoECG.Name.Equals(PeaksLabelsOutputs.TPeak))
                    annoCode = AnnotationCodes.NonBeat.TWAVE;
                else if (annoECG.Name.Equals(PeaksLabelsOutputs.TEnd))
                {
                    annoCode = AnnotationCodes.NonBeat.WFOFF;
                    number = 2;
                }

                // Create the new anno with its infos
                Annotation newWFDBAnno = AnnotationCodes.AnnotationCodesDict[annoCode].Clone();
                newWFDBAnno.index = index;
                newWFDBAnno.number = number;
                newWFDBAnno.channel = channel;

                wfdbAnnotationArray.Add(newWFDBAnno);
            }

            return wfdbAnnotationArray.ToArray();
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            // Open file dialogue to choose matlab file of a signal
            using (SaveFileDialog sd = new SaveFileDialog() { ValidateNames = true, Filter = "Header files|*.hea|All files|*.*", RestoreDirectory = true, FilterIndex = 1 })
            {
                // Check if the user clicked OK button
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    // Get the path of specified file
                    string filePath = sd.FileName;

                    // Get the directory path
                    string directoryPath = Path.GetDirectoryName(filePath);

                    // Get record and file names
                    string recordName = GetValidRecordFileName(filePath);
                    string fileName = recordName;
                    if (annotationFormatsComboBox.Text.Equals("MIT"))
                        fileName += ".dat";
                    else if (annotationFormatsComboBox.Text.Equals("AHA"))
                        fileName += ".edf";

                    // Generate the WFDBScope for the selected signal data
                    WFDBScope wfdbScope = new WFDBScope();
                    // Generate the header data
                    wfdbScope.Header = new Header()
                    {
                        RecordLineSpecs = new RecordLine() { RecordName = recordName, samplingFreq = _samplingRate, signalsNum = 1, samplesNum = _Samples.Length },

                        SignalLinesList = new List<SignalSpecsLine>()
                        {
                            new SignalSpecsLine() { FileName = fileName, format = int.Parse(signalFormatsComboBox.Text), adcGain = _qunatizationStep, description = descriptionTextBox.Text }
                        }
                    };
                    // Generate the signals dictionary
                    wfdbScope.SignalsDict = new Dictionary<string, Signal>(wfdbScope.Header.RecordLineSpecs.signalsNum);
                    for (int iSignal = 0; iSignal < wfdbScope.Header.SignalLinesList.Count; iSignal++)
                    {
                        SignalSpecsLine signalLine = wfdbScope.Header.SignalLinesList[iSignal];
                        Signal signal = new Signal() { FileName = signalLine.FileName, samplingFreq = _samplingRate, adcGain = signalLine.adcGain, Samples = _Samples, description = signalLine.description };
                        
                        wfdbScope.SignalsDict.Add(signal.FileName + @"\" + iSignal + @"\" + signal.description, signal);
                    }
                    // Generate the annotation
                    wfdbScope.AnnotationsDict = new Dictionary<string, Annotation[]>(wfdbScope.SignalsDict.Count);
                    for (int iSignal = 0; iSignal < wfdbScope.SignalsDict.Count; iSignal++)
                        wfdbScope.AnnotationsDict.Add(".cwd" + iSignal, ConvertAnnotationToWFDB(_AnnotationData, iSignal));

                    // Export the data to WFDB file
                    WFDBWrite.WriteWFDBData(wfdbScope, directoryPath, annotationFormatsComboBox.Text);
                }

                Close();
            }
        }
    }
}

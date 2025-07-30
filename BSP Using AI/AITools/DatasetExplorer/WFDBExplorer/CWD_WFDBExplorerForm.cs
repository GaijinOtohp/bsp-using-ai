using Biological_Signal_Processing_Using_AI.DetailsModify.Annotations;
using Biological_Signal_Processing_Using_AI.Garage;
using Biological_Signal_Processing_Using_AI.WFDB;
using BSP_Using_AI;
using BSP_Using_AI.AITools.DatasetExplorer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation.CWDNamigs;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.WFDB.Annotations.AnnotationsDefinitions;
using static Biological_Signal_Processing_Using_AI.WFDB.WFBDDefinitions;

namespace Biological_Signal_Processing_Using_AI.AITools.DatasetExplorer.WFDBExplorer
{
    public partial class WFDBExplorerForm
    {
        private void CWD_okButton_Click(object sender, EventArgs e)
        {
            int signalIndex = signalsComboBox.SelectedIndex;
            int annoIndex = annotationsComboBox.SelectedIndex;
            bool fixBorders = fixBordersCcheckBox.Checked;
            double signalStart = double.Parse(signalStartTextBox.Text);
            double signalEnd = double.Parse(signalEndTextBox.Text);

            // Show message for confirming the action
            DialogResult dialogResult = MessageBox.Show("Would you like to apply the same action to all files in the selected folder?", "Action confirmation", MessageBoxButtons.YesNo);

            Thread saveSignalsThread = new Thread(() =>
            {
                if (dialogResult == DialogResult.Yes)
                {
                    // Get the path of all headers in the selected folder
                    // Get the directory path and file name without extension
                    string directoryPath = Path.GetDirectoryName(_SelectedFilePath);

                    string[] headerFiles = System.IO.Directory.EnumerateFiles(directoryPath, "*.hea").ToArray();

                    // Include all the signals to the database
                    Invoke(new MethodInvoker(delegate () { savePprogressBar.Maximum = headerFiles.Length; }));
                    List<Thread> saveSigsThreads = new List<Thread>(headerFiles.Length + 1);
                    foreach (string headerPath in headerFiles)
                    {
                        Thread saveSigThread = new Thread(() =>
                        {
                            WFDBScope wfdbScope = WFDBRead.ReadWFDBInfo(headerPath);
                            SaveSignal(signalIndex, annoIndex, signalStart, signalEnd, fixBorders, wfdbScope);

                            Invoke(new MethodInvoker(delegate () { savePprogressBar.Value++; }));
                        });
                        saveSigThread.Start();
                        saveSigsThreads.Add(saveSigThread);
                    }
                    foreach (Thread t in saveSigsThreads)
                        t.Join();
                }
                else
                    SaveSignal(signalIndex, annoIndex, signalStart, signalEnd, fixBorders, _WFDBScope);

                Invoke(new MethodInvoker(delegate () { Close(); }));
            });
            saveSignalsThread.Start();
        }

        private void SaveSignal(int signalIndex, int annoIndex, double signalStart, double signalEnd, bool fixBorders, WFDBScope wfdbScope)
        {
            // Get the key of the selected signal and annotation
            string signalKey = wfdbScope.SignalsDict.Keys.ToList()[signalIndex];
            string annoKey = wfdbScope.AnnotationsDict.Keys.ToList()[annoIndex];
            // Get the starting and ending indexes of the selected signal
            int startingIndex = (int)(signalStart * wfdbScope.SignalsDict[signalKey].samplingFreq);
            int endingIndex = (int)(signalEnd * wfdbScope.SignalsDict[signalKey].samplingFreq);
            double signalSpanInSec = (endingIndex - startingIndex) / wfdbScope.SignalsDict[signalKey].samplingFreq;

            // Build the annotation data from the selected wfdb annotation
            AnnotationData annotationData = new AnnotationData(CharacteristicWavesDelineation.ObjectiveName);
            int[] wfdbBeatCodes = AnnotationCodes.Beat.GetBeatCodes();
            int pEdgeNum = 0;
            int qrsEdgeNum = 1;
            int tEdgeNum = 2;
            foreach (Annotation anno in wfdbScope.AnnotationsDict[annoKey])
            {
                string annoLabel = "";
                if (anno.codeValue == AnnotationCodes.NonBeat.WFON)
                {
                    if (anno.number == pEdgeNum)
                        annoLabel = PeaksLabelsOutputs.POnset;
                    else if (anno.number == qrsEdgeNum)
                        annoLabel = PeaksLabelsOutputs.QPeak;
                    else if (anno.number == tEdgeNum)
                        annoLabel = PeaksLabelsOutputs.TOnset;
                }
                else if (anno.codeValue == AnnotationCodes.NonBeat.WFOFF)
                {
                    if (anno.number == pEdgeNum)
                        annoLabel = PeaksLabelsOutputs.PEnd;
                    else if (anno.number == qrsEdgeNum)
                        annoLabel = PeaksLabelsOutputs.SPeak;
                    else if (anno.number == tEdgeNum)
                        annoLabel = PeaksLabelsOutputs.TEnd;
                }
                else if (anno.codeValue == AnnotationCodes.NonBeat.PWAVE)
                    annoLabel = PeaksLabelsOutputs.PPeak;
                else if (anno.codeValue == AnnotationCodes.NonBeat.TWAVE)
                    annoLabel = PeaksLabelsOutputs.TPeak;
                else if (wfdbBeatCodes.Contains(anno.codeValue))
                    annoLabel = PeaksLabelsOutputs.RPeak;

                if (!annoLabel.Equals("") && startingIndex < anno.index && anno.index < endingIndex)
                    annotationData.InsertAnnotation(annoLabel, AnnotationType.Point, anno.index - startingIndex, 0);
            }

            // Sort the rest of the signal infos
            string description = wfdbScope.SignalsDict[signalKey].description;
            string signalName = description + "\\" + signalKey + "\\" + signalSpanInSec;
            double startingIndexInSec = startingIndex / wfdbScope.SignalsDict[signalKey].samplingFreq;
            double[] signalData = wfdbScope.SignalsDict[signalKey].Samples.Where((value, index) => startingIndex <= index && index < endingIndex).Select(value => (double)value).ToArray();
            double samplingRate = wfdbScope.SignalsDict[signalKey].samplingFreq;
            double quantisationStep = wfdbScope.SignalsDict[signalKey].adcGain;

            // Fix borders annotation if requested
            if (fixBorders)
                annotationData = AnnotationTools.FixBordersAnnotation(annotationData);

            // Save the signal with its features in dataset
            DbStimulator dbStimulator = new DbStimulator();
            long id = dbStimulator.Insert("anno_ds",
                new string[] { "sginal_name", "starting_index", "signal_data", "sampling_rate", "quantisation_step", "anno_objective", "anno_data" },
                new Object[] { signalName, startingIndexInSec, GeneralTools.ObjectToByteArray(signalData), samplingRate,
                               quantisationStep, CharacteristicWavesDelineation.ObjectiveName, GeneralTools.ObjectToByteArray(annotationData) }, "CWD_WFDBExplorerForm");

            // Create an item of the signal features
            DatasetFlowLayoutPanelItemUserControl datasetFlowLayoutPanelItemUserControl = new DatasetFlowLayoutPanelItemUserControl();
            datasetFlowLayoutPanelItemUserControl.signalNameLabel.Text = signalName;
            datasetFlowLayoutPanelItemUserControl.startingIndexLabel.Text = startingIndexInSec.ToString();
            datasetFlowLayoutPanelItemUserControl.samplingRateLabel.Text = samplingRate.ToString();
            datasetFlowLayoutPanelItemUserControl.quantizationStepLabel.Text = quantisationStep.ToString();
            datasetFlowLayoutPanelItemUserControl._Table = "anno_ds";
            datasetFlowLayoutPanelItemUserControl._id = id;

            Invoke(new MethodInvoker(delegate () { _DatasetExplorerForm.signalsFlowLayoutPanel.Controls.Add(datasetFlowLayoutPanelItemUserControl); }));
        }
    }
}

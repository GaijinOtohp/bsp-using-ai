using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI;
using BSP_Using_AI.AITools.DatasetExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation.CWDNamigs;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.WFDB.Annotations.AnnotationsDefinitions;

namespace Biological_Signal_Processing_Using_AI.AITools.DatasetExplorer.WFDBExplorer
{
    public partial class WFDBExplorerForm
    {
        private void CWD_okButton_Click(object sender, EventArgs e)
        {
            // Get the starting and ending indexes of the selected signal
            int startingIndex = (int)(double.Parse(signalStartTextBox.Text) * _WFDBScope.SignalsDict[signalsComboBox.Text].samplingFreq);
            int endingIndex = (int)(double.Parse(signalEndTextBox.Text) * _WFDBScope.SignalsDict[signalsComboBox.Text].samplingFreq);
            double signalSpanInSec = (endingIndex - startingIndex) / _WFDBScope.SignalsDict[signalsComboBox.Text].samplingFreq;

            // Build the annotation data from the selected wfdb annotation
            AnnotationData annotationData = new AnnotationData(CharacteristicWavesDelineation.ObjectiveName);
            int[] wfdbBeatCodes = AnnotationCodes.Beat.GetBeatCodes();
            int pEdgeNum = 0;
            int qrsEdgeNum = 1;
            int tEdgeNum = 2;
            foreach (Annotation anno in _WFDBScope.AnnotationsDict[annotationsComboBox.Text])
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
            string signalName = signalsComboBox.Text + "\\" + signalSpanInSec;
            double startingIndexInSec = startingIndex / _WFDBScope.SignalsDict[signalsComboBox.Text].samplingFreq;
            double[] signalData = _WFDBScope.SignalsDict[signalsComboBox.Text].Samples.Where((value, index) => startingIndex <= index && index < endingIndex).Select(value => (double)value).ToArray();
            double samplingRate = _WFDBScope.SignalsDict[signalsComboBox.Text].samplingFreq;
            double quantisationStep = _WFDBScope.SignalsDict[signalsComboBox.Text].adcGain;

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

            _DatasetExplorerForm.signalsFlowLayoutPanel.Controls.Add(datasetFlowLayoutPanelItemUserControl);

            Close();
        }
    }
}

using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.AITools.RL_Objectives;
using Biological_Signal_Processing_Using_AI.DetailsModify.Annotations;
using Biological_Signal_Processing_Using_AI.Garage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.ReinforcementLearning.Environment;
using static Biological_Signal_Processing_Using_AI.AITools.RL_Objectives.CWD_RL;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Filters.CornersScanner;

namespace BSP_Using_AI.DetailsModify
{
    public partial class FormDetailsModify
    {
        public static (List<CornerSample> ScannedCorners, List<SignalSegment> SegmentsList) RLAutoCornersScanner(TFNETReinforcementL CWDReinforcementLModel, double[] SignalSamples, int samplingRate)
        {
            // Rescale samples to be in an amplitude interval of 4
            double globalAmpInterval = 4d;
            double[] RescaledSamples = GeneralTools.rescaleSignal(SignalSamples, globalAmpInterval);

            List<SignalSegment> SegmentsList = CWD_RL.SegmentTheMainSamples(RescaledSamples, samplingRate, 0.5d, null);

            List<CornerSample> scannedCornersList = new List<CornerSample>(SignalSamples.Length);

            foreach (SignalSegment Segment in SegmentsList)
            {
                double[] features = GetFeaturesValues(Segment);

                double[] atARTOutput = TF_NET_NN.predict(features, CWDReinforcementLModel, CWDReinforcementLModel.BaseModel.Session);

                List<RLDimension> dimList = CWDReinforcementLModel._DimensionsList;
                double at = dimList[0]._min + (atARTOutput[0] * (dimList[0]._max - dimList[0]._min));
                double art = dimList[1]._min + (atARTOutput[1] * (dimList[1]._max - dimList[1]._min));
                List<CornerSample> TempCornersList = ScanCorners(Segment.SegmentSamples, Segment.startingIndex, samplingRate, art, at);

                scannedCornersList.AddRange(TempCornersList);
            }

            return (scannedCornersList.DistinctBy(corner => corner._index).OrderBy(corner => corner._index).ToList(), SegmentsList);
        }

        private void predictButton_Click_CWDRL(string modelNameProblem)
        {
            // Get the selected model
            CWDReinforcementL cwdReinforcementL = (CWDReinforcementL)_objectivesModelsDic[modelNameProblem];
            TFNETReinforcementL CWDReinforcementLModel = cwdReinforcementL.CWDReinforcementLModel;

            // Scan for the corners using the selected deep Q-learning model
            (List<CornerSample> ScannedCorners, List<SignalSegment> SegmentsList) = RLAutoCornersScanner(CWDReinforcementLModel, _FilteringTools._FilteredSamples, _FilteringTools._samplingRate);

            // Create the annotation data
            _AnnotationData = new AnnotationData(CharacteristicWavesDelineation.ObjectiveName);
            foreach (CornerSample corner in ScannedCorners)
                _AnnotationData.InsertAnnotation("", AnnotationType.Point, corner._index, 0);

            // Show the annotation in the chart
            UpdatePointsAnnoPlot();

            // Show segment spans as annotation data
            List<AnnotationECG> SpansAnnoList = new List<AnnotationECG>(SegmentsList.Count);
            for (int iSegment = 0; iSegment < SegmentsList.Count; iSegment++)
                SpansAnnoList.Add(new AnnotationECG("seg" + iSegment, AnnotationType.Interval, SegmentsList[iSegment].startingIndex, SegmentsList[iSegment].endingIndex, _AnnotationData));
            foreach (AnnotationECG annoECG in SpansAnnoList)
                // Create the new annotation item and add it in featuresTableLayoutPanel
                featuresTableLayoutPanel.Controls.Add(new AnnotationItemUserControl(annoECG, this));

            predictionEnd();
        }
    }
}

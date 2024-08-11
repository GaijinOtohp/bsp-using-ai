using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.AITools.RL_Objectives;
using Biological_Signal_Processing_Using_AI.DetailsModify.Annotations;
using Biological_Signal_Processing_Using_AI.Garage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.RL_Objectives.CWD_RL;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Filters.CornersScanner;

namespace BSP_Using_AI.DetailsModify
{
    public partial class FormDetailsModify
    {
        private (List<CornerSample> ScannedCorners, List<SignalSegment> SegmentsList) RLAutoCornersScanner(TFNETReinforcementL CWDReinforcementLModel)
        {
            // Get the signal and segment it using the CWD_RL segmentation method
            int samplingRate = _FilteringTools._samplingRate;
            double[] SignalSamples = _FilteringTools._FilteredSamples;

            // Rescale samples to be in an amplitude interval of 4
            double globalAmpInterval = 4d;
            double[] RescaledSamples = GeneralTools.rescaleSignal(SignalSamples, globalAmpInterval);

            List<SignalSegment> SegmentsList = CWD_RL.SegmentTheMainSamples(RescaledSamples, samplingRate, 0.002d, 0.5d);

            Dictionary<int, CornerSample> scannedCornersDict = new Dictionary<int, CornerSample>(SignalSamples.Length);

            foreach (SignalSegment Segment in SegmentsList)
            {
                (double globalMean, double globalStdDev, double globalIQR, double segmentMean,
                     double segmentMin, double segmentMax, double segmentStdDev, double segmentIQR) = GetFeaturesValues(RescaledSamples, Segment);
                double[] features = new double[] { globalMean, globalStdDev, globalIQR, segmentMean, segmentMin, segmentMax, segmentStdDev, segmentIQR };

                double[] atARTOutput = TF_NET_NN.predict(features, CWDReinforcementLModel, CWDReinforcementLModel.BaseModel.Session);

                List<CornerSample> TempCornersList = ScanCorners(Segment.SegmentSamples, Segment.startingIndex, samplingRate, atARTOutput[1], atARTOutput[0] * 360d);

                // Copy the new scanned corners to scannedCornersIndecies and as annotation
                foreach (CornerSample CornerSample in TempCornersList)
                    if (!scannedCornersDict.ContainsKey(CornerSample._index))
                        scannedCornersDict.Add(CornerSample._index, CornerSample);
            }

            return (scannedCornersDict.OrderBy(keyVal => keyVal.Key).Select(keyVal => keyVal.Value).ToList(), SegmentsList);
        }

        private void predictButton_Click_CWDRL(string modelNameProblem)
        {
            // Get the selected model
            CWDReinforcementL cwdReinforcementL = (CWDReinforcementL)_objectivesModelsDic[modelNameProblem];
            TFNETReinforcementL CWDReinforcementLModel = cwdReinforcementL.CWDReinforcementLModel;

            // Scan for the corners using the selected deep Q-learning model
            (List<CornerSample> ScannedCorners, List<SignalSegment> SegmentsList) = RLAutoCornersScanner(CWDReinforcementLModel);

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

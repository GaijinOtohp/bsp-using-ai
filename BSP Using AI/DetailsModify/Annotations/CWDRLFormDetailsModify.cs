using Biological_Signal_Processing_Using_AI.AITools;
using Biological_Signal_Processing_Using_AI.AITools.RL_Objectives;
using Biological_Signal_Processing_Using_AI.DetailsModify.Annotations;
using Biological_Signal_Processing_Using_AI.Garage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        private class RLAutoScannerVars
        {
            public List<AnnotationECG> SegSpansList;
        }

        private RLAutoScannerVars _RLScannGlobVars;

        public static List<CornerSample> ScanSegmentCorners(SignalSegment Segment, int samplingRate, TFNETReinforcementL CWDReinforcementLModel)
        {
            double[] features = GetFeaturesValues(Segment);

            double[] atARTOutput = null;
            lock (CWDReinforcementLModel)
                atARTOutput = TF_NET_NN.predict(features, CWDReinforcementLModel, CWDReinforcementLModel.BaseModel.Session);

            List<RLDimension> dimList = CWDReinforcementLModel._DimensionsList;
            double at = dimList[0]._min + (atARTOutput[0] * (dimList[0]._max - dimList[0]._min));
            double art = dimList[1]._min + (atARTOutput[1] * (dimList[1]._max - dimList[1]._min));
            List<CornerSample> TempCornersList = ScanCorners(Segment.SegmentSamples, Segment.startingIndex, samplingRate, art, at);

            return TempCornersList;
        }

        public static (List<CornerSample> ScannedCorners, List<SignalSegment> SegmentsList) RLAutoCornersScanner_Training(TFNETReinforcementL CWDReinforcementLModel, double[] SignalSamples, int samplingRate)
        {
            // Rescale samples to be in an amplitude interval of 4
            double globalAmpInterval = 4d;
            double[] RescaledSamples = GeneralTools.rescaleSignal(SignalSamples, globalAmpInterval);

            List<SignalSegment> SegmentsList = CWD_RL.SegmentTheMainSamples(RescaledSamples, samplingRate, 0.5d, null, null);

            List<CornerSample> scannedCornersList = new List<CornerSample>(SignalSamples.Length);

            foreach (SignalSegment Segment in SegmentsList)
            {
                List<CornerSample> TempCornersList = ScanSegmentCorners(Segment, samplingRate, CWDReinforcementLModel);

                scannedCornersList.AddRange(TempCornersList);
            }

            return (scannedCornersList.DistinctBy(corner => corner._index).OrderBy(corner => corner._index).ToList(), SegmentsList);
        }

        private List<SignalSegment> RLAutoCornersScanner_Prediction(double[] SignalSamples, int samplingRate, ObjectiveBaseModel baseModel, SegmentDelegate segmentDelegate)
        {
            // Rescale samples to be in an amplitude interval of 4
            double globalAmpInterval = 4d;
            double[] RescaledSamples = GeneralTools.rescaleSignal(SignalSamples, globalAmpInterval);

            List<SignalSegment> SegmentsList = CWD_RL.SegmentTheMainSamples(RescaledSamples, samplingRate, 0.5d, baseModel, segmentDelegate);

            return SegmentsList;
        }

        private void RLAutoCornersScanner_SegmentDelegate(SignalSegment segment, int segmentCount, bool lastSegment, ObjectiveBaseModel baseModel)
        {
            // Get the selected model
            TFNETReinforcementL CWDReinforcementLModel = ((CWDReinforcementL)baseModel).CWDReinforcementLModel;

            // Scan the corners of the segment
            List<CornerSample> ScannedCorners = ScanSegmentCorners(segment, _FilteringTools._samplingRate, CWDReinforcementLModel);

            // Plot the scanned corners sequentially of the current segment
            try
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    // Show the annotation in the chart
                    foreach (CornerSample corner in ScannedCorners)
                        _AnnotationData.InsertAnnotation("", AnnotationType.Point, corner._index, 0);
                    UpdatePointsAnnoPlot();

                    // Create the segment span as annotation data
                    _RLScannGlobVars.SegSpansList.Add(new AnnotationECG("seg" + segmentCount, AnnotationType.Interval, segment.startingIndex, segment.endingIndex, _AnnotationData));

                    if (lastSegment)
                        predictionEnd();
                }));
            }
            catch (Exception e) { }
        }

        private void predictButton_Click_CWDRL(string modelNameProblem)
        {
            Thread rlPredictionThread = new Thread(() =>
            {
                // Get the selected model
                CWDReinforcementL cwdReinforcementL = (CWDReinforcementL)_objectivesModelsDic[modelNameProblem];

                // Create the annotation data
                _AnnotationData = new AnnotationData(CharacteristicWavesDelineation.ObjectiveName);

                // Initialize the global variables
                _RLScannGlobVars = new RLAutoScannerVars();
                _RLScannGlobVars.SegSpansList = new List<AnnotationECG>();

                // Scan for the corners using the selected deep Q-learning model
                RLAutoCornersScanner_Prediction(_FilteringTools._FilteredSamples, _FilteringTools._samplingRate, cwdReinforcementL, RLAutoCornersScanner_SegmentDelegate);

                // Show segment spans as annotation data
                try
                {
                    foreach (AnnotationECG annoECG in _RLScannGlobVars.SegSpansList)
                    {
                        // Create the new annotation item and add it in featuresTableLayoutPanel
                        this.Invoke(new MethodInvoker(delegate () { featuresTableLayoutPanel.Controls.Add(new AnnotationItemUserControl(annoECG, this)); }));
                        if (featuresTableLayoutPanel.Controls.Count % 30 == 0)
                            Thread.Sleep(100);
                    }
                }
                catch (Exception e) { }
            });
            rlPredictionThread.Start();
        }
    }
}

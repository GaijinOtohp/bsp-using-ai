using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives
{
    public partial class AIModels_ObjectivesArchitectures
    {
        public class WPWSyndromeDetection
        {
            public static string ObjectiveName = "WPW syndrome detection";

            //_______________________________________________________//
            //::::::::::::::::::::::ARTHT models::::::::::::::::::::://
            public class ARTHTModels : ObjectiveBaseModel
            {
                [DataMember]
                public Dictionary<string, CustomArchiBaseModel> ARTHTModelsDic = new Dictionary<string, CustomArchiBaseModel>(7)
                {
                    { ARTHTNamings.Step1RPeaksScanData, new CustomArchiBaseModel(0, 0, null) },
                    { ARTHTNamings.Step2RPeaksSelectionData, new CustomArchiBaseModel(0, 0, null) },
                    { ARTHTNamings.Step3BeatPeaksScanData, new CustomArchiBaseModel(0, 0, null) },
                    { ARTHTNamings.Step4PTSelectionData, new CustomArchiBaseModel(0, 0, null) },
                    { ARTHTNamings.Step5ShortPRScanData, new CustomArchiBaseModel(0, 0, null) },
                    { ARTHTNamings.Step6UpstrokesScanData, new CustomArchiBaseModel(0, 0, null) },
                    { ARTHTNamings.Step7DeltaExaminationData, new CustomArchiBaseModel(0, 0, null) },
                };

                protected override ObjectiveBaseModel CreateCloneInstance()
                {
                    return new ARTHTModels();
                }
                public override ObjectiveBaseModel Clone()
                {
                    ARTHTModels aRTHTModels = (ARTHTModels)base.Clone();

                    foreach (string stepName in ARTHTModelsDic.Keys)
                        // Each AI model has its own Clone method, and each Clone method returns a different AI model
                        // Then the Clone method should be Invoked from the object of the AI model type
                        aRTHTModels.ARTHTModelsDic[stepName] = ARTHTModelsDic[stepName].Clone();

                    return aRTHTModels;
                }
            }
            //_______________________________________________________//
            //:::::::::::::Steps input/output dimensions::::::::::::://
            public static (int inputDim, int outputDim, string[] outputNames, List<PCAitem> PCA) GetStepDimensions(string stepName, List<Sample> dataList, bool pcaActive)
            {
                // Compute PCA loading scores if PCA is active
                List<PCAitem> pca = new List<PCAitem>();
                if (pcaActive)
                    // If yes then compute PCA loading scores
                    pca = DataVisualisationForm.getPCA(dataList);
                int input = pca.Count > 0 ? pca.Count : 0;
                int output;
                string[] outputNames;

                if (stepName.Equals(ARTHTNamings.Step1RPeaksScanData))
                {
                    if (input == 0) input = 15;
                    outputNames = ARTHTNamings.PeaksScannerOutputs.GetNames();
                }
                else if (stepName.Equals(ARTHTNamings.Step2RPeaksSelectionData))
                {
                    if (input == 0) input = 2;
                    outputNames = ARTHTNamings.RSelectionOutputs.GetNames();
                }
                else if (stepName.Equals(ARTHTNamings.Step3BeatPeaksScanData))
                {
                    if (input == 0) input = 5;
                    outputNames = ARTHTNamings.PeaksScannerOutputs.GetNames();
                }
                else if (stepName.Equals(ARTHTNamings.Step4PTSelectionData))
                {
                    if (input == 0) input = 3;
                    outputNames = ARTHTNamings.PTSelectionOutputs.GetNames();
                }
                else if (stepName.Equals(ARTHTNamings.Step5ShortPRScanData))
                {
                    if (input == 0) input = 1;
                    outputNames = ARTHTNamings.ShortPROutputs.GetNames();
                }
                else if (stepName.Equals(ARTHTNamings.Step6UpstrokesScanData))
                {
                    if (input == 0) input = 6;
                    outputNames = ARTHTNamings.UpStrokeOutputs.GetNames();
                }
                else
                {
                    if (input == 0) input = 6;
                    outputNames = ARTHTNamings.DeltaExamOutputs.GetNames();
                }

                if (stepName.Equals(ARTHTNamings.Step1RPeaksScanData) || stepName.Equals(ARTHTNamings.Step3BeatPeaksScanData) || stepName.Equals(ARTHTNamings.Step4PTSelectionData))
                    output = 2;
                else
                    output = 1;

                return (input, output, outputNames, pca);
            }

            [Serializable]
            [DataContract(Name = "Structures.ARTHTFeatures", IsReference = true, Namespace = "http://schemas.datacontract.org/2004/07/Biological_Signal_Processing_Using_AI")]
            public class ARTHTFeatures
            {
                [DataMember]
                public int _processedStep { get; set; } = 0;

                [DataMember]
                public List<Beat> SignalBeats { get; set; } = new List<Beat>();

                [DataMember]
                public Dictionary<string, Data> StepsDataDic { get; set; } = new Dictionary<string, Data>(7)
            {
                {ARTHTNamings.Step1RPeaksScanData, new Data(ARTHTNamings.Step1RPeaksScanData) },
                {ARTHTNamings.Step2RPeaksSelectionData, new Data(ARTHTNamings.Step2RPeaksSelectionData) },
                {ARTHTNamings.Step3BeatPeaksScanData, new Data(ARTHTNamings.Step3BeatPeaksScanData) },
                {ARTHTNamings.Step4PTSelectionData, new Data(ARTHTNamings.Step4PTSelectionData) },
                {ARTHTNamings.Step5ShortPRScanData, new Data(ARTHTNamings.Step5ShortPRScanData) },
                {ARTHTNamings.Step6UpstrokesScanData, new Data(ARTHTNamings.Step6UpstrokesScanData) },
                {ARTHTNamings.Step7DeltaExaminationData, new Data(ARTHTNamings.Step7DeltaExaminationData) }
            };

                public void Clear()
                {
                    _processedStep = 0;
                    SignalBeats.Clear();
                    StepsDataDic[ARTHTNamings.Step1RPeaksScanData].Clear();
                    StepsDataDic[ARTHTNamings.Step2RPeaksSelectionData].Clear();
                    StepsDataDic[ARTHTNamings.Step3BeatPeaksScanData].Clear();
                    StepsDataDic[ARTHTNamings.Step4PTSelectionData].Clear();
                    StepsDataDic[ARTHTNamings.Step5ShortPRScanData].Clear();
                    StepsDataDic[ARTHTNamings.Step6UpstrokesScanData].Clear();
                    StepsDataDic[ARTHTNamings.Step7DeltaExaminationData].Clear();
                }
            }
            public class ARTHTNamings
            {
                public static string Features = "Features";
                public static string Outputs = "Outputs";

                public static string Step1RPeaksScanData { get; } = "R-peaks scan";
                public static string Step2RPeaksSelectionData { get; } = "R-peaks selection";
                public static string Step3BeatPeaksScanData { get; } = "Beat peaks scan";
                public static string Step4PTSelectionData { get; } = "P and T selection";
                public static string Step5ShortPRScanData { get; } = "Short PR scan";
                public static string Step6UpstrokesScanData { get; } = "Upstrokes scan";
                public static string Step7DeltaExaminationData { get; } = "Delta examination";

                public static string Mean = "mean";
                public static string Min = "min";
                public static string Max = "max";
                public static string StdDev = "STD_DEV";
                public static string IQR = "IQR";

                public class PeaksScannerOutputs
                {
                    public static string ART = "ART"; // Amplitude Ratio Threshold
                    public static string HT = "HT"; // Horizontal Threshold

                    public static string[] GetNames()
                    {
                        return typeof(PeaksScannerOutputs).GetFields().Select(field => (string)field.GetValue(null)).ToArray();
                    }
                }


                public static string RIntrvl = "RpRcur/RRav";
                public static string RAmp = "ampRcur/ampRp";

                public class RSelectionOutputs
                {
                    public static string RemoveR = "Remove R";

                    public static string[] GetNames()
                    {
                        return typeof(RSelectionOutputs).GetFields().Select(field => (string)field.GetValue(null)).ToArray();
                    }
                }


                public static string Beat = "Beat";


                public static string State = "State";

                public static string Stx = "Stx";
                public static string StRIntrvl = "(Stx - Rk) / (Rk - Rk-1)";
                public static string StAmp = "((ampStx - ampStx-1) + (ampStx - ampStx+1)) / 2";

                public class PTSelectionOutputs
                {
                    public static string PWave = "P wave";
                    public static string TWave = "T wave";

                    public static string[] GetNames()
                    {
                        return typeof(PTSelectionOutputs).GetFields().Select(field => (string)field.GetValue(null)).ToArray();
                    }
                }


                public static string PQIntrvl = "(Q - P) / (R - P)";

                public class ShortPROutputs
                {
                    public static string ShortPR = "Short PR";

                    public static string[] GetNames()
                    {
                        return typeof(ShortPROutputs).GetFields().Select(field => (string)field.GetValue(null)).ToArray();
                    }
                }


                public class UpStrokeOutputs
                {
                    public static string TDT = "TDT"; // Tangent Deviation Threshold

                    public static string[] GetNames()
                    {
                        return typeof(UpStrokeOutputs).GetFields().Select(field => (string)field.GetValue(null)).ToArray();
                    }
                }


                public static string DeltaAmp = "(ampDelta - ampQ) / (ampR - ampQ)";

                public class DeltaExamOutputs
                {
                    public static string WPWPattern = "WPW pattern";

                    public static string[] GetNames()
                    {
                        return typeof(DeltaExamOutputs).GetFields().Select(field => (string)field.GetValue(null)).ToArray();
                    }
                }
            }
        }
    }
}

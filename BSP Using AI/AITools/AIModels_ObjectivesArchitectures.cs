using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace Biological_Signal_Processing_Using_AI.AITools
{
    public class IdInterval
    {
        [DataMember]
        public long starting;
        [DataMember]
        public long ending;

        public IdInterval Clone()
        {
            IdInterval clonedIdInterval = new IdInterval();
            clonedIdInterval.starting = starting;
            clonedIdInterval.ending = ending;
            return clonedIdInterval;
        }

        public static bool operator ==(IdInterval leftIntervalItem, IdInterval rightIntervalItem)
        {
            bool status = false;
            if (leftIntervalItem.starting == rightIntervalItem.starting && leftIntervalItem.ending == rightIntervalItem.ending)
                status = true;
            return status;
        }
        public static bool operator !=(IdInterval leftIntervalItem, IdInterval rightIntervalItem)
        {
            bool status = false;
            if (leftIntervalItem.starting != rightIntervalItem.starting || leftIntervalItem.ending != rightIntervalItem.ending)
                status = true;
            return status;
        }
    }

    public class AIModels_ObjectivesArchitectures
    {
        [Serializable]
        [KnownType(typeof(CustomArchiBaseModel))]
        [KnownType(typeof(KNNModel))]
        [KnownType(typeof(KerasNETNeuralNetworkModel))]
        [KnownType(typeof(NaiveBayesModel))]
        [KnownType(typeof(TFNETNeuralNetworkModel))]
        [KnownType(typeof(TFKerasNeuralNetworkModel))]
        [KnownType(typeof(ARTHTModels))]
        [KnownType(typeof(CWDReinforcementL))]
        [KnownType(typeof(CWDLSTM))]
        [DataContract(IsReference = true)]
        public class ObjectiveBaseModel
        {
            [DataMember]
            public string ModelName { get; set; }
            [DataMember]
            public string ObjectiveName { get; set; }
            /// <summary>
            /// Training Details:
            /// Each train update creates a list of intervals (List<long[]>) of the _ids of the selected data
            /// </summary>
            [DataMember]
            public List<List<IdInterval>> DataIdsIntervalsList { get; set; } = new List<List<IdInterval>>();

            [DataMember]
            public long _validationTimeCompelxity { get; set; }
            [DataMember]
            public string _ValidationInfo { get; set; }

            protected virtual ObjectiveBaseModel CreateCloneInstance()
            {
                return new ObjectiveBaseModel();
            }
            public virtual ObjectiveBaseModel Clone()
            {
                ObjectiveBaseModel baseModelClone = CreateCloneInstance();
                baseModelClone.ModelName = ModelName;
                baseModelClone.ObjectiveName = ObjectiveName;
                baseModelClone._validationTimeCompelxity = _validationTimeCompelxity;
                baseModelClone._ValidationInfo = _ValidationInfo;

                baseModelClone.DataIdsIntervalsList = new List<List<IdInterval>>();
                foreach (List<IdInterval> clonedUpdateIntervals in DataIdsIntervalsList)
                {
                    List<IdInterval> updateIntervals = new List<IdInterval>();
                    for (int i = 0; i < clonedUpdateIntervals.Count; i++)
                        updateIntervals.Add(clonedUpdateIntervals[i].Clone());
                    baseModelClone.DataIdsIntervalsList.Add(updateIntervals);
                }

                return baseModelClone;
            }
        }
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
                    { ARTHTNamings.Step1RPeaksScanData, new CustomArchiBaseModel() },
                    { ARTHTNamings.Step2RPeaksSelectionData, new CustomArchiBaseModel() },
                    { ARTHTNamings.Step3BeatPeaksScanData, new CustomArchiBaseModel() },
                    { ARTHTNamings.Step4PTSelectionData, new CustomArchiBaseModel() },
                    { ARTHTNamings.Step5ShortPRScanData, new CustomArchiBaseModel() },
                    { ARTHTNamings.Step6UpstrokesScanData, new CustomArchiBaseModel() },
                    { ARTHTNamings.Step7DeltaExaminationData, new CustomArchiBaseModel() },
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
            public static (int inputDim, int outputDim, List<PCAitem> PCA) GetStepDimensions(string stepName, List<Sample> dataList, bool pcaActive)
            {
                // Compute PCA loading scores if PCA is active
                List<PCAitem> pca = new List<PCAitem>();
                if (pcaActive)
                    // If yes then compute PCA loading scores
                    pca = DataVisualisationForm.getPCA(dataList);
                int input = pca.Count > 0 ? pca.Count : 0;
                int output;

                if (stepName.Equals(ARTHTNamings.Step1RPeaksScanData))
                {
                    if (input == 0) input = 15;
                    output = 2;
                }
                else if (stepName.Equals(ARTHTNamings.Step2RPeaksSelectionData))
                {
                    if (input == 0) input = 2;
                    output = 1;
                }
                else if (stepName.Equals(ARTHTNamings.Step3BeatPeaksScanData))
                {
                    if (input == 0) input = 5;
                    output = 2;
                }
                else if (stepName.Equals(ARTHTNamings.Step4PTSelectionData))
                {
                    if (input == 0) input = 3;
                    output = 2;
                }
                else if (stepName.Equals(ARTHTNamings.Step5ShortPRScanData))
                {
                    if (input == 0) input = 1;
                    output = 1;
                }
                else // For upstroke scan and delta examination
                {
                    if (input == 0) input = 6;
                    output = 1;
                }

                return (input, output, pca);
            }
        }

        public class CharacteristicWavesDelineation
        {
            public static string ObjectiveName = "Characteristic waves delineation";

            //_______________________________________________________//
            //::::::::::::::::::::::CWD models::::::::::::::::::::://
            public class CWDReinforcementL : ObjectiveBaseModel
            {
                [DataMember]
                public TFNETReinforcementL CWDReinforcementLModel = new TFNETReinforcementL("", 0, 0, null);
                [DataMember]
                public TFNETReinforcementL CWDCrazyReinforcementLModel = new TFNETReinforcementL("", 0, 0, null);

                protected override ObjectiveBaseModel CreateCloneInstance()
                {
                    return new CWDReinforcementL();
                }
                public override ObjectiveBaseModel Clone()
                {
                    CWDReinforcementL cwdReinforcementL = (CWDReinforcementL)base.Clone();

                    cwdReinforcementL.CWDReinforcementLModel = (TFNETReinforcementL)CWDReinforcementLModel.Clone();
                    cwdReinforcementL.CWDCrazyReinforcementLModel = (TFNETReinforcementL)CWDCrazyReinforcementLModel.Clone();

                    return cwdReinforcementL;
                }
            }

            public class CWDLSTM : ObjectiveBaseModel
            {
                [DataMember]
                public TFNETReinforcementL CWDReinforcementLModel = new TFNETReinforcementL("", 0, 0, null);
                [DataMember]
                public TFNETReinforcementL CWDCrazyReinforcementLModel = new TFNETReinforcementL("", 0, 0, null);
                [DataMember]
                public TFNETLSTMModel CWDLSTMModel = new TFNETLSTMModel("", 0, 0, 0, layers: 0);

                protected override ObjectiveBaseModel CreateCloneInstance()
                {
                    return new CWDLSTM();
                }
                public override ObjectiveBaseModel Clone()
                {
                    CWDLSTM cwdLSTM = (CWDLSTM)base.Clone();

                    cwdLSTM.CWDReinforcementLModel = (TFNETReinforcementL)CWDReinforcementLModel.Clone();
                    cwdLSTM.CWDCrazyReinforcementLModel = (TFNETReinforcementL)CWDCrazyReinforcementLModel.Clone();
                    cwdLSTM.CWDLSTMModel = (TFNETLSTMModel)CWDLSTMModel.Clone();

                    return cwdLSTM;
                }
            }

            public class CWDNamigs
            {
                public static string RLCornersScanData = "Corners scan";
                public static string LSTMPeaksClassificationData = "Peaks classification";

                public static string GlobalMean = "global mean";
                public static string GlobalStdDev = "global STD_DEV";
                public static string GlobalIQR = "global IQR";
                public static string SegmentMin = "segment min";
                public static string SegmentMax = "segment max";
                public static string SegmentMean = "segment mean";
                public static string SegmentStdDev = "segment STD_DEV";
                public static string SegmentIQR = "segment IQR";

                public static string AT = "AT"; // Angle Threshold
                public static string ART = "ART"; // Amplitude Ratio Threshold

                public static string NextArgTanRatio = "ArgTan to next range peaks";
                public static string PrevArgTanRatio = "ArgTan to previous range peaks";
                public static string PPIntervRatio = "PP interval ratio";
                public static string RRIntervRatio = "RR interval ratio";
                public static string SegmPeakAmpRatio = "Segment peak amplitude ratio";
                public static string GlobPeakAmpRatio = "Global peak amplitude ratio";
                public static string SegmGlobAmpRatio = "Segment to global amplitude interval ratio";
                public static string NextHypotAvToGlob = "Hypotenuse to next range peaks global ratio";
                public static string PrevHypotAvToGlob = "Hypotenuse to previous range peaks global ratio";
                public static string NextHypotAvToSegm = "Hypotenuse to next range peaks segment ratio";
                public static string PrevHypotAvToSegm = "Hypotenuse to previous range peaks segment ratio";
                public static string NextMaxAmpArgTanRatio = "ArgTan to next range peaks maximum amplitude";
                public static string NextMinAmpArgTanRatio = "ArgTan to next range peaks minimum amplitude";
                public static string PrevMaxAmpArgTanRatio = "ArgTan to previous range peaks maximum amplitude";
                public static string PrevMinAmpArgTanRatio = "ArgTan to previous range peaks minimum amplitude";
                public static string NextAmpIntervRatio = "Next range amplitude interval ratio";
                public static string PrevAmpIntervRatio = "Previous range amplitude interval ratio";
                public static string NextHypotMaxToGlob = "Hypotenuse to next range max peak global ratio";
                public static string NextHypotMinToGlob = "Hypotenuse to next range min peak global ratio";
                public static string PrevHypotMaxToGlob = "Hypotenuse to previous range max peak global ratio";
                public static string PrevHypotMinToGlob = "Hypotenuse to previous range min peak global ratio";

                public static string POnset = "P-onset";
                public static string PPeak = "P-peak";
                public static string PEnd = "P-end";
                public static string QPeak = "Q-peak";
                public static string RPeak = "R-peak";
                public static string SPeak = "S-peak";
                public static string TOnset = "T-onset";
                public static string TPeak = "T-peak";
                public static string TEnd = "T-end";
                public static string Delta = "Delta";
                public static string Other = "Other";
                public static string Normal = "Normal";
                public static string Abnormal = "Abnormal";
            }
        }

        public class ArrhythmiaClassification : ObjectiveBaseModel
        {
            public static string ObjectiveName = "Arrhythmia classification";
        }
    }
}

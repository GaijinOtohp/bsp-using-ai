using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;

namespace Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives
{
    public partial class AIModels_ObjectivesArchitectures
    {
        public class CharacteristicWavesDelineation
        {
            public static string ObjectiveName = "Characteristic waves delineation";

            //_______________________________________________________//
            //::::::::::::::::::::::CWD models::::::::::::::::::::://
            public class CWDReinforcementL : ObjectiveBaseModel
            {
                [DataMember]
                public TFNETReinforcementL CWDReinforcementLModel = new TFNETReinforcementL("", 0, 0, null, null);
                [DataMember]
                public TFNETReinforcementL CWDCrazyReinforcementLModel = new TFNETReinforcementL("", 0, 0, null, null);

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
                public TFNETReinforcementL CWDReinforcementLModel = new TFNETReinforcementL("", 0, 0, null, null);
                [DataMember]
                public TFNETReinforcementL CWDCrazyReinforcementLModel = new TFNETReinforcementL("", 0, 0, null, null);
                [DataMember]
                public TFNETLSTMModel CWDLSTMModel = new TFNETLSTMModel("", 0, 0, null, 0, layers: 0);

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

                public class CornersScanOutputs
                {
                    public static string AT = "AT"; // Angle Threshold
                    public static string ART = "ART"; // Amplitude Ratio Threshold

                    public static string[] GetNames()
                    {
                        return typeof(CornersScanOutputs).GetFields().Select(field => (string)field.GetValue(null)).ToArray();
                    }
                }

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

                public class PeaksLabelsOutputs
                {
                    public static string POnset = "P-onset";
                    public static string PPeak = "P-peak";
                    public static string PEnd = "P-end";
                    public static string QPeak = "Q-peak";
                    public static string RPeak = "R-peak";
                    public static string SPeak = "S-peak";
                    public static string TOnset = "T-onset";
                    public static string TPeak = "T-peak";
                    public static string TEnd = "T-end";
                    public static string Other = "Other";
                    public static string Normal = "Normal";
                    public static string Abnormal = "Abnormal";

                    public static string[] GetNames()
                    {
                        return typeof(PeaksLabelsOutputs).GetFields().Select(field => (string)field.GetValue(null)).ToArray();
                    }
                }
                public static string Delta = "Delta";
            }
        }
    }
}

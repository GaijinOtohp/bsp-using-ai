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

                public class CornersScanOutputs
                {
                    public static string AT = "AT"; // Angle Threshold
                    public static string ART = "ART"; // Amplitude Ratio Threshold

                    public static string[] GetNames()
                    {
                        return typeof(CornersScanOutputs).GetFields().Select(field => (string)field.GetValue(null)).ToArray();
                    }
                }

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

                    public static string[] GetNames()
                    {
                        return typeof(PeaksLabelsOutputs).GetFields().Select(field => (string)field.GetValue(null)).ToArray();
                    }
                }
                public static string Normal = "Normal";
                public static string Abnormal = "Abnormal";
                public static string Delta = "Delta";
            }
        }
    }
}

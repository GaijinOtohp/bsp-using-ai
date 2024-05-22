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
            public List<List<long[]>> DataIdsIntervalsList { get; set; } = new List<List<long[]>>();

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

                baseModelClone.DataIdsIntervalsList = new List<List<long[]>>();
                foreach (List<long[]> clonedUpdateIntervals in DataIdsIntervalsList)
                {
                    List<long[]> updateIntervals = new List<long[]>();
                    for (int i = 0; i < clonedUpdateIntervals.Count; i++)
                        updateIntervals.Add((long[])clonedUpdateIntervals[i].Clone());
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
                public TFNETReinforcementL CWDReinforcementLModel = new TFNETReinforcementL("", 0, 0);

                protected override ObjectiveBaseModel CreateCloneInstance()
                {
                    return new CWDReinforcementL();
                }
                public override ObjectiveBaseModel Clone()
                {
                    CWDReinforcementL cwdReinforcementL = (CWDReinforcementL)base.Clone();

                    cwdReinforcementL.CWDReinforcementLModel = (TFNETReinforcementL)CWDReinforcementLModel.Clone();

                    return cwdReinforcementL;
                }
            }

            public class CWDNamigs
            {
                public static string RLCornersScanData = "Corners scan";

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
            }
        }

        public class ArrhythmiaClassification : ObjectiveBaseModel
        {
            public static string ObjectiveName = "Arrhythmia classification";
        }
    }
}

using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace Biological_Signal_Processing_Using_AI.AITools
{
    public class AIModels_ObjectivesArchitectures
    {
        public class WPWSyndromeDetection
        {
            public static string ObjectiveName = "WPW syndrome detection";

            //_______________________________________________________//
            //::::::::::::::::::::::ARTHT models::::::::::::::::::::://
            [Serializable]
            [KnownType(typeof(CustomBaseModel))]
            [KnownType(typeof(KNNModel))]
            [KnownType(typeof(KerasNETNeuralNetworkModel))]
            [KnownType(typeof(KerasNETModelLessNeuralNetwork))]
            [KnownType(typeof(NaiveBayesModel))]
            [KnownType(typeof(TFNETNeuralNetworkModel))]
            [KnownType(typeof(TFNETModelLessNeuralNetwork))]
            [KnownType(typeof(TFKerasNeuralNetworkModel))]
            [KnownType(typeof(TFKerasModelLessNeuralNetwork))]
            [DataContract(IsReference = true)]
            public class ARTHTModels
            {
                [DataMember]
                public string ModelName { get; set; }
                [DataMember]
                public string ProblemName { get; set; }
                /// <summary>
                /// Training Details:
                /// Each train update creates a list of intervals (List<long[]>) of the _ids of the selected data
                /// </summary>
                [DataMember]
                public List<List<long[]>> DataIdsIntervalsList { get; set; } = new List<List<long[]>>();

                [DataMember]
                public Dictionary<string, CustomBaseModel> ARTHTModelsDic = new Dictionary<string, CustomBaseModel>(7)
            {
                { ARTHTNamings.Step1RPeaksScanData, new CustomBaseModel() },
                { ARTHTNamings.Step2RPeaksSelectionData, new CustomBaseModel() },
                { ARTHTNamings.Step3BeatPeaksScanData, new CustomBaseModel() },
                { ARTHTNamings.Step4PTSelectionData, new CustomBaseModel() },
                { ARTHTNamings.Step5ShortPRScanData, new CustomBaseModel() },
                { ARTHTNamings.Step6UpstrokesScanData, new CustomBaseModel() },
                { ARTHTNamings.Step7DeltaExaminationData, new CustomBaseModel() },
            };

                [DataMember]
                public long _validationTimeCompelxity { get; set; }
                [DataMember]
                public string _ValidationInfo { get; set; }

                public ARTHTModels Clone()
                {
                    ARTHTModels aRTHTModels = new ARTHTModels();
                    aRTHTModels.ModelName = ModelName;
                    aRTHTModels.ProblemName = ProblemName;
                    aRTHTModels._validationTimeCompelxity = _validationTimeCompelxity;
                    aRTHTModels._ValidationInfo = _ValidationInfo;

                    aRTHTModels.DataIdsIntervalsList = new List<List<long[]>>();
                    foreach (List<long[]> clonedUpdateIntervals in DataIdsIntervalsList)
                    {
                        List<long[]> updateIntervals = new List<long[]>();
                        for (int i = 0; i < clonedUpdateIntervals.Count; i++)
                            updateIntervals.Add((long[])clonedUpdateIntervals[i].Clone());
                        aRTHTModels.DataIdsIntervalsList.Add(updateIntervals);
                    }

                    foreach (string stepName in ARTHTModelsDic.Keys)
                        // Each AI model has its own Clone method, and each Clone method returns a different AI model
                        // Then the Clone method should be Invoked from the object of the AI model type
                        aRTHTModels.ARTHTModelsDic[stepName] = (CustomBaseModel)ARTHTModelsDic[stepName].GetType().GetMethod("Clone").Invoke(ARTHTModelsDic[stepName], null);

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
        }

        public class ArrhythmiaClassification
        {
            public static string ObjectiveName = "Arrhythmia classification";
        }
    }
}

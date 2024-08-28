using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;

namespace Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives
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

    public partial class AIModels_ObjectivesArchitectures
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
    }
}

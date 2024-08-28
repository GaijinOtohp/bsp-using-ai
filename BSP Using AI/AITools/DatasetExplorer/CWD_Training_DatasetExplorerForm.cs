using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;

namespace BSP_Using_AI.AITools.DatasetExplorer
{
    public partial class DatasetExplorerForm
    {
        public void queryForTrainingDataset_CWD(string modelType)
        {
            // Check if there was any added intervals
            if (!(_objectiveModel.DataIdsIntervalsList.Count > _updatesNum))
                return;

            // Qurey for signals features in all last selected intervals from dataset
            (string selection, object[] selectionArgs) = SelectDataFromIntervals(_objectiveModel.DataIdsIntervalsList[_objectiveModel.DataIdsIntervalsList.Count - 1],
                                                                                 "and anno_objective=?", new object[] { CharacteristicWavesDelineation.ObjectiveName });

            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("anno_ds",
                                        new string[] { "sginal_name", "starting_index", "signal_data", "sampling_rate", "anno_data" },
                                        selection,
                                        selectionArgs,
                                        "", "DatasetExplorerFormForTraining_" + modelType));
            dbStimulatorThread.Start();
        }

        private void fitSelectionButton_Click_CWD(string modelType)
        {
            // Start training
            queryForTrainingDataset_CWD(modelType);
        }
    }
}

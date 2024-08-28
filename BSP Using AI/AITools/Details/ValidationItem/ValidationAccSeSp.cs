using Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.AITools.DatasetExplorer;
using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using BSP_Using_AI.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.AITools.AIBackThreadReportHolder;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;

namespace BSP_Using_AI.AITools.Details
{
    public partial class ValidationAccSeSp : UserControl, DbStimulatorReportHolder, AIBackThreadReportHolder
    {
        public Dictionary<string, ObjectiveBaseModel> _objectivesModelsDic = null;

        private ObjectiveBaseModel _objectiveModel;

        private CustomArchiBaseModel _InnerObjectiveModel;

        public ValidationAccSeSp(Dictionary<string, ObjectiveBaseModel> objectivesModelsDic, ObjectiveBaseModel objectiveModel, CustomArchiBaseModel innerObjectiveModel)
        {
            InitializeComponent();

            _objectivesModelsDic = objectivesModelsDic;
            _objectiveModel = objectiveModel;
            _InnerObjectiveModel = innerObjectiveModel;
        }

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void ValidationFlowLayoutPanelUserControl_Click(object sender, EventArgs e)
        {
            // Qurey for signals features in all last selected intervals from dataset
            List<IdInterval> allDataIdsIntervalsList = new List<IdInterval>();
            foreach (List<IdInterval> trainingIntervalsList in _objectiveModel.DataIdsIntervalsList)
                allDataIdsIntervalsList.AddRange(trainingIntervalsList);

            // Check which objective is this data for
            if (_objectiveModel is ARTHTModels)
                queryForSelectedDataset_ARTHT(allDataIdsIntervalsList);
            else if (_objectiveModel is CWDReinforcementL || (_objectiveModel is CWDLSTM && _InnerObjectiveModel is TFNETReinforcementL))
                queryForSelectedDataset_CWD(allDataIdsIntervalsList, "CWDReinforcementL");
            else if (_objectiveModel is CWDLSTM && _InnerObjectiveModel is TFNETLSTMModel)
                queryForSelectedDataset_CWD(allDataIdsIntervalsList, "CWDLSTM");
        }

        public void queryForSelectedDataset_ARTHT(List<IdInterval> allDataIdsIntervalsList)
        {
            (string selection, object[] selectionArgs) = DatasetExplorerForm.SelectDataFromIntervals(allDataIdsIntervalsList, null, null);

            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("dataset",
                                        new String[] { "features" },
                                        selection,
                                        selectionArgs,
                                        "", "ValidationFlowLayoutPanelUserControl_ARTHT"));
            dbStimulatorThread.Start();
        }

        public void queryForSelectedDataset_CWD(List<IdInterval> allDataIdsIntervalsList, string modelType)
        {
            (string selection, object[] selectionArgs) = DatasetExplorerForm.SelectDataFromIntervals(allDataIdsIntervalsList,
                                                                                     "and anno_objective=?", new object[] { CharacteristicWavesDelineation.ObjectiveName });

            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("anno_ds",
                                        new string[] { "sginal_name", "starting_index", "signal_data", "sampling_rate", "anno_data" },
                                        selection,
                                        selectionArgs,
                                        "", "ValidationFlowLayoutPanelUserControl_" + modelType));
            dbStimulatorThread.Start();
        }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //:::::::::::::::::::::::::::CROSS PROCESS FORM FUNCTIONS (INTERFACES)::::::::::::::::::::::://

        private List<Sample> SortDatasetSamples_ARTHT(DataTable dataTable)
        {
            List<Sample> dataList = new List<Sample>();

            ARTHTFeatures aRTHTFeatures = null;
            foreach (DataRow row in dataTable.AsEnumerable())
            {
                aRTHTFeatures = GeneralTools.ByteArrayToObject<ARTHTFeatures>(row.Field<byte[]>("features"));

                dataList.AddRange(aRTHTFeatures.StepsDataDic[_InnerObjectiveModel.Name].Samples);
            }

            return dataList;
        }

        private List<Sample> SortDatasetSamples_CWDReinforcementL(DataTable dataTable)
        {
            Dictionary<string, List<Sample>> trainingSamplesListsDict = DatasetExplorerForm.GetPreviousTrainingSamples(dataTable.AsEnumerable().ToList());

            List<Sample> dataList = trainingSamplesListsDict.SelectMany(dictPair => dictPair.Value).ToList();

            return dataList;
        }

        private List<Sample> SortDatasetSamples_CWDLSTM(DataTable dataTable)
        {
            List<List<Sample>> trainingDataListSequences = DatasetExplorerForm.BuildLSTMTrainingSequences(dataTable.AsEnumerable().ToList(), ((CWDLSTM)_objectiveModel).CWDReinforcementLModel);

            List<Sample> dataList = trainingDataListSequences.SelectMany(sequenceList => sequenceList).ToList();

            return dataList;
        }

        public void holdRecordReport(DataTable dataTable, string callingClassName)
        {
            if (!callingClassName.Contains("ValidationFlowLayoutPanelUserControl"))
                return;

            // Initialize list of features for selected step
            List<Sample> dataList = new List<Sample>();
            long datasetSize = dataTable.Rows.Count;

            // Get features of only the selected inner model
            if (callingClassName.Contains("ARTHT"))
                dataList = SortDatasetSamples_ARTHT(dataTable);
            else if (callingClassName.Contains("CWDReinforcementL"))
                dataList = SortDatasetSamples_CWDReinforcementL(dataTable);
            else if (callingClassName.Contains("CWDLSTM"))
                dataList = SortDatasetSamples_CWDLSTM(dataTable);

            // Send data to DataVisualisationForm
            DataVisualisationForm dataVisualisationForm = new DataVisualisationForm(_objectivesModelsDic,
                                                                                    _objectiveModel, _InnerObjectiveModel,
                                                                                    ((DetailsForm)this.FindForm())._modelId, dataList, datasetSize);
            dataVisualisationForm._ValidationItemUserControl = this;
            dataVisualisationForm.stepLabel.Text = modelTargetLabel.Text;
            this.Invoke(new MethodInvoker(delegate () { dataVisualisationForm.Show(); }));
        }

        public void holdAIReport(AIReport report, string callingClassName)
        {
            // Check if this is fitting progress report
            if (report.ReportType == AIReportType.FittingProgress)
            {
                // If yes then refresh progress bar of the selected model
                FittingProgAIReport progReport = (FittingProgAIReport)report;
                this.Invoke(new MethodInvoker(delegate () { updatetProgressBar.Maximum = progReport.fitMaxProgress; }));
                this.Invoke(new MethodInvoker(delegate () { updatetProgressBar.Value = progReport.fitProgress; }));
            }
            else if (report.ReportType == AIReportType.FittingComplete)
            {
                FittingCompAIReport compReport = (FittingCompAIReport)report;
                if (compReport.datasetSize == -1)
                {
                    // If yes then this is from PCA analysis, then just set the progress bar to its maximum
                    this.Invoke(new MethodInvoker(delegate () { updatetProgressBar.Maximum = 1; }));
                    this.Invoke(new MethodInvoker(delegate () { updatetProgressBar.Value = 1; }));
                }
            }
        }
    }
}

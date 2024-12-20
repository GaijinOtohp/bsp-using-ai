using Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI;
using BSP_Using_AI.AITools.DatasetExplorer;
using BSP_Using_AI.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.AITools.AIBackThreadReportHolder;

namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation.ValDataVis.ROC_thresholds
{
    public partial class ROCDataSelForm : Form, DbStimulatorReportHolder
    {
        private AIBackThreadReportHolder _aiBackThreadReportHolderForAIToolsForm;

        ObjectiveBaseModel _ObjectiveModel;
        long _modelId;

        private List<DataRow> _ModelOrderedSelectedDataList;
        private List<DataRow> _MoedlOrderedNonSeleDataList;
        private Dictionary<int, DataRow> _SelectedValiDataList;

        public int _lastSelectedItem_shift = -1;
        public bool _shiftClicked = false;

        public bool _ignoreEvent = false;

        public ROCDataSelForm(ObjectiveBaseModel objectiveModel, long modelId, AIBackThreadReportHolder aiBackThreadReportHolderForAIToolsForm)
        {
            InitializeComponent();

            _ObjectiveModel = objectiveModel;
            _modelId = modelId;
            _aiBackThreadReportHolderForAIToolsForm = aiBackThreadReportHolderForAIToolsForm;
        }

        public void queryForSignals_ARTHT()
        {
            // Qurey for all of the signals corresponding to the selected model
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("dataset",
                                        new string[] { "_id", "sginal_name", "starting_index", "sampling_rate", "quantisation_step", "features" },
                                        null,
                                        null,
                                        "", "ValDataSelectionForm_ARTHT"));
            dbStimulatorThread.Start();
        }

        public void queryForSignals_Anno(string annoObjective)
        {
            // Qurey for all of the signals corresponding to the selected model
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("anno_ds",
                                        new string[] { "_id", "sginal_name", "starting_index", "sampling_rate", "quantisation_step", "signal_data", "anno_data" },
                                        "anno_objective=?",
                                        new object[] { annoObjective },
                                        "", "ValDataSelectionForm_CWD"));
            dbStimulatorThread.Start();
        }

        public void initializeForm()
        {
            // Query for all signals in dataset table
            if (_ObjectiveModel is ARTHTModels)
                queryForSignals_ARTHT();
            else if (_ObjectiveModel is CWDReinforcementL || _ObjectiveModel is CWDLSTM)
                queryForSignals_Anno(CharacteristicWavesDelineation.ObjectiveName);
        }

        private void LoadAllData()
        {
            _SelectedValiDataList = new Dictionary<int, DataRow>();

            foreach (DataRow row in _MoedlOrderedNonSeleDataList)
            {
                ROCDSelItemUC rocDSelItemUC = new ROCDSelItemUC(row, "Not fitted", Color.LightGreen, _SelectedValiDataList);
                if (IsHandleCreated) this.Invoke(new MethodInvoker(delegate () { ValDataFlowLayoutPanel.Controls.Add(rocDSelItemUC); }));
            }
            foreach (DataRow row in _ModelOrderedSelectedDataList)
            {
                ROCDSelItemUC rocDSelItemUC = new ROCDSelItemUC(row, "Fitted", Color.DeepSkyBlue, _SelectedValiDataList);
                if (IsHandleCreated) this.Invoke(new MethodInvoker(delegate () { ValDataFlowLayoutPanel.Controls.Add(rocDSelItemUC); }));
            }
        }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //:::::::::::::::::::::::::::::::::::::Events Handlers::::::::::::::::::::::::::::::::::::::://
        private void ValDataSelectionForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the pressed key is "Shift"
            if (e.Shift)
                _shiftClicked = true;
        }

        private void ValDataSelectionForm_KeyUp(object sender, KeyEventArgs e)
        {
            _shiftClicked = false;
        }

        public static void UpdateThresholds(CWDLSTM cwdLSTM, List<List<Sample>> dataListSequences, AIBackThreadReportHolder optimizationReporter, string selectedModelName, long modelId)
        {
            if (dataListSequences.Count == 0)
                return;

            TFNETLSTMModel lstmModel = cwdLSTM.CWDLSTMModel;
            // Set the high and low outputs averaged to zeros
            // and initialize the ROC
            for (int iOutput = 0; iOutput < lstmModel._outputDim; iOutput++)
            {
                lstmModel.OutputsThresholds[iOutput]._highOutputAv = 0;
                lstmModel.OutputsThresholds[iOutput]._lowOutputAv = 0;
                lstmModel.OutputsThresholds[iOutput]._ROC = new Dictionary<double, (int _truePositives, int _falsePositives)>(101);
            }

            // Iterate through 100 possibilities of thresholds
            int[] rocBestGap = new int[lstmModel._outputDim];
            double[] rocBestThreshold = new double[lstmModel._outputDim];
            for (int iThreshold = 0; iThreshold < 100; iThreshold++)
            {
                // Modify the model's outputs' thresholds
                double newThreshold = iThreshold / 100d;
                for (int iOutput = 0; iOutput < lstmModel._outputDim; iOutput++)
                    lstmModel.OutputsThresholds[iOutput]._threshold = newThreshold;

                // Get validation data using the selected threshod
                (ValidationData validationData, OutputThresholdItem[] outThresholds) = DetailsForm.LSTM_ValidateTheModel(lstmModel, dataListSequences, lstmModel, null);

                // Insert the new evaluation to the ROC
                for (int iOutput = 0; iOutput < lstmModel._outputDim; iOutput++)
                {
                    int truePositives = validationData._ModelOutputsValidMetrics[iOutput]._truePositive;
                    int falsePositives = validationData._ModelOutputsValidMetrics[iOutput]._falsePositive;

                    lstmModel.OutputsThresholds[iOutput]._ROC.Add(newThreshold, (truePositives, falsePositives));

                    if (truePositives - falsePositives > rocBestGap[iOutput])
                    {
                        rocBestGap[iOutput] = truePositives - falsePositives;
                        rocBestThreshold[iOutput] = newThreshold;

                        lstmModel.OutputsThresholds[iOutput]._highOutputAv = outThresholds[iOutput]._highOutputAv;
                        lstmModel.OutputsThresholds[iOutput]._lowOutputAv = outThresholds[iOutput]._lowOutputAv;
                    }
                }

                // Send report about fitting is finished and models table should be updated
                if (optimizationReporter != null)
                    optimizationReporter.holdAIReport(new FittingProgAIReport()
                    {
                        ReportType = AIReportType.FittingProgress,
                        ModelName = selectedModelName,
                        fitProgress = iThreshold,
                        fitMaxProgress = 99
                    }, "AIToolsForm");
            }

            // Set the best thresholds
            for (int iOutput = 0; iOutput < lstmModel._outputDim; iOutput++)
                lstmModel.OutputsThresholds[iOutput]._threshold = rocBestThreshold[iOutput];

            // Update model in models table
            DbStimulator dbStimulator = new DbStimulator();
            if (cwdLSTM.DataIdsIntervalsList.Count > 0)
            {
                Thread dbStimulatorThread = new Thread(() => dbStimulator.Update("models", new string[] { "the_model" },
                    new object[] { GeneralTools.ObjectToByteArray(cwdLSTM.Clone()) }, modelId, "CWD_TF_NET_LSTM"));
                dbStimulatorThread.Start();
            }
        }

        private void startOptimizationButton_Click(object sender, EventArgs e)
        {
            // Get CWD LSTM base model
            CWDLSTM cwdLSTM = (CWDLSTM)_ObjectiveModel;

            // Build LSTM sequences for optimizing the thresholds
            List<List<Sample>> dataListSequences = DatasetExplorerForm.BuildLSTMTrainingSequences(_SelectedValiDataList.Values.ToList(), cwdLSTM.CWDReinforcementLModel);

            // Start the thresholds optimization
            string selectedModelName = _ObjectiveModel.ModelName + _ObjectiveModel.ObjectiveName;
            Thread validationThread = new Thread(() => UpdateThresholds(cwdLSTM, dataListSequences, _aiBackThreadReportHolderForAIToolsForm, selectedModelName, _modelId));
            validationThread.Start();

            // Close this window
            Close();
        }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //:::::::::::::::::::::::::::CROSS PROCESS FORM FUNCTIONS (INTERFACES)::::::::::::::::::::::://
        public void holdRecordReport(DataTable dataTable, string callingClassName)
        {
            if (!callingClassName.Contains("ValDataSelectionForm"))
                return;

            // Order datatable by name
            List<DataRow> rowsList = new List<DataRow>(dataTable.AsEnumerable());
            List<string> namesList = new List<string>();
            foreach (DataRow row in rowsList)
                namesList.Add(row.Field<string>("sginal_name"));
            rowsList = GeneralTools.OrderByTextWithNumbers(rowsList, namesList);

            // Copy data to the two lists _moedlNonSeleDataList and _modelSelectedDataList
            // where _moedlNonSeleDataList holds only the data that are not fitted in the model (both of the lists are used for the option fast validation)
            // and _modelSelectedDataList holds only the selected data for the model (used only for training and validating temporary models for crossvalidation)
            _ModelOrderedSelectedDataList = new List<DataRow>();
            _MoedlOrderedNonSeleDataList = new List<DataRow>();
            List<IdInterval> allDataIdsIntervalsList = _ObjectiveModel.DataIdsIntervalsList.SelectMany(IDIntervalsList => IDIntervalsList).ToList();
            foreach (DataRow row in rowsList)
            {
                long rowId = row.Field<long>("_id");
                bool rowSelected = false;
                foreach (IdInterval interval in allDataIdsIntervalsList)
                    if (interval.starting <= rowId && rowId <= interval.ending)
                    {
                        _ModelOrderedSelectedDataList.Add(row);
                        rowSelected = true;
                        break;
                    }

                if (!rowSelected)
                    _MoedlOrderedNonSeleDataList.Add(row);
            }

            LoadAllData();
        }
    }
}

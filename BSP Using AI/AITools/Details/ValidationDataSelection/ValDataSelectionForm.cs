using Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives;
using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI;
using BSP_Using_AI.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.Structures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;

namespace Biological_Signal_Processing_Using_AI.AITools.Details.ValidationDataSelection
{
    public partial class ValDataSelectionForm : Form, DbStimulatorReportHolder
    {
        public class ModelData
        {
            public List<DataRow> TrainingData = new List<DataRow>();
            public List<DataRow> ValidationData = new List<DataRow>();
        }

        public class ModelSamples
        {
            public List<Sample> TrainingData = new List<Sample>();
            public List<Sample> ValidationData = new List<Sample>();
        }

        public delegate void ValidateModel(List<ModelData> data, string validationInfo);

        ObjectiveBaseModel _ObjectiveModel;
        private ValidateModel _ValidateModel;

        private List<DataRow> _ModelOrderedSelectedDataList;
        private List<DataRow> _MoedlOrderedNonSeleDataList;
        private List<DataRow> _MoedlShuffeledSeleDataList;
        private Dictionary<int, DataRow> _SelectedValiDataList;

        private int _trainingSetCapacity;
        private int _foldCapacity;

        public int _lastSelectedItem_shift = -1;
        public bool _shiftClicked = false;

        public bool _ignoreEvent = false;

        public ValDataSelectionForm(ObjectiveBaseModel objectiveModel, ValidateModel validateModel)
        {
            InitializeComponent();

            _ObjectiveModel = objectiveModel;
            _ValidateModel = validateModel;
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

        private void ReloadModelSelectedData()
        {
            foreach (DataRow row in _MoedlShuffeledSeleDataList)
            {
                // Create an item of the model
                ValSeleDataItemUC validationDataItemUserControl = new ValSeleDataItemUC(SetSignalOrder, _MoedlShuffeledSeleDataList.Count);
                validationDataItemUserControl.signalNameLabel.Text = row.Field<string>("sginal_name");
                validationDataItemUserControl.startingIndexLabel.Text = row.Field<long>("starting_index").ToString();
                validationDataItemUserControl.samplingRateLabel.Text = row.Field<long>("sampling_rate").ToString();
                validationDataItemUserControl.quantizationStepLabel.Text = row.Field<long>("quantisation_step").ToString();
                // Set the naming of the category according to the validation type selected
                if (holdoutValidationRadioButton.Checked)
                {
                    // If yes then this is holdout validation type
                    // Check if this signal belongs to training set
                    if (ValDataFlowLayoutPanel.Controls.Count < _trainingSetCapacity)
                        validationDataItemUserControl.categoryLabel.Text = "Traning";
                    else
                        validationDataItemUserControl.categoryLabel.Text = "Validation";
                }
                else if (crossValidationRadioButton.Checked)
                {
                    // This is cross-validation type
                    // Set the fold number to which this signal belongs to
                    int foldNum = (ValDataFlowLayoutPanel.Controls.Count / _foldCapacity) + 1;
                    validationDataItemUserControl.categoryLabel.Text = "Fold " + foldNum;
                }
                // Set the order of the signal
                validationDataItemUserControl._ignoreEvent = true;
                validationDataItemUserControl.orderTextBox.Text = ValDataFlowLayoutPanel.Controls.Count.ToString();
                validationDataItemUserControl._ignoreEvent = false;

                if (IsHandleCreated) this.Invoke(new MethodInvoker(delegate () { ValDataFlowLayoutPanel.Controls.Add(validationDataItemUserControl); }));
            }
        }

        private void ReloadAllData()
        {
            _SelectedValiDataList = new Dictionary<int, DataRow>();

            foreach (DataRow row in _MoedlOrderedNonSeleDataList)
            {
                ValNonSeleDataItemUC valNonSeleDataItemUserControl = new ValNonSeleDataItemUC(row, "Not fitted", Color.LightGreen, _SelectedValiDataList);
                if (IsHandleCreated) this.Invoke(new MethodInvoker(delegate () { ValDataFlowLayoutPanel.Controls.Add(valNonSeleDataItemUserControl); }));
            }
            foreach (DataRow row in _ModelOrderedSelectedDataList)
            {
                ValNonSeleDataItemUC valNonSeleDataItemUserControl = new ValNonSeleDataItemUC(row, "Fitted", Color.DeepSkyBlue, _SelectedValiDataList);
                if (IsHandleCreated) this.Invoke(new MethodInvoker(delegate () { ValDataFlowLayoutPanel.Controls.Add(valNonSeleDataItemUserControl); }));
            }
        }

        public void ReloadData()
        {
            // Clear modelsFlowLayoutPanel
            /*if (ValDataFlowLayoutPanel.Controls.Count > 0)
                if (IsHandleCreated) this.Invoke(new MethodInvoker(delegate () { EventHandlers.fastClearFlowLayout(ref ValDataFlowLayoutPanel); }));*/
            ValDataFlowLayoutPanel.Controls.Clear();

            // Check which validation technique is selected
            if (trainingAndValidRadioButton.Checked)
                ReloadModelSelectedData();
            else if (fastValidadioButton.Checked)
                ReloadAllData();
        }

        private void SetSignalOrder(ValSeleDataItemUC item, int newOrderIndex)
        {
            // Get current index of the signal
            int currentOrderIndex = ValDataFlowLayoutPanel.Controls.IndexOf(item);

            // Set direction of movement
            int movementDirection = newOrderIndex > currentOrderIndex ? 1 : -1;

            // Change item index in flow layout panel
            string itemDataCategory = item.categoryLabel.Text;
            ValDataFlowLayoutPanel.Controls.SetChildIndex(item, newOrderIndex);
            if (movementDirection > 0)
                for (int i = newOrderIndex; i > currentOrderIndex; i--)
                    CopyItemToIndex(i, i - 1);
            else if (movementDirection < 0)
                for (int i = newOrderIndex; i < currentOrderIndex; i++)
                    CopyItemToIndex(i, i + 1);
            ((ValSeleDataItemUC)ValDataFlowLayoutPanel.Controls[currentOrderIndex]).categoryLabel.Text = itemDataCategory;
            ((ValSeleDataItemUC)ValDataFlowLayoutPanel.Controls[currentOrderIndex])._ignoreEvent = true;
            ((ValSeleDataItemUC)ValDataFlowLayoutPanel.Controls[currentOrderIndex]).orderTextBox.Text = currentOrderIndex.ToString();
            ((ValSeleDataItemUC)ValDataFlowLayoutPanel.Controls[currentOrderIndex])._ignoreEvent = false;

            // Get the signal dataRow

            // Change the orders in _moedlShuffeledSeleDataList
            DataRow itemDataRow = _MoedlShuffeledSeleDataList[currentOrderIndex];
            if (movementDirection > 0)
                for (int i = currentOrderIndex; i < newOrderIndex; i++)
                    _MoedlShuffeledSeleDataList[i] = _MoedlShuffeledSeleDataList[i + 1];
            else if (movementDirection < 0)
                for (int i = currentOrderIndex; i > newOrderIndex; i--)
                    _MoedlShuffeledSeleDataList[i] = _MoedlShuffeledSeleDataList[i - 1];
            _MoedlShuffeledSeleDataList[newOrderIndex] = itemDataRow;
        }
        private void CopyItemToIndex(int newIndex, int itemIndex)
        {
            ((ValSeleDataItemUC)ValDataFlowLayoutPanel.Controls[newIndex]).categoryLabel.Text = ((ValSeleDataItemUC)ValDataFlowLayoutPanel.Controls[itemIndex]).categoryLabel.Text;
            ((ValSeleDataItemUC)ValDataFlowLayoutPanel.Controls[newIndex])._ignoreEvent = true;
            ((ValSeleDataItemUC)ValDataFlowLayoutPanel.Controls[newIndex]).orderTextBox.Text = ((ValSeleDataItemUC)ValDataFlowLayoutPanel.Controls[itemIndex]).orderTextBox.Text;
            ((ValSeleDataItemUC)ValDataFlowLayoutPanel.Controls[newIndex])._ignoreEvent = false;
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

        private void trainingAndValidRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (trainingAndValidRadioButton.Checked)
            {
                // Enable the options for training and validation
                instrucitonLabel.Visible = false;
                holdoutValidationRadioButton.Enabled = true;
                crossValidationRadioButton.Enabled = true;
                shuffleButton.Enabled = true;
                applyChangesButton.Enabled = true;
                if (holdoutValidationRadioButton.Checked)
                    trainingSetRatioTextBox.Enabled = true;
                else if (crossValidationRadioButton.Checked)
                    numberOfFoldsTextBox.Enabled = true;

                // Load data
                ReloadData();
            }
        }

        private void fastValidadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (fastValidadioButton.Checked)
            {
                // Disable the options for training and validation
                instrucitonLabel.Visible = true;
                holdoutValidationRadioButton.Enabled = false;
                crossValidationRadioButton.Enabled = false;
                shuffleButton.Enabled = false;
                applyChangesButton.Enabled = false;
                trainingSetRatioTextBox.Enabled = false;
                numberOfFoldsTextBox.Enabled = false;

                // Load data
                ReloadData();
            }
        }

        private void shuffleButton_Click(object sender, EventArgs e)
        {
            // Shuffle all records first
            GeneralTools.Shuffle(_MoedlShuffeledSeleDataList);
            // Load data
            ReloadData();
        }

        private void holdoutValidationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (holdoutValidationRadioButton.Checked)
            {
                // Enable setting training ration and disable setting folds number of folds
                trainingSetRatioTextBox.Enabled = true;
                numberOfFoldsTextBox.Enabled = false;
                // Reload data
                ReloadData();
            }
        }
        private void crossValidationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (crossValidationRadioButton.Checked)
            {
                // Disable setting training ration and enable setting folds number of folds
                trainingSetRatioTextBox.Enabled = false;
                numberOfFoldsTextBox.Enabled = true;
                // Reload data
                ReloadData();
            }
        }

        private void trainingSetRatioTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.keypressNumbersAndDecimalOnly(sender, e);
        }

        private void trainingSetRatioTextBox_TextChanged(object sender, EventArgs e)
        {
            double ratio = 0.75d;
            if (!trainingSetRatioTextBox.Text.Equals("") && !trainingSetRatioTextBox.Text.Equals("."))
                ratio = double.Parse(trainingSetRatioTextBox.Text);
            // Check if ratio is not ok
            if (ratio > 1 || ratio < 0)
            {
                trainingSetRatioTextBox.Text = "0.75";
                ratio = 0.75d;
            }
            // Set training capacity
            _trainingSetCapacity = (int)(ratio * _MoedlShuffeledSeleDataList.Count);
        }

        private void numberOfFoldsTextBox_TextChanged(object sender, EventArgs e)
        {
            int foldsNum = 2;
            if (!numberOfFoldsTextBox.Text.Equals("") && !numberOfFoldsTextBox.Text.Equals("."))
                foldsNum = (int)double.Parse(numberOfFoldsTextBox.Text);
            // Set fold capacity
            _foldCapacity = (_MoedlShuffeledSeleDataList.Count / foldsNum) + (_MoedlShuffeledSeleDataList.Count % foldsNum > 0 ? 1 : 0);
        }

        private void applyChangesButton_Click(object sender, EventArgs e)
        {
            // Reload data
            ReloadData();
        }

        private void startValidationButton_Click(object sender, EventArgs e)
        {
            // Create model data
            List<ModelData> data = new List<ModelData>();
            // Create validation info
            string validationInfo = "";
            // Check which validation type is selected
            if (trainingAndValidRadioButton.Checked)
            {
                if (holdoutValidationRadioButton.Checked)
                {
                    // Create only one model data
                    ModelData modelData = new ModelData();
                    for (int i = 0; i < _MoedlShuffeledSeleDataList.Count; i++)
                        // Check if this signal belongs to training set
                        if (i < _trainingSetCapacity)
                            modelData.TrainingData.Add(_MoedlShuffeledSeleDataList[i]);
                        else
                            modelData.ValidationData.Add(_MoedlShuffeledSeleDataList[i]);
                    data.Add(modelData);
                    // Insert validation info
                    validationInfo = "(Holdout validation, " + Math.Round(_trainingSetCapacity / (double)_MoedlShuffeledSeleDataList.Count * 100, 0) + "% training)";
                }
                else if (crossValidationRadioButton.Checked)
                {
                    // Separete data into folds
                    List<DataRow>[] dataFolds = new List<DataRow>[(_MoedlShuffeledSeleDataList.Count / _foldCapacity) + (_MoedlShuffeledSeleDataList.Count % _foldCapacity > 0 ? 1 : 0)];
                    int selectedFold;
                    for (int i = 0; i < _MoedlShuffeledSeleDataList.Count; i++)
                    {
                        selectedFold = i / _foldCapacity;
                        // Check if the list of the selected fold is not created
                        if (dataFolds[selectedFold] == null)
                            // Create a new one
                            dataFolds[selectedFold] = new List<DataRow>();
                        // Insert the signal into the selected fold
                        dataFolds[selectedFold].Add(_MoedlShuffeledSeleDataList[i]);
                    }

                    // Create model datas for the separated folds
                    for (int i = 0; i < dataFolds.Length; i++)
                    {
                        // Create a new model data and fill it
                        ModelData modelData = new ModelData();
                        for (int j = 0; j < dataFolds.Length; j++)
                        {
                            // Check if validation fold is selected
                            if (i == j)
                                // If yes then insert fold data into validation data
                                foreach (DataRow row in dataFolds[j])
                                    modelData.ValidationData.Add(row);
                            else
                                // Insert fold data into training data
                                foreach (DataRow row in dataFolds[j])
                                    modelData.TrainingData.Add(row);
                        }
                        // Insert the new model data into data
                        data.Add(modelData);
                    }
                    // Insert validation info
                    validationInfo = "(Cross validation, " + dataFolds.Length + " folds)";
                }
            }
            else if (fastValidadioButton.Checked)
            {
                // Create only one model data with only validation data
                ModelData modelData = new ModelData();
                modelData.ValidationData.AddRange(_SelectedValiDataList.Values);
                data.Add(modelData);
                // Insert validation info
                validationInfo = "(Fast validation, " + _ModelOrderedSelectedDataList.Count + " training samples, " + _SelectedValiDataList.Count + " validation samples)";
            }

            // Start validation
            Thread validationThread = new Thread(() => _ValidateModel(data, validationInfo));
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

            _MoedlShuffeledSeleDataList = _ModelOrderedSelectedDataList.Select(row => row).ToList();
            // Set training capacity
            _trainingSetCapacity = (int)(0.75d * _MoedlShuffeledSeleDataList.Count);
            // and fold capacity
            _foldCapacity = (_MoedlShuffeledSeleDataList.Count / 10) + (_MoedlShuffeledSeleDataList.Count % 10 > 0 ? 1 : 0);

            // Shuffle all records first
            GeneralTools.Shuffle(_MoedlShuffeledSeleDataList);

            ReloadData();
        }
    }
}

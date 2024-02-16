using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace Biological_Signal_Processing_Using_AI.AITools.Details.ValidationDataSelection
{
    public partial class ValDataSelectionForm : Form
    {
        public class ModelData
        {
            public List<ARTHTFeatures> TrainingData = new List<ARTHTFeatures>();
            public List<ARTHTFeatures> ValidationData = new List<ARTHTFeatures>();
        }

        public delegate void ValidateModel(List<ModelData> data, string validationInfo);

        private List<DataRow> _dataList;
        private ValidateModel _ValidateModel;

        private int _trainingSetCapacity;
        private int _foldCapacity;

        public ValDataSelectionForm(DataTable dataTable, ValidateModel validateModel)
        {
            InitializeComponent();
            _ValidateModel = validateModel;

            // Set _dataLinkedList
            _dataList = new List<DataRow>(dataTable.AsEnumerable());

            // Set training capacity
            _trainingSetCapacity = (int)(0.75d * _dataList.Count);
            // and fold capacity
            _foldCapacity = (_dataList.Count / 10) + (_dataList.Count % 10 > 0 ? 1 : 0);

            // Shuffle all records first
            GeneralTools.Shuffle(_dataList);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // Load data
            ReloadData();
        }

        public void ReloadData()
        {
            // Clear modelsFlowLayoutPanel
            if (signalsFlowLayoutPanel.Controls.Count > 0)
                if (IsHandleCreated) this.Invoke(new MethodInvoker(delegate () { EventHandlers.fastClearFlowLayout(ref signalsFlowLayoutPanel); }));

            // Insert new items from records
            foreach (DataRow row in _dataList)
            {
                // Create an item of the model
                ValDataItemUC validationDataItemUserControl = new ValDataItemUC(SetSignalOrder, _dataList.Count);
                validationDataItemUserControl.signalNameLabel.Text = row.Field<string>("sginal_name");
                validationDataItemUserControl.startingIndexLabel.Text = row.Field<long>("starting_index").ToString();
                validationDataItemUserControl.samplingRateLabel.Text = row.Field<long>("sampling_rate").ToString();
                validationDataItemUserControl.quantizationStepLabel.Text = row.Field<long>("quantisation_step").ToString();
                // Set the naming of the category according to the validation type selected
                if (holdoutValidationRadioButton.Checked)
                {
                    // If yes then this is holdout validation type
                    // Check if this signal belongs to training set
                    if (signalsFlowLayoutPanel.Controls.Count < _trainingSetCapacity)
                        validationDataItemUserControl.categoryLabel.Text = "Traning";
                    else
                        validationDataItemUserControl.categoryLabel.Text = "Validation";
                }
                else if (crossValidationRadioButton.Checked)
                {
                    // This is cross-validation type
                    // Set the fold number to which this signal belongs to
                    int foldNum = (signalsFlowLayoutPanel.Controls.Count / _foldCapacity) + 1;
                    validationDataItemUserControl.categoryLabel.Text = "Fold " + foldNum;
                }
                // Set the order of the signal
                validationDataItemUserControl._ignoreEvent = true;
                validationDataItemUserControl.orderTextBox.Text = signalsFlowLayoutPanel.Controls.Count.ToString();
                validationDataItemUserControl._ignoreEvent = false;

                if (IsHandleCreated) this.Invoke(new MethodInvoker(delegate () { signalsFlowLayoutPanel.Controls.Add(validationDataItemUserControl); }));
            }
        }

        private void SetSignalOrder(ValDataItemUC item, int newOrderIndex)
        {
            // Get current index of the signal
            int currentOrderIndex = signalsFlowLayoutPanel.Controls.IndexOf(item);

            // Set direction of movement
            int movementDirection = newOrderIndex > currentOrderIndex ? 1 : -1;

            // Change item index in flow layout panel
            string itemDataCategory = item.categoryLabel.Text;
            signalsFlowLayoutPanel.Controls.SetChildIndex(item, newOrderIndex);
            if (movementDirection > 0)
                for (int i = newOrderIndex; i > currentOrderIndex; i--)
                    CopyItemToIndex(i, i - 1);
            else if (movementDirection < 0)
                for (int i = newOrderIndex; i < currentOrderIndex; i++)
                    CopyItemToIndex(i, i + 1);
            ((ValDataItemUC)signalsFlowLayoutPanel.Controls[currentOrderIndex]).categoryLabel.Text = itemDataCategory;
            ((ValDataItemUC)signalsFlowLayoutPanel.Controls[currentOrderIndex])._ignoreEvent = true;
            ((ValDataItemUC)signalsFlowLayoutPanel.Controls[currentOrderIndex]).orderTextBox.Text = currentOrderIndex.ToString();
            ((ValDataItemUC)signalsFlowLayoutPanel.Controls[currentOrderIndex])._ignoreEvent = false;

            // Get the signal dataRow

            // Change the orders in _dataList
            DataRow itemDataRow = _dataList[currentOrderIndex];
            if (movementDirection > 0)
                for (int i = currentOrderIndex; i < newOrderIndex; i++)
                    _dataList[i] = _dataList[i + 1];
            else if (movementDirection < 0)
                for (int i = currentOrderIndex; i > newOrderIndex; i--)
                    _dataList[i] = _dataList[i - 1];
            _dataList[newOrderIndex] = itemDataRow;
        }
        private void CopyItemToIndex(int newIndex, int itemIndex)
        {
            ((ValDataItemUC)signalsFlowLayoutPanel.Controls[newIndex]).categoryLabel.Text = ((ValDataItemUC)signalsFlowLayoutPanel.Controls[itemIndex]).categoryLabel.Text;
            ((ValDataItemUC)signalsFlowLayoutPanel.Controls[newIndex])._ignoreEvent = true;
            ((ValDataItemUC)signalsFlowLayoutPanel.Controls[newIndex]).orderTextBox.Text = ((ValDataItemUC)signalsFlowLayoutPanel.Controls[itemIndex]).orderTextBox.Text;
            ((ValDataItemUC)signalsFlowLayoutPanel.Controls[newIndex])._ignoreEvent = false;
        }

        private void shuffleButton_Click(object sender, EventArgs e)
        {
            // Shuffle all records first
            GeneralTools.Shuffle(_dataList);
            // Load data
            ReloadData();
        }

        private void holdoutValidationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Enable setting training ration and disable setting folds number of folds
            trainingSetRatioTextBox.Enabled = true;
            numberOfFoldsTextBox.Enabled = false;
            // Reload data
            ReloadData();
        }
        private void crossValidationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Disable setting training ration and enable setting folds number of folds
            trainingSetRatioTextBox.Enabled = false;
            numberOfFoldsTextBox.Enabled = true;
        }

        private void trainingSetRatioTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.textBoxNumberOnly(sender, e);
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
            _trainingSetCapacity = (int)(ratio * _dataList.Count);
        }

        private void numberOfFoldsTextBox_TextChanged(object sender, EventArgs e)
        {
            int foldsNum = 2;
            if (!numberOfFoldsTextBox.Text.Equals("") && !numberOfFoldsTextBox.Text.Equals("."))
                foldsNum = (int)double.Parse(numberOfFoldsTextBox.Text);
            // Set fold capacity
            _foldCapacity = (_dataList.Count / foldsNum) + (_dataList.Count % foldsNum > 0 ? 1 : 0);
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
            if (holdoutValidationRadioButton.Checked)
            {
                // Create only one model data
                ModelData modelData = new ModelData();
                for (int i = 0; i < _dataList.Count; i++)
                    // Check if this signal belongs to training set
                    if (i < _trainingSetCapacity)
                        modelData.TrainingData.Add(GeneralTools.ByteArrayToObject<ARTHTFeatures>(_dataList[i].Field<byte[]>("features")));
                    else
                        modelData.ValidationData.Add(GeneralTools.ByteArrayToObject<ARTHTFeatures>(_dataList[i].Field<byte[]>("features")));
                data.Add(modelData);
                // Insert validation info
                validationInfo = "(Holdout validation, " + Math.Round(_trainingSetCapacity / (double)_dataList.Count * 100, 0) + "% training)";
            }
            else if (crossValidationRadioButton.Checked)
            {
                // Separete data into folds
                List<ARTHTFeatures>[] dataFolds = new List<ARTHTFeatures>[(_dataList.Count / _foldCapacity) + (_dataList.Count % _foldCapacity > 0 ? 1 : 0)];
                int selectedFold;
                for (int i = 0; i < _dataList.Count; i++)
                {
                    selectedFold = i / _foldCapacity;
                    // Check if list of the selected fold is not created
                    if (dataFolds[selectedFold] == null)
                        // Create a new one
                        dataFolds[selectedFold] = new List<ARTHTFeatures>();
                    // Insert the signal into the selected fold
                    dataFolds[selectedFold].Add(GeneralTools.ByteArrayToObject<ARTHTFeatures>(_dataList[i].Field<byte[]>("features")));
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
                            foreach (ARTHTFeatures signalFeatures in dataFolds[j])
                                modelData.ValidationData.Add(signalFeatures);
                        else
                            // Insert fold data into training data
                            foreach (ARTHTFeatures signalFeatures in dataFolds[j])
                                modelData.TrainingData.Add(signalFeatures);
                    }
                    // Insert the new model data into data
                    data.Add(modelData);
                }
                // Insert validation info
                validationInfo = "(Cross validation, " + dataFolds.Length + " folds)";
            }

            // Start validation
            Thread validationThread = new Thread(() => _ValidateModel(data, validationInfo));
            validationThread.Start();

            // Close this window
            Close();
        }
    }
}

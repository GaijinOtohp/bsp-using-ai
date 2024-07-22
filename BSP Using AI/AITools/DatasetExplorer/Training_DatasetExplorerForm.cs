using Biological_Signal_Processing_Using_AI.AITools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.CharacteristicWavesDelineation;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;

namespace BSP_Using_AI.AITools.DatasetExplorer
{
    public partial class DatasetExplorerForm
    {
        public ObjectiveBaseModel _objectiveModel;

        public long _datasetSize { get; set; }
        public int _updatesNum;
        public AIToolsForm _aIToolsForm;

        public static (string selection, object[] selectionArgs) SelectDataFromIntervals(List<IdInterval> dataIdsIntervalsList, string appendSelection, object[] appendSelArgs)
        {
            if (appendSelArgs == null)
                appendSelArgs = new object[0];
            
            int totalIntervals = 1;
            if (dataIdsIntervalsList.Count > 0)
                totalIntervals += dataIdsIntervalsList.Count;

            int intervalsNum = 0;
            object[] selectionArgs = new object[totalIntervals * 2 + appendSelArgs.Length];
            string selection = "(_id>=? and _id<=?";
            selectionArgs[intervalsNum] = 0;
            selectionArgs[intervalsNum + 1] = 0;
            intervalsNum += 1;
            foreach (IdInterval datasetInterval in dataIdsIntervalsList)
            {
                selection += " or _id>=? and _id<=?";
                selectionArgs[intervalsNum * 2] = datasetInterval.starting;
                selectionArgs[intervalsNum * 2 + 1] = datasetInterval.ending;
                intervalsNum += 1;
            }
            selection += ") ";

            // Append the selection arguments
            if (appendSelArgs != null)
            {
                selection += appendSelection;
                for (int i = 0; i < appendSelArgs.Length; i++)
                    selectionArgs[intervalsNum * 2 + i] = appendSelArgs[i];
            }

            return (selection , selectionArgs);
        }

        private void holdRecordReport_Training_Item(/*DataTable dataTable, */DataRow row, DatasetFlowLayoutPanelItemUserControl trainingItem)
        {
            // Check the selection of the signal and disable featuresDetailsButton
            trainingItem.selectionCheckBox.Checked = true;
            trainingItem.featuresDetailsButton.Enabled = false;

            /*// Check if this data is from "dataset" table (which is for ARTHT training data)
            if (dataTable.TableName.Equals("dataset"))
                holdRecordReport_Training_ARTHT(row, trainingItem);*/

            // Check if its _id is included in _trainingDetails
            foreach (List<IdInterval> training in _objectiveModel.DataIdsIntervalsList)
                foreach (IdInterval datasetInterval in training)
                    if (row.Field<long>("_id") >= datasetInterval.starting && row.Field<long>("_id") <= datasetInterval.ending)
                    {
                        // If yes then disable the UserControl 
                        trainingItem.Enabled = false;
                        trainingItem.selectionCheckBox.Checked = false;
                    }
        }

        public void holdRecordReport_Training(DataTable dataTable, string callingClassName)
        {
            // Check if this data is for training ARTHT models CWDReinforcementL
            if (callingClassName.Contains("ARTHT"))
                holdRecordReport_ARTHT(dataTable, callingClassName);
            else if (callingClassName.Contains("CWDReinforcementL"))
                holdRecordReport_CWDReinforcementL(dataTable, callingClassName);
            else if (callingClassName.Contains("CWDLSTM"))
                holdRecordReport_CWDLSTM(dataTable, callingClassName);
        }

        private void fitSelectionButton_Click_Training(object sender, EventArgs e)
        {
            // Create a list from signalsFlowLayoutPanel and sort it according to item._id
            List<(bool enabled, bool selected, long id)> signalsList = new List<(bool enabled, bool selected, long id)>();
            foreach (DatasetFlowLayoutPanelItemUserControl item in signalsFlowLayoutPanel.Controls)
                signalsList.Add((item.Enabled, item.selectionCheckBox.Checked, item._id));
            // Sort the list according to _id
            signalsList.Sort((e1, e2) => { return e1.id.CompareTo(e2.id); });

            // Set selected signals in _trainingDetails
            List<IdInterval> datasetIntervalsList = new List<IdInterval>();
            datasetIntervalsList.Add(new IdInterval() { starting = -1, ending = -1 });
            // Iterate through all signals in signalsFlowLayoutPanel
            foreach ((bool enabled, bool selected, long id) item in signalsList)
                // Check if current item is enabled
                if (item.enabled)
                {
                    // If yes then check if it's selected for training
                    if (item.selected)
                    {
                        // If yes then check if this should be in the start of the interval or end
                        if (datasetIntervalsList[datasetIntervalsList.Count - 1].starting == -1)
                        {
                            // Then start and end of the interval
                            datasetIntervalsList[datasetIntervalsList.Count - 1].starting = item.id;
                            datasetIntervalsList[datasetIntervalsList.Count - 1].ending = item.id;
                        }
                        else
                            // Update the end of the interval
                            datasetIntervalsList[datasetIntervalsList.Count - 1].ending = item.id;
                    }
                    else
                    {
                        // If yes then check if last interval is set
                        if (datasetIntervalsList[datasetIntervalsList.Count - 1].ending != -1)
                            // If yes then start new interval
                            datasetIntervalsList.Add(new IdInterval() { starting = -1, ending = -1 });
                    }
                }
                else
                {
                    // If yes then check if last interval is set
                    if (datasetIntervalsList[datasetIntervalsList.Count - 1].ending != -1)
                        // If yes then start new interval
                        datasetIntervalsList.Add(new IdInterval() { starting = -1, ending = -1 });
                }

            // Remove last interval if it's not defined
            if (datasetIntervalsList[datasetIntervalsList.Count - 1].ending == -1)
                datasetIntervalsList.RemoveAt(datasetIntervalsList.Count - 1);

            // Insert new intervals in _trainingDetails if there exist any interval
            if (datasetIntervalsList.Count > 0)
                _objectiveModel.DataIdsIntervalsList.Add(datasetIntervalsList);

            // Check which objective is this data for
            if (_objectiveModel is ARTHTModels)
                fitSelectionButton_Click_ARTHT(sender, e);
            else if (_objectiveModel is CWDReinforcementL)
                fitSelectionButton_Click_CWD("CWDReinforcementL");
            else if (_objectiveModel is CWDLSTM)
                fitSelectionButton_Click_CWD("CWDLSTM");

            // Close form
            this.Close();
        }
    }
}

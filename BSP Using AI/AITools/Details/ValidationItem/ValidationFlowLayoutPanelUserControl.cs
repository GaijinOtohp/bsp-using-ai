using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using BSP_Using_AI.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools.Details
{
    public partial class ValidationFlowLayoutPanelUserControl : UserControl, DbStimulatorReportHolder
    {
        float[] OutputsThresholds;

        public ValidationFlowLayoutPanelUserControl(float[] outputsThresholds)
        {
            InitializeComponent();

            OutputsThresholds = outputsThresholds;
        }

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void thresholdTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.textBoxNumberOnly(sender, e);
        }

        private void thresholdTextBox_TextChanged(object sender, EventArgs e)
        {
            if (thresholdTextBox.Text.Length > 0)
                for (int i = 0; i < OutputsThresholds.Length; i++)
                    OutputsThresholds[i] = float.Parse(thresholdTextBox.Text);
        }

        private void ValidationFlowLayoutPanelUserControl_Click(object sender, EventArgs e)
        {
            queryForSelectedDataset(((DetailsForm)this.FindForm())._aRTHTModels.DataIdsIntervalsList, this);
        }

        public static void queryForSelectedDataset(List<List<long[]>> dataIdsIntervalsList, DbStimulatorReportHolder dbStimulatorReportHolder)
        {
            // Qurey for signals features in all selected intervals from dataset
            string selection = "_id>=? and _id<=?";
            int intervalsNum = 1;
            foreach (List<long[]> training in dataIdsIntervalsList)
                intervalsNum += training.Count;
            object[] selectionArgs = new object[intervalsNum * 2];
            intervalsNum = 0;
            selectionArgs[intervalsNum] = 0;
            selectionArgs[intervalsNum + 1] = 0;
            foreach (List<long[]> training in dataIdsIntervalsList)
                foreach (long[] datasetInterval in training)
                {
                    intervalsNum += 2;
                    selection += " or _id>=? and _id<=?";
                    selectionArgs[intervalsNum] = datasetInterval[0];
                    selectionArgs[intervalsNum + 1] = datasetInterval[1];
                }

            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(dbStimulatorReportHolder);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("dataset",
                                        new String[] { "features" },
                                        selection,
                                        selectionArgs,
                                        "", "ValidationFlowLayoutPanelUserControl"));
            dbStimulatorThread.Start();
        }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //:::::::::::::::::::::::::::CROSS PROCESS FORM FUNCTIONS (INTERFACES)::::::::::::::::::::::://
        public void holdRecordReport(DataTable dataTable, string callingClassName)
        {
            if (!callingClassName.Equals("ValidationFlowLayoutPanelUserControl"))
                return;

            // Initialize list of features for selected step
            List<Sample> dataList = new List<Sample>();

            // Get features of only the selected step
            string stepName = Name;

            // Iterate through each signal features and sort them in featuresLists
            ARTHTFeatures aRTHTFeatures = null;
            foreach (DataRow row in dataTable.AsEnumerable())
            {
                aRTHTFeatures = Garage.ByteArrayToObject<ARTHTFeatures>(row.Field<byte[]>("features"));

                foreach (Sample sample in aRTHTFeatures.StepsDataDic[stepName].Samples)
                    dataList.Add(sample);
            }

            // Send data to DataVisualisationForm
            DataVisualisationForm dataVisualisationForm = new DataVisualisationForm(((DetailsForm)this.FindForm())._tFBackThread._arthtModelsDic,
                                                                                    ((DetailsForm)this.FindForm())._aRTHTModels.ModelName, ((DetailsForm)this.FindForm())._aRTHTModels.ProblemName,
                                                                                    ((DetailsForm)this.FindForm())._modelId, stepName, dataList);
            dataVisualisationForm.stepLabel.Text = modelTargetLabel.Text;
            this.Invoke(new MethodInvoker(delegate () { dataVisualisationForm.Show(); }));
        }
    }
}

using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using BSP_Using_AI.Database;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BSP_Using_AI.AITools.Details
{
    public partial class ValidationFlowLayoutPanelUserControl : UserControl, DbStimulatorReportHolder
    {
        public ValidationFlowLayoutPanelUserControl()
        {
            InitializeComponent();
        }

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void thresholdTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.textBoxNumberOnly(sender, e);
        }

        private void ValidationFlowLayoutPanelUserControl_Click(object sender, EventArgs e)
        {
            queryFeatures();
        }

        private void queryFeatures()
        {
            // Qurey for signals features in all selected intervals from dataset
            string selection = "_id>=? and _id<=?";
            int intervalsNum = 1;
            foreach (List<long[]> training in ((DetailsForm)this.FindForm())._trainingDetails)
                intervalsNum += training.Count;
            object[] selectionArgs = new object[intervalsNum * 2];
            intervalsNum = 0;
            selectionArgs[intervalsNum] = 0;
            selectionArgs[intervalsNum + 1] = 0;
            foreach (List<long[]> training in ((DetailsForm)this.FindForm())._trainingDetails)
                foreach (long[] datasetInterval in training)
                {
                    intervalsNum += 2;
                    selection += " or _id>=? and _id<=?";
                    selectionArgs[intervalsNum] = datasetInterval[0];
                    selectionArgs[intervalsNum + 1] = datasetInterval[1];
                }

            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
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
            List<object[]> featuresList = new List<object[]>();

            // Get features of only the selected step
            int step = ((FlowLayoutPanel)this.Parent).Controls.IndexOf(this) + 1; // The first item is just beats states

            // Iterate through each signal features and sort them in featuresLists
            OrderedDictionary signalFeaturesOrderedDictionary = null;
            foreach (DataRow row in dataTable.AsEnumerable())
            {
                signalFeaturesOrderedDictionary = (OrderedDictionary)Garage.ByteArrayToObject(row.Field<byte[]>("features"));
                object[] stepFeatures = null;
                if (signalFeaturesOrderedDictionary.Count > step)
                    stepFeatures = (object[])signalFeaturesOrderedDictionary[step];
                else
                    continue;

                if (step == 1)
                    featuresList.Add(stepFeatures);
                else if (step == 4)
                    foreach (List<object[]> beat in stepFeatures)
                        foreach (object[] feature in beat)
                            featuresList.Add(feature);
                else
                    foreach (object[] feature in stepFeatures)
                        if (feature[0] != null)
                            featuresList.Add(feature);
                
            }

            // Send data to DataVisualisationForm
            DataVisualisationForm dataVisualisationForm = new DataVisualisationForm(((DetailsForm)this.FindForm())._tFBackThread._targetsModelsHashtable, ((DetailsForm)this.FindForm())._modelName,
                                                                                    ((DetailsForm)this.FindForm())._modelId, step - 1, featuresList);
            dataVisualisationForm.stepLabel.Text = modelTargetLabel.Text;
            this.Invoke(new MethodInvoker(delegate () { dataVisualisationForm.Show(); }));
        }
    }
}

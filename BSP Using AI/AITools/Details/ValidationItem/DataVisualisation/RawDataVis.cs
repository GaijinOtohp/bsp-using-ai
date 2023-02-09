using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation
{
    partial class DataVisualisationForm
    {
        private bool _ignoreEvent = false;
        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        private void setRawVisTab()
        {
            if (DataList.Count == 0)
                return;

            Random rand = new Random();
            // Set list boxes with model variables and outputes
            foreach (string featureName in DataList[0].DataParent.FeaturesLabelsIndx.Keys)
            {
                yInputFlowLayoutPanel.Controls.Add(createCheckBox("Feature: " + featureName, featureName, rawDataVisCheckBox_CheckedChanged));
                xInputFlowLayoutPanel.Controls.Add(createCheckBox("Feature: " + featureName, featureName, rawDataVisCheckBox_CheckedChanged));
            }
            foreach (string outputName in DataList[0].DataParent.OutputsLabelsIndx.Keys)
            {
                yInputFlowLayoutPanel.Controls.Add(createCheckBox("Output: " + outputName, outputName, rawDataVisCheckBox_CheckedChanged));
                xInputFlowLayoutPanel.Controls.Add(createCheckBox("Output: " + outputName, outputName, rawDataVisCheckBox_CheckedChanged));

                // If the selected model is for classification
                if (_stepName.Equals(ARTHTNamings.Step2RPeaksSelectionData) || _stepName.Equals(ARTHTNamings.Step4PTSelectionData) ||
                    _stepName.Equals(ARTHTNamings.Step5ShortPRScanData) || _stepName.Equals(ARTHTNamings.Step7DeltaExaminationData))
                    for (int j = 0; j < 2; j++)
                    {
                        RawVisItemUserControl rawVisItemUserControl = new RawVisItemUserControl(outputName + " (" + j + ")");
                        rawVisItemUserControl.Name = outputName;
                        rawVisItemUserControl.Tag = j;

                        // If yes then pick a different color number
                        Color color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                        rawVisItemUserControl.primaryColorButton.BackColor = color;
                        rawVisItemUserControl.secondaryColorButton.BackColor = Color.FromArgb(color.R - 60 > 0 ? color.R - 60 : 0, color.G - 60 > 0 ? color.G - 60 : 0, color.B - 60 > 0 ? color.B - 60 : 0);
                        outputFlowLayoutPanel.Controls.Add(rawVisItemUserControl);
                    }
                // If the selected model is for regression
                else
                {
                    RawVisItemUserControl rawVisItemUserControl = new RawVisItemUserControl(outputName);
                    rawVisItemUserControl.Name = outputName;

                    // If yes then pick a different color number
                    Color color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                    rawVisItemUserControl.primaryColorButton.BackColor = color;
                    rawVisItemUserControl.secondaryColorButton.BackColor = Color.FromArgb(color.R - 60 > 0 ? color.R - 60 : 0, color.G - 60 > 0 ? color.G - 60 : 0, color.B - 60 > 0 ? color.B - 60 : 0);
                    outputFlowLayoutPanel.Controls.Add(rawVisItemUserControl);
                }
            }
        }

        private void rawDataVisCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_ignoreEvent)
                return;

            _ignoreEvent = true;
            CheckBox rawDataVisCheckBox = (sender as CheckBox);
            FlowLayoutPanel parentFlowLayout = (FlowLayoutPanel)rawDataVisCheckBox.Parent;
            // Uncheck other items
            foreach (CheckBox otherCheckBox in parentFlowLayout.Controls)
                if (otherCheckBox.Checked && otherCheckBox != rawDataVisCheckBox)
                    otherCheckBox.Checked = false;
            _ignoreEvent = false;

            // Refresh chart
            refreshRawChart();
        }

        public void refreshRawChart()
        {
            double[] yAxisVals = new double[DataList.Count];
            double[] xAxisVals = new double[DataList.Count];
            float[] colorVals = new float[DataList.Count];

            // Remove all series from the chart
            rawChart.Series.Clear();
            // Enable outputsFlowLayoutPanel
            outputFlowLayoutPanel.Enabled = true;

            // Get the selected item in yInputCheckedListBox
            CheckBox yInputSelectedCheckBox = new CheckBox();
            foreach (CheckBox checkBox in yInputFlowLayoutPanel.Controls)
                if (checkBox.Checked)
                {
                    yInputSelectedCheckBox = checkBox;
                    break;
                }
            // Check if output is selected in Y axis
            bool yOutputSelected = false;
            if (yInputSelectedCheckBox.Text.Contains("Output"))
                // If yes then output is selected
                yOutputSelected = true;

            // Get the selected item in xInputCheckedListBox
            CheckBox xInputSelectedCheckBox = new CheckBox();
            foreach (CheckBox checkBox in xInputFlowLayoutPanel.Controls)
                if (checkBox.Checked)
                {
                    xInputSelectedCheckBox = checkBox;
                    break;
                }
            // Check if output is selected in Y axis
            bool xOutputSelected = false;
            if (xInputSelectedCheckBox.Text.Contains("Output"))
                // If yes then output is selected
                xOutputSelected = true;

            // Check if there is no selected item
            if (!yInputSelectedCheckBox.Checked && !xInputSelectedCheckBox.Checked)
                // If yes then just return
                return;

            // Disable outputsFlowLayoutPanel if one of the outputs is selected
            if (yOutputSelected || xOutputSelected)
                outputFlowLayoutPanel.Enabled = false;
            // Set the new series in the chart
            // Check if output is selected in both axis
            if (yOutputSelected || xOutputSelected)
            {
                // If yes then there will be only one series with "output" as label
                addNewSeries(rawChart, xInputSelectedCheckBox.Tag + " -> " + yInputSelectedCheckBox.Tag, Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0));
                // Copy values to yAxisVals and xAxisVals
                if (yOutputSelected && xOutputSelected)
                    for (int i = 0; i < yAxisVals.Length; i++)
                    {
                        yAxisVals[i] = DataList[i].getOutputByLabel((string)yInputSelectedCheckBox.Tag);
                        xAxisVals[i] = DataList[i].getOutputByLabel((string)xInputSelectedCheckBox.Tag);
                    }
                else if (yOutputSelected)
                    for (int i = 0; i < yAxisVals.Length; i++)
                    {
                        yAxisVals[i] = DataList[i].getOutputByLabel((string)yInputSelectedCheckBox.Tag);
                        xAxisVals[i] = DataList[i].getFeatureByLabel((string)xInputSelectedCheckBox.Tag);
                    }
                else if (xOutputSelected)
                    for (int i = 0; i < yAxisVals.Length; i++)
                    {
                        yAxisVals[i] = DataList[i].getFeatureByLabel((string)yInputSelectedCheckBox.Tag);
                        xAxisVals[i] = DataList[i].getOutputByLabel((string)xInputSelectedCheckBox.Tag);
                    }
                // Insert values in chart
                Garage.loadXYInChart(rawChart, xAxisVals, yAxisVals, null, 0, 0, "DataVisualisationForm");
            }
            else
            {
                // Check if the selected model is for classification
                if (_stepName.Equals(ARTHTNamings.Step2RPeaksSelectionData) || _stepName.Equals(ARTHTNamings.Step4PTSelectionData) ||
                    _stepName.Equals(ARTHTNamings.Step5ShortPRScanData) || _stepName.Equals(ARTHTNamings.Step7DeltaExaminationData))
                {
                    // If yes then create yAxisVals and xAxisVals of each selected output
                    foreach (RawVisItemUserControl rawVisItemUserControl in outputFlowLayoutPanel.Controls)
                        if (rawVisItemUserControl.outputCheckBox.Checked)
                        {
                            List<Sample> selectedSamples = new List<Sample>();
                            foreach (Sample sample in DataList)
                                // Check if the selected sample's output is of same value as rawVisItemUserControl1.Tag (0 for 0s and 1 for 1s)
                                if (sample.getOutputByLabel(rawVisItemUserControl.Name) == (int)rawVisItemUserControl.Tag)
                                    // If yes then insert the sample in selectedSamples
                                    selectedSamples.Add(sample);

                            // Now copy the selected input values
                            yAxisVals = new double[selectedSamples.Count];
                            xAxisVals = new double[selectedSamples.Count];
                            for (int i = 0; i < selectedSamples.Count; i++)
                            {
                                yAxisVals[i] = selectedSamples[i].getFeatureByLabel((string)yInputSelectedCheckBox.Tag);
                                xAxisVals[i] = selectedSamples[i].getFeatureByLabel((string)xInputSelectedCheckBox.Tag);
                            }

                            // Set the new series of the selected output
                            addNewSeries(rawChart, rawVisItemUserControl.outputCheckBox.Text, rawVisItemUserControl.primaryColorButton.BackColor, rawVisItemUserControl.secondaryColorButton.BackColor);
                            // Insert values in chart
                            Garage.loadXYInChart(rawChart, xAxisVals, yAxisVals, null, 0, rawChart.Series.Count - 1, "DataVisualisationForm");
                        }
                }
                else
                {
                    // If yes then create yAxisVals and xAxisVals of each selected output
                    foreach (RawVisItemUserControl rawVisItemUserControl in outputFlowLayoutPanel.Controls)
                        if (rawVisItemUserControl.outputCheckBox.Checked)
                        {
                            // If yes then copy all of the selected input values
                            for (int i = 0; i < DataList.Count; i++)
                            {
                                yAxisVals[i] = DataList[i].getFeatureByLabel((string)yInputSelectedCheckBox.Tag);
                                xAxisVals[i] = DataList[i].getFeatureByLabel((string)xInputSelectedCheckBox.Tag);
                                colorVals[i] = (float)DataList[i].getOutputByLabel(rawVisItemUserControl.Name);
                            }

                            // Normalize colorVals
                            (float mean, float min, float max) meanMinMax = Garage.MeanMinMax(colorVals);
                            float interval = meanMinMax.max - meanMinMax.min;
                            for (int i = 0; i < colorVals.Length; i++)
                                colorVals[i] = (colorVals[i] - meanMinMax.min) / interval;

                            // Set the new series of the selected output
                            addNewSeries(rawChart, rawVisItemUserControl.outputCheckBox.Text, rawVisItemUserControl.primaryColorButton.BackColor, rawVisItemUserControl.secondaryColorButton.BackColor);
                            // Insert values in chart
                            Garage.loadXYInChart(rawChart, xAxisVals, yAxisVals, colorVals, 0, rawChart.Series.Count - 1);
                        }
                }
            }
        }
    }
}

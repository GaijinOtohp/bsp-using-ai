using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation
{
    partial class DataVisualisationForm
    {
        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        private void setRawVisTab()
        {
            if (_featuresList.Count == 0)
                return;

            Random rand = new Random();
            // Set list boxes with model variables and outputes
            for (int i = 0; i < ((double[])_featuresList[0][0]).Length; i++)
            {
                yInputCheckedListBox.Items.Add("input " + (i + 1));
                xInputCheckedListBox.Items.Add("input " + (i + 1));
            }
            for (int i = 0; i < ((double[])_featuresList[0][1]).Length; i++)
            {
                yInputCheckedListBox.Items.Add("output " + (i + 1));
                xInputCheckedListBox.Items.Add("output " + (i + 1));

                // If the selected model is for classification
                if (_stepIndx == 1 || _stepIndx == 3 || _stepIndx == 4 || _stepIndx == 6)
                    for (int j = 0; j < 2; j++)
                    {
                        RawVisItemUserControl rawVisItemUserControl = new RawVisItemUserControl((i + 1) + " (" + j + ")");
                        // Check if there is more than one output
                        if (i > 0 || j > 0)
                        {
                            // If yes then pick a different color number
                            Color color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                            rawVisItemUserControl.primaryColorButton.BackColor = color;
                            rawVisItemUserControl.secondaryColorButton.BackColor = Color.FromArgb(color.R - 60 > 0 ? color.R - 60 : 0, color.G - 60 > 0 ? color.G - 60 : 0, color.B - 60 > 0 ? color.B - 60 : 0);
                        }
                        outputFlowLayoutPanel.Controls.Add(rawVisItemUserControl);
                    }
                // If the selected model is for regression
                else
                {
                    RawVisItemUserControl rawVisItemUserControl = new RawVisItemUserControl((i + 1).ToString());
                    // Check if there is more than one output
                    if (i > 0)
                    {
                        // If yes then pick a different color number
                        Color color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                        rawVisItemUserControl.primaryColorButton.BackColor = color;
                        rawVisItemUserControl.secondaryColorButton.BackColor = Color.FromArgb(color.R - 60 > 0 ? color.R - 60 : 0, color.G - 60 > 0 ? color.G - 60 : 0, color.B - 60 > 0 ? color.B - 60 : 0);
                    }
                    outputFlowLayoutPanel.Controls.Add(rawVisItemUserControl);
                }
            }
        }

        public void refreshRawChart()
        {
            double[] yAxisVals = new double[_featuresList.Count];
            double[] xAxisVals = new double[_featuresList.Count];

            // Remove all series from the chart
            rawChart.Series.Clear();
            // Enable outputsFlowLayoutPanel
            outputFlowLayoutPanel.Enabled = true;

            // Get the selected item in yInputCheckedListBox
            int yValsIndx = -1;
            if (yInputCheckedListBox.CheckedIndices.Count > 0)
                yValsIndx = yInputCheckedListBox.CheckedIndices[0];
            // Check if output is selected in Y axis
            bool yOutputSelected = false;
            if ((yValsIndx + 1) - ((double[])_featuresList[0][0]).Length > 0)
                // If yes then output is selected
                yOutputSelected = true;

            // Get the selected item in xInputCheckedListBox
            int xValsIndx = -1;
            if (xInputCheckedListBox.CheckedIndices.Count > 0)
                xValsIndx = xInputCheckedListBox.CheckedIndices[0];
            // Check if output is selected in Y axis
            bool xOutputSelected = false;
            if ((xValsIndx + 1) - ((double[])_featuresList[0][0]).Length > 0)
                // If yes then output is selected
                xOutputSelected = true;

            // Check if there is no selected item
            if (yValsIndx + xValsIndx < -1)
                // If yes then just return
                return;

            // Disable outputsFlowLayoutPanel if one of the outputs is selected
            if (yOutputSelected || xOutputSelected)
                outputFlowLayoutPanel.Enabled = false;
            // Set the new series in the chart
            // Check if output is selected in both axis
            if (yOutputSelected && xOutputSelected)
            {
                // If yes then there will be only one series with "output" as label
                addNewSeries(rawChart, "output", Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0));
                // Copy values to yAxisVals and xAxisVals
                yValsIndx = yValsIndx - ((double[])_featuresList[0][0]).Length;
                xValsIndx = xValsIndx - ((double[])_featuresList[0][0]).Length;
                for (int i = 0; i < yAxisVals.Length; i++)
                {
                    yAxisVals[i] = ((double[])_featuresList[i][1])[yValsIndx];
                    xAxisVals[i] = ((double[])_featuresList[i][1])[xValsIndx];
                }
                // Insert values in chart
                Garage.loadXYInChart(rawChart, xAxisVals, yAxisVals, null, 0, 0, "DataVisualisationForm");
            }
            // Check if one output is selected in Y axis
            else if (yOutputSelected)
            {
                // If yes then there will be only one series with the selected output name
                yValsIndx = yValsIndx - ((double[])_featuresList[0][0]).Length;
                addNewSeries(rawChart, "output " + (yValsIndx + 1), Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0));
                // Copy values to yAxisVals and xAxisVals
                for (int i = 0; i < yAxisVals.Length; i++)
                {
                    if (yValsIndx != -1) yAxisVals[i] = ((double[])_featuresList[i][1])[yValsIndx];
                    if (xValsIndx != -1) xAxisVals[i] = ((double[])_featuresList[i][0])[xValsIndx];
                }
                // Insert values in chart
                Garage.loadXYInChart(rawChart, xAxisVals, yAxisVals, null, 0, 0, "DataVisualisationForm");
            }
            // Check if one output is selected in X axis
            else if (xOutputSelected)
            {
                // If yes then there will be only one series with the selected output name
                xValsIndx = xValsIndx - ((double[])_featuresList[0][0]).Length;
                addNewSeries(rawChart, "output " + (xValsIndx + 1), Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0));
                // Copy values to yAxisVals and xAxisVals
                for (int i = 0; i < yAxisVals.Length; i++)
                {
                    if (yValsIndx != -1) yAxisVals[i] = ((double[])_featuresList[i][0])[yValsIndx];
                    if (xValsIndx != -1) xAxisVals[i] = ((double[])_featuresList[i][1])[xValsIndx];
                }
                // Insert values in chart
                Garage.loadXYInChart(rawChart, xAxisVals, yAxisVals, null, 0, 0, "DataVisualisationForm");
            }
            else
            {
                // Check if the selected model is for classification
                if (_stepIndx == 1 || _stepIndx == 3 || _stepIndx == 4 || _stepIndx == 6)
                {
                    // If yes then create yAxisVals and xAxisVals of each selected output
                    for (int i = 0; i < outputFlowLayoutPanel.Controls.Count; i++)
                        if (((RawVisItemUserControl)outputFlowLayoutPanel.Controls[i]).outputCheckBox.Checked)
                        {
                            List<int> selectedIndxs = new List<int>();
                            for (int j = 0; j < _featuresList.Count; j++)
                                if (((double[])_featuresList[j][1])[i / 2] == i % 2)
                                    // If yes insert selected index in selectedIndxs
                                    selectedIndxs.Add(j);
                            // Now copy the selected input values
                            yAxisVals = new double[selectedIndxs.Count];
                            xAxisVals = new double[selectedIndxs.Count];
                            for (int j = 0; j < selectedIndxs.Count; j++)
                            {
                                if (yValsIndx != -1) yAxisVals[j] = ((double[])_featuresList[selectedIndxs[j]][0])[yValsIndx];
                                if (xValsIndx != -1) xAxisVals[j] = ((double[])_featuresList[selectedIndxs[j]][0])[xValsIndx];
                            }

                            // Set the new series of the selected output
                            RawVisItemUserControl rawVisItemUserControl = (RawVisItemUserControl)outputFlowLayoutPanel.Controls[i];
                            addNewSeries(rawChart, rawVisItemUserControl.outputCheckBox.Text, rawVisItemUserControl.primaryColorButton.BackColor, rawVisItemUserControl.secondaryColorButton.BackColor);
                            // Insert values in chart
                            Garage.loadXYInChart(rawChart, xAxisVals, yAxisVals, null, 0, rawChart.Series.Count - 1, "DataVisualisationForm");
                        }
                }
                else
                {
                    // If yes then create yAxisVals and xAxisVals of each selected output
                    for (int i = 0; i < outputFlowLayoutPanel.Controls.Count; i++)
                        if (((RawVisItemUserControl)outputFlowLayoutPanel.Controls[i]).outputCheckBox.Checked)
                        {
                            // If yes then copy all of the selected input values
                            for (int j = 0; j < _featuresList.Count; j++)
                            {
                                if (yValsIndx != -1) yAxisVals[j] = ((double[])_featuresList[j][0])[yValsIndx];
                                if (xValsIndx != -1) xAxisVals[j] = ((double[])_featuresList[j][0])[xValsIndx];
                            }

                            // Set the new series of the selected output
                            RawVisItemUserControl rawVisItemUserControl = (RawVisItemUserControl)outputFlowLayoutPanel.Controls[i];
                            addNewSeries(rawChart, rawVisItemUserControl.outputCheckBox.Text, rawVisItemUserControl.primaryColorButton.BackColor, rawVisItemUserControl.secondaryColorButton.BackColor);
                            // Insert values in chart
                            Garage.loadXYInChart(rawChart, xAxisVals, yAxisVals, null, 0, rawChart.Series.Count - 1, "DataVisualisationForm");
                        }
                }
            }
        }
    }
}

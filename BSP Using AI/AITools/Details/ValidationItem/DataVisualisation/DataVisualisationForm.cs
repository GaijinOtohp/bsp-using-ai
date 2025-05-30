﻿using Biological_Signal_Processing_Using_AI.Garage;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation
{
    public partial class DataVisualisationForm : Form
    {
        public Dictionary<string, ObjectiveBaseModel> _ObjectivesModelsDic = null;

        private ObjectiveBaseModel _ObjectiveModel;

        private CustomArchiBaseModel _InnerObjectiveModel;

        public AIBackThreadReportHolder _ValidationItemUserControl;

        long _modelId;

        private List<Sample> DataList { get; set; }
        private long _datasetSize;

        int _firstMouseX;
        int _firstMouseY;

        public DataVisualisationForm(Dictionary<string, ObjectiveBaseModel> objectivesModelsDic, ObjectiveBaseModel objectiveModel, CustomArchiBaseModel innerObjectiveModel, long modelId, List<Sample> dataList, long datasetSize)
        {
            InitializeComponent();

            _ObjectivesModelsDic = objectivesModelsDic;
            _ObjectiveModel = objectiveModel;
            _InnerObjectiveModel = innerObjectiveModel;
            _modelId = modelId;
            DataList = dataList;
            _datasetSize = datasetSize;
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public void SetTabs()
        {
            setRawVisTab();
            setPCAVisTab();
            setValidationDetailsVisTab();
        }

        private void addNewSeries(FormsPlot formsPlot, double[] xAxisVals, double[] yAxisVals, double[] colorVals, string seriesName, Color primaryColor, Color secondaryColor)
        {
            if (colorVals == null)
                formsPlot.Plot.AddScatter(xAxisVals, yAxisVals, primaryColor, 0, 10, MarkerShape.filledCircle, LineStyle.None, seriesName);
            //plot.AddBubblePlot(null, null, 10, primaryColor, 2, secondaryColor);
            else
            {
                formsPlot.Plot.AddColorbar(ScottPlot.Drawing.Colormap.Turbo);
                for (int i = 0; i < xAxisVals.Length; i++)
                {
                    System.Drawing.Color color = ScottPlot.Drawing.Colormap.Turbo.GetColor(colorVals[i]);
                    formsPlot.Plot.AddPoint(xAxisVals[i], yAxisVals[i], color, 10);
                }
            }

            formsPlot.Plot.Legend();
        }

        public static CheckBox createCheckBox(string text, object tag, EventHandler eventHandler)
        {
            CheckBox checkBox = new CheckBox() { Text = text };
            checkBox.MinimumSize = new Size(147, 25);
            checkBox.AutoSize = true;
            checkBox.Tag = tag;
            checkBox.CheckedChanged += new EventHandler(eventHandler);

            return checkBox;
        }
    }
}

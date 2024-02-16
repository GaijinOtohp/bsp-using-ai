using Biological_Signal_Processing_Using_AI.Garage;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation
{
    public partial class DataVisualisationForm : Form
    {
        public Dictionary<string, ARTHTModels> _arthtModelsDic = null;

        string _ModelName;
        string _ProblemName;
        long _modelId;
        string _stepName;

        private List<Sample> DataList;

        bool _mouseDown = false;
        int _previousMouseX;
        int _previousMouseY;
        int _firstMouseX;
        int _firstMouseY;

        public DataVisualisationForm(Dictionary<string, ARTHTModels> arthtModelsDic, string modelName, string problemName, long modelId, string stepName, List<Sample> dataList)
        {
            InitializeComponent();

            _arthtModelsDic = arthtModelsDic;
            _ModelName = modelName;
            _ProblemName = problemName;
            _modelId = modelId;
            _stepName = stepName;
            DataList = dataList;

            setRawVisTab();
            setPCAVisTab();
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
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

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//

        private void button1_Click(object sender, EventArgs e)
        {
            GeneralTools.saveChartAsImage(rawChart);
        }
    }
}

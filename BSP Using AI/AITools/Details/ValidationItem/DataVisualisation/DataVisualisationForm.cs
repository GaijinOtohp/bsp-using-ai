using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
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
        private void addNewSeries(Chart chart, string seriesName, Color primaryColor, Color secondaryColor)
        {
            chart.Series.Add(seriesName);
            chart.Series[chart.Series.Count - 1].ChartType = SeriesChartType.Point;
            chart.Series[chart.Series.Count - 1].MarkerStyle = MarkerStyle.Circle;
            chart.Series[chart.Series.Count - 1].MarkerSize = 10;
            chart.Series[chart.Series.Count - 1].MarkerBorderWidth = 2;
            chart.Series[chart.Series.Count - 1].MarkerColor = primaryColor;
            chart.Series[chart.Series.Count - 1].MarkerBorderColor = secondaryColor;
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
        private void rawChart_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;
            _previousMouseX = e.X;
            _previousMouseY = e.Y;
            _firstMouseX = e.X;
            _firstMouseY = e.Y;
        }

        private void rawChart_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown)
            {
                EventHandlers.signalExhibitor_MouseMove(sender, e, _previousMouseX, _previousMouseY);
                _previousMouseX = e.X;
                _previousMouseY = e.Y;
            }
        }

        private void rawChart_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }

        private void signalExhibitor_MouseWheel(object sender, MouseEventArgs e)
        {
            EventHandlers.signalExhibitor_MouseWheel(sender, e, _previousMouseX, _previousMouseY);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Garage.saveChartAsImage(rawChart);
        }
    }
}

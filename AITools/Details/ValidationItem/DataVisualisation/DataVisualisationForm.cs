using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation
{
    public partial class DataVisualisationForm : Form
    {
        private List<double[]>[] _pcLoadingScoresArray;
        private Hashtable _targetsModelsHashtable = null;

        string _modelName;
        long _modelId;
        int _stepIndx;

        private List<object[]> _featuresList;

        bool _mouseDown = false;
        int _previousMouseX;
        int _previousMouseY;
        int _firstMouseX;
        int _firstMouseY;

        public DataVisualisationForm(Hashtable targetsModelsHashtable, string modelName, long modelId, int stepIndx, List<object[]> featuresList)
        {
            InitializeComponent();

            _targetsModelsHashtable = targetsModelsHashtable;
            _modelName = modelName;
            _modelId = modelId;
            _stepIndx = stepIndx;
            _featuresList = featuresList;

            _pcLoadingScoresArray = new List<double[]>[7];
            for (int i = 0; i < ((List<object[]>)_targetsModelsHashtable[_modelName]).Count; i++)
                _pcLoadingScoresArray[i] = (List<double[]>)((List<object[]>)_targetsModelsHashtable[_modelName])[i][1];

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

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void yInputCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Uncheck other items
            foreach (int indx in yInputCheckedListBox.CheckedIndices)
                if (yInputCheckedListBox.Items[indx] != yInputCheckedListBox.SelectedItem)
                    yInputCheckedListBox.SetItemChecked(indx, false);
            foreach (int indx in xInputCheckedListBox.CheckedIndices)
                if (xInputCheckedListBox.Items[indx] != xInputCheckedListBox.SelectedItem)
                    xInputCheckedListBox.SetItemChecked(indx, false);

            // Refresh chart
            refreshRawChart();
        }

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
    }
}

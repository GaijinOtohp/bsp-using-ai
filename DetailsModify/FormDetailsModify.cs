using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using BSP_Using_AI.AITools;
using BSP_Using_AI.DetailsModify.Filters;
using BSP_Using_AI.DetailsModify.SignalFusion;
using BSP_Using_AI.DetailsModify.Transforms.DWT;
using BSP_Using_AI.SignalHolderFolder;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.DetailsModify
{
    public partial class FormDetailsModify : Form
    {
        public SignalHolder _signalHolder;

        public double _samplingRate;
        public double _quantizationStep;
        public double _startingInSec;

        public double[] _samples;
        public double[] _filteredSamples;
        public object[] _dwtLevelsSamples;

        private List<int>[] _tempStatesList = new List<int>[] { new List<int>(), new List<int>(), new List<int>(), new List<int>() }; // {list for up-peaks, down-peaks, stable, selection}

        public object[] _dwtSpecs;

        public Hashtable _filteresHashtable;
        public OrderedDictionary _featuresOrderedDictionary;

        bool _predictionOn = false;
        public Hashtable _targetsModelsHashtable = null;

        bool _mouseDown = false;
        int _previousMouseX;
        int _previousMouseY;

        public long _id;
        public FormDetailsModify(double[] samples, double samplingRate, double quantizationStep, String path, double startingInSec)
        {
            InitializeComponent();

            // Initialize the discrete wavelet transform
            // {Wavelet type index, number of vanishing moments index, get all layers bool, maximum layer integer}
            _dwtSpecs = new object[4] { 0, 0, "haar", int.MaxValue };

            // Initialize form
            _filteresHashtable = new Hashtable();
            _featuresOrderedDictionary = new OrderedDictionary();
            _samples = samples;
            _samplingRate = samplingRate;
            _quantizationStep = quantizationStep;
            _startingInSec = startingInSec;
            pathLabel.Text = path;
            samplingRateTextBox.Text = samplingRate.ToString();
            quantizationStepTextBox.Text = quantizationStep.ToString();

            signalsPickerComboBox.Text = signalsPickerComboBox.Items[1].ToString();
            aiGoalComboBox.SelectedIndex = 0;

            // Load stuff on form
            if (samples != null)
                loadClass(samples, samplingRate, startingInSec);
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public void initializeForm(SignalHolder signalHolder)
        {
            _signalHolder = signalHolder;

            _filteredSamples = _signalHolder._filteredSamples;
            _filteresHashtable = _signalHolder._filteresHashtable;
            _featuresOrderedDictionary = _signalHolder._featuresOrderedDictionary;

            // Check if there is features selected
            if (_featuresOrderedDictionary.Count > 0)
            {
                initializeAIFilters();
                initializeAITools();
                previousButton_Click(null, null);
            }
            else
            {
                // Add filters in filtersFlowLayoutPanel
                foreach (string key in _filteresHashtable.Keys)
                {
                    // Make a variable for the cotrol to be added in the filtersFlowLayoutPanel
                    Control filterControl = null;
                    object[] filterObject = (object[])_filteresHashtable[key];

                    // Check which filter is selected
                    switch ((string)filterObject[0])
                    {
                        case "DC removal":
                            // for DC removal

                            // Set new DCRemovalUserControl with the key as its name in filterControl
                            filterControl = new Filters.DCRemovalUserControl();
                            filterControl.Name = key;
                            // Add the new filter
                            filtersFlowLayoutPanel.Controls.Add(filterControl);

                            // Set filterObject with its value
                            ((Filters.DCRemovalUserControl)filterControl).dcValueRemoveCheckBox.Checked = Convert.ToBoolean(filterObject[1]);
                            break;
                        case "Normalize signal":
                            // for Normilizing signal

                            // Set new DCRemovalUserControl with the key as its name in filterControl
                            filterControl = new NormalizedSignalUserControl();
                            filterControl.Name = key;

                            // Set filterObject with the value 0
                            filterObject = new object[] { "Normalize signal", 0, 0 };
                            break;
                        case "Absolute signal":
                            // for DC removal

                            // Set new DCRemovalUserControl with the key as its name in filterControl
                            filterControl = new AbsoluteSignalUserControl();
                            filterControl.Name = key;

                            // Set filterObject with the value 0
                            filterObject = new object[] { "Absolute signal", 0, 0 };
                            break;
                        case "Singal states viewer":
                            break;
                        default:
                            // This is for IIR filters
                            // Set new IIR filter with the key as its name in filterControl
                            filterControl = new DetailsModify.Filters.IIRFilters.IIRFilterUserControl(_samplingRate);
                            filterControl.Name = key;
                            // Set the label of the filter
                            ((DetailsModify.Filters.IIRFilters.IIRFilterUserControl)filterControl).nameFilterLabel.Text = (string)filterObject[0] + " filter";
                            // Add the new filter
                            filtersFlowLayoutPanel.Controls.Add(filterControl);

                            // Set filterObject with its value
                            ((DetailsModify.Filters.IIRFilters.IIRFilterUserControl)filterControl).filterTypeComboBox.SelectedIndex = (int)filterObject[1];
                            ((DetailsModify.Filters.IIRFilters.IIRFilterUserControl)filterControl).orderTextBox.Text = ((int)filterObject[2]).ToString();
                            ((DetailsModify.Filters.IIRFilters.IIRFilterUserControl)filterControl).frequencyScrollBar.Value = (int)((double)filterObject[3] * _samplingRate);
                            break;
                    }
                }

                // Add AI prediction models in modelTypeComboBox
                List<string> modelsList = new List<string>();
                foreach (string model in ((MainForm)signalHolder.FindForm())._targetsModelsHashtable.Keys)
                    modelsList.Add(model);
                modelsList.Sort();
                modelTypeComboBox.DataSource = modelsList;
            }
        }

        private void loadClass(double[] samples, double samplingRate, double startingInSec)
        {
            if (samples == null)
                return;

            // Calculate fft of signal
            try
            {
                double[] fftMag = applyFFT(samples);

                // Load signals inside charts
                Garage.loadSignalInChart(signalChart, samples, samplingRate, _quantizationStep, startingInSec, "FormDetailsModifySignal");

                // Set the real frequency rate
                // ftMag has only half of the spectrum (the real frequencies)
                double hzRate = (fftMag.Length * 2) / samplingRate;
                // Load fft inside its chart
                Garage.loadSignalInChart(spectrumChart, fftMag, _quantizationStep, hzRate, 0, "FormDetailsModify");
            } catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
            }
        }

        private double[] applyFFT(double[] samples)
        {
            double[] fftMag = Garage.calculateFFT(samples);

            // Check if samples contains values higher than "337,593,543,950,335"
            double threshold = 337593543950335D;
            bool skip = false;
            foreach (double sample in samples)
                if (sample > threshold)
                {
                    skip = true;
                    break;
                }

            // Check if the spectrum is empty
            if (double.IsNaN(fftMag[0]) || skip)
            {
                // If yes then set the signal and the spectrum to zeros
                for (int i = 0; i < samples.Length; i++)
                    samples[i] = 0D;
                for (int i = 0; i < fftMag.Length; i++)
                    fftMag[i] = 0D;

                // show the error and return
                //System.Windows.Forms.MessageBox.Show("The signal is not properly filtered", "Error \"Spectrum not properly calculated\"", System.Windows.Forms.MessageBoxButtons.OK);
            }

            return fftMag;
        }

        private void applyDWT(double[] samples, int layer)
        {
            if (samples == null)
                return;

            // Create dwt transform according to the selcted specs
            _dwtSpecs[3] = layer;
            _dwtLevelsSamples = Garage.calculateDWT(samples, (String)_dwtSpecs[2], layer);

            // Insert layers according _dwtSamples
            dwtLevelsComboBox.DataSource = null;
            List<String> layers = new List<string>(_dwtLevelsSamples.Length);
            for (int i = 1; i <= _dwtLevelsSamples.Length; i++)
                layers.Add("Level " + i.ToString());
            dwtLevelsComboBox.DataSource = layers;
        }

        private void applyFiltersOnSignal(double[] signal)
        {
            // Check if sampling rate isn't empty
            if (!samplingRateTextBox.Text.Equals("Sampling rate"))
            {
                // If yes then set the new sampling rate
                _samplingRate = double.Parse(samplingRateTextBox.Text);
                _quantizationStep = double.Parse(quantizationStepTextBox.Text);

                // Set _filteredSamples as _samples
                _filteredSamples = new double[signal.Length];
                for (int i = 0; i < signal.Length; i++)
                    _filteredSamples[i] = signal[i];

                // Apply filters twice. The first onces are the filters before transforms, and the second are after transforms
                for (int i = 0; i < 2; i++)
                {
                    // Get _filtersHashtable and iterate through each filter
                    foreach (object[] filter in _filteresHashtable.Values)
                    {
                        // Get filter type and apply it according to its type
                        switch (filter[0])
                        {
                            case "DC removal":
                                // Check if the dc removal is activated and if the filter is before or after the transform
                                if ((int)filter[1] == 1 && (int)filter[2] == i)
                                    // If yes then remove the dc value
                                    _filteredSamples = Garage.removeDCValue(_filteredSamples);
                                break;
                            case "Normalize signal":
                                // Check if the dc removal is activated and if the filter is before or after the transform
                                if ((int)filter[1] == 1 && (int)filter[2] == i)
                                    // If yes then apply normalization
                                    _filteredSamples = Garage.normalizeSignal(_filteredSamples);
                                break;
                            case "Absolute signal":
                                // Check if the dc removal is activated and if the filter is before or after the transform
                                if ((int)filter[1] == 1 && (int)filter[2] == i)
                                    // If yes then make all values of the signal absolute
                                    _filteredSamples = Garage.absoluteSignal(_filteredSamples);
                                break;
                            case "Singal states viewer":
                                break;
                            default:
                                // This is for IIR filters
                                // Check if the filter is before or after the transform
                                if ((int)filter[4] == i)
                                    _filteredSamples = Garage.iirFilter(_filteredSamples, (String)filter[0], (int)filter[1], (int)filter[2], (double)filter[3], _samplingRate);
                                break;
                        }
                    }

                    // Apply transform only in first iteration
                    if (i == 0 && dwtLevelsComboBox.Items.Count > 0)
                    {
                        // Set _filteredSamples according to the selected layer
                        // Create dwt transform according to the selcted specs
                        _dwtLevelsSamples = Garage.calculateDWT(_filteredSamples, (String)_dwtSpecs[2], (int)_dwtSpecs[3]);
                        _filteredSamples = new double[((double[])_dwtLevelsSamples[dwtLevelsComboBox.SelectedIndex]).Length];
                        for (int j = 0; j < ((double[])_dwtLevelsSamples[dwtLevelsComboBox.SelectedIndex]).Length; j++)
                            _filteredSamples[j] = ((double[])_dwtLevelsSamples[dwtLevelsComboBox.SelectedIndex])[j];

                        // Set sampling rate according to the selected level
                        _samplingRate = _samplingRate / Math.Pow(2, dwtLevelsComboBox.SelectedIndex + 1);
                    }
                }

                int selectedIndex = signalsPickerComboBox.SelectedIndex;
                if (selectedIndex == 0 && _samples != null)
                    loadClass(_samples, _samplingRate, _startingInSec);
                else if (selectedIndex == 1 && _filteredSamples != null)
                    loadClass(_filteredSamples, _samplingRate, _startingInSec);
            }
        }

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
        private void saveButton_Click(object sender, EventArgs e)
        {
            // Open file dialogue to choose the path where to save the image
            using (SaveFileDialog sfd = new SaveFileDialog() { Title = "Save an Image File", ValidateNames = true, Filter = "PNG Image|*.png", RestoreDirectory = true })
            {
                // Check if the user clicked OK button
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // If yes then save teh chart in the selected path

                    // Get the path of specified file
                    String filePath = sfd.FileName;

                    saveChartAsImage(filePath);
                }
            }
        }

        private void saveChartAsImage(string filePath)
        {
            // Scale the size of the chart
            int scaling = 20;
            System.IO.MemoryStream myStream = new System.IO.MemoryStream();
            Chart chart2 = new Chart();
            signalChart.Serializer.Save(myStream);
            chart2.Serializer.Load(myStream);
            chart2.Height = chart2.Height * scaling;
            chart2.Width = chart2.Width * scaling;
            foreach (Series serie in chart2.Series)
            {
                serie.BorderWidth = serie.BorderWidth * scaling;
                serie.MarkerSize = serie.MarkerSize * 15;
                serie.Font = new System.Drawing.Font("Microsoft Sans Serif", serie.Font.Size * 15);
                serie.SmartLabelStyle.CalloutLineWidth = serie.SmartLabelStyle.CalloutLineWidth * 10;
            }
            chart2.ChartAreas[0].AxisX.MajorGrid.LineWidth = chart2.ChartAreas[0].AxisX.MajorGrid.LineWidth * scaling;
            chart2.ChartAreas[0].AxisY.MajorGrid.LineWidth = chart2.ChartAreas[0].AxisY.MajorGrid.LineWidth * scaling;
            chart2.ChartAreas[0].AxisX.MajorTickMark.LineWidth = chart2.ChartAreas[0].AxisX.MajorTickMark.LineWidth * scaling;
            chart2.ChartAreas[0].AxisY.MajorTickMark.LineWidth = chart2.ChartAreas[0].AxisY.MajorTickMark.LineWidth * scaling;
            chart2.ChartAreas[0].AxisX.LineWidth = chart2.ChartAreas[0].AxisX.LineWidth * scaling;
            chart2.ChartAreas[0].AxisY.LineWidth = chart2.ChartAreas[0].AxisY.LineWidth * scaling;
            chart2.ChartAreas[0].AxisX.LabelAutoFitMinFontSize = chart2.ChartAreas[0].AxisX.LabelAutoFitMinFontSize * scaling;
            chart2.ChartAreas[0].AxisY.LabelAutoFitMinFontSize = chart2.ChartAreas[0].AxisY.LabelAutoFitMinFontSize * scaling;
            chart2.ChartAreas[0].AxisX.LabelAutoFitMaxFontSize = chart2.ChartAreas[0].AxisX.LabelAutoFitMaxFontSize * scaling;
            chart2.ChartAreas[0].AxisY.LabelAutoFitMaxFontSize = chart2.ChartAreas[0].AxisY.LabelAutoFitMaxFontSize * scaling;
            chart2.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", chart2.ChartAreas[0].AxisX.TitleFont.Size * scaling);
            chart2.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", chart2.ChartAreas[0].AxisY.TitleFont.Size * scaling);
            chart2.Legends[0].Font = new System.Drawing.Font("Microsoft Sans Serif", chart2.Legends[0].Font.Size * scaling);
            if (chart2.Titles.Count > 0)
                chart2.Titles[0].Font = new System.Drawing.Font("Microsoft Sans Serif", chart2.Titles[0].Font.Size * scaling);

            // Save the image from the scaled chart
            chart2.SaveImage(filePath, ChartImageFormat.Png);
        }

        private void signalExhibitor_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;
            _previousMouseX = e.X;
            _previousMouseY = e.Y;
        }

        private void signalExhibitor_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown)
            {
                EventHandlers.signalExhibitor_MouseMove(sender, e, _previousMouseX, _previousMouseY);
                _previousMouseX = e.X;
                _previousMouseY = e.Y;
            }
            // Check if AI tool is activated and in R selection step
            if ((_featuresOrderedDictionary.Count == 2 || _featuresOrderedDictionary.Count == 4) && (sender as Chart).Name.Equals("signalChart"))
            {
                // If yes then it is activated
                // Check if mouse cursor is near to a any state by 20% to the nearest state
                // Calculate x interval and y interval
                double xInterval = ((sender as Chart).ChartAreas[0].AxisX.Maximum - (sender as Chart).ChartAreas[0].AxisX.Minimum) * _samplingRate;
                double yInterval = (sender as Chart).ChartAreas[0].AxisY.Maximum - (sender as Chart).ChartAreas[0].AxisY.Minimum;

                // Get values of the position
                double xValue = ((sender as Chart).ChartAreas[0].AxisX.PixelPositionToValue(e.X) - _startingInSec) * _samplingRate;
                double yValue = (sender as Chart).ChartAreas[0].AxisY.PixelPositionToValue(e.Y);

                // Get all up points
                // Take the nearest point of up points in X
                int nearestState = int.MaxValue;
                for (int i = 1; i < _featuresOrderedDictionary.Count; i++)
                    foreach (int xState in _tempStatesList[i - 1])
                        if (Math.Abs(xValue - xState) < Math.Abs(xValue - nearestState))
                            nearestState = xState;

                // Check if cursor is near to the point by less than 20% in X and Y
                if (Math.Abs(xValue - nearestState) / xInterval < 0.2 && Math.Abs(yValue - _filteredSamples[nearestState]) / yInterval < 0.2)
                {
                    // If yes then make a selection point at this position
                    Garage.loadXYInChart(signalChart, new double[1] { (nearestState / _samplingRate) + _startingInSec }, new double[1] { _filteredSamples[nearestState] }, null, 0, 4, "FormDetailModify");
                    // Set selection state's index
                    _tempStatesList[3].Clear();
                    _tempStatesList[3].Add(nearestState);
                }
                else
                {
                    (sender as Chart).Series["Selection"].Points.Clear();
                    _tempStatesList[3].Clear();
                }
            }
        }

        private void signalExhibitor_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }

        private void signalChart_MouseClick(object sender, MouseEventArgs e)
        {
            // Check if AI tool is activated and in R selection step
            if ((_featuresOrderedDictionary.Count == 2 || _featuresOrderedDictionary.Count == 4) && (sender as Chart).Name.Equals("signalChart"))
            {
                // If yes then it is activated
                // Get the selcted point if it exists
                if (_tempStatesList[3].Count > 0)
                {
                    // Get the index of the point
                    for (int j = 1; j < 4; j++)
                        for (int i = 0; i < _tempStatesList[j - 1].Count; i++)
                            if (_tempStatesList[3][0] == _tempStatesList[j - 1][i])
                            {
                                // Check if it has a label then remove it, and if not add it
                                int selectedBeatIndx = 0;
                                if (featuresTableLayoutPanel.Controls.ContainsKey("P,T_selection_features"))
                                    selectedBeatIndx = ((ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls["P,T_selection_features"]).Items[0]).DropDownItems.Count;
                                if (!(sender as Chart).Series[j].Points[i].Label.Equals(""))
                                {
                                    if ((sender as Chart).Series[j].Points[i].Label.Equals("P"))
                                        ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][1] = int.MinValue;
                                    else if ((sender as Chart).Series[j].Points[i].Label.Equals("T"))
                                        ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][6] = int.MinValue;
                                    (sender as Chart).Series[j].Points[i].Label = "";
                                }
                                else if (_featuresOrderedDictionary.Count == 2)
                                    (sender as Chart).Series[j].Points[i].Label = "R";
                                else if (_featuresOrderedDictionary.Count == 4)
                                {
                                    int selectedIndx = ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][0] + _tempStatesList[j - 1][i];
                                    if (((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][1] == int.MinValue)
                                    {
                                        (sender as Chart).Series[j].Points[i].Label = "P";
                                        ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][1] = selectedIndx;
                                    } else if (((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][6] == int.MinValue)
                                    {
                                        (sender as Chart).Series[j].Points[i].Label = "T";
                                        ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][6] = selectedIndx;
                                    }
                                }
                            }
                }
            }
        }

        private void signalExhibitor_MouseWheel(object sender, MouseEventArgs e)
        {
            EventHandlers.signalExhibitor_MouseWheel(sender, e, _previousMouseX, _previousMouseY);
        }

        private void samplingRateTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.textBoxNumberOnly(sender, e);
        }

        private void samplingRateTextBox_Enter(object sender, EventArgs e)
        {
            EventHandlers.TextBox_Enter_Leave_Focus(sender as TextBox, "Sampling rate", true);
        }

        private void samplingRateTextBox_Leave(object sender, EventArgs e)
        {
            EventHandlers.TextBox_Enter_Leave_Focus(sender as TextBox, "Sampling rate", false);
        }

        public void applyButton_Click(object sender, EventArgs e)
        {
            if (_samples == null)
                return;

            applyFiltersOnSignal(_samples);
        }

        private void filtersComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            EventHandlers.filtersComboBox_SelectedIndexChanged(sender, this);
        }

        private void signalsPickerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check which signal is selected
            switch(signalsPickerComboBox.SelectedIndex)
            {
                case 0:
                    // Original signal is selected
                    loadClass(_samples, _samplingRate, _startingInSec);
                    break;
                case 1:
                    // Filtered signal is selected
                    loadClass(_filteredSamples, _samplingRate, _startingInSec);
                    break;
            }
        }

        private void fftButton_Click(object sender, EventArgs e)
        {
            // Set this button and the edit button and dwtLayersComboBox to disable
            editButton.Enabled = false;
            dwtLevelsComboBox.Enabled = false;
            fftButton.Enabled = false;

            // Set dwt button to enable
            dwtButton.Enabled = true;

            // Clear dwtLayersComboBox
            dwtLevelsComboBox.DataSource = null;

            // Refresh the apply button if autoApply is checked
            if (autoApplyCheckBox.Checked)
                applyButton_Click(null, null);
        }

        public void dwtButton_Click(object sender, EventArgs e)
        {
            // Set this button and the edit button and dwtLayersComboBox to enable
            editButton.Enabled = true;
            dwtLevelsComboBox.Enabled = true;
            fftButton.Enabled = true;

            // Set dwt button to disable
            dwtButton.Enabled = false;

            // Apply dwt on the original signal
            applyDWT(_samples, int.MaxValue);
        }

        private void dwtLayersComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Refresh the apply button if autoApply is checked
            if (autoApplyCheckBox.Checked)
                // Check if the selected index is not -1
                if (dwtLevelsComboBox.SelectedIndex > -1)
                    applyButton_Click(null, null);

            // get signal states viewer user control and modify it
            foreach(string key in _filteresHashtable.Keys)
                if (key.Contains("Singal states viewer"))
                {
                    SignalStatesViewerUserControl signalStatesViewerUserControl = (SignalStatesViewerUserControl)filtersFlowLayoutPanel.Controls[filtersFlowLayoutPanel.Controls.IndexOfKey(key)];

                    if (signalStatesViewerUserControl.hThresholdScrollBar.Maximum != (_filteredSamples.Length + signalStatesViewerUserControl.hThresholdScrollBar.LargeChange - 1))
                    {
                        signalStatesViewerUserControl.hThresholdScrollBar.Maximum = _filteredSamples.Length + signalStatesViewerUserControl.hThresholdScrollBar.LargeChange - 1;
                        signalStatesViewerUserControl.hThresholdScrollBar.Value = signalStatesViewerUserControl.hThresholdScrollBar.Value * (_filteredSamples.Length + signalStatesViewerUserControl.hThresholdScrollBar.LargeChange - 1) / signalStatesViewerUserControl.hThresholdScrollBar.Maximum;
                    }
                }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            // Check if the form is already opened, and close it if so
            if (Application.OpenForms.OfType<FormDWT>().Count() == 1)
                Application.OpenForms.OfType<FormDWT>().First().Close();

            // Create the new form
            FormDWT formDWT = new FormDWT(this);
            formDWT.Show();
        }

        private void signalFusionButton_Click(object sender, EventArgs e)
        {
            // Check if the form is already opened, and close it if so
            if (Application.OpenForms.OfType<FormSignalFusion>().Count() == 1)
                Application.OpenForms.OfType<FormSignalFusion>().First().Close();

            if (signalChart.Series[0].Points.Count < 1)
                return;

            // Get samples from signal chart
            double[] samples = new double[signalChart.Series[0].Points.Count];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = signalChart.Series[0].Points[i].YValues[0];

            // Get magor frequency from _samples after removing dc value
            double[] fftMag = applyFFT(Garage.removeDCValue(_samples));
            double magorFreuency = 0;
            double magorFreuencyValue = fftMag[0];
            for (int i = 1; i < spectrumChart.Series[0].Points.Count; i++)
                if (fftMag[i] > magorFreuencyValue)
                {
                    magorFreuencyValue = fftMag[i];
                    magorFreuency = i / ((fftMag.Length * 2) / _samplingRate);
                }

            // Create the new form
            FormSignalFusion formSignalFusion = new FormSignalFusion(samples, magorFreuency, _samplingRate, _quantizationStep, pathLabel.Text);
            formSignalFusion.Show();
        }

        private void sendSignalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Chart senderChart = (Chart)((sender as ToolStripItem).Owner as ContextMenuStrip).SourceControl;
            if (senderChart.Series[0].Points.Count < 1)
                return;

            // Get samples from signal chart
            double[] samples = new double[senderChart.Series[0].Points.Count];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = senderChart.Series[0].Points[i].YValues[0];

            // Check if the sender is spectrumChart
            if (senderChart.Name.Equals("spectrumChart"))
                // If yes then set the sampling rate as hertz sampling rate
                EventHandlers.sendSignalTool(samples, (samples.Length * 2) / _samplingRate, _quantizationStep, pathLabel.Text + "\\Collector");
            else
                EventHandlers.sendSignalTool(samples, _samplingRate, _quantizationStep, pathLabel.Text + "\\Collector");
        }

        private void analyseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Chart senderChart = (Chart)((sender as ToolStripItem).Owner as ContextMenuStrip).SourceControl;
            if (senderChart.Series[0].Points.Count < 1)
                return;

            // Get samples from signal chart
            double[] samples = new double[senderChart.Series[0].Points.Count];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = senderChart.Series[0].Points[i].YValues[0];

            // Check if the sender is spectrumChart
            if (senderChart.Name.Equals("spectrumChart"))
                // If yes then set the sampling rate as hertz sampling rate
                EventHandlers.analyseSignalTool(samples, (samples.Length * 2) / _samplingRate, _quantizationStep, pathLabel.Text + "\\Analyser");
            else
                EventHandlers.analyseSignalTool(samples, _samplingRate, _quantizationStep, pathLabel.Text + "\\Analyser");
        }

        private void predictButton_Click(object sender, EventArgs e)
        {
            // Get the selected model from modelTypeComboBox
            if (modelTypeComboBox.SelectedIndex > -1)
            {
                // Disable this button and modelTypeComboBox
                predictButton.Enabled = false;
                modelTypeComboBox.Enabled = false;

                // Set prediction to true
                _predictionOn = true;

                // Initialize tools
                setFeaturesLabelsButton_Click(null, null);

                // Start prediction
                nextButton_Click(null, null);
            }
        }

        //*******************************************************************************************************//
        //*******************************************************************************************************//
        //************************************************AI TOOLS***********************************************//
        public TFBackThread _tFBackThread;
        public readonly AutoResetEvent _signal = new AutoResetEvent(false);
        public readonly ConcurrentQueue<object[]> _queue = new ConcurrentQueue<object[]>();
        private double[] askForPrediction(double[] input, int step)
        {
            // Send information to TFBackThread
            string modelType = null;
            this.Invoke(new MethodInvoker(delegate () { modelType = modelTypeComboBox.Text; }));
            // Check which model is selected
            if (modelType.Contains("Neural network"))
            {
                // This is for neural network
                _tFBackThread._queue.Enqueue(new object[] { "predict", "FormDetailsModify", input, modelType, step, _signal, _queue });
                _tFBackThread._signal.Set();

                // Wait for the answer
                _signal.WaitOne();

                object[] item = null;
                while (_queue.TryDequeue(out item))
                {
                    // Check which function is selected
                    return (double[])item[0];
                }
            }
            else if (modelType.Contains("K-Nearest neighbor"))
            {
                // This is for knn
                return KNNBackThread.predict(input, (KNNBackThread.KNNModel)((List<object[]>)_targetsModelsHashtable[modelType])[step][0], (List<double[]>)((List<object[]>)_targetsModelsHashtable[modelType])[step][1]);
            }
            else if (modelType.Contains("Naive bayes"))
            {
                // This is for naive bayes
                return NaiveBayesBackThread.predict(input, (NaiveBayesBackThread.NaiveBayesModel)((List<object[]>)_targetsModelsHashtable[modelType])[step][0], (List<double[]>)((List<object[]>)_targetsModelsHashtable[modelType])[step][1]);
            }
            
            return null;
        }

        private void setFeaturesLabelsButton_Click(object sender, EventArgs e)
        {
            // Disable everything except ai tools
            enableAITools(true);

            // Set dc removal filter. Check it then set it disabled
            filtersComboBox.SelectedIndex = 3;
            ((DCRemovalUserControl)filtersFlowLayoutPanel.Controls[0]).dcValueRemoveCheckBox.Checked = true;
            ((DCRemovalUserControl)filtersFlowLayoutPanel.Controls[0]).Enabled = false;
            // Normalize signal and make its values absolute after DWT transform
            filtersComboBox.SelectedIndex = 4;
            ((NormalizedSignalUserControl)filtersFlowLayoutPanel.Controls[1]).normalizeSignalCheckBox.Checked = true;
            ((NormalizedSignalUserControl)filtersFlowLayoutPanel.Controls[1]).applyAfterTransformCheckBox.Checked = true;
            ((NormalizedSignalUserControl)filtersFlowLayoutPanel.Controls[1]).Enabled = false;
            filtersComboBox.SelectedIndex = 5;
            ((AbsoluteSignalUserControl)filtersFlowLayoutPanel.Controls[2]).absoluteSignalCheckBox.Checked = true;
            ((AbsoluteSignalUserControl)filtersFlowLayoutPanel.Controls[2]).applyAfterTransformCheckBox.Checked = true;
            ((AbsoluteSignalUserControl)filtersFlowLayoutPanel.Controls[2]).Enabled = false;
            // Set states visualiser
            filtersComboBox.SelectedIndex = 6;
            ((SignalStatesViewerUserControl)filtersFlowLayoutPanel.Controls[3]).showStatesCheckBox.Enabled = false;
            ((SignalStatesViewerUserControl)filtersFlowLayoutPanel.Controls[3]).showDeviationCheckBox.Enabled = false;
            ((SignalStatesViewerUserControl)filtersFlowLayoutPanel.Controls[3]).tdtThresholdScrollBar.Enabled = false;

            // Get first 3 levels of DWT of the signal
            applyDWT(_samples, 3);

            // Show instructions of the first goal
            featuresSettingInstructionsLabel.Text = "Set the best \"Threshold ratio\" and \"Hor Threshold\" so that only high peaks are visible.\n(Try setting them as high as possible for the third level).\nPress next after you finish.";

            // Enable discard and next button, and disable this button
            discardButton.Enabled = true;
            nextButton.Enabled = true;
            setFeaturesLabelsButton.Enabled = false;
        }

        private void discardButton_Click(object sender, EventArgs e)
        {
            // Show message for confirming the action
            DialogResult dialogResult = MessageBox.Show("Are you sure about discarding features selection?", "Action confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                // Disable save button in signal holder
                _signalHolder.saveButton.Enabled = false;
                // Enable everything except ai tools
                enableAITools(false);
                // Remove instructions
                featuresSettingInstructionsLabel.Text = "";
                // Enable setFeaturesLabelsButton, and disable other tools
                setFeaturesLabelsButton.Enabled = true;
                previousButton.Enabled = false;
                nextButton.Enabled = false;
                discardButton.Enabled = false;
                nextButton.Text = "Next";

                // Remove selected features
                _featuresOrderedDictionary.Clear();
                featuresTableLayoutPanel.Controls.Clear();

                signalChart.Series["Up peaks"].LabelForeColor = Color.Transparent;
                signalChart.Series["Up peaks"].Points.Clear();
                signalChart.Series["Down peaks"].Points.Clear();
                signalChart.Series["Stable"].Points.Clear();
                signalChart.Series["Selection"].Points.Clear();
                signalChart.Series["Labels"].Points.Clear();
                foreach (List<int> statesList in _tempStatesList)
                    statesList.Clear();
            }
        }

        private void enableAITools(bool enable)
        {
            // Remove all filters
            filtersFlowLayoutPanel.Controls.Clear();
            _filteresHashtable.Clear();

            signalFusionButton.Enabled = !enable;
            signalsPickerComboBox.Enabled = !enable;
            fftButton.Enabled = !enable;
            dwtButton.Enabled = !enable;
            editButton.Enabled = !enable;
            dwtLevelsComboBox.Enabled = enable;
            filtersComboBox.Enabled = !enable;

            if (enable)
                signalsPickerComboBox.SelectedIndex = 1;
            else
                fftButton_Click(null, null);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate () {
                // Check if next button is finish
                if (nextButton.Text.Equals("Finish"))
                {
                    // If yes then lunch finish function and return
                    finish();

                    // Check if prediction is on
                    if (_predictionOn)
                    {
                        // If yes then return prediction stuff to normal state
                        _predictionOn = false;
                        predictButton.Enabled = true;
                        modelTypeComboBox.Enabled = true;
                    }
                    
                    return;
                }

                ToolStripMenuItem featuresItems = null;
                ToolStripMenuItem flowLayoutItems01 = null;
                ToolStripMenuItem flowLayoutItems02 = null;
                ToolStripMenuItem flowLayoutItems03 = null;
                double[] inputs;
                double[] outputs;
                SignalStatesViewerUserControl signalStatesViewerUserControl = null;
                foreach (string key in _filteresHashtable.Keys)
                    if (key.Contains("Singal states viewer"))
                        signalStatesViewerUserControl = (SignalStatesViewerUserControl)filtersFlowLayoutPanel.Controls[filtersFlowLayoutPanel.Controls.IndexOfKey(key)];

                if (_featuresOrderedDictionary.Count == 0)
                    _featuresOrderedDictionary.Add("beats", null);

                // Get curretn step threshold
                float threshold = 1f;

                // Check which step of features selectoin is this
                switch (_featuresOrderedDictionary.Count)
                {
                    case 1:
                        // This is for QRS detection
                        featuresItems = new ToolStripMenuItem("QRS detection features");

                        inputs = new double[15];
                        outputs = null;
                        // Iterate through 3 levels of dwt
                        flowLayoutItems01 = new ToolStripMenuItem("Inputs");
                        double[] normalizedSignal;
                        OrderedDictionary statParams;
                        for (int i = 0; i < 3; i++)
                        {
                            // Normalize the absolute values of levelSignal
                            normalizedSignal = Garage.normalizeSignal(Garage.absoluteSignal((double[])_dwtLevelsSamples[i]));

                            // Create the pdf of the signal
                            statParams = Garage.statParams(normalizedSignal);
                            for (int j = 0; j < statParams.Count; j++)
                            {
                                inputs[i * 5 + j] = (double)statParams[j];
                                flowLayoutItems01.DropDownItems.Add(statParams.Cast<DictionaryEntry>().ElementAt(j).Key.ToString() + ": " + statParams[j].ToString());
                            }
                        }
                        featuresItems.DropDownItems.Add(flowLayoutItems01);
                        // get signal states viewer user control and set the features output
                        flowLayoutItems01 = new ToolStripMenuItem("Outputs");

                        // Check if outputs should be predicted
                        if (_predictionOn)
                        {
                            outputs = askForPrediction(inputs, _featuresOrderedDictionary.Count - 1);
                            signalStatesViewerUserControl.amplitudeThresholdScrollBar.Value = (int)(1000d - (outputs[0] * 1000d));
                            signalStatesViewerUserControl.hThresholdScrollBar.Maximum = _filteredSamples.Length + signalStatesViewerUserControl.hThresholdScrollBar.LargeChange - 1;
                            signalStatesViewerUserControl.hThresholdScrollBar.Value = (int)(outputs[1] * signalStatesViewerUserControl.hThresholdScrollBar.Maximum);
                        }
                        else
                            outputs = new double[2] { signalStatesViewerUserControl._thresholdRatio,
                                                    (double)signalStatesViewerUserControl.hThresholdScrollBar.Value / (double)signalStatesViewerUserControl.hThresholdScrollBar.Maximum};
                        flowLayoutItems01.DropDownItems.Add("Threa ratio: " + outputs[0]);
                        flowLayoutItems01.DropDownItems.Add("Hor threa: " + outputs[1]);

                        featuresItems.DropDownItems.Add(flowLayoutItems01);
                        // Insert the new feature
                        _featuresOrderedDictionary.Add("qrs_detection", new object[2] { inputs, outputs });

                        // Make the form ready for next goal
                        // Remove absolute signal filter
                        filtersFlowLayoutPanel.Controls.RemoveByKey("Absolute signal0");
                        foreach (string key in _filteresHashtable.Keys)
                            if (key.Contains("Absolute signal"))
                            {
                                _filteresHashtable.Remove(key);
                                break;
                            }
                        // Clear dwtLayersComboBox and disable it
                        dwtLevelsComboBox.DataSource = null;
                        dwtLevelsComboBox.Enabled = false;
                        // Refresh the apply button if autoApply is checked
                        applyButton_Click(null, null);
                        _tempStatesList = signalStatesViewerUserControl.showSignalStates();
                        signalStatesViewerUserControl.Enabled = false;
                        // Set the up peaks labels ready
                        signalChart.Series["Up peaks"].LabelForeColor = Color.Black;
                        for (int i = 0; i < signalChart.Series["Up peaks"].Points.Count; i++)
                            signalChart.Series["Up peaks"].Points[i].Label = "";

                        // Give the instruction for next goal, and enable previous button
                        featuresSettingInstructionsLabel.Text = "Select R peaks from the visible states in the chart.\nClick once on the peak to select, or click once again to unselect.\nPress next after you finish.";
                        previousButton.Enabled = true;
                        break;
                    case 2:
                        // This is for QRS selection
                        featuresItems = new ToolStripMenuItem("QRS selection features");

                        // Create a hashtable where to store R indexes and their amplitude
                        OrderedDictionary qrsOrderedDictionary = new OrderedDictionary();

                        // Get the states of the normalized signal
                        List<State> normalizedSignalStates = Garage.scanPeaks(_filteredSamples, Garage.amplitudeInterval(_filteredSamples), ((double[])((object[])_featuresOrderedDictionary[1])[1])[0], _quantizationStep, 45d, false);

                        // Get the states of each of 3 levels of normalized absolute dwt
                        List<State> absoluteDWTStates;
                        int lastSavedQRSIndx;
                        int interval;
                        for (int i = 0; i < 3; i++)
                        {
                            lastSavedQRSIndx = 0;
                            // Normalize the absolute values of levelSignal
                            normalizedSignal = Garage.normalizeSignal(Garage.absoluteSignal((double[])_dwtLevelsSamples[i]));
                            // Get its states
                            absoluteDWTStates = Garage.scanPeaks(normalizedSignal, Garage.amplitudeInterval(normalizedSignal), ((double[])((object[])_featuresOrderedDictionary[1])[1])[0], _quantizationStep, 45d, false);

                            // Iterate through each up state from absoluteDWTStates and compare it with states in normalizedSignalStates
                            // and save the nearest state of normalizedSignalStates to the up state of absoluteDWTStates in qrsList as R state
                            foreach (State dwtState in absoluteDWTStates)
                            {
                                if (!dwtState.Name.Equals("up"))
                                    continue;
                                // Start from the last saved qrs index in qrsList
                                interval = int.MaxValue;
                                for (int j = lastSavedQRSIndx; j < normalizedSignalStates.Count; j++)
                                {
                                    if (!normalizedSignalStates[j].Name.Equals("up"))
                                        continue;

                                    // Check if the state is getting closer
                                    if (interval >= Math.Abs(((int)dwtState._index * (int)Math.Pow(2, i + 1)) - (int)normalizedSignalStates[j]._index))
                                    {
                                        // Update the new interval and save the index of current state
                                        interval = Math.Abs(((int)dwtState._index * (int)Math.Pow(2, i + 1)) - (int)normalizedSignalStates[j]._index);
                                        lastSavedQRSIndx = j;
                                    } else
                                    {
                                        // If yes then the last state is the closest.
                                        // Then save the 6 states near it in qrsList if they didn't already exist
                                        for (int k = (lastSavedQRSIndx - 2 >= 0 ? lastSavedQRSIndx - 2 : 0); k < (lastSavedQRSIndx + 2 < normalizedSignalStates.Count ? lastSavedQRSIndx + 2 : normalizedSignalStates.Count); k++)
                                        if (!qrsOrderedDictionary.Contains((int)normalizedSignalStates[k]._index) && normalizedSignalStates[k].Name.Equals("up"))
                                            qrsOrderedDictionary.Add((int)normalizedSignalStates[k]._index, _filteredSamples[(int)normalizedSignalStates[k]._index]);
                                        break;
                                    }
                                }
                                // Save last state in qrsList if it didn't already exist
                                if (!qrsOrderedDictionary.Contains((int)normalizedSignalStates[lastSavedQRSIndx]._index))
                                    qrsOrderedDictionary.Add((int)normalizedSignalStates[lastSavedQRSIndx]._index, _filteredSamples[(int)normalizedSignalStates[lastSavedQRSIndx]._index]);
                            }
                        }

                        // Calculate RR average from qrsHashtable
                        double averageRR = 0;
                        for (int i = 1; i < qrsOrderedDictionary.Count; i++)
                            averageRR += ((int)qrsOrderedDictionary.Cast<DictionaryEntry>().ElementAt(i).Key - (int)qrsOrderedDictionary.Cast<DictionaryEntry>().ElementAt(i - 1).Key) / (qrsOrderedDictionary.Count - 1);

                        // Set the inputs and outputs for each qrs in qrsOrderedDictionary
                        object[] featuresArray = new object[qrsOrderedDictionary.Count];
                        List<int[]> beatsSpecs = new List<int[]>();
                        for (int i = 0; i < qrsOrderedDictionary.Count; i++)
                        {
                            // Set to remove this state
                            outputs = new double[1] { 1d };
                            // inputs = { (R_cur_indx - R_prev_indx) / averageRR, R_cur_amp / R_prev_amp }
                            // outputs = { remove_R_cur }
                            if (i == 0)
                                inputs = new double[2] { (double)Math.Abs((int)qrsOrderedDictionary.Cast<DictionaryEntry>().ElementAt(i).Key - (int)qrsOrderedDictionary.Cast<DictionaryEntry>().ElementAt(i + 1).Key) / averageRR,
                                                        (double)qrsOrderedDictionary[i] / (double)qrsOrderedDictionary[i + 1] };
                            else
                                inputs = new double[2] { (double)((int)qrsOrderedDictionary.Cast<DictionaryEntry>().ElementAt(i).Key - (int)qrsOrderedDictionary.Cast<DictionaryEntry>().ElementAt(i - 1).Key) / averageRR,
                                                        (double)qrsOrderedDictionary[i] / (double)qrsOrderedDictionary[i - 1] };

                            // Check if outputs should be predicted
                            if (_predictionOn)
                            {
                                outputs = askForPrediction(inputs, _featuresOrderedDictionary.Count - 1);

                                threshold = ((float[])((List<object[]>)_targetsModelsHashtable[modelTypeComboBox.Text])[1 /*second step threshold*/][2])[0];
                                // Check if this R state is selected not to be removed
                                if (outputs[0] < threshold)
                                {
                                    // If yes then add current state as R
                                    if (beatsSpecs.Count > 0)
                                    {
                                        int rightIndx = beatsSpecs.Count - 1;
                                        while ((int)qrsOrderedDictionary.Cast<DictionaryEntry>().ElementAt(i).Key < beatsSpecs[rightIndx][4])
                                        {
                                            rightIndx -= 1;
                                            if (rightIndx == -1)
                                                break;
                                        }
                                        beatsSpecs.Insert(rightIndx + 1, new int[9] { int.MinValue, int.MinValue, int.MinValue, int.MinValue, (int)qrsOrderedDictionary.Cast<DictionaryEntry>().ElementAt(i).Key, int.MinValue, int.MinValue, int.MinValue, int.MinValue }); // { beat_start_index, P_indext, q_index, delta_index, R_index, s_index, T_index, beat_end_index, WPW_index }
                                    }
                                    else
                                        beatsSpecs.Add(new int[9] { int.MinValue, int.MinValue, int.MinValue, int.MinValue, (int)qrsOrderedDictionary.Cast<DictionaryEntry>().ElementAt(i).Key, int.MinValue, int.MinValue, int.MinValue, int.MinValue }); // { beat_start_index, P_indext, q_index, delta_index, R_index, s_index, T_index, beat_end_index, WPW_index }
                                }
                            }
                            else
                            {
                                // Check if this R is selected
                                // Start from the last saved qrs index in qrsList
                                for (int j = 0; j < signalChart.Series["Up peaks"].Points.Count; j++)
                                {
                                    // Check if the state is getting closer
                                    if (Math.Abs(_tempStatesList[0][j] - (int)qrsOrderedDictionary.Cast<DictionaryEntry>().ElementAt(i).Key) == 0)
                                    {
                                        // If yes then the last state is the closest. Then check if this point's label is selected as R or not
                                        if (signalChart.Series["Up peaks"].Points[j].Label.Equals("R"))
                                        {
                                            // If yes then set the output to 0 (do not remove state)
                                            outputs = new double[1] { 0d };
                                            // Set the selected R index and its beat's start and end indexex
                                            if (beatsSpecs.Count > 0)
                                            {
                                                int rightIndx = beatsSpecs.Count - 1;
                                                while ((int)qrsOrderedDictionary.Cast<DictionaryEntry>().ElementAt(i).Key < beatsSpecs[rightIndx][4])
                                                {
                                                    rightIndx -= 1;
                                                    if (rightIndx == -1)
                                                        break;
                                                }
                                                beatsSpecs.Insert(rightIndx + 1, new int[9] { int.MinValue, int.MinValue, int.MinValue, int.MinValue, (int)qrsOrderedDictionary.Cast<DictionaryEntry>().ElementAt(i).Key, int.MinValue, int.MinValue, int.MinValue, int.MinValue }); // { beat_start_index, P_indext, q_index, delta_index, R_index, s_index, T_index, beat_end_index, WPW_index }
                                            }
                                            else
                                                beatsSpecs.Add(new int[9] { int.MinValue, int.MinValue, int.MinValue, int.MinValue, (int)qrsOrderedDictionary.Cast<DictionaryEntry>().ElementAt(i).Key, int.MinValue, int.MinValue, int.MinValue, int.MinValue }); // { beat_start_index, P_indext, q_index, delta_index, R_index, s_index, T_index, beat_end_index, WPW_index }
                                        }
                                        break;
                                    }
                                }
                            }

                            flowLayoutItems01 = new ToolStripMenuItem("R" + i);
                            flowLayoutItems02 = new ToolStripMenuItem("Inputs");
                            flowLayoutItems02.DropDownItems.Add("RpRcur/RRav: " + inputs[0]);
                            flowLayoutItems02.DropDownItems.Add("ampRcur/ampRp: " + inputs[1]);
                            flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);
                            flowLayoutItems02 = new ToolStripMenuItem("Outputs");
                            flowLayoutItems02.DropDownItems.Add("Remove R: " + outputs[0]);
                            flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                            featuresItems.DropDownItems.Add(flowLayoutItems01);
                            featuresArray[i] = new object[2] { inputs, outputs };
                        }
                        // Check if there is no beats selected
                        if (beatsSpecs.Count == 0)
                        {
                            // If yes then show a message of beats must be selected
                            MessageBox.Show("There must be at least one beat selected", "Warning \"Unexpected data\"", MessageBoxButtons.OK);
                            return;
                        }
                    
                        // Set selected R's beats starting and ending indexes
                        int starting = 0;
                        int ending = 0;
                        for (int i = 0; i < beatsSpecs.Count; i++)
                        {
                            if (i != 0)
                                starting = (beatsSpecs[i][4] + beatsSpecs[i - 1][4]) / 2;
                            if (i != beatsSpecs.Count - 1)
                                ending = (beatsSpecs[i][4] + beatsSpecs[i + 1][4]) / 2;
                            else
                                ending = _samples.Length - 1;

                            beatsSpecs[i][0] = starting;
                            beatsSpecs[i][7] = ending;
                        }
                        _featuresOrderedDictionary[0] = beatsSpecs;

                        // Insert the new feature
                        _featuresOrderedDictionary.Add("qrs_selection", featuresArray);

                        // Make the form ready for next goal
                        signalChart.Series["Selection"].Points.Clear();
                        // Show the first selected R
                        _filteredSamples = new double[(beatsSpecs[0][7] - beatsSpecs[0][0]) + 1];
                        for (int i = beatsSpecs[0][0]; i < beatsSpecs[0][7]+ 1; i++)
                            _filteredSamples[i - beatsSpecs[0][0]] = _samples[i];
                        signalStatesViewerUserControl.Enabled = true;
                        signalStatesViewerUserControl.amplitudeThresholdScrollBar.Value = 980;
                        signalStatesViewerUserControl.hThresholdScrollBar.Value = 1;
                        signalStatesViewerUserControl.hThresholdScrollBar.Maximum = _filteredSamples.Length + signalStatesViewerUserControl.hThresholdScrollBar.LargeChange - 1;
                        // Refresh the apply button if autoApply is checked
                        signalChart.Series["Up peaks"].LabelForeColor = Color.Transparent;
                        applyFiltersOnSignal(_filteredSamples);
                        _tempStatesList = signalStatesViewerUserControl.showSignalStates();

                        // Give the instruction for next goal, and enable previous button
                        featuresSettingInstructionsLabel.Text = "Set the best \"Threshold ratio\" and \"Hor Threshold\" for the segmentation of P and T waves.\n" + 1 + "/" + beatsSpecs.Count + "\nPress next after you finish.";
                        break;
                    case 3:
                        // This is for beats peaks detection (P, T)
                        // Get previous saved features if exists
                        if (featuresTableLayoutPanel.Controls.Count == 3)
                            featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[2]).Items[0];
                        else
                            featuresItems = new ToolStripMenuItem("P,T detection features");

                        // Create the pdf of the signal and insert its coefs as inputs
                        flowLayoutItems02 = new ToolStripMenuItem("Inputs");
                        statParams = Garage.statParams(_filteredSamples);
                        inputs = new double[5];
                        for (int j = 0; j < statParams.Count; j++)
                        {
                            inputs[j] = (double)statParams[j];
                            flowLayoutItems02.DropDownItems.Add(statParams.Cast<DictionaryEntry>().ElementAt(j).Key.ToString() + ": " + statParams[j].ToString());
                        }
                        flowLayoutItems01 = new ToolStripMenuItem("R" + featuresItems.DropDownItems.Count);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        // get signal states viewer user control and set the features output
                        flowLayoutItems02 = new ToolStripMenuItem("Outputs");

                        // Check if outputs should be predicted
                        if (_predictionOn)
                        {
                            outputs = askForPrediction(inputs, _featuresOrderedDictionary.Count - 1);
                            signalStatesViewerUserControl.hThresholdScrollBar.Value = (int)(outputs[1] * signalStatesViewerUserControl.hThresholdScrollBar.Maximum);
                        }
                        else
                            outputs = new double[2] { signalStatesViewerUserControl._thresholdRatio,
                                                (double)signalStatesViewerUserControl.hThresholdScrollBar.Value / (double)signalStatesViewerUserControl.hThresholdScrollBar.Maximum};
                        flowLayoutItems02.DropDownItems.Add("Threa ratio: " + outputs[0]);
                        flowLayoutItems02.DropDownItems.Add("Hor threa: " + outputs[1]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);
                        featuresItems.DropDownItems.Add(flowLayoutItems01);

                        // Set Q and S indexes of the beat
                        List<State> signalStates = Garage.scanPeaks(_filteredSamples, Garage.amplitudeInterval(_filteredSamples), outputs[0], _quantizationStep, 45d, false);
                        for (int i = 0; i < signalStates.Count; i++)
                            if ((int)signalStates[i]._index == (((List<int[]>)_featuresOrderedDictionary[0])[featuresItems.DropDownItems.Count - 1][4] - ((List<int[]>)_featuresOrderedDictionary[0])[featuresItems.DropDownItems.Count - 1][0]))
                            {
                                int qIndx = 0;
                                int sIndx = signalStates.Count - 1;
                                if (i > 0)
                                    qIndx = i - 1;
                                if (i < signalStates.Count - 1)
                                    sIndx = i + 1;
                                ((List<int[]>)_featuresOrderedDictionary[0])[featuresItems.DropDownItems.Count - 1][2] = (int)signalStates[qIndx]._index + ((List<int[]>)_featuresOrderedDictionary[0])[featuresItems.DropDownItems.Count - 1][0];
                                ((List<int[]>)_featuresOrderedDictionary[0])[featuresItems.DropDownItems.Count - 1][5] = (int)signalStates[sIndx]._index + ((List<int[]>)_featuresOrderedDictionary[0])[featuresItems.DropDownItems.Count - 1][0];
                            }

                        // Check if there exist next beat
                        if (featuresItems.DropDownItems.Count < ((List<int[]>)_featuresOrderedDictionary[0]).Count)
                        {
                            // If yes then set the next beat for segmentation
                            _filteredSamples = new double[(((List<int[]>)_featuresOrderedDictionary[0])[featuresItems.DropDownItems.Count][7] - ((List<int[]>)_featuresOrderedDictionary[0])[featuresItems.DropDownItems.Count][0]) + 1];
                            for (int i = ((List<int[]>)_featuresOrderedDictionary[0])[featuresItems.DropDownItems.Count][0]; i < ((List<int[]>)_featuresOrderedDictionary[0])[featuresItems.DropDownItems.Count][7] + 1; i++)
                                _filteredSamples[i - ((List<int[]>)_featuresOrderedDictionary[0])[featuresItems.DropDownItems.Count][0]] = _samples[i];
                            if (signalStatesViewerUserControl.hThresholdScrollBar.Maximum != (_filteredSamples.Length + signalStatesViewerUserControl.hThresholdScrollBar.LargeChange - 1))
                            {
                                signalStatesViewerUserControl.hThresholdScrollBar.Maximum = _filteredSamples.Length + signalStatesViewerUserControl.hThresholdScrollBar.LargeChange - 1;
                                signalStatesViewerUserControl.hThresholdScrollBar.Value = signalStatesViewerUserControl.hThresholdScrollBar.Value * (_filteredSamples.Length + signalStatesViewerUserControl.hThresholdScrollBar.LargeChange - 1) / signalStatesViewerUserControl.hThresholdScrollBar.Maximum;
                            }
                            // Refresh the apply button if autoApply is checked
                            applyFiltersOnSignal(_filteredSamples);
                            _tempStatesList = signalStatesViewerUserControl.showSignalStates();

                            // Give the instruction for next goal, and enable previous button
                            featuresSettingInstructionsLabel.Text = "Set the best \"Threshold ratio\" and \"Hor Threshold\" for the segmentation of P and T waves.\n" + (featuresItems.DropDownItems.Count + 1) + "/" + ((List<int[]>)_featuresOrderedDictionary[0]).Count + "\nPress next after you finish.";
                        } else
                        {
                            // If yes then insert all features in _featuresOrderedDictionary
                            featuresArray = new object[featuresItems.DropDownItems.Count];
                            for (int i = 0; i < featuresItems.DropDownItems.Count; i++)
                            {
                                inputs = new double[5];
                                for (int j = 0; j < 5; j++)
                                    inputs[j] = double.Parse(((ToolStripMenuItem)((ToolStripMenuItem)featuresItems.DropDownItems[i]).DropDownItems[0]).DropDownItems[j].Text.Split(new char[] { ':' })[1].Substring(1));

                                outputs = new double[2];
                                for (int j = 0; j < 2; j++)
                                    outputs[j] = double.Parse(((ToolStripMenuItem)((ToolStripMenuItem)featuresItems.DropDownItems[i]).DropDownItems[1]).DropDownItems[j].Text.Split(new char[] { ':' })[1].Substring(1));

                                featuresArray[i] = new object[2] { inputs, outputs };
                            }
                            // Insert the new feature
                            _featuresOrderedDictionary.Add("pt_detection", featuresArray);

                            // Make the form ready for next goal
                            // Show the first selected R
                            signalStatesViewerUserControl.amplitudeThresholdScrollBar.Enabled = false;
                            signalStatesViewerUserControl.hThresholdScrollBar.Enabled = false;
                            signalStatesViewerUserControl.applyButton.Enabled = false;
                            // Refresh the apply button if autoApply is checked
                            setNextBeat(0, signalStatesViewerUserControl, _featuresOrderedDictionary.Count - 1, 0, featuresItems);

                            // Give the instruction for next goal, and enable previous button
                            featuresSettingInstructionsLabel.Text = "Select P and T peaks from the visible states in the chart.\nClick once on the peak to select, or click once again to unselect.\n" + 1 + "/" + ((List<int[]>)_featuresOrderedDictionary[0]).Count + "\nPress next after you finish.";
                        }
                        break;
                    case 4:
                        // This is for P and T selection
                        // Get previous saved features if exists
                        if (featuresTableLayoutPanel.Controls.Count == 4)
                            featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[3]).Items[0];
                        else
                            featuresItems = new ToolStripMenuItem("P,T selection features");

                        // Get the index of selected beat
                        int selectedBeatIndx = featuresItems.DropDownItems.Count;

                        // Get states of the selected beat
                        signalStates = Garage.scanPeaks(_filteredSamples, Garage.amplitudeInterval(_filteredSamples), signalStatesViewerUserControl._thresholdRatio, _quantizationStep, 45d, false);
                        // Iterate through each state and set its features
                        // starting from the second and ending by the one berfore last
                        flowLayoutItems01 = new ToolStripMenuItem("R" + selectedBeatIndx);
                        int previousStateIndx;
                        int nextStateIndx;
                        double pProba = 0;
                        double tProba = 0;
                        for (int i = 1; i < signalStates.Count - 1; i++)
                        {
                            inputs = new double[3];
                            outputs = new double[2];
                            flowLayoutItems02 = new ToolStripMenuItem("St" + i);
                            // Set the inputs of the state
                            flowLayoutItems03 = new ToolStripMenuItem("Inputs");
                            inputs[0] = _filteredSamples[(int)signalStates[i]._index];
                            flowLayoutItems03.DropDownItems.Add("St" + i + ": " + inputs[0]);
                            if (selectedBeatIndx == 0)
                            {
                                inputs[1] = (double)((int)signalStates[i]._index + ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][0] - ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][4]) / (double)(((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx + 1][4] - ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][4]);
                                flowLayoutItems03.DropDownItems.Add("St" + i + "-R" + selectedBeatIndx + "/R1-R" + selectedBeatIndx + ": " + (inputs[1]));
                            } else
                            {
                                inputs[1] = (double)((int)signalStates[i]._index + ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][0] - ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][4]) / (double)(((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][4] - ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx - 1][4]);
                                flowLayoutItems03.DropDownItems.Add("St" + i + "-R" + selectedBeatIndx + "/R" + selectedBeatIndx + "-R" + (selectedBeatIndx - 1) + ": " + (inputs[1]));
                            }
                            // Get next and previous state's index
                            previousStateIndx = i - 1;
                            nextStateIndx = i + 1;
                            for (int j = i + 1; j < signalStates.Count; j++)
                            {
                                if (!signalStates[i].Name.Equals("stable"))
                                    if (signalStates[j].Name.Equals(signalStates[i].Name))
                                        break;

                                nextStateIndx = j;

                                if (j + 1 < signalStates.Count && signalStates[i].Name.Equals("stable"))
                                    if (!signalStates[j + 1].Name.Equals(signalStates[i + 1].Name))
                                        break;
                            }
                            for (int j = i - 1; j > -1; j--)
                            {
                                if (!signalStates[i].Name.Equals("stable"))
                                    if (signalStates[j].Name.Equals(signalStates[i].Name))
                                        break;

                                previousStateIndx = j;

                                if (j - 1 > -1 && signalStates[i].Name.Equals("stable"))
                                    if (!signalStates[j - 1].Name.Equals(signalStates[i - 1].Name))
                                        break;
                            }
                            inputs[2] = ((_filteredSamples[(int)signalStates[i]._index] * 2) - _filteredSamples[(int)signalStates[previousStateIndx]._index] - _filteredSamples[(int)signalStates[nextStateIndx]._index]) / 2;
                            flowLayoutItems03.DropDownItems.Add("((ampSt" + i + " - ampSt" + (previousStateIndx) + ") + " + "(ampSt" + i + " - ampSt" + nextStateIndx + ")) / 2" + ": " + inputs[2]);
                            flowLayoutItems02.DropDownItems.Add(flowLayoutItems03);

                            // Set the ooutputs of the state
                            flowLayoutItems03 = new ToolStripMenuItem("Outputs");
                            int stateIndx = (int)signalStates[i]._index + ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][0];
                            // Check if outputs should be predicted
                            if (_predictionOn)
                            {
                                outputs = askForPrediction(inputs, _featuresOrderedDictionary.Count - 1);
                                // Check if state is predicted as P or T peak
                                if (outputs[0] > pProba)
                                {
                                    // If yes then this one is selected as P peak
                                    // set its index in ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][1]
                                    ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][1] = stateIndx;
                                    pProba = outputs[0];
                                }
                                if (outputs[1] > tProba)
                                {
                                    // If yes then this one is selected as T peak
                                    // set its index in ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][6]
                                    ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][6] = stateIndx;
                                    tProba = outputs[1];
                                }
                            }
                            else
                            {
                                if ((stateIndx - 1) <= ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][1] && ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][1] <= (stateIndx + 1))
                                    outputs[0] = 1;
                                if ((stateIndx - 1) <= ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][6] && ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][6] <= (stateIndx + 1))
                                    outputs[1] = 1;
                            }
                            flowLayoutItems03.DropDownItems.Add("P wave: " + outputs[0]);
                            flowLayoutItems03.DropDownItems.Add("T wave: " + outputs[1]);
                            flowLayoutItems02.DropDownItems.Add(flowLayoutItems03);
                            flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);
                        }
                        featuresItems.DropDownItems.Add(flowLayoutItems01);

                        // Check if there exist next beat
                        selectedBeatIndx = featuresItems.DropDownItems.Count;
                        if (selectedBeatIndx < ((List<int[]>)_featuresOrderedDictionary[0]).Count)
                        {
                            // If yes then set the next beat for segmentation
                            setNextBeat(selectedBeatIndx, signalStatesViewerUserControl, _featuresOrderedDictionary.Count - 1, 0, featuresItems);

                            // Give the instruction for next goal, and enable previous button
                            featuresSettingInstructionsLabel.Text = "Select P and T peaks from the visible states in the chart.\nClick once on the peak to select, or click once again to unselect.\n" + (selectedBeatIndx + 1) + "/" + ((List<int[]>)_featuresOrderedDictionary[0]).Count + "\nPress next after you finish.";
                        } else
                        {
                            // If yes then insert all features in _featuresOrderedDictionary
                            featuresArray = new object[featuresItems.DropDownItems.Count];
                            List<object[]> beatStates;
                            for (int i = 0; i < featuresItems.DropDownItems.Count; i++)
                            {
                                beatStates = new List<object[]>();
                                for (int j = 0; j < ((ToolStripMenuItem)featuresItems.DropDownItems[i]).DropDownItems.Count; j++)
                                {
                                    inputs = new double[3];
                                    outputs = new double[2];
                                    for (int k = 0; k < 3; k++)
                                    {
                                        inputs[k] = double.Parse(((ToolStripMenuItem)((ToolStripMenuItem)((ToolStripMenuItem)featuresItems.DropDownItems[i]).DropDownItems[j]).DropDownItems[0]).DropDownItems[k].Text.Split(new char[] { ':' })[1].Substring(1));
                                        if (k < 2)
                                            outputs[k] = double.Parse(((ToolStripMenuItem)((ToolStripMenuItem)((ToolStripMenuItem)featuresItems.DropDownItems[i]).DropDownItems[j]).DropDownItems[1]).DropDownItems[k].Text.Split(new char[] { ':' })[1].Substring(1));
                                    }
                                    beatStates.Add(new object[2] { inputs, outputs });
                                }
                                featuresArray[i] = beatStates;
                            }
                            // Insert the new feature
                            _featuresOrderedDictionary.Add("pt_selection", featuresArray);

                            // Make the form ready for next goal
                            // Add the check box of short PR declaration in filtersFlowLayoutPanel
                            filtersFlowLayoutPanel.Controls.Add(new CheckExistanceUserControl());
                            ((CheckExistanceUserControl)filtersFlowLayoutPanel.Controls[3]).existanceOfCheckBox.Text = "Existance of short PR";
                            // Show the first selected R
                            setNextBeat(0, signalStatesViewerUserControl, _featuresOrderedDictionary.Count - 2, 0, featuresItems);

                            // Give the instruction for next goal, and enable previous button
                            featuresSettingInstructionsLabel.Text = "Check the box in the right list of filters if short PR is detected.\n" + 1 + "/" + ((List<int[]>)_featuresOrderedDictionary[0]).Count + "\nPress next after you finish.";
                        }
                        break;
                    case 5:
                        // This is for short PR detection
                        // Get previous saved features if exists
                        if (featuresTableLayoutPanel.Controls.Count == 5)
                            featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[4]).Items[0];
                        else
                            featuresItems = new ToolStripMenuItem("Short PR detection features");

                        // Get the index of selected beat
                        selectedBeatIndx = featuresItems.DropDownItems.Count;

                        inputs = new double[1];
                        outputs = new double[1];
                        // Set the input and output of detecting short PR of the selected beat
                        flowLayoutItems01 = new ToolStripMenuItem("R" + selectedBeatIndx);
                        // Set the inputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem("Inputs");
                        // Check if P state exists
                        if (((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][1] != int.MinValue)
                        {
                            inputs[0] = (double)(((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][2] - ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][1]) / (double)(((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][4] - ((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][1]);
                            flowLayoutItems02.DropDownItems.Add("PQ/PR: " + inputs[0]);
                        }
                        else
                            flowLayoutItems02.DropDownItems.Add("PQ/PR: NaN");
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        // Set the outputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem("Outputs");
                        // Check if P state exists
                        if (((List<int[]>)_featuresOrderedDictionary[0])[selectedBeatIndx][1] != int.MinValue)
                        {
                            // Check if outputs should be predicted
                            if (_predictionOn)
                                outputs = askForPrediction(inputs, _featuresOrderedDictionary.Count - 1);
                            else
                                outputs[0] = Convert.ToInt32(((CheckExistanceUserControl)filtersFlowLayoutPanel.Controls[3]).existanceOfCheckBox.Checked);
                            flowLayoutItems02.DropDownItems.Add("Short PR: " + outputs[0]);
                        }
                        else
                            flowLayoutItems02.DropDownItems.Add("Short PR: NaN");
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);
                        featuresItems.DropDownItems.Add(flowLayoutItems01);

                        // Check if there exist next beat
                        selectedBeatIndx = featuresItems.DropDownItems.Count;
                        int shortPRNmbr = 0;
                        int shortPRBeatIndx = -1;
                        if (selectedBeatIndx < ((List<int[]>)_featuresOrderedDictionary[0]).Count)
                        {
                            // If yes then set the next beat for segmentation
                            // Uncheck short PR declaration in filtersFlowLayoutPanel
                            ((CheckExistanceUserControl)filtersFlowLayoutPanel.Controls[3]).existanceOfCheckBox.Checked = false;
                            // Refresh the apply button if autoApply is checked
                            setNextBeat(selectedBeatIndx, signalStatesViewerUserControl, _featuresOrderedDictionary.Count - 2, 0, featuresItems);

                            // Give the instruction for next goal, and enable previous button
                            featuresSettingInstructionsLabel.Text = "Check the box in the right list of filters if short PR is detected.\n" + (selectedBeatIndx + 1) + "/" + ((List<int[]>)_featuresOrderedDictionary[0]).Count + "\nPress next after you finish.";
                        } else
                        {
                            // If yes then insert all features in _featuresOrderedDictionary
                            featuresArray = new object[featuresItems.DropDownItems.Count];
                            for (int i = 0; i < featuresItems.DropDownItems.Count; i++)
                            {
                                // Check if values are existing
                                if (!(((ToolStripMenuItem)((ToolStripMenuItem)featuresItems.DropDownItems[i]).DropDownItems[0]).DropDownItems[0].Text.Split(new char[] { ':' })[1].Substring(1).Equals("NaN")))
                                {
                                    inputs = new double[1] { double.Parse(((ToolStripMenuItem)((ToolStripMenuItem)featuresItems.DropDownItems[i]).DropDownItems[0]).DropDownItems[0].Text.Split(new char[] { ':' })[1].Substring(1)) };
                                    outputs = new double[1] { double.Parse(((ToolStripMenuItem)((ToolStripMenuItem)featuresItems.DropDownItems[i]).DropDownItems[1]).DropDownItems[0].Text.Split(new char[] { ':' })[1].Substring(1)) };
                                } else
                                {
                                    inputs = null;
                                    outputs = null;
                                }
                                featuresArray[i] = new object[2] { inputs, outputs };
                            }
                            // Insert the new feature
                            _featuresOrderedDictionary.Add("short_pr_detection", featuresArray);

                            // Make the form ready for next goal
                            // Remove the check box of short PR declaration in filtersFlowLayoutPanel
                            filtersFlowLayoutPanel.Controls.RemoveAt(3);
                            // Get curretn step threshold
                            threshold = ((float[])((List<object[]>)_targetsModelsHashtable[modelTypeComboBox.Text])[4 /*fifth step threshold*/][2])[0];
                            // Get number of declared short PR
                            for (int i = 0; i < ((object[])_featuresOrderedDictionary[_featuresOrderedDictionary.Count - 1]).Length; i++)
                            {
                                object[] features = (object[])((object[])_featuresOrderedDictionary[_featuresOrderedDictionary.Count - 1])[i];
                                if (features[1] != null)
                                    if (((double[])features[1])[0] >= threshold)
                                    {
                                        if (shortPRBeatIndx == -1)
                                            shortPRBeatIndx = i;
                                        shortPRNmbr++;
                                    }
                            }
                            // Check if there exist short PR beats
                            if (shortPRNmbr > 0)
                            {
                                // If yes then show the first beat with short PR                            
                                signalStatesViewerUserControl.applyButton.Enabled = true;
                                // Enable acceleration and acceleration scrollbar
                                signalStatesViewerUserControl.tdtThresholdScrollBar.Enabled = true;
                                signalStatesViewerUserControl.showDeviationCheckBox.Checked = true;
                                // Disactivate normalization
                                foreach (string key in _filteresHashtable.Keys)
                                    if (key.Contains("Normalize signal"))
                                        ((NormalizedSignalUserControl)filtersFlowLayoutPanel.Controls[filtersFlowLayoutPanel.Controls.IndexOfKey(key)]).normalizeSignalCheckBox.Checked = false;
                                // Refresh the apply button if autoApply is checked
                                setNextBeat(shortPRBeatIndx, signalStatesViewerUserControl, _featuresOrderedDictionary.Count - 3, 0, featuresItems);

                                // Give the instruction for next goal, and enable previous button
                                featuresSettingInstructionsLabel.Text = "Set the best \"Acceleration threshold\" where the state before R must be as high as possible for delta of WPW syndrome detection.\n" + 1 + "/" + shortPRNmbr + "\nPress next after you finish.";
                            } else
                            {
                                // Show finish button
                                // Clear all filters from filtersFlowLayoutPanel
                                filtersFlowLayoutPanel.Controls.Clear();
                                _filteresHashtable.Clear();

                                // Set next button to finish
                                nextButton.Text = "Finish";

                                // Remove instruction
                                featuresSettingInstructionsLabel.Text = "";
                            }                        
                        }
                        break;
                    case 6:
                        // This is for delta detection
                        // Get previous saved features if exists
                        if (featuresTableLayoutPanel.Controls.Count == 6)
                            featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[5]).Items[0];
                        else
                            featuresItems = new ToolStripMenuItem("Delta detection features");

                        // Get curretn step threshold
                        threshold = ((float[])((List<object[]>)_targetsModelsHashtable[modelTypeComboBox.Text])[4 /*fifth step threshold*/][2])[0];
                        // Get current short PR beat and next short PR beat
                        shortPRNmbr = 0;
                        shortPRBeatIndx = -1;
                        int nextShortPRBeatIndx = -1;
                        for (int i = 0; i < ((object[])_featuresOrderedDictionary[_featuresOrderedDictionary.Count - 1]).Length; i++)
                        {
                            object[] features = (object[])((object[])_featuresOrderedDictionary[_featuresOrderedDictionary.Count - 1])[i];
                            if (features[1] != null)
                                if (((double[])features[1])[0] >= threshold)
                                {
                                    if (shortPRNmbr == featuresItems.DropDownItems.Count)
                                        shortPRBeatIndx = i;
                                    else if (shortPRBeatIndx != -1 && nextShortPRBeatIndx == -1)
                                        nextShortPRBeatIndx = i;
                                    shortPRNmbr++;
                                }
                        }

                        // Set the input and output of detecting short PR of the selected beat
                        flowLayoutItems01 = new ToolStripMenuItem("R" + shortPRBeatIndx);
                        inputs = new double[6];
                        outputs = new double[1];
                    
                        // Set the inputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem("Inputs");
                        inputs[0] = ((double[])((object[])((object[])_featuresOrderedDictionary[_featuresOrderedDictionary.Count - 1])[shortPRBeatIndx])[0])[0];
                        flowLayoutItems02.DropDownItems.Add("PQ/PR: " + inputs[0]);

                        statParams = Garage.statParams(Garage.normalizeSignal(_filteredSamples));
                        for (int j = 0; j < statParams.Count; j++)
                        {
                            inputs[j + 1] = (double)statParams[j];
                            flowLayoutItems02.DropDownItems.Add(statParams.Cast<DictionaryEntry>().ElementAt(j).Key.ToString() + ": " + inputs[j + 1]);
                        }
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        // Set the outputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem("Outputs");
                        // Check if outputs should be predicted
                        if (_predictionOn)
                        {
                            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
                            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
                            outputs = askForPrediction(inputs, _featuresOrderedDictionary.Count - 1);
                            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
                            //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://

                            // Calculate it automatically where tangent deviation of the state before R must be the highest
                            // Iterate through all possible tangent deviations
                            double[] selectedTdtVal = new double[10];
                            int start = (outputs[0] * 100000) - 5 > 0 ? (int)((outputs[0] * 100000) - 5) : 1;
                            int end = (outputs[0] * 100000) + 5 < 100000 ? (int)((outputs[0] * 100000) + 5) : 100000;
                            for (int i = start; i < end; i++)
                            {
                                signalStates = Garage.scanPeaks(_filteredSamples, Garage.amplitudeInterval(_filteredSamples), signalStatesViewerUserControl._thresholdRatio, _quantizationStep, i / 100000d, true);
                                // Iterate through all states and take the one before R if existed
                                int selectedSt = 0;
                                for (int j = 0; j < signalStates.Count; j++)
                                    if ((int)signalStates[j]._index + ((List<int[]>)_featuresOrderedDictionary[0])[shortPRBeatIndx][0] == ((List<int[]>)_featuresOrderedDictionary[0])[shortPRBeatIndx][4])
                                    {
                                        if (j > 0)
                                            selectedSt = j - 1;
                                        break;
                                    }
                                // Check if its just Q state
                                if ((int)signalStates[selectedSt]._index + ((List<int[]>)_featuresOrderedDictionary[0])[shortPRBeatIndx][0] == ((List<int[]>)_featuresOrderedDictionary[0])[shortPRBeatIndx][2])
                                {
                                    // If yes then select R state
                                    selectedTdtVal[i - start] = (double)signalStates[selectedSt + 1]._deviantionAngle;
                                }
                            }
                            // Now select the one with the highest deviation
                            double highestNmbr = 0;
                            for (int i = 0; i < 10; i++)
                                if (selectedTdtVal[i] > highestNmbr)
                                {
                                    highestNmbr = selectedTdtVal[i];
                                    outputs[0] = (double)(i + start) / 100000d;
                                }
                            // Set the selected _tdtThresholdRatio
                            signalStatesViewerUserControl._tdtThresholdRatio = outputs[0];
                        }
                        else
                            outputs[0] = (double)signalStatesViewerUserControl._tdtThresholdRatio;                    
                        flowLayoutItems02.DropDownItems.Add("Acc threa: " + outputs[0]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        featuresItems.DropDownItems.Add(flowLayoutItems01);

                        // Set delta index of the beat
                        signalStates = Garage.scanPeaks(_filteredSamples, Garage.amplitudeInterval(_filteredSamples), signalStatesViewerUserControl._thresholdRatio, _quantizationStep, signalStatesViewerUserControl._tdtThresholdRatio, true);
                        for (int i = 0; i < signalStates.Count; i++)
                        {
                            int qStateIndx = ((List<int[]>)_featuresOrderedDictionary[0])[shortPRBeatIndx][2] - ((List<int[]>)_featuresOrderedDictionary[0])[shortPRBeatIndx][0];
                            int rStateIndx = ((List<int[]>)_featuresOrderedDictionary[0])[shortPRBeatIndx][4] - ((List<int[]>)_featuresOrderedDictionary[0])[shortPRBeatIndx][0];
                            //if ((stateIndx - 1) <= (int)signalStates[i][1] && (int)signalStates[i][1] <= (stateIndx + 1))
                            if (qStateIndx <= (int)signalStates[i]._index && (int)signalStates[i]._index < rStateIndx)
                                ((List<int[]>)_featuresOrderedDictionary[0])[shortPRBeatIndx][3] = (int)signalStates[i]._index + ((List<int[]>)_featuresOrderedDictionary[0])[shortPRBeatIndx][0];
                        }

                        // Check if there exist next beat
                        if (nextShortPRBeatIndx != -1)
                        {
                            // If yes then set the next short PR beat
                            setNextBeat(nextShortPRBeatIndx, signalStatesViewerUserControl, _featuresOrderedDictionary.Count - 3, 0, featuresItems);

                            // Give the instruction for next goal, and enable previous button
                            featuresSettingInstructionsLabel.Text = "Set the best \"Acceleration threshold\" where the state before R must be as high as possible for delta of WPW syndrome detection.\n" + (featuresItems.DropDownItems.Count + 1) + "/" + shortPRNmbr + "\nPress next after you finish.";
                        } else
                        {
                            // If yes then insert all features in _featuresOrderedDictionary
                            featuresArray = new object[featuresItems.DropDownItems.Count];
                            for (int i = 0; i < featuresItems.DropDownItems.Count; i++)
                            {
                                // Set the inputs
                                inputs = new double[6];
                                for (int j = 0; j < 6; j++)
                                {
                                    inputs[j] = double.Parse(((ToolStripMenuItem)((ToolStripMenuItem)featuresItems.DropDownItems[i]).DropDownItems[0]).DropDownItems[j].Text.Split(new char[] { ':' })[1].Substring(1));
                                }
                                // Set the output
                                outputs = new double[1] { double.Parse(((ToolStripMenuItem)((ToolStripMenuItem)featuresItems.DropDownItems[i]).DropDownItems[1]).DropDownItems[0].Text.Split(new char[] { ':' })[1].Substring(1)) };

                                featuresArray[i] = new object[2] { inputs, outputs };
                            }
                            // Insert the new feature
                            _featuresOrderedDictionary.Add("delta_detection", featuresArray);

                            // Make the form ready for next goal
                            // Add the check box of delta declaration in filtersFlowLayoutPanel
                            filtersFlowLayoutPanel.Controls.Add(new CheckExistanceUserControl());
                            ((CheckExistanceUserControl)filtersFlowLayoutPanel.Controls[3]).existanceOfCheckBox.Text = "Existance of delta";
                            // Get number of declared short PR
                            shortPRBeatIndx = -1;
                            shortPRNmbr = 0;
                            for (int i = 0; i < ((object[])_featuresOrderedDictionary[_featuresOrderedDictionary.Count - 2]).Length; i++)
                            {
                                object[] features = (object[])((object[])_featuresOrderedDictionary[_featuresOrderedDictionary.Count - 2])[i];
                                if (features[1] != null)
                                    if (((double[])features[1])[0] >= threshold)
                                    {
                                        if (shortPRBeatIndx == -1)
                                            shortPRBeatIndx = i;
                                        shortPRNmbr++;
                                    }
                            }
                            // Show the first beat with short PR
                            signalStatesViewerUserControl.applyButton.Enabled = false;
                            // Disable acceleration and acceleration scrollbar
                            signalStatesViewerUserControl.tdtThresholdScrollBar.Enabled = false;
                            // Refresh the apply button if autoApply is checked
                            setNextBeat(shortPRBeatIndx, signalStatesViewerUserControl, _featuresOrderedDictionary.Count - 4, 0, featuresItems);

                            // Give the instruction for next goal, and enable previous button
                            featuresSettingInstructionsLabel.Text = "Check the box in the right list of filters if delta of WPW syndrome is detected.\n" + 1 + "/" + shortPRNmbr + "\nPress next after you finish.";
                        }
                        break;
                    case 7:
                        // This is for delta selection
                        // Get previous saved features if exists
                        if (featuresTableLayoutPanel.Controls.Count == 7)
                            featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[6]).Items[0];
                        else
                            featuresItems = new ToolStripMenuItem("Delta declaration features");

                        // Get curretn step threshold
                        threshold = ((float[])((List<object[]>)_targetsModelsHashtable[modelTypeComboBox.Text])[4 /*fifth step threshold*/][2])[0];
                        // Get current short PR beat and next short PR beat
                        shortPRNmbr = 0;
                        shortPRBeatIndx = -1;
                        nextShortPRBeatIndx = -1;
                        for (int i = 0; i < ((object[])_featuresOrderedDictionary[_featuresOrderedDictionary.Count - 2]).Length; i++)
                        {
                            object[] features = (object[])((object[])_featuresOrderedDictionary[_featuresOrderedDictionary.Count - 2])[i];
                            if (features[1] != null)
                                if (((double[])features[1])[0] >= threshold)
                                {
                                    if (shortPRNmbr == featuresItems.DropDownItems.Count)
                                        shortPRBeatIndx = i;
                                    else if (shortPRBeatIndx != -1 && nextShortPRBeatIndx == -1)
                                        nextShortPRBeatIndx = i;
                                    shortPRNmbr++;
                                }
                        }

                        // Set the input and output of detecting short PR of the selected beat
                        flowLayoutItems01 = new ToolStripMenuItem("R" + shortPRBeatIndx);
                        inputs = new double[6];
                        outputs = new double[1];

                        // Set the inputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem("Inputs");

                        inputs[0] = (double)(((List<int[]>)_featuresOrderedDictionary[0])[shortPRBeatIndx][3] - ((List<int[]>)_featuresOrderedDictionary[0])[shortPRBeatIndx][2]) / (double)(((List<int[]>)_featuresOrderedDictionary[0])[shortPRBeatIndx][4] - ((List<int[]>)_featuresOrderedDictionary[0])[shortPRBeatIndx][2]);
                        flowLayoutItems02.DropDownItems.Add("(ampDelta - ampQ) / (ampR - ampQ): " + inputs[0]);

                        statParams = Garage.statParams(Garage.normalizeSignal(_filteredSamples));
                        for (int j = 0; j < statParams.Count; j++)
                        {
                            inputs[j + 1] = (double)statParams[j];
                            flowLayoutItems02.DropDownItems.Add(statParams.Cast<DictionaryEntry>().ElementAt(j).Key.ToString() + ": " + inputs[j + 1]);
                        }
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        // Set the outputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem("Outputs");
                        // Check if outputs should be predicted
                        if (_predictionOn)
                            outputs = askForPrediction(inputs, _featuresOrderedDictionary.Count - 1);
                        else
                            outputs[0] = Convert.ToInt32(((CheckExistanceUserControl)filtersFlowLayoutPanel.Controls[3]).existanceOfCheckBox.Checked);
                        flowLayoutItems02.DropDownItems.Add("WPW syndrome: " + outputs[0]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        featuresItems.DropDownItems.Add(flowLayoutItems01);

                        // Get curretn step threshold
                        threshold = ((float[])((List<object[]>)_targetsModelsHashtable[modelTypeComboBox.Text])[6 /*seventh step threshold*/][2])[0];
                        // Set WPW syndrome index if existed
                        ((List<int[]>)_featuresOrderedDictionary[0])[shortPRBeatIndx][8] = int.MinValue;
                        if (outputs[0] > threshold)
                                ((List<int[]>)_featuresOrderedDictionary[0])[shortPRBeatIndx][8] = ((List<int[]>)_featuresOrderedDictionary[0])[shortPRBeatIndx][3];

                        // Check if there exist next beat
                        if (nextShortPRBeatIndx != -1)
                        {
                            // If yes then set the next short PR beat
                            // Uncheck short PR declaration in filtersFlowLayoutPanel
                            ((CheckExistanceUserControl)filtersFlowLayoutPanel.Controls[3]).existanceOfCheckBox.Checked = false;
                            setNextBeat(nextShortPRBeatIndx, signalStatesViewerUserControl, _featuresOrderedDictionary.Count - 4, featuresItems.DropDownItems.Count, featuresItems);

                            // Give the instruction for next goal, and enable previous button
                            featuresSettingInstructionsLabel.Text = "Set the best \"Acceleration threshold\" where the state before R must be as high as possible for delta of WPW syndrome detection.\n" + (featuresItems.DropDownItems.Count + 1) + "/" + shortPRNmbr + "\nPress next after you finish.";
                        }
                        else
                        {
                            // If yes then insert all features in _featuresOrderedDictionary
                            featuresArray = new object[featuresItems.DropDownItems.Count];
                            for (int i = 0; i < featuresItems.DropDownItems.Count; i++)
                            {
                                // Set the inputs
                                inputs = new double[6];
                                for (int j = 0; j < 6; j++)
                                {
                                    inputs[j] = double.Parse(((ToolStripMenuItem)((ToolStripMenuItem)featuresItems.DropDownItems[i]).DropDownItems[0]).DropDownItems[j].Text.Split(new char[] { ':' })[1].Substring(1));
                                }
                                // Set the output
                                outputs = new double[1] { double.Parse(((ToolStripMenuItem)((ToolStripMenuItem)featuresItems.DropDownItems[i]).DropDownItems[1]).DropDownItems[0].Text.Split(new char[] { ':' })[1].Substring(1)) };

                                featuresArray[i] = new object[2] { inputs, outputs };
                            }
                            // Insert the new feature
                            _featuresOrderedDictionary.Add("delta_declaration", featuresArray);

                            // Clear all filters from filtersFlowLayoutPanel
                            filtersFlowLayoutPanel.Controls.Clear();
                            _filteresHashtable.Clear();

                            // Set next button to finish
                            nextButton.Text = "Finish";

                            // Remove instruction
                            featuresSettingInstructionsLabel.Text = "";
                        }
                        break;
                }

                // Show selected features in featuresFlowLayoutPanel
                MenuStrip selectedFeature = new MenuStrip();
                selectedFeature.Name = featuresItems.Text.Replace(" ", "_");
                selectedFeature.Items.Add(featuresItems);
                if (featuresTableLayoutPanel.Controls.ContainsKey(selectedFeature.Name))
                    featuresTableLayoutPanel.Controls.RemoveByKey(selectedFeature.Name);
                featuresTableLayoutPanel.Controls.Add(selectedFeature);

                // Check if prediction is on and _featuresOrderedDictionary is less than 8 elements
                if (_predictionOn)
                {
                    // If yes then press next button
                    Thread nextButtonClickThread = new Thread(() => nextButton_Click(sender, e));
                    nextButtonClickThread.Start();
                }
            }));
        }

        private void setNextBeat(int beatIndex, SignalStatesViewerUserControl signalStatesViewerUserControl, int thresholdsIndx, int accelerationIndx, ToolStripMenuItem featuresItems)
        {
            _filteredSamples = new double[(((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][7] - ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][0]) + 1]; ;
            for (int i = ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][0]; i < ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][7] + 1; i++)
                _filteredSamples[i - ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][0]] = _samples[i];

            for (int i = 1; i < 4; i++)
                signalChart.Series[i].Points.Clear();
            signalStatesViewerUserControl.amplitudeThresholdScrollBar.Value = (int)(1000d - (((double[])((object[])((object[])_featuresOrderedDictionary[thresholdsIndx])[beatIndex])[1])[0] * 1000d));
            signalStatesViewerUserControl.hThresholdScrollBar.Maximum = _filteredSamples.Length + signalStatesViewerUserControl.hThresholdScrollBar.LargeChange - 1;
            signalStatesViewerUserControl.hThresholdScrollBar.Value = (int)(((double[])((object[])((object[])_featuresOrderedDictionary[thresholdsIndx])[beatIndex])[1])[1] * signalStatesViewerUserControl.hThresholdScrollBar.Maximum);
            if (_featuresOrderedDictionary.Count > 6)
                signalStatesViewerUserControl.tdtThresholdScrollBar.Value = (int)(((double[])((object[])((object[])_featuresOrderedDictionary[_featuresOrderedDictionary.Count - 1])[accelerationIndx])[1])[0] * 100000d);
            // Refresh the apply button if autoApply is checked
            applyFiltersOnSignal(_filteredSamples);
            _tempStatesList = signalStatesViewerUserControl.showSignalStates();

            // Set the up peaks labels ready
            for (int i = 1; i < 4; i++)
            {
                signalChart.Series[i].LabelForeColor = Color.Black;
                for (int j = 0; j < signalChart.Series[i].Points.Count; j++)
                {
                    int stateIndx = _tempStatesList[i - 1][j] + ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][0];
                    if ((stateIndx - 1) <= ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][1] && ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][1] <= (stateIndx + 1))
                        signalChart.Series[i].Points[j].Label = "P";
                    else if ((stateIndx - 1) <= ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][2] && ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][2] <= (stateIndx + 1))
                        signalChart.Series[i].Points[j].Label = "Q";
                    else if ((stateIndx - 1) <= ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][3] && ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][3] <= (stateIndx + 1))
                        signalChart.Series[i].Points[j].Label = "Delta";
                    else if ((stateIndx - 1) <= ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][4] && ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][4] <= (stateIndx + 1))
                        signalChart.Series[i].Points[j].Label = "R";
                    else if ((stateIndx - 1) <= ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][5] && ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][5] <= (stateIndx + 1))
                        signalChart.Series[i].Points[j].Label = "S";
                    else if ((stateIndx - 1) <= ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][6] && ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][6] <= (stateIndx + 1))
                        signalChart.Series[i].Points[j].Label = "T";
                    else
                        signalChart.Series[i].Points[j].Label = "";
                    if ((stateIndx - 1) <= ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][8] && ((List<int[]>)_featuresOrderedDictionary[0])[beatIndex][8] <= (stateIndx + 1))
                        signalChart.Series[i].Points[j].Label = "WPW";
                }
            }
        }

        public void finish()
        {
            // Show signal with peaks labels
            _filteredSamples = new double[_samples.Length];
            for (int i = 0; i < _filteredSamples.Length; i++)
                _filteredSamples[i] = _samples[i];
            applyButton_Click(null, null);

            List<object[]> peaksLabels = new List<object[]>();
            foreach (int[] beat in ((List<int[]>)_featuresOrderedDictionary[0]))
            {
                if (beat[1] != int.MinValue)
                    peaksLabels.Add(new object[] { beat[1], _samples[beat[1]], "P" });
                if (beat[2] != int.MinValue)
                    peaksLabels.Add(new object[] { beat[2], _samples[beat[2]], "Q" });
                if (beat[8] != int.MinValue)
                    peaksLabels.Add(new object[] { beat[8], _samples[beat[8]], "WPW" });
                else if (beat[3] != int.MinValue)
                    peaksLabels.Add(new object[] { beat[3], _samples[beat[3]], "Delta" });
                if (beat[4] != int.MinValue)
                    peaksLabels.Add(new object[] { beat[4], _samples[beat[4]], "R" });
                if (beat[5] != int.MinValue)
                    peaksLabels.Add(new object[] { beat[5], _samples[beat[5]], "S" });
                if (beat[6] != int.MinValue)
                    peaksLabels.Add(new object[] { beat[6], _samples[beat[6]], "T" });
            }

            double[] labelsX = new double[peaksLabels.Count];
            double[] labelsY = new double[peaksLabels.Count];
            string[] labels = new string[peaksLabels.Count];
            for (int i = 0; i < peaksLabels.Count; i++)
            {
                labelsX[i] = (double)((int)peaksLabels[i][0]) / _samplingRate;
                labelsY[i] = (double)peaksLabels[i][1];
                labels[i] = (string)peaksLabels[i][2];
            }

            for (int i = 1; i < 5; i++)
                signalChart.Series[i].Points.Clear();
            Garage.loadXYInChart(signalChart, labelsX, labelsY, labels, _startingInSec, 5, "FormDetailsModify");

            // Enable everything
            signalFusionButton.Enabled = true;
            signalsPickerComboBox.Enabled = true;
            fftButton.Enabled = true;
            filtersComboBox.Enabled = true;
            // Disable finish button
            nextButton.Enabled = false;

            // Enable save button in signal holder
            if (_signalHolder != null)
            _signalHolder.saveButton.Enabled = true;
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem featuresItems = null;
            SignalStatesViewerUserControl signalStatesViewerUserControl = null;
            foreach (string key in _filteresHashtable.Keys)
                if (key.Contains("Singal states viewer"))
                    signalStatesViewerUserControl = (SignalStatesViewerUserControl)filtersFlowLayoutPanel.Controls[filtersFlowLayoutPanel.Controls.IndexOfKey(key)];

            // Check if last features array should be removed
            if (_featuresOrderedDictionary.Count == featuresTableLayoutPanel.Controls.Count + 1)
                _featuresOrderedDictionary.RemoveAt(_featuresOrderedDictionary.Count - 1);

            // Get curretn step threshold
            float threshold = ((float[])((List<object[]>)_targetsModelsHashtable[modelTypeComboBox.Text])[4 /*fifth step threshold*/][2])[0];
            // Check which step of features selectoin is this
            switch (_featuresOrderedDictionary.Count)
            {
                case 7:
                    // This is for delta declaration
                    if (nextButton.Text.Equals("Finish"))
                    {
                        // Disable save button in signal holder
                        _signalHolder.saveButton.Enabled = false;
                        // Disable everything except ai tools
                        enableAITools(true);

                        // Set dc removal filter. Check it then set it disabled
                        filtersComboBox.SelectedIndex = 3;
                        ((DCRemovalUserControl)filtersFlowLayoutPanel.Controls[0]).dcValueRemoveCheckBox.Checked = true;
                        ((DCRemovalUserControl)filtersFlowLayoutPanel.Controls[0]).Enabled = false;
                        // Normalize signal
                        filtersComboBox.SelectedIndex = 4;
                        ((NormalizedSignalUserControl)filtersFlowLayoutPanel.Controls[1]).normalizeSignalCheckBox.Checked = false;
                        ((NormalizedSignalUserControl)filtersFlowLayoutPanel.Controls[1]).applyAfterTransformCheckBox.Checked = true;
                        ((NormalizedSignalUserControl)filtersFlowLayoutPanel.Controls[1]).Enabled = false;
                        // Set states visualiser
                        filtersComboBox.SelectedIndex = 6;
                        ((SignalStatesViewerUserControl)filtersFlowLayoutPanel.Controls[2]).showStatesCheckBox.Enabled = false;
                        ((SignalStatesViewerUserControl)filtersFlowLayoutPanel.Controls[2]).showDeviationCheckBox.Enabled = false;
                        ((SignalStatesViewerUserControl)filtersFlowLayoutPanel.Controls[2]).tdtThresholdScrollBar.Enabled = false;

                        // Enable next button
                        nextButton.Enabled = true;
                        nextButton.Text = "Next";

                        // Add the check box of delta declaration in filtersFlowLayoutPanel
                        filtersFlowLayoutPanel.Controls.Add(new CheckExistanceUserControl());
                        ((CheckExistanceUserControl)filtersFlowLayoutPanel.Controls[3]).existanceOfCheckBox.Text = "Existance of delta";
                        // Disable acceleration and acceleration scrollbar
                        signalStatesViewerUserControl = (SignalStatesViewerUserControl)filtersFlowLayoutPanel.Controls[2];
                        signalStatesViewerUserControl.tdtThresholdScrollBar.Enabled = false;
                        signalStatesViewerUserControl.amplitudeThresholdScrollBar.Enabled = false;
                        signalStatesViewerUserControl.hThresholdScrollBar.Enabled = false;
                        signalStatesViewerUserControl.showStatesCheckBox.Enabled = false;
                        signalStatesViewerUserControl.showDeviationCheckBox.Checked = true;
                        signalStatesViewerUserControl.showDeviationCheckBox.Enabled = false;
                        signalStatesViewerUserControl.applyButton.Enabled = false;
                    }

                    // Get previous saved features if exists
                    featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[6]).Items[0];
                    // Remove last feature from featuresItems if existed
                    featuresItems.DropDownItems.RemoveAt(featuresItems.DropDownItems.Count - 1);

                    // Get previous short PR beat
                    int shortPRNmbr = 0;
                    int previousShortPRBeatIndx = -1;
                    for (int i = 0; i < ((object[])_featuresOrderedDictionary[_featuresOrderedDictionary.Count - 2]).Length; i++)
                    {
                        object[] features = (object[])((object[])_featuresOrderedDictionary[_featuresOrderedDictionary.Count - 2])[i];
                        if (features[1] != null)
                            if (((double[])features[1])[0] >= threshold)
                            {
                                if (shortPRNmbr == featuresItems.DropDownItems.Count)
                                    previousShortPRBeatIndx = i;
                                shortPRNmbr++;
                            }
                    }
                    // Set the previous short PR beat
                    // Uncheck short PR declaration in filtersFlowLayoutPanel
                    ((CheckExistanceUserControl)filtersFlowLayoutPanel.Controls[3]).existanceOfCheckBox.Checked = false;
                    setNextBeat(previousShortPRBeatIndx, signalStatesViewerUserControl, _featuresOrderedDictionary.Count - 4, featuresItems.DropDownItems.Count, featuresItems);

                    // Give the instruction for next goal, and enable previous button
                    featuresSettingInstructionsLabel.Text = "Set the best \"Acceleration threshold\" where the state before R must be as high as possible for delta of WPW syndrome detection.\n" + (featuresItems.DropDownItems.Count + 1) + "/" + shortPRNmbr + "\nPress next after you finish.";
                    break;
                case 6:
                    // This is for delta detection
                    // Get previous saved features if exists
                    featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[5]).Items[0];
                    // Remove last feature from featuresItems if existed
                    featuresItems.DropDownItems.RemoveAt(featuresItems.DropDownItems.Count - 1);

                    // Get previous short PR beat
                    shortPRNmbr = 0;
                    previousShortPRBeatIndx = -1;
                    for (int i = 0; i < ((object[])_featuresOrderedDictionary[_featuresOrderedDictionary.Count - 1]).Length; i++)
                    {
                        object[] features = (object[])((object[])_featuresOrderedDictionary[_featuresOrderedDictionary.Count - 1])[i];
                        if (features[1] != null)
                            if (((double[])features[1])[0] >= threshold)
                            {
                                if (shortPRNmbr == featuresItems.DropDownItems.Count)
                                    previousShortPRBeatIndx = i;
                                shortPRNmbr++;
                            }
                    }

                    // Set last short PR for delta detection
                    if (featuresItems.DropDownItems.Count + 1 == shortPRNmbr)
                    {
                        // Remove the check box of short PR declaration in filtersFlowLayoutPanel
                        filtersFlowLayoutPanel.Controls.RemoveAt(3);
                        // Enable acceleration and acceleration scrollbar
                        signalStatesViewerUserControl.applyButton.Enabled = true;
                        signalStatesViewerUserControl.tdtThresholdScrollBar.Enabled = true;
                        signalStatesViewerUserControl.showDeviationCheckBox.Checked = true;
                    }
                    // Show the last beat with short PR
                    for (int i = 1; i < 4; i++)
                        signalChart.Series[i].Points.Clear();
                    setNextBeat(previousShortPRBeatIndx, signalStatesViewerUserControl, _featuresOrderedDictionary.Count - 3, 0, featuresItems);

                    // Give the instruction for next goal, and enable previous button
                    featuresSettingInstructionsLabel.Text = "Set the best \"Acceleration threshold\" where the state before R must be as high as possible for delta of WPW syndrome detection.\n" + (featuresItems.DropDownItems.Count + 1) + "/" + shortPRNmbr + "\nPress next after you finish.";
                    break;
                case 5:
                    // This is for short PR detection
                    if (nextButton.Text.Equals("Finish"))
                    {
                        // Disable save button in signal holder
                        _signalHolder.saveButton.Enabled = false;
                        // Disable everything except ai tools
                        enableAITools(true);

                        // Set dc removal filter. Check it then set it disabled
                        filtersComboBox.SelectedIndex = 3;
                        ((DCRemovalUserControl)filtersFlowLayoutPanel.Controls[0]).dcValueRemoveCheckBox.Checked = true;
                        ((DCRemovalUserControl)filtersFlowLayoutPanel.Controls[0]).Enabled = false;
                        // Normalize signal
                        filtersComboBox.SelectedIndex = 4;
                        ((NormalizedSignalUserControl)filtersFlowLayoutPanel.Controls[1]).normalizeSignalCheckBox.Checked = true;
                        ((NormalizedSignalUserControl)filtersFlowLayoutPanel.Controls[1]).applyAfterTransformCheckBox.Checked = true;
                        ((NormalizedSignalUserControl)filtersFlowLayoutPanel.Controls[1]).Enabled = false;
                        // Set states visualiser
                        filtersComboBox.SelectedIndex = 6;
                        ((SignalStatesViewerUserControl)filtersFlowLayoutPanel.Controls[2]).showStatesCheckBox.Enabled = false;
                        ((SignalStatesViewerUserControl)filtersFlowLayoutPanel.Controls[2]).showDeviationCheckBox.Enabled = false;
                        ((SignalStatesViewerUserControl)filtersFlowLayoutPanel.Controls[2]).tdtThresholdScrollBar.Enabled = false;

                        // Enable next button
                        nextButton.Enabled = true;
                        nextButton.Text = "Next";

                        // Disable acceleration and acceleration scrollbar
                        signalStatesViewerUserControl.tdtThresholdScrollBar.Enabled = false;
                        signalStatesViewerUserControl.amplitudeThresholdScrollBar.Enabled = false;
                        signalStatesViewerUserControl.hThresholdScrollBar.Enabled = false;
                        signalStatesViewerUserControl.showStatesCheckBox.Enabled = false;
                        signalStatesViewerUserControl.showDeviationCheckBox.Enabled = false;
                        signalStatesViewerUserControl.applyButton.Enabled = false;
                    }

                    // Get previous saved features if exists
                    featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[4]).Items[0];
                    // Remove last feature from featuresItems if existed
                    featuresItems.DropDownItems.RemoveAt(featuresItems.DropDownItems.Count - 1);

                    // Add the check box of short PR declaration in filtersFlowLayoutPanel
                    if (featuresItems.DropDownItems.Count + 1 == ((List<int[]>)_featuresOrderedDictionary[0]).Count)
                    {
                        signalStatesViewerUserControl.tdtThresholdScrollBar.Enabled = false;
                        signalStatesViewerUserControl.showDeviationCheckBox.Checked = false;
                        signalStatesViewerUserControl.showDeviationCheckBox.Enabled = false;
                        signalStatesViewerUserControl.applyButton.Enabled = false;

                        filtersFlowLayoutPanel.Controls.Add(new CheckExistanceUserControl());
                        ((CheckExistanceUserControl)filtersFlowLayoutPanel.Controls[3]).existanceOfCheckBox.Text = "Existance of short PR";
                    }

                    // Uncheck short PR declaration in filtersFlowLayoutPanel
                    ((CheckExistanceUserControl)filtersFlowLayoutPanel.Controls[3]).existanceOfCheckBox.Checked = false;
                    // Refresh the apply button if autoApply is checked
                    for (int i = 1; i < 4; i++)
                        signalChart.Series[i].Points.Clear();
                    int selectedBeatIndx = featuresItems.DropDownItems.Count;
                    setNextBeat(selectedBeatIndx, signalStatesViewerUserControl, _featuresOrderedDictionary.Count - 2, 0, featuresItems);

                    // Give the instruction for next goal, and enable previous button
                    featuresSettingInstructionsLabel.Text = "Check the box in the right list of filters if short PR is detected.\n" + (selectedBeatIndx + 1) + "/" + ((List<int[]>)_featuresOrderedDictionary[0]).Count + "\nPress next after you finish.";
                    break;
                case 4:
                    // This is for P and T selection
                    // Get previous saved features if exists
                    featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[3]).Items[0];
                    // Remove last feature from featuresItems if existed
                    featuresItems.DropDownItems.RemoveAt(featuresItems.DropDownItems.Count - 1);

                    // Remove the check box of short PR declaration in filtersFlowLayoutPanel
                    if (featuresItems.DropDownItems.Count + 1 == ((List<int[]>)_featuresOrderedDictionary[0]).Count)
                    {
                        filtersFlowLayoutPanel.Controls.RemoveAt(3);
                    }

                    // Set the next beat for segmentation
                    for (int i = 1; i < 4; i++)
                        signalChart.Series[i].Points.Clear();
                    selectedBeatIndx = featuresItems.DropDownItems.Count;
                    setNextBeat(selectedBeatIndx, signalStatesViewerUserControl, _featuresOrderedDictionary.Count - 1, 0, featuresItems);

                    // Give the instruction for next goal, and enable previous button
                    featuresSettingInstructionsLabel.Text = "Select P and T peaks from the visible states in the chart.\nClick once on the peak to select, or click once again to unselect.\n" + (selectedBeatIndx + 1) + "/" + ((List<int[]>)_featuresOrderedDictionary[0]).Count + "\nPress next after you finish.";
                    break;
                case 3:
                    // This is for beats peaks detection (P, T)
                    // Get previous saved features if exists
                    featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[2]).Items[0];
                    // Remove last feature from featuresItems if existed
                    featuresItems.DropDownItems.RemoveAt(featuresItems.DropDownItems.Count - 1);

                    // Set filters for this step
                    if (featuresItems.DropDownItems.Count + 1 == ((List<int[]>)_featuresOrderedDictionary[0]).Count)
                    {
                        for (int i = 1; i < 4; i++)
                            signalChart.Series[i].LabelForeColor = Color.Transparent;

                        signalStatesViewerUserControl.amplitudeThresholdScrollBar.Enabled = true;
                        signalStatesViewerUserControl.hThresholdScrollBar.Enabled = true;
                        signalStatesViewerUserControl.applyButton.Enabled = true;
                    }

                    // Set the previous beat for segmentation
                    _filteredSamples = new double[(((List<int[]>)_featuresOrderedDictionary[0])[featuresItems.DropDownItems.Count][7] - ((List<int[]>)_featuresOrderedDictionary[0])[featuresItems.DropDownItems.Count][0]) + 1];
                    for (int i = ((List<int[]>)_featuresOrderedDictionary[0])[featuresItems.DropDownItems.Count][0]; i < ((List<int[]>)_featuresOrderedDictionary[0])[featuresItems.DropDownItems.Count][7] + 1; i++)
                        _filteredSamples[i - ((List<int[]>)_featuresOrderedDictionary[0])[featuresItems.DropDownItems.Count][0]] = _samples[i];
                    if (signalStatesViewerUserControl.hThresholdScrollBar.Maximum != (_filteredSamples.Length + signalStatesViewerUserControl.hThresholdScrollBar.LargeChange - 1))
                    {
                        signalStatesViewerUserControl.hThresholdScrollBar.Maximum = _filteredSamples.Length + signalStatesViewerUserControl.hThresholdScrollBar.LargeChange - 1;
                        signalStatesViewerUserControl.hThresholdScrollBar.Value = signalStatesViewerUserControl.hThresholdScrollBar.Value * (_filteredSamples.Length + signalStatesViewerUserControl.hThresholdScrollBar.LargeChange - 1) / signalStatesViewerUserControl.hThresholdScrollBar.Maximum;
                    }
                    // Refresh the apply button if autoApply is checked
                    for (int i = 1; i < 4; i++)
                        signalChart.Series[i].Points.Clear();
                    applyFiltersOnSignal(_filteredSamples);
                    _tempStatesList = signalStatesViewerUserControl.showSignalStates();

                    // Give the instruction for next goal, and enable previous button
                    featuresSettingInstructionsLabel.Text = "Set the best \"Threshold ratio\" and \"Hor Threshold\" for the segmentation of P and T waves.\n" + (featuresItems.DropDownItems.Count + 1) + "/" + ((List<int[]>)_featuresOrderedDictionary[0]).Count + "\nPress next after you finish.";
                    break;
                case 2:
                    // This is for QRS selection
                    // Get previous saved features if exists
                    featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[1]).Items[0];
                    // Remove last feature from featuresItems if existed
                    featuresItems.DropDownItems.Clear();

                    // Refresh the apply button if autoApply is checked
                    for (int i = 1; i < 4; i++)
                        signalChart.Series[i].Points.Clear();
                    _filteredSamples = new double[_samples.Length];
                    for (int i = 0; i < _filteredSamples.Length; i++)
                        _filteredSamples[i] = _samples[i];
                    applyButton_Click(null, null);
                    signalStatesViewerUserControl.amplitudeThresholdScrollBar.Value = (int)(1000d - (((double[])((object[])_featuresOrderedDictionary[1])[1])[0] * 1000d));
                    signalStatesViewerUserControl.hThresholdScrollBar.Maximum = _filteredSamples.Length + signalStatesViewerUserControl.hThresholdScrollBar.LargeChange - 1;
                    signalStatesViewerUserControl.hThresholdScrollBar.Value = (int)(((double[])((object[])_featuresOrderedDictionary[1])[1])[1] * signalStatesViewerUserControl.hThresholdScrollBar.Maximum);
                    _tempStatesList = signalStatesViewerUserControl.showSignalStates();
                    signalStatesViewerUserControl.Enabled = false;
                    // Set the up peaks labels ready
                    signalChart.Series["Up peaks"].LabelForeColor = Color.Black;
                    for (int i = 0; i < signalChart.Series["Up peaks"].Points.Count; i++)
                        signalChart.Series["Up peaks"].Points[i].Label = "";

                    // Give the instruction for next goal, and enable previous button
                    featuresSettingInstructionsLabel.Text = "Select R peaks from the visible states in the chart.\nClick once on the peak to select, or click once again to unselect.\nPress next after you finish.";
                    break;
                case 1:
                    // This is for QRS detection
                    // Get previous saved features if exists
                    featuresItems = (ToolStripMenuItem)((MenuStrip)featuresTableLayoutPanel.Controls[0]).Items[0];
                    // Remove last feature from featuresItems if existed
                    featuresItems.DropDownItems.Clear();

                    // Set the up peaks labels ready
                    for (int i = 1; i < 4; i++)
                        signalChart.Series[i].Points.Clear();
                    signalChart.Series["Up peaks"].LabelForeColor = Color.Transparent;
                    signalStatesViewerUserControl.Enabled = true;
                    dwtLevelsComboBox.Enabled = true;
                    // Abdolute signal maker
                    filtersComboBox.SelectedIndex = 5;
                    ((AbsoluteSignalUserControl)filtersFlowLayoutPanel.Controls[3]).absoluteSignalCheckBox.Checked = true;
                    ((AbsoluteSignalUserControl)filtersFlowLayoutPanel.Controls[3]).applyAfterTransformCheckBox.Checked = true;
                    ((AbsoluteSignalUserControl)filtersFlowLayoutPanel.Controls[3]).Enabled = false;

                    // Get first 3 levels of DWT of the signal
                    applyDWT(_samples, 3);

                    // Show instructions of the first goal
                    featuresSettingInstructionsLabel.Text = "Set the best \"Threshold ratio\" and \"Hor Threshold\" so that only high peaks are visible.\n(Try setting them as high as possible for the third level).\nPress next after you finish.";

                    // Disable previous button
                    previousButton.Enabled = false;
                    break;
            }

            // Show selected features in featuresFlowLayoutPanel
            MenuStrip selectedFeature = new MenuStrip();
            selectedFeature.Name = featuresItems.Text.Replace(" ", "_");
            selectedFeature.Items.Add(featuresItems);
            if (featuresTableLayoutPanel.Controls.ContainsKey(selectedFeature.Name))
                featuresTableLayoutPanel.Controls.RemoveByKey(selectedFeature.Name);
            if (featuresItems.DropDownItems.Count > 0)
                featuresTableLayoutPanel.Controls.Add(selectedFeature);
        }

        public void initializeAITools()
        {
            // Set features in featuresTableLayoutPanel from _featuresOrderedDictionary
            ToolStripMenuItem featuresItems = null;
            ToolStripMenuItem flowLayoutItems01 = null;
            ToolStripMenuItem flowLayoutItems02 = null;
            ToolStripMenuItem flowLayoutItems03 = null;
            double[] inputs;
            double[] outputs;
            for (int i = 1; i < _featuresOrderedDictionary.Count; i++)
            {
                if (i == 1)
                {
                    // This is for QRS detection
                    featuresItems = new ToolStripMenuItem("QRS detection features");

                    inputs = (double[])((object[])_featuresOrderedDictionary[i])[0];
                    outputs = (double[])((object[])_featuresOrderedDictionary[i])[1];
                    // Iterate through 3 levels of dwt
                    flowLayoutItems01 = new ToolStripMenuItem("Inputs");
                    for (int j = 0; j < 3; j++)
                        for (int k = 0; k < 5; k++)
                        {
                            if (k == 0)
                                flowLayoutItems01.DropDownItems.Add("mean: " + inputs[j * 5 + k]);
                            else if (k == 1)
                                flowLayoutItems01.DropDownItems.Add("min: " + inputs[j * 5 + k]);
                            else if (k == 2)
                                flowLayoutItems01.DropDownItems.Add("max: " + inputs[j * 5 + k]);
                            else if (k == 3)
                                flowLayoutItems01.DropDownItems.Add("std_dev: " + inputs[j * 5 + k]);
                            else
                                flowLayoutItems01.DropDownItems.Add("iqr: " + inputs[j * 5 + k]);
                        }
                    featuresItems.DropDownItems.Add(flowLayoutItems01);
                    // get signal states viewer user control and set the features output
                    flowLayoutItems01 = new ToolStripMenuItem("Outputs");
                    flowLayoutItems01.DropDownItems.Add("Threa ratio: " + outputs[0]);
                    flowLayoutItems01.DropDownItems.Add("Hor threa: " + outputs[1]);

                    featuresItems.DropDownItems.Add(flowLayoutItems01);
                } else if (i == 2)
                {
                    // This is for QRS selection
                    featuresItems = new ToolStripMenuItem("QRS selection features");

                    for (int j = 0; j < ((object[])_featuresOrderedDictionary[i]).Length; j++)
                    {
                        inputs = (double[])((object[])((object[])_featuresOrderedDictionary[i])[j])[0];
                        outputs = (double[])((object[])((object[])_featuresOrderedDictionary[i])[j])[1];
                        flowLayoutItems01 = new ToolStripMenuItem("R" + j);
                        flowLayoutItems02 = new ToolStripMenuItem("Inputs");
                        flowLayoutItems02.DropDownItems.Add("RpRcur/RRav: " + inputs[0]);
                        flowLayoutItems02.DropDownItems.Add("ampRcur/ampRp: " + inputs[1]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);
                        flowLayoutItems02 = new ToolStripMenuItem("Outputs");
                        flowLayoutItems02.DropDownItems.Add("Remove R: " + outputs[0]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        featuresItems.DropDownItems.Add(flowLayoutItems01);
                    }
                } else if (i == 3)
                {
                    // This is for beats peaks detection (P, T)
                    featuresItems = new ToolStripMenuItem("P,T detection features");

                    for (int j = 0; j < ((object[])_featuresOrderedDictionary[i]).Length; j++)
                    {
                        inputs = (double[])((object[])((object[])_featuresOrderedDictionary[i])[j])[0];
                        outputs = (double[])((object[])((object[])_featuresOrderedDictionary[i])[j])[1];
                        flowLayoutItems01 = new ToolStripMenuItem("R" + j);
                        flowLayoutItems02 = new ToolStripMenuItem("Inputs");
                        flowLayoutItems02.DropDownItems.Add("mean: " + inputs[0]);
                        flowLayoutItems02.DropDownItems.Add("min: " + inputs[1]);
                        flowLayoutItems02.DropDownItems.Add("max: " + inputs[2]);
                        flowLayoutItems02.DropDownItems.Add("std_dev: " + inputs[3]);
                        flowLayoutItems02.DropDownItems.Add("iqr: " + inputs[4]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);
                        flowLayoutItems02 = new ToolStripMenuItem("Outputs");
                        flowLayoutItems02.DropDownItems.Add("Threa ratio: " + outputs[0]);
                        flowLayoutItems02.DropDownItems.Add("Hor threa: " + outputs[1]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        featuresItems.DropDownItems.Add(flowLayoutItems01);
                    }
                } else if (i == 4)
                {
                    // This is for P and T selection
                    featuresItems = new ToolStripMenuItem("P,T selection features");

                    List<object[]> beatStates;
                    for (int j = 0; j < ((object[])_featuresOrderedDictionary[i]).Length; j++)
                    {
                        flowLayoutItems01 = new ToolStripMenuItem("R" + j);
                        beatStates = (List<object[]>)((object[])_featuresOrderedDictionary[i])[j];
                        for (int k = 0; k < beatStates.Count; k++)
                        {
                            inputs = (double[])beatStates[k][0];
                            outputs = (double[])beatStates[k][1];

                            flowLayoutItems02 = new ToolStripMenuItem("St" + k);
                            // Set the inputs of the state
                            flowLayoutItems03 = new ToolStripMenuItem("Inputs");
                            flowLayoutItems03.DropDownItems.Add("St" + i + ": " + inputs[0]);
                            if (j == 0)
                                flowLayoutItems03.DropDownItems.Add("St" + k + "-R" + j + "/R1-R" + j + ": " + inputs[1]);
                            else
                                flowLayoutItems03.DropDownItems.Add("St" + k + "-R" + j + "/R" + j + "-R" + (j - 1) + ": " + inputs[1]);
                            flowLayoutItems03.DropDownItems.Add("((ampSt" + k + " - ampSt" + (k - 1) + ") + " + "(ampSt" + k + " - ampSt" + (k + 1) + ")) / 2" + ": " + inputs[2]);
                            flowLayoutItems02.DropDownItems.Add(flowLayoutItems03);
                            // Set the ooutputs of the state
                            flowLayoutItems03 = new ToolStripMenuItem("Outputs");
                            flowLayoutItems03.DropDownItems.Add("P wave: " + outputs[0]);
                            flowLayoutItems03.DropDownItems.Add("T wave: " + outputs[1]);
                            flowLayoutItems02.DropDownItems.Add(flowLayoutItems03);
                            flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);
                        }
                        featuresItems.DropDownItems.Add(flowLayoutItems01);
                    }
                } else if (i == 5)
                {
                    // This is for short PR detection
                    featuresItems = new ToolStripMenuItem("Short PR detection features");

                    for (int j = 0; j < ((object[])_featuresOrderedDictionary[i]).Length; j++)
                    {
                        inputs = (double[])((object[])((object[])_featuresOrderedDictionary[i])[j])[0];
                        outputs = (double[])((object[])((object[])_featuresOrderedDictionary[i])[j])[1];

                        // Set the input and output of detecting short PR of the selected beat
                        flowLayoutItems01 = new ToolStripMenuItem("R" + j);
                        // Set the inputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem("Inputs");
                        // Check if P state exists
                        if (inputs != null)
                            flowLayoutItems02.DropDownItems.Add("PQ/PR: " + inputs[0]);
                        else
                            flowLayoutItems02.DropDownItems.Add("PQ/PR: NaN");
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        // Set the outputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem("Outputs");
                        // Check if P state exists
                        if (outputs != null)
                            flowLayoutItems02.DropDownItems.Add("Short PR: " + outputs[0]);
                        else
                            flowLayoutItems02.DropDownItems.Add("Short PR: NaN");
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);
                        featuresItems.DropDownItems.Add(flowLayoutItems01);
                    }
                } else if (i == 6)
                {
                    // This is for delta detection
                    featuresItems = new ToolStripMenuItem("Delta detection features");

                    for (int j = 0; j < ((object[])_featuresOrderedDictionary[i]).Length; j++)
                    {
                        inputs = (double[])((object[])((object[])_featuresOrderedDictionary[i])[j])[0];
                        outputs = (double[])((object[])((object[])_featuresOrderedDictionary[i])[j])[1];

                        // Set the input and output of detecting short PR of the selected beat
                        flowLayoutItems01 = new ToolStripMenuItem("R" + j);
                        // Set the inputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem("Inputs");
                        flowLayoutItems02.DropDownItems.Add("PQ/PR: " + inputs[0]);
                        flowLayoutItems02.DropDownItems.Add("mean: " + inputs[1]);
                        flowLayoutItems02.DropDownItems.Add("min: " + inputs[2]);
                        flowLayoutItems02.DropDownItems.Add("max: " + inputs[3]);
                        flowLayoutItems02.DropDownItems.Add("std_dev: " + inputs[4]);
                        flowLayoutItems02.DropDownItems.Add("iqr: " + inputs[5]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        // Set the outputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem("Outputs");
                        flowLayoutItems02.DropDownItems.Add("Acc threa: " + outputs[0]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        featuresItems.DropDownItems.Add(flowLayoutItems01);
                    }                    
                } else if (i == 7)
                {
                    // This is for delta declaration
                    featuresItems = new ToolStripMenuItem("Delta declaration features");

                    for (int j = 0; j < ((object[])_featuresOrderedDictionary[i]).Length; j++)
                    {
                        inputs = (double[])((object[])((object[])_featuresOrderedDictionary[i])[j])[0];
                        outputs = (double[])((object[])((object[])_featuresOrderedDictionary[i])[j])[1];

                        // Set the input and output of detecting short PR of the selected beat
                        flowLayoutItems01 = new ToolStripMenuItem("R" + j);
                        // Set the inputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem("Inputs");
                        flowLayoutItems02.DropDownItems.Add("(ampDelta - ampQ) / (ampR - ampQ): " + inputs[0]);
                        flowLayoutItems02.DropDownItems.Add("mean: " + inputs[1]);
                        flowLayoutItems02.DropDownItems.Add("min: " + inputs[2]);
                        flowLayoutItems02.DropDownItems.Add("max: " + inputs[3]);
                        flowLayoutItems02.DropDownItems.Add("std_dev: " + inputs[4]);
                        flowLayoutItems02.DropDownItems.Add("iqr: " + inputs[5]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        // Set the outputs of the state
                        flowLayoutItems02 = new ToolStripMenuItem("Outputs");
                        flowLayoutItems02.DropDownItems.Add("WPW syndrome: " + outputs[0]);
                        flowLayoutItems01.DropDownItems.Add(flowLayoutItems02);

                        featuresItems.DropDownItems.Add(flowLayoutItems01);
                    }
                }

                // Show selected features in featuresFlowLayoutPanel
                MenuStrip selectedFeature = new MenuStrip();
                selectedFeature.Name = featuresItems.Text.Replace(" ", "_");
                selectedFeature.Items.Add(featuresItems);
                featuresTableLayoutPanel.Controls.Add(selectedFeature);
            }
        }

        public void initializeAIFilters()
        {
            // Remove all filters
            filtersFlowLayoutPanel.Controls.Clear();
            _filteresHashtable.Clear();

            signalFusionButton.Enabled = false;
            signalsPickerComboBox.Enabled = false;
            fftButton.Enabled = false;
            dwtButton.Enabled = false;
            editButton.Enabled = false;
            filtersComboBox.Enabled = false;

            signalsPickerComboBox.SelectedIndex = 1;

            // Enable discard and next button, and disable this button
            discardButton.Enabled = true;
            previousButton.Enabled = true;
            nextButton.Enabled = true;
            setFeaturesLabelsButton.Enabled = false;

            // Set filters according to its corresponding step
            // Set dc removal filter. Check it then set it disabled
            filtersComboBox.SelectedIndex = 3;
            ((DCRemovalUserControl)filtersFlowLayoutPanel.Controls[0]).dcValueRemoveCheckBox.Checked = true;
            ((DCRemovalUserControl)filtersFlowLayoutPanel.Controls[0]).Enabled = false;
            // Normalize signal
            filtersComboBox.SelectedIndex = 4;
            ((NormalizedSignalUserControl)filtersFlowLayoutPanel.Controls[1]).normalizeSignalCheckBox.Checked = true;
            ((NormalizedSignalUserControl)filtersFlowLayoutPanel.Controls[1]).applyAfterTransformCheckBox.Checked = true;
            ((NormalizedSignalUserControl)filtersFlowLayoutPanel.Controls[1]).Enabled = false;

            // Set states visualiser
            filtersComboBox.SelectedIndex = 6;
            ((SignalStatesViewerUserControl)filtersFlowLayoutPanel.Controls[2]).showStatesCheckBox.Enabled = false;
            ((SignalStatesViewerUserControl)filtersFlowLayoutPanel.Controls[2]).showDeviationCheckBox.Enabled = false;
            ((SignalStatesViewerUserControl)filtersFlowLayoutPanel.Controls[2]).tdtThresholdScrollBar.Enabled = false;

            // Get first 3 levels of DWT of the signal
            applyDWT(_samples, 3);
            // Clear dwtLayersComboBox and disable it
            dwtLevelsComboBox.DataSource = null;
            dwtLevelsComboBox.Enabled = false;
            // Refresh the apply button if autoApply is checked
            applyButton_Click(null, null);

            int i = _featuresOrderedDictionary.Count;
            if (i == 1)
            {
                // Abdolute signal maker
                filtersComboBox.SelectedIndex = 5;
                ((AbsoluteSignalUserControl)filtersFlowLayoutPanel.Controls[3]).absoluteSignalCheckBox.Checked = true;
                ((AbsoluteSignalUserControl)filtersFlowLayoutPanel.Controls[3]).applyAfterTransformCheckBox.Checked = true;
                ((AbsoluteSignalUserControl)filtersFlowLayoutPanel.Controls[3]).Enabled = false;
            }
            else if (i == 5)
            {
                // Add the check box of delta declaration in filtersFlowLayoutPanel
                filtersFlowLayoutPanel.Controls.Add(new CheckExistanceUserControl());
                ((CheckExistanceUserControl)filtersFlowLayoutPanel.Controls[3]).existanceOfCheckBox.Text = "Existance of short PR";
            }
            else if (i == 7)
            {
                // Add the check box of delta declaration in filtersFlowLayoutPanel
                filtersFlowLayoutPanel.Controls.Add(new CheckExistanceUserControl());
                ((CheckExistanceUserControl)filtersFlowLayoutPanel.Controls[3]).existanceOfCheckBox.Text = "Existance of delta";
            }
            else if (i == 8)
                // Set next button to finish
                nextButton.Text = "Finish";
        }
    }
}

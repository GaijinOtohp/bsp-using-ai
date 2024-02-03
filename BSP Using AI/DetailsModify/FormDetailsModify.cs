using BSP_Using_AI.SignalHolderFolder;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.DetailsModify
{
    public partial class FormDetailsModify : Form
    {
        public Dictionary<string, IPlottable> _Plots = new Dictionary<string, IPlottable>();

        public SignalHolder _signalHolder { get; set; }

        public ARTHTFeatures _arthtFeatures = new ARTHTFeatures();

        bool _predictionOn = false;
        public Dictionary<string, ARTHTModels> _arthtModelsDic = null;

        bool _mouseDown = false;
        int _previousMouseX;
        int _previousMouseY;

        public long _id;
        public FormDetailsModify(FilteringTools filteringTools, String path)
        {
            InitializeComponent();

            // Initialize form
            _FilteringTools = filteringTools;
            _FilteringTools.SetFormDetailModify(this);
            pathLabel.Text = path;
            samplingRateTextBox.Text = _FilteringTools._samplingRate.ToString();
            _FilteringTools.SetAutoApply(false);
            quantizationStepTextBox.Text = _FilteringTools._quantizationStep.ToString();

            signalsPickerComboBox.SelectedIndex = 1;
            aiGoalComboBox.SelectedIndex = 0;

            // Set plots titles and labels
            signalChart.Plot.XAxis.Label("Time (s)");
            signalChart.Plot.YAxis.Label("Voltage (mV)");
            signalChart.Plot.Legend();
            signalChart.Refresh();
            spectrumChart.Plot.XAxis.Label("Frequency (Hz)");
            spectrumChart.Plot.YAxis.Label("Magnitude (mV)");
            spectrumChart.Refresh();

            // Insert signal, up, down, stable, selection, and labels plots in signalChart
            _Plots.Add(SANamings.Signal, signalChart.Plot.GetPlottables()[0]);
            _Plots.Add(SANamings.UpPeaks, Garage.AddScatterPlot(signalChart, Color.Blue, label: SANamings.UpPeaks));
            _Plots.Add(SANamings.DownPeaks, Garage.AddScatterPlot(signalChart, Color.Red, label: SANamings.DownPeaks));
            _Plots.Add(SANamings.StableStates, Garage.AddScatterPlot(signalChart, Color.Black, label: SANamings.StableStates));
            _Plots.Add(SANamings.Selection, signalChart.Plot.AddBubblePlot());
            _Plots.Add(SANamings.Labels, Garage.AddScatterPlot(signalChart, Color.Blue, label: SANamings.Labels));

            _FilteringTools.SetAutoApply(true);
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public void initializeForm(SignalHolder signalHolder)
        {
            _signalHolder = signalHolder;
            _signalHolder._FilteringTools = _FilteringTools;
            _arthtFeatures = _signalHolder._arthtFeatures;

            // Insert previous selected filters if existed
            if (_FilteringTools._FiltersDic.Count > 0)
            {
                List<FilterBase> sortedFiltersDic = _FilteringTools._FiltersDic.OrderBy(filter => filter.Value._sortOrder).Select(filter => filter.Value).ToList();
                foreach (FilterBase filter in sortedFiltersDic)
                    filtersFlowLayoutPanel.Controls.Add(filter._FilterControl);
            }

            // Check if there is features selected
            if (_arthtFeatures._processedStep > 0)
            {
                initializeAIFilters();
                initializeAITools();
                previousButton_Click(null, null);
            }

            // Add AI prediction models in modelTypeComboBox
            List<(string modelName, string modelNameProblem)> modelsNamesList = new List<(string, string)>();
            foreach (ARTHTModels model in ((MainForm)signalHolder.FindForm())._arthtModelsDic.Values)
                modelsNamesList.Add((model.ModelName, model.ModelName + model.ProblemName));
            modelsNamesList = Garage.OrderByTextWithNumbers(modelsNamesList, modelsNamesList.Select(item => item.modelNameProblem).ToList());
            modelTypeComboBox.DisplayMember = "modelNameProblem";
            modelTypeComboBox.ValueMember = "modelName";
            foreach ((string modelName, string modelNameProblem) modelsNames in modelsNamesList)
                modelTypeComboBox.Items.Add(new { modelName = modelsNames.modelName, modelNameProblem = modelsNames.modelNameProblem });
        }

        private void loadSignal(double[] samples, double samplingRate, double startingInSec)
        {
            if (samples == null)
                return;

            // Calculate fft of signal
            try
            {
                double[] fftMag = applyFFT(samples);

                // Load signals inside charts
                Garage.loadSignalInChart(signalChart, samples, samplingRate, startingInSec, "FormDetailsModifySignal");

                // Set the real frequency rate
                // ftMag has only half of the spectrum (the real frequencies)
                double hzRate = (fftMag.Length * 2) / samplingRate;
                // Load fft inside its chart
                Garage.loadSignalInChart(spectrumChart, fftMag, hzRate, 0, "FormDetailsModify");
            }
            catch (Exception e)
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
    }
}

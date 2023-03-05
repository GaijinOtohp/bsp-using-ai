using BSP_Using_AI.AITools;
using BSP_Using_AI.DetailsModify.Filters;
using BSP_Using_AI.DetailsModify.SignalFusion;
using BSP_Using_AI.SignalHolderFolder;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace BSP_Using_AI.DetailsModify
{
    public partial class FormDetailsModify : Form
    {
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
            List<string> modelsList = new List<string>();
            foreach (string model in ((MainForm)signalHolder.FindForm())._arthtModelsDic.Keys)
                modelsList.Add(model);
            modelsList = Garage.OrderByTextWithNumbers(modelsList, modelsList);
            modelTypeComboBox.DataSource = modelsList;
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

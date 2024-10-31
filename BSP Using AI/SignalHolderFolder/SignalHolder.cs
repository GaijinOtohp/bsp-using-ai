using Biological_Signal_Processing_Using_AI.Garage;
using System;
using System.Linq;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace BSP_Using_AI.SignalHolderFolder
{
    public partial class SignalHolder : UserControl
    {
        public FilteringTools _FilteringTools { get; set; } = new FilteringTools(1, 1, null);

        public double[] _samples { get; set; }

        public ARTHTFeatures _arthtFeatures { get; set; }

        public SignalHolder()
        {
            InitializeComponent();

            // Initialize variables
            _arthtFeatures = new ARTHTFeatures();
        }

        //*******************************************************************************************************//
        //*******************************************CLASS FUNCTIONS*********************************************//
        /// <summary> 
        /// Loads the selected signal in signalExhibitor from _samples, starting from an included param in secs.
        /// The signal is loaded for 15 secs as max.
        /// </summary>
        public void loadSignalStartingFrom(double startingInSecs)
        {
            // Set the new startingInSec
            _FilteringTools.SetStartingInSecond(startingInSecs);

            // Get the selected signal span
            double spanInSecs = 10;
            if (signalSpanTextBox.Text.Length > 0)
                spanInSecs = double.Parse(signalSpanTextBox.Text);

            // Get the selected samples
            int startingIndex = (int)(startingInSecs * _FilteringTools._samplingRate);
            int endingIndex = (int)((startingInSecs + spanInSecs) * _FilteringTools._samplingRate);
            double[] truncatedSamples = _samples.Where((value, index) => startingIndex <= index && index <= endingIndex).ToArray();

            // Insert signal values inside signal holder chart
            _FilteringTools.SetOriginalSamples(truncatedSamples);
            GeneralTools.loadSignalInChart(signalExhibitor, _FilteringTools._RawSamples, _FilteringTools._samplingRate, _FilteringTools._startingInSec, "SignalHolderSignal");

            // Set the limits of the plot to be 10 seconds
            double span = Math.Min(10, truncatedSamples.Length / (double)_FilteringTools._samplingRate);
            signalExhibitor.Plot.SetAxisLimitsX(startingInSecs, startingInSecs + span);
            signalExhibitor.Refresh();
        }
    }
}

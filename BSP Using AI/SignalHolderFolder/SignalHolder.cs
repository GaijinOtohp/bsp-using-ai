using Biological_Signal_Processing_Using_AI.Garage;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace BSP_Using_AI.SignalHolderFolder
{
    public partial class SignalHolder : UserControl
    {
        public FilteringTools _FilteringTools { get; set; } = new FilteringTools(1, 1, null);

        public double[] _samples { get; set; }

        public ARTHTFeatures _arthtFeatures { get; set; }

        bool _mouseDown = false;
        int _previousMouseX;
        int _previousMouseY;

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
            double[] truncatedSamples;
            // Check if the signal is more than 10 secs
            if ((_samples.Length / _FilteringTools._samplingRate) > 10)
            {
                double truncPeriod = 10;
                // If yes then plot only 10 secs of the signal
                // Check if the inserted starting param is negative number
                if (startingInSecs < 0)
                    // If yes then set the starting param as 0
                    startingInSecs = 0;

                // starting from the inserted param as in secs
                int starting = (int)(startingInSecs * _FilteringTools._samplingRate);
                int ending = starting + (int)(truncPeriod * _FilteringTools._samplingRate);
                // Check if _samples contains enogh samples for 10 sec starting from the starting index
                if (ending > _samples.Length)
                {
                    // If yes then set the ending as the length of _samples
                    ending = _samples.Length;
                    // and the starting as 10 secs from the ending
                    starting = ending - (int)(truncPeriod * _FilteringTools._samplingRate);
                }

                // Set the new startingInSec
                _FilteringTools.SetStartingInSecond(starting / (double)_FilteringTools._samplingRate);

                int numSamples = ending - starting;
                truncatedSamples = new double[numSamples];

                for (int i = 0; i < numSamples; i++)
                    truncatedSamples[i] = _samples[i + starting];
            }
            else
            {
                // If yes then copy _samples in _truncatedSamples
                truncatedSamples = _samples;
            }

            // Insert signal values inside signal holder chart
            _FilteringTools.SetOriginalSamples(truncatedSamples);
            GeneralTools.loadSignalInChart(signalExhibitor, _FilteringTools._RawSamples, _FilteringTools._samplingRate, _FilteringTools._startingInSec, "SignalHolderSignal");
        }
    }
}

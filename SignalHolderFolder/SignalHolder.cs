using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.Specialized;
using System.Threading;

namespace BSP_Using_AI.SignalHolderFolder
{
    public partial class SignalHolder : UserControl
    {

        public double _samplingRate = 0D;
        public double _quantizationStep = 0D;

        public double _startingInSec = 0D;

        public double[] _samples = null;
        public double[] _truncatedSamples = null;
        public double[] _filteredSamples = null;

        public Hashtable _filteresHashtable = null;
        public OrderedDictionary _featuresOrderedDictionary = null;

        bool _mouseDown = false;
        int _previousMouseX;
        int _previousMouseY;

        public SignalHolder()
        {
            InitializeComponent();

            // Initialize variables
            _filteresHashtable = new Hashtable();
            _featuresOrderedDictionary = new System.Collections.Specialized.OrderedDictionary();
        }

        private void chooseFileButton_Click(object sender, EventArgs e)
        {
            EventHandlers.chooseFileButton_Click(sender, e);
        }

        private void detailsModifyButton_Click(object sender, EventArgs e)
        {
            EventHandlers.detailsModifyButton_Click(sender);
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            EventHandlers.forwardButton_Click(sender);
        }

        private void backwardButton_Click(object sender, EventArgs e)
        {
            EventHandlers.backwardButton_Click(sender);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Save the signal with its features in dataset
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.initialize("dataset", new string[] { "sginal_name", "starting_index", "signal", "sampling_rate", "features" },
                new Object[] { pathLabel.Text, _startingInSec, Garage.ObjectToByteArray(_truncatedSamples), _samplingRate, Garage.ObjectToByteArray(_featuresOrderedDictionary) }, "SignalHolder");
            Thread dbStimulatorThread = new Thread(new ThreadStart(dbStimulator.run));
            dbStimulatorThread.Start();

            // Update the notification badge for unfitted signals
            Control badge = MainFormFolder.BadgeControl.GetBadge(this.FindForm());
            badge.Text = (int.Parse(badge.Text) + 1).ToString();
            badge.Visible = true;
        }

        private void sendSignalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EventHandlers.sendSignalTool(_truncatedSamples, _samplingRate, _quantizationStep, pathLabel.Text + "\\Collector");
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
        }

        private void signalExhibitor_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }

        private void signalExhibitor_MouseWheel(object sender, MouseEventArgs e)
        {
            EventHandlers.signalExhibitor_MouseWheel(sender, e, _previousMouseX, _previousMouseY);
        }

        private void signalExhibitor_MouseEnter(object sender, EventArgs e)
        {
            ((MainForm)FindForm()).scrollAllowed = false;
        }

        private void signalExhibitor_MouseLeave(object sender, EventArgs e)
        {
            ((MainForm)FindForm()).scrollAllowed = true;
        }

        //*******************************************************************************************************//
        //*******************************************CLASS FUNCTIONS*********************************************//
        /// <summary> 
        /// Loads the selected signal in signalExhibitor from _samples, starting from an included param in secs.
        /// The signal is loaded for 15 secs as max.
        /// </summary>
        public void loadSignalStartingFrom(double startingInSecs)
        {
            // Check if the signal is more than 15 secs
            if ((_samples.Length / _samplingRate) > 10)
            {
                int truncPeriod = 10;
                // If yes then plot only 15 secs of the signal
                // Check if the inserted starting param is negative number
                if (startingInSecs < 0D)
                    // If yes then set the starting param as 0
                    startingInSecs = 0D;

                // starting from the inserted param as in secs
                int starting = (int) (startingInSecs * _samplingRate);
                int ending = (int) (starting + (truncPeriod * _samplingRate));
                // Check if _samples contains enogh samples for 15 sec starting from the starting index
                if (ending > _samples.Length)
                {
                    // If yes then set the ending as the length of _samples
                    ending = _samples.Length;
                    // and the starting as 15 secs from the ending
                    starting = (int)(ending - (truncPeriod * _samplingRate));
                }

                // Set the new startingInSec
                _startingInSec = starting / _samplingRate;

                int numSamples = (int)(truncPeriod * _samplingRate);
                _truncatedSamples = new Double[numSamples];

                for (int i = 0; i < numSamples; i++)
                    _truncatedSamples[i] = _samples[i + starting];
            }
            else
            {
                // If yes then copy _samples in _truncatedSamples
                _truncatedSamples = new Double[_samples.Length];

                for (int i = 0; i < _samples.Length; i++)
                    _truncatedSamples[i] = _samples[i];
            }

            // Insert signal values inside signal holder chart
            Garage.loadSignalInChart((Chart)Controls.Find("signalExhibitor", false)[0], _truncatedSamples, _samplingRate, _quantizationStep, _startingInSec, "SignalHolderSignal");
            _filteredSamples = new double[_truncatedSamples.Length];
            for (int i = 0; i < _truncatedSamples.Length; i++)
                _filteredSamples[i] = _truncatedSamples[i];
        }
    }
}

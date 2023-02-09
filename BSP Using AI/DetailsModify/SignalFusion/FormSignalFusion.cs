using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace BSP_Using_AI.DetailsModify.SignalFusion
{
    public partial class FormSignalFusion : Form
    {
        FilteringTools _FilteringTools;

        double _offset;

        List<int[]> _qrsPeaks;
        List<int[]> _synchronizedQRSPeaks;
        List<double[]> _synchronizedSamples;
        double[] _synchronizedSamplesOffsets;
        List<double[]> _orthogonalizedSignals;

        int _periodSampling;

        bool _mouseDown = false;
        int _previousMouseX;
        int _previousMouseY;
        public FormSignalFusion(FilteringTools filteringTools, double magorFrequency, string path)
        {
            InitializeComponent();

            _FilteringTools = filteringTools;
            pathLabel.Text = path + "\\Fusion";
            // Get a period samples number
            _periodSampling = (int)(_FilteringTools._samplingRate / magorFrequency);
            // Set period duration in
            periodDurationTextBox.Text = Math.Round(_periodSampling / (double)_FilteringTools._samplingRate, 2).ToString();

            setPeriods();
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        private void setPeriods()
        {
            // Get how many periods we can make from samples using periodSampling
            int periodsNumber = _FilteringTools._FilteredSamples.Length / _periodSampling;

            // Insert periods numbers in periodPickerComboBox
            periodPickerComboBox.DataSource = null;
            List<String> periods = new List<string>(periodsNumber);
            for (int i = 1; i <= periodsNumber; i++)
                periods.Add("period " + i.ToString());
            periodPickerComboBox.DataSource = periods;

            if (periods.Count == 0)
                return;

            _offset = 0D;

            // Set the offset scrollbar
            offsetScrollBar.Maximum = 2 * _periodSampling + offsetScrollBar.LargeChange - offsetScrollBar.SmallChange;
            offsetScrollBar.Value = _periodSampling;
            // And offset value
            offsetLabel.Text = "Offset: 0 secs";

            loadPeriod();
        }
        private void loadPeriod()
        {
            // Show the selected period in periodsChart
            double[] periodSamples;
            double offset = 0D;
            // Check if periods are centralized
            if (centralizeCheckBox.Checked)
            {
                if (periodPickerComboBox.SelectedIndex < 0)
                    return;
                periodSamples = _synchronizedSamples[periodPickerComboBox.SelectedIndex];
                offset = _synchronizedSamplesOffsets[periodPickerComboBox.SelectedIndex] / _FilteringTools._samplingRate;
            }
            else
            {
                // If yes then load period from _samples directly
                periodSamples = getPeriodFromIndexAsDouble(periodPickerComboBox.SelectedIndex, (int)_offset, _periodSampling);
                offset = (_offset + periodPickerComboBox.SelectedIndex * _periodSampling) / _FilteringTools._samplingRate;
            }

            // Load period inside periodsChart
            Garage.loadSignalInChart(periodsChart, periodSamples, _FilteringTools._samplingRate, offset, "FormSignalFusion");
        }

        private double[] getPeriodFromIndexAsDouble(int index, int offset, int periodSampling)
        {
            double[] periodSamples = new double[periodSampling];

            for (int i = 0; i < periodSampling; i++)
            {
                // Check if offset is moving the negative values or is crossing the end of the signal
                int sampleIndex = i + offset + (index * periodSampling);
                if (sampleIndex >= 0 && sampleIndex < _FilteringTools._FilteredSamples.Length)
                    // If yes then add the sample from _samples
                    periodSamples[i] = _FilteringTools._FilteredSamples[sampleIndex];
            }
            return periodSamples;
        }
        private float[] getPeriodFromIndexAsFloat(int index, int offset, int periodSampling)
        {
            float[] periodSamples = new float[periodSampling];

            for (int i = 0; i < periodSampling; i++)
            {
                // Check if offset is moving the negative values or is crossing the end of the signal
                int sampleIndex = i + offset + (index * periodSampling);
                if (sampleIndex >= 0 && sampleIndex < _FilteringTools._FilteredSamples.Length)
                    // If yes then add the sample from _samples
                    periodSamples[i] = (float)_FilteringTools._FilteredSamples[sampleIndex];
            }
            return periodSamples;
        }

        private void additionMultiplicationFusion(bool addition, bool multiplication)
        {
            double[] additionSamples = new double[_periodSampling];

            // Iterate through each period and add it in additionSamples as an average
            int countedPeriods = periodPickerComboBox.Items.Count;
            for (int i = 0; i < countedPeriods; i++)
            {
                for (int j = 0; j < _periodSampling; j++)
                {
                    // Check fi should operate on synchronized signal or original signal
                    if (centralizeCheckBox.Checked)
                    {
                        // If yes then operate of synchronized periods
                        if (addition)
                            additionSamples[j] += _synchronizedSamples[i][j];
                        if (multiplication)
                            additionSamples[j] *= 1 + _synchronizedSamples[i][j];
                    }
                    else
                    {
                        // Check if offset is moving the negative values or is crossing the end of the signal
                        int sampleIndex = j + (int)_offset + (i * _periodSampling);
                        if (sampleIndex >= 0 && sampleIndex < _FilteringTools._FilteredSamples.Length)
                        {
                            if (addition)
                                // If yes then add the sample from _samples
                                additionSamples[j] += _FilteringTools._FilteredSamples[sampleIndex] / countedPeriods;
                            if (multiplication)
                                // If yes then multiply by the sample from _samples
                                additionSamples[j] *= 1 + _FilteringTools._FilteredSamples[sampleIndex] / countedPeriods;
                        }
                    }
                }
            }

            // Load the fused signal in fusionChart
            Garage.loadSignalInChart(fusionChart, additionSamples, _FilteringTools._samplingRate, _offset / _FilteringTools._samplingRate, "FormSignalFusion");
        }

        private void crossCorrelationFusion()
        {
            double[] intercorrelation;
            double[] intercorrelationAverage = new double[_periodSampling * 2 - 1];
            double[] sign1 = new double[_periodSampling];
            double[] sign2 = new double[_periodSampling];

            // Iterate through each signal except the last one
            // and intercorrelate it with the signals that comes after it
            int countedIntercorrelation = Garage.factorial(periodPickerComboBox.Items.Count - 1);
            for (int i = 0; i < periodPickerComboBox.Items.Count - 1; i++)
            {
                // Get the first signal
                if (centralizeCheckBox.Checked)
                    for (int j = 0; j < _periodSampling; j++)
                        sign1[j] = _synchronizedSamples[i][j];
                else
                    sign1 = getPeriodFromIndexAsDouble(i, (int)_offset, _periodSampling);

                // Iterate through the periods that comes after the selected period in sign1
                for (int k = i + 1; k < periodPickerComboBox.Items.Count; k++)
                {
                    // Get the second signal
                    if (centralizeCheckBox.Checked)
                        for (int j = 0; j < _periodSampling; j++)
                            sign2[j] = _synchronizedSamples[k][j];
                    else
                        sign2 = getPeriodFromIndexAsDouble(k, (int)_offset, _periodSampling);

                    // Calculate the intercorrelation
                    intercorrelation = Garage.crossCorrelation(sign1, sign2);

                    // Add the result in intercorrelationAverage
                    for (int j = 0; j < intercorrelationAverage.Length; j++)
                        intercorrelationAverage[j] += intercorrelation[j] / countedIntercorrelation;
                }
            }

            // Load the fused signal in fusionChart
            Garage.loadSignalInChart(fusionChart, intercorrelationAverage, _FilteringTools._samplingRate, -_periodSampling / _FilteringTools._samplingRate, "FormSignalFusion");
        }

        private void orthogonalizationFusion()
        {
            // Create the list of original signals (periods)
            List<double[]> signals = new List<double[]>(periodPickerComboBox.Items.Count);
            double[] signal;
            for (int i = 0; i < periodPickerComboBox.Items.Count; i++)
            {
                // Create periods and add them in signals
                signal = getPeriodFromIndexAsDouble(i, (int)_offset, _periodSampling);
                signals.Add(signal);
            }

            // Create the orthogonalized signals
            // Check if periods are centralized
            if (centralizeCheckBox.Checked)
                _orthogonalizedSignals = Garage.orthogonalization(_synchronizedSamples);
            else
                _orthogonalizedSignals = Garage.orthogonalization(signals);

            // Insert orthogonalized signals names in orthogonalSignalsComboBox
            orthogonalSignalsComboBox.DataSource = null;
            List<String> psis = new List<string>(_orthogonalizedSignals.Count);
            for (int i = 1; i <= _orthogonalizedSignals.Count; i++)
                psis.Add("Psi " + i.ToString());
            orthogonalSignalsComboBox.DataSource = psis;
        }

        private void centralizePeriods()
        {
            // Scan all peaks and stable states
            /// Returns the states of each move in the signal
            /// as object [] {"state", its index}
            /// where the state could be up, down, or stable
            List<State> states = Garage.scanPeaks(_FilteringTools._FilteredSamples, 0.02, 1, Double.NaN, _FilteringTools._samplingRate, false)[SANamings.AllPeaks];

            // Scan for QRS peaks
            /// Returns QRS indexes as int[] {Q index, R index, S index}
            /// where the energy of the QRS should be at least 60% higher than interval
            _qrsPeaks = Garage.scanQRS(_FilteringTools._FilteredSamples, states);
            _synchronizedQRSPeaks = new List<int[]>(_qrsPeaks.Count);

            // Insert centralized periods from _samples in _synchronizedSamples according to qrsPeaks R indexes
            _synchronizedSamples = new List<double[]>(_qrsPeaks.Count);
            _synchronizedSamplesOffsets = new double[_qrsPeaks.Count];
            double[] period;
            int originalSignalOffset;
            int periodOffset;
            int periodStartingIndex = 0;
            int periodSampling = _periodSampling;
            for (int i = 0; i < _qrsPeaks.Count; i++)
            {
                // Calculate periodSampling according to (current qrs peak, next qrs peak, and periodStartingIndex)
                if (i + 1 < _qrsPeaks.Count)
                    periodSampling = (_qrsPeaks[i + 1][1] + _qrsPeaks[i][1]) / 2 - periodStartingIndex;
                else
                    periodSampling = _FilteringTools._FilteredSamples.Length - periodStartingIndex;

                if (periodSampling > _periodSampling)
                    periodSampling = _periodSampling;
                // Insert the new centralized period
                period = new double[_periodSampling];

                // Calculate the new offset of the original signal of the period
                originalSignalOffset = _qrsPeaks[i][1] - periodSampling / 2;
                periodOffset = (_periodSampling - periodSampling) / 2;
                _synchronizedSamplesOffsets[i] = originalSignalOffset + periodOffset;

                int sampleIndex;
                for (int j = 0; j < periodSampling; j++)
                {
                    // Check if offset is moving the negative values or is crossing the end of the signal
                    sampleIndex = j + originalSignalOffset;
                    if (sampleIndex >= 0 && sampleIndex < _FilteringTools._FilteredSamples.Length)
                        // If yes then add the sample from _samples
                        period[j + periodOffset] = _FilteringTools._FilteredSamples[sampleIndex];
                }

                // Add the new centralized period in _synchronizedSamples
                _synchronizedSamples.Add(period);

                // Set the new starting index
                periodStartingIndex += periodSampling;
            }
        }

        private void synchronizePeriods()
        {
            // Synchronize all qrs peaks to as the duration between Q peak and S peak should be 0.07 secs

            // Set the number of samples of 0.07 sec according to samplingRate
            int syncSamples = (int)(0.07D * _FilteringTools._samplingRate);

            // Iterate through every beat in _synchronizedSamples
            double[] newSynchronizedSamples;
            double[] oldTrunc;
            double[] newTrunc;
            int syncTruncHalfIndex = (syncSamples + 1) / 2;
            int trunc1stHalfIndex;
            int trunc2ndHalfIndex;
            int qrsDuration;
            int difference;
            int processesNum;
            int startingOffset;
            int copyIndex = 0;
            bool copyLastValue;
            for (int i = 0; i < _synchronizedSamples.Count; i++)
            {
                // Get QRS duration of the selected beat
                qrsDuration = _qrsPeaks[i][2] - _qrsPeaks[i][0];
                difference = syncSamples - qrsDuration;

                // Check if the difference is odd and (qrsDuration + 1) is even
                copyLastValue = false;
                if (difference % 2 == 1)
                    // If yes then we should copy the last value of each newTrunc as the value that comes before it
                    copyLastValue = true;

                trunc1stHalfIndex = (qrsDuration + 1) / 2;
                trunc2ndHalfIndex = trunc1stHalfIndex - (qrsDuration + 1) % 2;

                // Calculate the number of processes
                processesNum = _synchronizedSamples[i].Length / (qrsDuration + 1);

                // Calculate the starting offset
                int sPeakIndex = (_periodSampling / 2 - 1) + (_qrsPeaks[i][2] - _qrsPeaks[i][1]);
                startingOffset = sPeakIndex - ((processesNum / 2) * (qrsDuration + 1)) + 1;

                // Create new synchronized samples
                newSynchronizedSamples = new double[(processesNum + 1) * (syncSamples + 1)];
                // Check if selected qrsDuration is less than syncSamples
                if (0 < difference)
                {
                    // If yes then beat should be expanded leaving the R peak in the center

                    // Iterate through each trunc
                    for (int j = 0; j < processesNum; j++)
                    {

                        // Copy the original trunc in oldTrunc while keeping the center peaks in the center
                        oldTrunc = new double[syncSamples + 1];
                        newTrunc = new double[syncSamples + 1];
                        for (int k = 0; k < qrsDuration + 1; k++)
                            if ((startingOffset + (qrsDuration + 1) * j + k) > _synchronizedSamples[i].Length - 1)
                                break;
                            else
                                oldTrunc[difference / 2 + k] = _synchronizedSamples[i][startingOffset + (qrsDuration + 1) * j + k];

                        // Start expanding from the center sample
                        newTrunc[syncTruncHalfIndex] = oldTrunc[syncTruncHalfIndex];
                        // while giving the first half more expansion than the second half
                        for (int k = 1; k < difference - (difference / 2) + 1; k++)
                        {
                            // Expand the first half (lower half)
                            copyIndex = syncTruncHalfIndex - (2 * k) + (trunc1stHalfIndex * (k / trunc1stHalfIndex));
                            newTrunc[copyIndex + 1] = (oldTrunc[syncTruncHalfIndex - k] + oldTrunc[syncTruncHalfIndex - k + 1]) / 2;
                            newTrunc[copyIndex] = oldTrunc[syncTruncHalfIndex - k];

                            // Check if difference has crossed the center of (qrsDuration + 1)
                            if ((k % trunc1stHalfIndex) == 0)
                                // If yes then copy newTrunc in oldTrunc
                                for (int l = 0; l < newTrunc.Length; l++)
                                    oldTrunc[l] = newTrunc[l];
                        }
                        // Copy the rest of oldTrunc in newTrunc
                        for (int k = copyIndex - 1; k > -1; k--)
                            newTrunc[k] = oldTrunc[k + (copyIndex - syncTruncHalfIndex) / 2 + syncTruncHalfIndex - copyIndex];

                        // Expand the second half (uper half)
                        for (int k = 1; k < difference / 2 + 1; k++)
                        {
                            // Expand the second half
                            copyIndex = syncTruncHalfIndex + (2 * k) - (trunc2ndHalfIndex * (k / trunc2ndHalfIndex));
                            newTrunc[copyIndex - 1] = (oldTrunc[syncTruncHalfIndex + k] + oldTrunc[syncTruncHalfIndex + k - 1]) / 2;
                            newTrunc[copyIndex] = oldTrunc[syncTruncHalfIndex + k];

                            // Check if difference has crossed the center of (qrsDuration + 1)
                            if ((k % trunc2ndHalfIndex) == 0)
                                // If yes then copy newTrunc in oldTrunc
                                for (int l = 0; l < newTrunc.Length; l++)
                                    oldTrunc[l] = newTrunc[l];
                        }
                        // Copy the rest of oldTrunc in newTrunc
                        for (int k = copyIndex + 1; k < oldTrunc.Length; k++)
                        {
                            int index = k + (copyIndex - syncTruncHalfIndex) / 2 + syncTruncHalfIndex - copyIndex;
                            if (index < oldTrunc.Length)
                                newTrunc[k] = oldTrunc[index];
                        }
                        // Check if we should copy the last value
                        if (copyLastValue)
                            // If yes then copy the last value as the value that comes before it
                            newTrunc[newTrunc.Length - 1] = newTrunc[newTrunc.Length - 2];

                        // Add the new trunc in newSynchronizedSamples
                        for (int k = 0; k < newTrunc.Length; k++)
                            newSynchronizedSamples[(syncSamples + 1) * j + k] = newTrunc[k];
                    }
                }
                // Check if it is greater
                else if (0 > difference)
                {
                    // If yes then beat should be shrinked leaving the R peak in the center

                    // Iterate through each trunc
                    for (int j = 0; j < processesNum; j++)
                    {

                        // Copy the original trunc in oldTrunc while keeping the center peaks in the center
                        oldTrunc = new double[qrsDuration + 1];
                        newTrunc = new double[syncSamples + 1];
                        for (int k = 0; k < qrsDuration + 1; k++)
                            if ((startingOffset + (qrsDuration + 1) * j + k) > _synchronizedSamples[i].Length - 1)
                                break;
                            else
                                oldTrunc[k] = _synchronizedSamples[i][startingOffset + (qrsDuration + 1) * j + k];

                        // Start shrinking from the center sample
                        newTrunc[syncTruncHalfIndex] = oldTrunc[trunc1stHalfIndex];
                        // while giving the first half more shrink than the second half
                        for (int k = 1; k < Math.Abs(difference) - (Math.Abs(difference) / 2) + 1; k++)
                        {
                            // shrink the first half (lower half)
                            copyIndex = syncTruncHalfIndex - k;
                            if (copyIndex > -1)
                                newTrunc[copyIndex] = oldTrunc[trunc1stHalfIndex - 2 * k];
                        }
                        // Copy the rest of oldTrunc in newTrunc
                        for (int k = copyIndex - 1; k > -1; k--)
                            newTrunc[k] = oldTrunc[k + (copyIndex - syncTruncHalfIndex) * 2 + trunc1stHalfIndex - copyIndex];

                        // shrink the second half (uper half)
                        for (int k = 1; k < Math.Abs(difference) / 2 + 1; k++)
                        {
                            // shrink the second half
                            copyIndex = syncTruncHalfIndex + k;
                            if (copyIndex < newTrunc.Length && (trunc1stHalfIndex + 2 * k) < oldTrunc.Length)
                                newTrunc[copyIndex] = oldTrunc[trunc1stHalfIndex + 2 * k];
                        }
                        // Copy the rest of oldTrunc in newTrunc
                        for (int k = copyIndex + 1; k < newTrunc.Length; k++)
                            newTrunc[k] = oldTrunc[k + (copyIndex - syncTruncHalfIndex) * 2 + trunc1stHalfIndex - copyIndex];

                        // Add the new trunc in newSynchronizedSamples
                        for (int k = 0; k < newTrunc.Length; k++)
                            newSynchronizedSamples[(syncSamples + 1) * j + k] = newTrunc[k];
                    }
                }

                // Set the synchronized signal in _synchronizedSamples
                // while taking care of centralizing the R peak
                int indexOfNew = 0;
                int indexOfCopy = 0;
                int qrDuration = _qrsPeaks[i][1] - _qrsPeaks[i][0];
                int rOffset;
                int qPeakIndex = (_periodSampling / 2 - 1) - qrDuration;
                int processesBeforeQPeak = (qPeakIndex - startingOffset) / (qrsDuration + 1);
                if (qrDuration > trunc1stHalfIndex)
                {
                    rOffset = qrDuration - trunc1stHalfIndex;
                    if (rOffset == (Math.Abs(difference) / 2))
                        rOffset -= 2;
                    else if (rOffset > (Math.Abs(difference) / 2))
                        rOffset = (Math.Abs(difference) / 2);
                    // Set starting index of expanding process
                    if (0 < difference)
                        indexOfCopy = (processesBeforeQPeak * difference) - startingOffset + (difference - (difference / 2)) + rOffset;
                    // Set starting index of shrinking process
                    else if (0 > difference)
                        indexOfNew = (processesBeforeQPeak * Math.Abs(difference)) + startingOffset + (Math.Abs(difference) - (Math.Abs(difference) / 2)) + rOffset;
                }
                else
                {
                    rOffset = trunc1stHalfIndex - qrDuration;
                    rOffset = (Math.Abs(difference) - (Math.Abs(difference) / 2) + 1) - rOffset;
                    if (rOffset < 0)
                        rOffset = 0;
                    // Set starting index of expanding process
                    if (0 < difference)
                        indexOfCopy = (processesBeforeQPeak * difference) - startingOffset + rOffset;
                    // Set starting index of shrinking process
                    else if (0 > difference)
                        indexOfNew = (processesBeforeQPeak * Math.Abs(difference)) + startingOffset + rOffset;
                }

                for (int j = 0; j < _synchronizedSamples[i].Length; j++)
                {
                    // Condition for shrinking: (j + indexOfNew) < _synchronizedSamples[i].Length && j < newSynchronizedSamples.Length
                    // Condition for expanding: indexOfCopy > -1
                    if ((j + indexOfNew) < _synchronizedSamples[i].Length && j < newSynchronizedSamples.Length && indexOfCopy > -1)
                        _synchronizedSamples[i][j + indexOfNew] = newSynchronizedSamples[j + indexOfCopy];
                }
            }
        }

        //*******************************************************************************************************//
        //********************************************EVENT HANDLERS*********************************************//
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

        private void periodDurationTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && !(e.KeyChar == '.'))
            {
                e.Handled = true;
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            // Set the new _periodSampling if periodDurationTextBox had a valid number
            if (!(periodDurationTextBox.Text.Equals("") || periodDurationTextBox.Text.Equals(".")))
                _periodSampling = (int)(double.Parse(periodDurationTextBox.Text) * _FilteringTools._samplingRate);

            // Check if centration is checked
            if (centralizeCheckBox.Checked)
            {
                // If yes then centralize periods according to the new _periodSampling
            }
            else
            {
                // If yes then reload the selected period
                setPeriods();
            }
        }

        private void periodPickerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadPeriod();
        }

        private void offsetScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            // Set the new offset
            _offset = 0D + (offsetScrollBar.Value - _periodSampling);

            offsetLabel.Text = "Offset: " + Math.Round(_offset / _FilteringTools._samplingRate, 2).ToString() + " secs";

            loadPeriod();
        }

        private void AdditionRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // If it is checked
            if (AdditionRadioButton.Checked)
                // If yes then load the fused signal
                additionMultiplicationFusion(true, false);

            // Disable orthogonalSignalsComboBox
            orthogonalSignalsComboBox.Enabled = false;
            // Disable fuseOrthogonalizationButton
            fuseOrthogonalizationButton.Enabled = false;
        }

        private void multiplicationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // If it is checked
            if (multiplicationRadioButton.Checked)
                // If yes then load the fused signal
                additionMultiplicationFusion(false, true);

            // Disable orthogonalSignalsComboBox
            orthogonalSignalsComboBox.Enabled = false;
            // Disable fuseOrthogonalizationButton
            fuseOrthogonalizationButton.Enabled = false;
        }

        private void crossCorrelationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // If it is checked
            if (crossCorrelationRadioButton.Checked)
                crossCorrelationFusion();

            // Disable orthogonalSignalsComboBox
            orthogonalSignalsComboBox.Enabled = false;
            // Disable fuseOrthogonalizationButton
            fuseOrthogonalizationButton.Enabled = false;
        }

        private void OrthogonalisationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // If it is checked
            if (OrthogonalisationRadioButton.Checked)
                orthogonalizationFusion();

            // Enable orthogonalSignalsComboBox
            orthogonalSignalsComboBox.Enabled = true;
            // Enable fuseOrthogonalizationButton
            fuseOrthogonalizationButton.Enabled = true;
        }

        private void orthogonalSignalsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Load the fused signal in fusionChart
            if (orthogonalSignalsComboBox.SelectedIndex >= 0)
                Garage.loadSignalInChart(fusionChart, _orthogonalizedSignals[orthogonalSignalsComboBox.SelectedIndex], _FilteringTools._samplingRate, 0D, "FormSignalFusion");
        }

        private void fuseOrthogonalizationButton_Click(object sender, EventArgs e)
        {
            double[] additionSamples = new double[_periodSampling];

            // Iterate through each orthogonalized signal and add it in additionSamples as an average
            int countedOrthogonalizedSigs = orthogonalSignalsComboBox.Items.Count;
            for (int i = 0; i < countedOrthogonalizedSigs; i++)
                for (int j = 0; j < _periodSampling; j++)
                    additionSamples[j] += _orthogonalizedSignals[i][j] / countedOrthogonalizedSigs;

            // Load the fused signal in fusionChart
            Garage.loadSignalInChart(fusionChart, additionSamples, _FilteringTools._samplingRate, _offset / _FilteringTools._samplingRate, "FormSignalFusion");
        }

        private void centralizeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            int periodsNumber;

            // Check if it is checked or unchecked
            if (centralizeCheckBox.Checked)
            {
                // If yes then disable offsetScrollBar and set the offset to nothing
                offsetScrollBar.Enabled = false;
                offsetLabel.Text = "Offset: - secs";
                // Enable synchronizePeriodsCheckBox
                synchronizePeriodsCheckBox.Enabled = true;

                centralizePeriods();

                // Set periods number
                periodsNumber = _synchronizedSamples.Count;
            }
            else
            {
                // If yes then enable offsetScrollBar and set the offset
                offsetScrollBar.Enabled = true;
                offsetLabel.Text = "Offset: " + Math.Round(_offset / _FilteringTools._samplingRate, 2).ToString() + " secs";
                // Uncheck and disable synchronizePeriodsCheckBox
                synchronizePeriodsCheckBox.Checked = false;
                synchronizePeriodsCheckBox.Enabled = false;

                // Calculate periods number
                periodsNumber = _FilteringTools._FilteredSamples.Length / _periodSampling;

                loadPeriod();
            }

            // Insert periods numbers in periodPickerComboBox
            periodPickerComboBox.DataSource = null;
            List<String> periods = new List<string>(periodsNumber);
            for (int i = 1; i <= periodsNumber; i++)
                periods.Add("period " + i.ToString());
            periodPickerComboBox.DataSource = periods;
        }

        private void synchronizePeriodsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Check if it is checked or unchecked
            if (synchronizePeriodsCheckBox.Checked)
            {
                // If yes then synchronize periods
                synchronizePeriods();
            }
            else
            {
                // If yes then centralize periods
                centralizePeriods();
            }

            loadPeriod();
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
            // Clone _FilteringTools
            FilteringTools filteringTools = _FilteringTools.Clone();
            filteringTools.SetOriginalSamples(samples);

            EventHandlers.sendSignalTool(filteringTools, pathLabel.Text + "\\Collector");
        }

        private void analyseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Chart senderChart = (Chart)((sender as ToolStripItem).Owner as ContextMenuStrip).SourceControl;
            if (senderChart.Series[0].Points.Count < 1)
                return;

            // Clone _FilteringTools
            FilteringTools filteringTools = _FilteringTools.Clone();
            // Remove filters
            filteringTools.RemoveAllFilters();
            // Get samples from signal chart
            double[] samples = new double[senderChart.Series[0].Points.Count];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = senderChart.Series[0].Points[i].YValues[0] * _FilteringTools._quantizationStep;
            filteringTools.SetOriginalSamples(samples);

            EventHandlers.analyseSignalTool(filteringTools, pathLabel.Text + "\\Analyser");
        }
    }
}

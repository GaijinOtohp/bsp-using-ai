using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using BSP_Using_AI.DetailsModify;

namespace BSP_Using_AI
{
    class Garage
    {
        //*******************************************************************************************************//
        //*************************************INSERT SIGNAL VECTOR IN CHART*************************************//
        public static void loadXYInChart(Chart chart, double[] xValues, double[] yValues, string[] labels, double startingIndex, int selectedSeries, String reference)
        {
            Series series = chart.Series[selectedSeries];

            series.Points.Clear();

            // Insert the signal in the chart
            int indx = 0;
            for (int i = 0; i < xValues.Length; i++)
            {
                indx = series.Points.AddXY(startingIndex + xValues[i], yValues[i]);
                if (labels != null)
                    series.Points[indx].Label = labels[i];
            }
                
            chart.Series[selectedSeries] = series;
        }

        public static void loadSignalInChart(Chart chart, double[] samples, double samplingRate, double startingIndex, String reference)
        {
            Series series = chart.Series[0];

            series.Points.Clear();

            // Insert the signal in the chart
            for (int i = 0; i < samples.Length; i++)
                series.Points.AddXY(startingIndex + (i / samplingRate), samples[i]);
            chart.Series[0] = series;
        }

        //*******************************************************************************************************//
        //****************************************ABSOLUTE VALUES SIGNAL*****************************************//
        public static double[] absoluteSignal(double[] signal)
        {
            double[] samples = new double[signal.Length];
            for (int i = 0; i < signal.Length; i++)
                samples[i] = Math.Abs(signal[i]);

            return samples;
        }

        //*******************************************************************************************************//
        //***************************************NORMALIZED VALUES SIGNAL****************************************//
        public static double[] normalizeSignal(double[] signal)
        {
            // Calculate the normalizationCoef of the selected signal
            double[] samples = new double[signal.Length];
            double normalizationCoef = 0D;
            for (int i = 0; i < signal.Length; i++)
                normalizationCoef += signal[i] * signal[i];
            normalizationCoef = Math.Sqrt(normalizationCoef);

            for (int i = 0; i < signal.Length; i++)
                samples[i] = signal[i] / normalizationCoef;

            return samples;
        }

        //*******************************************************************************************************//
        //******************************************PDF COEFS OF SIGNAL******************************************//
        public static OrderedDictionary statParams(double[] signal)
        {
            OrderedDictionary statParams = new OrderedDictionary();

            // Get mean, min, and max of the signal
            (double mean, double min, double max) = meanMinMax(signal);

            // Get standart deviation of the signal
            double stdDev = stdDevCalc(signal, mean);

            // Calculate inter quartile range (IQR)
            double IQR = signalIQR(signal);

            statParams.Add("mean", mean);
            statParams.Add("min", min);
            statParams.Add("max", max);
            statParams.Add("std_dev", stdDev);
            statParams.Add("iqr", IQR);
            return statParams;
        }

        public static (double mean, double min, double max) meanMinMax(double[] signal)
        {
            // Calculate mean, min, max of signal
            double mean = 0d;
            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;
            foreach (double sample in signal)
            {
                mean += sample / signal.Length;
                if (sample < min)
                    min = sample;
                if (sample > max)
                    max = sample;
            }

            return (mean, min, max);
        }

        public static double stdDevCalc(double[] signal, double mean)
        {
            // Calculate standard deviation
            double stdDev = 0d;
            foreach (double sample in signal)
                stdDev += Math.Pow(sample - mean, 2) / signal.Length;
            stdDev = Math.Sqrt(stdDev);

            return stdDev;
        }

        //*******************************************************************************************************//
        //*********************************************IQR OF SIGNAL*********************************************//
        private static double signalIQR(double[] signal)
        {
            // Sort the signal
            double[] samples = new double[signal.Length];
            for (int i = 0; i < signal.Length; i++)
                samples[i] = signal[i];

            Array.Sort(samples);

            int mid_indx = medianIndex(0, samples.Length);
            double Q1 = samples[medianIndex(0, mid_indx)];
            double Q3 = samples[medianIndex(mid_indx + 1, samples.Length)];

            return (Q3 - Q1);
        }

        private static int medianIndex(int left, int right)
        {
            int median = right - left + 1;
            median = (median + 1) / 2 - 1;
            return median + left;
        }

        //*******************************************************************************************************//
        //*********************************************FFT OF SIGNAL*********************************************//
        public static double[] calculateFFT(double[] signal)
        {
            // Create the signal of length of power of 2
            double[] signalOfPowerOf2 = createPowerOf2Signal(signal);

            // Create the vector of fft signal
            NWaves.Transforms.RealFft fft = new NWaves.Transforms.RealFft(signalOfPowerOf2.Length);

            // Create a copy of the signal in float items
            float[] signalInPowerOf2 = new float[signalOfPowerOf2.Length];
            for (int i = 0; i < signalOfPowerOf2.Length; i++)
                signalInPowerOf2[i] = (float)signalOfPowerOf2[i];

            // Calculate the fft
            float[] realOutputInPowerOf2 = new float[signalOfPowerOf2.Length / 2 + 1];
            fft.MagnitudeSpectrum(signalInPowerOf2, realOutputInPowerOf2, true);
            // Remove the power of the removed negative frequencies
            double[] magnitudeSpectrum = new double[signalOfPowerOf2.Length / 2 + 1];
            for (int i = 0; i < magnitudeSpectrum.Length; i++)
                magnitudeSpectrum[i] = (double)realOutputInPowerOf2[i] / 2;

            return magnitudeSpectrum;
        }

        //*******************************************************************************************************//
        //***********************************************DC REMOVAL**********************************************//
        public static double[] removeDCValue(double[] signal)
        {
            if (signal.Length == 0)
                return null;

            // Calculate the dc value
            double dcValue = signal[0];
            for (int i = 1; i < signal.Length; i++)
                dcValue = ((dcValue * i) + signal[i]) / (i + 1);

            // Remove the dc value from the singal samples
            for (int i = 0; i < signal.Length; i++)
                signal[i] -= dcValue;

            return signal;
        }

        //*******************************************************************************************************//
        //***********************************************IIR FILTERs*********************************************//
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="type">0: as low pass. 1: as high pass</param>
        /// <param name="order"></param>
        /// <param name="normalizedFreq">The cut off frequency in between 0 - 0.5</param>
        /// <returns></returns>
        public static double[] iirFilter(double[] signal, String filterName, int type, int order, double normalizedFreq, double samplingRate)
        {
            if (signal.Length == 0)
                return null;

            // Set the signal as float
            float[] floatSignal = new float[signal.Length];
            for (int i = 0; i < signal.Length; i++)
                floatSignal[i] = (float)signal[i];

            // Check the filter type
            object filter = null;
            NWaves.Signals.DiscreteSignal filtered = null;
            switch (filterName)
            {
                case "Butter Worth":
                    // This is butterworth filter
                    // Check which type is selected
                    if (type == 0)
                    {
                        // If yes then generate a low pass filter
                        filter = new NWaves.Filters.Butterworth.LowPassFilter(normalizedFreq, order);
                        filtered = ((NWaves.Filters.Butterworth.LowPassFilter)filter).ApplyTo(new NWaves.Signals.DiscreteSignal((int)samplingRate, floatSignal));
                    } else
                    {
                        // If yes then generate a high pass filter
                        filter = new NWaves.Filters.Butterworth.HighPassFilter(normalizedFreq, order);
                        filtered = ((NWaves.Filters.Butterworth.HighPassFilter)filter).ApplyTo(new NWaves.Signals.DiscreteSignal((int)samplingRate, floatSignal));
                    }
                    break;
                case "Chebyshev I":
                    // This is Chebyshev I filter
                    // Check which type is selected
                    if (type == 0)
                    {
                        // If yes then generate a low pass filter
                        filter = new NWaves.Filters.ChebyshevI.LowPassFilter(normalizedFreq, order);
                        filtered = ((NWaves.Filters.ChebyshevI.LowPassFilter)filter).ApplyTo(new NWaves.Signals.DiscreteSignal((int)samplingRate, floatSignal));
                    } else
                    {
                        // If yes then generate a high pass filter
                        filter = new NWaves.Filters.ChebyshevI.HighPassFilter(normalizedFreq, order);
                        filtered = ((NWaves.Filters.ChebyshevI.HighPassFilter)filter).ApplyTo(new NWaves.Signals.DiscreteSignal((int)samplingRate, floatSignal));
                    }
                    break;
                case "Chebyshev II":
                    // This is Chebyshev II filter
                    // Check which type is selected
                    if (type == 0)
                    {
                        // If yes then generate a low pass filter
                        filter = new NWaves.Filters.ChebyshevII.LowPassFilter(normalizedFreq, order);
                        filtered = ((NWaves.Filters.ChebyshevII.LowPassFilter)filter).ApplyTo(new NWaves.Signals.DiscreteSignal((int)samplingRate, floatSignal));
                    }
                    else
                    {
                        // If yes then generate a high pass filter
                        filter = new NWaves.Filters.ChebyshevII.HighPassFilter(normalizedFreq, order);
                        filtered = ((NWaves.Filters.ChebyshevII.HighPassFilter)filter).ApplyTo(new NWaves.Signals.DiscreteSignal((int)samplingRate, floatSignal));
                    }
                    break;
            }

            // Copy the filtered signal in signal as double
            for (int i = 0; i < filtered.Length; i++)
                signal[i] = (double)filtered.Samples[i];

            return signal;
        }

        //*******************************************************************************************************//
        //**********************************************DWT TRANSFORM********************************************//
        public static object[] calculateDWT(double[] signal, String waveletName, int maxLevel)
        {
            if (signal.Length == 0)
                return null;
            // Create the signal of length of power of 2
            double[] signalOfPowerOf2 = createPowerOf2Signal(signal);
            // Create a copy of the signal in float items
            float[] signalInPowerOf2 = new float[signalOfPowerOf2.Length];
            for (int i = 0; i < signalOfPowerOf2.Length; i++)
                signalInPowerOf2[i] = (float)signalOfPowerOf2[i];

            // Create the dwt transform
            NWaves.Transforms.Wavelets.Fwt fwt = new NWaves.Transforms.Wavelets.Fwt(signalOfPowerOf2.Length, new NWaves.Transforms.Wavelets.Wavelet(waveletName));

            // Set the maximum level can be reached
            int allLevels = fwt.MaxLevel(signalOfPowerOf2.Length);
            if (maxLevel != int.MaxValue)
                allLevels = maxLevel;

            // Calculate dwt and insert it in dwtOfSignal
            float[] dwtOfSignal = new float[signalOfPowerOf2.Length];
            fwt.Direct(signalInPowerOf2, dwtOfSignal, 1);

            // Sort dwt levels in dwtLevelsSamples from dwtOfSignal
            object[] dwtLevelsSamples = new object[allLevels];

            int levelSignalLength = signal.Length / 2;
            if (signal.Length % 2 == 1)
                levelSignalLength += 1;

            int levelAddedSamplesLen = (dwtOfSignal.Length - signal.Length) / 2;

            dwtLevelsSamples[0] = new double[levelSignalLength];
            for (int j = 0; j < levelSignalLength; j++)
                ((double[])dwtLevelsSamples[0])[j] = dwtOfSignal[levelSignalLength + levelAddedSamplesLen + j];

            signal = new double[levelSignalLength];
            for (int j = 0; j < levelSignalLength; j++)
                signal[j] = dwtOfSignal[j];

            try
            {
                for (int i = 1; i < allLevels; i++)
                {
                    // Create the signal of length of power of 2
                    signalOfPowerOf2 = createPowerOf2Signal(signal);
                    // Create a copy of the signal in float items
                    signalInPowerOf2 = new float[signalOfPowerOf2.Length];
                    for (int j = 0; j < signalOfPowerOf2.Length; j++)
                        signalInPowerOf2[j] = (float)signalOfPowerOf2[j];

                    // Calculate dwt and insert it in dwtOfSignal
                    dwtOfSignal = new float[signalOfPowerOf2.Length];
                    fwt.Direct(signalInPowerOf2, dwtOfSignal, 1);

                    levelSignalLength = signal.Length / 2;
                    if (signal.Length % 2 == 1)
                        levelSignalLength += 1;

                    levelAddedSamplesLen = (dwtOfSignal.Length - signal.Length) / 2;

                    dwtLevelsSamples[i] = new double[levelSignalLength];
                    for (int j = 0; j < levelSignalLength; j++)
                        ((double[])dwtLevelsSamples[i])[j] = dwtOfSignal[levelSignalLength + levelAddedSamplesLen + j];

                    signal = new double[levelSignalLength];
                    for (int j = 0; j < levelSignalLength; j++)
                        signal[j] = dwtOfSignal[j];
                }
            }
            catch (Exception e)
            {

            }
            return dwtLevelsSamples;
        }

        //*******************************************************************************************************//
        //*******************************************POWER OF 2 SIGNAL*******************************************//
        public static double[] createPowerOf2Signal(double[] signal)
        {
            // Get the length in power of 2 of the signal
            int signalLength = signal.Length;
            int lengthInPowerOf2 = 2;
            while (lengthInPowerOf2 < signalLength)
                lengthInPowerOf2 *= 2;

            // Create the new singal of power of 2
            double[] signalInPowerOf2 = new double[lengthInPowerOf2];

            // Add the signal inside the vector
            // and multiply by the number of added zeros to the origianl signal
            // to keep the energy of the signal the same
            int offset = lengthInPowerOf2 - signalLength;
            for (int i = 0; i < signalLength; i++)
                if (offset != 0)
                    signalInPowerOf2[i] = (signal[i] * lengthInPowerOf2 / signalLength);
                else
                    signalInPowerOf2[i] = signal[i];
            for (int i = signalLength; i < lengthInPowerOf2; i++)
                signalInPowerOf2[i] = 0F;

            // return the new signal
            return signalInPowerOf2;
        }

        //*******************************************************************************************************//
        //******************************************FACTORIAL OF NUMBER******************************************//
        public static int factorial(int n)
        {
            // single line to find factorial
            return (n == 1 || n == 0) ?
                    1 : n * factorial(n - 1);
        }

        //*******************************************************************************************************//
        //*************************************ORTHOGONALIZATION OF SIGNALS**************************************//
        public static List<double[]> orthogonalization(List<double[]> signals)
        {
            // Create the list of orthogonalized singals
            List<double[]> orthogonalizedSignals = new List<double[]>(signals.Count);

            if (signals.Count == 0)
                return orthogonalizedSignals;

            // Iterate through each signal of signals and orthogonalize it with what is in orthogonalizedSignals
            double[] newPsi;
            double dotProductCoef;
            double normalizationCoef;
            foreach (double[] signal in signals)
            {
                newPsi = new double[signal.Length];
                for (int i = 0; i < newPsi.Length; i++)
                    newPsi[i] = signal[i];
                // Iterate through each signals in orthogonalizedSignals
                // and remove the projection of selected signal on the orthogonalizedSignals from the seleced signal
                foreach (double[] Psi in orthogonalizedSignals)
                {
                    // Calculate the new dotProductCoef between current signal and psi
                    dotProductCoef = 0D;
                    for (int i = 0; i < newPsi.Length; i++)
                        dotProductCoef += signal[i] * Psi[i];

                    // Remove the projection from the signal
                    for (int i = 0; i < newPsi.Length; i++)
                        newPsi[i] -= dotProductCoef * Psi[i];
                }

                // Normalize the new vector
                normalizationCoef = 0D;
                foreach (double sample in newPsi)
                    normalizationCoef += sample * sample;
                normalizationCoef = Math.Sqrt(normalizationCoef);
                for (int i = 0; i < newPsi.Length; i++)
                    newPsi[i] = newPsi[i] / normalizationCoef;

                // Add new orthogonalized signal
                orthogonalizedSignals.Add(newPsi);
            }

            return orthogonalizedSignals;
        }

        //*******************************************************************************************************//
        //*********************************************SIGNAL INTERVAL*******************************************//
        public static double amplitudeInterval(double[] signal)
        {
            // Get y values interval
            double min = signal[0];
            double max = signal[0];
            foreach (double sample in signal)
                if (sample < min)
                    min = sample;
                else if (sample > max)
                    max = sample;

            return (max - min);
        }

        //*******************************************************************************************************//
        //**********************************************PEAKS SCANNER********************************************//
        /// <summary>
        /// Returns the states of each move in the signal
        /// as object [] {"state", its index}
        /// where the state could be up, down, or stable
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="interval">is the interval between max and min (max - min)</param>
        /// <param name="thresholdRatio">vertical resolution</param>
        /// <param name="horThreshold">horizontal resolution</param>
        /// <param name="accThresholdRatio">minimum angle rotation between last state and next one</param>
        /// <param name="accelerationEnabled">calculate acceleration if enabled</param>
        /// <returns></returns>
        public static List<object[]> scanPeaks(double[] samples, double interval, double thresholdRatio, int horThreshold, double tdtThresholdRatio, bool deviationtionEnabled)
        {
            List<object[]> states = new List<object[]>();

            bool resetEverything = false;
            double[] up = new double[3] { 0d, 0d, Double.NegativeInfinity }; // { amplitude_of_total_steps, highest_amplitude_reached_index, highest_amplitude_reached_value }
            double[] down = new double[3] { 0d, 0d, Double.PositiveInfinity }; // { amplitude_of_total_steps, lowest_amplitude_reached_index, lowest_amplitude_reached_value }

            int[] lastUpIndx = new int[2] { 0, 0 }; // { index_in_signal, index_in_states }
            int[] lastDownIndx = new int[2] { 0, 0 }; // { index_in_signal, index_in_states }
            int[] lastStableIndx = new int[2] { 0, 0 }; // { index_in_signal, index_in_states }

            double previousSample = samples[0];
            int lastStateIndx = 0;
            double tangentedeviation = 0d;
            double lastTan = 0d;
            double secondLastTan = 0d;
            double tolerance = 0d;
            for (int i = 1; i < samples.Length; i++)
            {
                // Check if this move is going up/down/or stable
                if (samples[i] > previousSample)
                {
                    // Check if current sample value is the highest recorded
                    if (samples[i] > up[2])
                    {
                        up[1] = i;
                        up[2] = samples[i];

                        // Check if last state is up and is not away more than horThreshold
                        if (lastUpIndx[0] > lastDownIndx[0] && lastUpIndx[0] > lastStableIndx[0] && i - lastUpIndx[0] < horThreshold)
                            // If yes then update up peak
                            addUpState(samples, ref states, ref up, ref lastUpIndx, lastDownIndx, lastStableIndx, ref resetEverything, false);
                    }

                    //up[0] += Math.Abs(samples[i] - previousSample);
                }
                else if (samples[i] < previousSample)
                {
                    // Check if current sample value is the lowest recorded
                    if (samples[i] < down[2])
                    {
                        down[1] = i;
                        down[2] = samples[i];

                        // Check if last state is up and is not away more than horThreshold
                        if (lastDownIndx[0] > lastUpIndx[0] && lastDownIndx[0] > lastStableIndx[0] && i - lastDownIndx[0] < horThreshold)
                            // If yes then update down peak
                            addDownState(samples, ref states, ref down, lastUpIndx, ref lastDownIndx, lastStableIndx, ref resetEverything, false);
                    }

                    //down[0] += Math.Abs(samples[i] - previousSample);
                }

                // Get last state's index
                if (states.Count > 0)
                    if (((string)states[states.Count - 1][0]).Equals("stable"))
                        lastStateIndx = (int)states[states.Count - 1][4];
                    else
                        lastStateIndx = (int)states[states.Count - 1][1];

                // Check if its absolute change of moving up is thresholdRatio % more than interval (max - min)
                //if ((up[0] - down[0]) / interval > thresholdRatio)
                if ((samples[i] - samples[lastStateIndx]) / interval > thresholdRatio)
                    // If yes then add new up peak
                    addUpState(samples, ref states, ref up, ref lastUpIndx, lastDownIndx, lastStableIndx, ref resetEverything, false);
                // Check if its absolute change of moving down is more than interval (max - min) with thresholdRatio%
                //else if ((down[0] - up[0]) / interval > thresholdRatio)
                else if (-(samples[i] - samples[lastStateIndx]) / interval > thresholdRatio)
                    // If yes then add new down peak
                    addDownState(samples, ref states, ref down, lastUpIndx, ref lastDownIndx, lastStableIndx, ref resetEverything, false);
                // Check if current index is away from the index of last state with more than horThreshold
                // and the absolure change between up and down is less than thresholdRatio
                //else if ((i - lastUpIndx[0] > horThreshold && i - lastDownIndx[0] > horThreshold && i - lastStableIndx[0] > horThreshold) && Math.Abs((up[0] - down[0]) / interval) < thresholdRatio)
                else if ((i - lastUpIndx[0] > horThreshold && i - lastDownIndx[0] > horThreshold && i > lastStableIndx[0]) && Math.Abs((samples[i] - samples[lastStateIndx]) / interval) < thresholdRatio)
                    // If yes then add new stable state
                    addStableState(ref states, lastUpIndx, lastDownIndx, ref lastStableIndx, i - 1, ref resetEverything, false);

                ///:::::::::::::::::::: FOT TANGENT DEVIATION TOLERANCE :::::::::::::::::::::///
                // Set tangent deviation angle if enabled
                if (states.Count > 1 && deviationtionEnabled)
                {
                    // Calculate this deviation in amplitude
                    tolerance = opposite(samples, states);

                    // Check if tolerance is higher than tdtThresholdRatio % of interval
                    if (tolerance / interval > tdtThresholdRatio)
                    {
                        // If yes then new tangent deviation reached threshold
                        // update last state's properties
                        states[states.Count - 1][4] = states[states.Count - 1][1];
                        states[states.Count - 1][1] = states[states.Count - 1][4];

                        // Add new state for the new accelerated state
                        if (states[states.Count - 1][0].Equals("up"))
                            addUpState(samples, ref states, ref up, ref lastUpIndx, lastDownIndx, lastStableIndx, ref resetEverything, true);
                        else if (states[states.Count - 1][0].Equals("down"))
                            addDownState(samples, ref states, ref down, lastUpIndx, ref lastDownIndx, lastStableIndx, ref resetEverything, true);
                        else if (states[states.Count - 1][0].Equals("stable"))
                            addStableState(ref states, lastUpIndx, lastDownIndx, ref lastStableIndx, i - 1, ref resetEverything, true);
                    }

                    ///:::::::::::::::::::: STATE ROTAION :::::::::::::::::::::///
                    // Update second last state's acceleration value
                    if (states.Count > 2)
                    {
                        lastTan = (samples[(int)states[states.Count - 1][1]] - samples[(int)states[states.Count - 2][1]]) / ((int)states[states.Count - 1][1] - (int)states[states.Count - 2][1]);
                        secondLastTan = (samples[(int)states[states.Count - 2][1]] - samples[(int)states[states.Count - 3][1]]) / ((int)states[states.Count - 2][1] - (int)states[states.Count - 3][1]);

                        //states[states.Count - 2][2] = lastTan / secondLastTan;
                        states[states.Count - 2][2] = (Math.Atan(lastTan) - Math.Atan(secondLastTan)) * 180 / Math.PI;
                    }
                }

                // Check if everything should be resetted
                if (resetEverything)
                {
                    //up[0] = 0d;
                    up[2] = double.NegativeInfinity;
                    //down[0] = 0d;
                    down[2] = double.PositiveInfinity;

                    resetEverything = false;
                }

                // Set the new previous sample
                previousSample = samples[i];

            }

            return states;
        }

        private static double opposite(double[] samples, List<object[]> states)
        {
            // Calculate tangent argument between last two states
            states[states.Count - 1][3] = (samples[(int)states[states.Count - 1][1]] - samples[(int)states[states.Count - 2][1]]) / ((int)states[states.Count - 1][1] - (int)states[states.Count - 2][1]);
            // Update mean_amplitude
            if (!double.IsNegativeInfinity((double)states[states.Count - 1][5]))
                states[states.Count - 1][5] = ((double)states[states.Count - 1][5] + samples[(int)states[states.Count - 1][1]]) / 2;
            else
                states[states.Count - 1][5] = (samples[(int)states[states.Count - 1][1]] + samples[(int)states[states.Count - 2][1]]) / 2;
            // Update mean_tangent
            states[states.Count - 1][6] = ((double)states[states.Count - 1][5] - samples[(int)states[states.Count - 2][1]]) / ((int)states[states.Count - 1][1] - (int)states[states.Count - 2][1]);

            // Calculate deviation between last tangent and mean tangent in rad
            double tangentedeviation = Math.Abs(Math.Atan((double)states[states.Count - 1][3]) - Math.Atan((double)states[states.Count - 1][6]));
            // Calculate this deviation in amplitude
            return Math.Sin(tangentedeviation) * Math.Sqrt(Math.Pow(samples[(int)states[states.Count - 1][1]] - samples[(int)states[states.Count - 2][1]], 2) + Math.Pow((int)states[states.Count - 1][1] - (int)states[states.Count - 2][1], 2));
        }

        private static void addUpState(double[] samples, ref List<object[]> states, ref double[] up, ref int[] lastUpIndx, int[] lastDownIndx, int[] lastStableIndx, ref bool resetEverything, bool justAdd)
        {
            if (states.Count > 0)
            {
                // Check if there is different state before this new up state
                if ((lastDownIndx[0] > lastUpIndx[0] && (int)up[1] > lastDownIndx[0]) || (lastStableIndx[0] > lastUpIndx[0] && (int)up[1] > lastStableIndx[0]) || justAdd)
                {
                    // If yes then just add the new down state
                    states.Add(new object[7] { "up", (int)up[1], 0d, double.NegativeInfinity, (int)up[1], double.NegativeInfinity, double.NegativeInfinity }); // { state, index_in_signal, acceleration, first_tangente, first_tangentes_indx, mean_amplitude, mean_tangent }
                    lastUpIndx[0] = (int)up[1];
                    lastUpIndx[1] = states.Count - 1;
                    // Reset everything
                    resetEverything = true;
                }
                // Check if last down state is greater than the current new one
                else if (samples[(int)up[1]] > samples[lastUpIndx[0]])
                {
                    // If yes then update last down state
                    states[lastUpIndx[1]][1] = (int)up[1];
                    lastUpIndx[0] = (int)up[1];
                    // Reset everything
                    resetEverything = true;
                }
            }
            else
            {
                states.Add(new object[7] { "up", (int)up[1], 0d, double.NegativeInfinity, (int)up[1], double.NegativeInfinity, double.NegativeInfinity }); // { state, index_in_signal, acceleration, first_tangente, first_tangentes_indx, mean_amplitude, mean_tangent }
                lastUpIndx[0] = (int)up[1];
                lastUpIndx[1] = states.Count - 1;
                // Reset everything
                resetEverything = true;
            }
        }

        private static void addDownState(double[] samples, ref List<object[]> states, ref double[] down, int[] lastUpIndx, ref int[] lastDownIndx, int[] lastStableIndx, ref bool resetEverything, bool justAdd)
        {
            if (states.Count > 0)
            {
                // Check if there is different state before this new down state
                if ((lastUpIndx[0] > lastDownIndx[0] && (int)down[1] > lastUpIndx[0]) || (lastStableIndx[0] > lastDownIndx[0] && (int)down[1] > lastStableIndx[0])|| justAdd)
                {
                    // If yes then just add the new down state
                    states.Add(new object[7] { "down", (int)down[1], 0d, double.NegativeInfinity, (int)down[1], double.NegativeInfinity, double.NegativeInfinity }); // { state, index_in_signal, acceleration, first_tangente, first_tangentes_indx, mean_amplitude, mean_tangent }
                    lastDownIndx[0] = (int)down[1];
                    lastDownIndx[1] = states.Count - 1;
                    // Reset everything
                    resetEverything = true;
                }
                // Check if last down state is greater than the current new one
                else if (samples[(int)down[1]] < samples[lastDownIndx[0]])
                {
                    // If yes then update last down state
                    states[lastDownIndx[1]][1] = (int)down[1];
                    lastDownIndx[0] = (int)down[1];
                    // Reset everything
                    resetEverything = true;
                }
            }
            else
            {
                states.Add(new object[7] { "down", (int)down[1], 0d, double.NegativeInfinity, (int)down[1], double.NegativeInfinity, double.NegativeInfinity }); // { state, index_in_signal, acceleration, first_tangente, first_tangentes_indx, mean_amplitude, mean_tangent }
                lastDownIndx[0] = (int)down[1];
                lastDownIndx[1] = states.Count - 1;
                // Reset everything
                resetEverything = true;
            }
        }

        private static void addStableState(ref List<object[]> states, int[] lastUpIndx, int[] lastDownIndx, ref int[] lastStableIndx, int i, ref bool resetEverything, bool justAdd)
        {
            if (states.Count > 0)
            {
                // Check if there is different state before this new down state
                if ((lastDownIndx[0] > lastStableIndx[0] && i > lastDownIndx[0]) || (lastUpIndx[0] > lastStableIndx[0] && i > lastUpIndx[0]) || justAdd)
                {
                    // If yes then just add the new down state
                    states.Add(new object[7] { "stable", i, 0d, double.NegativeInfinity, i, double.NegativeInfinity, double.NegativeInfinity }); // { state, index_in_signal, acceleration, first_tangente, first_tangentes_indx, mean_amplitude, mean_tangent }
                    lastStableIndx[0] = i;
                    lastStableIndx[1] = states.Count - 1;
                }
                else
                {
                    // If yes then update last stable state
                    states[lastStableIndx[1]][1] = i;
                    lastStableIndx[0] = i;
                }
            }
            else
            {
                states.Add(new object[7] { "stable", i, 0d, double.NegativeInfinity, i, double.NegativeInfinity, double.NegativeInfinity }); // { state, index_in_signal, acceleration, first_tangente, first_tangentes_indx, mean_amplitude, mean_tangent }
                lastStableIndx[0] = i;
                lastStableIndx[1] = states.Count - 1;
            }
            // Reset everything
            resetEverything = true;
        }

        //*******************************************************************************************************//
        //***********************************************QRS SCANNER*********************************************//
        /// <summary>
        /// Returns QRS indexes as int[] {Q index, R index, S index}
        /// where the energy of the QRS should be at least 60% higher than interval
        /// </summary>
        /// <param name="samples">signal samples</param>
        /// <param name="interval">interval between max and min (max - min)</param>
        /// <param name="states">list of all peaks</param>
        /// <returns></returns>
        public static List<int[]> scanQRS(double[] samples, double interval, List<object[]> states)
        {
            // Scan for QRS peaks
            // Get QRS peaks indexes from _samples
            List<int[]> qrsPeaks = new List<int[]>(states.Count); // as [Q index, R index, S index]
            // Where should (((up1 - down0) + (up1 - down1)) / 2) > 60% of the interval
            int[] qrsPeak = new int[3] { -1, -1, -1 };
            for (int i = 0; i < states.Count; i++)
            {
                // Check if we are looking for the first up
                if (qrsPeak[0] == -1)
                {
                    // If yes then check if current status is up
                    if (states[i][0].Equals("up"))
                        // If yes then set the previous state index as Q
                        if (i > 0)
                            qrsPeak[0] = (int)states[i - 1][1];
                        else
                            qrsPeak[0] = 0;
                    else
                        // If yes then just continue to the next iteration
                        continue;
                }
                // Check if we are looking for the R peak
                else if (qrsPeak[1] == -1)
                {
                    // If yes then check if current status is down
                    if (states[i][0].Equals("down"))
                        // Check if previous state was stable
                        if (states[i - 1][0].Equals("stable"))
                            // If yes then set the middle of stable status as R
                            qrsPeak[1] = ((int)states[i - 1][1] + (int)states[i - 2][1]) / 2;
                        else
                            // If yes then set the previous state index as R
                            qrsPeak[1] = (int)states[i - 1][1];
                    else
                        // If yes then just continue to the next iteration
                        continue;
                }
                else
                {
                    // If yes then we are looking for the S peak
                    // Check if current status is up or the last status
                    if (states[i][0].Equals("up"))
                    {
                        // If yes then set the previous state index as S
                        // Check if the previous status wasn't stable
                        if (!states[i - 1][0].Equals("stable"))
                            qrsPeak[2] = (int)states[i - 1][1];
                        else
                            qrsPeak[2] = (int)states[i - 2][1];

                        // Check if current qrs energy is higher than 60% of interval
                        if (((samples[qrsPeak[1]] - samples[qrsPeak[0]] + samples[qrsPeak[1]] - samples[qrsPeak[2]]) / (2 * interval)) > 0.6)
                            // If yes then insert the new peak
                            qrsPeaks.Add(qrsPeak);
                        qrsPeak = new int[3] { (int)states[i - 1][1], -1, -1 };
                    }
                    else if (i + 1 == states.Count)
                    {
                        // If yes then set current status index as S
                        qrsPeak[2] = (int)states[i][1];

                        // Check if current qrs energy is higher than 60% of interval
                        if (((samples[qrsPeak[1]] - samples[qrsPeak[0]] + samples[qrsPeak[1]] - samples[qrsPeak[2]]) / (2 * interval)) > 0.6)
                            // If yes then insert the new peak
                            qrsPeaks.Add(qrsPeak);
                        qrsPeak = new int[3] { -1, -1, -1 };
                    }
                    else
                        // If yes then just continue to the next iteration
                        continue;
                }
            }

            return qrsPeaks;
        }

        //*******************************************************************************************************//
        //******************************************DYNAMIC TIME WRAPING*****************************************//
        public static double[,] dynamicTimeWraping(double[] signal1, double[] signal2, int window)
        {
            // Initailize dynamic time wraping matrix
            double[,] dtw = new double[signal1.Length + 1, signal2.Length + 1];

            // Set the window constraint
            window = Math.Max(window, Math.Abs(signal1.Length - signal2.Length));

            // Set the initial values of dtw matrix
            for (int i = 0; i < signal1.Length + 1; i++)
                for (int j = 0; j < signal2.Length + 1; j++)
                    if ((j >= Math.Max(1, i - window)) && (j <= Math.Min(signal2.Length, i + window)) && (i != 0))
                        dtw[i, j] = 0D;
                    else
                        dtw[i, j] = double.PositiveInfinity;
            dtw[0, 0] = 0D;

            // Calculate dwt matrix
            double cost;
            double last_min;
            for (int i = 1; i < signal1.Length + 1; i++)
                for (int j = Math.Max(1, i - window); j <= Math.Min(signal2.Length, i + window); j++)
                {
                    // Calculate the cost of current position
                    cost = Math.Abs(signal1[i - 1] - signal2[j - 1]);

                    // Take the min value from the last square of 4 elements
                    last_min = Math.Min(dtw[i - 1, j], Math.Min(dtw[i, j - 1], dtw[i - 1, j - 1]));

                    dtw[i, j] = cost + last_min;
                }

            return dtw;
        }

        public static object[] dynamicTimeWrapingDistancePath(double[] signal1, double[] signal2, int window)
        {
            // Create the path and distance variables
            List<int[]> path = new List<int[]>();
            path.Add(new int[] { 0, 0 });

            double distance = -1;

            // Calculate dtw matrix
            double[,] dtw = dynamicTimeWraping(signal1, signal2, window);
            // create the path
            int i = 1;
            int j = 1;
            while ((i < dtw.GetLength(0) - 1) || (j < dtw.GetLength(1) - 1))
            {
                // Check if this is the last vertical index
                if (i + 1 == dtw.GetLength(0))
                    // If yes then the next move is horizontal
                    j += 1;
                // Check if this is the last horizontal index
                else if (j + 1 == dtw.GetLength(1))
                    // If yes then the next move is horizontal
                    i += 1;
                // Check if the next diagonal move hast the shortest distance in the next square
                else if ((dtw[i + 1, j + 1] <= dtw[i, j + 1]) && (dtw[i + 1, j + 1] <= dtw[i + 1, j]))
                {
                    // If yes then add it to the path
                    i += 1;
                    j += 1;
                }
                // If no then check if the move is horizontal or vertical
                else if (dtw[i + 1, j] <= dtw[i, j + 1])
                    // If yes then the next move is vertical
                    i += 1;
                else
                    // If yes then it is horizontal
                    j += 1;

                // Add the new path index in path
                path.Add(new int[] { i - 1, j - 1 });
            }

            // Get the distance from the path
            if (path.Count > 0)
                distance = dtw[i, j];

            return new object[2] { distance, path };
        }

        //*******************************************************************************************************//
        //*******************************************INTERCORRELATION********************************************//
        public static double[] crossCorrelation(double[] signal1, double[] signal2)
        {
            // Create the variable for intercorrelation
            double[] intercorrelationDouble = new double[signal1.Length + signal2.Length - 1];

            float[] sign1 = new float[signal1.Length];
            float[] sign2 = new float[signal2.Length];

            // Get the length in power of 2 of the signal
            int signalLength = intercorrelationDouble.Length;
            int lengthInPowerOf2 = 2;
            while (lengthInPowerOf2 < signalLength)
                lengthInPowerOf2 *= 2;
            float[] intercorrelationFloat = new float[lengthInPowerOf2];

            // Create the correlation function
            NWaves.Operations.Convolution.Convolver corr = new NWaves.Operations.Convolution.Convolver(lengthInPowerOf2);

            // Get the first signal
            for (int i = 0; i < signal1.Length; i++)
                sign1[i] = (float)signal1[i];

            // Get the second signal
            for (int i = 0; i < signal2.Length; i++)
                sign2[i] = (float)signal2[i];

            // Calculate the intercorrelation
            corr.CrossCorrelate(sign1, sign2, intercorrelationFloat);

            // Copy the result in intercorrelationDouble
            for (int i = 0; i < intercorrelationDouble.Length; i++)
                intercorrelationDouble[i] += (double)intercorrelationFloat[i];

            return intercorrelationDouble;
        }

        //*******************************************************************************************************//
        //******************************************MINIMUM SUBTRACTION******************************************//
        public static double[] minimumSubtraction(double[] signal1, double[] signal2)
        {
            // Create the variable for subtraction signal
            int subtractionSignalLength = signal1.Length + signal2.Length;
            double[] subtractionSignal = new double[subtractionSignalLength];

            // Calculate the subtraction coefition
            double newsubtractionCoef;
            double[] subtractionCoefIndex = new double[2] { double.PositiveInfinity, 0 };
            for (int i = 0; i <= subtractionSignalLength; i++)
            {
                newsubtractionCoef = 0D;

                // Calculate the subtraction of two signals delayed
                subtractionSignal = subtractSignalsInDelay(signal1, signal2, i);

                // Calculate the total energy of the subtraction
                foreach (double sample in subtractionSignal)
                    newsubtractionCoef += sample / subtractionSignalLength;

                // Check if new subtracted signals energy is less than previous one
                if (newsubtractionCoef < subtractionCoefIndex[0])
                {
                    // If yes then save it in subtractionCoefIndex with its delay value
                    subtractionCoefIndex[0] = newsubtractionCoef;
                    subtractionCoefIndex[1] = i;
                }
            }

            return subtractSignalsInDelay(signal1, signal2, (int)subtractionCoefIndex[1]); ;
        }

        public static double[] subtractSignalsInDelay(double[] signal1, double[] signal2, int delay)
        {
            // Create the variable for subtraction signal
            int subtractionSignalLength = signal1.Length + signal2.Length;
            double[] subtractionSignal = new double[subtractionSignalLength];

            for (int j = 0; j < subtractionSignalLength; j++)
            {
                // Check if the delay is still larger than the first signal
                if (delay > signal1.Length)
                {
                    // If yes then we should start from the second signal
                    if (j < delay - signal1.Length)
                        // If yes then take sample from the second signal only
                        subtractionSignal[j] = Math.Abs(signal2[j]);
                    // Check if we should subtract from the two signals
                    else if (j < signal2.Length && j < delay)
                        // If yes then we take the absolute value of the subtraction between the two
                        subtractionSignal[j] = Math.Abs(signal2[j] - signal1[j - (delay - signal1.Length)]);
                    // Check if we should take the rest of the fisrt signal
                    else if (j < delay)
                        subtractionSignal[j] = Math.Abs(signal1[j - (delay - signal1.Length)]);
                    // Check if the second signal is still passing the first signal
                    else if (delay > signal1.Length && j < signal2.Length)
                        subtractionSignal[j] = Math.Abs(signal2[j]);
                    else
                        // If yes then break the for loop
                        break;
                }
                else
                {
                    // Check if we should take from the first signal
                    if (delay + j < signal1.Length)
                        // If yes then take sample from the first signal only
                        subtractionSignal[j] = Math.Abs(signal1[j]);
                    // Check if we should subtract from the two signals
                    else if (j < signal1.Length && j + delay < subtractionSignalLength && delay > 0)
                        // If yes then we take the absolute value of the subtraction between the two
                        subtractionSignal[j] = Math.Abs(signal1[j] - signal2[j + delay - signal1.Length]);
                    // Check if we should take the rest of the second signal
                    else if (j + delay < subtractionSignalLength)
                        subtractionSignal[j] = Math.Abs(signal2[j - signal1.Length + delay]);
                    // Check if signal 1 is passing signal 2
                    else if (delay > signal2.Length && j < signal1.Length)
                        subtractionSignal[j] = Math.Abs(signal1[j]);
                    else
                        // If yes then break the for loop
                        break;
                }
            }

            return subtractionSignal;
        }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //::::::::::::::::::::::::::::::::SERIALIZE/DESERIALIZE OBJECT::::::::::::::::::::::::::::::://
        // Convert an object to a byte array
        public static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        // Convert a byte array to an Object
        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                try
                {
                    var obj = binForm.Deserialize(memStream);
                    return obj;
                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return null;
            }
        }

        //*******************************************************************************************************//
        //*******************SEARCH FOR PARTICULAR STRING INSIDE ALL FILES OF A DIRECTORY************************//
        public static void searchForInDir(String searchingFor, String dirPath, String fileExtension)
        {
            // Look for all directories inside the selected one
            int searchedDirs = 0;
            List<String> directories = new List<string>();
            directories.Add(dirPath);

            while (directories.Count > searchedDirs)
            {
                directories.AddRange(System.IO.Directory.EnumerateDirectories(directories[searchedDirs]));
                searchedDirs += 1;
            }

            // Now search for the particular word inside all directories
            foreach (string directorie in directories)
                foreach (string file in System.IO.Directory.EnumerateFiles(directorie, "*." + fileExtension))
                {
                    string contents = System.IO.File.ReadAllText(file);
                    if (contents.Contains(searchingFor))
                        Console.WriteLine(file);
                }
        }

        //*******************************************************************************************************//
        //**********************************************SHUFFLE ARRAY********************************************//
        public static void Shuffle<T>(List<T> array)
        {
            Random rng = new Random();
            int n = array.Count;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        //*******************************************************************************************************//
        //*********************************************PERIOD AS STRING******************************************//
        public static string PeriodInSecToString(long period)
        {
            long days = period / 86400L;
            long hours = (period % 86400L) / 3600L;
            long minutes = ((period % 86400L) % 3600) / 60;
            long sec = period % 60;

            string timeToFinish = "";
            if (days != 0)
                timeToFinish += days + " days, ";
            if (hours != 0)
                timeToFinish += hours + " hours, ";
            if (minutes != 0)
                timeToFinish += minutes + " minutes, ";
            if (sec != 0)
                timeToFinish += sec + " sec";

            return timeToFinish;
        }

        public static long StringToPeriodInSec(string periodString)
        {
            long period = 0L;
            string stringTrunc = "";

            if (periodString.Contains("days"))
            {
                stringTrunc = periodString.Split(new string[] { " days" }, 2, StringSplitOptions.RemoveEmptyEntries)[0];
                period += long.Parse(stringTrunc.Substring(stringTrunc.LastIndexOf(" "))) * 86400L;
            }
            if (periodString.Contains("hours"))
            {
                stringTrunc = periodString.Split(new string[] { " hours" }, 2, StringSplitOptions.RemoveEmptyEntries)[0];
                period += long.Parse(stringTrunc.Substring(stringTrunc.LastIndexOf(" "))) * 3600L;
            }
            if (periodString.Contains("minutes"))
            {
                stringTrunc = periodString.Split(new string[] { " minutes" }, 2, StringSplitOptions.RemoveEmptyEntries)[0];
                period += long.Parse(stringTrunc.Substring(stringTrunc.LastIndexOf(" "))) * 60L;
            }
            if (periodString.Contains("sec"))
            {
                stringTrunc = periodString.Split(new string[] { " sec" }, 2, StringSplitOptions.RemoveEmptyEntries)[0];
                period += long.Parse(stringTrunc.Substring(stringTrunc.LastIndexOf(" ")));
            }

            return period;
        }

        //*******************************************************************************************************//
        //**************************************REARRANGE INPUT OF FEATURES**************************************//
        public static List<object[]> rearrangeFeaturesInput(List<object[]> featuresLists, List<double[]> pcLoadingScores)
        {
            if (pcLoadingScores.Count > 0)
                for (int i = 0; i < featuresLists.Count; i++)
                {
                    double[] inputBuff = new double[pcLoadingScores.Count];
                    for (int j = 0; j < pcLoadingScores.Count; j++)
                        for (int k = 0; k < ((double[])featuresLists[i][0]).Length; k++)
                        inputBuff[j] += ((double[])featuresLists[i][0])[k] * pcLoadingScores[j][k];
                    featuresLists[i][0] = inputBuff;
                }
            return featuresLists;
        }

        public static double[] rearrangeInput(double[] input, List<double[]> pcLoadingScores)
        {
            if (pcLoadingScores.Count > 0)
            {
                double[] inputBuff = new double[pcLoadingScores.Count];
                for (int j = 0; j < pcLoadingScores.Count; j++)
                    for (int k = 0; k < input.Length; k++)
                        inputBuff[j] += input[k] * pcLoadingScores[j][k];
                input = inputBuff;
            }

            return input;
        }
    }
}

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
using static Biological_Signal_Processing_Using_AI.Structures;

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
        public static List<State> scanPeaks(double[] samples, double interval, double thresholdRatio, int horThreshold, double tdtThresholdRatio, bool deviationtionEnabled)
        {
            List<State> states = new List<State>();

            // Add the first temporary state as a stable state
            states.Add(new State
            {
                Name = "stable",
                _index = 0,
                _value = samples[0],
                _minIndex = 0,
                _min = samples[0],
                _maxIndex = 0,
                _max = samples[0],
                _deviantionAngle = 0d,
                _meanFromLastState = samples[0],
                _meanTangentFromLastState = double.MinValue,
                _tangentFromLastState = double.MinValue
            });

            // Create a variable for a temporary new state
            State tempNewState = new State
            {
                _index = 0,
                _value = samples[0],
                _minIndex = 0,
                _min = samples[0],
                _maxIndex = 0,
                _max = samples[0],
                _deviantionAngle = 0d,
                _meanFromLastState = samples[0],
                _meanTangentFromLastState = double.MinValue,
                _tangentFromLastState = double.MinValue
            };

            // Create a variable for last recorded state
            State lastState;
            double differenceRatio;
            double oppositeRatio;
            // Iterate through all next samples in the signal
            for (int i = 1; i < samples.Length; i++)
            {
                // Get last state
                lastState = states[states.Count - 1];

                // Update the temporary new state
                tempNewState._index = i;
                tempNewState._value = samples[i];
                tempNewState._meanFromLastState = (tempNewState._meanFromLastState + samples[i]) / 2;

                // Compute difference ratio between current sample and last state
                differenceRatio = (samples[i] - lastState._value) / interval;

                // Calculate opposite
                oppositeRatio = opposite(ref states, ref tempNewState) / interval;

                // Update min and max of tempNewState
                if (samples[i] < tempNewState._min)
                {
                    tempNewState._min = samples[i];
                    tempNewState._minIndex = i;
                }
                if (samples[i] > tempNewState._max)
                {
                    tempNewState._max = samples[i];
                    tempNewState._maxIndex = i;
                }

                // Check if opposite reaches thresholdRatio
                if (oppositeRatio > thresholdRatio)
                {
                    // Check if amplitude between current sample and last state is greater than thresholdRatio
                    if (differenceRatio > thresholdRatio)
                    {
                        // If yes then set the new state as Up
                        tempNewState.Name = "up";
                        tempNewState._value = tempNewState._max;
                        tempNewState._index = tempNewState._maxIndex;
                    }
                    // Check if amplitude between current sample and last state is less than -thresholdRatio
                    else if (differenceRatio < -thresholdRatio)
                    {
                        // If yes then set the new state as Up
                        tempNewState.Name = "down";
                        tempNewState._value = tempNewState._min;
                        tempNewState._index = tempNewState._minIndex;
                    }
                    else
                        // If yes then set the new state as Stable
                        tempNewState.Name = "stable";

                    // Check if temporary new state has a name
                    if (tempNewState.Name != null)
                    {
                        // If yes then add a new state
                        states.Add(tempNewState);

                        // Refresh the temporary new state
                        tempNewState = new State
                        {
                            _index = i,
                            _value = samples[i],
                            _minIndex = i,
                            _min = samples[i],
                            _maxIndex = i,
                            _max = samples[i],
                            _deviantionAngle = 0d,
                            _meanFromLastState = samples[i],
                            _meanTangentFromLastState = double.MinValue,
                            _tangentFromLastState = double.MinValue
                        };
                    }
                }

                ///:::::::::::::::::::: FOT TANGENT DEVIATION TOLERANCE :::::::::::::::::::::///
                // Set tangent deviation angle if enabled
                if (states.Count > 1 && deviationtionEnabled)
                {
                    // Calculate this deviation in amplitude
                    double tolerance = opposite(ref states, ref tempNewState);

                    // Check if tolerance is higher than tdtThresholdRatio % of interval
                    if (tolerance / interval > tdtThresholdRatio)
                    {
                        // If yes then new tangent deviation reached threshold
                        // Add new state for the new accelerated state
                        if (lastState.Name.Equals("up"))
                            states.Add(new State
                            {
                                Name = "up",
                                _index = i,
                                _value = samples[i],
                                _minIndex = i,
                                _min = samples[i],
                                _maxIndex = i,
                                _max = samples[i],
                                _deviantionAngle = 0d,
                                _meanFromLastState = samples[i],
                                _meanTangentFromLastState = double.MinValue,
                                _tangentFromLastState = double.MinValue
                            });
                        else if (lastState.Name.Equals("down"))
                            states.Add(new State
                            {
                                Name = "down",
                                _index = i,
                                _value = samples[i],
                                _minIndex = i,
                                _min = samples[i],
                                _maxIndex = i,
                                _max = samples[i],
                                _deviantionAngle = 0d,
                                _meanFromLastState = samples[i],
                                _meanTangentFromLastState = double.MinValue,
                                _tangentFromLastState = double.MinValue
                            });
                        else if (lastState.Name.Equals("stable"))
                            states.Add(new State
                            {
                                Name = "stable",
                                _index = i,
                                _value = samples[i],
                                _minIndex = i,
                                _min = samples[i],
                                _maxIndex = i,
                                _max = samples[i],
                                _deviantionAngle = 0d,
                                _meanFromLastState = samples[i],
                                _meanTangentFromLastState = double.MinValue,
                                _tangentFromLastState = double.MinValue
                            });
                    }

                    ///:::::::::::::::::::: STATE ROTAION :::::::::::::::::::::///
                    // Update second last state's acceleration value
                    if (states.Count > 2)
                    {
                        double lastTan = (states[states.Count - 1]._value - states[states.Count - 2]._value) / (states[states.Count - 1]._index - states[states.Count - 2]._index);
                        double secondLastTan = (states[states.Count - 2]._value - states[states.Count - 3]._value) / (states[states.Count - 2]._index - states[states.Count - 3]._index);

                        states[states.Count - 2]._deviantionAngle = (Math.Atan(lastTan) - Math.Atan(secondLastTan)) * 180 / Math.PI;
                    }
                }

            }

            return states;
        }

        private static double opposite(ref List<State> states, ref State tempNewState)
        {
            // Calculate tangent argument between last two states
            tempNewState._tangentFromLastState = (tempNewState._value - states[states.Count - 1]._value) / (tempNewState._index - states[states.Count - 1]._index);
            // Update mean_tangent
            tempNewState._meanTangentFromLastState = (tempNewState._meanFromLastState - states[states.Count - 1]._value) / (tempNewState._index - states[states.Count - 1]._index);

            // Calculate deviation between last tangent and mean tangent in rad
            double tangentedeviation = Math.Abs(Math.Atan(tempNewState._tangentFromLastState) - Math.Atan(tempNewState._meanTangentFromLastState));
            // Calculate this deviation in amplitude
            double opposite = Math.Sin(tangentedeviation) * Math.Sqrt(Math.Pow(tempNewState._value - states[states.Count - 1]._value, 2) + Math.Pow(tempNewState._index - states[states.Count - 1]._index, 2));

            return opposite;
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
        public static List<int[]> scanQRS(double[] samples, double interval, List<State> states)
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
                    if (states[i].Name.Equals("up"))
                        // If yes then set the previous state index as Q
                        if (i > 0)
                            qrsPeak[0] = (int)states[i - 1]._index;
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
                    if (states[i].Name.Equals("down"))
                        // Check if previous state was stable
                        if (states[i - 1].Name.Equals("stable"))
                            // If yes then set the middle of stable status as R
                            qrsPeak[1] = ((int)states[i - 1]._index + (int)states[i - 2]._index) / 2;
                        else
                            // If yes then set the previous state index as R
                            qrsPeak[1] = (int)states[i - 1]._index;
                    else
                        // If yes then just continue to the next iteration
                        continue;
                }
                else
                {
                    // If yes then we are looking for the S peak
                    // Check if current status is up or the last status
                    if (states[i].Name.Equals("up"))
                    {
                        // If yes then set the previous state index as S
                        // Check if the previous status wasn't stable
                        if (!states[i - 1].Name.Equals("stable"))
                            qrsPeak[2] = (int)states[i - 1]._index;
                        else
                            qrsPeak[2] = (int)states[i - 2]._index;

                        // Check if current qrs energy is higher than 60% of interval
                        if (((samples[qrsPeak[1]] - samples[qrsPeak[0]] + samples[qrsPeak[1]] - samples[qrsPeak[2]]) / (2 * interval)) > 0.6)
                            // If yes then insert the new peak
                            qrsPeaks.Add(qrsPeak);
                        qrsPeak = new int[3] { (int)states[i - 1]._index, -1, -1 };
                    }
                    else if (i + 1 == states.Count)
                    {
                        // If yes then set current status index as S
                        qrsPeak[2] = (int)states[i]._index;

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

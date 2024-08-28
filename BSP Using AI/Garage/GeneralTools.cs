using BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation;
using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.Renderable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.Structures;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace Biological_Signal_Processing_Using_AI.Garage
{
    class GeneralTools
    {
        //*******************************************************************************************************//
        //*************************************INSERT SIGNAL VECTOR IN CHART*************************************//
        public static void loadXYInChart(FormsPlot chart, IPlottable plottable, double[] xValues, double[] yValues, string[] labels, double startingInSec, bool resetAxis, string reference)
        {
            if (plottable is ScatterPlot scatterPlot)
            {
                if (xValues != null && xValues.Length > 0)
                {
                    scatterPlot.OffsetX = startingInSec;
                    scatterPlot.Update(xValues, yValues);
                    scatterPlot.DataPointLabels = CreateEmptyStrings(scatterPlot.PointCount);
                    if (labels != null)
                    {
                        scatterPlot.DataPointLabels = labels;
                        scatterPlot.DataPointLabelFont.Color = Color.Black;
                    }
                    else
                        scatterPlot.DataPointLabelFont.Color = Color.Transparent;
                    scatterPlot.IsVisible = true;
                }
                else
                {
                    scatterPlot.Update(new double[] { 0 }, new double[] { 0 });
                    scatterPlot.DataPointLabels = CreateEmptyStrings(scatterPlot.PointCount);
                    scatterPlot.IsVisible = false;
                }
            }
            // Set the axis limits automatically to fit the data on the plot
            if (resetAxis)
                chart.Plot.AxisAuto();
            // Display changes
            chart.Refresh();
        }

        public static void TimeSpanVisibility(FormsPlot chart, IPlottable hSpan, bool visible)
        {
            hSpan.IsVisible = visible;
            chart.Refresh();
        }

        public static string[] CreateEmptyStrings(int length)
        {
            string[] strings = new string[length];
            for (int i = 0; i < length; i++)
                strings[i] = "";
            return strings;
        }

        public static ScatterPlot AddScatterPlot(FormsPlot chart, Color color, string label)
        {
            ScatterPlot scatterPlot = chart.Plot.AddScatterPoints(new double[] { 0 }, new double[] { 0 }, color, label: label);
            scatterPlot.IsVisible = false;
            Legend legend = chart.Plot.Legend();
            legend.Orientation = ScottPlot.Orientation.Horizontal;
            return scatterPlot;
        }

        public static HSpan AddHorizontalSpan(FormsPlot chart, Color color, string label, HorizSpan_Dragged_Delegate draggingHandler)
        {
            HSpan horizSpan = chart.Plot.AddHorizontalSpan(0, 0, Color.FromArgb(51, color), label: label);
            horizSpan.Dragged += new System.EventHandler(draggingHandler);
            horizSpan.DragEnabled = true;
            horizSpan.BorderLineWidth = 2;
            horizSpan.BorderLineStyle = LineStyle.Solid;
            horizSpan.BorderColor = color;
            horizSpan.IsVisible = false;
            return horizSpan;
        }

        public static SignalPlot loadSignalInChart(FormsPlot chart, double[] samples, double samplingRate, double startingInSec, string reference)
        {
            // Get all non signal plottables and clear the plot
            List<IPlottable> nonSignalPlotables = new List<IPlottable>();
            IPlottable[] plottables = chart.Plot.GetPlottables();
            if (plottables.Length > 0)
                foreach (IPlottable plottable in plottables)
                    if (plottable is not SignalPlot)
                        nonSignalPlotables.Add(plottable);
            chart.Plot.Clear();

            // Insert the new signal
            SignalPlot signalPlot = chart.Plot.AddSignal(samples, samplingRate, color: Color.FromArgb(255, 31, 119, 180));
            signalPlot.OffsetX = startingInSec;
            signalPlot.MarkerSize = 0;
            signalPlot.LineWidth = 0.7;

            // Insert the non signal plottables
            foreach (IPlottable plottable in nonSignalPlotables)
                chart.Plot.Add(plottable);

            // Set the axis limits automatically to fit the data on the plot
            chart.Plot.AxisAuto();

            chart.Refresh();

            return signalPlot;
        }
        public static void loadSignalInChart(FormsPlot chart, double[] xValues, double[] yValues, double startingInSec, string reference)
        {
            // Check if the chart has an only one scatter plot
            IPlottable[] plottables = chart.Plot.GetPlottables();
            ScatterPlot scatterPlot = null;
            bool resetPlot = true;
            if (plottables.Length == 1)
                if (plottables[0] is ScatterPlot)
                {
                    scatterPlot = (ScatterPlot)plottables[0];
                    resetPlot = false;
                }

            // Check if it does not have a scatter plot
            if (resetPlot)
            {
                // Then create one
                chart.Plot.Clear();
                scatterPlot = chart.Plot.AddScatterLines(xValues, yValues);
                scatterPlot.OffsetX = startingInSec;
                scatterPlot.LineWidth = 0.7;
                chart.Refresh();
            }
            else
                // Else update the existen scatterplot
                loadXYInChart(chart, scatterPlot, xValues, yValues, null, startingInSec, true, reference);
        }

        public static (double nearestX, double nearestY, int index) GetPointNearestXYSignalPlot(SignalPlot signalPlot, double cursorX, double cursorY)
        {
            int index = signalPlot.Ys.Select((y, i) => (Math.Abs(y - cursorY) + Math.Abs((i / signalPlot.SampleRate + signalPlot.OffsetX) - cursorX), i)).Min().i;

            double nearestX = index / signalPlot.SampleRate + signalPlot.OffsetX;
            double nearestY = signalPlot.Ys[index];

            return (nearestX, nearestY, index);
        }

        //*******************************************************************************************************//
        //****************************************ABSOLUTE VALUES SIGNAL*****************************************//
        public static double[] absoluteSignal(double[] samples)
        {
            double[] filteredSamples = new double[samples.Length];
            for (int i = 0; i < samples.Length; i++)
                filteredSamples[i] = Math.Abs(samples[i]);

            return filteredSamples;
        }

        //*******************************************************************************************************//
        //*****************************************Vector normalization******************************************//
        public static (double[] normalizedSamples, double normalizationCoef) vectorNormalization(double[] samples)
        {
            // Calculate the normalizationCoef of the selected signal
            double[] normalizedSamples = new double[samples.Length];
            double normalizationCoef = 0d;
            foreach (double sample in samples)
                normalizationCoef += sample * sample;
            normalizationCoef = Math.Sqrt(normalizationCoef);

            for (int i = 0; i < samples.Length; i++)
            {
                normalizedSamples[i] = samples[i] / normalizationCoef;
                if (double.IsNaN(normalizedSamples[i]))
                    normalizedSamples[i] = 0d;
            }

            return (normalizedSamples, normalizationCoef);
        }
        //*******************************************************************************************************//
        //*****************************************min max normalization*****************************************//
        public static double[] normalizeSignal(double[] samples)
        {
            (double mean, double min, double max) meanMinMax = MeanMinMax(samples);
            double ampInterval = meanMinMax.max - meanMinMax.min;
            // Normalize the samples
            double[] filteredSamples = new double[samples.Length];

            for (int i = 0; i < samples.Length; i++)
                filteredSamples[i] = (samples[i] - meanMinMax.min) / ampInterval;

            return filteredSamples;
        }

        //*******************************************************************************************************//
        //*****************************************min max normalization*****************************************//
        public static double[] rescaleSignal(double[] samples, double newAmpInterval)
        {
            (double mean, double min, double max) meanMinMax = MeanMinMax(samples);
            double ampInterval = meanMinMax.max - meanMinMax.min;
            double scalingRatio = newAmpInterval / ampInterval;
            // Normalize the samples
            double[] filteredSamples = new double[samples.Length];

            for (int i = 0; i < samples.Length; i++)
                filteredSamples[i] = (samples[i] - meanMinMax.min) * scalingRatio;

            return filteredSamples;
        }

        //*******************************************************************************************************//
        //******************************************PDF COEFS OF SIGNAL******************************************//
        public static List<StatParam> statParams(double[] samples)
        {
            List<StatParam> statParams = new List<StatParam>(5);

            // Get mean, min, and max of the signal
            (double mean, double min, double max) = MeanMinMax(samples);

            // Get standart deviation of the signal
            double stdDev = stdDevCalc(samples, mean);

            // Calculate inter quartile range (IQR)
            double IQR = signalIQR(samples);

            statParams.Add(new StatParam() { Name = ARTHTNamings.Mean, _value = mean });
            statParams.Add(new StatParam() { Name = ARTHTNamings.Min, _value = min });
            statParams.Add(new StatParam() { Name = ARTHTNamings.Max, _value = max });
            statParams.Add(new StatParam() { Name = ARTHTNamings.StdDev, _value = stdDev });
            statParams.Add(new StatParam() { Name = ARTHTNamings.IQR, _value = IQR });
            return statParams;
        }

        public static (float mean, float min, float max) MeanMinMax(float[] samples)
        {
            // Calculate mean, min, max of signal
            float mean = 0f;
            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;
            foreach (float sample in samples)
            {
                mean += sample / samples.Length;
                if (sample < min)
                    min = sample;
                if (sample > max)
                    max = sample;
            }

            return (mean, min, max);
        }
        public static (double mean, double min, double max) MeanMinMax(double[] samples)
        {
            // Calculate mean, min, max of signal
            double mean = 0d;
            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;
            foreach (double sample in samples)
            {
                mean += sample / samples.Length;
                if (sample < min)
                    min = sample;
                if (sample > max)
                    max = sample;
            }

            return (mean, min, max);
        }

        public static double stdDevCalc(double[] samples, double mean)
        {
            // Calculate standard deviation
            double stdDev = 0d;
            foreach (double sample in samples)
                stdDev += Math.Pow(sample - mean, 2) / samples.Length;
            stdDev = Math.Sqrt(stdDev);

            return stdDev;
        }

        //*******************************************************************************************************//
        //*********************************************IQR OF SIGNAL*********************************************//
        public static double signalIQR(double[] signal)
        {
            // Sort the signal
            double[] samples = new double[signal.Length];
            for (int i = 0; i < signal.Length; i++)
                samples[i] = signal[i];

            Array.Sort(samples);

            int mid_indx = medianIndex(0, samples.Length);
            double Q1 = samples[medianIndex(0, mid_indx)];
            double Q3 = samples[medianIndex(mid_indx + 1, samples.Length)];

            return Q3 - Q1;
        }

        public static int medianIndex(int left, int right)
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
        public static double[] removeDCValue(double[] samples)
        {
            if (samples.Length == 0)
                return null;
            double[] filteredSamples = (double[])samples.Clone();

            // Calculate the dc value
            double dcValue = samples[0];
            for (int i = 1; i < samples.Length; i++)
                dcValue = (dcValue * i + samples[i]) / (i + 1);

            // Remove the dc value from the singal samples
            for (int i = 0; i < samples.Length; i++)
                filteredSamples[i] -= dcValue;

            return filteredSamples;
        }

        //*******************************************************************************************************//
        //**********************************************DWT TRANSFORM********************************************//
        public static List<double[]> calculateDWT(double[] samples, string waveletName, int maxLevel)
        {
            if (samples.Length == 0)
                return null;
            // Create the signal of length of power of 2
            double[] signalOfPowerOf2 = createPowerOf2Signal(samples);
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
            List<double[]> dwtLevelsSamples = new List<double[]>(allLevels);

            int levelSignalLength = samples.Length / 2;
            if (samples.Length % 2 == 1)
                levelSignalLength += 1;

            int levelAddedSamplesLen = (dwtOfSignal.Length - samples.Length) / 2;

            dwtLevelsSamples.Add(new double[levelSignalLength]);
            for (int j = 0; j < levelSignalLength; j++)
                dwtLevelsSamples[0][j] = dwtOfSignal[levelSignalLength + levelAddedSamplesLen + j];

            samples = new double[levelSignalLength];
            for (int j = 0; j < levelSignalLength; j++)
                samples[j] = dwtOfSignal[j];

            try
            {
                for (int i = 1; i < allLevels; i++)
                {
                    // Create the signal of length of power of 2
                    signalOfPowerOf2 = createPowerOf2Signal(samples);
                    // Create a copy of the signal in float items
                    signalInPowerOf2 = new float[signalOfPowerOf2.Length];
                    for (int j = 0; j < signalOfPowerOf2.Length; j++)
                        signalInPowerOf2[j] = (float)signalOfPowerOf2[j];

                    // Calculate dwt and insert it in dwtOfSignal
                    dwtOfSignal = new float[signalOfPowerOf2.Length];
                    fwt.Direct(signalInPowerOf2, dwtOfSignal, 1);

                    levelSignalLength = samples.Length / 2;
                    if (samples.Length % 2 == 1)
                        levelSignalLength += 1;

                    levelAddedSamplesLen = (dwtOfSignal.Length - samples.Length) / 2;

                    dwtLevelsSamples.Add(new double[levelSignalLength]);
                    for (int j = 0; j < levelSignalLength; j++)
                        dwtLevelsSamples[i][j] = dwtOfSignal[levelSignalLength + levelAddedSamplesLen + j];

                    samples = new double[levelSignalLength];
                    for (int j = 0; j < levelSignalLength; j++)
                        samples[j] = dwtOfSignal[j];
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
                    signalInPowerOf2[i] = signal[i] * lengthInPowerOf2 / signalLength;
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
            return n == 1 || n == 0 ?
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
                newPsi = vectorNormalization(newPsi).normalizedSamples;

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

            return max - min;
        }

        //*******************************************************************************************************//
        //**********************************************PEAKS SCANNER********************************************//
        /// <summary>
        /// Returns the states of each move in the signal
        /// as object [] {"state", its index}
        /// where the state could be up, down, or stable
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="ART">threshold for amplitude resolution</param>
        /// <param name="HT">threshold for horizontal resolution</param>
        /// <param name="TDT">threshold for the opposite of tangent deviation between last state and next state</param>
        /// <param name="deviationtionEnabled">compute tangent deviation if enabled</param>
        /// <returns></returns>
        public static Dictionary<string, List<State>> scanPeaks(double[] samples, double ART, int HT, double TDT, double samplingRate, bool deviationtionEnabled)
        {
            double amplitudeInterval = GeneralTools.amplitudeInterval(samples);
            Dictionary<string, List<State>> statesDIc = new Dictionary<string, List<State>>(5)
            {
                { SANamings.AllPeaks, new List<State>() },
                { SANamings.ScatterPlotsNames.UpPeaks, new List<State>() },
                { SANamings.ScatterPlotsNames.DownPeaks, new List<State>() },
                { SANamings.ScatterPlotsNames.StableStates, new List<State>() },
            };

            if (amplitudeInterval == 0 || samplingRate == 0) return statesDIc;

            bool resetEverything = false;
            TempState up = new TempState() { Name = SANamings.Up, _edgeValue = double.NegativeInfinity };
            TempState down = new TempState() { Name = SANamings.Down, _edgeValue = double.PositiveInfinity };
            TempState stable = new TempState() { Name = SANamings.Stable };

            State lastUpState = new State() { Name = up.Name, _index = 0, _value = samples[0], _firstApearanceValue = samples[0] }; ////////// Verify this
            State lastDownState = new State() { Name = down.Name, _index = 0, _value = samples[0], _firstApearanceValue = samples[0] }; ////////// Verify this
            State lastStableState = new State() { Name = stable.Name, _index = 0, _value = samples[0], _firstApearanceValue = samples[0] }; ////////// Verify this

            double previousSample = samples[0];
            double lastStateValue = samples[0];
            double opposite;
            for (int i = 1; i < samples.Length; i++)
            {
                // Check if this move is going up/down/or stable
                if (samples[i] > previousSample)
                {
                    // Check if current sample value is the highest recorded
                    if (samples[i] > up._edgeValue)
                    {
                        up._edgeIndex = i;
                        up._edgeValue = samples[i];

                        // Check if last state is up and is not away more than horThreshold
                        if (lastUpState._index > lastDownState._index && lastUpState._index > lastStableState._index && i - lastUpState._index < HT)
                            // If yes then update up peak
                            addUpState(ref statesDIc, ref up, ref lastUpState, lastDownState, lastStableState, ref resetEverything, false);
                    }
                }
                else if (samples[i] < previousSample)
                {
                    // Check if current sample value is the lowest recorded
                    if (samples[i] < down._edgeValue)
                    {
                        down._edgeIndex = i;
                        down._edgeValue = samples[i];

                        // Check if last state is up and is not away more than horThreshold
                        if (lastDownState._index > lastUpState._index && lastDownState._index > lastStableState._index && i - lastDownState._index < HT)
                            // If yes then update down peak
                            addDownState(ref statesDIc, ref down, lastUpState, ref lastDownState, lastStableState, ref resetEverything, false);
                    }
                }

                // Get last state's index
                int lastStateIndex = statesDIc[SANamings.AllPeaks].Count - 1;
                if (statesDIc[SANamings.AllPeaks].Count > 0)
                    if (statesDIc[SANamings.AllPeaks][lastStateIndex].Name.Equals(SANamings.Stable))
                        lastStateValue = statesDIc[SANamings.AllPeaks][lastStateIndex]._firstApearanceValue;
                    else
                        lastStateValue = statesDIc[SANamings.AllPeaks][lastStateIndex]._value;

                // Check if its absolute change of moving up is thresholdRatio % more than interval (max - min)
                if ((samples[i] - lastStateValue) / amplitudeInterval > ART)
                    // If yes then add new up peak
                    addUpState(ref statesDIc, ref up, ref lastUpState, lastDownState, lastStableState, ref resetEverything, false);
                // Check if its absolute change of moving down is more than interval (max - min) with thresholdRatio%
                else if (-(samples[i] - lastStateValue) / amplitudeInterval > ART)
                    // If yes then add new down peak
                    addDownState(ref statesDIc, ref down, lastUpState, ref lastDownState, lastStableState, ref resetEverything, false);
                // Check if current index is away from the index of last state with more than horThreshold
                // and the absolure change between up and down is less than thresholdRatio
                else if (i - lastUpState._index > HT && i - lastDownState._index > HT && i > lastStableState._index && Math.Abs((samples[i] - lastStateValue) / amplitudeInterval) < ART)
                {
                    // If yes then add new stable state
                    stable._edgeIndex = i - 1;
                    stable._edgeValue = samples[i - 1];
                    addStableState(ref statesDIc, ref stable, lastUpState, lastDownState, ref lastStableState, ref resetEverything, false);
                }

                ///:::::::::::::::::::: FOT TANGENT DEVIATION TOLERANCE :::::::::::::::::::::///
                // Set tangent deviation angle if enabled
                if (statesDIc[SANamings.AllPeaks].Count > 1 && deviationtionEnabled)
                {
                    // Update last state's index
                    lastStateIndex = statesDIc[SANamings.AllPeaks].Count - 1;
                    opposite = Opposite(statesDIc[SANamings.AllPeaks][lastStateIndex - 1], statesDIc[SANamings.AllPeaks][lastStateIndex], samplingRate);

                    // Check if opposite is higher than tdtThresholdRatio % of interval
                    if (opposite / amplitudeInterval > TDT)
                    {
                        // If yes then new tangent deviation reached threshold

                        // Add new state for the new accelerated state
                        if (statesDIc[SANamings.AllPeaks][lastStateIndex].Name.Equals(SANamings.Up))
                            addUpState(ref statesDIc, ref up, ref lastUpState, lastDownState, lastStableState, ref resetEverything, true);
                        else if (statesDIc[SANamings.AllPeaks][lastStateIndex].Name.Equals(SANamings.Down))
                            addDownState(ref statesDIc, ref down, lastUpState, ref lastDownState, lastStableState, ref resetEverything, true);
                        else if (statesDIc[SANamings.AllPeaks][lastStateIndex].Name.Equals(SANamings.Stable))
                        {
                            stable._edgeIndex = i - 1;
                            stable._edgeValue = samples[i - 1];
                            addStableState(ref statesDIc, ref stable, lastUpState, lastDownState, ref lastStableState, ref resetEverything, true);
                        }
                    }

                    ///:::::::::::::::::::: STATE ROTAION :::::::::::::::::::::///
                    // Update second last state's acceleration value
                    if (statesDIc[SANamings.AllPeaks].Count > 2)
                        statesDIc[SANamings.AllPeaks][lastStateIndex - 1]._deviantionAngle = (Math.Atan(statesDIc[SANamings.AllPeaks][lastStateIndex]._meanTangentFromLastState) - Math.Atan(statesDIc[SANamings.AllPeaks][lastStateIndex - 1]._meanTangentFromLastState)) *
                                                                    180 / Math.PI;
                }

                // Check if everything should be resetted
                if (resetEverything)
                {
                    up._edgeValue = double.NegativeInfinity;
                    down._edgeValue = double.PositiveInfinity;

                    resetEverything = false;
                }

                // Set the new previous sample
                previousSample = samples[i];

            }

            return statesDIc;
        }

        private static void addUpState(ref Dictionary<string, List<State>> statesDIc, ref TempState up, ref State lastUpState, State lastDownState, State lastStableState, ref bool resetEverything, bool justAdd)
        {
            if (statesDIc[SANamings.AllPeaks].Count > 0)
            {
                // Check if there is different state before this new up state
                if (lastDownState._index > lastUpState._index && up._edgeIndex > lastDownState._index || lastStableState._index > lastUpState._index && up._edgeIndex > lastStableState._index || justAdd)
                {
                    // If yes then just add the new down state
                    State upState = new State() { Name = up.Name, _index = up._edgeIndex, _value = up._edgeValue, _firstApearanceValue = up._edgeValue };
                    statesDIc[SANamings.AllPeaks].Add(upState);
                    statesDIc[SANamings.ScatterPlotsNames.UpPeaks].Add(upState);
                    lastUpState = upState;
                    // Reset everything
                    resetEverything = true;
                }
                // Check if last down state is greater than the current new one
                else if (up._edgeValue > lastUpState._value)
                {
                    // If yes then update last up state
                    lastUpState._index = up._edgeIndex;
                    lastUpState._value = up._edgeValue;
                    // Reset everything
                    resetEverything = true;
                }
            }
            else
            {
                State upState = new State() { Name = up.Name, _index = up._edgeIndex, _value = up._edgeValue, _firstApearanceValue = up._edgeValue };
                statesDIc[SANamings.AllPeaks].Add(upState);
                statesDIc[SANamings.ScatterPlotsNames.UpPeaks].Add(upState);
                lastUpState = upState;
                // Reset everything
                resetEverything = true;
            }
        }

        private static void addDownState(ref Dictionary<string, List<State>> statesDIc, ref TempState down, State lastUpState, ref State lastDownState, State lastStableState, ref bool resetEverything, bool justAdd)
        {
            if (statesDIc[SANamings.AllPeaks].Count > 0)
            {
                // Check if there is different state before this new down state
                if (lastUpState._index > lastDownState._index && down._edgeIndex > lastUpState._index || lastStableState._index > lastDownState._index && down._edgeIndex > lastStableState._index || justAdd)
                {
                    // If yes then just add the new down state
                    State downState = new State() { Name = down.Name, _index = down._edgeIndex, _value = down._edgeValue, _firstApearanceValue = down._edgeValue };
                    statesDIc[SANamings.AllPeaks].Add(downState);
                    statesDIc[SANamings.ScatterPlotsNames.DownPeaks].Add(downState);
                    lastDownState = downState;
                    // Reset everything
                    resetEverything = true;
                }
                // Check if last down state is greater than the current new one
                else if (down._edgeValue < lastDownState._value)
                {
                    // If yes then update last down state
                    lastDownState._index = down._edgeIndex;
                    lastDownState._value = down._edgeValue;
                    // Reset everything
                    resetEverything = true;
                }
            }
            else
            {
                State downState = new State() { Name = down.Name, _index = down._edgeIndex, _value = down._edgeValue, _firstApearanceValue = down._edgeValue };
                statesDIc[SANamings.AllPeaks].Add(downState);
                statesDIc[SANamings.ScatterPlotsNames.DownPeaks].Add(downState);
                lastDownState = downState;
                // Reset everything
                resetEverything = true;
            }
        }

        private static void addStableState(ref Dictionary<string, List<State>> statesDIc, ref TempState stable, State lastUpState, State lastDownState, ref State lastStableState, ref bool resetEverything, bool justAdd)
        {
            if (statesDIc[SANamings.AllPeaks].Count > 0)
            {
                // Check if there is different state before this new down state
                if (lastDownState._index > lastStableState._index && stable._edgeIndex > lastDownState._index || lastUpState._index > lastStableState._index && stable._edgeIndex > lastUpState._index || justAdd)
                {
                    // If yes then just add the new stable state
                    State stableState = new State() { Name = stable.Name, _index = stable._edgeIndex, _value = stable._edgeValue, _firstApearanceValue = stable._edgeValue };
                    statesDIc[SANamings.AllPeaks].Add(stableState);
                    statesDIc[SANamings.ScatterPlotsNames.StableStates].Add(stableState);
                    lastStableState = stableState;
                }
                else
                {
                    // If yes then update last stable state
                    lastStableState._index = stable._edgeIndex;
                    lastStableState._value = stable._edgeValue;
                }
            }
            else
            {
                State stableState = new State() { Name = stable.Name, _index = stable._edgeIndex, _value = stable._edgeValue, _firstApearanceValue = stable._edgeValue };
                statesDIc[SANamings.AllPeaks].Add(stableState);
                statesDIc[SANamings.ScatterPlotsNames.StableStates].Add(stableState);
                lastStableState = stableState;
            }
            // Reset everything
            resetEverything = true;
        }

        public static double Opposite(State secondLastState, State lastState, double samplingRate)
        {
            // Calculate tangent argument between last two states
            lastState._tangentFromLastState = (lastState._value - secondLastState._value) / ((lastState._index - secondLastState._index) / samplingRate);
            // Update mean_tangent
            if (double.IsNegativeInfinity(lastState._meanTangentFromLastState))
                lastState._meanTangentFromLastState = (lastState._value - secondLastState._value) / ((lastState._index - secondLastState._index) / samplingRate);
            else
                lastState._meanTangentFromLastState = (lastState._meanTangentFromLastState * (lastState._index - secondLastState._index) + (lastState._value - secondLastState._value) / ((lastState._index - secondLastState._index) / samplingRate)) / (lastState._index - secondLastState._index + 1);

            // Calculate deviation between last tangent and mean tangent in rad
            //double tangentedeviation = Math.Abs(Math.Atan(tempNewState._tangentFromLastState) - Math.Atan(tempNewState._meanTangentFromLastState));
            double tangentedeviation = Math.Atan(lastState._tangentFromLastState) - Math.Atan(lastState._meanTangentFromLastState);
            // Calculate this deviation in amplitude
            double segment = Math.Sqrt(Math.Pow(lastState._value - secondLastState._value, 2) + Math.Pow((lastState._index - secondLastState._index) / samplingRate, 2));
            double opposite = Math.Sin(tangentedeviation) * segment;

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
        public static List<int[]> scanQRS(double[] samples, List<State> states)
        {
            double amplitudeInterval = GeneralTools.amplitudeInterval(samples);
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
                    if (states[i].Name.Equals(SANamings.Up))
                        // If yes then set the previous state index as Q
                        if (i > 0)
                            qrsPeak[0] = states[i - 1]._index;
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
                    if (states[i].Name.Equals(SANamings.Down))
                        // Check if previous state was stable
                        if (states[i - 1].Name.Equals(SANamings.Stable))
                            // If yes then set the middle of stable status as R
                            qrsPeak[1] = (states[i - 1]._index + states[i - 2]._index) / 2;
                        else
                            // If yes then set the previous state index as R
                            qrsPeak[1] = states[i - 1]._index;
                    else
                        // If yes then just continue to the next iteration
                        continue;
                }
                else
                {
                    // If yes then we are looking for the S peak
                    // Check if current status is up or the last status
                    if (states[i].Name.Equals(SANamings.Up))
                    {
                        // If yes then set the previous state index as S
                        // Check if the previous status wasn't stable
                        if (!states[i - 1].Name.Equals(SANamings.Stable))
                            qrsPeak[2] = states[i - 1]._index;
                        else
                            qrsPeak[2] = states[i - 2]._index;

                        // Check if current qrs energy is higher than 60% of interval
                        if ((samples[qrsPeak[1]] - samples[qrsPeak[0]] + samples[qrsPeak[1]] - samples[qrsPeak[2]]) / (2 * amplitudeInterval) > 0.6)
                            // If yes then insert the new peak
                            qrsPeaks.Add(qrsPeak);
                        qrsPeak = new int[3] { states[i - 1]._index, -1, -1 };
                    }
                    else if (i + 1 == states.Count)
                    {
                        // If yes then set current status index as S
                        qrsPeak[2] = states[i]._index;

                        // Check if current qrs energy is higher than 60% of interval
                        if ((samples[qrsPeak[1]] - samples[qrsPeak[0]] + samples[qrsPeak[1]] - samples[qrsPeak[2]]) / (2 * amplitudeInterval) > 0.6)
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
            double[,] dtw = new double[signal1.Length, signal2.Length];

            // Set the window constraint
            window = Math.Max(window, Math.Abs(signal1.Length - signal2.Length));

            // Set the initial values of dtw matrix
            for (int i = 0; i < signal1.Length; i++)
                for (int j = 0; j < signal2.Length; j++)
                    if (j >= Math.Max(1, i - window) && j < Math.Min(signal2.Length, i + window + 1) && i != 0)
                        dtw[i, j] = 0D;
                    else
                        dtw[i, j] = double.PositiveInfinity;
            dtw[0, 0] = Math.Abs(signal1[0] - signal2[0]);

            // Calculate dwt matrix (the accumulated cost matrix)
            double cost;
            double last_min;
            for (int i = 1; i < signal1.Length; i++)
                for (int j = Math.Max(1, i - window); j < Math.Min(signal2.Length, i + window + 1); j++)
                {
                    // Calculate the cost of current position
                    cost = Math.Abs(signal1[i - 1] - signal2[j - 1]);

                    // Take the min value from the last square of 4 elements
                    last_min = Math.Min(dtw[i - 1, j], Math.Min(dtw[i, j - 1], dtw[i - 1, j - 1]));

                    dtw[i, j] = cost + last_min;
                }

            return dtw;
        }

        public static (double distance, double[,] dtw, (int sig1Indx, int sig2Indx)[] path, double[] pathDistance) dynamicTimeWrapingDistancePath(double[] signal1, double[] signal2, int window)
        {
            // Create the path and distance variables
            List<(int sig1Indx, int sig2Indx)> path = new List<(int sig1Indx, int sig2Indx)>();

            double distance; // total cost of the difference between the two signals

            // Calculate dtw matrix
            double[,] dtw = dynamicTimeWraping(signal1, signal2, window);
            // The last cell in DWT is the distance between the two signals
            int i = dtw.GetLength(0) - 1;
            int j = dtw.GetLength(1) - 1;
            distance = dtw[i, j];
            // create the path
            path.Add((i, j));
            while (i > 0 || j > 0)
            {
                // Check if this is the last vertical index
                if (i == 0)
                    // If yes then the next move is horizontal
                    j -= 1;
                // Check if this is the last horizontal index
                else if (j == 0)
                    // If yes then the next move is horizontal
                    i -= 1;
                // Check if the next diagonal move hast the shortest distance in the next square
                else if (dtw[i - 1, j - 1] <= dtw[i, j - 1] && dtw[i - 1, j - 1] <= dtw[i - 1, j])
                {
                    // If yes then add it to the path
                    i -= 1;
                    j -= 1;
                }
                // If no then check if the move is horizontal or vertical
                else if (dtw[i - 1, j] <= dtw[i, j - 1])
                    // If yes then the next move is vertical
                    i -= 1;
                else
                    // If yes then it is horizontal
                    j -= 1;

                // Add the new path index in path
                path.Add((i, j));
            }

            // Now reverse the path
            path.Reverse();

            // Get the difference in distance between the two signals along the optimal path
            double[] pathDistance = new double[path.Count];
            for (int k = 0; k < pathDistance.Length; k++)
                pathDistance[k] = Math.Abs(signal1[path[k].sig1Indx] - signal2[path[k].sig2Indx]);


            return (distance, dtw, path.ToArray(), pathDistance);
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
                intercorrelationDouble[i] += intercorrelationFloat[i];

            return intercorrelationDouble;
        }

        //*******************************************************************************************************//
        //******************************************MINIMUM SUBTRACTION******************************************//
        public static double[] minimumDistance(double[] signal1, double[] signal2)
        {
            // Compute cross-correlation between the two sginals
            double[] crossCor = crossCorrelation(signal1, signal2);
            // Get the index of the highest value in crossCor
            int optimalDelay = Array.IndexOf(crossCor, crossCor.Max());
            // Offset the delay to the middle
            optimalDelay = crossCor.Length / 2 - optimalDelay;
            int absOptimalDelay = Math.Abs(optimalDelay);

            // Set the fixed signal and the delayed signal
            double[] fixedSignal = null;
            double[] delayedSignal = null;
            if (optimalDelay < 0)
            {
                // If yes then the first signal is delayed by optimalDelay
                fixedSignal = signal2;
                delayedSignal = signal1;
            }
            else
            {
                // If yes then the second signal is delayed by optimalDelay
                fixedSignal = signal1;
                delayedSignal = signal2;
            }

            // Compute the distance between the two signals
            double[] minDistance = new double[Math.Max(fixedSignal.Length, delayedSignal.Length + absOptimalDelay)];
            for (int i = 0; i < minDistance.Length; i++)
            {
                double fixedSigVal = 0, delayedSigVal = 0;
                int delayedIndx = i - absOptimalDelay;
                if (i < fixedSignal.Length)
                    fixedSigVal = fixedSignal[i];
                if (delayedIndx >= 0 && delayedIndx < delayedSignal.Length)
                    delayedSigVal = delayedSignal[delayedIndx];

                minDistance[i] = Math.Abs(fixedSigVal - delayedSigVal);
            }
            return minDistance;
        }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //:::::::::::::::::::::::::::::::::::Compute the derivative:::::::::::::::::::::::::::::::::://
        /// <summary>
        /// Computes the derivative of the signal according to "delta"
        /// "delta" should be of the same unit as the one for the "samplingRate"
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="samplingRate"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static double[] Derivative(double[] signal, int samplingRate, double delta)
        {
            // Convert delta to samples count
            int digDelta = (int)(samplingRate * delta) + ((samplingRate * delta) % 1 > 0 ? 1 : 0);
            // Devide delta to two parts prefDelta and suffDelta
            // prefDelta should be greater if necessary
            int prefDelta = digDelta / 2 + (digDelta % 2 > 0 ? 1 : 0);
            int suffDelta = digDelta - prefDelta;

            // Compute the derivative
            double[] derivative = new double[signal.Length];
            for (int i = 0; i < signal.Length; i++)
                if (prefDelta <= i && i + suffDelta < signal.Length)
                    derivative[i] = (signal[i + suffDelta] - signal[i - prefDelta]) / digDelta;

            return derivative;
        }

        //*******************************************************************************************************//
        //****************************************SIGNAL UP/DOWN SAMPLING****************************************//
        public static double[] UpDownSampling(double[] signal, int oldSamplingRate, int newSamplingRate)
        {
            // Create the NWaves signal
            NWaves.Signals.DiscreteSignal nWavesSignal = new NWaves.Signals.DiscreteSignal(oldSamplingRate, signal.Select(sample => (float)sample).ToArray());
            // Create the resampler
            NWaves.Operations.Resampler resampler = new NWaves.Operations.Resampler();
            // Resample the signal
            nWavesSignal = resampler.Resample(nWavesSignal, newSamplingRate);
            // Return the resampled signal as double[]
            return nWavesSignal.Samples.Select(sample => (double)sample).ToArray();
        }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //::::::::::::::::::::::::::::::::SERIALIZE/DESERIALIZE OBJECT::::::::::::::::::::::::::::::://
        // Convert an object to a byte array
        public static byte[] ObjectToByteArray<T>(T obj)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(T));

            using (var ms = new MemoryStream())
            {
                dcs.WriteObject(ms, obj);
                return ms.ToArray();
            }
        }

        // Convert a byte array to an object
        public static T ByteArrayToObject<T>(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);

                DataContractSerializer dcs = new DataContractSerializer(typeof(T));

                try
                {
                    return (T)dcs.ReadObject(memStream);
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e.Message);
                }
                return default;
            }
        }

        //*******************************************************************************************************//
        //*******************SEARCH FOR PARTICULAR STRING INSIDE ALL FILES OF A DIRECTORY************************//
        public static void searchForInDir(string searchingFor, string dirPath, string fileExtension)
        {
            searchingFor = searchingFor.ToLower();
            // Look for all directories inside the selected one
            int searchedDirs = 0;
            List<string> directories = new List<string>();
            directories.Add(dirPath);

            while (directories.Count > searchedDirs)
            {
                directories.AddRange(Directory.EnumerateDirectories(directories[searchedDirs]));
                searchedDirs += 1;
            }

            // Now search for the particular word inside all directories
            foreach (string directorie in directories)
                foreach (string file in Directory.EnumerateFiles(directorie, "*." + fileExtension))
                {
                    string contents = File.ReadAllText(file).ToLower();
                    if (contents.Contains(searchingFor))
                        Console.WriteLine(file);
                }
        }

        //*******************************************************************************************************//
        //****************Separate signals from WFDB as text to the specified signals as text********************//
        public static void TxtWFDBToTxtSignals(string txtFilePath, string[] signalsNames)
        {
            // Check if the file exists and is of txt extension
            FileInfo fileInfo = new FileInfo(txtFilePath);
            if (fileInfo.Exists && fileInfo.Extension.EndsWith("txt"))
            {
                string[] separatedNumbers;
                List<(string filePath, StringBuilder samples)> signalsList = new List<(string fileName, StringBuilder samples)>(signalsNames.Length);
                for (int i = 0; i < signalsNames.Length; i++)
                    signalsList.Add((GetFilePathWithoutExtenstion(fileInfo.FullName) + "_0" + (i + 1) + signalsNames[i] + ".txt", new StringBuilder()));
                // If yes then read lines of the file
                foreach (string line in File.ReadLines(fileInfo.FullName))
                {
                    // Separate the line according the existed spaces bewteen the characters
                    separatedNumbers = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    // Iterate through the parts of the line
                    // The first part is the sample number only
                    for (int i = 1; i < separatedNumbers.Length; i++)
                        signalsList[i - 1].samples.Append(separatedNumbers[i] + " ");
                }
                // Write the files of the separated signals
                foreach ((string filePath, StringBuilder samples) in signalsList)
                    if (samples.Length > 0)
                        File.WriteAllText(filePath, samples.ToString());
            }
        }
        public static string GetFilePathWithoutExtenstion(string filePath)
        {
            // Check if the file path has an extension
            if (Path.HasExtension(filePath))
                return filePath.Substring(0, filePath.LastIndexOf("."));
            return filePath;
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

        public static void Shuffle(DataTable dataTable)
        {
            Random rng = new Random();
            int n = dataTable.Rows.Count;
            while (n > 1)
            {
                int k = rng.Next(n--);
                DataRow temp = dataTable.Rows[n];
                foreach (DataColumn column in temp.Table.Columns)
                {
                    dataTable.Rows[n][column.ColumnName] = dataTable.Rows[k][column.ColumnName];
                    dataTable.Rows[k][column.ColumnName] = temp[column.ColumnName];
                }
            }
        }

        //*******************************************************************************************************//
        //*********************************************PERIOD AS STRING******************************************//
        public static string PeriodInSecToString(long period)
        {
            long days = period / 86400L;
            long hours = period % 86400L / 3600L;
            long minutes = period % 86400L % 3600 / 60;
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
        public static string PeriodInMillSecToString(long period)
        {
            long dayInMills = 86400000L;
            long hourInMills = 3600000L;
            long minuteInMills = 60000L;
            long secondInMills = 1000L;

            long days = period / dayInMills;
            long hours = period % dayInMills / hourInMills;
            long minutes = period % dayInMills % hourInMills / minuteInMills;
            long sec = period % minuteInMills / secondInMills;
            long mills = period % secondInMills;

            string timeToFinish = "";
            if (days != 0)
                timeToFinish += days + " days, ";
            if (hours != 0)
                timeToFinish += hours + " hours, ";
            if (minutes != 0)
                timeToFinish += minutes + " minutes, ";
            if (sec != 0)
                timeToFinish += sec + " sec, ";
            if (mills != 0)
                timeToFinish += mills + " mills";

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
        public static List<Sample> rearrangeFeaturesInput(List<Sample> dataLists, List<PCAitem> pcLoadingScores)
        {
            if (pcLoadingScores.Count > 0)
                for (int i = 0; i < dataLists.Count; i++)
                {
                    int selectedPCsNum = 0;
                    foreach (PCAitem pcaItem in pcLoadingScores)
                        if (pcaItem._selected)
                            selectedPCsNum++;
                    double[] inputBuff = new double[selectedPCsNum];
                    selectedPCsNum = 0;
                    foreach (PCAitem pcaItem in pcLoadingScores)
                        if (pcaItem._selected)
                        {
                            for (int k = 0; k < dataLists[i].getFeatures().Length; k++)
                                inputBuff[selectedPCsNum] += dataLists[i].getFeatures()[k] * pcaItem.EigenVector[k].loadingScore;
                            selectedPCsNum++;
                        }
                    dataLists[i].insertFeaturesArray(inputBuff);
                }
            return dataLists;
        }

        public static double[] rearrangeInput(double[] input, List<PCAitem> pcLoadingScores)
        {
            if (pcLoadingScores.Count > 0)
            {
                int selectedPCsNum = 0;
                foreach (PCAitem pcaItem in pcLoadingScores)
                    if (pcaItem._selected)
                        selectedPCsNum++;
                double[] inputBuff = new double[selectedPCsNum];
                selectedPCsNum = 0;
                foreach (PCAitem pcaItem in pcLoadingScores)
                    if (pcaItem._selected)
                    {
                        for (int k = 0; k < input.Length; k++)
                            inputBuff[selectedPCsNum] += input[k] * pcaItem.EigenVector[k].loadingScore;
                        selectedPCsNum++;
                    }
                input = inputBuff;
            }

            return input;
        }

        //*******************************************************************************************************//
        //**********************************Order list by string with numbers************************************//
        public static List<T> OrderByTextWithNumbers<T>(List<T> objectsList, List<string> namesList)
        {
            // Get elements to order from textsList
            List<(int originalOrder, List<object> elements)> elementsToOrder = GetTextsElements(namesList);

            // Order objectsList according to the number of elements in elementsToOrder
            List<T> orderedObjectsList = new List<T>();
            OrderByElements(elementsToOrder, objectsList, orderedObjectsList);

            return orderedObjectsList;
        }

        private static void OrderByElements<T>(List<(int originalOrder, List<object> elements)> elementsToOrder, List<T> originalObjectsList, List<T> orderedObjectsList)
        {
            // Group elements by variable type
            List<IGrouping<string, (int, List<object>)>> elementsGroupsByType = elementsToOrder.GroupBy(element => element.elements[0].GetType().Name).ToList();
            // Check if there exist more than one type
            if (elementsGroupsByType.Count > 1)
            {
                // If yes then order each group separated according to its type
                // Sort groups by key
                elementsGroupsByType = elementsGroupsByType.OrderBy(group => group.Key).ToList();
                foreach (IGrouping<string, (int, List<object>)> group in elementsGroupsByType)
                    OrderByElements(group.ToList(), originalObjectsList, orderedObjectsList);

                // return
                return;
            }

            // Start grouping elementsToOrder by the first element type
            List<IGrouping<object, (int, List<object>)>> elementsGroups = elementsToOrder.GroupBy(element => element.elements[0]).ToList();
            // Sort groups by key
            elementsGroups = elementsGroups.OrderBy(group => group.Key).ToList();
            // Reorder each group
            foreach (IGrouping<object, (int, List<object>)> group in elementsGroups)
            {
                // Remove the first sorting element from the group sorting elements
                List<(int, List<object>)> newElementsToOrder = new List<(int, List<object>)>();
                List<(int, List<object>)> elmentsToInsert = new List<(int, List<object>)>();
                foreach ((int originalOrder, List<object> elements) elements in group)
                {
                    if (elements.elements.Count > 1)
                    {
                        elements.elements.RemoveAt(0);
                        newElementsToOrder.Add(elements);
                    }
                    else
                        elmentsToInsert.Add(elements);
                }
                // Insert elmentsToInsert in orderedObjectsList
                foreach ((int originalOrder, List<object> elements) in elmentsToInsert)
                    orderedObjectsList.Add(originalObjectsList[originalOrder]);
                // Order the newElementsToOrder
                if (newElementsToOrder.Count > 0)
                    OrderByElements(newElementsToOrder, originalObjectsList, orderedObjectsList);
            }
        }

        private static List<(int originalOrder, List<object> elements)> GetTextsElements(List<string> textsList)
        {
            List<(int originalOrder, List<object> elements)> elementsToOrder = new List<(int, List<object>)>(textsList.Count);
            for (int i = 0; i < textsList.Count; i++)
            {
                string text = textsList[i];

                List<object> elements = new List<object>();
                bool isString = false, isDigit = false;
                StringBuilder stringBuilder = new StringBuilder();
                for (int j = 0; j < text.Length; j++)
                {
                    char c = text[j];
                    // Check if the previous char type was string and is different than current one
                    if (char.IsDigit(c) && isString)
                    {
                        // If yes then insert previous string in elements
                        elements.Add(stringBuilder.ToString());
                        // Clear the string builder
                        stringBuilder.Clear();
                        isString = false;
                    }
                    // Check if the previous char type was digit and is different than current one
                    if (!char.IsDigit(c) && isDigit)
                    {
                        // If yes then insert previous number in elements
                        elements.Add(int.Parse(stringBuilder.ToString()));
                        // Clear the string builder
                        stringBuilder.Clear();
                        isDigit = false;
                    }

                    // Update stringBuilder
                    stringBuilder.Append(c);
                    if (char.IsDigit(c))
                        isDigit = true;
                    else
                        isString = true;

                    // Check if this is the last character
                    if (j == text.Length - 1)
                        if (isDigit)
                            elements.Add(int.Parse(stringBuilder.ToString()));
                        else if (isString)
                            elements.Add(stringBuilder.ToString());
                }
                // Insert text elements in elementsToOrder
                elementsToOrder.Add((i, elements));
            }
            return elementsToOrder;
        }

        //*******************************************************************************************************//
        //****************************************Save chart as HD image*****************************************//
        public static void saveChartAsImage(FormsPlot signalChart)
        {
            // Open file dialogue to choose the path where to save the image
            using (SaveFileDialog sfd = new SaveFileDialog() { Title = "Save an Image File", ValidateNames = true, Filter = "PNG Image|*.png", RestoreDirectory = true })
            {
                // Check if the user clicked OK button
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // If yes then save teh chart in the selected path

                    // Get the path of specified file
                    string filePath = sfd.FileName;

                    // Scale the size of the chart
                    int scaling = 20;
                    FormsPlot scaledChart = new FormsPlot();
                    scaledChart.Size = signalChart.Size;
                    scaledChart.Plot.XLabel(signalChart.Plot.XAxis.AxisLabel.Label);
                    scaledChart.Plot.YLabel(signalChart.Plot.YAxis.AxisLabel.Label);
                    scaledChart.Plot.SetAxisLimits(signalChart.Plot.GetAxisLimits());
                    Legend legend = scaledChart.Plot.Legend();
                    legend.Orientation = ScottPlot.Orientation.Horizontal;
                    foreach (IPlottable plottable in signalChart.Plot.GetPlottables())
                    {
                        if (plottable is ScatterPlot scatterPlot)
                        {
                            ScatterPlot scalScatterPlot = scaledChart.Plot.AddScatter(scatterPlot.Xs, scatterPlot.Ys, scatterPlot.Color, (float)scatterPlot.LineWidth,
                                                                                                                                         scatterPlot.MarkerSize * 3 / 4,
                                                                                                                                         scatterPlot.MarkerShape, scatterPlot.LineStyle, scatterPlot.Label);
                            scalScatterPlot.OffsetX = scatterPlot.OffsetX;
                            scalScatterPlot.DataPointLabels = scatterPlot.DataPointLabels;
                            scalScatterPlot.DataPointLabelFont = scatterPlot.DataPointLabelFont;
                            scalScatterPlot.DataPointLabelFont.Size = scalScatterPlot.DataPointLabelFont.Size * 3 / 4;
                            scalScatterPlot.IsVisible = scatterPlot.IsVisible;
                        }
                        // Convert signals to scatterplots to keep their resolution
                        else if (plottable is SignalPlot signalPlot)
                        {
                            double[] Xs = new double[signalPlot.Ys.Length];
                            for (int i = 0; i < Xs.Length; i++)
                                Xs[i] = i / signalPlot.SampleRate;
                            ScatterPlot scalScatterPlot = scaledChart.Plot.AddScatter(Xs, signalPlot.Ys, signalPlot.Color, (float)signalPlot.LineWidth,
                                                                                                                                         signalPlot.MarkerSize,
                                                                                                                                         signalPlot.MarkerShape, signalPlot.LineStyle, signalPlot.Label);
                            scalScatterPlot.OffsetX = signalPlot.OffsetX;
                            scalScatterPlot.IsVisible = signalPlot.IsVisible;
                        }
                    }

                    // Save the image from the scaled chart
                    scaledChart.Plot.SaveFig(filePath, scale: scaling);
                }
            }
        }
    }
}

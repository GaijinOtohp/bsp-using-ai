using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.DetailsModify.FiltersControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace Biological_Signal_Processing_Using_AI.DetailsModify.Filters
{
    public class CornersScanner : FilterBase
    {
        public class CornerSample
        {
            public int _index;
            public double _value;

            public double _prevTan;
            public double _nextTan;
            public double _deviationAngle; // Argument

            public double _prevMag;
            public double _nextMag;

            public CornerSample Clone()
            {
                return new CornerSample()
                {
                    _index = _index,
                    _value = _value,
                    _prevTan = _prevTan,
                    _nextTan = _nextTan,
                    _deviationAngle = _deviationAngle,
                    _prevMag = _prevMag,
                    _nextMag = _nextMag
                };
            }
        }

        public class CornerInterval
        {
            public string Name;

            public int starting;
            public int ending;

            public int cornerIndex;
        }

        private double[] SpanSamples;

        private int _scanStartingIndex { get; set; } = 0;

        public bool _autoApply { get; set; } = true;
        public bool _showAngles { get; set; } = false;

        private bool _forSelectionBubbles = false;
        public delegate void SelectAllTypesPoints();
        public SelectAllTypesPoints _SelectAllTypesPointsDelegate;

        public double _art { get; set; } = 0.2; // Amplitude ratio threshold
        public double _at { get; set; } = 20; // Angle threshold

        public List<CornerSample> _CornersList { get; set; }

        public override CornersScanner Clone(FilteringTools filteringTools)
        {
            // Clone filter properties
            CornersScanner clonedCornersScanner = new CornersScanner(filteringTools);
            clonedCornersScanner.SpanSamples = (double[])SpanSamples.Clone();
            clonedCornersScanner._scanStartingIndex = _scanStartingIndex;
            clonedCornersScanner._autoApply = _autoApply;
            clonedCornersScanner._showAngles = _showAngles;
            clonedCornersScanner._forSelectionBubbles = _forSelectionBubbles;
            clonedCornersScanner._art = _art;
            clonedCornersScanner._at = _at;

            clonedCornersScanner._CornersList = new List<CornerSample>(_CornersList.Count);
            foreach (CornerSample corner in _CornersList)
                clonedCornersScanner._CornersList.Add(corner.Clone());
            clonedCornersScanner.CloneBase(this);
            // CLone the control
            if (_FilterControl != null)
            {
                clonedCornersScanner._FilterControl = new CornersScannerUserControl(clonedCornersScanner);
                clonedCornersScanner.ActivateAutoApply(_autoApply);
                clonedCornersScanner.SetART(_art);
                clonedCornersScanner.SetAT(_at);
                clonedCornersScanner.ActivateGenerally(_activated);
            }
            return clonedCornersScanner;
        }

        public CornersScanner(FilteringTools parentFilteringTools)
        {
            _ParentFilteringTools = parentFilteringTools;
            Name = GetType().Name;
        }
        public override Control InitializeFilterControl()
        {
            return new CornersScannerUserControl(this);
        }
        public override (double[] filteredSignal, bool reloadSignal) ApplyFilter(double[] filteredSamples, bool forceApply, bool showResultsInChart)
        {
            if (this._autoApply || forceApply)
            {
                if (_forSelectionBubbles && SpanSamples != null)
                {
                    this.ScanCorners(SpanSamples);
                    _SelectAllTypesPointsDelegate();
                }
                else if (!_forSelectionBubbles)
                {
                    this.ScanCorners(filteredSamples);
                    // Show states in the chart
                    if (this._FilterControl != null && showResultsInChart)
                        if (this._FilterControl.IsHandleCreated) ((CornersScannerUserControl)this._FilterControl).showSignalCorners(this._CornersList);
                }
            }
            return (filteredSamples, false);
        }
        public override void Activate(bool activate)
        {
            ((CornersScannerUserControl)_FilterControl).showCornersCheckBox.Checked = activate;
        }

        public void SetART(double art)
        {
            if (art < 0 || art > 1)
                return;
            _art = art;
            // Update the control
            UpdateControl();
            _ParentFilteringTools?.ApplyFilters(false);
        }
        public void SetAT(double at)
        {
            if (at < 0 || at > 360)
                return;
            _at = at;
            // Update the control
            UpdateControl();
            _ParentFilteringTools?.ApplyFilters(false);
        }
        public void ActivateAutoApply(bool activate)
        {
            _autoApply = activate;
            // Update the control
            UpdateControl();
            _ParentFilteringTools?.ApplyFilters(false);
        }
        public void ShowAngles(bool show)
        {
            _showAngles = show;
            // Update the control
            UpdateControl();
            _ParentFilteringTools?.ApplyFilters(false);
        }
        public void SetScanStartIndex(int startingIndex)
        {
            _scanStartingIndex = startingIndex;
        }
        public void SetSpanSamples(double[] spanSamples)
        {
            SpanSamples = spanSamples;
            _ParentFilteringTools?.ApplyFilters(false);
        }
        public void SetForSelectionBubbles(bool activate, SelectAllTypesPoints selectAllTypesPointsDelegate)
        {
            _forSelectionBubbles = activate;
            _SelectAllTypesPointsDelegate = selectAllTypesPointsDelegate;
        }

        public void UpdateControl()
        {
            if (_FilterControl != null)
            {
                CornersScannerUserControl cornersScanner = (CornersScannerUserControl)_FilterControl;
                if (!_ignoreEvent)
                {
                    _ignoreEvent = true;
                    cornersScanner.artScrollBar.Value = cornersScanner.artScrollBar.GetMax() - (int)(_art * cornersScanner.artScrollBar.GetMax());
                    cornersScanner.artValueTextBox.Text = Math.Round(_art, 3).ToString();
                    cornersScanner.angleThresholdScrollBar.Value = (int)(_at * 10);
                    cornersScanner.atValueTextBox.Text = Math.Round(_at, 2).ToString();
                    cornersScanner.autoApplyCheckBox.Checked = _autoApply;
                    cornersScanner.showDeviationCheckBox.Checked = _showAngles;
                    _ignoreEvent = false;
                }
            }
        }

        //------------------------------------------------------------------------------//
        private static (double mag, double tan) MagTan(CornerSample beginningSample, CornerSample endingSample, double samplingRate)
        {
            double xDiff = (endingSample._index - beginningSample._index) / samplingRate;
            double yDiff = endingSample._value - beginningSample._value;

            double mag = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
            double tan = yDiff / xDiff;

            return (mag, tan);
        }

        public List<CornerSample> ScanCorners(double[] samples)
        {
            _CornersList = ScanCorners(samples, _scanStartingIndex, (double)this._ParentFilteringTools._samplingRate, _art, _at);

            return _CornersList;
        }
        public static List<CornerSample> ScanCorners(double[] samples, int scanStartingIndex, double samplingRate, double art, double at)
        {
            List<CornerSample> cornersList = new List<CornerSample>();
            double amplitudeInterval = GeneralTools.amplitudeInterval(samples);

            CornerSample[] corners = new CornerSample[samples.Length];
            CornerSample latestCorner = null;


            for (int i = 0; i < samples.Length; i++)
            {
                // Set current sample
                // samples is an excerpt from the the _OriginalRawSamples.
                // That's why "_scanStartingIndex" is added as the padding of the excerpt
                corners[i] = new CornerSample { _index = scanStartingIndex + i, _value = samples[i] };

                if (i == 0)
                    latestCorner = corners[0];

                // Get the index of the last corner without the padding
                int latCorShiftIndx = latestCorner._index - scanStartingIndex;

                // Compute _prevMag and _prevTan of the current sample
                if (i - latCorShiftIndx > 0)
                    (corners[i]._prevMag, corners[i]._prevTan) = MagTan(latestCorner, corners[i], samplingRate);

                // Check if the current sample is two indexes ahead of the latest corner
                if (i - latCorShiftIndx > 1)
                {
                    // Update _nextMag, _nextTan, and _deviationAngle of the samples between the latest state and the current sample
                    for (int j = latCorShiftIndx + 1; j < i; j++)
                    {
                        (corners[j]._nextMag, corners[j]._nextTan) = MagTan(corners[j], corners[i], samplingRate);

                        corners[j]._deviationAngle = (Math.Atan(corners[j]._nextTan) - Math.Atan(corners[j]._prevTan)) * 180 / Math.PI;
                    }

                    // Select the samples with the angle deviation that exceeds angThreshold
                    // and both of _prevMeanMag and _nextMeanMag exceeds amplitudeInterval * magThreshold
                    CornerSample[] selectedSamples = corners.Select((corner, index) => (corner, index)).Where(tuple => tuple.index > latCorShiftIndx && tuple.index < i).
                                                                                           Where(tuple => tuple.corner._nextMag > amplitudeInterval * art
                                                                                           && tuple.corner._prevMag > amplitudeInterval * art
                                                                                           && Math.Abs(tuple.corner._deviationAngle) > at).
                                                                                           Select(tuple => tuple.corner).ToArray();

                    // Check if there is any selected samples that fulfills the conditions
                    if (selectedSamples.Length > 0)
                    {
                        // Select the one with the largest segments
                        cornersList.Add(selectedSamples.OrderByDescending(corner => corner._prevMag + corner._nextMag).ToArray()[0]);
                        latestCorner = cornersList[cornersList.Count - 1];

                        // If new corner is created
                        // then update all previousMag and _prevTan of the new corner's next samples
                        latCorShiftIndx = latestCorner._index - scanStartingIndex;
                        for (int j = 1; j <= i - latCorShiftIndx; j++)
                            (corners[j + latCorShiftIndx]._prevMag, corners[j + latCorShiftIndx]._prevTan) = MagTan(latestCorner, corners[j + latCorShiftIndx], samplingRate);
                    }
                }
            }


            return cornersList;
        }

        public static List<CornerInterval> ApproximateIndexesToIntervals(AnnotationECG[] cornersIndexes, double tolerance, double[] fullSignal, int samplingRate)
        {
            // There might be two corners on the same index
            // They should have the same interval but with their unique label
            Dictionary<int, CornerInterval> intervalsDict = new Dictionary<int, CornerInterval>();
            List<CornerInterval> intervals = new List<CornerInterval>();

            for (int i = 0; i < cornersIndexes.Length; i++)
            {
                CornerInterval indexInterval = new CornerInterval() { cornerIndex = cornersIndexes[i].GetIndexes().starting, Name = cornersIndexes[i].Name };
                // Check if the interval of the current corner already exists
                if (intervalsDict.ContainsKey(indexInterval.cornerIndex))
                {
                    // If yes then just clounn the corner interval but with the different naming
                    indexInterval.starting = intervalsDict[indexInterval.cornerIndex].starting;
                    indexInterval.ending = intervalsDict[indexInterval.cornerIndex].ending;
                }
                else
                {
                    // Create a new interval for the corner
                    // Get the index of the next and previous corners if exists
                    int[] prevCorIndexArray = cornersIndexes.Where(ecgAnno => ecgAnno.GetIndexes().starting < indexInterval.cornerIndex).Select(ecgAnno => ecgAnno.GetIndexes().starting).ToArray();
                    int[] nextCorIndexArray = cornersIndexes.Where(ecgAnno => ecgAnno.GetIndexes().starting > indexInterval.cornerIndex).Select(ecgAnno => ecgAnno.GetIndexes().starting).ToArray();
                    if (prevCorIndexArray.Length > 0)
                    {
                        // Compute the distance of the tolerance and find its index
                        double prevDistAmpTol = Math.Sqrt(Math.Pow((indexInterval.cornerIndex - prevCorIndexArray.Max()) / (double)samplingRate, 2) + Math.Pow(fullSignal[indexInterval.cornerIndex] - fullSignal[prevCorIndexArray.Max()], 2)) *
                                                tolerance / 100d;
                        // Compute the starting index using prevDistAmpTol and the ending index "indexInterval.cornerIndex"
                        // by computing distances from the latest corner in prevCorIndexArray
                        double distDiff = double.PositiveInfinity;
                        for (int iCornIndex = prevCorIndexArray.Max(); iCornIndex < indexInterval.cornerIndex; iCornIndex++)
                        {
                            double iDistAmp = Math.Sqrt(Math.Pow((indexInterval.cornerIndex - iCornIndex) / (double)samplingRate, 2) + Math.Pow(fullSignal[indexInterval.cornerIndex] - fullSignal[iCornIndex], 2));
                            if (Math.Abs(iDistAmp - prevDistAmpTol) < distDiff)
                            {
                                indexInterval.starting = iCornIndex;
                                distDiff = Math.Abs(iDistAmp - prevDistAmpTol);
                            }
                        }
                    }
                    else
                        indexInterval.starting = indexInterval.cornerIndex - (int)(tolerance * (indexInterval.cornerIndex - 0) / 100f);

                    if (nextCorIndexArray.Length > 0)
                    {
                        // Compute the distance of the tolerance and find its index
                        double nextDistAmpTol = Math.Sqrt(Math.Pow((nextCorIndexArray.Min() - indexInterval.cornerIndex) / (double)samplingRate, 2) + Math.Pow(fullSignal[nextCorIndexArray.Min()] - fullSignal[indexInterval.cornerIndex], 2)) *
                                                tolerance / 100d;
                        // Compute the ending index using prevDistAmpTol and the staring index "indexInterval.cornerIndex"
                        // by computing distances from the latest corner in nextCorIndexArray
                        double distDiff = double.PositiveInfinity;
                        for (int iCornIndex = indexInterval.cornerIndex + 1; iCornIndex <= nextCorIndexArray.Min(); iCornIndex++)
                        {
                            double iDistAmp = Math.Sqrt(Math.Pow((iCornIndex - indexInterval.cornerIndex) / (double)samplingRate, 2) + Math.Pow(fullSignal[iCornIndex] - fullSignal[indexInterval.cornerIndex], 2));
                            if (Math.Abs(iDistAmp - nextDistAmpTol) < distDiff)
                            {
                                indexInterval.ending = iCornIndex;
                                distDiff = Math.Abs(iDistAmp - nextDistAmpTol);
                            }
                        }
                    }
                    else
                        indexInterval.ending = indexInterval.cornerIndex + (int)(tolerance * ((fullSignal.Length - 1) - indexInterval.cornerIndex) / 100f);

                    // Add the new interval in the dictionary
                    intervalsDict.Add(indexInterval.cornerIndex, indexInterval);
                }
                // Add the corner's interval to the list
                intervals.Add(indexInterval);
            }

            return intervals;
        }
    }
}

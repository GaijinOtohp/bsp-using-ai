using Biological_Signal_Processing_Using_AI.DetailsModify.FiltersControls;
using Biological_Signal_Processing_Using_AI.Garage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace Biological_Signal_Processing_Using_AI.DetailsModify.Filters
{
    public class AddGWN : FilterBase
    {
        private double _snrDB = 10;

        public override AddGWN Clone(FilteringTools filteringTools)
        {
            // Clone filter properties
            AddGWN clonedAddGWN = new AddGWN(filteringTools);
            clonedAddGWN.CloneBase(this);
            // CLone the control
            if (_FilterControl != null)
            {
                clonedAddGWN._FilterControl = new AddGWNUserControl(clonedAddGWN);
                clonedAddGWN.ActivateGenerally(_activated);
            }
            return clonedAddGWN;
        }

        public AddGWN(FilteringTools parentFilteringTools)
        {
            _ParentFilteringTools = parentFilteringTools;
            Name = GetType().Name;
        }
        public override Control InitializeFilterControl()
        {
            return new AddGWNUserControl(this);
        }
        public override (double[] filteredSignal, bool reloadSignal) ApplyFilter(double[] filteredSamples, bool forceApply, bool showResultsInChart)
        {
            if (_activated)
                return (AddGaussianWhiteNoise(filteredSamples, _snrDB), true);
            else
                return (filteredSamples, true);
        }
        public override void Activate(bool activate)
        {
            ((AddGWNUserControl)_FilterControl).activateCheckBox.Checked = activate;
        }

        public double SNRdb
        {
            get { return _snrDB; }
            set
            {
                _snrDB = value;

                _ParentFilteringTools?.ApplyFilters(false);
                
            }
        }

        public static double[] AddGaussianWhiteNoise(double[] samples, double snrDB)
        {
            double[] noisySamples = new double[samples.Length];

            // Convert snrDB to snr
            // since snrDB = 10 * log10(snr)
            //double snr = Math.Exp(snrDB / 10 * Math.Log(10));
            double snr = Math.Pow(10, snrDB / 10);

            // Compute the variance of the signal
            double vs = GeneralTools.Variance(samples, GeneralTools.Mean(samples));

            // Compute the gaussian white noise
            // Since snr = vs / vn (variance of the signal / variance of the noise)
            // in DB snrDB = vsDB - vnDB
            double vn = vs / snr;
            // Gaussian white noise
            double[] gwn = GeneralTools.GaussianWhiteNoise(0, vn, samples.Length);

            // Add the nosie to the signal
            noisySamples = samples.Select((samp, index) => samp + gwn[index]).ToArray();

            return noisySamples;
        }
    }
}

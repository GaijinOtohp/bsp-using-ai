using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.DetailsModify.FiltersControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace Biological_Signal_Processing_Using_AI.DetailsModify.Filters
{
    public class Normalize : FilterBase
    {
        public override Normalize Clone(FilteringTools filteringTools)
        {
            // Clone filter properties
            Normalize clonedNormalize = new Normalize(filteringTools);
            clonedNormalize.CloneBase(this);
            // CLone the control
            if (_FilterControl != null)
            {
                clonedNormalize._FilterControl = new NormalizedSignalUserControl(clonedNormalize);
                clonedNormalize.ActivateGenerally(_activated);
            }
            return clonedNormalize;
        }

        public Normalize(FilteringTools parentFilteringTools)
        {
            _ParentFilteringTools = parentFilteringTools;
            Name = GetType().Name;
        }
        public override Control InitializeFilterControl()
        {
            return new NormalizedSignalUserControl(this);
        }
        public override (double[] filteredSignal, bool reloadSignal) ApplyFilter(double[] filteredSamples, bool forceApply, bool showResultsInChart)
        {
            return (GeneralTools.normalizeSignal(filteredSamples), true);
        }
        public override void Activate(bool activate)
        {
            ((NormalizedSignalUserControl)_FilterControl).normalizeSignalCheckBox.Checked = activate;
        }
    }
}

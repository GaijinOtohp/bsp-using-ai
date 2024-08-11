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
    public class Absolute : FilterBase
    {
        public override Absolute Clone(FilteringTools filteringTools)
        {
            // Clone filter properties
            Absolute clonedAbsolute = new Absolute(filteringTools);
            clonedAbsolute.CloneBase(this);
            // CLone the control
            if (_FilterControl != null)
            {
                clonedAbsolute._FilterControl = new AbsoluteSignalUserControl(clonedAbsolute);
                clonedAbsolute.ActivateGenerally(_activated);
            }
            return clonedAbsolute;
        }

        public Absolute(FilteringTools parentFilteringTools)
        {
            _ParentFilteringTools = parentFilteringTools;
            Name = GetType().Name;
        }
        public override Control InitializeFilterControl()
        {
            return new AbsoluteSignalUserControl(this);
        }
        public override (double[] filteredSignal, bool reloadSignal) ApplyFilter(double[] filteredSamples, bool forceApply, bool showResultsInChart)
        {
            return (GeneralTools.absoluteSignal(filteredSamples), true);
        }
        public override void Activate(bool activate)
        {
            ((AbsoluteSignalUserControl)_FilterControl).absoluteSignalCheckBox.Checked = activate;
        }
    }
}

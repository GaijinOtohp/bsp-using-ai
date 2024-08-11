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
    public class DCRemoval : FilterBase
    {
        public override DCRemoval Clone(FilteringTools filteringTools)
        {
            // Clone filter properties
            DCRemoval clonedDCRemoval = new DCRemoval(filteringTools);
            clonedDCRemoval.CloneBase(this);
            // CLone the control
            if (_FilterControl != null)
            {
                clonedDCRemoval._FilterControl = new DCRemovalUserControl(clonedDCRemoval);
                clonedDCRemoval.ActivateGenerally(_activated);
            }
            return clonedDCRemoval;
        }

        public DCRemoval(FilteringTools parentFilteringTools)
        {
            _ParentFilteringTools = parentFilteringTools;
            Name = GetType().Name;
        }
        public override Control InitializeFilterControl()
        {
            return new DCRemovalUserControl(this);
        }
        public override (double[] filteredSignal, bool reloadSignal) ApplyFilter(double[] filteredSamples, bool forceApply, bool showResultsInChart)
        {
            return (GeneralTools.removeDCValue(filteredSamples), true);
        }
        public override void Activate(bool activate)
        {
            ((DCRemovalUserControl)_FilterControl).dcValueRemoveCheckBox.Checked = activate;
        }
    }
}

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
    public class ExistenceDeclare : FilterBase
    {
        public int _exists { get; set; } = 0;
        public string _Label { get; set; }

        public override ExistenceDeclare Clone(FilteringTools filteringTools)
        {
            // Clone filter properties
            ExistenceDeclare existanceDeclare = new ExistenceDeclare(filteringTools, _Label);
            existanceDeclare.CloneBase(this);
            // CLone the control
            if (_FilterControl != null)
            {
                existanceDeclare._FilterControl = new CheckExistenceUserControl(existanceDeclare);
                existanceDeclare.ActivateGenerally(_activated);
            }
            return existanceDeclare;
        }

        public ExistenceDeclare(FilteringTools parentFilteringTools, string label)
        {
            _ParentFilteringTools = parentFilteringTools;
            _Label = label;
            Name = GetType().Name;
        }
        public override Control InitializeFilterControl()
        {
            return new CheckExistenceUserControl(this);
        }
        public override (double[] filteredSignal, bool reloadSignal) ApplyFilter(double[] filteredSamples, bool forceApply, bool showResultsInChart)
        {
            return (filteredSamples, false);
        }
        public override void Activate(bool activate)
        {

        }

        public void SetExistance(bool exists)
        {
            _exists = Convert.ToInt32(exists);
            if (!_ignoreEvent)
            {
                _ignoreEvent = true;
                ((CheckExistenceUserControl)_FilterControl).existenceOfCheckBox.Checked = exists;
                _ignoreEvent = false;
            }
        }
    }
}

using Biological_Signal_Processing_Using_AI.DetailsModify.Filters;
using System;
using System.Windows.Forms;

namespace BSP_Using_AI.DetailsModify.FiltersControls
{
    public partial class AbsoluteSignalUserControl : UserControl
    {
        Absolute _absolute;
        public AbsoluteSignalUserControl(Absolute absolute)
        {
            InitializeComponent();
            _absolute = absolute;

            Name = absolute.Name;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _absolute.RemoveFilter();
        }

        private void dcValueRemoveCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            // Update _peaksAnalyzer
            if (!_absolute._ignoreEvent)
            {
                _absolute._ignoreEvent = true;
                _absolute.ActivateGenerally(absoluteSignalCheckBox.Checked);
                _absolute._ignoreEvent = false;
            }
        }
    }
}

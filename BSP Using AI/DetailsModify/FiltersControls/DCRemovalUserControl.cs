using System;
using System.Windows.Forms;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace BSP_Using_AI.DetailsModify.FiltersControls
{
    public partial class DCRemovalUserControl : UserControl
    {
        DCRemoval _dCRemoval;

        public DCRemovalUserControl(DCRemoval dCRemoval)
        {
            InitializeComponent();
            _dCRemoval = dCRemoval;

            Name = dCRemoval.Name;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _dCRemoval.RemoveFilter();
        }

        private void dcValueRemoveCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            // Update _peaksAnalyzer
            if (!_dCRemoval._ignoreEvent)
            {
                _dCRemoval._ignoreEvent = true;
                _dCRemoval.ActivateGenerally(dcValueRemoveCheckBox.Checked);
                _dCRemoval._ignoreEvent = false;
            }
        }
    }
}

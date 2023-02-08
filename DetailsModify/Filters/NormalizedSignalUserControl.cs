using System;
using System.Windows.Forms;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace BSP_Using_AI.DetailsModify.Filters
{
    public partial class NormalizedSignalUserControl : UserControl
    {
        Normalize _normalize;
        public NormalizedSignalUserControl(Normalize normalize)
        {
            InitializeComponent();
            _normalize = normalize;

            Name = normalize.Name;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _normalize.RemoveFilter();
        }

        private void dcValueRemoveCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            // Update _peaksAnalyzer
            if (!_normalize._ignoreEvent)
            {
                _normalize._ignoreEvent = true;
                _normalize.ActivateGenerally(normalizeSignalCheckBox.Checked);
                _normalize._ignoreEvent = false;
            }
        }
    }
}

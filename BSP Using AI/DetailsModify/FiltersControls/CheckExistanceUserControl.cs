using Biological_Signal_Processing_Using_AI.DetailsModify.Filters;
using System.Windows.Forms;

namespace BSP_Using_AI.DetailsModify.FiltersControls
{
    public partial class CheckExistenceUserControl : UserControl
    {
        ExistenceDeclare _ExistanceDeclare;

        public CheckExistenceUserControl(ExistenceDeclare existanceDeclare)
        {
            InitializeComponent();
            _ExistanceDeclare = existanceDeclare;
            existenceOfCheckBox.Text = _ExistanceDeclare._Label;
        }

        private void existenceOfCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_ExistanceDeclare._ignoreEvent)
            {
                _ExistanceDeclare._ignoreEvent = true;
                _ExistanceDeclare.SetExistance(existenceOfCheckBox.Checked);
                _ExistanceDeclare._ignoreEvent = false;
            }
        }
    }
}

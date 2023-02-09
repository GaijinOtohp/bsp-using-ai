using System.Windows.Forms;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace BSP_Using_AI.DetailsModify.Filters
{
    public partial class CheckExistanceUserControl : UserControl
    {
        ExistanceDeclare _ExistanceDeclare;

        public CheckExistanceUserControl(ExistanceDeclare existanceDeclare)
        {
            InitializeComponent();
            _ExistanceDeclare = existanceDeclare;
            existanceOfCheckBox.Text = _ExistanceDeclare._Label;
        }

        private void existanceOfCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_ExistanceDeclare._ignoreEvent)
            {
                _ExistanceDeclare._ignoreEvent = true;
                _ExistanceDeclare.SetExistance(existanceOfCheckBox.Checked);
                _ExistanceDeclare._ignoreEvent = false;
            }
        }
    }
}

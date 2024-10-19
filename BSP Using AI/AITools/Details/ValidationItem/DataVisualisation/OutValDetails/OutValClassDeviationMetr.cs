using System.Windows.Forms;

namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation.OutValDetails
{
    public partial class OutValClassDeviationMetr : UserControl
    {
        public OutValClassDeviationMetr()
        {
            InitializeComponent();
        }

        private void classificationThresholdTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.keypressNumbersAndDecimalOnly(sender, e);
        }
    }
}

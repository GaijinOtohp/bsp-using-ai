using System.Windows.Forms;

namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation.OutValDetails
{
    public partial class OutValClassAccF1ScoreMetr : UserControl
    {
        public OutValClassAccF1ScoreMetr()
        {
            InitializeComponent();
        }

        private void classificationThresholdTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.keypressNumbersAndDecimalOnly(sender, e);
        }
    }
}

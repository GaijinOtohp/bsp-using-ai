using System;
using System.Windows.Forms;

namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation
{
    public partial class RawVisItemUserControl : UserControl
    {
        public RawVisItemUserControl(string outputLabel)
        {
            InitializeComponent();

            outputCheckBox.Text = outputLabel;
        }

        private void primaryColorButton_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                (sender as Button).BackColor = colorDialog.Color;

                // Refresh chart
                ((DataVisualisationForm)this.FindForm()).refreshRawChart();
            }
        }

        private void outputCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Refresh chart
            ((DataVisualisationForm)this.FindForm()).refreshRawChart();
        }
    }
}

using Biological_Signal_Processing_Using_AI.Garage;
using System;
using System.Windows.Forms;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace BSP_Using_AI.MainFormFolder.SignalsCollectionFolder
{
    public partial class UserControlSignalPower : UserControl
    {
        FilteringTools _FilteringTools;

        bool _mouseDown = false;
        int _previousMouseX;
        int _previousMouseY;

        public UserControlSignalPower(FilteringTools filteringTools)
        {
            InitializeComponent();

            // Initialize variables
            _FilteringTools = filteringTools;

            // Set plots titles and labels
            signalExhibitor.Plot.XAxis.Label("Time (s)");
            signalExhibitor.Plot.YAxis.Label("Voltage (mV)");
            signalExhibitor.Refresh();

            // Calculate power and set it in signalPowerValueLabel
            double signalPower = 0D;
            foreach (double sample in filteringTools._FilteredSamples)
                signalPower += Math.Pow(sample, 2) / filteringTools._FilteredSamples.Length;

            signalPowerValueLabel.Text = Math.Round(signalPower, 5).ToString();

            // Insert signal in chart
            GeneralTools.loadSignalInChart(signalExhibitor, filteringTools._FilteredSamples, filteringTools._samplingRate, 0, "UserControlSignalPower");
        }

        private void sendSignalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EventHandlers.sendSignalTool(_FilteringTools, pathLabel.Text + "\\Collector");
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Remove this User control from its parent flow layout panel
            ((FlowLayoutPanel)this.Parent).Controls.Remove(this);
        }

        private void signalExhibitor_MouseEnter(object sender, EventArgs e)
        {
            ((FormSignalsCollector)FindForm()).scrollAllowed = false;
        }

        private void signalExhibitor_MouseLeave(object sender, EventArgs e)
        {
            ((FormSignalsCollector)FindForm()).scrollAllowed = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace BSP_Using_AI.MainFormFolder.SignalsCollectionFolder
{
    public partial class UserControlSignalPower : UserControl
    {
        private double[] _samples;

        private double _samplingRate;
        private double _quantizationStep;

        bool _mouseDown = false;
        int _previousMouseX;
        int _previousMouseY;

        public UserControlSignalPower(double[] samples, double samplingRate, double quantizationStep)
        {
            InitializeComponent();

            // Initialize variables
            _samples = samples;
            _samplingRate = samplingRate;
            _quantizationStep = quantizationStep;

            // Calculate power and set it in signalPowerValueLabel
            double signalPower = 0D;
            foreach (double sample in _samples)
                signalPower += Math.Abs(sample * sample) / _samples.Length;

            signalPowerValueLabel.Text = Math.Round(signalPower, 5).ToString();

            // Insert signal in chart
            Garage.loadSignalInChart((Chart)Controls.Find("signalExhibitor", false)[0], _samples, _samplingRate, _quantizationStep, 0, "UserControlSignalPower");
        }

        private void sendSignalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EventHandlers.sendSignalTool(_samples, _samplingRate, _quantizationStep, pathLabel.Text + "\\Collector");
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Remove this User control from its parent flow layout panel
            ((FlowLayoutPanel)this.Parent).Controls.Remove(this);
        }

        private void signalExhibitor_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;
            _previousMouseX = e.X;
            _previousMouseY = e.Y;
        }

        private void signalExhibitor_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown)
            {
                EventHandlers.signalExhibitor_MouseMove(sender, e, _previousMouseX, _previousMouseY);
                _previousMouseX = e.X;
                _previousMouseY = e.Y;
            }
        }

        private void signalExhibitor_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }

        private void signalExhibitor_MouseWheel(object sender, MouseEventArgs e)
        {
            EventHandlers.signalExhibitor_MouseWheel(sender, e, _previousMouseX, _previousMouseY);
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

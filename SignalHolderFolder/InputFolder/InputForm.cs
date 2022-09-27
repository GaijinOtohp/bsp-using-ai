using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BSP_Using_AI.SignalHolderFolder.Input
{
    public partial class InputForm : Form
    {
        public String _filePath;
        public SignalHolder _currentSignalHolder;

        public InputForm(String title, List<String[]> inputValuesList)
        {
            InitializeComponent();

            // Set the title of the requested input values
            titleLabel.Text = title;

            // Sort the requested values from inputValuesArray in inputFlowLayoutPanel
            foreach (String[] input in inputValuesList)
            {
                // Create a new InputValueUserControl
                // for the requested value to input
                InputValueUserControl inputValueUserControl = new InputValueUserControl();
                inputValueUserControl.inputLabel.Text = input[0];

                // Add the new user control in inputFlowLayoutPanel
                inputFlowLayoutPanel.Controls.Add(inputValueUserControl);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            EventHandlers.okButton_Click(sender, e);
        }
    }
}

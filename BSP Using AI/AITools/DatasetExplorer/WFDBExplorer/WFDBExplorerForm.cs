using Biological_Signal_Processing_Using_AI.WFDB;
using BSP_Using_AI;
using BSP_Using_AI.AITools.DatasetExplorer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.WFDB.WFBDDefinitions;

namespace Biological_Signal_Processing_Using_AI.AITools.DatasetExplorer.WFDBExplorer
{
    public partial class WFDBExplorerForm : Form
    {
        WFDBScope _WFDBScope;

        DatasetExplorerForm _DatasetExplorerForm;

        public WFDBExplorerForm(DatasetExplorerForm datasetExplorerForm)
        {
            InitializeComponent();

            _DatasetExplorerForm = datasetExplorerForm;
        }

        private void chooseFileButton_Click(object sender, EventArgs e)
        {
            // Open file dialogue to choose matlab file of a signal
            using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, ValidateNames = true, Filter = "Header files|*.hea|DAT files|*.dat|EDF files|*.edf|All files|*.*", RestoreDirectory = true, FilterIndex = 1 })
            {
                // Check if the user clicked OK button
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // Get the path of specified file
                    String filePath = ofd.FileName;

                    // Read the data for WFDBScope
                    _WFDBScope = WFDBRead.ReadWFDBInfo(filePath);

                    // Fill the signals and annotaions combo boxes
                    signalsComboBox.DataSource = _WFDBScope.SignalsDict.Keys.ToList();
                    annotationsComboBox.DataSource = _WFDBScope.AnnotationsDict.Keys.ToList();
                }
            }
        }

        private void signalsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the displayed data of the selected signal
            samplingRateTextBox.Text = _WFDBScope.SignalsDict[signalsComboBox.Text].samplingFreq.ToString();
            quantizationStepTextBox.Text = _WFDBScope.SignalsDict[signalsComboBox.Text].adcGain.ToString();
            descriptionTextBox.Text = _WFDBScope.SignalsDict[signalsComboBox.Text].description;
            signalEndTextBox.Text = (_WFDBScope.SignalsDict[signalsComboBox.Text].Samples.Length / _WFDBScope.SignalsDict[signalsComboBox.Text].samplingFreq).ToString();
        }

        private void signalStartTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.keypressNumbersAndDecimalOnly(sender, e);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            CWD_okButton_Click(sender, e);
        }
    }
}

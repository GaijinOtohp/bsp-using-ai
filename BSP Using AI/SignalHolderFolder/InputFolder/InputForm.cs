using Biological_Signal_Processing_Using_AI;
using Biological_Signal_Processing_Using_AI.WFDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels_Objectives.AIModels_ObjectivesArchitectures.WPWSyndromeDetection;
using static Biological_Signal_Processing_Using_AI.Structures;
using static Biological_Signal_Processing_Using_AI.WFDB.WFBDDefinitions;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace BSP_Using_AI.SignalHolderFolder.Input
{
    public partial class InputForm : Form
    {
        public string _FilePath;
        WFDBScope _WFDBScope;
        public SignalHolder _CurrentSignalHolder;

        public InputForm(string filePath, SignalHolder signalHolder)
        {
            InitializeComponent();

            // Set the file path and current signal holder
            _FilePath = filePath;
            _CurrentSignalHolder = signalHolder;

            // Read WFDB signal if exists
            _WFDBScope = WFDBRead.ReadWFDBInfo(filePath);
            // Fill the signals combo boxes
            if (_WFDBScope.SignalsDict != null)
                signalsComboBox.Items.AddRange(_WFDBScope.SignalsDict.Keys.ToArray());
            else
                signalsComboBox.Items.Add(Path.GetFileName(filePath));
            signalsComboBox.SelectedIndex = 0;
        }

        private void samplingRateTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.keypressNumbersAndDecimalOnly(sender, e);
        }

        private void signalsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the displayed data of the selected signal
            if (_WFDBScope.SignalsDict != null)
            {
                samplingRateTextBox.Text = _WFDBScope.SignalsDict[signalsComboBox.Text].samplingFreq.ToString();
                quantizationStepTextBox.Text = _WFDBScope.SignalsDict[signalsComboBox.Text].adcGain.ToString();
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Get sampling rate and quantization step
            int samplingRate;
            double quantizationStep;
            bool showError = false;
            string errorContent = "The following input is not valid:\n";
            if (samplingRateTextBox.Text.Equals("") || samplingRateTextBox.Text.Equals(".")) { errorContent += ". Sampling rate\n"; showError = true; }
            if (quantizationStepTextBox.Text.Equals("") || quantizationStepTextBox.Text.Equals(".")) { errorContent += ". Quantization step"; showError = true; }

            if (showError)
            {
                MessageBox.Show(errorContent, "Error \"Unexpected data type\"", MessageBoxButtons.OK);
                return;
            }

            // Get sampling rate and quantization step
            samplingRate = int.Parse(samplingRateTextBox.Text);
            quantizationStep = double.Parse(quantizationStepTextBox.Text);

            // Check if the selected file is "mat" file or "txt"
            String extension = System.IO.Path.GetExtension(_FilePath);
            if (extension.Equals(".mat"))
            {
                try
                {
                    // If yes then this is a mat file
                    // Initialize the reader of matlab file
                    Accord.IO.MatReader reader = new Accord.IO.MatReader(_FilePath, false, false);

                    // Get field name (variable name) of this mat file
                    String fieldName = new Accord.IO.MatReader(_FilePath).FieldNames[0];

                    // Read the vector and insert it inside values matrix
                    UInt16[,] buffer = reader[fieldName].Value as UInt16[,];
                    _CurrentSignalHolder._samples = new double[buffer.Length];
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        _CurrentSignalHolder._samples[i] = buffer[i, 0];
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("This file has an unsupported data type\n"
                        + "You can create the supported file, which should include a matrix of one column \"[N,1]\" using the followin command on MATLAB:\n"
                        + ">> save('example.mat', 'val')\n"
                        + "Where:\n"
                        + "example.mat: is the file name, which you want to create\n"
                        + "val: is the variable that holds the one column matrix of the signal values", "Error \"Unexpected data type\"", MessageBoxButtons.OK);
                    return;
                }
            }
            else if (extension.Equals(".txt"))
            {
                // If yes then this is a text file
                String line;
                try
                {
                    //Pass the file path and file name to the StreamReader constructor
                    System.IO.StreamReader sr = new System.IO.StreamReader(_FilePath);

                    // Read each charachter and iterate through digits till the end of the file
                    List<Double> bufferList = new List<Double>();
                    int buffer = sr.Read();
                    String digit = "";
                    while (buffer != -1)
                    {
                        // Check if current character is a space character
                        if (Char.IsWhiteSpace((Char)buffer))
                        {
                            // If yes then add new digit in the list
                            if (!digit.Equals(""))
                                bufferList.Add(Double.Parse(digit));

                            digit = "";
                        }
                        else
                        {
                            // If yes then just add this charachter to the new digit
                            digit += (Char)buffer;
                        }
                        // Read next char
                        buffer = sr.Read();
                    }

                    //close the file
                    sr.Close();

                    // Insert the bufferList values in currentSignalHolder
                    _CurrentSignalHolder._samples = new Double[bufferList.Count];
                    for (int i = 0; i < bufferList.Count; i++)
                    {
                        _CurrentSignalHolder._samples[i] = bufferList[i];
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("This file has an unsupported data type", "Error \"Unexpected data type\"", MessageBoxButtons.OK);
                    return;
                }
            }

            // Set the path of the signal, sampling rate, and quantization step
            _CurrentSignalHolder.pathLabel.Text = _FilePath.Substring(_FilePath.LastIndexOf("\\"));
            _CurrentSignalHolder._arthtFeatures = new ARTHTFeatures();
            _CurrentSignalHolder._FilteringTools = new FilteringTools(samplingRate, quantizationStep, null);

            // Include the WFDB signal if exists
            if (_WFDBScope.SignalsDict != null)
            {
                _CurrentSignalHolder._samples = _WFDBScope.SignalsDict[signalsComboBox.Text].Samples.Select(value => (double)value).ToArray();
                _CurrentSignalHolder.pathLabel.Text = signalsComboBox.Text;
            }

            // Insert signal values inside signal holder chart
            _CurrentSignalHolder.loadSignalStartingFrom(0D);

            // Create new signal holder inside the parent of this control
            // if this control was the last in the list
            FlowLayoutPanel signalsFlowLayout = _CurrentSignalHolder.Parent as FlowLayoutPanel;
            if (signalsFlowLayout.Controls.GetChildIndex(_CurrentSignalHolder) + 1 == signalsFlowLayout.Controls.Count)
            {
                SignalHolder signalHolder = new SignalHolder();
                signalHolder.Width = signalsFlowLayout.Width;

                signalsFlowLayout.Controls.Add(signalHolder);

                CustomVScrollBar vScrollBar = ((MainForm)signalsFlowLayout.FindForm()).vScrollBar;
                if (signalHolder.Height * signalsFlowLayout.Controls.Count > signalsFlowLayout.Height)
                {
                    vScrollBar.LargeChange = signalsFlowLayout.Height;
                    vScrollBar.SetMax(signalHolder.Height * signalsFlowLayout.Controls.Count);

                    signalsFlowLayout.VerticalScroll.SmallChange = vScrollBar.SmallChange;
                    signalsFlowLayout.VerticalScroll.LargeChange = vScrollBar.LargeChange;
                    signalsFlowLayout.VerticalScroll.Maximum = vScrollBar.Maximum;
                }
            }

            // Close the input form
            Close();
        }
    }
}

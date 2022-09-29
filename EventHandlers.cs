using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using BSP_Using_AI.SignalHolderFolder;
using BSP_Using_AI.SignalHolderFolder.Input;
using BSP_Using_AI.DetailsModify;
using BSP_Using_AI.DetailsModify.Filters;
using BSP_Using_AI.MainFormFolder.SignalsComparisonFolder;
using BSP_Using_AI.MainFormFolder.SignalsCollectionFolder;

namespace BSP_Using_AI
{
    class EventHandlers
    {
//*******************************************************************************************************//
//*********************************************MAIN FORM*************************************************//
//*******************************************************************************************************//
        /**
         * Main form signal flow layout panel on size changed handler
         */
        public static void signalFlowLayout_SizeChanged(object sender, int minSize)
        {
            FlowLayoutPanel signalsFlowLayout = sender as FlowLayoutPanel;
            signalsFlowLayout.SuspendLayout();
            foreach (Control ctrl in signalsFlowLayout.Controls)
            {
                // get new width of flow layout panel
                int newWidth = signalsFlowLayout.ClientSize.Width;
                // Check if the new width is more than signal holder's default width
                if (newWidth > minSize)
                {
                    // If yes then change width of signal holder controls
                    ctrl.Width = signalsFlowLayout.ClientSize.Width - 10;
                }
                else
                {
                    ctrl.Width = minSize;
                }
            }
            signalsFlowLayout.ResumeLayout();
        }

        public static void learnedSignalsButton_Click(object sender, EventArgs e)
        {
            // If yes then open the signals form
            // Check if the form is already opened, and close it if so
            if (Application.OpenForms.OfType<AIToolsForm>().Count() == 1)
                Application.OpenForms.OfType<AIToolsForm>().First().Close();

            // Open a new form
            AIToolsForm aIToolsForm = new AIToolsForm();
            aIToolsForm._targetsModelsHashtable = ((MainForm)(sender as Control).FindForm())._targetsModelsHashtable;
            aIToolsForm._mainForm = (MainForm)(sender as Control).FindForm();
            aIToolsForm._tFBackThread = ((MainForm)(sender as Control).FindForm())._tFBackThread;
            aIToolsForm._tFBackThread._tFBackThreadReportHolderForAIToolsForm = aIToolsForm;

            aIToolsForm.Text = "Learned signals";
            aIToolsForm.Show();
        }

        public static void signalsComparatorButton_Click()
        {
            // If yes then open signals comparator form
            // Check if the form is already opened, and close it if so
            if (Application.OpenForms.OfType<FormSignalsComparison>().Count() == 1)
                Application.OpenForms.OfType<FormSignalsComparison>().First().Close();

            // Open a new form
            FormSignalsComparison formSignalsComparison = new FormSignalsComparison();

            formSignalsComparison.Show();
        }

        public static void signalsCollectorButton_Click()
        {
            // If yes then open signals comparator form
            String formName = "FormSignalsCollector";
            int formCopyNum = 0;
            // Check if the form is already opened, and close it if so
            for (int i = 0; i < Application.OpenForms.OfType<FormSignalsCollector>().Count(); i++)
            {
                formCopyNum = i;
                // Check if the opened forms don't contain a name of current number
                if (!Application.OpenForms.OfType<FormSignalsCollector>().ElementAt(i).Name.Equals(formName + formCopyNum.ToString()))
                    // If yes then just break the loop
                    break;
                // Check if there are already 5 forms
                if (i == 4)
                    // If yes then don't opern a new one
                    return;

                formCopyNum += 1;
            }

            // Open a new form
            FormSignalsCollector formSignalsCollector = new FormSignalsCollector();
            formSignalsCollector.Name = formName + formCopyNum.ToString();

            formSignalsCollector.Show();
        }

        //*******************************************************************************************************//
        //*************************************SIGNAL HOLDER USER CONTROL****************************************//
        //*******************************************************************************************************//
        public static void chooseFileButton_Click(object sender, EventArgs e)
        {
            // Open file dialogue to choose matlab file of a signal
            using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, ValidateNames = true, Filter = "MAT file|*.mat|Text file|*.txt|All files|*.*", RestoreDirectory = true, FilterIndex = 2 })
            {
                // Check if the user clicked OK button
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // If yes then load the mat file data into signal exhibitor

                    // Get the path of specified file
                    String filePath = ofd.FileName;

                    /// Get sampling rate from user
                    /// 
                    // Check if the form is already opened, and close it if so
                    if (Application.OpenForms.OfType<InputForm>().Count() == 1)
                        Application.OpenForms.OfType<InputForm>().First().Close();

                    List<String[]> requestedInput = new List<String[]>();
                    requestedInput.Add(new String[] { "Sampling rate (Hz)", "" });
                    requestedInput.Add(new String[] { "Quantization step (adu/mV)", "" });

                    InputForm inputForm = new InputForm("Please insert the sampling rate of the chosen singal", requestedInput);
                    inputForm.Text = "Input values";
                    // Set the file path and current signal holder
                    inputForm._filePath = filePath;
                    inputForm._currentSignalHolder = (SignalHolder)(sender as Button).Parent;
                    // Show the form
                    inputForm.Show();
                }
            }
        }

        public static void detailsModifyButton_Click(object sender)
        {
            // Get signal holder from sender
            SignalHolder signalHolder = (sender as Button).Parent as SignalHolder;

            // Check if the form is already opened, and wlose it if so
            if (Application.OpenForms.OfType<FormDetailsModify>().Count() > 0)
                try
                {
                    foreach (FormDetailsModify form in Application.OpenForms.OfType<FormDetailsModify>())
                        if (form.Text.Equals("Signal details"))
                            form.Close();
                } catch (Exception e)
                {

                }

            // Check if the signal is more than 15 secs
            double[] samples = signalHolder._samples;
            if (signalHolder._truncatedSamples != null)
                // If yes then take the truncated signal
                samples = signalHolder._truncatedSamples;

            // Open a new form
            FormDetailsModify formDetailsModify = new FormDetailsModify(samples, signalHolder._samplingRate, signalHolder._quantizationStep, signalHolder.pathLabel.Text + "\\Modify", signalHolder._startingInSec);
            formDetailsModify._targetsModelsHashtable = ((MainForm)(sender as Button).FindForm())._targetsModelsHashtable;
            formDetailsModify._tFBackThread = ((MainForm)(sender as Button).FindForm())._tFBackThread;
            formDetailsModify.initializeForm(signalHolder);

            formDetailsModify.Text = "Signal details";
            formDetailsModify.Show();
        }

        public static void signalExhibitor_MouseMove(object sender, MouseEventArgs e, int previousMouseX, int previousMouseY)
        {
            int offsetX = previousMouseX - e.X;
            int offsetY = e.Y - previousMouseY;

            Chart chart = sender as Chart;

            Axis xAxis = chart.ChartAreas[0].AxisX;
            Axis yAxis = chart.ChartAreas[0].AxisY;

            double xMin = xAxis.Minimum;
            double xMax = xAxis.Maximum;
            double yMin = yAxis.Minimum;
            double yMax = yAxis.Maximum;
            double xScaledMin = xAxis.ScaleView.ViewMinimum;
            double xScaledMax = xAxis.ScaleView.ViewMaximum;
            double yScaledMin = yAxis.ScaleView.ViewMinimum;
            double yScaledMax = yAxis.ScaleView.ViewMaximum;

            xAxis.ScaleView.Position += offsetX / ( ((Chart)sender).Width / (xScaledMax - xScaledMin));

            yAxis.ScaleView.Position += offsetY / (((Chart)sender).Height / (yScaledMax - yScaledMin));
        }

        public static void signalExhibitor_MouseWheel(object sender, MouseEventArgs e, int previousMouseX, int previousMouseY)
        {
            Chart chart = (Chart)sender;
            Axis xAxis = chart.ChartAreas[0].AxisX;
            Axis yAxis = chart.ChartAreas[0].AxisY;

            int chartWidth = chart.Size.Width;
            int chartHeight = chart.Size.Height;

            double xMin = xAxis.Minimum;
            double xMax = xAxis.Maximum;
            double yMin = yAxis.Minimum;
            double yMax = yAxis.Maximum;

            double xScaledMin = xAxis.ScaleView.ViewMinimum;
            double xScaledMax = xAxis.ScaleView.ViewMaximum;
            double yScaledMin = yAxis.ScaleView.ViewMinimum;
            double yScaledMax = yAxis.ScaleView.ViewMaximum;

            double xCurrentPosition = xAxis.PixelPositionToValue(e.X);
            double yCurrentPosition = yAxis.PixelPositionToValue(e.Y);

            try
            {
                if (e.Delta < 0) // Scrolled down.
                {
                    double posXStart = xScaledMin - (xCurrentPosition - xScaledMin) / 8d;

                    double xPreviousInterval = xScaledMax - xScaledMin;
                    double xOffsetRatio = (xCurrentPosition - xScaledMin) * 100 / xPreviousInterval;
                    double xNewInterval = 100 * (xCurrentPosition - posXStart) / xOffsetRatio;

                    double posXFinish = posXStart + xNewInterval;

                    double posYStart = yScaledMin - (yCurrentPosition - yScaledMin) / 8d;

                    double yPreviousInterval = yScaledMax - yScaledMin;
                    double yOffsetRatio = (yCurrentPosition - yScaledMin) * 100 / yPreviousInterval;
                    double yNewInterval = 100 * (yCurrentPosition - posYStart) / yOffsetRatio;

                    double posYFinish = posYStart + yNewInterval;

                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    yAxis.ScaleView.Zoom(posYStart, posYFinish);
                }
                else if (e.Delta > 0) // Scrolled up.
                {
                    double posXStart = xCurrentPosition - (xCurrentPosition - xScaledMin) / 1.05d ;

                    double xPreviousInterval = xScaledMax - xScaledMin;
                    double xOffsetRatio = (xCurrentPosition - xScaledMin) * 100 / xPreviousInterval;
                    double xNewInterval = 100 * (xCurrentPosition - posXStart) / xOffsetRatio;

                    double posXFinish = posXStart + xNewInterval;
                    
                    double posYStart = yCurrentPosition - (yCurrentPosition - yScaledMin) / 1.05d;

                    double yPreviousInterval = yScaledMax - yScaledMin;
                    double yOffsetRatio = (yCurrentPosition - yScaledMin) * 100 / yPreviousInterval;
                    double yNewInterval = 100 * (yCurrentPosition - posYStart) / yOffsetRatio;

                    double posYFinish = posYStart + yNewInterval;

                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    yAxis.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch { }
        }

        public static void forwardButton_Click(object sender)
        {
            // Move the signal 5 secs forward
            SignalHolder signalHolder = ((sender as Button).Parent as SignalHolder);

            signalHolder.loadSignalStartingFrom(signalHolder._startingInSec + 5D);
        }

        public static void backwardButton_Click(object sender)
        {
            // Move the signal 5 secs backward
            SignalHolder signalHolder = ((sender as Button).Parent as SignalHolder);

            signalHolder.loadSignalStartingFrom(signalHolder._startingInSec - 5D);
        }

        //*******************************************************************************************************//
        //*****************************************FROM DETAILS MODIFY*******************************************//
        public static void TextBox_Enter_Leave_Focus(TextBox textBox, String defaultText, bool enter)
        {
            if (enter && textBox.Text.Equals(defaultText))
            {
                textBox.Text = "";

                textBox.ForeColor = System.Drawing.Color.Black;
            }
            else if (!enter && textBox.Text.Equals(""))
            {
                textBox.Text = defaultText;

                textBox.ForeColor = System.Drawing.Color.Silver;
            }
        }

        public static void textBoxNumberOnly(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.')) || ((e.KeyChar == '.') && ((sender as TextBox).Text.Replace(" ", "").Equals("") || (sender as TextBox).Text.Contains("."))))
            {
                e.Handled = true;
            }
        }

        public static void filtersComboBox_SelectedIndexChanged(object sender, FormDetailsModify formDetailsModify)
        {
            // Make a variable for the cotrol to be added in the filtersFlowLayoutPanel
            Control filterControl = null;
            // and the object for _filtersHashtable
            object[] filterObject = null;

            // Set the id of the new control
            int id = 0;
            int selectedIndex = (sender as ComboBox).SelectedIndex;
            if (selectedIndex < 0)
                return;
            String key = (sender as ComboBox).Items[selectedIndex].ToString() + id.ToString();

            // Check if the new id is already in _filtersHashtable
            while (formDetailsModify._filteresHashtable.ContainsKey(key))
            {
                // If yes then set a new id
                id += 1;
                key = (sender as ComboBox).Items[selectedIndex].ToString() + id.ToString();
            }

            // Check which filter is selected
            switch ((sender as ComboBox).Items[selectedIndex])
            {
                case "DC removal":
                    // for DC removal

                    // Set new DCRemovalUserControl with the key as its name in filterControl
                    filterControl = new DCRemovalUserControl();
                    filterControl.Name = key;

                    // Set filterObject with the value 0
                    filterObject = new object[] { "DC removal", 0, 0 };
                    break;
                case "Normalize signal":
                    // for Normilizing signal

                    // Set new NormalizedSignalUserControl with the key as its name in filterControl
                    filterControl = new NormalizedSignalUserControl();
                    filterControl.Name = key;

                    // Set filterObject with the value 0
                    filterObject = new object[] { "Normalize signal", 0, 0 };
                    break;
                case "Absolute signal":
                    // for absoluting signal

                    // Set new AbsoluteSignalUserControl with the key as its name in filterControl
                    filterControl = new AbsoluteSignalUserControl();
                    filterControl.Name = key;

                    // Set filterObject with the value 0
                    filterObject = new object[] { "Absolute signal", 0, 0 };
                    break;
                case "Singal states viewer":
                    // for Signal staets viewer

                    // Set new DCRemovalUserControl with the key as its name in filterControl
                    filterControl = new SignalStatesViewerUserControl(formDetailsModify);
                    filterControl.Name = key;

                    // Set filterObject with the value 0
                    filterObject = new object[] { "Singal states viewer", true, true, 0.02D };
                    break;
                default:
                    // This is for IIR filters
                    // Set new IIR filter with the key as its name in filterControl
                    filterControl = new DetailsModify.Filters.IIRFilters.IIRFilterUserControl(formDetailsModify._samplingRate);
                    filterControl.Name = key;
                    // Set the label of the filter
                    (filterControl as DetailsModify.Filters.IIRFilters.IIRFilterUserControl).nameFilterLabel.Text = (sender as ComboBox).Items[selectedIndex] + " filter";

                    // Set filterObject with the value 0
                    filterObject = new object[] { (sender as ComboBox).Items[selectedIndex], 0, 10, 0.5D, 0 };
                    break;
            }

            // Set the combobox selection to nothing
            (sender as ComboBox).SelectedIndex = -1;

            // Add the new control in filtersFlowLayoutPanel and its value in _filtersHashtable
            // if it wasn't null
            if (filterControl != null)
            {
                // Get filtersFlowLayoutPanel
                FlowLayoutPanel filtersFlowLayoutPanel = (FlowLayoutPanel)formDetailsModify.Controls.Find("filtersFlowLayoutPanel", false)[0];
                // Add the new filter
                filtersFlowLayoutPanel.Controls.Add(filterControl);
                // Add its value in filtersHashtable
                formDetailsModify._filteresHashtable.Add(filterControl.Name, filterObject);
            }
        }

        //*******************************************************************************************************//
        //*****************************************FILTERS USER CONTROLS*****************************************//
        //*******************************************************************************************************//
        public static void deleteToolStripMenuItem_Click(UserControl filterUserControl)
        {
            // This is the delete function of the filter
            // Remove the filter from _filtersHashtable
            System.Collections.Hashtable filtersHashtable = ((FormDetailsModify)filterUserControl.FindForm())._filteresHashtable;
            filtersHashtable.Remove(filterUserControl.Name);

            // remove this filter from filtersFlowLayoutPanel
            ((FlowLayoutPanel)filterUserControl.Parent).Controls.Remove(filterUserControl);
        }

        //*******************************************************************************************************//
        //***********************************************INPUT FORM**********************************************//
        //*******************************************************************************************************//
        public static void okButton_Click(object sender, EventArgs e)
        {
            // Get the filePath
            String filePath = ((InputForm)(sender as Button).FindForm())._filePath;

            // Get sampling rate
            double samplingRate;
            double quantizationStep;
            try
            {
                samplingRate = double.Parse(((InputValueUserControl)(((FlowLayoutPanel)(((InputForm)(sender as Button).FindForm()).inputFlowLayoutPanel)).Controls[0])).inputTextBox.Text);
                quantizationStep = double.Parse(((InputValueUserControl)(((FlowLayoutPanel)(((InputForm)(sender as Button).FindForm()).inputFlowLayoutPanel)).Controls[1])).inputTextBox.Text);
            } catch (Exception exception)
            {
                MessageBox.Show("Insert a valid sampling rate and quantizationStep", "Error \"Unexpected data type\"", MessageBoxButtons.OK);
                return;
            }

            // Get current signal holder user control
            SignalHolder currentSignalHolder = (((InputForm)(sender as Button).Parent)._currentSignalHolder as SignalHolder);

            // Check if the selected file is "mat" file or "txt"
            String extension = System.IO.Path.GetExtension(filePath);
            if (extension.Equals(".mat"))
            {
                try
                {
                    // If yes then this is a mat file
                    // Initialize the reader of matlab file
                    Accord.IO.MatReader reader = new Accord.IO.MatReader(filePath, false, false);

                    // Get field name (variable name) of this mat file
                    String fieldName = new Accord.IO.MatReader(filePath).FieldNames[0];

                    // Read the vector and insert it inside values matrix
                    UInt16[,] buffer = reader[fieldName].Value as UInt16[,];
                    currentSignalHolder._samples = new Double[buffer.Length];
                    currentSignalHolder._samplingRate = samplingRate;
                    currentSignalHolder._quantizationStep = quantizationStep;
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        currentSignalHolder._samples[i] = buffer[i, 0];
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
                    System.IO.StreamReader sr = new System.IO.StreamReader(filePath);

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
                    currentSignalHolder.pathLabel.Text = filePath.Substring(filePath.LastIndexOf("\\"));
                    currentSignalHolder._samples = new Double[bufferList.Count];
                    currentSignalHolder._samplingRate = samplingRate;
                    currentSignalHolder._quantizationStep = quantizationStep;
                    for (int i = 0; i < bufferList.Count; i++)
                    {
                        currentSignalHolder._samples[i] = bufferList[i];
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show("This file has an unsupported data type", "Error \"Unexpected data type\"", MessageBoxButtons.OK);
                    return;
                }
            }

            // Insert signal values inside signal holder chart
            currentSignalHolder.loadSignalStartingFrom(0D);

            // Create new signal holder inside the parent of this control
            // if this control was the last in the list
            int signalsFlowLayoutControlsCount = (currentSignalHolder.Parent as FlowLayoutPanel).Controls.Count;
            int currentSignalHolderIndex = (currentSignalHolder.Parent as FlowLayoutPanel).Controls.GetChildIndex((UserControl)currentSignalHolder as UserControl);
            if (currentSignalHolderIndex + 1 == signalsFlowLayoutControlsCount && signalsFlowLayoutControlsCount < 20)
            {
                FlowLayoutPanel flowLayoutPanel = currentSignalHolder.Parent as FlowLayoutPanel;
                SignalHolder signalHolder = new SignalHolder();
                signalHolder.Name = "signalHolder" + (currentSignalHolderIndex + 1);

                if (flowLayoutPanel.Width > 900)
                {
                    // If yes then change width of signal holder controls
                    signalHolder.Width = flowLayoutPanel.Width;
                }
                else
                {
                    signalHolder.Width = 900;
                }
                flowLayoutPanel.Controls.Add(signalHolder);

                VScrollBar vScrollBar = ((MainForm)flowLayoutPanel.FindForm()).vScrollBar;
                if (signalHolder.Height * flowLayoutPanel.Controls.Count > flowLayoutPanel.Height)
                {
                    vScrollBar.LargeChange = flowLayoutPanel.Height;
                    vScrollBar.Maximum = signalHolder.Height * flowLayoutPanel.Controls.Count;

                    flowLayoutPanel.VerticalScroll.LargeChange = flowLayoutPanel.Height;
                    flowLayoutPanel.VerticalScroll.Maximum = signalHolder.Height * flowLayoutPanel.Controls.Count;
                }
            }

            // Close the input form
            Application.OpenForms.OfType<InputForm>().First().Close();
        }

        //*******************************************************************************************************//
        //******************************SIGNALS COMPARISON & SIGNALS COLLECTOR***********************************//
        //*******************************************************************************************************//
        public static void sendSignalTool(double[] signal, double samplingRate, double quantizationStep, String path)
        {
            // Check if FormSignalsComparison is opened
            if (Application.OpenForms.OfType<FormSignalsComparison>().Count() == 1)
            {
                // If yes then send the signal to the form
                Application.OpenForms.OfType<FormSignalsComparison>().ElementAt(0).insertSignal(signal, samplingRate, quantizationStep);
            }

            // Check if there is FormSignalsCollector opened
            for (int i = 0; i < Application.OpenForms.OfType<FormSignalsCollector>().Count(); i++)
            {
                // If yes then send the signal to the form
                Application.OpenForms.OfType<FormSignalsCollector>().ElementAt(i).insertSignal(signal, samplingRate, quantizationStep, path);
            }
        }

        public static void analyseSignalTool(double[] signal, double samplingRate, double quantizationStep, String path)
        {
            // Open a new form
            FormDetailsModify formDetailsModify = new FormDetailsModify(signal, samplingRate, quantizationStep, path, 0);

            // Remove features selections options
            formDetailsModify.setFeaturesLabelsButton.Visible = false;
            formDetailsModify.discardButton.Visible = false;
            formDetailsModify.aiGoalComboBox.Visible = false;
            formDetailsModify.featuresSettingInstructionsLabel.Visible = false;
            formDetailsModify.featuresTableLayoutPanel.Visible = false;
            formDetailsModify.nextButton.Visible = false;
            formDetailsModify.previousButton.Visible = false;
            formDetailsModify.predictButton.Visible = false;
            formDetailsModify.modelTypeComboBox.Visible = false;
            formDetailsModify.Height = 593;

            formDetailsModify.Text = "Signal analyser";
            formDetailsModify.Show();
        }
    }
}

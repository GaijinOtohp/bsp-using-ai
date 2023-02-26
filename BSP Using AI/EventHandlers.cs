using BSP_Using_AI.DetailsModify;
using BSP_Using_AI.DetailsModify.Filters;
using BSP_Using_AI.MainFormFolder.SignalsCollectionFolder;
using BSP_Using_AI.MainFormFolder.SignalsComparisonFolder;
using BSP_Using_AI.SignalHolderFolder;
using BSP_Using_AI.SignalHolderFolder.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

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
            aIToolsForm._arthtModelsDic = ((MainForm)(sender as Control).FindForm())._arthtModelsDic;
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

            xAxis.ScaleView.Position += offsetX / (((Chart)sender).Width / (xScaledMax - xScaledMin));

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
                    double posXStart = xCurrentPosition - (xCurrentPosition - xScaledMin) / 1.05d;

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

        //*******************************************************************************************************//
        //*****************************************FROM DETAILS MODIFY*******************************************//
        public static void textBoxNumberOnly(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.')) || ((e.KeyChar == '.') && ((sender as TextBox).Text.Replace(" ", "").Equals("") || (sender as TextBox).Text.Contains("."))))
            {
                e.Handled = true;
            }
        }

        //*******************************************************************************************************//
        //*******************************************Clear flow layout*******************************************//
        //*******************************************************************************************************//
        public static void fastClearFlowLayout(ref CustomFlowLayoutPanel flowLayoutPanel)
        {
            // Get parent control of this flowLayoutPanel
            Control parentControl = flowLayoutPanel.Parent;
            // Create a new flowLayout with the sampe properties as curent one
            CustomFlowLayoutPanel newFlowLayoutPanel = new CustomFlowLayoutPanel();
            newFlowLayoutPanel.Name = flowLayoutPanel.Name;
            newFlowLayoutPanel.Anchor = flowLayoutPanel.Anchor;
            newFlowLayoutPanel.AutoScroll = flowLayoutPanel.AutoScroll;
            newFlowLayoutPanel.Margin = flowLayoutPanel.Margin;
            newFlowLayoutPanel.Size = flowLayoutPanel.Size;
            newFlowLayoutPanel.Location = flowLayoutPanel.Location;
            newFlowLayoutPanel.BackColor = flowLayoutPanel.BackColor;
            newFlowLayoutPanel.ForeColor = flowLayoutPanel.ForeColor;
            // Dispose recordsFflowLayoutçRTpSrTp for faster clearing its controls
            flowLayoutPanel.Dispose();
            // Inser the new flowLayout in recordsFflowLayoutçRTpSrTp
            flowLayoutPanel = newFlowLayoutPanel;

            parentControl.Controls.Add(flowLayoutPanel);
        }

        //*******************************************************************************************************//
        //******************************SIGNALS COMPARISON & SIGNALS COLLECTOR***********************************//
        //*******************************************************************************************************//
        public static void sendSignalTool(FilteringTools filteringTools, String path)
        {
            // Check if FormSignalsComparison is opened
            if (Application.OpenForms.OfType<FormSignalsComparison>().Count() == 1)
            {
                // If yes then send the signal to the form
                Application.OpenForms.OfType<FormSignalsComparison>().ElementAt(0).insertSignal(filteringTools);
            }

            // Check if there is FormSignalsCollector opened
            for (int i = 0; i < Application.OpenForms.OfType<FormSignalsCollector>().Count(); i++)
            {
                // If yes then send the signal to the form
                Application.OpenForms.OfType<FormSignalsCollector>().ElementAt(i).insertSignal(filteringTools, path);
            }
        }

        public static void analyseSignalTool(FilteringTools filteringTools, String path)
        {
            // Open a new form
            FormDetailsModify formDetailsModify = new FormDetailsModify(filteringTools, path);

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

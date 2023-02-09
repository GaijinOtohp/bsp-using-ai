using System;
using System.Windows.Forms;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace BSP_Using_AI.MainFormFolder.SignalsCollectionFolder
{
    public partial class FormSignalsCollector : Form
    {
        public bool scrollAllowed = true;

        public FormSignalsCollector()
        {
            InitializeComponent();
        }

        //*******************************************************************************************************//
        //********************************************CLASS FUNCTIONS********************************************//
        public void insertSignal(FilteringTools filteringTools, String path)
        {
            // Check if the flow layout is open for new signals
            if (selectSignalCheckBox.Checked)
            {
                // Create new UserControlSignalPower and insert it in signalsFlowLayoutPanel
                UserControlSignalPower userControlSignalPower = new UserControlSignalPower(filteringTools);
                userControlSignalPower.pathLabel.Text = path;
                if (signalsFlowLayoutPanel.Width > 900)
                {
                    // If yes then change width of signal holder controls
                    userControlSignalPower.Width = signalsFlowLayoutPanel.Width;
                }
                else
                {
                    userControlSignalPower.Width = 900;
                }

                signalsFlowLayoutPanel.Controls.Add(userControlSignalPower);

                if (userControlSignalPower.Height * signalsFlowLayoutPanel.Controls.Count > signalsFlowLayoutPanel.Height)
                {
                    vScrollBar.LargeChange = signalsFlowLayoutPanel.Height;
                    vScrollBar.Maximum = userControlSignalPower.Height * signalsFlowLayoutPanel.Controls.Count;

                    signalsFlowLayoutPanel.VerticalScroll.LargeChange = signalsFlowLayoutPanel.Height;
                    signalsFlowLayoutPanel.VerticalScroll.Maximum = userControlSignalPower.Height * signalsFlowLayoutPanel.Controls.Count;
                }

                // Uncheck the button
                selectSignalCheckBox.Checked = false;
            }
        }

        public void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            signalsFlowLayoutPanel.VerticalScroll.Value = vScrollBar.Value;
        }

        private void signalsFlowLayoutPanel_SizeChanged(object sender, EventArgs e)
        {
            EventHandlers.signalFlowLayout_SizeChanged(sender, 900);
        }

        private void signalExhibitor_MouseWheelScroll(object sender, MouseEventArgs e)
        {
            // Check if scroll is allowed (mouse is not in a signal exhibitor)
            if (scrollAllowed)
            {
                int diff;
                if (e.Delta < 0)
                {
                    diff = vScrollBar.Value + vScrollBar.SmallChange;
                    if (diff < vScrollBar.Maximum - vScrollBar.LargeChange + 1)
                        vScrollBar.Value = diff;
                    else
                        vScrollBar.Value = vScrollBar.Maximum - vScrollBar.LargeChange + 1;
                }
                else
                {
                    diff = vScrollBar.Value - vScrollBar.SmallChange;
                    if (diff > 0)
                        vScrollBar.Value = diff;
                    else
                        vScrollBar.Value = 1;
                }

                vScrollBar1_Scroll(null, null);
            }
        }
    }
}

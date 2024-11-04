using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.DetailsModify;
using BSP_Using_AI.SignalHolderFolder.Input;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace BSP_Using_AI.SignalHolderFolder
{
    public partial class SignalHolder
    {
        private void chooseFileButton_Click(object sender, EventArgs e)
        {
            // Open file dialogue to choose matlab file of a signal
            using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, ValidateNames = true, Filter = "Header files|*.hea|MAT file|*.mat|Text file|*.txt|DAT files|*.dat|EDF files|*.edf|All files|*.*", RestoreDirectory = true, FilterIndex = 1 })
            {
                // Check if the user clicked OK button
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // If yes then load the mat file data into signal exhibitor

                    // Get the path of specified file
                    String filePath = ofd.FileName;

                    // Check if the form is already opened, and close it if so
                    if (Application.OpenForms.OfType<InputForm>().Count() > 0)
                        Application.OpenForms.OfType<InputForm>().First().Close();

                    InputForm inputForm = new InputForm(filePath, this);
                    // Show the form
                    inputForm.Show();
                }
            }
        }

        private void detailsModifyButton_Click(object sender, EventArgs e)
        {
            // Open a new form
            FormDetailsModify formDetailsModify = new FormDetailsModify(_FilteringTools.Clone(), pathLabel.Text + "\\Modify");
            formDetailsModify._objectivesModelsDic = ((MainForm)FindForm())._objectivesModelsDic;
            formDetailsModify._tFBackThread = ((MainForm)FindForm())._tFBackThread;
            formDetailsModify.initializeForm(this);

            formDetailsModify.Text = "Signal details";
            formDetailsModify.Show();
        }

        private void signalSpanTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.keypressNumbersAndDecimalOnly(sender, e);
        }

        private void signalSpanTextBox_TextChanged(object sender, System.EventArgs e)
        {
            loadSignalStartingFrom(_FilteringTools._startingInSec);
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            // Move the signal 5 secs forward
            loadSignalStartingFrom(_FilteringTools._startingInSec + 5);
        }

        private void backwardButton_Click(object sender, EventArgs e)
        {
            // Move the signal 5 secs backward
            loadSignalStartingFrom(_FilteringTools._startingInSec - 5);
        }

        private void sendSignalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EventHandlers.sendSignalTool(_FilteringTools.Clone(), pathLabel.Text + "\\Collector");
        }

        private void signalExhibitor_MouseEnter(object sender, EventArgs e)
        {
            ((MainForm)FindForm()).scrollAllowed = false;
        }

        private void signalExhibitor_MouseLeave(object sender, EventArgs e)
        {
            ((MainForm)FindForm()).scrollAllowed = true;
        }
    }
}

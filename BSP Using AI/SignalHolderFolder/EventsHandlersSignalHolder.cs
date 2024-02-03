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
            using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, ValidateNames = true, Filter = "MAT file|*.mat|Text file|*.txt|All files|*.*", RestoreDirectory = true, FilterIndex = 2 })
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

                    InputForm inputForm = new InputForm();
                    // Set the file path and current signal holder
                    inputForm._FilePath = filePath;
                    inputForm._CurrentSignalHolder = this;
                    // Show the form
                    inputForm.Show();
                }
            }
        }

        private void detailsModifyButton_Click(object sender, EventArgs e)
        {
            // Check if the form is already opened, and wlose it if so
            /*FormDetailsModify[] formDetailsModifyArray = Application.OpenForms.OfType<FormDetailsModify>().ToArray();
            for (int i = 0; i < formDetailsModifyArray.Length; i++)
                if (formDetailsModifyArray[i].Text.Equals("Signal details"))
                    formDetailsModifyArray[i].Close();*/

            // Open a new form
            FormDetailsModify formDetailsModify = new FormDetailsModify(_FilteringTools.Clone(), pathLabel.Text + "\\Modify");
            formDetailsModify._arthtModelsDic = ((MainForm)FindForm())._arthtModelsDic;
            formDetailsModify._tFBackThread = ((MainForm)FindForm())._tFBackThread;
            formDetailsModify.initializeForm(this);

            formDetailsModify.Text = "Signal details";
            formDetailsModify.Show();
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

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Save the signal with its features in dataset
            DbStimulator dbStimulator = new DbStimulator();
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Insert("dataset", new string[] { "sginal_name", "starting_index", "signal", "sampling_rate", "quantisation_step", "features" },
                new Object[] { pathLabel.Text, _FilteringTools._startingInSec, Garage.ObjectToByteArray(_FilteringTools._OriginalRawSamples), _FilteringTools._samplingRate,
                               _FilteringTools._quantizationStep, Garage.ObjectToByteArray(_arthtFeatures) }, "SignalHolder"));
            dbStimulatorThread.Start();

            // Update the notification badge for unfitted signals
            Control badge = MainFormFolder.BadgeControl.GetBadge(this.FindForm());
            badge.Text = (int.Parse(badge.Text) + 1).ToString();
            badge.Visible = true;
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

using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI;
using BSP_Using_AI.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;

namespace Biological_Signal_Processing_Using_AI.DetailsModify.Annotations
{
    public partial class AnnotationModify : Form, DbStimulatorReportHolder
    {
        AnnotationItemUserControl _AnnoItem;

        AnnotationECG _Anno;

        public AnnotationModify(AnnotationItemUserControl annoItem)
        {
            InitializeComponent();

            _AnnoItem = annoItem;
            _Anno = annoItem._Anno;

            // Fill the name combobox with suggestions
            // From current _AnnotationData
            List<string> nameSource = _Anno.ParentAnnoData.GetAnnotations().GroupBy(anno => anno.Name).Select(anno => anno.Key).ToList();
            // Copy suggestions to DataSource
            nameComboBox.DataSource = nameSource;
            // From annotation dataset
            DbStimulator dbStimulator = new DbStimulator();
            dbStimulator.bindToRecordsDbStimulatorReportHolder(this);
            Thread dbStimulatorThread = new Thread(() => dbStimulator.Query("anno_ds",
                                new String[] { "anno_data" },
                                null,
                                null,
                                "", "AnnotationModify"));
            dbStimulatorThread.Start();

            // Present the annotation values
            nameComboBox.Text = _Anno.Name;
            startingIndexTextBox.Text = _Anno.GetIndexes().starting.ToString();
            endingIndexTextBox.Text = _Anno.GetIndexes().ending.ToString();

            // Change the starting index's label according to the type of the annotation
            if (_Anno.GetAnnotationType().Equals(AnnotationType.Point))
            {
                AnnotationTypeLabel.Text = "Point annotation";
                startingIndexLabel.Text = "Index";
                endingIndexLabel.Visible = false;
                endingIndexTextBox.Visible = false;
            }

            // Set the focus to AnnotationTypeLabel
            this.ActiveControl = AnnotationTypeLabel;
        }

        private void startingIndexTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.keypressNumbersOnly(sender, e);
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            // Show message for confirming the action
            DialogResult dialogResult = MessageBox.Show("Are you sure about removing the annotation \"" + _Anno.Name + "\"?", "Action confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                _AnnoItem.Remove();
                this.Dispose();
            }
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            // Copy data from the text boxes into _Anno
            string name = nameComboBox.Text;
            int startingIndex = 0;
            int endngIndex = 0;
            if (startingIndexTextBox.Text.Length > 0)
                startingIndex = int.Parse(startingIndexTextBox.Text);
            if (_Anno.GetAnnotationType().Equals(AnnotationType.Interval) && endingIndexTextBox.Text.Length > 0)
                endngIndex = int.Parse(endingIndexTextBox.Text);

            _AnnoItem.SetNewVals(name, startingIndex, endngIndex);

            this.Close();
        }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //:::::::::::::::::::::::::::CROSS PROCESS FORM FUNCTIONS (INTERFACES)::::::::::::::::::::::://
        public void holdRecordReport(DataTable dataTable, string callingClassName)
        {
            if (!callingClassName.Equals("AnnotationModify"))
                return;

            List<string> nameSource = (List<string>)nameComboBox.DataSource;
            foreach (DataRow row in dataTable.Rows)
            {
                AnnotationData annoData = GeneralTools.ByteArrayToObject<AnnotationData>(row.Field<byte[]>("anno_data"));
                nameSource.AddRange(annoData.GetAnnotations().GroupBy(anno => anno.Name).Select(anno => anno.Key).ToList());
            }

            nameSource = nameSource.GroupBy(name => name).Select(name => name.Key).ToList();

            // Copy suggestions to DataSource
            _AnnoItem.Invoke(new MethodInvoker(delegate () { nameComboBox.DataSource = nameSource; }));
        }
    }
}

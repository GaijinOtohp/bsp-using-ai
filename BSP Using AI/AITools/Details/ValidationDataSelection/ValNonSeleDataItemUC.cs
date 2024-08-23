using BSP_Using_AI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Biological_Signal_Processing_Using_AI.AITools.Details.ValidationDataSelection
{
    public partial class ValNonSeleDataItemUC : UserControl
    {
        private DataRow currentItemDataRow;
        private Dictionary<int, DataRow> _GlobalSelectedValiDataList;

        public ValNonSeleDataItemUC(DataRow row, string catLabel, Color backColor, Dictionary<int, DataRow> selectedValiDataList)
        {
            InitializeComponent();
            currentItemDataRow = row;
            _GlobalSelectedValiDataList = selectedValiDataList;

            signalNameLabel.Text = row.Field<string>("sginal_name");
            startingIndexLabel.Text = row.Field<long>("starting_index").ToString();
            samplingRateLabel.Text = row.Field<long>("sampling_rate").ToString();
            quantizationStepLabel.Text = row.Field<long>("quantisation_step").ToString();
            categoryLabel.Text = catLabel;
            BackColor = backColor;
        }

        private void forValidationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Get current item's index in signalsFlowLayoutPanel
            int currentItemIndx = ((FlowLayoutPanel)this.Parent).Controls.GetChildIndex(this);
            // Change its row existence in selectedValiDataList
            if (forValidationCheckBox.Checked)
                _GlobalSelectedValiDataList.Add(currentItemIndx, currentItemDataRow);
            else
                _GlobalSelectedValiDataList.Remove(currentItemIndx);

            if (((ValDataSelectionForm)this.FindForm())._ignoreEvent)
                return;

            ((ValDataSelectionForm)this.FindForm())._ignoreEvent = true;

            // Check if "Shift" button is clicked and a previous selected item
            if (((ValDataSelectionForm)this.FindForm())._shiftClicked && ((ValDataSelectionForm)this.FindForm())._lastSelectedItem_shift != -1)
            {
                // If yes then alter the selection status of each item in the shifted interval
                int start = ((ValDataSelectionForm)this.FindForm())._lastSelectedItem_shift + 1;
                int end = currentItemIndx;
                if (start > end)
                {
                    start = currentItemIndx + 1;
                    end = ((ValDataSelectionForm)this.FindForm())._lastSelectedItem_shift;
                }
                for (int i = start; i < end; i++)
                    if (((FlowLayoutPanel)this.Parent).Controls[i].Enabled)
                        ((ValNonSeleDataItemUC)((FlowLayoutPanel)this.Parent).Controls[i]).forValidationCheckBox.Checked = !((ValNonSeleDataItemUC)((FlowLayoutPanel)this.Parent).Controls[i]).forValidationCheckBox.Checked;
            }

            // Update _lastSelectedItem_shift
            ((ValDataSelectionForm)this.FindForm())._lastSelectedItem_shift = currentItemIndx;

            ((ValDataSelectionForm)this.FindForm())._ignoreEvent = false;
        }
    }
}

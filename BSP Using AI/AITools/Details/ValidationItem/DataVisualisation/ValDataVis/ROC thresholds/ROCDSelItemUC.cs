using BSP_Using_AI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation.ValDataVis.ROC_thresholds
{
    public partial class ROCDSelItemUC : UserControl
    {
        private DataRow currentItemDataRow;
        private Dictionary<int, DataRow> _GlobalSelectedOptiDataList;

        public ROCDSelItemUC(DataRow row, string catLabel, Color backColor, Dictionary<int, DataRow> selectedValiDataList)
        {
            InitializeComponent();
            currentItemDataRow = row;
            _GlobalSelectedOptiDataList = selectedValiDataList;

            signalNameLabel.Text = row.Field<string>("sginal_name");
            startingIndexLabel.Text = row.Field<long>("starting_index").ToString();
            samplingRateLabel.Text = row.Field<long>("sampling_rate").ToString();
            quantizationStepLabel.Text = row.Field<long>("quantisation_step").ToString();
            categoryLabel.Text = catLabel;
            BackColor = backColor;
        }

        private void forOptimizationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Get current item's index in signalsFlowLayoutPanel
            int currentItemIndx = ((FlowLayoutPanel)this.Parent).Controls.GetChildIndex(this);
            // Change its row existence in selectedValiDataList
            if (forOptimizationCheckBox.Checked)
                _GlobalSelectedOptiDataList.Add(currentItemIndx, currentItemDataRow);
            else
                _GlobalSelectedOptiDataList.Remove(currentItemIndx);

            if (((ROCDataSelForm)this.FindForm())._ignoreEvent)
                return;

            ((ROCDataSelForm)this.FindForm())._ignoreEvent = true;

            // Check if "Shift" button is clicked and a previous selected item
            if (((ROCDataSelForm)this.FindForm())._shiftClicked && ((ROCDataSelForm)this.FindForm())._lastSelectedItem_shift != -1)
            {
                // If yes then alter the selection status of each item in the shifted interval
                int start = ((ROCDataSelForm)this.FindForm())._lastSelectedItem_shift + 1;
                int end = currentItemIndx;
                if (start > end)
                {
                    start = currentItemIndx + 1;
                    end = ((ROCDataSelForm)this.FindForm())._lastSelectedItem_shift;
                }
                for (int i = start; i < end; i++)
                    if (((FlowLayoutPanel)this.Parent).Controls[i].Enabled)
                        ((ROCDSelItemUC)((FlowLayoutPanel)this.Parent).Controls[i]).forOptimizationCheckBox.Checked = !((ROCDSelItemUC)((FlowLayoutPanel)this.Parent).Controls[i]).forOptimizationCheckBox.Checked;
            }

            // Update _lastSelectedItem_shift
            ((ROCDataSelForm)this.FindForm())._lastSelectedItem_shift = currentItemIndx;

            ((ROCDataSelForm)this.FindForm())._ignoreEvent = false;
        }
    }
}

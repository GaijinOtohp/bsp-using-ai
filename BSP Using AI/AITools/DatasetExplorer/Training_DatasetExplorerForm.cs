using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP_Using_AI.AITools.DatasetExplorer
{
    public partial class DatasetExplorerForm
    {
        private void holdRecordReport_Training(DataTable dataTable, DataRow row, DatasetFlowLayoutPanelItemUserControl trainingItem)
        {
            // Check the selection of the signal and disable featuresDetailsButton
            trainingItem.selectionCheckBox.Checked = true;
            trainingItem.featuresDetailsButton.Enabled = false;

            // Check if this data is from "dataset" table (which is for ARTHT training data)
            if (dataTable.TableName.Equals("dataset"))
                holdRecordReport_Training_ARTHT(row, trainingItem);
        }
    }
}

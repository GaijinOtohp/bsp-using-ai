using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BSP_Using_AI.DetailsModify.Filters
{
    public partial class DCRemovalUserControl : UserControl
    {
        public DCRemovalUserControl()
        {
            InitializeComponent();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EventHandlers.deleteToolStripMenuItem_Click(this);
        }

        private void dcValueRemoveCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            // Check if the box is checked or not
            if ((sender as CheckBox).Checked)
                // If yes then the box is checked
                // Set the filter value to 1 in _filtersHashtable
                ((object[])((FormDetailsModify)this.FindForm())._filteresHashtable[this.Name])[1] = 1;
            else
                // Set the filter value to 0
                ((object[])((FormDetailsModify)this.FindForm())._filteresHashtable[this.Name])[1] = 0;

            // Check if the auto apply is checked
            if (((FormDetailsModify)this.FindForm()).autoApplyCheckBox.Checked)
                // If yes then apply the changes
                ((FormDetailsModify)this.FindForm()).applyButton_Click(null, null);
        }

        private void applyAfterTransformCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            // Check if the box is checked or not
            if ((sender as CheckBox).Checked)
                // If yes then the box is checked
                // Set the filter value to 1 in _filtersHashtable
                ((object[])((FormDetailsModify)this.FindForm())._filteresHashtable[this.Name])[2] = 1;
            else
                // Set the filter value to 0
                ((object[])((FormDetailsModify)this.FindForm())._filteresHashtable[this.Name])[2] = 0;

            // Check if the auto apply is checked
            if (((FormDetailsModify)this.FindForm()).autoApplyCheckBox.Checked)
                // If yes then apply the changes
                ((FormDetailsModify)this.FindForm()).applyButton_Click(null, null);
        }
    }
}

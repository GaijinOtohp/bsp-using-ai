using Biological_Signal_Processing_Using_AI.DetailsModify.Filters;
using BSP_Using_AI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Biological_Signal_Processing_Using_AI.DetailsModify.FiltersControls
{
    public partial class AddGWNUserControl : UserControl
    {
        AddGWN Filter;

        public AddGWNUserControl(AddGWN filter)
        {
            InitializeComponent();

            Filter = filter;
            Filter._ignoreEvent = true;
            snrDBTextBox.Text = Filter.SNRdb.ToString();
            Filter._ignoreEvent = false;
        }

        private void windowSizeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.keypressNegPosNumbersAndDecimalOnly(sender, e);
        }

        private void snrDBTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!Filter._ignoreEvent)
            {
                Filter._ignoreEvent = true;
                double snrDB = 0;
                if (snrDBTextBox.Text.Length > 0 && !snrDBTextBox.Text.Equals("."))
                    snrDB = double.Parse(snrDBTextBox.Text);
                Filter.SNRdb = snrDB;
                Filter._ignoreEvent = false;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filter.RemoveFilter();
        }

        private void activateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!Filter._ignoreEvent)
            {
                Filter._ignoreEvent = true;
                Filter.ActivateGenerally(activateCheckBox.Checked);
                Filter._ignoreEvent = false;
            }
        }
    }
}

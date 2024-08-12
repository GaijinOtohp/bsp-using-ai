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
    public partial class MedianFilterUserControl : UserControl
    {
        MedianFilter Filter;

        public MedianFilterUserControl(MedianFilter filter)
        {
            InitializeComponent();

            Filter = filter;
        }

        private void windowSizeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.keypressNumbersAndDecimalOnly(sender, e);
        }

        private void windowSizeTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!Filter._ignoreEvent)
            {
                Filter._ignoreEvent = true;
                int windowSize = 0;
                if (windowSizeTextBox.Text.Length > 0 && !windowSizeTextBox.Text.Equals("."))
                    windowSize = int.Parse(windowSizeTextBox.Text);
                Filter.SetWindowSize(windowSize);
                Filter._ignoreEvent = false;
            }
        }

        private void strideSizeTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!Filter._ignoreEvent)
            {
                Filter._ignoreEvent = true;
                int strideSize = 0;
                if (strideSizeTextBox.Text.Length > 0 && !strideSizeTextBox.Text.Equals("."))
                    strideSize = int.Parse(strideSizeTextBox.Text);
                Filter.SetStrideSize(strideSize);
                Filter._ignoreEvent = false;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filter.RemoveFilter();
        }
    }
}

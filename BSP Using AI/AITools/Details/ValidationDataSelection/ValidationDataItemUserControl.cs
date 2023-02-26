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

namespace Biological_Signal_Processing_Using_AI.AITools.Details.ValidationDataSelection
{
    public partial class ValidationDataItemUserControl : UserControl
    {
        public delegate void SetSignalOrder(ValidationDataItemUserControl item, int newOrderIndex);

        private SetSignalOrder _SetSignalOrder;

        private int _maxOrderIndex;

        public bool _ignoreEvent = false;

        public ValidationDataItemUserControl(SetSignalOrder setSignalOrder, int maxtOrderIndex)
        {
            InitializeComponent();
            _SetSignalOrder = setSignalOrder;
            _maxOrderIndex = maxtOrderIndex;
        }

        private void orderTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.textBoxNumberOnly(sender, e);
        }

        private void orderTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!orderTextBox.Text.Equals("") && !orderTextBox.Text.Equals(".") && !_ignoreEvent)
            {
                int newOrderIndex = (int)double.Parse(orderTextBox.Text);
                if (newOrderIndex < _maxOrderIndex)
                    _SetSignalOrder(this, newOrderIndex);
            }
        }
    }
}

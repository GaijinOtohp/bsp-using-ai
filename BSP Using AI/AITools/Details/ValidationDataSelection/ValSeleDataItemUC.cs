﻿using BSP_Using_AI;
using System;
using System.Windows.Forms;

namespace Biological_Signal_Processing_Using_AI.AITools.Details.ValidationDataSelection
{
    public partial class ValSeleDataItemUC : UserControl
    {
        public delegate void SetSignalOrder(ValSeleDataItemUC item, int newOrderIndex);

        private SetSignalOrder _SetSignalOrder;

        private int _maxOrderIndex;

        public bool _ignoreEvent = false;

        public ValSeleDataItemUC(SetSignalOrder setSignalOrder, int maxtOrderIndex)
        {
            InitializeComponent();
            _SetSignalOrder = setSignalOrder;
            _maxOrderIndex = maxtOrderIndex;
        }

        private void orderTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.keypressNumbersAndDecimalOnly(sender, e);
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

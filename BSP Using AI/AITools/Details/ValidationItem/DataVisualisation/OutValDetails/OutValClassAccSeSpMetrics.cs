﻿using System.Windows.Forms;

namespace BSP_Using_AI.AITools.Details.ValidationItem.DataVisualisation.OutValDetails
{
    public partial class OutValClassAccSeSpMetrics : UserControl
    {
        public OutValClassAccSeSpMetrics()
        {
            InitializeComponent();
        }

        private void classificationThresholdTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventHandlers.keypressNumbersAndDecimalOnly(sender, e);
        }
    }
}

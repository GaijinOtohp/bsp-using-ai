using Biological_Signal_Processing_Using_AI.Garage;
using BSP_Using_AI.DetailsModify;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Biological_Signal_Processing_Using_AI.DetailsModify.Annotations.AnnotationsStructures;
using static Biological_Signal_Processing_Using_AI.Structures;

namespace Biological_Signal_Processing_Using_AI.DetailsModify.Annotations
{
    public partial class AnnotationItemUserControl : UserControl
    {
        public AnnotationECG _Anno;
        private FormDetailsModify _FormDetailsModify;

        public AnnotationItemUserControl(AnnotationECG anno, FormDetailsModify formDetailsModify)
        {
            InitializeComponent();

            _Anno = anno;
            _FormDetailsModify = formDetailsModify;

            // Present the annotation values
            SetVals(_Anno.Name, _Anno.GetIndexes().starting, _Anno.GetIndexes().ending);

            // Change the starting index's label according to the type of the annotation
            if (_Anno.GetAnnotationType().Equals(AnnotationType.Point))
            {
                startingIndexLabel.Text = "Index";
                endingIndexLabel.Visible = false;
                endingIndexTextBox.Visible = false;
            }
        }

        private void SetVals(string name, int startingIndex, int endingIndex)
        {
            nameCheckBox.Text = name;
            startingIndexTextBox.Text = startingIndex.ToString();
            endingIndexTextBox.Text = endingIndex.ToString();
        }

        public void Remove()
        {
            // Remove the annotation from _CWDAnnotationData
            _Anno.Remove();
            // Update the annotations in the plots
            _FormDetailsModify.UpdateAnnotationsPlots();
            // Remove this Usercontrol and dispose it
            _FormDetailsModify.featuresTableLayoutPanel.Controls.Remove(this);
            this.Dispose();
        }

        public void SetNewVals(string name, int startingIndex, int endngIndex)
        {
            // Set the new values of the annotation into _CWDAnnotationData
            _Anno.SetNewVals(name, startingIndex, endngIndex);
            // Update the annotations in the plots
            _FormDetailsModify.UpdateAnnotationsPlots();
            // Update this user control
            SetVals(name, startingIndex, endngIndex);
        }

        private void AnnotationItemUserControl_MouseEnter(object sender, EventArgs e)
        {
            SignalPlot signalPlot = (SignalPlot)_FormDetailsModify._Plots[SANamings.Signal];
            BubblePlot selectionBubble = (BubblePlot)_FormDetailsModify._Plots[SANamings.Selection];
            HSpan intervalSpan = (HSpan)_FormDetailsModify._Plots[SANamings.IntervalHorizSpan];

            // Get annotation indexes
            (int starting, int ending) = _Anno.GetIndexes();

            // Check if this control belongs to a point or an interval annotation
            if (_Anno.GetAnnotationType() == AnnotationType.Point)
            {
                // If yes then add a new bubble as a point annotation selection
                selectionBubble.Add(starting / signalPlot.SampleRate + signalPlot.OffsetX, signalPlot.Ys[starting], 5, Color.Red, 2, ForeColor);
                _FormDetailsModify.signalChart.Refresh();
            }
            else
            {
                intervalSpan.X1 = starting / signalPlot.SampleRate + signalPlot.OffsetX;
                intervalSpan.X2 = ending / signalPlot.SampleRate + signalPlot.OffsetX;
                GeneralTools.TimeSpanVisibility(_FormDetailsModify.signalChart, intervalSpan, true);
            }
        }

        private void AnnotationItemUserControl_MouseLeave(object sender, EventArgs e)
        {
            AnnotationItemUserControl annoItem = this is AnnotationItemUserControl ? this : (AnnotationItemUserControl)this.Parent;
            Point position = annoItem.PointToClient(System.Windows.Forms.Cursor.Position);

            // Check if the mouse point is outside annoItem
            if (annoItem.Height <= position.Y || annoItem.Width <= position.X || position.Y < 0 || position.X < 0)
            {
                // Hide every selection
                BubblePlot selectionBubble = (BubblePlot)_FormDetailsModify._Plots[SANamings.Selection];
                HSpan intervalSpan = (HSpan)_FormDetailsModify._Plots[SANamings.IntervalHorizSpan];

                selectionBubble.Clear();
                GeneralTools.TimeSpanVisibility(_FormDetailsModify.signalChart, intervalSpan, false);
            }
        }

        private void AnnotationItemUserControl_Click(object sender, EventArgs e)
        {
            AnnotationModify annotationModify = new AnnotationModify(this);
            annotationModify.Show();
        }

        private void deleteCurrentAnnotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show message for confirming the action
            DialogResult dialogResult = MessageBox.Show("Are you sure about removing the annotation \"" + _Anno.Name + "\"?", "Action confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                this.Remove();
            }
        }

        private void deleteAllSelectedAnnotationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show message for confirming the action
            DialogResult dialogResult = MessageBox.Show("Are you sure about removing all of the selected annotations?", "Action confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                // Iterate through all annotations in featuresTableLayoutPanel
                List<AnnotationItemUserControl> annoItems = _FormDetailsModify.featuresTableLayoutPanel.Controls.OfType<AnnotationItemUserControl>().ToList();
                foreach (AnnotationItemUserControl item in annoItems)
                    // Check if item is selected to be removed
                    if (item.nameCheckBox.Checked && item != this)
                    {
                        // Remove the annotation from _CWDAnnotationData
                        item._Anno.Remove();
                        // Remove this Usercontrol and dispose it
                        _FormDetailsModify.featuresTableLayoutPanel.Controls.Remove(item);
                        item.Dispose();
                    }
                // Check if current annotation item is checked to be deleted
                if (this.nameCheckBox.Checked)
                    // If yes then remove it
                    Remove();
                else
                    // Update the annotations in the plots
                    _FormDetailsModify.UpdateAnnotationsPlots();
            }
        }

        private void nameCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Get current item's index in featuresTableLayoutPanel
            TableLayoutPanel featuresFlowLayout = _FormDetailsModify.featuresTableLayoutPanel;
            int currentItemIndx = featuresFlowLayout.Controls.GetChildIndex(this);
            // Check if "Shift" button is clicked and a previous selected item
            if (_FormDetailsModify._AnnoKeys.shift && _FormDetailsModify._lastSelectedItem_shift != -1)
            {
                // If yes then alter the selection status of each item in the shifted interval
                int start = _FormDetailsModify._lastSelectedItem_shift + 1;
                int end = currentItemIndx;
                if (start > end)
                {
                    start = currentItemIndx + 1;
                    end = _FormDetailsModify._lastSelectedItem_shift;
                }
                for (int i = start; i < end; i++)
                        ((AnnotationItemUserControl)featuresFlowLayout.Controls[i]).nameCheckBox.Checked = !((AnnotationItemUserControl)featuresFlowLayout.Controls[i]).nameCheckBox.Checked;
            }

            // Update _lastSelectedItem_shift
            _FormDetailsModify._lastSelectedItem_shift = currentItemIndx;
        }
    }
}

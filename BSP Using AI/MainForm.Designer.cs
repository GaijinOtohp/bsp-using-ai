using System;
using static Biological_Signal_Processing_Using_AI.AITools.AIModels;

namespace BSP_Using_AI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            // Close tensorflow thread server
            _tFBackThread._queue.Enqueue(new QueueSignalInfo()
            {
                TargetFunc = "Close",
                CallingClass = "MainForm",
            });
            _tFBackThread._signal.Set();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.aiToolsButton = new System.Windows.Forms.Button();
            this.signalsComparatorButton = new System.Windows.Forms.Button();
            this.signalsCollectorButton = new System.Windows.Forms.Button();
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.signalsFlowLayout = new BSP_Using_AI.CustomFlowLayoutPanel();
            this.SuspendLayout();
            // 
            // aiToolsButton
            // 
            this.aiToolsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.aiToolsButton.Location = new System.Drawing.Point(801, 10);
            this.aiToolsButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.aiToolsButton.Name = "aiToolsButton";
            this.aiToolsButton.Size = new System.Drawing.Size(111, 25);
            this.aiToolsButton.TabIndex = 1;
            this.aiToolsButton.Text = "AI tools";
            this.aiToolsButton.UseVisualStyleBackColor = true;
            this.aiToolsButton.Click += new System.EventHandler(this.ChoseRecapButton_Click);
            // 
            // signalsComparatorButton
            // 
            this.signalsComparatorButton.Location = new System.Drawing.Point(9, 10);
            this.signalsComparatorButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.signalsComparatorButton.Name = "signalsComparatorButton";
            this.signalsComparatorButton.Size = new System.Drawing.Size(111, 25);
            this.signalsComparatorButton.TabIndex = 2;
            this.signalsComparatorButton.Text = "Signals comparator";
            this.signalsComparatorButton.UseVisualStyleBackColor = true;
            this.signalsComparatorButton.Click += new System.EventHandler(this.signalsComparatorButton_Click);
            // 
            // signalsCollectorButton
            // 
            this.signalsCollectorButton.Location = new System.Drawing.Point(124, 10);
            this.signalsCollectorButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.signalsCollectorButton.Name = "signalsCollectorButton";
            this.signalsCollectorButton.Size = new System.Drawing.Size(111, 25);
            this.signalsCollectorButton.TabIndex = 3;
            this.signalsCollectorButton.Text = "Signals collector";
            this.signalsCollectorButton.UseVisualStyleBackColor = true;
            this.signalsCollectorButton.Click += new System.EventHandler(this.signalsCollectorButton_Click);
            // 
            // vScrollBar
            // 
            this.vScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScrollBar.LargeChange = 356;
            this.vScrollBar.Location = new System.Drawing.Point(898, 41);
            this.vScrollBar.Maximum = 355;
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(21, 356);
            this.vScrollBar.SmallChange = 20;
            this.vScrollBar.TabIndex = 26;
            this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // signalsFlowLayout
            // 
            this.signalsFlowLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.signalsFlowLayout.Location = new System.Drawing.Point(0, 41);
            this.signalsFlowLayout.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.signalsFlowLayout.Name = "signalsFlowLayout";
            this.signalsFlowLayout.Size = new System.Drawing.Size(896, 356);
            this.signalsFlowLayout.TabIndex = 27;
            this.signalsFlowLayout.SizeChanged += new System.EventHandler(this.signalsFlowLayout_SizeChanged);
            this.signalsFlowLayout.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.signalExhibitor_MouseWheelScroll);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 398);
            this.Controls.Add(this.signalsFlowLayout);
            this.Controls.Add(this.vScrollBar);
            this.Controls.Add(this.signalsCollectorButton);
            this.Controls.Add(this.signalsComparatorButton);
            this.Controls.Add(this.aiToolsButton);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "MainForm";
            this.Text = "BSP Using AI";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button aiToolsButton;
        private System.Windows.Forms.Button signalsComparatorButton;
        private System.Windows.Forms.Button signalsCollectorButton;
        public System.Windows.Forms.VScrollBar vScrollBar;
        private CustomFlowLayoutPanel signalsFlowLayout;
    }
}


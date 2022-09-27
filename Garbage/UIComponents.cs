using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

namespace BSP_Using_AI
{
    class UIComponents
    {
        /**
         * Function for signal holder creation
         */
        public static Panel CreateSignalHolder(Control parent, String name, Nullable<Size> size, Point localtion, int tabIndex)
        {
            /* using (Panel signalHolder = new Panel()
             {
                 Anchor = ((AnchorStyles)((AnchorStyles.Left | AnchorStyles.Right))),
                 AutoSize = true,
                 Location = localtion,
                 Name = name,
                 Parent = parent,
                 Size = (Size)size,
                 TabIndex = tabIndex
             })
             {
                 signalHolder.Controls.Add(CreateChart(signalHolder, "Signal Exhibitor", new Size(716, 207), new Point(8, 8), 0, "Signal Exhibitor"));
                 signalHolder.Controls.Add(CreateButton("Choose File", new Size(144, 32), new Point(740, 8)));
                 return signalHolder;
             }*/
            Panel signalHolder = new Panel();
            signalHolder.Anchor = ((AnchorStyles)((AnchorStyles.Left | AnchorStyles.Right)));
            signalHolder.AutoSize = true;
            signalHolder.Location = localtion;
            signalHolder.Name = name;
            signalHolder.Parent = parent;
            signalHolder.Size = (Size)size;
            signalHolder.TabIndex = tabIndex;
            signalHolder.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            signalHolder.Controls.Add(CreateButton("Choose File", new Size(144, 32), new Point(740, 8)));
            return signalHolder;
        }

        public static Chart CreateChart(Control parent, String name, Nullable<Size> size, Point localtion, int tabIndex, String text)
        {
            using (Chart signalExhibitor = new Chart() 
            {
                Anchor = ((AnchorStyles)((AnchorStyles.Left | AnchorStyles.Right))),
                Location = localtion,
                //new Point(8, 8),
                Name = name,
                Palette = ChartColorPalette.SemiTransparent,
                Parent = parent,
                Size = (Size)size,
                //new Size(716, 207),
                TabIndex = tabIndex,
                Text = text
        })
            {
                return signalExhibitor;
            }
        }
        /**
         * Function for button creation
         */
        public static Button CreateButton(String text, Nullable<Size> size, Point localtion)
        {
            Button button = new Button();
            if (size == null)
            {
                button.Size = new Size(40, 30);
            }
            else
            {
                button.Size = (Size)size;
            }
            button.Margin = new Padding(8);
            button.Location = localtion;
            button.Text = text;
            return button;
        }

        /**
         * Function for Text box creation
         */
        public static TextBox CreateTextBox(String textHint, Boolean password, Boolean numbersOnly)
        {
            TextBox textBox = new TextBox();
            textBox.Margin = new Padding(8);
            textBox.Text = textHint;
            return textBox;
        }

        /**
         * Function for label creation
         */
        public static Label CreateLabel(String text, Nullable<Size> size, Point localtion)
        {
            Label label = new Label();
            label.Margin = new Padding(8);
            if (size == null)
            {
                label.AutoSize = true;
            }
            else
            {
                label.Size = (Size)size;
            }
            label.Location = localtion;
            label.Text = text;
            return label;
        }
    }
}

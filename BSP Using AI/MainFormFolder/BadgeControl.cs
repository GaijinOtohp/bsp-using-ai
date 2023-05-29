using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BSP_Using_AI.MainFormFolder
{
    static class BadgeControl
    {
        private static List<Control> controls = new List<Control>();

        static public bool AddBadgeTo(Control ctl, string Text)
        {
            if (controls.Contains(ctl)) return false;

            Badge badge = new Badge();
            badge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            badge.Visible = false;
            badge.AutoSize = true;
            badge.Padding = new Padding(1);
            badge.Text = Text;
            badge.BackColor = Color.Transparent;
            controls.Add(ctl);
            ctl.Parent.Controls.Add(badge);
            badge.BringToFront();
            SetPosition(badge, ctl);

            return true;
        }

        static public bool RemoveBadgeFrom(Control ctl)
        {
            Badge badge = GetBadge(ctl);
            if (badge != null)
            {
                ctl.Controls.Remove(badge);
                controls.Remove(ctl);
                return true;
            }
            else return false;
        }

        static public void SetBadgeText(Control ctl, string newText)
        {
            Badge badge = GetBadge(ctl);
            if (badge != null)
            {
                badge.Text = newText;
                SetPosition(badge, ctl);
            }
        }

        static public string GetBadgeText(Control ctl)
        {
            Badge badge = GetBadge(ctl);
            if (badge != null) return badge.Text;
            return "";
        }

        static private void SetPosition(Badge badge, Control ctl)
        {
            badge.Location = new Point(ctl.Location.X - 5,
                                       ctl.Height);

        }

        static public void SetClickAction(Control ctl, Action<Control> action)
        {
            Badge badge = GetBadge(ctl);
            if (badge != null) badge.ClickEvent = action;
        }

        static public Badge GetBadge(Control ctl)
        {
            for (int c = 0; c < ctl.Controls.Count; c++)
                if (ctl.Controls[c] is Badge) return ctl.Controls[c] as Badge;
            return null;
        }


        public class Badge : Label
        {
            Color BackColor = Color.Red;
            Color ForeColor = Color.White;
            Font font = new Font("Sans Serif", 10f);

            public Action<Control> ClickEvent;

            public Badge() { }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.FillEllipse(new SolidBrush(BackColor), this.ClientRectangle);
                e.Graphics.DrawString(Text, font, new SolidBrush(ForeColor), 2, 0);
            }

            protected override void OnClick(EventArgs e)
            {
                ClickEvent(this);
            }

        }
    }
}

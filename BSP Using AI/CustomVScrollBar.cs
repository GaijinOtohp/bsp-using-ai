using System.Windows.Forms;

namespace Biological_Signal_Processing_Using_AI
{
    public class CustomVScrollBar : VScrollBar
    {
        public void SetMax(int max)
        {
            Maximum = max + (LargeChange - SmallChange);
        }

        public int GetMax()
        {
            return Maximum - (LargeChange - SmallChange);
        }
    }
}

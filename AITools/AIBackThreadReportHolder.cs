using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP_Using_AI.AITools
{
    public interface AIBackThreadReportHolder
    {
        void holdAIReport(object[] paramms, String callingClassName);
    }
}

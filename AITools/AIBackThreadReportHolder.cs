using System;

namespace BSP_Using_AI.AITools
{
    public interface AIBackThreadReportHolder
    {
        void holdAIReport(object[] paramms, String callingClassName);
    }
}

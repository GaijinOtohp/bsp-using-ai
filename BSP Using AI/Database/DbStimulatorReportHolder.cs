using System;
using System.Data;

namespace BSP_Using_AI.Database
{
    public interface DbStimulatorReportHolder
    {
        void holdRecordReport(DataTable dataTable, String callingClassName);
    }
}

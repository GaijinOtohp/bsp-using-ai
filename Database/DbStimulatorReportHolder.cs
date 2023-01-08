using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP_Using_AI.Database
{
    interface DbStimulatorReportHolder
    {
        void holdRecordReport(DataTable dataTable, String callingClassName);
    }
}

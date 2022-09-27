using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP_Using_AI.Database
{
    interface DbStimulatorReportHolder
    {
        void holdRecordReport(List<Object[]> records, String callingClassName);
    }
}

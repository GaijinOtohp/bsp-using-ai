using System;

namespace BSP_Using_AI.AITools
{
    public interface AIBackThreadReportHolder
    {
        public enum AIReportType
        {
            FittingProgress,
            FittingComplete,
            CreateModel
        }

        public class AIReport
        {
            public AIReportType ReportType;
        }

        public class FittingProgAIReport : AIReport
        {
            public string ModelName;
            public int fitProgress;
            public int fitMaxProgress;
        }

        public class FittingCompAIReport : AIReport
        {
            public string ModelName;
            public long datasetSize;
        }

        void holdAIReport(AIReport report, string callingClassName);

        public delegate void FittingProgAIReportDelegate(int currentProgress, int maxProgress);
    }
}

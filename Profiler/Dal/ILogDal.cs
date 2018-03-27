using System.Collections.Generic;

namespace Profiling
{
    internal interface ILogDal
    {
        void AddWarning(WarningItem item);
        void AddLog(LogItem item);
        void UpdateStatistics(IEnumerable<StatisticValue> valueList);
        bool IsAlive();
        void DeleteStatistic(int _pid);
    }
}
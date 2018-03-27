using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Profiling
{
    public interface IMultiLogger: IDisposable
    {
        void AddWarningByThreshold(string callerName, string callParams, long timeElapsed);
        void WriteStatistic(DateTime _prevState, DateTime currentTime, IEnumerable<StatisticValue> statistic);
        void Debug(string data, [CallerMemberName] string member = "");
        void Warn(string data, [CallerMemberName] string member = "");
        void Error(string data, [CallerMemberName] string member = "");
        void Error(Exception ex, [CallerMemberName] string member = "");
        void Sys(string data, [CallerMemberName] string member = "");
    }
}
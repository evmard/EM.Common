using System;

namespace Profiling
{
    internal interface IProfilerEngine: IDisposable
    {
        long Threshold { get; }

        void AddWarningByThreshold(string _callerName, string _callParams, long timeElapsed);
        void AddMonitoring(string _callerName, long timeElapsed);
        void DoProfile(Action actor, string actorName, string callParams);
    }
}
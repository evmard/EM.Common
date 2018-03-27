using System;
using System.Diagnostics;

namespace Profiling
{
    internal class ProfilingScope : IDisposable
    {
        private readonly IProfilerEngine _profiler;
        private readonly Stopwatch _watcher;
        private readonly string _callerName;
        private readonly string _callParams;

        public ProfilingScope(IProfilerEngine profiler, string callerName, string callParams)
        {
            _profiler = profiler;
            _watcher = new Stopwatch();
            _callerName = callerName;
            _callParams = callParams;
            _watcher.Start();
        }

        public void Dispose()
        {
            _watcher.Stop();
            var timeOfExit = DateTime.Now;
            long threshold = _profiler.Threshold;
            var timeElapsed = _watcher.ElapsedMilliseconds;
            if (timeElapsed > threshold)
                _profiler.AddWarningByThreshold(_callerName, _callParams, timeElapsed);

            _profiler.AddMonitoring(_callerName, timeElapsed);
        }
    }
}

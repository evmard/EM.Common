using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace Profiling
{
    internal class ProfilerEngine : IProfilerEngine, IDisposable
    {
        private IMultiLogger _logger;
        private ConcurrentDictionary<string, MonitoringItem> _monitoringDict;
        private readonly Timer _timer;
        private DateTime _prevState;
        private long _pid = Process.GetCurrentProcess().Id;

        internal ProfilerEngine(IMultiLogger logger, long monitorPeriod)
        {
            _logger = logger;
            _monitoringDict = new ConcurrentDictionary<string, MonitoringItem>();
            _timer = new Timer(monitorPeriod);
            _timer.AutoReset = true;
            _timer.Elapsed += WriteStatistic;
            _timer.Disposed += WriteStatistic;
            _prevState = DateTime.Now;
            _timer.Start();
        }

        private void WriteStatistic(object sender, EventArgs e)
        {
            var statistic = new List<StatisticValue>(_monitoringDict.Values.Select(item => item.GetValues()));
            var currentTime = DateTime.Now;
            _logger.WriteStatistic(_prevState, currentTime, statistic);
            _prevState = currentTime;
        }

        public void DoProfile(Action actor, string actorName, string callParams)
        {
            using (var scope = new ProfilingScope(this, actorName, callParams))
            {
                actor();
            }
        }

        public long Threshold { get { return 2000; } }

        public void AddMonitoring(string actorName, long timeElapsed)
        {
            MonitoringItem monitorItem;
            if (!_monitoringDict.TryGetValue(actorName, out monitorItem))
            {
                monitorItem = new MonitoringItem(_pid, actorName);
                _monitoringDict.TryAdd(actorName, monitorItem);
            }

            monitorItem.AddCall(timeElapsed);
        }

        public void AddWarningByThreshold(string actorName, string callParams, long timeElapsed)
        {
            _logger.AddWarningByThreshold(actorName, callParams, timeElapsed);
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();            
        }
    }
}

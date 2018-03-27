using log4net;
using log4net.Config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Timers;
using Utils;

namespace Profiling
{
    public class MultiLogger : IMultiLogger, IDisposable
    {
        private readonly System.Timers.Timer _timer;
        private readonly LogLevel _logLevel;
        private readonly LogMethod _logMethods;
        private readonly ILogDal _db;
        private readonly ILog _fileLog = LogManager.GetLogger("FILELOG");
        private readonly ILog _consoleLog = LogManager.GetLogger("CONSOLELOG");
        private bool _dbIsAvaliable;

        internal MultiLogger(ILogDal db)
        {
            
            XmlConfigurator.Configure();
            _db = db;

            _pid = Process.GetCurrentProcess().Id;

            _warningQueue = new ConcurrentQueue<WarningItem>();
            _logQueue = new ConcurrentQueue<LogItem>();
            _statisticQueue = new ConcurrentQueue<Statistic>();
            _timer = new System.Timers.Timer(1000);
            _timer.AutoReset = true;
            _timer.Elapsed += SaveAll;
            _logLevel = ConfigUtils.GetEnum("LogLevel", LogLevel.Warning);
            _logMethods = ConfigUtils.GetEnum("LogMethods", LogMethod.DataBase | LogMethod.File);

            try
            {
                if (Environment.UserInteractive)
                    Console.OutputEncoding = Encoding.UTF8;
            }
            catch { }

            if (_logMethods.HasFlag(LogMethod.DataBase))
            {
                _dbIsAvaliable = _db.IsAlive();
                var dbMessage = _dbIsAvaliable
                    ? "Log database connected"
                    : "Log database unavaliable!";
                var time = DateTime.Now.ToString("yy.MM.dd HH:mm:ss.fff");
                var level = _dbIsAvaliable ? LogLevel.System : LogLevel.Error;
                WriteToConsole(level, $"{time} PID:{_pid}\t{LogLevel.System}\t{dbMessage}");
                _fileLog.Error($"{time} PID:{_pid}\t{LogLevel.System}\t{dbMessage}");
            }
            _timer.Start();
        }

        internal void CheckDb()
        {

        }

        private void SaveAll(object sender, EventArgs e)
        {
            try
            {
                SaveWarnings();
            }
            catch(Exception ex)
            {
                WriteToConsole(LogLevel.Error, $"SaveWarnings Error: {ex.Message}");
            }
            try
            {
                SaveLogs();
            }
            catch (Exception ex)
            {
                WriteToConsole(LogLevel.Error, $"SaveLogs Error: {ex.Message}");                
            }
            try
            {
                SaveSatistics();
            }
            catch (Exception ex)
            {
                WriteToConsole(LogLevel.Error, $"SaveSatistics Error: {ex.Message}");
            }
        }

        private void WriteToConsole(LogLevel level, string msg)
        {
            if (!Environment.UserInteractive)
                return;

            ConsoleColor originalColor = Console.ForegroundColor;
            ConsoleColor msgColor;
            switch (level)
            {
                case LogLevel.Debug:
                    msgColor = ConsoleColor.DarkCyan;
                    break;
                case LogLevel.Warning:
                    msgColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    msgColor = ConsoleColor.Red;
                    break;
                case LogLevel.System:
                    msgColor = ConsoleColor.Green;
                    break;
                default:
                    msgColor = originalColor;
                    break;
            }
            Console.ForegroundColor = msgColor;
            _consoleLog.Error(msg);            
            Console.ForegroundColor = originalColor;
        }

        private void SaveSatistics()
        {
            while (!_statisticQueue.IsEmpty)
            {
                Statistic item;
                if (_statisticQueue.TryDequeue(out item))
                    SaveStatistic(item);
            }
        }

        private void SaveStatistic(Statistic item)
        {
            var strStat = item.ToString();
            if (_logMethods.HasFlag(LogMethod.Console))
            {
                if (string.IsNullOrEmpty(strStat))
                    WriteToConsole(LogLevel.Debug, strStat);
            }

            if (_logMethods.HasFlag(LogMethod.File))
            {                
                if (string.IsNullOrEmpty(strStat))
                    _fileLog.Error(strStat);
            }

            if (_logMethods.HasFlag(LogMethod.DataBase) && _dbIsAvaliable)
                _db.UpdateStatistics(item.ValueList);
        }

        private void SaveLogs()
        {
            while (!_logQueue.IsEmpty)
            {
                LogItem item;
                if (_logQueue.TryDequeue(out item))
                    SaveLogItem(item);
            }
        }

        private void SaveLogItem(LogItem item)
        {
            if (_logMethods.HasFlag(LogMethod.Console) || item.Level == LogLevel.System)
                WriteToConsole(item.Level, item.ToString());

            if (_logMethods.HasFlag(LogMethod.File))
            {
                var logStr = item.ToString();
                _fileLog.Error(logStr);

                
            }

            if (_logMethods.HasFlag(LogMethod.DataBase) && _dbIsAvaliable)
                _db.AddLog(item);
        }

        private void SaveWarnings()
        {
            while(!_warningQueue.IsEmpty)
            {
                WarningItem item;
                if (_warningQueue.TryDequeue(out item))
                    SaveWarning(item);
            }
        }

        private void SaveWarning(WarningItem item)
        {
            if (_logMethods.HasFlag(LogMethod.Console))
                WriteToConsole(LogLevel.Warning, item.ToString());

            if (_logMethods.HasFlag(LogMethod.File))
                _fileLog.Error(item.ToString());

            if (_logMethods.HasFlag(LogMethod.DataBase) && _dbIsAvaliable)
                _db.AddWarning(item);
        }

        //private static MultiLogger _instance;
        private static object _instLock = new object();

        private int _pid;
        private ConcurrentQueue<WarningItem> _warningQueue;
        private ConcurrentQueue<LogItem> _logQueue;
        private ConcurrentQueue<Statistic> _statisticQueue;

        public void AddWarningByThreshold(string callerName, string callParams, long timeElapsed)
        {
            var wtime = DateTime.Now;
            var tid = Thread.CurrentThread.ManagedThreadId;
            _logQueue.Enqueue(new LogItem
            {
                CallerName = callerName,
                Level = LogLevel.Warning,
                Message = $"Actor {callerName}\tElapsed:{timeElapsed}\tparams:{callParams}",
                ProcessId = _pid,
                WarnTime = wtime,
                TreadId = tid
            });

            _warningQueue.Enqueue(new WarningItem
            {
                ProcessId = _pid,
                WarnTime = wtime,
                CallerName = callerName,
                CallParams = callParams,
                TimeElapsed = timeElapsed,
                TreadId = tid
            });
        }

        public void Error(string data, [CallerMemberName] string member = "")
        {
            AddLogToQueue(data, member, LogLevel.Error);
        }

        public void Debug(string data, [CallerMemberName] string member = "")
        {
            if (_logLevel <= LogLevel.Debug)
                AddLogToQueue(data, member, LogLevel.Debug);
        }

        public void Sys(string data, [CallerMemberName] string member = "")
        {
            AddLogToQueue(data, member, LogLevel.System);
        }

        public void Warn(string data, [CallerMemberName] string member = "")
        {
            if (_logLevel <= LogLevel.Warning)
                AddLogToQueue(data, member, LogLevel.Warning);
        }

        public void WriteStatistic(DateTime _prevState, DateTime currentTime, IEnumerable<StatisticValue> statistic)
        {
            var stat = new Statistic
            {
                From = _prevState,
                To = currentTime,
                ValueList = new List<StatisticValue>(statistic)
            };

            _statisticQueue.Enqueue(stat);
        }

        private void AddLogToQueue(string data, string member, LogLevel level)
        {
            var tid = Thread.CurrentThread.ManagedThreadId;
            _logQueue.Enqueue(new LogItem
            {
                CallerName = member,
                Level = level,
                Message = data,
                ProcessId = _pid,
                WarnTime = DateTime.Now,
                TreadId = tid
            });
        }

        public void Dispose()
        {
            _timer.Stop();
            SaveAll(this, null);
            if (_logMethods.HasFlag(LogMethod.DataBase) && _dbIsAvaliable)
            {
                try { _db.DeleteStatistic(_pid); }
                catch (Exception e) { _fileLog.Error($"DbLog Error {e.Message}"); }
            }
        }

        public void Error(Exception ex, [CallerMemberName] string member = "")
        {
            var error = GetFullException(ex);
            error += ex.StackTrace;
            Error(error, member);
        }

        private string GetFullException(Exception ex, string parentErr = "")
        {
            var err = $"{parentErr}{ex.Message}\n";
            if (ex.InnerException != null)
                return GetFullException(ex.InnerException, err);

            return err;
        }
    }

    [Flags]
    public enum LogMethod
    {
        Console = 0x01,
        File = 0x02,
        DataBase = 0x04
    }
}

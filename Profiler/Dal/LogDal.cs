using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Profiling
{
    public class LogDal : ILogDal
    {
        private readonly IDbContextFactory<LogContext> _contextFactory;
        public LogDal(IDbContextFactory<LogContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public void AddLog(LogItem item)
        {
            using (var db = _contextFactory.Create())
            {
                db.Logs.Add(item);
                db.SaveChanges();
            }
        }

        public void AddWarning(WarningItem item)
        {
            using (var db = _contextFactory.Create())
            {
                db.Warnings.Add(item);
                db.SaveChanges();
            }
        }

        public void DeleteStatistic(int processId)
        {
            using (var db = _contextFactory.Create())
            {
                db.Database.ExecuteSqlCommand($"DELETE FROM [dbo].[StatisticValues] WHERE [ProcessId] = {processId};");
            }
        }

        public bool IsAlive()
        {
            using (var db = _contextFactory.Create())
            {
                try
                {
                    var res = db.Database.SqlQuery<int>("SELECT 1;").ToList();
                    return res.Any();
                }
                catch
                {
                    return false;
                }
            }
        }

        public void UpdateStatistics(IEnumerable<StatisticValue> valueList)
        {
            var newStat = new List<StatisticValue>();
            using (var db = _contextFactory.Create())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                foreach (var item in valueList)
                {
                    var oldStat = db.Statistics.Find(item.Id);
                    if (oldStat != null)
                    {
                        db.Entry(oldStat).CurrentValues.SetValues(item);
                    }
                    else
                    {
                        db.Statistics.Add(item);
                    }
                }
                db.SaveChanges();
            }
        }
    }
}
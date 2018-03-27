using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiling
{
    public class LogContext: DbContext
    {
        public LogContext(string connectionName): base(connectionName) { }

        public DbSet<LogItem> Logs { get; set; }
        public DbSet<StatisticValue> Statistics { get; set; }
        public DbSet<WarningItem> Warnings { get; set; }
    }

    public class LogContextFactory : IDbContextFactory<LogContext>
    {
        private readonly string _connectionName = "LogDb";

        public LogContext Create()
        {
            return new LogContext(_connectionName);
        }
    }
}

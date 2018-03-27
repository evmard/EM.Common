using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Profiling
{
    public class LogItem
    {
        public LogItem()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        [Index]
        public LogLevel Level { get; set; }
        public DateTime WarnTime { get; set; }
        [Index]
        public long ProcessId { get; set; }

        public long TreadId { get; set; }
        public string Message { get; set; }
        [Index]
        [StringLength(512)]
        public string CallerName { get; set; }

        public override string ToString()
        {
            var time = WarnTime.ToString("yy.MM.dd HH:mm:ss.fff");
            return $"{time} PID:{ProcessId} TID:{TreadId}\t{Level}\t{Message}\t{CallerName}";
        }
    }

    public enum LogLevel
    {
        Debug = 0,
        Warning = 1,
        Error = 2,
        System = 3
    }
}
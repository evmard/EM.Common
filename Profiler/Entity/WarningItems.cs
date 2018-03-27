using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Profiling
{
    public class WarningItem
    {

        public WarningItem()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        public DateTime WarnTime { get; set; }
        [Index]
        public long ProcessId { get; set; }

        public long TreadId { get; set; }
        [Index]
        [StringLength(512)]
        public string CallerName { get; set; }
        public string CallParams { get; set; }
        public long TimeElapsed { get; set; }

        public override string ToString()
        {
            var time = WarnTime.ToString("yy.MM.dd HH:mm:ss.fff");
            return $"{time} PID:{ProcessId} TID:{TreadId}\t{CallerName}\tElapsed:{TimeElapsed}\n\tParams:\t{CallParams}";
        }
    }
}
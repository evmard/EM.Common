using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace Profiling
{
    public class StatisticValue
    {
        [Key]
        public Guid Id { get; set; }
        [Index]
        [StringLength(512)]
        public string ActorName { get; set; }
        [Index]
        public long ProcessId { get; set; }
        public long Min { get; set; }
        public long Max { get; set; }
        public long CalledTimes { get; set; }
        public long Summ { get; set; }
        public long Average { get; set; }

        public StatisticValue()
        { }

        public StatisticValue(long pid, string actorName, long min, long max, long callTimes, long summ)
        {
            ProcessId = pid;
            ActorName = actorName;
            Min = min;
            Max = max;
            CalledTimes = callTimes;
            Summ = summ;
            Average = Summ / CalledTimes;
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes($"pid:{ProcessId};{ActorName}"));
                Id = new Guid(hash);
            }
        }

        public override string ToString()
        {
            return $"{ProcessId}\t{ActorName}\t{Min}\t{Max}\t{CalledTimes}\t{Summ}\t{Average}";
        }

        public void Update(StatisticValue item)
        {
            Min = item.Min;
            Max = item.Max;
            CalledTimes = item.CalledTimes;
            Summ = item.Summ;
            Average = Summ / CalledTimes;
        }
    }
}

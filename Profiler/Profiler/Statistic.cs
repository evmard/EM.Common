using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Profiling
{
    public class Statistic
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public List<StatisticValue> ValueList { get; set; }

        public override string ToString()
        {
            if (!ValueList.Any())
                return string.Empty;

            StringBuilder sBuilder = new StringBuilder();
            sBuilder.AppendLine($"============== Statistic from {From} to {To} =====================");
            sBuilder.AppendLine("PID\tActorName\tMin\tMax\tCalledTimes\tSumm\tAverage");
            foreach (var item in ValueList.OrderBy(val => val.ActorName))
                sBuilder.AppendLine(item.ToString());
            sBuilder.AppendLine("========================================================================");
            return sBuilder.ToString();
        }
    }
}
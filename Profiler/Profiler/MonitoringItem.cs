namespace Profiling
{
    public class MonitoringItem
    {
        public string ActorName { get; private set; }

        private long Min  = -1;
        private long Max  = 0;
        private long CalledTimes = 0;
        private long Summ = 0;
        private long PID;

        public MonitoringItem(long pid, string actorName)
        {
            ActorName = actorName;
            PID = pid;
        }

        public void AddCall(long elapsed)
        {
            lock (this)
            {
                ++CalledTimes;

                if (Min < 0 || Min > elapsed)
                    Min = elapsed;
                if (Max < elapsed)
                    Max = elapsed;

                Summ += elapsed;
            }
        }

        public StatisticValue GetValues()
        {
            lock(this)
            {
                return new StatisticValue(PID, ActorName, Min, Max, CalledTimes, Summ);
            }
        }
    }

    
}
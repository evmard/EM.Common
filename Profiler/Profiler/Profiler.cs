using System;
using System.Runtime.CompilerServices;

namespace Profiling
{
    public class Profiler
    {
        private static IProfilerEngine engine;
        private static object locker = new object();
        public static void Do(Action actor, string callParams, [CallerMemberName] string callerName = "")
        {
            if (engine == null)
            {
                lock(locker)
                {
                    if (engine == null)
                        engine = new ProfilerEngine(Logger.Instance, 60000);
                }
            }
            engine.DoProfile(actor, callerName, callParams);
        }

        public static void Dispose()
        {
            lock (locker)
            {
                if (engine != null)
                    engine.Dispose();

                engine = null;
            }
        }
    }
}

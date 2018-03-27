using WinService;
using Profiling;
using System;

namespace ServiceStressTestService
{
    class Program
    {
        static int Main(string[] args)
        {
            int exitCode;
            using (var service = new TestService())
            {
                exitCode = ConsoleUtils.RunService(args, service);
            }

            Profiler.Dispose();
            Logger.Dispose();

            if (Environment.UserInteractive)
            {
                Environment.Exit(exitCode);
            }

            return exitCode;
        }
    }
}
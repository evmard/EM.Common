using System;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace WinService
{
    public static class ConsoleUtils
    {
        public static int RunService<T>(string[] args, T instance)
            where T : IWinService
        {
            var exitCode = 0;
            try
            {
                string serviceName = null;

                if (args.Contains("name", StringComparer.InvariantCultureIgnoreCase))
                {
                    for (var i = 0; i < args.Length; ++i)
                    {
                        if (args[i] == "name" && i + 1 < args.Length)
                        {
                            serviceName = args[i + 1];
                        }
                    }
                }

                if (Environment.UserInteractive)
                    Console.OutputEncoding = Encoding.UTF8;

                if (args.Contains("-install", StringComparer.InvariantCultureIgnoreCase))
                {
                    WindowsServiceInstaller<T>.RuntimeInstall(serviceName);
                }
                else if (args.Contains("-uninstall", StringComparer.InvariantCultureIgnoreCase))
                {
                    WindowsServiceInstaller<T>.RuntimeUnInstall(serviceName);
                }
                else
                {
                    var implementation = instance;

                    if (Environment.UserInteractive)
                        ServiceLooper(args, implementation);
                    else
                        ServiceBase.Run(new WindowsServiceBase(implementation));
                }
            }

            catch (Exception ex)
            {
                exitCode = -1;
                WriteToConsole(ConsoleColor.Red, "An exception occurred: {0}", ex);
            }
            return exitCode;
        }

        public static void ServiceLooper(string[] args, IWinService service)
        {
            string serviceName = service.GetType().Name;
            bool isRunning = true;

            service.OnStart(args);

            while (isRunning)
            {
                WriteToConsole(ConsoleColor.Yellow, "Enter [Q]uit, [P]ause, [R]esume : ");
                isRunning = HandleConsoleInput(service, Console.ReadLine());
            }

            service.OnStop();
            service.OnShutdown();
        }

        private static bool HandleConsoleInput(IWinService service, string line)
        {
            bool canContinue = true;

            if (line != null)
            {
                switch (line.ToUpper())
                {
                    case "Q":
                        canContinue = false;
                        break;

                    case "P":
                        service.OnPause();
                        break;

                    case "R":
                        service.OnContinue();
                        break;

                    default:
                        WriteToConsole(ConsoleColor.Red, "Enter [Q]uit, [P]ause, [R]esume :");
                        break;
                }
            }

            return canContinue;
        }

        public static void WriteToConsole(ConsoleColor foregroundColor, string format,
            params object[] formatArguments)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;

            Console.WriteLine(format, formatArguments);
            Console.Out.Flush();

            Console.ForegroundColor = originalColor;
        }
    }
}

using System;
using System.Configuration.Install;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using System.ServiceProcess;

namespace WinService
{
    [RunInstaller(true)]
    public partial class WindowsServiceInstaller<T> : Installer 
        where T : IWinService
    {
        public WindowsServiceAttribute Configuration { get; set; }

        public WindowsServiceInstaller(string _serviceName = null)
        {
            var windowsServiceType = typeof(T);

            var attribute = windowsServiceType.GetAttribute<WindowsServiceAttribute>();
            Configuration = attribute ?? throw new ArgumentException("Type to install must be marked with a WindowsServiceAttribute.", "windowsServiceType");

            if (!string.IsNullOrWhiteSpace(_serviceName))
            {
                Configuration.Name = _serviceName;
            }            
        }

        public static void RuntimeInstall(string _serviceName = null)            
        {
            string path = "/assemblypath=" + Assembly.GetEntryAssembly().Location;

            using (var ti = new TransactedInstaller())
            {
                ti.Installers.Add(new WindowsServiceInstaller<T>(_serviceName));
                ti.Context = new InstallContext(null, new[] { path });
                ti.Install(new Hashtable());
            }
        }

        public static void RuntimeUnInstall(string _serviceName = null, params Installer[] otherInstallers)
        {
            string path = "/assemblypath=" + Assembly.GetEntryAssembly().Location;

            using (var ti = new TransactedInstaller())
            {
                ti.Installers.Add(new WindowsServiceInstaller<T>(_serviceName));
                ti.Context = new InstallContext(null, new[] { path });
                ti.Uninstall(null);
            }
        }

        public override void Install(IDictionary savedState)
        {
            ConsoleUtils.WriteToConsole(ConsoleColor.White, "Installing service {0}.", Configuration.Name);

            ConfigureInstallers();
            base.Install(savedState);

            if (!string.IsNullOrWhiteSpace(Configuration.EventLogSource))
            {
                if (!EventLog.SourceExists(Configuration.EventLogSource))
                {
                    EventLog.CreateEventSource(Configuration.EventLogSource, "Application");
                }
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            ConsoleUtils.WriteToConsole(ConsoleColor.White, "Un-Installing service {0}.", Configuration.Name);

            ConfigureInstallers();
            base.Uninstall(savedState);

            if (!string.IsNullOrWhiteSpace(Configuration.EventLogSource))
            {
                if (EventLog.SourceExists(Configuration.EventLogSource))
                {
                    EventLog.DeleteEventSource(Configuration.EventLogSource);
                }
            }
        }

        public override void Rollback(IDictionary savedState)
        {
            ConsoleUtils.WriteToConsole(ConsoleColor.White, "Rolling back service {0}.", Configuration.Name);

            ConfigureInstallers();
            base.Rollback(savedState);
        }

        private void ConfigureInstallers()
        {
            Installers.Add(ConfigureProcessInstaller());
            Installers.Add(ConfigureServiceInstaller());
        }


        private ServiceProcessInstaller ConfigureProcessInstaller()
        {
            var result = new ServiceProcessInstaller();

            if (string.IsNullOrEmpty(Configuration.UserName))
            {
                result.Account = ServiceAccount.LocalService;
                result.Username = null;
                result.Password = null;
            }
            else
            {
                result.Account = ServiceAccount.User;
                result.Username = Configuration.UserName;
                result.Password = Configuration.Password;
            }

            return result;
        }

        private ServiceInstaller ConfigureServiceInstaller()
        {
            var result = new ServiceInstaller
            {
                ServiceName = Configuration.Name,
                DisplayName = Configuration.DisplayName,
                Description = Configuration.Description,
                StartType = Configuration.StartMode,
                ServicesDependedOn = Configuration.ServiceDependsOn
            };

            return result;
        }
    }
}

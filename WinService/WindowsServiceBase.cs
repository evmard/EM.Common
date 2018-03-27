using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace WinService
{
    public partial class WindowsServiceBase : ServiceBase
    {
        public IWinService ServiceImplementation { get; private set; }

        public WindowsServiceBase(IWinService serviceImplementation)
        {
            ServiceImplementation = serviceImplementation ?? throw new ArgumentNullException("serviceImplementation");
            ConfigureServiceFromAttributes(serviceImplementation);
        }

        protected override void OnContinue()
        {
            ServiceImplementation.OnContinue();
        }

        protected override void OnPause()
        {
            ServiceImplementation.OnPause();
        }

        protected override void OnCustomCommand(int command)
        {
            ServiceImplementation.OnCustomCommand(command);
        }

        protected override void OnShutdown()
        {
            ServiceImplementation.OnShutdown();
        }

        protected override void OnStart(string[] args)
        {
            ServiceImplementation.OnStart(args);
        }

        protected override void OnStop()
        {
            ServiceImplementation.OnStop();
        }

        private void ConfigureServiceFromAttributes(IWinService serviceImplementation)
        {
            var attribute = serviceImplementation.GetType().GetAttribute<WindowsServiceAttribute>();

            if (attribute != null)
            {
                if (!string.IsNullOrWhiteSpace(attribute.EventLogSource))
                {
                    EventLog.Source = attribute.EventLogSource;
                }

                CanStop = attribute.CanStop;
                CanPauseAndContinue = attribute.CanPauseAndContinue;
                CanShutdown = attribute.CanShutdown;

                CanHandlePowerEvent = false;
                CanHandleSessionChangeEvent = false;
                AutoLog = true;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("IWindService implementer {0} must have a WindowsServiceAttribute.",
                                  serviceImplementation.GetType().FullName));
            }
        }
    }
}


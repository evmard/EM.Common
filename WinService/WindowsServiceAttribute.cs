using System;
using System.ServiceProcess;

namespace WinService
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WindowsServiceAttribute : Attribute
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }        
        public string Description { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EventLogSource { get; set; }
        public ServiceStartMode StartMode { get; set; }
        public bool CanPauseAndContinue { get; set; }
        public bool CanShutdown { get; set; }
        public bool CanStop { get; set; }
        public string[] ServiceDependsOn { get; set; }
        public WindowsServiceAttribute(string name)
        {
            Name = name;
            Description = name;
            DisplayName = name;

            ServiceDependsOn = new string[0];
            CanStop = true;
            CanShutdown = true;
            CanPauseAndContinue = true;
            StartMode = ServiceStartMode.Manual;
            EventLogSource = null;
            Password = null;
            UserName = null;
        }
    }
}
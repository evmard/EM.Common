using System;

namespace WinService
{
    public interface IWinService : IDisposable
    {
        void OnStart(string[] args);
        void OnStop();
        void OnPause();
        void OnContinue();
        void OnShutdown();
        void OnCustomCommand(int command);
    }
}

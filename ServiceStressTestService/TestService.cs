using MicroServiceBase;
using System;
using WinService;
using ServiceStressTestContract;
using MicroServiceBase.Contract;
using System.Threading;

namespace ServiceStressTestService
{
    [WindowsService("TestService")]
    class TestService : RMSServiceBase, IWinService
    {
        Random rnd = new Random();
        public TestService() : base(TestContracts.Instance.GetContracts())
        {
            RegisterRCPHandler(RpcTest.GetQueueName(), new RPCHandler<StringRequest, StringResponce>(Test));
            RegisterRCPHandler(RpcTest2.GetQueueName(), new RPCHandler<StringRequest, StringResponce>(Test2));
            RegisterRCPHandler(RpcTest3.GetQueueName(), new RPCHandler<StringRequest, StringResponce>(Test3));
        }

        private StringResponce Test(StringRequest arg)
        {
            Thread.Sleep(rnd.Next(100));
            return new StringResponce { Value = arg.Value };
        }

        private StringResponce Test2(StringRequest arg)
        {
            Thread.Sleep(rnd.Next(100));
            return new StringResponce { Value = arg.Value };
        }
        private StringResponce Test3(StringRequest arg)
        {
            Thread.Sleep(rnd.Next(100));
            return new StringResponce { Value = arg.Value };
        }

        public void OnStart(string[] args)
        {
            Start();
        }

        public void OnStop()
        {
            Stop();
        }

        public void OnPause()
        {
            Stop();
        }

        public void OnContinue()
        {
            Start();
        }

        public void OnShutdown()
        {
            Shutdown();
        }

        public void OnCustomCommand(int command)
        {
        }
    }
}

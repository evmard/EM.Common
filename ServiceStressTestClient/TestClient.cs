using MicroServiceBase;
using MicroServiceBase.Contract;
using ServiceStressTestContract;

namespace ServiceStressTestClient
{
    public class TestClient : RMSClientBase
    {
        public TestClient() : base(TestContracts.Instance.GetContracts())
        {
        }

        public static TestClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (locker)
                    {
                        if (_instance == null)
                            _instance = new TestClient();
                    }
                }

                return _instance;
            }
        }
        private static object locker = new object();
        private static TestClient _instance;

        public StringResponce Test(string msg)
        {
            return SendRpc<StringResponce>(new StringRequest { Value = msg }, RpcTest.GetQueueName());
        }

        public StringResponce Test2(string msg)
        {
            return SendRpc<StringResponce>(new StringRequest { Value = msg }, RpcTest2.GetQueueName());
        }
        public StringResponce Test3(string msg)
        {
            return SendRpc<StringResponce>(new StringRequest { Value = msg }, RpcTest3.GetQueueName());
        }
    }
}

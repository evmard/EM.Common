using MicroServiceBase.Contract;

namespace ServiceStressTestContract
{
    public class RpcTest : RpcContractInfo<StringRequest, StringResponce>
    {
        public override string QueueName { get { return GetQueueName(GetType()); } }
        public static string GetQueueName()
        {
            return GetQueueName(typeof(RpcTest));
        }
    }

    public class RpcTest2 : RpcContractInfo<StringRequest, StringResponce>
    {
        public override string QueueName { get { return GetQueueName(GetType()); } }
        public static string GetQueueName()
        {
            return GetQueueName(typeof(RpcTest2));
        }
    }

    public class RpcTest3 : RpcContractInfo<StringRequest, StringResponce>
    {
        public override string QueueName { get { return GetQueueName(GetType()); } }
        public static string GetQueueName()
        {
            return GetQueueName(typeof(RpcTest3));
        }
    }
}

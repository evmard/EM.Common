using System;

namespace MicroServiceBase.Contract
{
    public abstract class RpcContractInfo<TReqest, TResponce> : IContractInfo
        where TReqest : IRMSSerializable
        where TResponce : IResponce
    {

        public RpcContractInfo()
        {
            IsRpc = true;
            ReqestType = typeof(TReqest);
            ResponceType = typeof(TResponce);
        }

        public static string GetQueueName(Type currentType)
        {
            return string.Format("{0}.{1}", currentType.Namespace, currentType.Name);
        }

        public abstract string QueueName { get; }
        public bool IsRpc { get; private set; }
        public Type ReqestType { get; private set; }
        public Type ResponceType { get; private set; }

        public override string ToString()
        {
            return string.Format("[RPC] {0}", QueueName);
        }
    }
}

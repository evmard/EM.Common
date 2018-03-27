using System;

namespace MicroServiceBase.Contract
{
    public abstract class ContractInfo<TReqest> : IContractInfo
        where TReqest : IRMSSerializable
    {

        protected ContractInfo()
        {
            IsRpc = false;
            ReqestType = typeof(TReqest);
            ResponceType = null;
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
            return string.Format("[Queue] {1}", QueueName);
        }
    }
}

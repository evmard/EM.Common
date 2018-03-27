using System;

namespace MicroServiceBase.Contract
{
    public interface IContractInfo
    {
        string QueueName { get; }
        bool IsRpc { get; }

        Type ReqestType { get; }
        Type ResponceType { get; }
    }
}
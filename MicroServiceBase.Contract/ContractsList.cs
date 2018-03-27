using System.Collections.Generic;
using Utils;

namespace MicroServiceBase.Contract
{
    public abstract class ContractsList<T> : Singletone<T>
        where T : class, new()
    {
        protected void AddContract(IContractInfo contract)
        {
            _contracts.Add(contract.QueueName, contract);
        }

        public IEnumerable<IContractInfo> GetContracts()
        {
            return _contracts.Values;
        }

        private Dictionary<string, IContractInfo> _contracts = new Dictionary<string, IContractInfo>();
    }
}

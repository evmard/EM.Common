using MicroServiceBase.Contract;

namespace ServiceStressTestContract
{
    public class TestContracts : ContractsList<TestContracts>
    {
        public TestContracts()
        {
            AddContract(new RpcTest());
            AddContract(new RpcTest2());
            AddContract(new RpcTest3());
        }
    }
}

using MicroServiceBase.Contract;
using RabbitMQ.Client;

namespace MicroServiceBase
{
    public class RMSQueueAndChannel
    {
        public RMSQueueAndChannel(IContractInfo info, IModel channel, IConnection connection)
        {
            Info = info;
            Channel = channel;
            Connection = connection;
        }

        public IConnection Connection { get; private set; }
        public IContractInfo Info { get; private set; }
        public IModel Channel { get; private set; }
    }
}
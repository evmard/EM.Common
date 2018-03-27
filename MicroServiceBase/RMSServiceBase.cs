using MicroServiceBase.Contract;
using MicroServiceBase.Exceptions;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Profiling;
using System.Text;

namespace MicroServiceBase
{
    public abstract class RMSServiceBase : RMSBase
    {
        public RMSServiceBase(IEnumerable<IContractInfo> contractList)
        {
            _contractList = contractList;
        }

        public void Start()
        {
            if (_started)
                return;

            Logger.Instance.Sys("Service Started", GetType().Name);

            _started = true;
            QueueDeclare(_contractList);
            ConsumeQueues(_contractList);
        }
        public void Stop()
        {
            if (!_started)
                return;

            CloseChannels();
            _started = false;

            Logger.Instance.Sys("Service Stopped", GetType().Name);
        }

        protected void RegisterQueueHandler<TReqest>(string queueName, QueueHandler<TReqest> handler)
            where TReqest : JsonSerializable<TReqest>, new()
        {
            if (queueName == null)
                throw new ArgumentNullException("queueName");

            var info = GetContractInfo(queueName);
            CheckQueueContract(handler.DataType, info);
            try
            {
                QueueHandlersByQName.Add(queueName, handler.Handler);
            }
            catch
            {
                throw new RMSContractException(string.Format("Contract with name '{0}' already registred", queueName));
            }            
        }
        protected void RegisterRCPHandler<TReqest, TResult>(string queueName, RPCHandler<TReqest, TResult> handler)
            where TReqest : JsonSerializable<TReqest>, new()
            where TResult : IResponce, new()
        {
            if (queueName == null)
                throw new ArgumentNullException("queueName");

            var info = GetContractInfo(queueName);
            CheckRPCContract(handler.ReqestType, handler.ResponceType, info);
            try
            {
                RPCHandlersByQName.Add(queueName, handler.Handler);
            }
            catch
            {
                throw new RMSContractException(string.Format("Contract with name '{0}' already registred", queueName));
            }
        }

        private bool _started;
        private IEnumerable<IContractInfo> _contractList;
        private Dictionary<string, Action<byte[]>> QueueHandlersByQName = new Dictionary<string, Action<byte[]>>();
        private Dictionary<string, Func<byte[], byte[]>> RPCHandlersByQName = new Dictionary<string, Func<byte[], byte[]>>();

        private IContractInfo GetContractInfo(string queueName)
        {
            var info = _contractList.FirstOrDefault(item => string.Equals(item.QueueName, queueName));
            if (info == null)
                throw new RMSContractException(string.Format("Contract with name '{0}' not found", queueName));

            return info;
        }
        private Action<byte[]> GetQueueHandler(string queueName)
        {
            try
            {
                return QueueHandlersByQName[queueName];
            }
            catch
            {
                throw new RMSContractException(string.Format("Contract with name '{0}' doesn't registred as queue handler", queueName));
            }
        }
        private Func<byte[], byte[]> GetRpcHandler(string queueName)
        {
            try
            {
                return RPCHandlersByQName[queueName];
            }
            catch
            {
                throw new RMSContractException(string.Format("Contract with name '{0}' doesn't registred as rpc handler", queueName));
            }
        }
        private void ConsumeQueues(IEnumerable<IContractInfo> _contractList)
        {
            foreach (var contract in _contractList)
            {
                var qac = GetQueueAndChannel(contract.QueueName);
                var consumer = new EventingBasicConsumer(qac.Channel);
                if (qac.Info.IsRpc)
                    BindRpc(qac, consumer);
                else
                    BindQueue(qac, consumer);                
            }
        }
        private void BindRpc(RMSQueueAndChannel qac, EventingBasicConsumer consumer)
        {
            var rpcHandler = GetRpcHandler(qac.Info.QueueName);
            consumer.Received += (model, eventArg) =>
            {
                var reqest = eventArg.Body;
                var responceProps = qac.Channel.CreateBasicProperties();
                var props = eventArg.BasicProperties;
                responceProps.CorrelationId = props.CorrelationId;

                var jsonString = Encoding.UTF8.GetString(reqest);
                byte[] result = null;
                Logger.Instance.Debug($"Received RPC {qac.Info.QueueName} params: {jsonString}");
                Profiler.Do(
                    () => { result = rpcHandler(reqest); },
                    jsonString,
                    qac.Info.QueueName);
                Logger.Instance.Debug($"End RPC {qac.Info.QueueName}");

                qac.Channel.BasicPublish("", props.ReplyTo, false, responceProps, result);
                qac.Channel.BasicAck(eventArg.DeliveryTag, false);
            };
            qac.Channel.BasicConsume(qac.Info.QueueName, false, "", false, false, null, consumer);
        }
        private void BindQueue(RMSQueueAndChannel qac, EventingBasicConsumer consumer)
        {
            var queueHandler = GetQueueHandler(qac.Info.QueueName);
            consumer.Received += (model, eventArg) =>
            {
                var reqest = eventArg.Body;
                var jsonString = Encoding.UTF8.GetString(reqest);

                Logger.Instance.Debug($"Received call {qac.Info.QueueName} params: {jsonString}");
                Profiler.Do( 
                    () => { queueHandler(reqest); },
                    jsonString,
                    qac.Info.QueueName);
                Logger.Instance.Debug($"End call {qac.Info.QueueName}");

                qac.Channel.BasicAck(eventArg.DeliveryTag, false);
            };
            qac.Channel.BasicConsume(qac.Info.QueueName, false, "", false, false, null, consumer);
        }
    }
}

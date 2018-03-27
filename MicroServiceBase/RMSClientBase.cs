using MicroServiceBase.Contract;
using MicroServiceBase.Exceptions;
using Profiling;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using System;
using System.Collections.Generic;

namespace MicroServiceBase
{
    public abstract class RMSClientBase : RMSBase
    {
        public RMSClientBase(IEnumerable<IContractInfo> contractList) : base()
        {
            QueueDeclare(contractList);
        }

        protected TResult SendRpc<TResult>(IRMSSerializable requestData, string queueName, EventHandler timeOutHandler = null, EventHandler disconnectedHandler = null)
            where TResult : JsonSerializable<TResult>
        {
            if (timeOutHandler == null)
                timeOutHandler = DefaultTimedOutHandler;

            if (disconnectedHandler == null)
                disconnectedHandler = DefaultDisconnectedHandler;

            var queueAndChannel = GetQueueAndChannel(queueName);
            return SendRpc<TResult>(requestData, queueAndChannel, timeOutHandler, disconnectedHandler);
        }
        protected void SendMsg(IRMSSerializable data, string queueName)
        {
            var queueAndChannel = GetQueueAndChannel(queueName);
            SendMsg(data, queueAndChannel);
        }

        private TResult SendRpc<TResult>(IRMSSerializable requestData, RMSQueueAndChannel queue, EventHandler timeOutHandler, EventHandler disconnectedHandler)
            where TResult : JsonSerializable<TResult>
        {
            CheckRPCContract(requestData.GetType(), typeof(TResult), queue.Info);

            byte[] replyMessageBytes = null;
            var reqDataStr = requestData.GetString();
            Logger.Instance.Debug($"Send RPC {queue.Info.QueueName} data: {reqDataStr}");
            Profiler.Do(() =>
            {                
                lock (queue.Channel)
                {
                    using (var client = new SimpleRpcClient(queue.Channel, queue.Info.QueueName))
                    {
                        client.TimeoutMilliseconds = DefualtTimeOut;
                        client.Disconnected += disconnectedHandler;
                        client.TimedOut += timeOutHandler;
                        replyMessageBytes = client.Call(requestData.GetBytes());
                    }
                }
            }, reqDataStr, queue.Info.QueueName);
            var result = JsonSerializable<TResult>.GetObject(replyMessageBytes);
            Logger.Instance.Debug($"End RPC {queue.Info.QueueName}");
            return result;
        }

        protected virtual int DefualtTimeOut { get; } = 1000000;

        protected virtual void DefaultTimedOutHandler(object sender, EventArgs e)
        {
            throw new RMSTimeOutException(string.Format("RPC responce timeout. Sender: {0}", sender.GetType().Name));
        }

        protected virtual void DefaultDisconnectedHandler(object sender, System.EventArgs e)
        {
            throw new RMSDisconnectedException(string.Format("Disconnected. Sender: {0}", sender.GetType().Name));
        }

        private void SendMsg(IRMSSerializable data, RMSQueueAndChannel queue)
        {
            CheckQueueContract(data.GetType(), queue.Info);

            var reqDataStr = data.GetString();
            Logger.Instance.Debug($"Send call {queue.Info.QueueName} data: {reqDataStr}");
            Profiler.Do(() =>
            {
                lock (queue.Channel)
                {
                    queue.Channel.BasicPublish("", queue.Info.QueueName, null, data.GetBytes());
                }
            }, reqDataStr, queue.Info.QueueName);
            Logger.Instance.Debug($"End call {queue.Info.QueueName}");
        }
    }
}

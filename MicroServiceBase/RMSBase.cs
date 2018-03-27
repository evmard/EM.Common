using MicroServiceBase.Contract;
using MicroServiceBase.Exceptions;
using Profiling;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace MicroServiceBase
{
    public abstract class RMSBase : IDisposable
    {
        public RMSBase()
        {
            Factory = new ConnectionFactory()
            {
                HostName = MQHost,
                Port = MQPort,
                UserName = MQUser,
                Password = MQPass,
                VirtualHost = MQVirtualHost
            };

            if (MQUseSsl)
            {
                Factory.Ssl.Version = System.Security.Authentication.SslProtocols.Tls11;

                Factory.Ssl.AcceptablePolicyErrors = System.Net.Security.SslPolicyErrors.RemoteCertificateNotAvailable |
                    System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch |
                    System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors;

                Factory.Ssl.Enabled = true;
            }
        }

        public virtual string MQHost { get { return ConfigUtils.GetString("MQHost"); } }
        public virtual string MQUser { get { return ConfigUtils.GetString("MQUser"); } }
        public virtual int MQPort { get { return ConfigUtils.GetInt("MQPort"); } }
        public virtual string MQPass { get { return ConfigUtils.GetString("MQPass"); } }
        public virtual string MQVirtualHost { get { return ConfigUtils.GetString("MQVirtHost"); } }
        public virtual bool MQUseSsl { get { return ConfigUtils.GetBool("MQUseSsl"); } }

        protected void QueueDeclare(IEnumerable<IContractInfo> contractList)
        {
            if (contractList == null || !contractList.Any())
                throw new RMSContractException("Contract list is empty");

            foreach (var contract in contractList)
            {
                var connetion = Factory.CreateConnection();
                var channel = connetion.CreateModel();

                try
                {
                    QueueAndChannelByName.Add(contract.QueueName, new RMSQueueAndChannel(contract, channel, connetion));
                }
                catch (Exception ex)
                {
                    if (channel != null)
                        channel.Dispose();

                    Logger.Instance.Error($"Can not add queue with name = '{contract.QueueName}'");
                    throw new RMSContractException(string.Format("Can not add queue with name = '{0}'", contract.QueueName), ex);
                }

                channel.BasicQos(0, 1, false);
                channel.QueueDeclare(contract.QueueName, true, false, false);
            }
        }
        protected RMSQueueAndChannel GetQueueAndChannel(string queueName)
        {
            try
            {
                return QueueAndChannelByName[queueName];
            }
            catch(Exception ex)
            {
                Logger.Instance.Error($"The queue '{queueName}' not found");
                throw new RMSContractException(string.Format("The queue '{0}' not found", queueName), ex);
            }
        }

        protected void CloseChannels()
        {
            var channels = QueueAndChannelByName.Values.ToList();
            QueueAndChannelByName.Clear();
            foreach (var qac in channels)
            {
                qac.Channel.Close();
                qac.Connection.Close();
            }
        }

        public void Shutdown()
        {
            CloseChannels();            
        }

        protected static void CheckQueueContract(Type dataType, IContractInfo info)
        {
            if (info.IsRpc)
                throw new RMSContractException(string.Format("The queue '{0}' IS RPC", info.QueueName));

            if (dataType != info.ReqestType)
                throw new RMSContractException(string.Format("The queue '{0}': wrong reqest data type", info.QueueName));
        }
        protected static void CheckRPCContract(Type reqestType, Type responceType, IContractInfo info)
        {
            if (!info.IsRpc)
                throw new RMSContractException(string.Format("The queue '{0}' IS NOT RPC", info.QueueName));

            if (reqestType != info.ReqestType)
                throw new RMSContractException(string.Format("The queue '{0}': wrong reqest data type", info.QueueName));

            if (responceType != info.ResponceType)
                throw new RMSContractException(string.Format("The queue '{0}': wrong responce data type", info.QueueName));
        }

        private readonly ConnectionFactory Factory;
        private Dictionary<string, RMSQueueAndChannel> QueueAndChannelByName = new Dictionary<string, RMSQueueAndChannel>();

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Shutdown();
                }

                disposedValue = true;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}

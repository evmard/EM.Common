using System;

namespace MicroServiceBase.Contract
{
    public class QueueHandler<TReqest>
        where  TReqest : JsonSerializable<TReqest>, new()
    {
        public QueueHandler(Action<TReqest> handler)
        {
            _handler = handler;
            Handler = HandlerWrapper;
            DataType = typeof(TReqest);
        }        

        public Action<byte[]> Handler { get; private set; }
        public Type DataType { get; private set; }

        private readonly Action<TReqest> _handler;
        private void HandlerWrapper(byte[] data)
        {
            var typedData = JsonSerializable<TReqest>.GetObject(data);
            _handler(typedData);
        } 
    }
}

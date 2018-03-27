using Profiling;
using System;

namespace MicroServiceBase.Contract
{
    public class RPCHandler<TReqest, TResult>
        where TReqest : JsonSerializable<TReqest>
        where TResult : IResponce, new()
    {
        public RPCHandler(Func<TReqest, TResult> handler)
        {
            ReqestType = typeof(TReqest);
            ResponceType = typeof(TResult);
            _handler = handler;
        }

        public Type ReqestType { get; private set; }
        public Type ResponceType { get; private set; }
        public Func<byte[], byte[]> Handler { get { return HandlerWrapper; } }

        private readonly Func<TReqest, TResult> _handler;
        private byte[] HandlerWrapper(byte[] reqest)
        {
            TReqest typedData = null;
            try
            {
                typedData = JsonSerializable<TReqest>.GetObject(reqest);
            }
            catch
            {
                var errorResult = new TResult();
                errorResult.ErrorMsg = "Bad reqest data type";
                return errorResult.GetBytes();
            }

            try
            {
                var result = _handler(typedData);
                return result.GetBytes();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
                var errorResult = new TResult();
                errorResult.ErrorMsg = ex.Message;
                return errorResult.GetBytes();
            }
        }
    }
}

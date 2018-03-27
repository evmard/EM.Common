using System;

namespace MicroServiceBase.Contract
{
    public class IntResponce : JsonSerializable<IntResponce>, IResponce
    {
        public string ErrorMsg { get; set; }
        public int Value { get; set; }
        public ServiceOperationResult Result { get; set; } = ServiceOperationResult.OK;
    }

    public class LongResponce : JsonSerializable<LongResponce>, IResponce
    {
        public string ErrorMsg { get; set; }
        public long Value { get; set; }
        public ServiceOperationResult Result { get; set; } = ServiceOperationResult.OK;
    }

    public class StringResponce : JsonSerializable<StringResponce>, IResponce
    {
        public string ErrorMsg { get; set; }
        public string Value { get; set; }
        public ServiceOperationResult Result { get; set; } = ServiceOperationResult.OK;
    }

    public class BoolResponce : JsonSerializable<BoolResponce>, IResponce
    {
        public string ErrorMsg { get; set; }
        public bool Value { get; set; }
        public ServiceOperationResult Result { get; set; } = ServiceOperationResult.OK;
    }

    public class DateTimeResponce : JsonSerializable<DateTimeResponce>, IResponce
    {
        public string ErrorMsg { get; set; }
        public DateTime Value { get; set; }
        public ServiceOperationResult Result { get; set; } = ServiceOperationResult.OK;
    }

    public class GuidResponce : JsonSerializable<GuidResponce>, IResponce
    {
        public string ErrorMsg { get; set; }
        public Guid Value { get; set; }
        public ServiceOperationResult Result { get; set; } = ServiceOperationResult.OK;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServiceBase.Contract
{
    public interface IAirlineRequest : IRMSSerializable
    {
        string AirlineCode { get; set; }
    }

    public class IntRequest : JsonSerializable<IntRequest>
    {
        public int Value { get; set; }
    }

    public class LongRequest : JsonSerializable<LongRequest>
    {
        public long Value { get; set; }
    }

    public class StringRequest : JsonSerializable<StringRequest>
    {
        public string Value { get; set; }
    }

    public class BoolRequest : JsonSerializable<BoolRequest>
    {
        public bool Value { get; set; }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;

namespace MicroServiceBase.Contract
{
    public abstract class JsonSerializable<T> : IRMSSerializable
    {
        public byte[] GetBytes()
        {
            var jsonString = JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(jsonString);
        }

        public static T GetObject(byte[] bytes)
        {
            var jsonString = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public string GetString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class DateTimeConverter : IsoDateTimeConverter
    {
        public DateTimeConverter()
        {
            base.DateTimeFormat = "yyyy-MM-dd HH:mm";
        }
    }

    public class DateTimeSecConverter : IsoDateTimeConverter
    {
        public DateTimeSecConverter()
        {
            base.DateTimeFormat = "yyyy-MM-dd H:mm:ss";
        }
    }

    public class DateOnlyConverter : IsoDateTimeConverter
    {
        public DateOnlyConverter()
        {
            base.DateTimeFormat = "yyyy-MM-dd";
        }
    }
}

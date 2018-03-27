using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Crm.DbUpdater.Core
{
    public class PatchInfo
    {
        [JsonConverter(typeof(VersionSerializer))]
        public Version From { get; set; }
        [JsonConverter(typeof(VersionSerializer))]
        public Version To { get; set; }
        public string Hash { get; set; }
        public List<PatchFileInfo> Files { get; set; }
    }

    internal class VersionSerializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(string));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);            
            if (token.Type == JTokenType.String)
            {
                var value = token.ToString();
                return new Version(value);
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var typed = value as Version;
            if (typed == null)
                return;

            writer.WriteValue(typed.ToString());
        }
    }
}

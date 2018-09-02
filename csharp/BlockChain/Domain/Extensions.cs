using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BlockChain.Domain
{
    public static class Extensions
    {
        public static string AsJson(this object source)
        {
            var settings = new JsonSerializerSettings()
            {
                // Ensure order of properties in serialized objects
                ContractResolver = new OrderedContractResolver()
            };

            return JsonConvert.SerializeObject(source, Formatting.Indented, settings);
        }

        public static T FromJson<T>(this string source)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }

        public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> collection)
        {
            return new List<T>().AsReadOnly();
        }

        public static bool HasProperty(dynamic data, string name)
        {
            if (data is ExpandoObject)
                return ((IDictionary<string, object>)data).ContainsKey(name);

            return data.GetType().GetProperty(name) != null;
        }

        public static bool HasProperties(dynamic data, params string[] names)
        {
            return names.All(name => data.HasProperty(name));
        }
    }
}
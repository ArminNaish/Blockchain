using System;
using System.Collections.Generic;
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

        public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> collection)
        {
            return new List<T>().AsReadOnly();
        }
    }
}
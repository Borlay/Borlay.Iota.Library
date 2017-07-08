using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Borlay.Iota.Library.Iri
{
    internal static class SerializeExtensions
    {
        public static string ToJson(this object request)
        {
            var serializer = new Newtonsoft.Json.JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include
            };

            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    serializer.ContractResolver = new LowercaseContractResolver();
                    serializer.Serialize(jsonTextWriter, request);

                    var result = stringWriter.ToString();
                    return result;
                }
            }
        }

        private class LowercaseContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string name)
            {
                // first letter small to match API naming conventions
                return Char.ToLowerInvariant(name[0]) + name.Substring(1);
            }
        }
    }
}

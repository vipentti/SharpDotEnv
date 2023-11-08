using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpDotEnv.Extensions.Configuration
{
    internal static class DotEnvKeyNormalizer
    {
        public static string Normalize(string key) => key.Replace("__", ConfigurationPath.KeyDelimiter);

        public static Dictionary<string, string?> ReadNormalizedEnvironment(Stream stream, string normalizedPrefix)
        {
            return NormalizeEnvironment(DotEnv.Parse(stream), normalizedPrefix);
        }

        public static Dictionary<string, string?> NormalizeEnvironment(IDictionary<string, string> env, string normalizedPrefix)
        {
            var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            foreach (var kvp in env)
            {
                AddIfNormalizedKeyMatchesPrefix(data, Normalize(kvp.Key), kvp.Value, normalizedPrefix: normalizedPrefix);
            }

            return data;
        }

        private static void AddIfNormalizedKeyMatchesPrefix(Dictionary<string, string?> data, string normalizedKey, string value, string normalizedPrefix)
        {
            if (normalizedKey.StartsWith(normalizedPrefix, StringComparison.OrdinalIgnoreCase))
            {
#pragma warning disable IDE0057 // Use range operator
                data[normalizedKey.Substring(normalizedPrefix.Length)] = value;
#pragma warning restore IDE0057 // Use range operator
            }
        }
    }
}

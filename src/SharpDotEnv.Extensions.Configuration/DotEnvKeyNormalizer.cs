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
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                data[normalizedKey[normalizedPrefix.Length..]] = value;
#else
                data[normalizedKey.Substring(normalizedPrefix.Length)] = value;
#endif
            }
        }
    }
}

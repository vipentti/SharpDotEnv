// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SharpDotEnv.Extensions.Configuration;

internal static class DotEnvKeyNormalizer
{
    public static string Normalize(string key) =>
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        key.Replace("__", ConfigurationPath.KeyDelimiter, StringComparison.Ordinal);
#else
        key.Replace("__", ConfigurationPath.KeyDelimiter);
#endif

    public static Dictionary<string, string?> ReadNormalizedEnvironment(
        Stream stream,
        string normalizedPrefix
    )
    {
        return NormalizeEnvironment(DotEnv.Parse(stream), normalizedPrefix);
    }

    public static Dictionary<string, string?> NormalizeEnvironment(
        IDictionary<string, string> env,
        string normalizedPrefix
    )
    {
        var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (var kvp in env)
        {
            AddIfNormalizedKeyMatchesPrefix(
                data,
                Normalize(kvp.Key),
                kvp.Value,
                normalizedPrefix: normalizedPrefix
            );
        }

        return data;
    }

    private static void AddIfNormalizedKeyMatchesPrefix(
        Dictionary<string, string?> data,
        string normalizedKey,
        string value,
        string normalizedPrefix
    )
    {
        if (normalizedKey.StartsWith(normalizedPrefix, StringComparison.OrdinalIgnoreCase))
        {
#pragma warning disable IDE0057 // Use range operator
            data[normalizedKey.Substring(normalizedPrefix.Length)] = value;
#pragma warning restore IDE0057 // Use range operator
        }
    }
}

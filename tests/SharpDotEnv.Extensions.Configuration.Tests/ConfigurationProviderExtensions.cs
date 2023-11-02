using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SharpDotEnv.Extensions.Configuration.Tests;

public static class ConfigurationProviderExtensions
{
    public static string? Get(this IConfigurationProvider provider, string key)
    {
        if (!provider.TryGet(key, out var value))
        {
            throw new KeyNotFoundException($"Key: '{key}' not found");
        }

        return value;
    }
}

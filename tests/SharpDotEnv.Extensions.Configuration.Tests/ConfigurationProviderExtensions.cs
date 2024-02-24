// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

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

// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System.IO;
using Microsoft.Extensions.Configuration;

namespace SharpDotEnv.Extensions.Configuration;

/// <summary>
/// A dotenv file based <see cref="FileConfigurationProvider"/>.
/// <para>
/// dotenv files are simple text files for defining environment variables
/// </para>
/// </summary>
/// <examples>
/// # this is a comment
/// this_is_a_key = this is its value which can include spaces
/// </examples>
public class DotEnvConfigurationProvider : FileConfigurationProvider
{
    private readonly string _prefix;
    private readonly string _normalizedPrefix;

    /// <summary>
    /// Initializes a new instance with the specified source.
    /// </summary>
    /// <param name="source">The source settings.</param>
    public DotEnvConfigurationProvider(FileConfigurationSource source)
        : base(source)
    {
        _prefix = string.Empty;
        _normalizedPrefix = string.Empty;
        if (source is DotEnvConfigurationSource src && src.Prefix != null)
        {
            _prefix = src.Prefix;
            _normalizedPrefix = DotEnvKeyNormalizer.Normalize(_prefix);
        }
    }

    /// <summary>
    /// Initializes a new instance with the specified source with the specified prefix
    /// </summary>
    /// <param name="source">The source settings.</param>
    /// <param name="prefix">A prefix used to filter the environment variables in the dotenv file.</param>
    public DotEnvConfigurationProvider(FileConfigurationSource source, string? prefix)
        : base(source)
    {
        _prefix = prefix ?? string.Empty;
        _normalizedPrefix = DotEnvKeyNormalizer.Normalize(_prefix);
    }

    /// <inheritdoc />
    public override void Load(Stream stream)
    {
        Data = DotEnvKeyNormalizer.ReadNormalizedEnvironment(stream, _normalizedPrefix);
    }
}

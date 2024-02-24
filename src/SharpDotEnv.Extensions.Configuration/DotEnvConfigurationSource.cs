// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace SharpDotEnv.Extensions.Configuration;

/// <summary>
/// Configuration source for a dotenv file
/// </summary>
public class DotEnvConfigurationSource : FileConfigurationSource
{
    /// <summary>
    /// A prefix used to filter environment variables in the dotenv file
    /// </summary>
    public string? Prefix { get; set; }

    /// <inheritdoc />
    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        EnsureDefaults(builder);
        return new DotEnvConfigurationProvider(this, Prefix);
    }

    /// <summary>
    /// If no file provider has been set: For absolute Path, this will creates a <see cref="DotEnvPhysicalFileProvider"/>
    /// for the nearest existing directory. Otherwise create an instance of <see cref="DotEnvPhysicalFileProvider"/> set to <see cref="AppContext.BaseDirectory"/>
    /// </summary>
    public void ResolveFileProvider(IFileProvider? baseProvider)
    {
        if (
            FileProvider is null
            && !string.IsNullOrEmpty(Path)
            && System.IO.Path.IsPathRooted(Path)
        )
        {
            // This creates a PhysicalFileProvider
            // We simply reuse the same logic but replace it with our own provider
            base.ResolveFileProvider();

            var root = FileProvider is PhysicalFileProvider physical
                ? physical.Root
                : AppContext.BaseDirectory;
            FileProvider = new DotEnvPhysicalFileProvider(root ?? string.Empty);
        }
        else if (FileProvider is null)
        {
            var root = baseProvider is PhysicalFileProvider physical
                ? physical.Root
                : AppContext.BaseDirectory;
            FileProvider = new DotEnvPhysicalFileProvider(root ?? string.Empty);
        }
    }
}

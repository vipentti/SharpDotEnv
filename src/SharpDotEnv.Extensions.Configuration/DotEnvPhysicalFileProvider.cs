// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

namespace SharpDotEnv.Extensions.Configuration;

/// <inheritdoc/>
public sealed class DotEnvPhysicalFileProvider : PhysicalFileProvider
{
    /// <summary>
    /// Initializes a new instance of a DotEnvPhysicalFileProvider at the given root directory.
    /// using default filters; <c>Hidden | System</c>
    /// </summary>
    /// <param name="root">The root directory. This should be an absolute path.</param>
    public DotEnvPhysicalFileProvider(string root)
        : base(root, ExclusionFilters.Hidden | ExclusionFilters.System) { }

    /// <summary>
    /// Initializes a new instance of a DotEnvPhysicalFileProvider at the given root directory.
    /// </summary>
    /// <param name="root">The root directory. This should be an absolute path.</param>
    /// <param name="filters">Specifies which files or directories are excluded.</param>
    public DotEnvPhysicalFileProvider(string root, ExclusionFilters filters)
        : base(root, filters) { }
}

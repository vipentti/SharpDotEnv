// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System;
using System.IO;
using Microsoft.Extensions.FileProviders;
using SharpDotEnv.Extensions.Configuration;

namespace Microsoft.Extensions.Configuration;

/// <summary>
/// Extension methods for adding <see cref="DotEnvConfigurationProvider"/>.
/// </summary>
public static class DotEnvConfigurationExtensions
{
    /// <summary>
    /// Adds a dotenv configuration source to <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="path">Path relative to the base path stored in
    /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
    public static IConfigurationBuilder AddDotEnvFile(
        this IConfigurationBuilder builder,
        string path
    ) =>
        builder.AddDotEnvFile(
            provider: null,
            path,
            prefix: null,
            optional: false,
            reloadOnChange: false
        );

    /// <summary>
    /// Adds a dotenv configuration source to <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="path">Path relative to the base path stored in
    /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
    /// <param name="optional">Whether the file is optional.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddDotEnvFile(
        this IConfigurationBuilder builder,
        string path,
        bool optional
    ) =>
        builder.AddDotEnvFile(
            provider: null,
            path,
            prefix: null,
            optional: optional,
            reloadOnChange: false
        );

    /// <summary>
    /// Adds a dotenv configuration source to <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="path">Path relative to the base path stored in
    /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
    /// <param name="optional">Whether the file is optional.</param>
    /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddDotEnvFile(
        this IConfigurationBuilder builder,
        string path,
        bool optional,
        bool reloadOnChange
    ) =>
        builder.AddDotEnvFile(
            provider: null,
            path,
            prefix: null,
            optional: optional,
            reloadOnChange: reloadOnChange
        );

    /// <summary>
    /// Adds a dotenv configuration source to <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="path">Path relative to the base path stored in
    /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
    /// <param name="prefix">Optional prefix to filter variables in the dotenv file</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddDotEnvFile(
        this IConfigurationBuilder builder,
        string path,
        string? prefix
    ) =>
        builder.AddDotEnvFile(
            provider: null,
            path,
            prefix: prefix,
            optional: false,
            reloadOnChange: false
        );

    /// <summary>
    /// Adds a dotenv configuration source to <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="path">Path relative to the base path stored in
    /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
    /// <param name="prefix">Optional prefix to filter variables in the dotenv file</param>
    /// <param name="optional">Whether the file is optional.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddDotEnvFile(
        this IConfigurationBuilder builder,
        string path,
        string? prefix,
        bool optional
    ) =>
        builder.AddDotEnvFile(
            provider: null,
            path,
            prefix: prefix,
            optional: optional,
            reloadOnChange: false
        );

    /// <summary>
    /// Adds a dotenv configuration source to <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="path">Path relative to the base path stored in
    /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
    /// <param name="prefix">Optional prefix to filter variables in the dotenv file</param>
    /// <param name="optional">Whether the file is optional.</param>
    /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddDotEnvFile(
        this IConfigurationBuilder builder,
        string path,
        string? prefix,
        bool optional,
        bool reloadOnChange
    ) =>
        builder.AddDotEnvFile(
            provider: null,
            path,
            prefix: prefix,
            optional: optional,
            reloadOnChange: reloadOnChange
        );

    /// <summary>
    /// Adds a dotenv configuration source to <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="provider">The <see cref="IFileProvider"/> to use to access the file.</param>
    /// <param name="path">Path relative to the base path stored in
    /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
    /// <param name="prefix">Optional prefix to filter variables in the dotenv file</param>
    /// <param name="optional">Whether the file is optional.</param>
    /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"></exception>
    public static IConfigurationBuilder AddDotEnvFile(
        this IConfigurationBuilder builder,
        IFileProvider? provider,
        string path,
        string? prefix,
        bool optional,
        bool reloadOnChange
    )
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Invalid path", nameof(path));
        }

        return builder.AddDotEnvFile(s =>
        {
            s.FileProvider = provider;
            s.Path = path;
            s.Optional = optional;
            s.ReloadOnChange = reloadOnChange;
            s.Prefix = prefix;
            s.ResolveFileProvider(builder.GetFileProvider());
        });
    }

    /// <summary>
    /// Adds a dotenv configuration source to <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="configureSource">Configures the source.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddDotEnvFile(
        this IConfigurationBuilder builder,
        Action<DotEnvConfigurationSource> configureSource
    ) => builder.Add(configureSource);

    /// <summary>
    /// Adds a dotenv configuration source to <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="stream">The <see cref="Stream"/> to read the dotenv configuration data from.</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddDotEnvStream(
        this IConfigurationBuilder builder,
        Stream stream
    ) => builder.AddDotEnvStream(stream, prefix: null);

    /// <summary>
    /// Adds a dotenv configuration source to <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
    /// <param name="stream">The <see cref="Stream"/> to read the dotenv configuration data from.</param>
    /// <param name="prefix">Optional prefix to filter variables in the dotenv file</param>
    /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddDotEnvStream(
        this IConfigurationBuilder builder,
        Stream stream,
        string? prefix
    )
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.Add<DotEnvStreamConfigurationSource>(src =>
        {
            src.Prefix = prefix;
            src.Stream = stream;
        });
    }
}

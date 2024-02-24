// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System;
using System.Text;
using SharpDotEnv.Internal;

namespace SharpDotEnv;

/// <summary>
/// Configuration options for loading DotEnv files
/// </summary>
public class DotEnvConfigOptions
{
    /// <summary>
    /// Default options
    /// </summary>
    public static readonly DotEnvConfigOptions DefaultOptions = new DotEnvConfigOptions();

    /// <summary>
    /// Logger used by default
    /// </summary>
    public static readonly Action<string> DefaultDebugLogger = Console.WriteLine;

    /// <summary>
    /// Create a new instance using default options
    /// </summary>
    public DotEnvConfigOptions()
    {
        Path = ".env";
        DebugLogger = DefaultDebugLogger;
    }

    /// <summary>
    /// Create a copy based on the given options
    /// </summary>
    /// <param name="options">Options to copy from</param>
    public DotEnvConfigOptions(DotEnvConfigOptions options)
    {
        ThrowHelpers.ThrowIfNull(options);

        Path = options.Path;
        Encoding = options.Encoding;
        Override = options.Override;
        IgnoreExceptions = options.IgnoreExceptions;
        Debug = options.Debug;
        DebugLogger = options.DebugLogger;
    }

    /// <summary>
    /// <para>Default: <c>.env</c></para>
    /// <para>Path to the dotenv file. By default searches in <see cref="AppContext.BaseDirectory"/> and <see cref="System.IO.Directory.GetCurrentDirectory"/> in that order.</para>
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// <para>Default: <see cref="Encoding.UTF8"/></para>
    /// <para>Specify the encoding of the file containing environment variables.</para>
    /// </summary>
    public Encoding Encoding { get; set; } = Encoding.UTF8;

    /// <summary>
    /// <para>Default: <c>false</c></para>
    /// <para>Override any environment variables that have already been set on your machine with values from your .env file.</para>
    /// </summary>
    public bool Override { get; set; }

    /// <summary>
    /// <para>Default: <c>false</c></para>
    /// <para>Ignore exceptions thrown when trying to load the .env file</para>
    /// </summary>
    public bool IgnoreExceptions { get; set; }

    /// <summary>
    /// <para>Default: <c>false</c></para>
    /// <para>Output debug logging</para>
    /// </summary>
    public bool Debug { get; set; }

    /// <summary>
    /// <para>Default: <c>Write to stdout</c></para>
    /// <para>Write debug logs using the provided debug logger</para>
    /// </summary>
    public Action<string> DebugLogger { get; set; } = DefaultDebugLogger;

    /// <summary>
    /// Return a new instance based on this with the specified <paramref name="path"/>
    /// </summary>
    /// <param name="path">New path</param>
    /// <returns>New instance</returns>
    public DotEnvConfigOptions WithPath(string path) =>
        new DotEnvConfigOptions(this) { Path = path };

    /// <summary>
    /// Return a new instance based on this with the specified <paramref name="encoding"/>
    /// </summary>
    /// <param name="encoding">New encoding</param>
    /// <returns>New instance</returns>
    public DotEnvConfigOptions WithEncoding(Encoding encoding) =>
        new DotEnvConfigOptions(this) { Encoding = encoding };

    /// <summary>
    /// Return a new instance based on this with the specified <paramref name="value"/>
    /// </summary>
    /// <param name="value">New value to set</param>
    /// <returns>New instance</returns>
    public DotEnvConfigOptions WithOverride(bool value) =>
        new DotEnvConfigOptions(this) { Override = value };

    /// <summary>
    /// Return a new instance based on this with debug set to true
    /// </summary>
    /// <returns>New instance</returns>
    public DotEnvConfigOptions WithDebug() => new DotEnvConfigOptions(this) { Debug = true };

    /// <summary>
    /// Return a new instance based on this with the specified <paramref name="debug"/>
    /// </summary>
    /// <param name="debug">New value to set</param>
    /// <returns>New instance</returns>
    public DotEnvConfigOptions WithDebug(bool debug) =>
        new DotEnvConfigOptions(this) { Debug = debug };

    /// <summary>
    /// Return a new instance based on this with the specified <paramref name="logger"/>
    /// </summary>
    /// <param name="logger">DebugLogger to use</param>
    /// <returns>New instance</returns>
    public DotEnvConfigOptions WithDebugLogger(Action<string> logger) =>
        new DotEnvConfigOptions(this) { DebugLogger = logger };

    /// <summary>
    /// Return a new instance based on this with the specified <paramref name="ignore"/>
    /// </summary>
    /// <param name="ignore">Whether to ignore exceptions or not</param>
    /// <returns>New instance</returns>
    public DotEnvConfigOptions WithIgnoreExceptions(bool ignore) =>
        new DotEnvConfigOptions(this) { IgnoreExceptions = ignore };
}

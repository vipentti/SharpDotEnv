// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpDotEnv.Internal;

namespace SharpDotEnv;

/// <summary>
/// Environment variables parsed from a dotenv <c>.env</c> file.
/// </summary>
public class DotEnv : Dictionary<string, string>
{
    /// <summary>
    /// Default empty configuration
    /// </summary>
    public static readonly DotEnv Empty = new DotEnv();

    /// <summary>
    /// Parse environment variables from a file
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <returns>Parsed environment variables</returns>
    public static DotEnv ParseFile(string filePath) => ParseFile(filePath, null);

    /// <summary>
    /// Parse environment variables from a file with the given encoding
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <param name="encoding">Encoding to use when parsing the file. Default is UTF8</param>
    /// <returns>Parsed environment variables</returns>
    public static DotEnv ParseFile(string filePath, Encoding? encoding)
    {
        return Parser.ParseEnvironment(File.ReadAllText(filePath, encoding ?? Encoding.UTF8));
    }

    /// <summary>
    /// Parse environment variables from the given string.
    /// </summary>
    /// <param name="env">Variables in dotenv format</param>
    /// <returns>Parsed environment variables</returns>
    public static DotEnv Parse(string env) => Parser.ParseEnvironment(env);

    /// <summary>
    /// Parse environment variables from the stream
    /// </summary>
    /// <param name="stream">Stream of environment variables</param>
    /// <returns>Parsed environment variables</returns>
    public static DotEnv Parse(Stream stream) => Parser.ParseEnvironment(stream);

    /// <summary>
    /// Parse environment variables from the stream with the given encoding
    /// </summary>
    /// <param name="stream">Stream of environment variables</param>
    /// <param name="encoding">Encoding of the stream</param>
    /// <returns>Parsed environment variables</returns>
    public static DotEnv Parse(Stream stream, Encoding encoding) =>
        Parser.ParseEnvironment(stream, encoding);

    /// <summary>
    /// Parse environment variables using the provided options
    /// </summary>
    /// <param name="options">Options used for reading the environment variables</param>
    /// <returns>Environment variables read based on the options</returns>
    /// <exception cref="ArgumentNullException">When options are null</exception>
    public static DotEnv Parse(DotEnvConfigOptions options)
    {
        ThrowHelpers.ThrowIfNull(options);

        try
        {
            var path = Utils.GetEnvFilePath(options.Path, 1, options.IgnoreExceptions);

            if (string.IsNullOrEmpty(path))
            {
                return Empty;
            }

            if (options.Debug && options.DebugLogger != null)
            {
                options.DebugLogger($"Loading env file: {path}");
            }

            using (var stream = File.OpenRead(path))
            {
                return Parse(stream, options.Encoding);
            }
        }
        catch (Exception ex) when (options.IgnoreExceptions)
        {
            if (options.Debug && options.DebugLogger != null)
            {
                options.DebugLogger($"Parse failed: {ex}");
            }

            return new DotEnv();
        }
    }

    /// <summary>
    /// Parse and configure the environment variables using the default options
    /// </summary>
    /// <returns>Environment variables read based on the options</returns>
    public static DotEnv Config() => Config(DotEnvConfigOptions.DefaultOptions);

    /// <summary>
    /// Parse and configure environment variables using the provided options
    /// </summary>
    /// <param name="options">Options used for reading the environment variables</param>
    /// <returns>Environment variables read based on the options</returns>
    public static DotEnv Config(DotEnvConfigOptions options) =>
        Parse(options).SetEnvironmentVariables(options.Override);

    /// <summary>
    /// Set the current values from this instance as environment variables
    /// </summary>
    /// <param name="overrideExisting">If <c>true</c> existing environment variables will be overriden</param>
    /// <returns>This instance</returns>
    public DotEnv SetEnvironmentVariables(bool overrideExisting)
    {
        foreach (var kvp in this)
        {
            var existing = Environment.GetEnvironmentVariable(kvp.Key);

            if (existing == null || overrideExisting)
            {
                Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
            }
        }

        return this;
    }

    /// <summary>
    /// Initializes a new instance that is empty.
    /// </summary>
    public DotEnv() { }

    /// <summary>
    /// Initializes a new instance copying values from the <paramref name="dictionary"/>
    /// </summary>
    /// <param name="dictionary">The dictionary to copy values from</param>
    public DotEnv(IDictionary<string, string> dictionary)
        : base(dictionary) { }

    /// <summary>
    /// Initialize a new instance with the specified capacity
    /// </summary>
    /// <param name="capacity">Initial capacity</param>
    public DotEnv(int capacity)
        : base(capacity) { }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    /// <summary>
    /// Initializes a new instance copying values from the <paramref name="collection"/>
    /// </summary>
    /// <param name="collection">The collection to copy values from</param>
    public DotEnv(IEnumerable<KeyValuePair<string, string>> collection)
        : base(collection) { }
#endif
}

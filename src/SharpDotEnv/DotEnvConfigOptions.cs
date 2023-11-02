using System;
using System.Text;

namespace SharpDotEnv
{
    /// <summary>
    /// Configuration options for loading DotEnv files
    /// </summary>
    public class DotEnvConfigOptions
    {
        public static readonly DotEnvConfigOptions DefaultOptions = new DotEnvConfigOptions();
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

        public DotEnvConfigOptions WithPath(string path)
            => new DotEnvConfigOptions(this) { Path = path };

        public DotEnvConfigOptions WithEncoding(Encoding encoding)
            => new DotEnvConfigOptions(this) { Encoding = encoding };

        public DotEnvConfigOptions WithOverride(bool @override)
            => new DotEnvConfigOptions(this) { Override = @override };

        public DotEnvConfigOptions WithDebug()
            => new DotEnvConfigOptions(this) { Debug = true };

        public DotEnvConfigOptions WithDebug(bool debug)
            => new DotEnvConfigOptions(this) { Debug = debug };

        public DotEnvConfigOptions WithDebugLogger(Action<string> logger)
            => new DotEnvConfigOptions(this) { DebugLogger = logger };

        public DotEnvConfigOptions WithIgnoreExceptions(bool ignore)
            => new DotEnvConfigOptions(this) { IgnoreExceptions = ignore };
    }
}

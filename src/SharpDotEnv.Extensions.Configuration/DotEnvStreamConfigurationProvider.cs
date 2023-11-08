﻿using Microsoft.Extensions.Configuration;
using System.IO;

namespace SharpDotEnv.Extensions.Configuration
{
    /// <summary>
    /// A dotenv file based <see cref="StreamConfigurationProvider"/>.
    /// </summary>
    public class DotEnvStreamConfigurationProvider : StreamConfigurationProvider
    {
        private readonly string _prefix;
        private readonly string _normalizedPrefix;

        /// <summary>
        /// Initializes a new instance with the specified source with the specified prefix
        /// </summary>
        /// <param name="source">The source settings.</param>
        public DotEnvStreamConfigurationProvider(StreamConfigurationSource source) : base(source)
        {
            _prefix = string.Empty;
            _normalizedPrefix = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance with the specified source with the specified prefix
        /// </summary>
        /// <param name="source">The source settings.</param>
        /// <param name="prefix">A prefix used to filter the environment variables in the dotenv file.</param>
        public DotEnvStreamConfigurationProvider(
            StreamConfigurationSource source,
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            string? prefix
#else
            string prefix
#endif
            ) : base(source)
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
}

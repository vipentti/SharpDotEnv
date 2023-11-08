﻿using Microsoft.Extensions.Configuration;

namespace SharpDotEnv.Extensions.Configuration
{
    /// <summary>
    /// Configuration source for stream containing dotenv formatted data
    /// </summary>
    public class DotEnvStreamConfigurationSource : StreamConfigurationSource
    {
        /// <summary>
        /// A prefix used to filter environment variables in the dotenv file
        /// </summary>
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public string? Prefix { get; set; }
#else
        public string Prefix { get; set; }
#endif

        /// <inheritdoc />
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
            => new DotEnvStreamConfigurationProvider(this, Prefix);
    }
}

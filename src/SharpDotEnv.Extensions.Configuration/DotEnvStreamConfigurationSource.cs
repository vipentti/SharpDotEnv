using Microsoft.Extensions.Configuration;

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
        public string? Prefix { get; set; }

        /// <inheritdoc />
        public override IConfigurationProvider Build(IConfigurationBuilder builder) =>
            new DotEnvStreamConfigurationProvider(this, Prefix);
    }
}

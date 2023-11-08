using System;
using System.IO;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace SharpDotEnv.Extensions.Configuration.Tests;

public static class DotEnvConfigurationExtensionsTests
{
    public class AddDotEnvFile
    {
        private readonly ConfigurationBuilder builder = new ConfigurationBuilder();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Throws_IfFilePathIsNullOrEmpty(string? path)
        {
            var act = () => DotEnvConfigurationExtensions.AddDotEnvFile(builder, path!);

            act.Should()
                .ThrowExactly<ArgumentException>()
                .Which
                .ParamName
                .Should()
                .Be(nameof(path));
        }

        [Fact]
        public void Throws_IfFileDoesNotExistAndIsNotOptional()
        {
            var path = ".env-does-not-exist";
            var act = () => builder.AddDotEnvFile(path, optional: false).Build();
            act.Should()
                .ThrowExactly<FileNotFoundException>()
                .WithMessage(
                    $"The configuration file '{path}' was not found and is not optional.*"
                );
        }

        [Fact]
        public void DoesNotThrow_IfFileDoesNotExistAndIsOptional()
        {
            var act = () => builder.AddDotEnvFile(".env-does-not-exist", optional: true).Build();
            act.Should().NotThrow();
        }

        [Fact]
        public void SupportsLoadingDataFromFileProvider()
        {
            var env = """
            test = value
            """;

            builder.AddDotEnvFile(
                provider: env.StringToFileProvider(),
                ".env-does-not-exist",
                prefix: null,
                optional: false,
                reloadOnChange: false
            );

            var config = builder.Build();
            config["test"].Should().Be("value");
        }

        [Fact]
        public void SupportsLoadingDataFromFileProviderWithPrefix()
        {
            var env = """
            notincluded = value
            PREFIX_included = value
            """;

            builder.AddDotEnvFile(
                provider: env.StringToFileProvider(),
                ".env-does-not-exist",
                prefix: "PREFIX_",
                optional: false,
                reloadOnChange: false
            );

            var config = builder.Build();
            config["included"].Should().Be("value");
            config["notincluded"].Should().BeNull();
        }
    }

    public class AddDotEnvStream
    {
        private readonly ConfigurationBuilder builder = new ConfigurationBuilder();

        [Fact]
        public void Throws_IfStreamIsNull()
        {
            var act = () => builder.AddDotEnvStream(null!).Build();

            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Source.Stream cannot be null.");
        }

        [Fact]
        public void SupportsLoadingDataFromStream()
        {
            var env = """
            test = value
            """;

            var config = builder.AddDotEnvStream(env.StringToStream()).Build();
            config["test"].Should().Be("value");
        }

        [Fact]
        public void SupportsLoadingDataFromStreamWithPrefix()
        {
            var env = """
            notincluded = value
            PREFIX_included = value
            """;

            var config = builder.AddDotEnvStream(env.StringToStream(), prefix: "PREFIX_").Build();
            config["included"].Should().Be("value");
            config["notincluded"].Should().BeNull();
        }

        [Fact]
        public void Throws_IfReadingMultipleTimesFromStream()
        {
            var env = """
            test = value
            """;

            _ = builder.AddDotEnvStream(env.StringToStream()).Build();

            var act = () => builder.Build();

            act.Should().ThrowExactly<ArgumentException>().WithMessage("Stream was not readable.");
        }

        [Fact]
        public void Throws_IfStreamWasDisposed()
        {
            var env = """
            test = value
            """;
            using (var stream = env.StringToStream())
            {
                builder.AddDotEnvStream(stream);
            }

            var act = () => builder.Build();

            act.Should().ThrowExactly<ArgumentException>().WithMessage("Stream was not readable.");
        }
    }
}

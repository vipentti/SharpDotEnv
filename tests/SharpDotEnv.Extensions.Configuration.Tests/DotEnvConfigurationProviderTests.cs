// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace SharpDotEnv.Extensions.Configuration.Tests;

public class DotEnvConfigurationProviderTests
{
    private static ConfigurationProvider GetDotEnvProvider(string env, string? prefix = null)
    {
        var provider = new DotEnvConfigurationProvider(
            new DotEnvConfigurationSource() { Prefix = prefix }
        );
        provider.Load(env.StringToStream());
        return provider;
    }

    [Fact]
    public void DoubleUnderScoresAreConvertedToKeyPaths()
    {
        var env = """
            Nested__Path__With__Keys = some nested value
            """;

        var provider = GetDotEnvProvider(env);

        provider.Get("Nested:Path:With:Keys").Should().Be("some nested value");
    }

    [Fact]
    public void DoubleUnderScoresAlsoWorkWithPrefixes()
    {
        var env = """
            Some__Other__Nested = `does not get included`
            PREFIX_Nested__Path__With__Keys = some nested value
            """;

        var provider = GetDotEnvProvider(env, prefix: "PREFIX_");

        provider.Get("Nested:Path:With:Keys").Should().Be("some nested value");

        var notfound = () => provider.Get("Some:Other:Nested");
        notfound.Should().ThrowExactly<KeyNotFoundException>();
    }

    [Fact]
    public void Supports_EmptyValues()
    {
        var env = """
            empty =  # comment
            """;

        var provider = GetDotEnvProvider(env);

        provider.Get("empty").Should().BeEmpty();
    }

    [Fact]
    public void Supports_MultilineValues()
    {
        var env = """
            multiline = "this is a value
            on multiple
            lines"
            """;

        var provider = GetDotEnvProvider(env);

        provider
            .Get("multiline")
            .Should()
            .Be(
                """
                this is a value
                on multiple
                lines
                """.Replace("\r\n", "\n")
            );
    }
}

// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using FluentAssertions;
using SharpDotEnv.Exceptions;
using Xunit;

namespace SharpDotEnv.Tests;

public class ParserTests
{
    public static readonly TheoryData<string, DotEnv> ParserTestCases = new TheoryData<
        string,
        DotEnv
    >()
    {
        {
            @"
        # comment
        ",
            DotEnv.Empty
        },
        {
            @"
        empty=
        ",
            new DotEnv() { { "empty", "" }, }
        },
        {
            @"
        empty=
        not-empty= not-empty-value works quite well
        ",
            new DotEnv() { { "empty", "" }, { "not-empty", "not-empty-value works quite well" }, }
        },
    };

    [Theory]
    [MemberData(nameof(ParserTestCases))]
    public void Should_return_expected_environment_variables(string input, DotEnv expected)
    {
        var parsed = DotEnv.Parse(input);
        parsed.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Should_throw_ParseException_when_KeyToken_IsNotKey()
    {
        const string env = """
            = hello
            """;

        var act = () => DotEnv.Parse(env);

        act.Should()
            .ThrowExactly<DotEnvParseException>()
            .WithMessage("Expected key. Got 'Value='hello''");
    }

    [Fact]
    public void Should_throw_ParseException_when_ValueToken_IsEof()
    {
        const string env = """
            hello
            """;

        var act = () => DotEnv.Parse(env);

        act.Should()
            .ThrowExactly<DotEnvParseException>()
            .WithMessage("Expected value for key: 'hello'");
    }
}

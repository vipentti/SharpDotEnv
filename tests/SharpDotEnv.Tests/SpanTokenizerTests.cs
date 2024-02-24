// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System.Collections.Generic;
using FluentAssertions;
using SharpDotEnv.Internal;
using Xunit;

namespace SharpDotEnv.Tests;

public class SpanTokenizerTests
{
    [Theory]
    [MemberData(
        nameof(TokenTestCases.WithCommentsAndWhitespace),
        MemberType = typeof(TokenTestCases)
    )]
    public void Should_return_expected_tokens(string input, TestToken[] expectedTokens)
    {
        var tokenizer = new SpanTokenizer(input.Trim());
        var readTokens = new List<TestToken>();
        while (tokenizer.MoveNext(out var tok))
        {
            readTokens.Add(TestToken.From(tok));
        }

        readTokens.Should().BeEquivalentTo(expectedTokens, options => options.IncludingInternalProperties());
    }

    [Fact]
    public void Sample_test()
    {
        var tokenizer = new SpanTokenizer(
            @"
        value = 1234
        ".Trim()
        );
        var readTokens = new List<TestToken>();
        while (tokenizer.MoveNext(out var tok))
        {
            readTokens.Add(TestToken.From(tok));
        }

        readTokens
            .Should()
            .BeEquivalentTo(
                new TestToken[]
                {
                    new TestToken(TokenType.Key, "value"),
                    new TestToken(TokenType.Whitespace, " "),
                    new TestToken(TokenType.Equals, "="),
                    new TestToken(TokenType.Whitespace, " "),
                    new TestToken(TokenType.Value, "1234"),
                }
                , options => options.IncludingInternalProperties()
            );
    }

    [Fact]
    public void Skip_comments_and_whitespace()
    {
        var tokenizer = new SpanTokenizer(
            @"
        # this is a comment
        value1 = 1
        # this is a comment 2
        # this is a comment 3
        value2 = 2
        ".Trim(),
            skipComments: true,
            skipWhitespace: true
        );

        var readTokens = new List<TestToken>();
        while (tokenizer.MoveNext(out var tok))
        {
            readTokens.Add(TestToken.From(tok));
        }

        var expectedTokens = new TestToken[]
        {
            new TestToken(TokenType.Key, "value1"),
            new TestToken(TokenType.Value, "1"),
            new TestToken(TokenType.Key, "value2"),
            new TestToken(TokenType.Value, "2"),
        };

        readTokens.Should().BeEquivalentTo(expectedTokens, options => options.IncludingInternalProperties());
    }

    [Fact]
    public void Can_tokenize()
    {
        var tokenizer = new SpanTokenizer(
            @"

        # this is a comment

        single-value=1

        single-quoted-value='some value in single quotes'

        double-quoted-value=""some value in double quotes""

        backtick-quoted-value=`some value in backtick quotes`

        ",
            skipWhitespace: true
        );

        var expectedTokens = new TestToken[]
        {
            new TestToken(TokenType.Comment, $"# this is a comment"),
            new TestToken(TokenType.Key, "single-value"),
            new TestToken(TokenType.Value, "1"),
            new TestToken(TokenType.Key, "single-quoted-value"),
            new TestToken(TokenType.SingleQuoteValue, "some value in single quotes"),
            new TestToken(TokenType.Key, "double-quoted-value"),
            new TestToken(TokenType.DoubleQuoteValue, "some value in double quotes"),
            new TestToken(TokenType.Key, "backtick-quoted-value"),
            new TestToken(TokenType.BacktickValue, "some value in backtick quotes"),
        };

        var readTokens = new List<TestToken>();
        while (tokenizer.MoveNext(out var tok))
        {
            readTokens.Add(TestToken.From(tok));
        }

        readTokens.Should().BeEquivalentTo(expectedTokens, options => options.IncludingInternalProperties());
    }
}

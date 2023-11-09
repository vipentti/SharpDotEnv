// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using SharpDotEnv.Internal;
using Xunit;

namespace SharpDotEnv.Tests
{
    public class StreamTokenizerTests
    {
        [Theory]
        [MemberData(
            nameof(TokenTestCases.WithCommentsAndWhitespace),
            MemberType = typeof(TokenTestCases)
        )]
        public void Should_return_expected_tokens(string input, TestToken[] expectedTokens)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(input.Trim())))
            using (var reader = new StreamReader(stream))
            {
                var tokenizer = new StreamTokenizer(reader);

                var readTokens = new List<TestToken>();

                while (tokenizer.MoveNext(out var tok))
                {
                    readTokens.Add(TestToken.From(tok));
                }

                readTokens.Should().BeEquivalentTo(expectedTokens);
            }
        }

        [Fact]
        public void Sample_test()
        {
            using (
                var stream = new MemoryStream(
                    Encoding
                        .UTF8
                        .GetBytes(
                            @"
        value = 1234
        ".Trim()
                        )
                )
            )
            using (var reader = new StreamReader(stream))
            {
                var tokenizer = new StreamTokenizer(reader);
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
                    );
            }
        }
    }
}

// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System;
using SharpDotEnv.Internal;
using Xunit;

namespace SharpDotEnv.Tests
{
    internal static class TokenTestCases
    {
        public static readonly TheoryData<string, TestToken[]> WithCommentsAndWhitespace =
            new TheoryData<string, TestToken[]>()
            {
                { "", Array.Empty<TestToken>() },
                {
                    @"
# comment

    # another comment
        ",
                    new TestToken[]
                    {
                        new TestToken(TokenType.Comment, "# comment"),
                        new TestToken(
                            TokenType.Whitespace,
                            Environment.NewLine + Environment.NewLine + "    "
                        ),
                        new TestToken(TokenType.Comment, "# another comment"),
                        // new TestToken(TokenType.Whitespace, Environment.NewLine),
                    }
                },
                {
                    @"
        empty=
        ",
                    new TestToken[]
                    {
                        new TestToken(TokenType.Key, "empty"),
                        new TestToken(TokenType.Equals, "="),
                        new TestToken(TokenType.Value, ""),
                    }
                },
                {
                    @"
empty=
    not-empty= not-empty-value works quite well
        ",
                    new TestToken[]
                    {
                        new TestToken(TokenType.Key, "empty"),
                        new TestToken(TokenType.Equals, "="),
                        new TestToken(TokenType.Value, ""),
                        new TestToken(TokenType.Whitespace, Environment.NewLine + "    "),
                        new TestToken(TokenType.Key, "not-empty"),
                        new TestToken(TokenType.Equals, "="),
                        new TestToken(TokenType.Whitespace, " "),
                        new TestToken(TokenType.Value, "not-empty-value works quite well"),
                    }
                },
                {
                    @"
        not-empty  =   leading whitespace    and  trailing    whitespace is ignored   #end
        ",
                    new TestToken[]
                    {
                        new TestToken(TokenType.Key, "not-empty"),
                        new TestToken(TokenType.Whitespace, "  "),
                        new TestToken(TokenType.Equals, "="),
                        new TestToken(TokenType.Whitespace, "   "),
                        new TestToken(
                            TokenType.Value,
                            "leading whitespace    and  trailing    whitespace is ignored"
                        ),
                        new TestToken(TokenType.Whitespace, "   "),
                        new TestToken(TokenType.Comment, "#end"),
                    }
                },
                {
                    @"
        DONT_EXPAND_UNQUOTED=dontexpand\nnewlines
        ",
                    new TestToken[]
                    {
                        new TestToken(TokenType.Key, "DONT_EXPAND_UNQUOTED"),
                        new TestToken(TokenType.Equals, "="),
                        new TestToken(TokenType.Value, "dontexpand\\nnewlines"),
                    }
                },
                {
                    @"
        INLINE_COMMENTS_SPACE=inline comments start with a#number sign. no space required.
        ",
                    new TestToken[]
                    {
                        new TestToken(TokenType.Key, "INLINE_COMMENTS_SPACE"),
                        new TestToken(TokenType.Equals, "="),
                        new TestToken(TokenType.Value, "inline comments start with a"),
                        new TestToken(TokenType.Comment, "#number sign. no space required."),
                    }
                },
                {
                    @"
        EQUAL_SIGN_IN_VALUE_PREFIX====equals sign works = in value  # end
        ",
                    new TestToken[]
                    {
                        new TestToken(TokenType.Key, "EQUAL_SIGN_IN_VALUE_PREFIX"),
                        new TestToken(TokenType.Equals, "="),
                        new TestToken(TokenType.Value, "===equals sign works = in value"),
                        new TestToken(TokenType.Whitespace, "  "),
                        new TestToken(TokenType.Comment, "# end"),
                    }
                },
                {
                    @"
        EQUAL_SIGN_IN_VALUE_SUFFIX=equals sign works = in value ====  # end
        ",
                    new TestToken[]
                    {
                        new TestToken(TokenType.Key, "EQUAL_SIGN_IN_VALUE_SUFFIX"),
                        new TestToken(TokenType.Equals, "="),
                        new TestToken(TokenType.Value, "equals sign works = in value ===="),
                        new TestToken(TokenType.Whitespace, "  "),
                        new TestToken(TokenType.Comment, "# end"),
                    }
                },
            };
    }
}

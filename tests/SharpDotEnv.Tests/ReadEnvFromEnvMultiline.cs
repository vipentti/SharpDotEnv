// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System.IO;
using System.Text;
using FluentAssertions;
using Xunit;

namespace SharpDotEnv.Tests
{
    public class ReadEnvFromEnvMultiline
    {
        private readonly DotEnv fileEnv = DotEnv.ParseFile(
            Path.Combine(Directory.GetCurrentDirectory(), ".env-multiline"),
            Encoding.UTF8
        );
        private readonly DotEnv streamEnv = DotEnv.Parse(
            File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), ".env-multiline")),
            Encoding.UTF8
        );

        [Theory]
        [InlineData("BASIC", "basic")]
        [InlineData("AFTER_LINE", "after_line")]
        [InlineData("EMPTY", "")]
        [InlineData("SINGLE_QUOTES", "single_quotes")]
        [InlineData("SINGLE_QUOTES_SPACED", "    single quotes    ")]
        [InlineData("DOUBLE_QUOTES", "double_quotes")]
        [InlineData("DOUBLE_QUOTES_SPACED", "    double quotes    ")]
        [InlineData("EXPAND_NEWLINES", "expand\nnew\nlines")]
        [InlineData("EXPAND_CR", "expand\rcarriage\rreturns")]
        [InlineData("EXPAND_CRLF", "expand\r\ncarriage\r\nreturns")]
        [InlineData("DONT_EXPAND_UNQUOTED", "dontexpand\\nnewlines")]
        [InlineData("DONT_EXPAND_SQUOTED", "dontexpand\\nnewlines")]
        [InlineData("DONT_EXPAND_UNQUOTED_CR", "dontexpand\\rnewlines")]
        [InlineData("DONT_EXPAND_SQUOTED_CR", "dontexpand\\rnewlines")]
        [InlineData("DONT_EXPAND_UNQUOTED_CRLF", "dontexpand\\r\\nnewlines")]
        [InlineData("DONT_EXPAND_SQUOTED_CRLF", "dontexpand\\r\\nnewlines")]
        [InlineData("EQUAL_SIGNS", "equals==")]
        [InlineData("RETAIN_INNER_QUOTES", "{\"foo\": \"bar\"}")]
        [InlineData("RETAIN_INNER_QUOTES_AS_STRING", "{\"foo\": \"bar\"}")]
        [InlineData("TRIM_SPACE_FROM_UNQUOTED", "some spaced out string")]
        [InlineData("USERNAME", "therealnerdybeast@example.tld")]
        [InlineData("SPACED_KEY", "parsed")]
        [InlineData("MULTI_DOUBLE_QUOTED", "THIS\nIS\nA\nMULTILINE\nSTRING")]
        [InlineData("MULTI_SINGLE_QUOTED", "THIS\nIS\nA\nMULTILINE\nSTRING")]
        [InlineData("MULTI_BACKTICKED", "THIS\nIS\nA\n\"MULTILINE\'S\"\nSTRING")]
        public void Should_have_expected_value(string key, string expected)
        {
            fileEnv[key].Should().Be(expected);
            streamEnv[key].Should().Be(expected);
        }
    }
}

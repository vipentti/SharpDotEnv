using FluentAssertions;
using Xunit;

namespace SharpDotEnv.Tests
{
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
                new DotEnv()
                {
                    { "empty", "" },
                    { "not-empty", "not-empty-value works quite well" },
                }
            },
        };

        [Theory]
        [MemberData(nameof(ParserTestCases))]
        public void Should_return_expected_environment_variables(string input, DotEnv expected)
        {
            var parsed = DotEnv.Parse(input);
            parsed.Should().BeEquivalentTo(expected);
        }
    }
}

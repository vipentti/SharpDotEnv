using FluentAssertions;
using SharpDotEnv.Internal;
using System.IO;
using Xunit;

namespace SharpDotEnv.Tests.Internal
{
    public class UtilsTests
    {
        public static readonly string Sep = Path.DirectorySeparatorChar.ToString();

        [Theory]
        [MemberData(nameof(Paths))]
        public void PathCombine_Should_Combine_Paths_Expectedly(string basePath, string[] paths, string expected)
        {
            var result = Utils.PathCombine(basePath, paths);
            result.Should().Be(expected);
        }

        public static readonly TheoryData<string, string[], string> Paths = new TheoryData<string, string[], string>()
        {
            {"base-path", new[] { "with/separator" }, string.Join(Sep, "base-path", "with", "separator") },
            {"base-path", new[] { "with/separator", "\\other" }, string.Join(Sep, "base-path", "with", "separator", "other") },
            {"base-path", new[] { "with/separator\\other/nested1", "nested2/nested3\\nested4" }, string.Join(Sep, "base-path", "with", "separator", "other", "nested1", "nested2", "nested3", "nested4") },
        };
    }
}

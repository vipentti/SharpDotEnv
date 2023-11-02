using SharpDotEnv.Internal;
using Xunit;

namespace SharpDotEnv.Tests
{
    public class SpanTokenTheoryAttribute : TheoryAttribute
    {
        public SpanTokenTheoryAttribute()
        {
            if (!Parser.SupportSpanTokens)
            {
                Skip = "SpanToken not supported";
            }
        }
    }
}

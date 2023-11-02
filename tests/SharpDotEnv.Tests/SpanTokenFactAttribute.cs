using SharpDotEnv.Internal;
using Xunit;

namespace SharpDotEnv.Tests
{
    public class SpanTokenFactAttribute : FactAttribute
    {
        public SpanTokenFactAttribute()
        {
            if (!Parser.SupportSpanTokens)
            {
                Skip = "SpanToken not supported";
            }
        }
    }
}

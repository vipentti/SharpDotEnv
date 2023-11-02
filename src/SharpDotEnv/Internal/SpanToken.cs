
namespace SharpDotEnv.Internal
{
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    using System;

    internal readonly ref struct SpanToken
    {
        public SpanToken(TokenType type, ReadOnlySpan<char> value)
        {
            Type = type;
            Value = value;
        }

        public TokenType Type { get; }

        public ReadOnlySpan<char> Value { get; }

        public override string ToString() => $"{Type}='{Value.ToString()}'";
    }
#else
    // simple implementation to ensure tests compile
    internal readonly struct SpanToken
    {
        public SpanToken(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }
        public TokenType Type { get; }
        public string Value { get; }
        public override string ToString() => $"{Type}='{Value}'";

    }
#endif

    internal static class SpanTokenExtensions
    {
        public static StreamToken ToStreamToken(this SpanToken spanToken) => new StreamToken(spanToken.Type, spanToken.Value.ToString());
    }
}

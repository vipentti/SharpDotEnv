namespace SharpDotEnv.Internal
{
    public enum TokenType
    {
        Eof,
        Whitespace,
        Equals,
        Comment,
        Key,
        Value,
        SingleQuoteValue,
        DoubleQuoteValue,
        BacktickValue,
    }

    internal static class TokenTypeExtensions
    {
        public static bool IsEof(this TokenType tokenType) =>
            tokenType == TokenType.Eof;

        public static bool IsWhitespace(this TokenType tokenType) =>
            tokenType == TokenType.Whitespace || tokenType == TokenType.Equals;

        public static bool IsValue(this TokenType tokenType) =>
            tokenType == TokenType.Value
            || tokenType == TokenType.SingleQuoteValue
            || tokenType == TokenType.DoubleQuoteValue
            || tokenType == TokenType.BacktickValue;

        public static bool IsEof(this StreamToken tok) => tok.Type.IsEof();
        public static bool IsValue(this StreamToken tok) => tok.Type.IsValue();
        public static bool IsWhitespace(this StreamToken tok) => tok.Type.IsWhitespace();
        public static bool IsComment(this StreamToken tok) => tok.Type == TokenType.Comment;

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public static bool IsEof(this SpanToken tok) => tok.Type.IsEof();
        public static bool IsValue(this SpanToken tok) => tok.Type.IsValue();
        public static bool IsWhitespace(this SpanToken tok) => tok.Type.IsWhitespace();
        public static bool IsComment(this SpanToken tok) => tok.Type == TokenType.Comment;
#endif
    }
}

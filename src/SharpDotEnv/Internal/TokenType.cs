// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

namespace SharpDotEnv.Internal
{
    internal enum TokenType
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
        public static bool IsEof(this TokenType tokenType) => tokenType == TokenType.Eof;

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

        public static bool IsEof(this SpanToken tok) => tok.Type.IsEof();

        public static bool IsValue(this SpanToken tok) => tok.Type.IsValue();

        public static bool IsWhitespace(this SpanToken tok) => tok.Type.IsWhitespace();

        public static bool IsComment(this SpanToken tok) => tok.Type == TokenType.Comment;
    }
}

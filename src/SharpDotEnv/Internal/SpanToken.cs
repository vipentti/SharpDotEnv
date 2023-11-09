// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

namespace SharpDotEnv.Internal
{
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

    internal static class SpanTokenExtensions
    {
        public static StreamToken ToStreamToken(this SpanToken spanToken) =>
            new StreamToken(spanToken.Type, spanToken.Value.ToString());
    }
}

// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using SharpDotEnv.Internal;

namespace SharpDotEnv.Tests;

public readonly record struct TestToken
{
    internal TestToken(TokenType type, string value)
    {
        Type = type;
        Value = value;
    }

    internal TokenType Type { get; }
    internal string Value { get; }

    public override string ToString() => $"{Type}='{Value}'";

    internal static TestToken From(SpanToken tok) =>
        new TestToken(tok.Type, tok.Value.ToString());

    internal static TestToken From(StreamToken tok) => new TestToken(tok.Type, tok.Value);
}

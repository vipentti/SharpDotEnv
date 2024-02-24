// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

namespace SharpDotEnv.Internal;

internal readonly struct StreamToken
{
    public StreamToken(TokenType type, string value)
    {
        Type = type;
        Value = value;
    }

    public TokenType Type { get; }

    public string Value { get; }

    public override string ToString() => $"{Type}='{Value}'";
}

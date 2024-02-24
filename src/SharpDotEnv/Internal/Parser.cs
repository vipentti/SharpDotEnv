// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System.IO;
using System.Text;
using SharpDotEnv.Exceptions;
using System;
using System.Runtime.CompilerServices;

namespace SharpDotEnv.Internal;

internal static class Parser
{
    public static DotEnv ParseEnvironment(string input)
    {
        var variables = new DotEnv();
        ParseEnvironment(variables, input);
        return variables;
    }

    public static DotEnv ParseEnvironment(Stream input, Encoding? encoding = default)
    {
        var variables = new DotEnv();
        ParseEnvironment(variables, input, encoding);
        return variables;
    }

    private static void ParseEnvironment(DotEnv variables, string input)
    {
        var tokenizer = new SpanTokenizer(input, skipComments: true, skipWhitespace: true);

        while (tokenizer.MoveNext(out var keyToken))
        {
            _ = tokenizer.MoveNext(out var valueToken);
            AddToEnvironment(variables, keyToken.ToStreamToken(), valueToken.ToStreamToken());
        }
    }

    private static void ParseEnvironment(
        DotEnv variables,
        Stream input,
        Encoding? encoding = default
    )
    {
        using (var streamReader = new StreamReader(input, encoding ?? Encoding.UTF8))
        {
            var tokenizer = new StreamTokenizer(
                streamReader,
                skipComments: true,
                skipWhitespace: true
            );

            while (tokenizer.MoveNext(out var keyToken))
            {
                _ = tokenizer.MoveNext(out var valueToken);
                AddToEnvironment(variables, keyToken, valueToken);
            }
        }
    }

    private static void AddToEnvironment(
        DotEnv variables,
        StreamToken keyToken,
        StreamToken valueToken
    )
    {
        ValidateTokens(keyToken, valueToken);

        // Normalize line endings
        var value = valueToken.Value;

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        value = value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace("\r", "\n", StringComparison.Ordinal);
#else
        value = value.Replace("\r\n", "\n").Replace("\r", "\n");
#endif

        if (valueToken.Type == TokenType.DoubleQuoteValue)
        {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            value = value.Replace("\\n", "\n", StringComparison.Ordinal).Replace("\\r", "\r", StringComparison.Ordinal);
#else
            value = value.Replace("\\n", "\n").Replace("\\r", "\r");
#endif
        }

        variables[keyToken.Value] = value;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ValidateTokens(StreamToken keyToken, StreamToken valueToken)
    {
        if (keyToken.Type != TokenType.Key)
        {
            throw new DotEnvParseException(Strings.FormatError_ExpectedKey(keyToken));
        }

        var key = keyToken.Value;

        if (valueToken.IsEof())
        {
            throw new DotEnvParseException(Strings.FormatError_ExpectedValueForKey(key));
        }

        if (!valueToken.IsValue())
        {
            throw new DotEnvParseException(Strings.FormatError_InvalidValue(key, valueToken));
        }
    }
}

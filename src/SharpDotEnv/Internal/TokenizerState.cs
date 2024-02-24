// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

namespace SharpDotEnv.Internal;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

internal class TokenizerState
{
    internal LexMode LexMode;
    internal int Position;
    internal int Line;
    internal int Column;
    internal int Steps;
    internal int Start;
    internal readonly bool SkipComments;
    internal readonly bool SkipWhitespace;

    public TokenizerState(bool skipComments, bool skipWhitespace)
    {
        SkipComments = skipComments;
        SkipWhitespace = skipWhitespace;
        Position = 0;
        Line = 0;
        Column = 0;
        Steps = 0;
        LexMode = LexMode.Key;
    }

    internal void CheckStep(char current)
    {
        Steps++;
        Debug.Assert(Steps < 1_000_000, $"Parser seems stuck at '{current}'");
    }

    internal void ResetStart()
    {
        Start = Position;
    }

    internal void AdvancePosition()
    {
        Position++;
    }

    internal void BumpPositionAndColumn()
    {
        Position++;
        Column++;
    }

    internal void BumpLine()
    {
        Line++;
        Column = 0;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal NotSupportedException CreateNotSupportedCharacterException(char ch) =>
        new($"Character: '{ch}' at {Line}:{Column} (position: {Position}) is not supported.");

#if NETCOREAPP2_1_OR_GREATER || NET5_0_OR_GREATER
    [DoesNotReturn]
#endif
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal void ThrowNotSupportedCharacterException(char ch) =>
        throw CreateNotSupportedCharacterException(ch);

#if NETCOREAPP2_1_OR_GREATER || NET5_0_OR_GREATER
    [DoesNotReturn]
#endif
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal void ThrowExpectedToConsumeException(char expected, char actual)
    {
        throw new NotSupportedException(
            $"Expected to consume '{TokenizerUtils.GetEscapeSequence(expected)}' at {Line}:{Column} (position: {Position}) but found '{TokenizerUtils.GetEscapeSequence(actual)}'"
        );
    }
}

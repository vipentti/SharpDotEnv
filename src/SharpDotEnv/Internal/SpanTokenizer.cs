// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

namespace SharpDotEnv.Internal;

using System;
using System.Diagnostics;
using static SharpDotEnv.Internal.TokenizerUtils;

internal readonly ref struct SpanTokenizer
{
    private readonly ReadOnlySpan<char> _input;
    private readonly TokenizerState _state;

    public SpanTokenizer(string input, bool skipComments = false, bool skipWhitespace = false)
        : this(input.AsSpan(), skipComments, skipWhitespace) { }

    public SpanTokenizer(
        ReadOnlySpan<char> input,
        bool skipComments = false,
        bool skipWhitespace = false
    )
    {
        _input = input;
        _state = new(skipComments, skipWhitespace);
    }

    public readonly char Current => Peek();

    public readonly bool IsDone => _state.Position >= _input.Length;

    public bool MoveNext(out SpanToken token)
    {
        while (MoveNextImpl(out token))
        {
            if (_state.SkipComments && token.IsComment())
            {
                continue;
            }

            if (_state.SkipWhitespace && token.IsWhitespace())
            {
                continue;
            }

            break;
        }

        return token.Type != TokenType.Eof;
    }

    public readonly char Peek()
    {
        return _state.Position < _input.Length ? _input[_state.Position] : NullChar;
    }

    public char Read()
    {
        var ch = Peek();
        _state.AdvancePosition();
        return ch;
    }

    private bool MoveNextImpl(out SpanToken token)
    {
        ResetStart();

        var result = Peek() switch
        {
            '#' => LexComment(),
            var _ when _state.LexMode == LexMode.Value => LexValue(),
            var it when _state.LexMode == LexMode.Key && IsValidKeyChar(it) => LexKey(),
            var it when char.IsWhiteSpace(it) => LexWhitespace(),
            '=' => LexEquals(),
            NullChar => default,
            var it => throw _state.CreateNotSupportedCharacterException(it),
        };

        token = result;
        return token.Type != TokenType.Eof;
    }

    private readonly SpanToken LexEquals()
    {
        Bump();
        _state.LexMode = LexMode.Value;
        return Emit(TokenType.Equals);
    }

    private SpanToken LexValue()
    {
        var result = Peek() switch
        {
            '\'' => LexQuotedValue(TokenType.SingleQuoteValue, '\''),
            '"' => LexQuotedValue(TokenType.DoubleQuoteValue, '"'),
            '`' => LexQuotedValue(TokenType.BacktickValue, '`'),
            _ => LexRawValue(),
        };

        if (result.Type != TokenType.Whitespace)
        {
            _state.LexMode = LexMode.Key;
        }

        return result;
    }

    private SpanToken LexQuotedValue(TokenType type, char ch)
    {
        // consume the opening character
        EatAssert(ch);
        ResetStart();

        AcceptRun(it => it != ch);

        // Emit the token without the closing and opening characters
        var token = Emit(type);

        // consume the closing character
        EatAssert(ch);

        return token;
    }

    private SpanToken LexRawValue()
    {
        // AcceptRun(ch => IsNotEol(ch) && ch != '#');
        // leading whitespace is returned first
        // trailing whitespace is returned after
        // but we need to buffer until we have read to end
        var foundStartOfValue = false;

        ResetStart();

        var start = _state.Position;

        while (Peek() is char ch && !IsNul(ch))
        {
            if (IsEol(ch) || ch == '#')
            {
                break;
            }
            else if (char.IsWhiteSpace(ch) && !foundStartOfValue)
            {
                return LexWhitespace();
            }
            else
            {
                foundStartOfValue = true;
                _state.Position++;
            }
        }

        if (_state.Position > start)
        {
            var lastCharIndex = LastIndexOf(
                it => !char.IsWhiteSpace(it),
                start,
                _state.Position - 1
            );

            _state.Position = start;

            Debug.Assert(lastCharIndex > -1, "Expected at least one character");

            for (var i = start; i <= lastCharIndex; ++i)
            {
                Bump();
            }
        }

        return Emit(TokenType.Value);
    }

    private readonly int LastIndexOf(Func<char, bool> pred, int start = 0, int? end = null)
    {
        end ??= _input.Length - 1;
        for (var i = end.Value; i >= start; --i)
        {
            if (pred(_input[i]))
            {
                return i;
            }
        }

        return -1;
    }

    private SpanToken LexKey()
    {
        ResetStart();
        AcceptRun(IsValidKeyChar);
        return Emit(TokenType.Key);
    }

    private SpanToken LexComment()
    {
        AcceptRun(IsNotEol);
        return Emit(TokenType.Comment);
    }

    private SpanToken LexWhitespace()
    {
        AcceptRun(char.IsWhiteSpace);
        return Emit(TokenType.Whitespace);
    }

    private void EatAssert(char ch)
    {
        if (!IsAt(ch))
        {
            _state.ThrowExpectedToConsumeException(ch, Current);
        }
        Bump();
    }

    private readonly SpanToken Emit(TokenType type)
    {
        var token = new SpanToken(type, _input.Slice(_state.Start, _state.Position - _state.Start));
        ResetStart();
        return token;
    }

    private readonly void Bump()
    {
        CheckStep();
        _state.BumpPositionAndColumn();

        if (Peek() == '\n')
        {
            BumpLine();
        }
    }

    private readonly void BumpLine()
    {
        _state.BumpLine();
    }

    private readonly void ResetStart()
    {
        _state.ResetStart();
    }

    private void AcceptRun(Func<char, bool> pred)
    {
        while (Peek() is char ch && ch != NullChar && pred(ch))
        {
            Bump();
        }
    }

    private readonly bool IsAt(char ch)
    {
        CheckStep();
        return Current == ch;
    }

    private readonly void CheckStep()
    {
        _state.CheckStep(Current);
    }
}

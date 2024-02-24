// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

namespace SharpDotEnv.Internal;

using System;
using System.Diagnostics;
using static SharpDotEnv.Internal.TokenizerUtils;

internal ref struct SpanTokenizer
{
    public readonly ReadOnlySpan<char> _input;
    private LexMode _lexMode;
    private int _position;
    private int _line;
    private int _column;
    private int _steps;
    private int _start;
    private readonly bool _skipComments;
    private readonly bool _skipWhitespace;

    public SpanTokenizer(string input, bool skipComments = false, bool skipWhitespace = false)
        : this(input.AsSpan(), skipComments, skipWhitespace) { }

    public SpanTokenizer(
        ReadOnlySpan<char> input,
        bool skipComments = false,
        bool skipWhitespace = false
    )
    {
        _position = 0;
        _line = 0;
        _column = 0;
        _steps = 0;
        _start = 0;
        _lexMode = LexMode.Key;
        _input = input;
        _skipComments = skipComments;
        _skipWhitespace = skipWhitespace;
    }

    public readonly char Current => Peek();

    public readonly bool IsDone => _position >= _input.Length;

    public bool MoveNext(out SpanToken token)
    {
        while (MoveNextImpl(out token))
        {
            if (_skipComments && token.IsComment())
            {
                continue;
            }

            if (_skipWhitespace && token.IsWhitespace())
            {
                continue;
            }

            break;
        }

        return token.Type != TokenType.Eof;
    }

    public readonly char Peek()
    {
        return _position < _input.Length ? _input[_position] : NullChar;
    }

    public char Read()
    {
        var ch = Peek();
        _position++;
        return ch;
    }

    private bool MoveNextImpl(out SpanToken token)
    {
        ResetStart();

        var result = Peek() switch
        {
            '#' => LexComment(),
            var _ when _lexMode == LexMode.Value => LexValue(),
            var it when _lexMode == LexMode.Key && IsValidKeyChar(it) => LexKey(),
            var it when char.IsWhiteSpace(it) => LexWhitespace(),
            '=' => LexEquals(),
            NullChar => default,
            var it
                => throw new NotImplementedException(
                    $"Char: '{it}' at {_line}:{_column} {_position}"
                )
        };

        token = result;
        return token.Type != TokenType.Eof;
    }

    private SpanToken LexEquals()
    {
        Bump();
        _lexMode = LexMode.Value;
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
            _lexMode = LexMode.Key;
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

        int start = _position;

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
                _position++;
            }
        }

        if (_position > start)
        {
            int lastCharIndex = LastIndexOf(it => !char.IsWhiteSpace(it), start, _position - 1);

            _position = start;

            Debug.Assert(lastCharIndex > -1, "Expected at least one character");

            for (int i = start; i <= lastCharIndex; ++i)
            {
                Bump();
            }
        }

        return Emit(TokenType.Value);
    }

    private readonly int LastIndexOf(Func<char, bool> pred, int start = 0, int? end = null)
    {
        end ??= _input.Length - 1;
        for (int i = end.Value; i >= start; --i)
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
        var token = Emit(TokenType.Comment);
        return token;
    }

    private SpanToken LexWhitespace()
    {
        AcceptRun(char.IsWhiteSpace);
        return Emit(TokenType.Whitespace);
    }

    private bool TryEat(char ch)
    {
        if (IsAt(ch))
        {
            Bump();
            return true;
        }

        return false;
    }

    private void EatAssert(char ch)
    {
        Debug.Assert(
            IsAt(ch),
            $"Expected to consume '{GetEscapeSequence(ch)}' at {_line}:{_column} pos: {_position} but found '{GetEscapeSequence(Current)}'"
        );
        Bump();
    }

    private SpanToken Emit(TokenType type)
    {
        var token = new SpanToken(type, _input.Slice(_start, _position - _start));
        ResetStart();
        return token;
    }

    private void Bump()
    {
        CheckStep();
        _position++;
        _column++;

        if (Peek() == '\n')
        {
            BumpLine();
        }
    }

    private void BumpLine()
    {
        _line++;
        _column = 0;
    }

    private void ResetStart()
    {
        _start = _position;
    }

    private void AcceptRun(Func<char, bool> pred)
    {
        while (Peek() is char ch && ch != NullChar && pred(ch))
        {
            Bump();
        }
    }

    private bool IsAt(char ch)
    {
        CheckStep();
        return Current == ch;
    }

    private void CheckStep()
    {
        _steps++;
        Debug.Assert(_steps < 1_000_000, $"Parser seems stuck at '{Current}'");
    }
}

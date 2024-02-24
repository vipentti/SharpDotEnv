// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using static SharpDotEnv.Internal.TokenizerUtils;

namespace SharpDotEnv.Internal;

internal class StreamTokenizer
{
    private readonly StreamReader _stream;
    private readonly StringBuilder _currentValueBuilder;
    private readonly IndexedQueue<char> _buffer;
    private readonly TokenizerState _state;

    public StreamTokenizer(
        StreamReader stream,
        bool skipComments = false,
        bool skipWhitespace = false
    )
    {
        _currentValueBuilder = new StringBuilder();
        _buffer = new IndexedQueue<char>();
        _stream = stream;
        _state = new(skipComments, skipWhitespace);
    }

    public bool IsDone => _stream.EndOfStream;

    public char Current => Peek();

    public char Peek()
    {
        if (_buffer.TryPeek(out var read))
        {
            return read;
        }

        return StreamPeek();
    }

    private char StreamPeek()
    {
        var next = _stream.Peek();
        return next != -1 ? (char)next : NullChar;
    }

    private char StreamRead()
    {
        var next = _stream.Read();
        return next != -1 ? (char)next : NullChar;
    }

    private char Next()
    {
        if (_buffer.TryDequeue(out var result))
        {
            return result;
        }

        return StreamRead();
    }

    public bool MoveNext(out StreamToken token)
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

    private bool MoveNextImpl(out StreamToken token)
    {
        ResetStart();

        token = default;

        switch (Peek())
        {
            case '#':
                token = LexComment();
                break;
            case var _ when _state.LexMode == LexMode.Value:
                token = LexValue();
                break;
            case var it when _state.LexMode == LexMode.Key && IsValidKeyChar(it):
                token = LexKey();
                break;
            case var it when char.IsWhiteSpace(it):
                token = LexWhitespace();
                break;
            case '=':
                token = LexEquals();
                break;
            case NullChar:
                break;
            default:
                _state.ThrowNotSupportedCharacterException(Peek());
                break;
        }

        return token.Type != TokenType.Eof;
    }

    private StreamToken LexEquals()
    {
        Bump();
        _state.LexMode = LexMode.Value;
        return Emit(TokenType.Equals);
    }

    private StreamToken LexValue()
    {
        StreamToken result;

        switch (Peek())
        {
            case '\'':
                result = LexQuotedValue(TokenType.SingleQuoteValue, '\'');
                break;
            case '"':
                result = LexQuotedValue(TokenType.DoubleQuoteValue, '"');
                break;
            case '`':
                result = LexQuotedValue(TokenType.BacktickValue, '`');
                break;
            default:
                result = LexRawValue();
                break;
        }

        if (result.Type != TokenType.Whitespace)
        {
            _state.LexMode = LexMode.Key;
        }

        return result;
    }

    private StreamToken LexQuotedValue(TokenType type, char ch)
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

    private StreamToken LexRawValue()
    {
        // leading whitespace is returned first
        // trailing whitespace is returned after
        // but we need to buffer until we have read to end
        var foundStartOfValue = false;

        _buffer.Clear();

        while (StreamPeek() is char ch && !IsNul(ch))
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
                _buffer.Enqueue(StreamRead());
            }
        }

        if (_buffer.Count > 0)
        {
            int lastCharIndex = _buffer.LastIndexOf(it => !char.IsWhiteSpace(it));

            Debug.Assert(lastCharIndex > -1, "Expected at least one character");

            // Consume all non ws characters
            for (int i = 0; i <= lastCharIndex; ++i)
            {
                Bump();
            }
        }

        return Emit(TokenType.Value);
    }

    private StreamToken LexKey()
    {
        ResetStart();
        AcceptRun(IsValidKeyChar);
        return Emit(TokenType.Key);
    }

    private StreamToken LexComment()
    {
        AcceptRun(IsNotEol);
        var token = Emit(TokenType.Comment);
        return token;
    }

    private StreamToken LexWhitespace()
    {
        AcceptRun(char.IsWhiteSpace);
        return Emit(TokenType.Whitespace);
    }

    private StreamToken Emit(TokenType type)
    {
        var token = new StreamToken(type, _currentValueBuilder.ToString());
        ResetStart();
        return token;
    }

    private void ResetStart()
    {
        _currentValueBuilder.Clear();
    }

    private void CheckStep()
    {
        _state.CheckStep(Current);
    }

    private void EatAssert(char ch)
    {
        if (!IsAt(ch))
        {
            _state.ThrowExpectedToConsumeException(ch, Current);
        }
        Bump();
    }

    private void Bump()
    {
        CheckStep();

        var next = Next();

        if (next != NullChar)
        {
            _currentValueBuilder.Append(next);
        }

        _state.BumpPositionAndColumn();

        if (Peek() == '\n')
        {
            BumpLine();
        }
    }

    private void BumpLine()
    {
        _state.BumpLine();
    }

    private void AcceptRun(Func<char, bool> pred)
    {
        while (Peek() is char ch && !IsNul(ch) && pred(ch))
        {
            Bump();
        }
    }

    private bool IsAt(char ch)
    {
        CheckStep();
        return Current == ch;
    }
}

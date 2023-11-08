using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using static SharpDotEnv.Internal.TokenizerUtils;

namespace SharpDotEnv.Internal
{
    internal struct StreamTokenizer
    {
        private readonly StreamReader _stream;
        private readonly StringBuilder _currentValueBuilder;
        private readonly IndexedQueue<char> _buffer;
        private LexMode _lexMode;
        private int _position;
        private int _line;
        private int _column;
        private int _steps;
        private readonly bool _skipComments;
        private readonly bool _skipWhitespace;

        public StreamTokenizer(
            StreamReader stream,
            bool skipComments = false,
            bool skipWhitespace = false
        )
        {
            _currentValueBuilder = new StringBuilder();
            _buffer = new IndexedQueue<char>();
            _position = 0;
            _line = 0;
            _column = 0;
            _steps = 0;
            _lexMode = LexMode.Key;
            _stream = stream;
            _skipComments = skipComments;
            _skipWhitespace = skipWhitespace;
        }

        public readonly bool IsDone => _stream.EndOfStream;

        public readonly char Current => Peek();

        public readonly char Peek()
        {
            if (_buffer.TryPeek(out var read))
            {
                return read;
            }

            return StreamPeek();
        }

        private readonly char StreamPeek()
        {
            var next = _stream.Peek();
            return next != -1 ? (char)next : NullChar;
        }

        private readonly char StreamRead()
        {
            var next = _stream.Read();
            return next != -1 ? (char)next : NullChar;
        }

        private readonly char Next()
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

        private bool MoveNextImpl(out StreamToken token)
        {
            ResetStart();

            token = default;

            switch (Peek())
            {
                case '#':
                    token = LexComment();
                    break;
                case var _ when _lexMode == LexMode.Value:
                    token = LexValue();
                    break;
                case var it when _lexMode == LexMode.Key && IsValidKeyChar(it):
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
                    throw new NotImplementedException(
                        $"Char: '{Peek()}' at {_line}:{_column} {_position}"
                    );
            }

            return token.Type != TokenType.Eof;
        }

        private StreamToken LexEquals()
        {
            Bump();
            _lexMode = LexMode.Value;
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
                _lexMode = LexMode.Key;
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

        private readonly StreamToken Emit(TokenType type)
        {
            var token = new StreamToken(type, _currentValueBuilder.ToString());
            ResetStart();
            return token;
        }

        private readonly void ResetStart()
        {
            _currentValueBuilder.Clear();
        }

        private void CheckStep()
        {
            _steps++;
            Debug.Assert(_steps < 1_000_000, $"Parser seems stuck at '{Current}'");
        }

        private void EatAssert(char ch)
        {
            Debug.Assert(
                IsAt(ch),
                $"Expected to consume '{GetEscapeSequence(ch)}' at {_line}:{_column} pos: {_position} but found '{GetEscapeSequence(Current)}'"
            );
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
}

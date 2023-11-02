﻿using SharpDotEnv.Exceptions;
using System.IO;
using System.Text;

namespace SharpDotEnv.Internal
{
    internal static class Parser
    {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        internal static readonly bool SupportSpanTokens = true;
#else
        internal static readonly bool SupportSpanTokens = false;
#endif

        public static DotEnv ParseEnvironment(string input)
        {
            var variables = new DotEnv();

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            ParseEnvironment(variables, input);
#else
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            {
                ParseEnvironment(variables, stream, Encoding.UTF8);
            }
#endif
            return variables;
        }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public static DotEnv ParseEnvironment(Stream input, Encoding? encoding = default)
#else
        public static DotEnv ParseEnvironment(Stream input, Encoding encoding = default)
#endif
        {
            var variables = new DotEnv();
            ParseEnvironment(variables, input, encoding);
            return variables;
        }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        private static void ParseEnvironment(DotEnv variables, string input)
        {
            var tokenizer = new SpanTokenizer(input, skipComments: true, skipWhitespace: true);

            while (tokenizer.MoveNext(out var keyToken))
            {
                _ = tokenizer.MoveNext(out var valueToken);
                AddToEnvironment(variables, keyToken.ToStreamToken(), valueToken.ToStreamToken());
            }
        }
#endif

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        private static void ParseEnvironment(DotEnv variables, Stream input, Encoding? encoding = default)
#else
        private static void ParseEnvironment(DotEnv variables, Stream input, Encoding encoding = default)
#endif
        {
            using (var streamReader = new StreamReader(input, encoding ?? Encoding.UTF8))
            {
                var tokenizer = new StreamTokenizer(streamReader, skipComments: true, skipWhitespace: true);

                while (tokenizer.MoveNext(out var keyToken))
                {
                    _ = tokenizer.MoveNext(out var valueToken);
                    AddToEnvironment(variables, keyToken, valueToken);
                }
            }
        }

        private static void AddToEnvironment(DotEnv variables, StreamToken keyToken, StreamToken valueToken)
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

            var value = valueToken.Value;

            if (valueToken.Type == TokenType.DoubleQuoteValue)
            {
                value = value.Replace("\\n", "\n");
            }

            variables[key] = value;
        }
    }
}
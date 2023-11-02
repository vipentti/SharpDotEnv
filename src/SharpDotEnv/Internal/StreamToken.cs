namespace SharpDotEnv.Internal
{
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
}

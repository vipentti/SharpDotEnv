// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System.Globalization;

namespace SharpDotEnv.Internal;

internal static class TokenizerUtils
{
    public const char NullChar = '\0';

    public static bool IsValidKeyChar(char ch) => !char.IsWhiteSpace(ch) && ch != '=' && !IsNul(ch);

    public static bool IsEol(char ch) => ch == '\r' || ch == '\n';

    public static bool IsNul(char ch) => ch == NullChar;

    public static bool IsNotEol(char ch) => !IsEol(ch);

    public static string GetEscapeSequence(char c) => "\\u" + ((int)c).ToString("X4", CultureInfo.InvariantCulture);
}

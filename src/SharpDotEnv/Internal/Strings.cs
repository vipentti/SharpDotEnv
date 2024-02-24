// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace SharpDotEnv.Internal;

[SuppressMessage(
    "Design",
    "CA1863:Cache a 'CompositeFormat' for repeated use in this formatting operation",
    Justification = "Not all current TargetFrameworks support it."
)]
internal static class Strings
{
    public const string Error_ExpectedKey = "Expected key. Got '{0}'";
    public const string Error_ExpectedValueForKey = "Expected value for key: '{0}'";
    public const string Error_InvalidValue = "Expected valid value for '{0}'. Got '{1}'";

    public static string FormatError_ExpectedKey(object arg) =>
        string.Format(CultureInfo.InvariantCulture, Error_ExpectedKey, arg);

    public static string FormatError_ExpectedValueForKey(object arg) =>
        string.Format(CultureInfo.InvariantCulture, Error_ExpectedValueForKey, arg);

    public static string FormatError_InvalidValue(object arg1, object arg2) =>
        string.Format(CultureInfo.InvariantCulture, Error_InvalidValue, arg1, arg2);
}

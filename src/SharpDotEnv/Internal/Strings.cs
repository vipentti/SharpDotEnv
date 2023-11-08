namespace SharpDotEnv.Internal
{
    internal static class Strings
    {
        public const string Error_ExpectedKey = "Expected key. Got '{0}'";
        public const string Error_ExpectedValueForKey = "Expected value for key: '{0}'";
        public const string Error_InvalidValue = "Expected valid value for '{0}'. Got '{1}'";

        public static string FormatError_ExpectedKey(object arg) =>
            string.Format(Error_ExpectedKey, arg);

        public static string FormatError_ExpectedValueForKey(object arg) =>
            string.Format(Error_ExpectedKey, arg);

        public static string FormatError_InvalidValue(object arg1, object arg2) =>
            string.Format(Error_ExpectedKey, arg1, arg2);
    }
}

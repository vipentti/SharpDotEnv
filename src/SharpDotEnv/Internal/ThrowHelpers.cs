// Copyright 2023-2024 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/SharpDotEnv/blob/main/LICENSE.md

using System;
using System.Runtime.CompilerServices;
#if NETCOREAPP3_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace SharpDotEnv.Internal
{
    internal static class ThrowHelpers
    {
        public static void ThrowIfNull(
    #if NETCOREAPP3_0_OR_GREATER
            [NotNull]
    #endif
            object? argument,
            [CallerArgumentExpression(nameof(argument))]
            string? paramName = default)
        {
            if (argument is null)
            {
                ThrowNullException(paramName);
            }
        }

    #if NETCOREAPP3_0_OR_GREATER
        [DoesNotReturn]
    #endif
        private static void ThrowNullException(string? paramName)
            => throw new ArgumentNullException(paramName);
    }
}

#if !NETCOREAPP3_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute
    {
        public CallerArgumentExpressionAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }

        public string ParameterName { get; }
    }
}
#endif

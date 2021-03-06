#region License

// Copyright (c) K2 Workflow (SourceCode Technology Holdings Inc.). All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

#endregion

using System;
using System.Runtime.CompilerServices;

namespace SourceCode.Clay
{
    /// <summary>
    /// Utility functions for <see cref="System.String"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns the specified number of characters from the left of a string.
        /// Tolerates <paramref name="length"/> values that are too large or too small (or negative).
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="length">The length.</param>
        public static string Left(this string str, int length)
        {
            if (string.IsNullOrEmpty(str) || length >= str.Length) return str;
            if (length <= 1)
            {
                if (length == 1) return char.ToString(str[0]);
                return string.Empty;
            }

            // Per existing Substring behavior, we don't respect surrogate pairs
            return str.Substring(0, length);
        }

        /// <summary>
        /// Returns the specified number of characters from the right of a string.
        /// Tolerates <paramref name="length"/> values that are too large or too small (or negative).
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="length">The length.</param>
        public static string Right(this string str, int length)
        {
            if (string.IsNullOrEmpty(str) || length >= str.Length) return str;
            if (length <= 1)
            {
                if (length == 1) return char.ToString(str[str.Length - 1]);
                return string.Empty;
            }

            // Per existing Substring behavior, we don't respect surrogate pairs
            return str.Substring(str.Length - length);
        }

        /// <summary>
        /// Truncates a string to a specified width, respecting surrogate pairs and inserting
        /// an ellipsis '…' in the final position.
        /// Tolerates width values that are too large, too small or negative.
        /// If the value is already smaller than the specified width, the original value is returned.
        /// If the character at the elided boundary is in a surrogate pair then the pair is treated atomically.
        /// In this case the result may be shorter than specified.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="totalWidth">The total width.</param>
        /// <returns>The elided string.</returns>
        public static string Elide(this string str, int totalWidth)
        {
            // No need to elide if string is already within the required size
            if (str is null || totalWidth <= 2 || str.Length <= totalWidth)
                return str;

            // Since Elide is expected to be used primarily for display purposes, it needs to respect
            // surrogate pairs and not blindly split them in half.
            // https://stackoverflow.com/questions/2241348/what-is-unicode-utf-8-utf-16
            // https://stackoverflow.com/questions/14347799/how-do-i-create-a-string-with-a-surrogate-pair-inside-of-it

            // Expect non-surrogates by default
            int len = totalWidth;

            // High | Low (default on x86/x64)
            if (BitConverter.IsLittleEndian)
            {
                // If target is the LOW surrogate, replace both (returning a shorter-by-1-than-expected result)
                if (char.IsLowSurrogate(str[len - 1]))
                    len--;

                // If it's the HIGH surrogate, we're replacing it regardless
            }

            // Low | High
            else
            {
                // If target is the HIGH surrogate, replace both (returning a shorter-by-1-than-expected result)
                if (char.IsHighSurrogate(str[len - 1]))
                    len--;

                // If it's the LOW surrogate, we're replacing it regardless
            }

#if !NETSTANDARD2_0
            // Write directly into the string’s memory on the heap, thus avoiding any intermediate allocations
            string sb = string.Create(len, len - 1, (dst, last) => // Say string has len=4, => last=3
            {
                str.AsSpan(0, last).CopyTo(dst); // Only copy first 3 chars: [0, 1, 2]
                dst[last] = '…'; // Explicitly set 4th char: [3]
            });
#else
            string sb = str.Substring(0, len - 1) + '…';
#endif
            return sb;
        }

        /// <summary>
        /// Compares two strings using ordinal rules, with checks for null and reference equality.
        /// Similar to the framework-provided <see cref="string.CompareOrdinal(string, string)"/> method.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsOrdinal(this string x, string y)
            => StringComparer.Ordinal.Equals(x, y);

        /// <summary>
        /// Removes the specified prefix if it is found at the start of the string.
        /// Uses <see cref="StringComparison.Ordinal"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="prefix">The prefix to remove.</param>
        public static string RemoveStart(this string str, string prefix)
            => RemoveStart(str, prefix, StringComparison.Ordinal);

        /// <summary>
        /// Removes the specified prefix if it is found at the start of the string.
        /// Uses the specified <see cref="StringComparison"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="prefix">The prefix to remove.</param>
        /// <param name="comparisonType">The string comparison method to use.</param>
        public static string RemoveStart(this string str, string prefix, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(str)
                || string.IsNullOrEmpty(prefix))
                return str;

            int len = str.Length - prefix.Length;
            if (len < 0)
                return str;

            if (!str.StartsWith(prefix, comparisonType))
                return str;

            return str.Substring(prefix.Length, len);
        }

        /// <summary>
        /// Removes the specified suffix if it is found at the end of the string.
        /// Uses <see cref="StringComparison.Ordinal"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="suffix">The suffix to remove.</param>
        public static string RemoveEnd(this string str, string suffix)
            => RemoveEnd(str, suffix, StringComparison.Ordinal);

        /// <summary>
        /// Removes the specified suffix if it is found at the end of the string.
        /// Uses the specified <see cref="StringComparison"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="suffix">The suffix to remove.</param>
        /// <param name="comparisonType">The string comparison method to use.</param>
        public static string RemoveEnd(this string str, string suffix, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(str)
                || string.IsNullOrEmpty(suffix))
                return str;

            int len = str.Length - suffix.Length;
            if (len < 0)
                return str;

            if (!str.EndsWith(suffix, comparisonType))
                return str;

            return str.Substring(0, len);
        }
    }
}

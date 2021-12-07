using System;
using System.Text;
using System.Text.RegularExpressions;
using Float.Core.Exceptions;

namespace Float.Core.Extensions
{
    /// <summary>
    /// Extensions on the string primitive type.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Convert this string to a base64 encoded string.
        /// </summary>
        /// <returns>The encoded string.</returns>
        /// <param name="value">This string object.</param>
        public static string ToBase64(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidStringArgumentException(nameof(value));
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// Convert this string to base64 encoded bytes.
        /// </summary>
        /// <returns>The encoded bytes.</returns>
        /// <param name="value">This string object.</param>
        public static byte[] ToBase64Bytes(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidStringArgumentException(nameof(value));
            }

            return Encoding.UTF8.GetBytes(value);
        }

        /// <summary>
        /// Convert this base64 encoded string to a decoded string.
        /// </summary>
        /// <returns>The decoded string.</returns>
        /// <param name="value">This string object.</param>
        public static string FromBase64(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidStringArgumentException(nameof(value));
            }

            return Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }

        /// <summary>
        /// Convert this string to a base64 URL encoded string per https://tools.ietf.org/html/rfc4648#page-7.
        /// </summary>
        /// <returns>The encoded string.</returns>
        /// <param name="value">This string object.</param>
#pragma warning disable CA1055 // URI-like return values should not be strings
        public static string ToUrlEncodedBase64(this string value)
#pragma warning restore CA1055 // URI-like return values should not be strings
        {
            return value?.ToBase64().Replace('+', '-').Replace('_', '/').Replace("=", string.Empty);
        }

        /// <summary>
        /// Convert a base64 encoded string to a decoded string.
        /// </summary>
        /// <returns>The decoded string.</returns>
        /// <param name="value">This string object.</param>
#pragma warning disable CA1055 // URI-like return values should not be strings
        public static string FromUrlEncodedBase64(this string value)
#pragma warning restore CA1055 // URI-like return values should not be strings
        {
            return value?.Replace('+', '-').Replace('/', '_').PadRightToMultiple(3, '=').FromBase64();
        }

        /// <summary>
        /// Pad this string on the right side until its length is a multiple of the given value.
        /// If this string's length is already divisible by the given value, this returns this string.
        /// </summary>
        /// <returns>The padded string.</returns>
        /// <param name="value">This string object.</param>
        /// <param name="multiple">The desired multiple by which the resultant string's length should be divisible.</param>
        /// <param name="paddingChar">The character with which to pad.</param>
        public static string PadRightToMultiple(this string value, int multiple, char paddingChar)
        {
            if (multiple < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(multiple), "Multiple must be greater than zero.");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidStringArgumentException(nameof(value));
            }

            if (value.Length % multiple == 0)
            {
                return value;
            }

            return value.PadRight((value.Length / multiple * multiple) + multiple, paddingChar);
        }

        /// <summary>
        /// Returns this string trimmed to the first occurance of the given character.
        /// For example, "go with float".TrimmedToFirstOccuranceOf('w') -> "go w".
        /// </summary>
        /// <returns>This string to the first occurance of the given character.</returns>
        /// <param name="thisString">This string.</param>
        /// <param name="value">The character for which to get the first occurance.</param>
        public static string TrimmedToFirstOccuranceOf(this string thisString, char value)
        {
            if (string.IsNullOrEmpty(thisString))
            {
                return thisString;
            }

            var index = thisString.IndexOf(value);

            if (index > 0)
            {
                return thisString.Substring(0, index);
            }

            return thisString;
        }

        /// <summary>
        /// Returns this string with the html tags removed.
        /// </summary>
        /// <returns>This string to the first occurance of the given character.</returns>
        /// <param name="value">The string you want to remove html tags from.</param>
        public static string RemoveHTMLFromString(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return Regex.Replace(value.Replace("\n", string.Empty), "<.*?>|<!--[^\\[](.*?)-->", string.Empty).Trim();
        }

        /// <summary>
        /// Returns null if this string is null, empty, or whitespace. Otherwise, returns the given string.
        /// </summary>
        /// <param name="str">This string.</param>
        /// <returns>Null if the string is null, empty, or whitespace; the given string otherwise.</returns>
        public static string NullIfWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? null : str;
        }
    }
}

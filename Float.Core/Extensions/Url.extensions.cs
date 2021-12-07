using System;
using Float.Core.Exceptions;

namespace Float.Core.Extensions
{
    /// <summary>
    /// Extensions on the Url/Uri/String path types.
    /// </summary>
    public static class UrlExtensions
    {
        /// <summary>
        /// Generates a regex pattern given a URL String.
        /// </summary>
        /// <returns>A Regex Pattern string for matching.</returns>
        /// <param name="urlString">This string.</param>
        public static string PatternForMatchingEitherHost(this string urlString)
        {
            if (string.IsNullOrWhiteSpace(urlString))
            {
                throw new InvalidStringArgumentException(nameof(urlString));
            }

            return new Uri(urlString).PatternForMatchingEitherHost();
        }

        /// <summary>
        /// Generates a regex pattern given a URI.
        /// </summary>
        /// <returns>A Regex Pattern string for matching.</returns>
        /// <param name="uri">This uri.</param>
        public static string PatternForMatchingEitherHost(this Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            // We have to strip the host www tld out (if it exists)
            // This is so we can match it in either scenario.
            // We've also decided to trim a trailing "/" in order to make it as flexible as possible.
            return $"^((http[s]?):\\/)?\\/?(www.)?({uri.Host.TrimStart("www.".ToCharArray())}{uri.PathAndQuery.TrimEnd('/')})";
        }
    }
}

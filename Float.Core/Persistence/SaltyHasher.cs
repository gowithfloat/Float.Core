using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Float.Core.Exceptions;
using Float.Core.Extensions;

namespace Float.Core.Persistence
{
    /// <summary>
    /// Class to create a SHA256 salted hash.
    /// </summary>
    public static class SaltyHasher
    {
        /// <summary>
        /// Generates a SHA256 hash.
        /// </summary>
        /// <returns>The hash or the combined salt and secret.</returns>
        /// <param name="salt">String to append to the secret.</param>
        /// <param name="secret">The secret.</param>
        public static string GenerateHash(string salt, string secret)
        {
            if (string.IsNullOrEmpty(salt))
            {
                throw new InvalidStringArgumentException(nameof(salt));
            }

            if (string.IsNullOrEmpty(secret))
            {
                throw new InvalidStringArgumentException(nameof(secret));
            }

            using var sha = new SHA256Managed();

            return sha.ComputeHash((secret + salt).ToBase64Bytes())
                .Aggregate(new StringBuilder(), (sb, bytes) => sb.Append(bytes.ToString(CultureInfo.InvariantCulture)))
                .ToString();
        }
    }
}

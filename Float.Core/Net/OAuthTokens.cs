using System;
using Float.Core.Exceptions;

namespace Float.Core.Net
{
    /// <summary>
    /// Class containing OAuth tokens and helper methods for those tokens.
    /// </summary>
    public sealed class OAuthTokens
    {
        const int ExpireSoonThresholdSeconds = 60 * 10; // 10 minutes

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthTokens"/> class.
        /// </summary>
        /// <param name="accessToken">Access token.</param>
        /// <param name="refreshToken">Refresh token.</param>
        /// <param name="durationSeconds">Duration the the token is valid for.</param>
        /// <param name="created">The timestamp this token was created.</param>
        public OAuthTokens(string accessToken, string refreshToken, int durationSeconds, DateTime? created = null)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new InvalidStringArgumentException(nameof(accessToken));
            }

            Created = created ?? DateTime.Now;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            DurationSeconds = durationSeconds;
        }

        /// <summary>
        /// Gets the date this token was created.
        /// </summary>
        /// <value>The date the token was created.</value>
        public DateTime Created { get; }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <value>The access token.</value>
        public string AccessToken { get; }

        /// <summary>
        /// Gets the refresh token.
        /// </summary>
        /// <value>The refresh token.</value>
        public string RefreshToken { get; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public int DurationSeconds { get; }

        /// <summary>
        /// Gets the date that this token expires.
        /// </summary>
        /// <value>The expiration date.</value>
        public DateTime Expires => Created.AddSeconds(DurationSeconds);

        /// <summary>
        /// Gets a value indicating whether the access token is expiring soon, and therefore if it should be refreshed.
        /// </summary>
        /// <value><c>true</c>, if token should be refreshed, <c>false</c> otherwise.</value>
        public bool ShouldRefresh => DateTime.Now.AddSeconds(ExpireSoonThresholdSeconds) > Expires;

        /// <summary>
        /// Returns a <see cref="string"/> that represents the current <see cref="OAuthTokens"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the current <see cref="OAuthTokens"/>.</returns>
        public override string ToString()
        {
            return $"[{GetType().Name}: {nameof(AccessToken)}: {AccessToken}, {nameof(RefreshToken)}: {RefreshToken}, {nameof(DurationSeconds)}: {DurationSeconds}]";
        }
    }
}

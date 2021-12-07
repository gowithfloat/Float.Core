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
        readonly DateTime timeCreated;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthTokens"/> class.
        /// </summary>
        /// <param name="accessToken">Access token.</param>
        /// <param name="refreshToken">Refresh token.</param>
        /// <param name="durationSeconds">Duration the the token is valid for.</param>
        public OAuthTokens(string accessToken, string refreshToken, int durationSeconds)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new InvalidStringArgumentException(nameof(accessToken));
            }

            timeCreated = DateTime.Now;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            DurationSeconds = durationSeconds;
        }

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
        /// Gets a value indicating whether the access token is expiring soon, and therefore if it should be refreshed.
        /// </summary>
        /// <value><c>true</c>, if token should be refreshed, <c>false</c> otherwise.</value>
        public bool ShouldRefresh => (DateTime.Now - timeCreated).Seconds >= DurationSeconds - ExpireSoonThresholdSeconds;

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

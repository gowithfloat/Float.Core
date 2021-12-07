namespace Float.Core.Net
{
    /// <summary>
    /// Grant type.
    /// </summary>
    public enum OAuth2GrantType
    {
        /// <summary>
        /// The password grant type.
        /// </summary>
        Password,

        /// <summary>
        /// The client_credentials grant type.
        /// </summary>
        ClientCredentials,

        /// <summary>
        /// The authorization code grant type.
        /// </summary>
        AuthorizationCode,
    }
}

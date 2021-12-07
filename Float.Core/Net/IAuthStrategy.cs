using System.Net.Http;
using System.Threading.Tasks;

namespace Float.Core.Net
{
    /// <summary>
    /// Authentication strategy for authenticating HTTP requests.
    /// </summary>
    public interface IAuthStrategy
    {
        /// <summary>
        /// Gets a value indicating whether the user is authenticated with the server.
        /// </summary>
        /// <remarks>As this property is synchronous, it should not be used if you expect this check to take a noticable duration.</remarks>
        /// <value><c>true</c>, if there is an authenticated user, <c>false</c> otherwise.</value>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Gets a value indicating whether the user is authenticated with the server.
        /// </summary>
        /// <remarks>Use this with an auth strategy whose secure store may block during access.</remarks>
        /// <returns><c>true</c>, if there is an authenticated user, <c>false</c> otherwise.</returns>
        Task<bool> IsAuthenticatedAsync();

        /// <summary>
        /// Adds authentication to a HttpRequestMessage.
        /// </summary>
        /// <returns>A HttpRequestMessage with Authentication added to it.</returns>
        /// <param name="request">The Request.</param>
        Task<HttpRequestMessage> AuthenticateRequest(HttpRequestMessage request);

        /// <summary>
        /// Login the specified username and password.
        /// </summary>
        /// <returns>If the user did successfully login.</returns>
        /// <param name="username">The Username.</param>
        /// <param name="password">The Password.</param>
        Task<bool> Login(string username = null, string password = null);

        /// <summary>
        /// Logout the user. This method should dispose of any derived information used to authenticate.
        /// </summary>
        /// <returns><c>true</c>, if the logout was successful, <c>false</c> otherwise.</returns>
        Task<bool> Logout();
    }
}

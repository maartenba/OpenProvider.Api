// ReSharper disable once CheckNamespace

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace OpenProvider.Api
{
    public partial interface IClient
    {
        /// <summary>Login with username and password, and store the authentication token for future requests with this client.</summary>
        /// <returns>A successful response.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<AuthLoginResponse> LoginAndStoreAsync(AuthLoginRequest body);
        
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Login with username and password, and store the authentication token for future requests with this client.</summary>
        /// <returns>A successful response.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        Task<AuthLoginResponse> LoginAndStoreAsync(AuthLoginRequest body, CancellationToken cancellationToken);
        
        /// <summary>
        /// Logout and get rid of the stored access token.
        /// </summary>
        void Logout();
    }

    public partial class Client : IClient
    {
        private string _accessToken;

        public Client(HttpClient httpClient, string accessToken = null)
            : this(httpClient)
        {
            _accessToken = accessToken;
        }

        partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
        {
            if (_accessToken != null)
            {
                request.Headers.Authorization = AuthenticationHeaderValue.Parse("Bearer " + _accessToken);
            }
        }

        partial void PrepareRequest(HttpClient client, HttpRequestMessage request, System.Text.StringBuilder urlBuilder)
        {
            if (_accessToken != null)
            {
                request.Headers.Authorization = AuthenticationHeaderValue.Parse("Bearer " + _accessToken);
            }
        }

        public Task<AuthLoginResponse> LoginAndStoreAsync(AuthLoginRequest body)
        {
            return LoginAndStoreAsync(body, System.Threading.CancellationToken.None);
        }

        public async Task<AuthLoginResponse> LoginAndStoreAsync(AuthLoginRequest body,
            CancellationToken cancellationToken)
        {
            Logout();

            var result = await LoginAsync(body, cancellationToken);
            if (result.Code == 0 && !string.IsNullOrEmpty(result.Data.Token))
            {
                _accessToken = result.Data.Token;
            }

            return result;
        }

        public void Logout()
        {
            _accessToken = null;
        }
    }
}
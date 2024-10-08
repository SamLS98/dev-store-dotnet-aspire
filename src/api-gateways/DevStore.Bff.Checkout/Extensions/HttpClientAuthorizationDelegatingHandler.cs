using DevStore.WebAPI.Core.User;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace DevStore.Bff.Checkout.Extensions
{
    public class HttpClientAuthorizationDelegatingHandler(IAspNetUser aspNetUser) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationHeader = aspNetUser.GetHttpContext().Request.Headers.Authorization;

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                request.Headers.Add("Authorization", [authorizationHeader]);
            }

            var token = aspNetUser.GetUserToken();

            if (token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
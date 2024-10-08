using DevStore.WebAPI.Core.User;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace DevStore.WebApp.MVC.Services.Handlers
{
    public class HttpClientAuthorizationDelegatingHandler(IAspNetUser user) : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationHeader = user.GetHttpContext().Request.Headers.Authorization;

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                request.Headers.Add("Authorization", [authorizationHeader]);
            }

            var token = user.GetUserToken();

            if (token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
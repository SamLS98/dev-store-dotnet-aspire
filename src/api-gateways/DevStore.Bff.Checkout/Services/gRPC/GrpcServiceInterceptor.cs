using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DevStore.Bff.Checkout.Services.gRPC
{
    public class GrpcServiceInterceptor(IHttpContextAccessor httpContextAccesor) : Interceptor
    {
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var token = httpContextAccesor.HttpContext.Request.Headers.Authorization;

            var headers = new Metadata
            {
                {"Authorization", token}
            };

            var options = context.Options.WithHeaders(headers);
            context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, options);

            return base.AsyncUnaryCall(request, context, continuation);
        }
    }
}
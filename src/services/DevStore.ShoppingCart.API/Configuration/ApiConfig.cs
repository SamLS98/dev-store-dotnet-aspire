using DevStore.ShoppingCart.API.Data;
using DevStore.ShoppingCart.API.Services.gRPC;
using DevStore.WebAPI.Core.Configuration;
using DevStore.WebAPI.Core.DatabaseFlavor;
using static DevStore.WebAPI.Core.DatabaseFlavor.ProviderConfiguration;

namespace DevStore.ShoppingCart.API.Configuration
{
    public static class ApiConfig
    {
        public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpc();

            services.AddCors(options =>
            {
                options.AddPolicy("Total",
                    builder =>
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
            });

            
        }

        public static void UseApiConfiguration(this WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Under certain scenarios, e.g minikube / linux environment / behind load balancer
            // https redirection could lead dev's to over complicated configuration for testing purpouses
            // In production is a good practice to keep it true
            if (app.Configuration["USE_HTTPS_REDIRECTION"] == "true")
                app.UseHttpsRedirection();

            app.UseCors("Total");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGrpcService<ShoppingCartGrpcService>().RequireCors("Total");

            app.MapDefaultEndpoints();
        }
    }
}
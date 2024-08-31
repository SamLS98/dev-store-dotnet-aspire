using DevStore.WebAPI.Core.DatabaseFlavor;
using DevStore.WebAPI.Core.Extensions;
using Serilog;
using static DevStore.WebAPI.Core.DatabaseFlavor.ProviderConfiguration;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Logging.AddSerilog(new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger());

var healthCheckBuilder = builder.Services.AddHealthChecksUI(setupSettings: setup =>
{
    setup.SetHeaderText("DevStore - Status Page");
    string endpoints = builder.Configuration.GetSection("ENDPOINTS").Get<string>()!;

    foreach (var endpoint in endpoints.Split(";"))
    {
        var name = endpoint.Split('|')[0];
        var uri = endpoint.Split('|')[1];

        setup.AddHealthCheckEndpoint(name, uri);
    }

    setup.UseApiEndpointHttpMessageHandler(sp => HttpExtensions.ConfigureClientHandler());
});

healthCheckBuilder.AddInMemoryStorage();

var app = builder.Build();

app.MapDefaultEndpoints();

// Under certain scenarios, e.g minikube / linux environment / behind load balancer
// https redirection could lead dev's to over complicated configuration for testing purpouses
// In production is a good practice to keep it true
if (app.Configuration["USE_HTTPS_REDIRECTION"] == "true")
{
    app.UseHttpsRedirection();
    app.UseHsts();
}

app.MapHealthChecksUI(setup =>
{
    setup.AddCustomStylesheet("devstore.css");
    setup.UIPath = "/";
    setup.PageTitle = "DevStore - Status";
});
app.Run();

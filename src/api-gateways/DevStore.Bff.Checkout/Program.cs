using DevStore.Bff.Checkout.Configuration;
using DevStore.Bff.Checkout.Extensions;
using DevStore.WebAPI.Core.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Logging.AddSerilog(new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger());

#region Configure Services

builder.Services.AddApiConfiguration(builder.Configuration);

builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.AddSwaggerConfiguration();

builder.Services.RegisterServices();

builder.Services.AddMessageBusConfiguration(Environment.GetEnvironmentVariable("ConnectionStrings__messaging"));

builder.Services.ConfigureGrpcServices(builder.Configuration);


var app = builder.Build();
#endregion

#region Configure Pipeline


app.UseSwaggerConfiguration();

app.UseApiConfiguration(app.Environment);

app.Run();

#endregion
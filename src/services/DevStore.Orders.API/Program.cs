using DevStore.Orders.API.Configuration;
using DevStore.Orders.Infra.Context;
using DevStore.WebAPI.Core.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.AddServiceDefaults();

builder.Logging.AddSerilog(new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger());

builder.AddSqlServerDbContext<OrdersContext>("DSOrders");

#region Configure Services

builder.Services.AddApiConfiguration(builder.Configuration);

builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.AddSwaggerConfiguration();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

builder.Services.RegisterServices();

builder.Services.AddMessageBusConfiguration(Environment.GetEnvironmentVariable("ConnectionStrings__messaging"));

var app = builder.Build();
#endregion

#region Configure Pipeline

DbMigrationHelpers.EnsureSeedData(app).Wait();

app.UseSwaggerConfiguration();

app.UseApiConfiguration(app.Environment);

app.Run();

#endregion
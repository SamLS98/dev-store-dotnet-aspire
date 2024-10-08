using DevStore.Catalog.API.Configuration;
using DevStore.Catalog.API.Data;
using DevStore.WebAPI.Core.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Logging.AddSerilog(new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger());

builder.AddSqlServerDbContext<CatalogContext>("DSCatalog");

#region Configure Services

builder.Services.AddApiConfiguration(builder.Configuration);

builder.Services.AddMessageBusConfiguration(Environment.GetEnvironmentVariable("ConnectionStrings__messaging"));

builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.AddSwaggerConfiguration();

builder.Services.RegisterServices();


var app = builder.Build();
#endregion

#region Configure Pipeline

DbMigrationHelpers.EnsureSeedData(app).Wait();

app.UseSwaggerConfiguration();

app.UseApiConfiguration(app.Environment);

app.Run();

#endregion
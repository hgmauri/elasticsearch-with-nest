using System;
using Elastic.Apm.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sample.Elasticsearch.WebApi.Core.Extensions;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);
    SerilogExtensions.AddSerilog(builder.Configuration);
    builder.Host.UseSerilog(Log.Logger);

    builder.Services.AddApiConfiguration();

    builder.Services.AddElasticsearch(builder.Configuration);
    builder.Services.AddSwagger(builder.Configuration);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseApiConfiguration(app.Environment);

    app.UseSwaggerDoc();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}
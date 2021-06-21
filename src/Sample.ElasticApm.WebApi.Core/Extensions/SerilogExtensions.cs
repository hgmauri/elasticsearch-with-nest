using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.Elasticsearch;

namespace Sample.Elasticsearch.WebApi.Core.Extensions
{
    public static class SerilogExtensions
    {
        public static void AddSerilog(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.WithProperty("ApplicationName", $"API Exemplo - {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}")
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithDemystifiedStackTraces()
                .WriteTo.Debug()
                .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
                .Filter.ByExcluding(z => z.MessageTemplate.Text.Contains("erro de negócio"))
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticsearchSettings:uri"]))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = "indexlogs",
                    ModifyConnectionSettings = x => x.BasicAuthentication(configuration["ElasticsearchSettings:username"], configuration["ElasticsearchSettings:password"])
                })
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .CreateLogger();
        }
    }
}

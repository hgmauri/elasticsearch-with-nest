using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;
using Sample.Elasticsearch.Domain.Applications;
using Sample.Elasticsearch.Domain.Interfaces;
using Sample.Elasticsearch.Domain.Repositories;
using Sample.Elasticsearch.WebApi.Core.HealthCheck;
using Sample.Elasticsearch.WebApi.Core.Middleware;
using Serilog;

namespace Sample.Elasticsearch.WebApi.Core.Extensions
{
    public static class ApiConfigurationExtensions
    {
        public static void AddApiConfiguration(this IServiceCollection services)
        {
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddTransient<IActorsApplication, ActorsApplication>();
            services.AddTransient<IActorsRepository, ActorsRepository>();
            services.AddScoped<IMyCustomService, MyCustomService>();

            services.AddResponseCompression();
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

            services.AddControllers();
        }

        public static void UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = SerilogExtensions.EnrichFromRequest);

            app.UseMiddleware<RequestSerilLogMiddleware>();
            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
        }
    }
}

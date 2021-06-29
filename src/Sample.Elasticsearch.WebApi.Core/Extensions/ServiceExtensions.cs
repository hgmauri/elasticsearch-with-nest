using Microsoft.Extensions.DependencyInjection;
using Sample.Elasticsearch.Domain.Applications;
using Sample.Elasticsearch.Domain.Interfaces;
using Sample.Elasticsearch.Domain.Repositories;
using Sample.Elasticsearch.WebApi.Core.HealthCheck;

namespace Sample.Elasticsearch.WebApi.Core.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IActorsApplication, ActorsApplication>();
            services.AddTransient<IActorsRepository, ActorsRepository>();
            services.AddScoped<IMyCustomService, MyCustomService>();
        }
    }
}

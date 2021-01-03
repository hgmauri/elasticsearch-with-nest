using Microsoft.Extensions.DependencyInjection;
using Sample.Elasticsearch.Domain.Application;
using Sample.Elasticsearch.Domain.Concrete;
using Sample.Elasticsearch.WebApi.Core.HealthCheck;

namespace Sample.Elasticsearch.WebApi.Core.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IActorsApplication, ActorsApplication>();
            services.AddScoped<IMyCustomService, MyCustomService>();
        }
    }
}

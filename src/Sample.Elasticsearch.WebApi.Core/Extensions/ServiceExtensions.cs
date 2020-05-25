using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.Elasticsearch.Domain.Application;
using Sample.Elasticsearch.Domain.Concrete;

namespace Sample.Elasticsearch.WebApi.Core.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IActorsApplication, ActorsApplication>();
        }
    }
}

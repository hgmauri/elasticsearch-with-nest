using Sample.Elasticsearch.Infrastructure.Elastic;
using Sample.Elasticsearch.Infrastructure.Indices;
using Sample.Elasticsearch.Infrastructure.Interfaces;

namespace Sample.Elasticsearch.Domain.Interfaces
{
    public interface IActorsRepository : IElasticBaseRepository<IndexActors>
    {
    }
}

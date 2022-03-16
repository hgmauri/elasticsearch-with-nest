using Sample.Elasticsearch.Infrastructure.Elastic;
using Sample.Elasticsearch.Infrastructure.Indices;

namespace Sample.Elasticsearch.Domain.Interfaces;

public interface IActorsRepository : IElasticBaseRepository<IndexActors>
{
}
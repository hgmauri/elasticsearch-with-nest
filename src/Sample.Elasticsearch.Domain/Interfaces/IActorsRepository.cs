using Sample.Elasticsearch.Domain.Indices;

namespace Sample.Elasticsearch.Domain.Interfaces
{
    public interface IActorsRepository : IElasticBaseRepository<IndexActors>
    {
    }
}

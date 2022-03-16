using Nest;
using Sample.Elasticsearch.Domain.Interfaces;
using Sample.Elasticsearch.Infrastructure.Elastic;
using Sample.Elasticsearch.Infrastructure.Indices;

namespace Sample.Elasticsearch.Domain.Repositories;

public class ActorsRepository : ElasticBaseRepository<IndexActors>, IActorsRepository
{
    public ActorsRepository(IElasticClient elasticClient)
        : base(elasticClient)
    {
    }

    public override string IndexName { get; } = "indexactors";
}
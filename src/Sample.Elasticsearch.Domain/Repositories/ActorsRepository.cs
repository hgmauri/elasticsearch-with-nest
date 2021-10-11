using Nest;
using Sample.Elasticsearch.Domain.Interfaces;
using Sample.Elasticsearch.Infrastructure.Indices;
using Sample.Elasticsearch.Infrastructure.Repositories;

namespace Sample.Elasticsearch.Domain.Repositories
{
    public class ActorsRepository : ElasticBaseRepository<IndexActors>, IActorsRepository
    {
        public ActorsRepository(IElasticClient elasticClient) 
            : base(elasticClient)
        {
        }

        public override string IndexName { get; } = "indexactors";
    }
}

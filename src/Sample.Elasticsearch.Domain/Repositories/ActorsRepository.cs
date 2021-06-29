using Nest;
using Sample.Elasticsearch.Domain.Indices;
using Sample.Elasticsearch.Domain.Interfaces;

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

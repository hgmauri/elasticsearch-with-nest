using System;
using System.Collections.Generic;
using System.Linq;
using Nest;
using Sample.Elasticsearch.Domain.Concrete;
using Sample.Elasticsearch.Domain.Indices;

namespace Sample.Elasticsearch.Domain.Application
{
    public class ActorsApplication : IActorsApplication
    {
        private readonly IElasticClient _elasticClient;

        public ActorsApplication(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public void PostActorsSample()
        {
            var descriptor = new BulkDescriptor();

            if (!_elasticClient.Indices.Exists(nameof(IndexActors).ToLower()).Exists)
                _elasticClient.Indices.Create(nameof(IndexActors).ToLower());

            descriptor.UpdateMany<IndexActors>(IndexActors.GetSampleData(), (b, u) => b
                .Index(nameof(IndexActors).ToLower())
                .Doc(u)
                .DocAsUpsert());

            var insert = _elasticClient.Bulk(descriptor);

            if (!insert.IsValid)
                throw new Exception(insert.OriginalException.ToString());
        }

        public ICollection<IndexActors> GetAll()
        {
            var result = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .Size(10)
                .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            return result?.ToList();
        }

        public ICollection<IndexActors> GetByName(string name)
        {
            var query = new QueryContainerDescriptor<IndexActors>().Term(t => t.Field(f => f.Name).Value(name));

            var result = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .Query(s => query)
                .Size(10)
                .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            return result?.ToList();
        }
    }
}

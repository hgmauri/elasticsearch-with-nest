using System;

namespace Sample.Elasticsearch.Infrastructure.Indices
{
    public abstract class ElasticBaseIndex
    {
        public string Id { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}

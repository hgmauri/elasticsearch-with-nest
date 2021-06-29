using System;

namespace Sample.Elasticsearch.Domain.Indices
{
    public abstract class ElasticBaseIndex
    {
        public string Id { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}

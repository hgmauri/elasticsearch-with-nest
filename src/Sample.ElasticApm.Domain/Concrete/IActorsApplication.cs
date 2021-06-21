using System;
using System.Collections.Generic;
using Sample.Elasticsearch.Domain.Indices;
using Sample.Elasticsearch.Domain.Model;

namespace Sample.Elasticsearch.Domain.Concrete
{
    public interface IActorsApplication
    {
        void PostActorsSample();
        ICollection<IndexActors> GetAll();
        ICollection<IndexActors> GetByName(string name);
        ICollection<IndexActors> GetByDescription(string description);
        ICollection<IndexActors> GetActorsCondition(string name, string description, DateTime? birthdate);
        ICollection<IndexActors> GetActorsAllCondition(string term);
        ActorsAggregationModel GetActorsAggregation();
    }
}

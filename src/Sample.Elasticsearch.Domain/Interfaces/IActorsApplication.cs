using Sample.Elasticsearch.Infrastructure.Abstractions;
using Sample.Elasticsearch.Infrastructure.Indices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Elasticsearch.Domain.Interfaces
{
    public interface IActorsApplication
    {
        Task PostActorsSample();
        Task<ICollection<IndexActors>> GetAll();
        Task<ICollection<IndexActors>> GetByName(string name);
        ICollection<IndexActors> SearchInAllFiels(string term);
        ICollection<IndexActors> GetByDescription(string description);
        ICollection<IndexActors> GetActorsCondition(string name, string description, DateTime? birthdate);
        ICollection<IndexActors> GetActorsAllCondition(string term);
        ActorsAggregationModel GetActorsAggregation();
    }
}

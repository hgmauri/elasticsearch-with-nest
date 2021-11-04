using Sample.Elasticsearch.Infrastructure.Abstractions;
using Sample.Elasticsearch.Infrastructure.Indices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Elasticsearch.Domain.Interfaces
{
    public interface IActorsApplication
    {
        Task InsertManyAsync();
        Task<ICollection<IndexActors>> GetAllAsync();
        Task<ICollection<IndexActors>> GetByNameWithTerm(string name);
        Task<ICollection<IndexActors>> GetByNameWithMatch(string name);
        Task<ICollection<IndexActors>> GetByNameAndDescriptionMultiMatch(string term);
        Task<ICollection<IndexActors>> GetByNameWithMatchPhrase(string name);
        Task<ICollection<IndexActors>> GetByNameWithMatchPhrasePrefix(string name);
        Task<ICollection<IndexActors>> GetByNameWithWildcard(string name);
        Task<ICollection<IndexActors>> GetByNameWithFuzzy(string name);
        Task<ICollection<IndexActors>> SearchInAllFiels(string term);
        Task<ICollection<IndexActors>> GetByDescriptionMatch(string description);
        Task<ICollection<IndexActors>> GetActorsCondition(string name, string description, DateTime? birthdate);
        Task<ICollection<IndexActors>> GetActorsAllCondition(string term);
        Task<ActorsAggregationModel> GetActorsAggregation();
    }
}

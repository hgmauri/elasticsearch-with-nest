using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using Sample.Elasticsearch.Domain.Interfaces;
using Sample.Elasticsearch.Infrastructure.Abstractions;
using Sample.Elasticsearch.Infrastructure.Indices;

namespace Sample.Elasticsearch.Domain.Applications
{
    public class ActorsApplication : IActorsApplication
    {
        private readonly IActorsRepository _actorsRepository;
        public ActorsApplication(IActorsRepository actorsRepository)
        {
            _actorsRepository = actorsRepository;
        }

        public async Task InsertManyAsync()
        {
            await _actorsRepository.InsertManyAsync(NestExtensions.GetSampleData());
        }

        public async Task<ICollection<IndexActors>> GetAllAsync()
        {
            var result = await _actorsRepository.GetAllAsync();

            return result.ToList();
        }

        //lowcase
        public async Task<ICollection<IndexActors>> GetByNameWithTerm(string name)
        {
            var query = new QueryContainerDescriptor<IndexActors>().Term(p => p.Field(p => p.Name).Value(name));
            var result = await _actorsRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        //using operator OR in case insensitive
        public async Task<ICollection<IndexActors>> GetByNameWithMatch(string name)
        {
            var query = new QueryContainerDescriptor<IndexActors>().Match(p => p.Field(f => f.Name).Query(name));
            var result = await _actorsRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ICollection<IndexActors>> GetByNameWithMatchPhrase(string name)
        {
            var query = new QueryContainerDescriptor<IndexActors>().MatchPhrase(p => p.Field(f => f.Name).Query(name));
            var result = await _actorsRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ICollection<IndexActors>> GetByNameWithMatchPhrasePrefix(string name)
        {
            var query = new QueryContainerDescriptor<IndexActors>().MatchPhrasePrefix(p => p.Field(f => f.Name).Query(name));
            var result = await _actorsRepository.SearchAsync(_ => query);
            
            return result?.ToList();
        }

        //contains
        public async Task<ICollection<IndexActors>> GetByNameWithWildcard(string name)
        {
            var query = new QueryContainerDescriptor<IndexActors>().Wildcard(w => w.Field(f => f.Name).Value(name + "*"));
            var result = await _actorsRepository.SearchAsync(_ => query);
            
            return result?.ToList();
        }

        public async Task<ICollection<IndexActors>> GetByNameWithFuzzy(string name)
        {
            var query = new QueryContainerDescriptor<IndexActors>().Fuzzy(descriptor => descriptor.Field(p => p.Name).Value(name));
            var result = await _actorsRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ICollection<IndexActors>> SearchInAllFiels(string term)
        {
            var query = NestExtensions.BuildMultiMatchQuery<IndexActors>(term);
            var result = await _actorsRepository.SearchAsync(_ => query);

            return result.ToList();
        }

        public async Task<ICollection<IndexActors>> GetByDescriptionMatch(string description)
        {
            //case insensitive
            var query = new QueryContainerDescriptor<IndexActors>().Match(p => p.Field(f => f.Description).Query(description));
            var result = await _actorsRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ICollection<IndexActors>> GetActorsCondition(string name, string description, DateTime? birthdate)
        {
            QueryContainer query = new QueryContainerDescriptor<IndexActors>();

            if (!string.IsNullOrEmpty(name))
            {
                query = query && new QueryContainerDescriptor<IndexActors>().MatchPhrasePrefix(qs => qs.Field(fs => fs.Name).Query(name));
            }
            if (!string.IsNullOrEmpty(description))
            {
                query = query && new QueryContainerDescriptor<IndexActors>().MatchPhrasePrefix(qs => qs.Field(fs => fs.Description).Query(description));
            }
            if (birthdate.HasValue)
            {
                query = query && new QueryContainerDescriptor<IndexActors>()
                .Bool(b => b.Filter(f => f.DateRange(dt => dt
                                           .Field(field => field.BirthDate)
                                           .GreaterThanOrEquals(birthdate)
                                           .LessThanOrEquals(birthdate)
                                           .TimeZone("+00:00"))));
            }

            var result = await _actorsRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ICollection<IndexActors>> GetActorsAllCondition(string term)
        {
            var query = new QueryContainerDescriptor<IndexActors>().Bool(b => b.Must(m => m.Exists(e => e.Field(f => f.Description))));
            int.TryParse(term, out var numero);

            query = query && new QueryContainerDescriptor<IndexActors>().Wildcard(w => w.Field(f => f.Name).Value($"*{term}*")) //bad performance, use MatchPhrasePrefix
                    || new QueryContainerDescriptor<IndexActors>().Wildcard(w => w.Field(f => f.Description).Value($"*{term}*")) //bad performance, use MatchPhrasePrefix
                    || new QueryContainerDescriptor<IndexActors>().Term(w => w.Age, numero)
                    || new QueryContainerDescriptor<IndexActors>().Term(w => w.TotalMovies, numero);

            var result = await _actorsRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ActorsAggregationModel> GetActorsAggregation()
        {
            var query = new QueryContainerDescriptor<IndexActors>().Bool(b => b.Must(m => m.Exists(e => e.Field(f => f.Description))));

            var result = await _actorsRepository.SearchAsync(_ => query, a =>
                        a.Sum("TotalAge", sa => sa.Field(o => o.Age))
                        .Sum("TotalMovies", sa => sa.Field(p => p.TotalMovies))
                        .Average("AvAge", sa => sa.Field(p => p.Age)));

            var totalAge = NestExtensions.ObterBucketAggregationDouble(result.Aggregations, "TotalAge");
            var totalMovies = NestExtensions.ObterBucketAggregationDouble(result.Aggregations, "TotalMovies");
            var avAge = NestExtensions.ObterBucketAggregationDouble(result.Aggregations, "AvAge");

            return new ActorsAggregationModel { TotalAge = totalAge, TotalMovies = totalMovies, AverageAge = avAge };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using Sample.Elasticsearch.Domain.Abstractions;
using Sample.Elasticsearch.Domain.Indices;
using Sample.Elasticsearch.Domain.Interfaces;

namespace Sample.Elasticsearch.Domain.Applications
{
    public class ActorsApplication : IActorsApplication
    {
        private readonly IElasticClient _elasticClient;
        private readonly IActorsRepository _actorsRepository;
        public ActorsApplication(IElasticClient elasticClient, IActorsRepository actorsRepository)
        {
            _elasticClient = elasticClient;
            _actorsRepository = actorsRepository;
        }

        public async Task PostActorsSample()
        {
            await _actorsRepository.InsertManyAsync(NestExtensions.GetSampleData());
        }

        public async Task<ICollection<IndexActors>> GetAll()
        {
            var result = await _actorsRepository.GetAllAsync();

            //or
            var result1 = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            //or
            var result2 = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .MatchAll()).Documents.ToList();

            //or
            var result3 = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .From(0)
                .Size(10)
                .MatchAll()).Documents.ToList();

            //or (scroll)
            var results = new List<IndexActors>();
            var isScrollSetHasData = true;
            var result4 = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .From(0)
                .Size(10)
                .Scroll("1m")
                .MatchAll());

            if (result4.Documents.Any())
                results.AddRange(result4.Documents);

            var scrollid = result4.ScrollId;

            while (isScrollSetHasData)
            {
                var loopingResponse = _elasticClient.Scroll<IndexActors>("1m", scrollid);
                if (loopingResponse.IsValid)
                {
                    results.AddRange(loopingResponse.Documents);
                    scrollid = loopingResponse.ScrollId;
                }
                isScrollSetHasData = loopingResponse.Documents.Any();
            }

            await _elasticClient.ClearScrollAsync(new ClearScrollRequest(scrollid));

            return result.ToList();
        }

        public async Task<ICollection<IndexActors>> GetByName(string name)
        {
            //lowcase
            var result = await _actorsRepository.SearchAsync(descriptor =>
             {
                 return descriptor.Query(containerDescriptor => containerDescriptor.Term(p => p.Field(p => p.Name).Value(name)));
             });

            //contains
            var result2 = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .Query(s => s.Wildcard(w => w.Field(f => f.Name).Value(name + "*")))
                .Size(10)
                .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            //using operator OR in case insensitive
            var result3 = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .Query(s => s.Match(m => m.Field(f => f.Name).Query(name)))
                .Size(10)
                .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            //using operator AND in case insensitive
            var result4 = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .Query(s => s.Match(m => m.Field(f => f.Name).Query(name).Operator(Operator.And)))
                .Size(10)
                .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            //usign match phrase
            var result5 = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .Query(s => s.MatchPhrase(m => m.Field(f => f.Name).Query(name)))
                .Size(10)
                .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            //usign match phrase slop 1 inconsistency
            var result6 = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .Query(s => s.MatchPhrase(m => m.Field(f => f.Name).Query(name).Slop(1)))
                .Size(10)
                .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            return result?.ToList();
        }

        public ICollection<IndexActors> SearchInAllFiels(string term)
        {
            var result = _elasticClient.Search<IndexActors>(s => s
                .Query(p => NestExtensions.BuildMultiMatchQuery<IndexActors>(term))).Documents.ToList();

            return result;
        }

        public ICollection<IndexActors> GetByDescription(string description)
        {
            //case insensitive
            var query = new QueryContainerDescriptor<IndexActors>().Match(t => t.Field(f => f.Description).Query(description));
            var result = _elasticClient.Search<IndexActors>(s => s
                    .Index(nameof(IndexActors).ToLower())
                    .Query(s => query)
                    .Size(10)
                    .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            //case sensitive
            var query1 = new QueryContainerDescriptor<IndexActors>().Term(t => t.Description, description);
            var result1 = _elasticClient.Search<IndexActors>(s => s
                    .Index(nameof(IndexActors).ToLower())
                    .Query(s => query)
                    .Size(10)
                    .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            return result?.ToList();
        }

        public ICollection<IndexActors> GetActorsCondition(string name, string description, DateTime? birthdate)
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

            var result = _elasticClient.Search<IndexActors>(s => s
                    .Index(nameof(IndexActors).ToLower())
                    .Query(s => query)
                    .Size(10)
                    .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            return result?.ToList();
        }

        public ICollection<IndexActors> GetActorsAllCondition(string term)
        {
            var query = new QueryContainerDescriptor<IndexActors>().Bool(b => b.Must(m => m.Exists(e => e.Field(f => f.Description))));
            int.TryParse(term, out var numero);

            query = query && new QueryContainerDescriptor<IndexActors>().Wildcard(w => w.Field(f => f.Name).Value($"*{term}*")) //bad performance, use MatchPhrasePrefix
                    || new QueryContainerDescriptor<IndexActors>().Wildcard(w => w.Field(f => f.Description).Value($"*{term}*")) //bad performance, use MatchPhrasePrefix
                    || new QueryContainerDescriptor<IndexActors>().Term(w => w.Age, numero)
                    || new QueryContainerDescriptor<IndexActors>().Term(w => w.TotalMovies, numero);

            var result = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .Query(s => query)
                .Size(10)
                .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            return result?.ToList();
        }

        public ActorsAggregationModel GetActorsAggregation()
        {
            var query = new QueryContainerDescriptor<IndexActors>().Bool(b => b.Must(m => m.Exists(e => e.Field(f => f.Description))));

            var result = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .Query(s => query)
                .Aggregations(a => a.Sum("TotalAge", sa => sa.Field(o => o.Age))
                            .Sum("TotalMovies", sa => sa.Field(p => p.TotalMovies))
                            .Average("AvAge", sa => sa.Field(p => p.Age))
                        ));

            var totalAge = NestExtensions.ObterBucketAggregationDouble(result.Aggregations, "TotalAge");
            var totalMovies = NestExtensions.ObterBucketAggregationDouble(result.Aggregations, "TotalMovies");
            var avAge = NestExtensions.ObterBucketAggregationDouble(result.Aggregations, "AvAge");

            return new ActorsAggregationModel { TotalAge = totalAge, TotalMovies = totalMovies, AverageAge = avAge };
        }
    }
}

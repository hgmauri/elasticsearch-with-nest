using System;
using System.Collections.Generic;
using System.Linq;
using Nest;
using Sample.Elasticsearch.Domain.Concrete;
using Sample.Elasticsearch.Domain.Indices;
using Sample.Elasticsearch.Domain.Model;
using static System.Int32;

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

            _elasticClient.IndexMany<IndexActors>(IndexActors.GetSampleData());

            //or
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
            var results = new List<IndexActors>();
            var isScrollSetHasData = true;

            var result = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            var result2 = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .MatchAll()).Documents.ToList();

            var result3 = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .From(0)
                .Size(10)
                .MatchAll()).Documents.ToList();

            //scroll
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

            _elasticClient.ClearScroll(new ClearScrollRequest(scrollid));

            return result.ToList();
        }

        public ICollection<IndexActors> GetByName(string name)
        {
            //usado em lowcase
            var query = new QueryContainerDescriptor<IndexActors>().Term(t => t.Field(f => f.Name).Value(name));

            var result = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .Query(s => query)
                .Size(10)
                .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            var result2 = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .Query(s => s.Wildcard(w => w.Field(f => f.Name).Value(name + "*")))
                .Size(10)
                .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            var result3 = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .Query(s => s.Match(m => m.Field(f => f.Name).Query(name))) //Procura cada termo com o operador OR, case insensitive
                                                                            //.Query(s => s.Match(m => m.Field(f => f.Name).Query(name).Operator(Operator.And)) //com o operador AND
                .Size(10)
                .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            var result4 = _elasticClient.Search<IndexActors>(s => s
                .Index(nameof(IndexActors).ToLower())
                .Query(s => s.MatchPhrase(m => m.Field(f => f.Name).Query(name))) //Procura o termo que contenha a frase exata
                                                                                  //.Query(s => s.MatchPhrase(m => m.Field(f => f.Name).Query(name).Slop(1))) //Procura o termo que contenha a frase exata, pulando uma inconsistencia
                .Size(10)
                .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            return result?.ToList();
        }

        public ICollection<IndexActors> GetByDescription(string description)
        {
            //use Fuzzy para autocomplete
            var query = new QueryContainerDescriptor<IndexActors>().Match(t => t.Field(f => f.Description).Query(description)); //case insensitive, OU
            //var query = new QueryContainerDescriptor<IndexActors>().Term(t => t.Description, description); //case sensitive
            var result = _elasticClient.Search<IndexActors>(s => s
                    .Index(nameof(IndexActors).ToLower())
                    .Query(s => query)
                    .Size(10)
                    .Sort(q => q.Descending(p => p.BirthDate)))?.Documents;

            return result?.ToList();
        }

        public ICollection<IndexActors> GetActorsCondition(string name, string description, DateTime? birthdate)
        {
            //use Fuzzy para autocomplete

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
            TryParse(term, out int numero);

            query = query && new QueryContainerDescriptor<IndexActors>().Wildcard(w => w.Field(f => f.Name).Value($"*{term}*")) //performance ruim, use MatchPhrasePrefix
                    || new QueryContainerDescriptor<IndexActors>().Wildcard(w => w.Field(f => f.Description).Value($"*{term}*")) //performance ruim, use MatchPhrasePrefix
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

            var totalAge = ObterBucketAggregationDouble(result.Aggregations, "TotalAge");
            var totalMovies = ObterBucketAggregationDouble(result.Aggregations, "TotalMovies");
            var avAge = ObterBucketAggregationDouble(result.Aggregations, "AvAge");

            return new ActorsAggregationModel { TotalAge = totalAge, TotalMovies = totalMovies, AverageAge = avAge };
        }

        public static double ObterBucketAggregationDouble(AggregateDictionary agg, string bucket)
        {
            return agg.BucketScript(bucket).Value.HasValue ? agg.BucketScript(bucket).Value.Value : 0;
        }
    }
}

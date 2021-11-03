using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using Sample.Elasticsearch.Infrastructure.Abstractions;
using Sample.Elasticsearch.Infrastructure.Indices;
using Serilog;

namespace Sample.Elasticsearch.Infrastructure.Elastic
{
    public abstract class ElasticBaseRepository<T> : IElasticBaseRepository<T> where T : ElasticBaseIndex
    {
        private readonly IElasticClient _elasticClient;

        public abstract string IndexName { get; }

        protected ElasticBaseRepository(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<T> GetAsync(string id)
        {
            var response = await _elasticClient.GetAsync(DocumentPath<T>.Id(id).Index(IndexName));

            if (response.IsValid)
                return response.Source;

            Log.Error(response.OriginalException, response.ServerError?.ToString());
            return null;
        }

        public async Task<T> GetAsync(IGetRequest request)
        {
            var response = await _elasticClient.GetAsync<T>(request);

            if (response.IsValid)
                return response.Source;

            Log.Error(response.OriginalException, response.ServerError?.ToString());
            return null;
        }

        public async Task<T> FindAsync(string id)
        {
            var response = await _elasticClient.GetAsync(DocumentPath<T>.Id(id).Index(IndexName));

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return response.Source;
        }

        public async Task<T> FindAsync(IGetRequest request)
        {
            var response = await _elasticClient.GetAsync<T>(request);

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return response.Source;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var search = new SearchDescriptor<T>(IndexName).MatchAll();
            var response = await _elasticClient.SearchAsync<T>(search);

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return response.Hits.Select(hit => hit.Source).ToList();
        }

        public async Task<IEnumerable<T>> GetManyAsync(IEnumerable<string> ids)
        {
            var response = await _elasticClient.GetManyAsync<T>(ids, IndexName);

            return response.Select(item => item.Source).ToList();
        }

        public async Task<IEnumerable<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> request)
        {
            var response = await _elasticClient.SearchAsync<T>(s =>
                s.Index(IndexName)
                    .Query(request)
                    .Lenient());

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return response.Hits.Select(hit => hit.Source).ToList();
        }

        public async Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> request,
            Func<AggregationContainerDescriptor<T>, IAggregationContainer> aggregationsSelector)
        {
            var response = await _elasticClient.SearchAsync<T>(s =>
                s.Index(IndexName)
                    .Query(request)
                    .Aggregations(aggregationsSelector));

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return response;
        }

        public async Task<IEnumerable<T>> SearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector)
        {
            var list = new List<T>();
            var response = await _elasticClient.SearchAsync(selector);

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return response.Hits.Select(hit => hit.Source).ToList();
        }

        public async Task<IEnumerable<T>> SearchInAllFields(string term)
        {
            var result = await _elasticClient.SearchAsync<T>(s => s.Query(p => NestExtensions.BuildMultiMatchQuery<T>(term)));

            return result.Documents.ToList();
        }

        public async Task<bool> CreateIndexAsync()
        {
            if (!(await _elasticClient.Indices.ExistsAsync(IndexName)).Exists)
            {
                await _elasticClient.Indices.CreateAsync(IndexName, c =>
                {
                    c.Map<T>(p => p.AutoMap());
                    return c;
                });
            }
            return true;
        }

        public async Task<bool> InsertAsync(T model)
        {
            var response = await _elasticClient.IndexAsync(model, descriptor => descriptor.Index(IndexName));

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return true;
        }

        public async Task<bool> InsertManyAsync(IList<T> tList)
        {
            var response = await _elasticClient.IndexManyAsync(tList, IndexName);

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return true;
        }

        public async Task<bool> UpdateAsync(T model)
        {
            var response = await _elasticClient.UpdateAsync(DocumentPath<T>.Id(model.Id).Index(IndexName), p => p.Doc(model));

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return true;
        }

        public async Task<bool> UpdatePartAsync(T model, object partialEntity)
        {
            var request = new UpdateRequest<T, object>(IndexName, model.Id)
            {
                Doc = partialEntity
            };
            var response = await _elasticClient.UpdateAsync(request);

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return true;
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            var response = await _elasticClient.DeleteAsync(DocumentPath<T>.Id(id).Index(IndexName));

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return true;
        }

        public async Task<bool> DeleteByQueryAsync(Func<QueryContainerDescriptor<T>, QueryContainer> selector)
        {
            var response = await _elasticClient.DeleteByQueryAsync<T>(q => q
                .Query(selector)
                .Index(IndexName)
            );

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return true;
        }

        public async Task<long> GetTotalCountAsync()
        {
            var search = new SearchDescriptor<T>(IndexName).MatchAll();
            var response = await _elasticClient.SearchAsync<T>(search);

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return response.Total;
        }

        public async Task<bool> ExistAsync(string id)
        {
            var response = await _elasticClient.DocumentExistsAsync(DocumentPath<T>.Id(id).Index(IndexName));

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return response.Exists;
        }
    }
}

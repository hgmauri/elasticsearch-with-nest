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
        protected IElasticClient _elasticClient;

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

            if (response.IsValid)
                return response.Source;

            Log.Error(response.OriginalException, response.ServerError?.ToString());
            return null;
        }

        public async Task<T> FindAsync(IGetRequest request)
        {
            var response = await _elasticClient.GetAsync<T>(request);

            if (response.IsValid)
                return response.Source;

            Log.Error(response.OriginalException, response.ServerError?.ToString());
            return null;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            IList<T> list = new List<T>();
            var search = new SearchDescriptor<T>(IndexName).MatchAll();
            var response = await _elasticClient.SearchAsync<T>(search);
            if (response.IsValid)
            {
                foreach (var hit in response.Hits)
                {
                    list.Add(hit.Source);
                }
                return list;
            }

            throw new Exception(response.ServerError.ToString());
        }

        public async Task<IEnumerable<T>> GetManyAsync(IEnumerable<string> ids)
        {
            var response = await _elasticClient.GetManyAsync<T>(ids, IndexName);
            return response.Select(item => item.Source).ToList();
        }

        public async Task<IEnumerable<T>> SearchAsync(ISearchRequest request)
        {
            var list = new List<T>();
            var response = await _elasticClient.SearchAsync<T>(request);

            if (response.IsValid)
            {
                list.AddRange(from hit in response.Hits select hit.Source);
                return list;
            }

            Log.Error(response.OriginalException, response.ServerError?.ToString());
            return null;
        }

        public async Task<T> SearchByFieldAsync(Func<QueryContainerDescriptor<T>, QueryContainer> request)
        {
            var response = await _elasticClient.SearchAsync<T>(s => s.Index(IndexName).Query(request));

            if (response.IsValid)
            {
                var list = from hit in response.Hits select hit.Source;
                var entry = list.FirstOrDefault();

                return entry;
            }

            Log.Error(response.OriginalException, response.ServerError?.ToString());
            return null;
        }

        public async Task<IEnumerable<T>> SearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector)
        {
            var list = new List<T>();
            var response = await _elasticClient.SearchAsync(selector);

            if (response.IsValid)
            {
                list.AddRange(from hit in response.Hits select hit.Source);
                return list;
            }

            Log.Error(response.OriginalException, response.ServerError?.ToString());
            return null;
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
                await _elasticClient.Indices.CreateAsync(IndexName, descriptor =>
                {
                    descriptor.Map(mappingDescriptor => mappingDescriptor.AutoMap<T>());
                    return descriptor;
                });
            }
            return true;
        }

        public async Task<bool> InsertAsync(T model)
        {
            var response = await _elasticClient.IndexAsync(model, descriptor => descriptor.Index(IndexName));

            if (response.IsValid)
                return true;

            Log.Error(response.OriginalException, response.ServerError?.ToString());
            return false;
        }

        public async Task<bool> InsertManyAsync(IList<T> tList)
        {
            var response = await _elasticClient.IndexManyAsync(tList, IndexName);

            if (response.IsValid)
                return true;

            Log.Error(response.OriginalException, response.ServerError?.ToString());
            return false;
        }

        public async Task<bool> UpdateAsync(T model)
        {
            var response = await _elasticClient.UpdateAsync(DocumentPath<T>.Id(model.Id).Index(IndexName), p => p.Doc(model));

            if (response.IsValid)
                return true;

            Log.Error(response.OriginalException, response.ServerError?.ToString());
            return false;
        }

        public async Task<bool> UpdatePartAsync(T model, object partialEntity)
        {
            var request = new UpdateRequest<T, object>(IndexName, model.Id)
            {
                Doc = partialEntity
            };
            var response = await _elasticClient.UpdateAsync(request);

            if (response.IsValid)
                return true;

            Log.Error(response.OriginalException, response.ServerError?.ToString());
            return false;
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            var response = await _elasticClient.DeleteAsync(DocumentPath<T>.Id(id).Index(IndexName));

            if (response.IsValid)
                return true;

            Log.Error(response.OriginalException, response.ServerError?.ToString());
            return false;
        }

        public async Task<bool> DeleteByQueryAsync(Func<QueryContainerDescriptor<T>, QueryContainer> selector)
        {
            var response = await _elasticClient.DeleteByQueryAsync<T>(q => q
                .Query(selector)
                .Index(IndexName)
            );

            if (response.IsValid)
                return true;

            Log.Error(response.OriginalException, response.ServerError?.ToString());
            return false;
        }

        public async Task<long> GetTotalCountAsync()
        {
            var search = new SearchDescriptor<T>(IndexName).MatchAll();
            var response = await _elasticClient.SearchAsync<T>(search);

            if (response.IsValid)
                return response.Total;

            Log.Error(response.OriginalException, response.ServerError?.ToString());
            return default;
        }

        public async Task<bool> ExistAsync(string id)
        {
            var response = await _elasticClient.DocumentExistsAsync(DocumentPath<T>.Id(id).Index(IndexName));
            return response.Exists;
        }
    }
}

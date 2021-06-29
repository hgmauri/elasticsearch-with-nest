using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using Sample.Elasticsearch.Domain.Indices;
using Sample.Elasticsearch.Domain.Interfaces;

namespace Sample.Elasticsearch.Domain.Repositories
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
            GetResponse<T> response = await _elasticClient.GetAsync(DocumentPath<T>.Id(id).Index(IndexName));
            if (response.IsValid)
            {
                return response.Source;
            }

            throw new Exception(response.ServerError.ToString());
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

        public async Task<T> GetAsync(IGetRequest request)
        {
            GetResponse<T> response = await _elasticClient.GetAsync<T>(request);
            if (response.IsValid)
            {
                return response.Source;
            }

            throw new Exception(response.ServerError.ToString());
        }

        public async Task<T> FindAsync(string id)
        {
            var response = await _elasticClient.GetAsync(DocumentPath<T>.Id(id).Index(IndexName));
            return response.IsValid ? response.Source : null;
        }

        public async Task<T> FindAsync(IGetRequest request)
        {
            GetResponse<T> response = await _elasticClient.GetAsync<T>(request);
            return response.IsValid ? response.Source : null;
        }

        public async Task<IEnumerable<T>> GetManyAsync(IEnumerable<string> ids)
        {
            IEnumerable<IMultiGetHit<T>> response = await _elasticClient.GetManyAsync<T>(ids, IndexName);
            return response.Select(item => item.Source).ToList();
        }

        public async Task<IEnumerable<T>> SearchAsync(ISearchRequest request)
        {
            IList<T> list = new List<T>();
            ISearchResponse<T> response = await _elasticClient.SearchAsync<T>(request);
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

        public async Task<IEnumerable<T>> SearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector)
        {
            IList<T> list = new List<T>();
            ISearchResponse<T> response = await _elasticClient.SearchAsync(selector);
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

        public IEnumerable<IHit<T>> HitsSearch(ISearchRequest request)
        {
            ISearchResponse<T> response = _elasticClient.Search<T>(request);
            if (response.IsValid)
            {
                return response.Hits;
            }

            throw new Exception(response.ServerError.ToString());
        }

        public async Task<IEnumerable<IHit<T>>> HitsSearchAsync(ISearchRequest request)
        {
            ISearchResponse<T> response = await _elasticClient.SearchAsync<T>(request);
            if (response.IsValid)
            {
                return response.Hits;
            }

            throw new Exception(response.ServerError.ToString());
        }

        public IEnumerable<IHit<T>> HitsSearch(Func<SearchDescriptor<T>, ISearchRequest> selector)
        {
            ISearchResponse<T> response = _elasticClient.Search(selector);
            return response.IsValid ? response.Hits : throw new Exception(response.ServerError.ToString());
        }

        public async Task<IEnumerable<IHit<T>>> HitsSearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector)
        {
            ISearchResponse<T> response = await _elasticClient.SearchAsync(selector);
            return response.IsValid ? response.Hits : throw new Exception(response.ServerError.ToString());
        }

        public async Task<bool> InsertAsync(T t)
        {
            if (!(await _elasticClient.Indices.ExistsAsync(IndexName)).Exists)
            {
                await _elasticClient.Indices.CreateAsync(IndexName);
            }
            var response = await _elasticClient.IndexDocumentAsync(t);
            return response.IsValid ? true : throw new Exception(response.ServerError.ToString());
        }

        public async Task<bool> InsertManyAsync(IList<T> tList)
        {
            if (!(await _elasticClient.Indices.ExistsAsync(IndexName)).Exists)
            {
                await _elasticClient.Indices.CreateAsync(IndexName);
            }
            var response = await _elasticClient.IndexManyAsync(tList);
            return response.IsValid ? true : throw new Exception(response.ServerError.ToString());
        }

        public async Task<bool> UpdateAsync(T t)
        {
            UpdateResponse<T> response = await _elasticClient.UpdateAsync(DocumentPath<T>.Id(t.Id).Index(IndexName), p => p.Doc(t));
            return response.IsValid ? true : throw new Exception(response.ServerError.ToString());
        }

        public async Task<bool> UpdatePartAsync(T t, object partialEntity)
        {
            IUpdateRequest<T, object> request = new UpdateRequest<T, object>(IndexName, t.Id)
            {
                Doc = partialEntity
            };
            UpdateResponse<T> response = await _elasticClient.UpdateAsync(request);
            return response.IsValid ? true : throw new Exception(response.ServerError.ToString());
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            var response = await _elasticClient.DeleteAsync(DocumentPath<T>.Id(id).Index(IndexName));
            return response.IsValid ? true : throw new Exception(response.ServerError.ToString());
        }

        public async Task<long> GetTotalCountAsync()
        {
            var search = new SearchDescriptor<T>(IndexName).MatchAll();
            var response = await _elasticClient.SearchAsync<T>(search);
            if (response.IsValid)
            {
                return response.Total;
            }

            throw new Exception(response.ServerError.ToString());
        }

        public async Task<bool> ExistAsync(string id)
        {
            var response = await _elasticClient.DocumentExistsAsync(DocumentPath<T>.Id(id).Index(IndexName));
            return response.Exists;
        }
    }
}

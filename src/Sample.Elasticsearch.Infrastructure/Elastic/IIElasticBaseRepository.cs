using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace Sample.Elasticsearch.Infrastructure.Elastic
{
    public interface IElasticBaseRepository<T> where T : class
    {
        Task<T> GetAsync(string id);
        Task<T> GetAsync(IGetRequest request);
        Task<T> FindAsync(string id);
        Task<T> FindAsync(IGetRequest request);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetManyAsync(IEnumerable<string> ids);
        Task<IEnumerable<T>> SearchAsync(ISearchRequest request);
        Task<IEnumerable<T>> SearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector);
        Task<T> SearchByFieldAsync(Func<QueryContainerDescriptor<T>, QueryContainer> request);
        Task<IEnumerable<T>> SearchInAllFields(string term);
        Task<bool> CreateIndexAsync();
        Task<bool> InsertAsync(T t);
        Task<bool> InsertManyAsync(IList<T> tList);
        Task<bool> UpdateAsync(T t);
        Task<bool> UpdatePartAsync(T t, object partialEntity);
        Task<long> GetTotalCountAsync();
        Task<bool> DeleteByIdAsync(string id);
        Task<bool> DeleteByQueryAsync(Func<QueryContainerDescriptor<T>, QueryContainer> selector);
        Task<bool> ExistAsync(string id);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace Sample.Elasticsearch.Domain.Interfaces
{
    public interface IElasticBaseRepository<T> where T : class
    {
        Task<T> GetAsync(string id);
        Task<T> GetAsync(IGetRequest request);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> FindAsync(string id);
        Task<T> FindAsync(IGetRequest request);
        Task<IEnumerable<T>> GetManyAsync(IEnumerable<string> ids);
        Task<IEnumerable<T>> SearchAsync(ISearchRequest request);
        Task<IEnumerable<T>> SearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector);
        Task<bool> InsertAsync(T t);
        Task<bool> InsertManyAsync(IList<T> tList);
        Task<bool> UpdateAsync(T t);
        Task<bool> UpdatePartAsync(T t, object partialEntity);
        Task<long> GetTotalCountAsync();
        Task<bool> DeleteByIdAsync(string id);
        Task<bool> ExistAsync(string id);
    }
}

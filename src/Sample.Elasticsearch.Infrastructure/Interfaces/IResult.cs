
namespace Sample.Elasticsearch.Infrastructure.Interfaces
{
    public interface IResult<T> where T : class
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}

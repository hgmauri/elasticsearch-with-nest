using Sample.Elasticsearch.Infrastructure.Interfaces;

namespace Sample.Elasticsearch.Infrastructure.Abstractions
{
    public class Result<T> : IResult<T> where T : class, new()
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }

        public Result(bool success, string message, T data)
        {
            Data = data;
            Message = message;
            Success = success;
        }

        public Result(bool success, string message)
        {
            Message = message;
            Success = success;
        }

        public Result(bool success, T data)
        {
            Data = data;
            Success = success;
        }
        public Result(bool success)
        {
            Success = success;
        }

        public static Result<T> FailureResult(string message) => new(false, message);
        public static Result<T> SuccessResult(T data) => new(true, data);
    }
}

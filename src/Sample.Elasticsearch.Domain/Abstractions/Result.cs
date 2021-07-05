using Sample.Elasticsearch.Domain.Interfaces;

namespace Sample.Elasticsearch.Domain.Abstractions
{
    public class Result<T> : IResult<T> where T : class, new()
    {
        public Result(string message, bool success, T data)
        {
            Data = data;
            Message = message;
            IsSuccess = success;
        }

        public Result(bool success, string message)
        {
            Message = message;
            IsSuccess = success;
        }

        public Result(bool success, T data)
        {
            Data = data;
            IsSuccess = success;
        }
        public Result(bool success)
        {
            IsSuccess = success;
        }
        public T Data { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public static Result<T> Failure(string message) => new Result<T>(false, message);
        public static Result<T> Success(T data) => new Result<T>(true, data);
    }
}

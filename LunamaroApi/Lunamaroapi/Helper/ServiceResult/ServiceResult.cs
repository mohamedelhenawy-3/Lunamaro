namespace Lunamaroapi.Helper.ServiceResult
{
    public class ServiceResult <T>
    {
        public bool Success { get; init; }
        public string? Error { get; init; }
        public int StatusCode { get; init; }
        public T? Data { get; init; }

        public static ServiceResult<T> Ok(T data)
            => new() { Success = true, Data = data, StatusCode = 200 };

        public static ServiceResult<T> Fail(string error, int statusCode)
            => new() { Success = false, Error = error, StatusCode = statusCode };
    }
}

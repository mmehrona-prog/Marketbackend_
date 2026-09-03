using System.Net;

namespace MarketBackend.DTOs
{
    public sealed class APIResponse<T>
    {
        public bool Status { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }
        public static APIResponse<T> Ok(T data, HttpStatusCode code=HttpStatusCode.OK)
        {
            return new APIResponse<T>
            {
                Status = true,
                StatusCode = code,
                Data = data,
                Error=string.Empty
            };
        }

        public static APIResponse<T> Fail(string error, HttpStatusCode code = HttpStatusCode.BadRequest)
        {
            return new APIResponse<T>
            {
                Status = false,
                StatusCode = code,
                Data = default,
                Error = error
            };
        }
    }
}

using System.Net;

namespace MarketBackend.DTOs
{
    public class APIResponse
    {
        public bool Status { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public dynamic Data { get; set; }
        public dynamic Error { get; set; }
    }
}

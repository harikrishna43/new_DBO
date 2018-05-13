using System.Net;

namespace DBO.Response
{
    public class ResponseContent
    {
        public string Message { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }

    public class ResponseContent<T> : ResponseContent
    {
        public T Data { get; set; }
    }
}
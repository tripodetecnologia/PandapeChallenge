using System.Net;

namespace Challenge.Application.Website.Models
{
    public class ResponseModel
    {
        public bool IsCorrect { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public HttpStatusCode Status { get; set; }

    }
}
using System.Net;

namespace Challenge.Common
{
    public class ResponseFactory<TEntity>
    {
        public int Code { get; set; }
        public string Message { get; set; }        
        public HttpStatusCode Status { get; set; }        
        public TEntity Body { get; set; }

        public static ResponseFactory<TEntity> RequestOK(TEntity data, string message = "")
        {
            return new ResponseFactory<TEntity>
            {
                Code = GenericResponseCodes.CodeOK,
                Message = string.IsNullOrWhiteSpace(message) ? "OK" : message,
                Body = data
            };
        }

        public static ResponseFactory<TEntity> RequestValidation(string message)
        {
            return new ResponseFactory<TEntity>
            {
                Code = GenericResponseCodes.CodeValidation,
                Message = message,
                Body = default(TEntity)
            };
        }

        public static ResponseFactory<TEntity> RequestError(string message = "")
        {
            return new ResponseFactory<TEntity>
            {
                Code = GenericResponseCodes.CodeError,
                Message = string.IsNullOrWhiteSpace(message) ? "Error inesperado" : message,
                Body = default(TEntity)
            };
        }
    }

    public static class GenericResponseCodes
    {
        public const int CodeOK = 1;
        public const int CodeValidation = 0;
        public const int CodeError = -1;
    }
}
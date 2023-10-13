namespace Challenge.Application.Website.Utils
{
    public static class JsonResponseFactory
    {
        public static object ErrorResponse(string error)
        {
            return new { Success = false, Message = error };
        }
    }
}
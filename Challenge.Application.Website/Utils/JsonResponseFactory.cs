namespace Challenge.Application.Website.Utils
{
    public static class JsonResponseFactory
    {
        public static object ErrorResponse(string error)
        {
            return new { Success = false, Message = error };
        }
        public static object SuccessResponse(string message)
        {
            return new { Success = true, Message = message };
        }
    }
}
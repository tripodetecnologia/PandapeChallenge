namespace Challenge.Domain.LogService
{
    public interface ILogService
    {
        void Add(LogKey logKey, string data);
        void Generate();
    }
}

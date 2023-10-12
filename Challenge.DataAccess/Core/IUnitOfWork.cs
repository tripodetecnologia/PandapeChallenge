namespace Challenge.DataAccess.Core
{
    public interface IUnitOfWork
    {
        IRepository<T> Repository<T>() where T : class;
        Task SaveChangesAsync();                

    }
}

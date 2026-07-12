namespace Domain.CronosBot.Repositories
{
    public interface IUnitOfWork
    {
        Task Commit();
    }
}

namespace MrCMS.DataAccess
{
    public interface IDbContextFactory
    {
        IDbContext GetContext();
    }
}
namespace MrCMS.Helpers
{
    public interface IDbContextFactory
    {
        IDbContext GetContext();
    }
}
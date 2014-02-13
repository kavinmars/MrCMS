using System.Data.Common;

namespace MrCMS.DataAccess
{
    public interface IDbConfiguration
    {
        DbConnection GetDbConnection();
    }
}
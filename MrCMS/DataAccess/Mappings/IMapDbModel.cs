using System.Data.Entity;

namespace MrCMS.DataAccess.Mappings
{
    public interface IMapDbModel
    {
        void Map(DbModelBuilder dbModelBuilder);
        int Specificity { get; }
    }
}
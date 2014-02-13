using System.Data.Entity.Infrastructure.Pluralization;

namespace MrCMS.DataAccess
{
    public class UnpluralizedService : IPluralizationService
    {
        public string Pluralize(string word)
        {
            return word;
        }

        public string Singularize(string word)
        {
            return word;
        }
    }
}
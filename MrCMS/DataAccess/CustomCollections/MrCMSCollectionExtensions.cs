using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.DataAccess.CustomCollections
{
    public static class MrCMSCollectionExtensions
    {
        public static MrCMSCollection<T> ToMrCMSCollection<T>(this IEnumerable<T> enumerable) where T : SystemEntity
        {
            return new MrCMSCollection<T>(enumerable);
        }
        public static MrCMSSet<T> ToMrCMSSet<T>(this IEnumerable<T> enumerable) where T : SystemEntity
        {
            return new MrCMSSet<T>(enumerable);
        }
        public static MrCMSList<T> ToMrCMSList<T>(this IEnumerable<T> enumerable) where T : SystemEntity
        {
            return new MrCMSList<T>(enumerable);
        }
    }
}
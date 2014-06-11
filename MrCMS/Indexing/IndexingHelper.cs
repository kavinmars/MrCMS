using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Tasks;
using MrCMS.Website;
using Ninject;

namespace MrCMS.Indexing
{
    public static class IndexingHelper
    {
        //public static bool AnyIndexes<T>()
        //{
        //    return GetDefinitionTypes<T>().Any();
        //}
        public static bool AnyIndexes(IKernel kernel, object obj, LuceneOperation operation)
        {
            if (obj == null)
                return false;
            return
                GetIndexDefinitionTypes(kernel).Any(
                    definition => definition.GetUpdateTypes(operation).Any(type => type.IsAssignableFrom(obj.GetType())));
        }
        //public static List<IndexDefinition> GetDefinitionTypes<T>()
        //{
        //    return DefinitionTypes(typeof(T));
        //}

        //private static List<IndexDefinition> DefinitionTypes(Type typeToCheck)
        //{
        //    //throw new NotImplementedException();
        //    var definitionTypes =
        //        IndexDefinitionTypes.FindAll(type => type.UpdateTypes.Any(typeToCheck.IsAssignableFrom));
        //    return definitionTypes;
        //}

        public static List<IndexDefinition> GetIndexDefinitionTypes(IKernel kernel)
        {
                return
                    TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(IndexDefinition<>))
                        .Select(type => kernel.Get(type) as IndexDefinition)
                        .ToList();
        }

        public static IndexDefinition Get<T>(IKernel kernel) where T :IndexDefinition
        {
            return GetIndexDefinitionTypes(kernel).OfType<T>().FirstOrDefault();
        }
    }
}
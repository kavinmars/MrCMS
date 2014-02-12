using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Helpers;

namespace MrCMS.DataAccess.Mappings
{
    public static class DbMappings
    {
        private static Dictionary<Type, IMapDbModel> _mappers;
        public static Dictionary<Type, IMapDbModel> Mappers
        {
            get { return _mappers = _mappers ?? GetMappers(); }
        }

        public static Dictionary<Type, IMapDbModel> GetMappers()
        {
            var dictionary = new Dictionary<Type, IMapDbModel>();

            foreach (var type in TypeHelper.GetMappedClassesAssignableFrom<SystemEntity>().Where(type => !type.ContainsGenericParameters))
            {
                var types = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(SystemEntityMapping<>).MakeGenericType(type));
                if (types.Any())
                {
                    var mapDbModel = Activator.CreateInstance(types.First()) as IMapDbModel;
                    dictionary.Add(type, mapDbModel);
                }
                else
                {
                    bool mapped = false;
                    var allTypesAssignableFrom = TypeHelper.GetAllTypesAssignableFrom(typeof(SystemEntityMapping<>));
                    if (allTypesAssignableFrom.Any())
                    {
                        Type entityType = type;
                        foreach (var assType in allTypesAssignableFrom.Where(mappingType => IsValid(mappingType,entityType)).OrderBy(mappingType=>Specificity(mappingType,entityType)))
                        {
                            try
                            {
                                var mapDbModel =  Activator.CreateInstance(assType.MakeGenericType(type)) as IMapDbModel;
                                dictionary.Add(type, mapDbModel);
                                mapped = true;
                                break;
                            }
                            catch
                            {
                                
                            }
                        }
                    }
                    if (!mapped)
                    {
                        var mapDbModel =
                            Activator.CreateInstance(typeof (DefaultSystemEntityMapping<>).MakeGenericType(type)) as
                                IMapDbModel;
                        dictionary.Add(type, mapDbModel);
                    }
                }
            }
            return dictionary;
        }

        private static bool IsValid(Type mappingType, Type entityType)
        {
            try
            {
                var mapDbModel = Activator.CreateInstance(mappingType.MakeGenericType(entityType)) as IMapDbModel;
                return mapDbModel != null;
            }
            catch
            {
                return false;
            }
        }

        private static int Specificity(Type mappingType, Type entityType)
        {
            try
            {
                var mapDbModel = Activator.CreateInstance(mappingType.MakeGenericType(entityType)) as IMapDbModel;
                return mapDbModel.Specificity;
            }
            catch
            {
                return int.MaxValue;
            }
        }
    }
}
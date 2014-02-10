using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elmah;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Tests
{
    public abstract class InMemoryDatabaseTest : MrCMSTest
    {
        protected static IDbContext Session;

        protected InMemoryDatabaseTest()
        {
            //var dbConfiguration = new DbConfiguration
            //    {
            //        Override = new SQLiteConnection(ConfigurationManager.ConnectionStrings["mrcms"].ConnectionString)
            //    };

            //Database.SetInitializer(new DropCreateDatabaseAlways<MrCMSDbContext>());
            //var mrCMSDbContext = new MrCMSDbContext(dbConfiguration);
            var site = new Site { Name = "Current Site", BaseUrl = "www.currentsite.com" };
            Session = new InMemoryDbContext(site);

            SetupUser();


            CurrentSite = Session.Transact(session =>
                {
                    CurrentRequestData.CurrentSite = site;
                    session.Add(site);
                    return site;
                });

            CurrentRequestData.SiteSettings = new SiteSettings { TimeZone = TimeZoneInfo.Local.Id };

            CurrentRequestData.ErrorSignal = new ErrorSignal();
        }

        protected Site CurrentSite { get; set; }


        private void SetupUser()
        {
            var user = new User
                {
                    Email = "test@example.com",
                    IsActive = true,
                };

            var adminUserRole = new UserRole
                {
                    Name = UserRole.Administrator
                };

            user.Roles = new HashSet<UserRole> { adminUserRole };
            adminUserRole.Users = new HashSet<User> { user };

            CurrentRequestData.CurrentUser = user;
        }

        public override void Dispose()
        {
            Dispose(true);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (Session != null)
                {
                    Session.Dispose();
                    Session = null;
                }
                base.Dispose();
            }
        }
    }

    public class InMemoryDbContext : IDbContext
    {
        private readonly Site _currentSite;
        private readonly Dictionary<Type, HashSet<SystemEntity>> _data;
        private readonly List<Type> _typesToMap;

        public InMemoryDbContext(Site currentSite)
        {
            _currentSite = currentSite;
            _typesToMap = GetTypesToMap();
            _data = new Dictionary<Type, HashSet<SystemEntity>>();
            foreach (Type type in _typesToMap)
            {
                _data[type] = new HashSet<SystemEntity>();
            }
        }

        private List<Type> GetTypesToMap()
        {
         
            List<Type> allClasses = TypeHelper.GetMappedClassesAssignableFrom<SystemEntity>()
                                              .Select(GetDataType)
                                              .Where(type => type != null)
                                              .Distinct()
                                              .ToList();
            var systemTypes =
                allClasses.FindAll(type => type != typeof (SiteEntity) && type.BaseType == typeof (SystemEntity));
            var siteTypes = allClasses.FindAll(type => type.BaseType == typeof(SiteEntity));
            var toMap = systemTypes.Concat(siteTypes).ToList();
            return toMap;
        }

        public void Dispose()
        {
            _data.Clear();
        }

        public T Get<T>(int id) where T : SystemEntity
        {
            Type type = GetDataType(typeof(T));
            if (type == null)
                return null;
            HashSet<SystemEntity> systemEntities = _data[type];
            return systemEntities.FirstOrDefault(entity => entity.Id == id) as T;
        }

        public SystemEntity Get(Type type, int id)
        {
            type = GetDataType(type);
            if (type == null)
                return null;
            HashSet<SystemEntity> systemEntities = _data[type];
            return systemEntities.FirstOrDefault(entity => entity.Id == id);
        }

        public T Add<T>(T entity) where T : SystemEntity
        {
            Type type = GetDataType(typeof(T));
            if (type == null)
                return null;
            HashSet<SystemEntity> systemEntities = _data[type];
            entity.Id = systemEntities.Any() ? systemEntities.Max(systemEntity => systemEntity.Id) + 1 : 1;
            if (entity is SiteEntity && (entity as SiteEntity).Site == null)
                (entity as SiteEntity).Site = _currentSite;
            systemEntities.Add(entity);
            return entity;
        }

        public T Update<T>(T entity) where T : SystemEntity
        {
            Type type = GetDataType(typeof(T));
            if (type == null)
                return null;
            HashSet<SystemEntity> systemEntities = _data[type];
            SystemEntity systemEntity = systemEntities.FirstOrDefault(entity1 => entity1.Id == entity.Id);
            if (systemEntity == null)
                return null;

            systemEntities.Remove(systemEntity);
            systemEntities.Add(entity);
            return entity;
        }

        public void Delete<T>(T entity) where T : SystemEntity
        {
            Type type = GetDataType(typeof(T));
            if (type == null)
                return;
            HashSet<SystemEntity> systemEntities = _data[type];
            systemEntities.Remove(entity);
        }

        public T AddOrUpdate<T>(T entity) where T : SystemEntity
        {
            Type type = GetDataType(typeof(T));
            if (type == null)
                return null;
            HashSet<SystemEntity> systemEntities = _data[type];
            SystemEntity systemEntity = systemEntities.FirstOrDefault(entity1 => entity1.Id == entity.Id);
            if (systemEntity != null)
                Update(entity);
            else
                Add(entity);
            return entity;
        }

        public IQueryable<TEntity> Set<TEntity>() where TEntity : SystemEntity
        {
            Type type = GetDataType(typeof(TEntity));
            if (type == null)
                return null;
            HashSet<SystemEntity> systemEntities = _data[type];

            return systemEntities.OfType<TEntity>().AsQueryable();
        }

        public IQueryable<SystemEntity> Set(Type entityType)
        {
            Type type = GetDataType(entityType);
            if (type == null)
                return null;
            HashSet<SystemEntity> systemEntities = _data[type];

            return systemEntities.AsQueryable();
        }

        public int SaveChanges()
        {
            return 0;
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.Run(() => SaveChanges());
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => SaveChanges());
        }

        public Database Database { get { throw new NotImplementedException(); } }

        private static Type GetDataType(Type type)
        {
            Type thisType = type;
            while (thisType != null && thisType.BaseType != typeof(SiteEntity) && thisType.BaseType != typeof(SystemEntity))
            {
                thisType = thisType.BaseType;
            }
            return thisType;
        }
    }
}
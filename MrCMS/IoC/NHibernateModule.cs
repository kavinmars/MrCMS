using System;
using System.Collections.Generic;
using System.Web;
using MrCMS.DbConfiguration;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Helpers;
using NHibernate;
using Ninject;
using Ninject.Modules;
using Ninject.Parameters;
using Ninject.Web.Common;

namespace MrCMS.IoC
{
    public class NHibernateModule : NinjectModule
    {
        public NHibernateModule(NHibernateConfigurator configurator, Func<ISessionFactory> getSessionFactory = null, Func<ISession> getSession = null)
        {
            _configurator = configurator;
            _getSessionFactory = getSessionFactory;
            _getSession = getSession;
            _sessionFactory = _getSessionFactory != null ? _getSessionFactory() : _configurator.CreateSessionFactory();
        }
        public NHibernateModule(DatabaseType databaseType, bool inDevelopment = false, bool cacheEnabled = true)
        {
            _configurator = new NHibernateConfigurator { CacheEnabled = cacheEnabled, DatabaseType = databaseType, InDevelopment = inDevelopment };
            _sessionFactory = _getSessionFactory != null ? _getSessionFactory() : _configurator.CreateSessionFactory();
        }

        private readonly NHibernateConfigurator _configurator;

        private readonly Func<ISessionFactory> _getSessionFactory;
        private readonly Func<ISession> _getSession;
        private readonly ISessionFactory _sessionFactory;

        public override void Load()
        {
            Kernel.Bind<ISessionFactory>().ToMethod(context => _sessionFactory).InSingletonScope();

            Kernel.Bind<ISession>()
                .ToMethod(
                    context =>
                        _getSession != null
                            ? _getSession()
                            : context.Kernel.Get<ISessionFactory>().OpenFilteredSession())
                .InSingletonScope();
            //if (_forWebsite)
            //{
            //}
            //else
            //{
            //    Kernel.Bind<ISession>().ToMethod(context => _getSession != null ? _getSession() : context.Kernel.Get<ISessionFactory>().OpenFilteredSession()).
            //        InThreadScope();
            //}
        }
    }
}
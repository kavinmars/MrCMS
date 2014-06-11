using System;
using Microsoft.Owin;
using MrCMS.DbConfiguration;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Engine;
using Ninject;
using Ninject.Modules;

namespace MrCMS.IoC
{
    public class NHibernateModule : NinjectModule
    {
        private readonly IOwinContext _context;
        private readonly NHibernateConfigurator _configurator;

        private readonly Func<ISession> _getSession;
        private readonly Func<ISessionFactory> _getSessionFactory;
        private readonly IOwinContext _owinContext;
        private readonly ISessionFactory _sessionFactory;

        public NHibernateModule(NHibernateConfigurator configurator, Func<ISessionFactory> getSessionFactory = null,
            Func<ISession> getSession = null, IOwinContext owinContext = null)
        {
            _configurator = configurator;
            _getSessionFactory = getSessionFactory;
            _owinContext = owinContext;
            _getSession = getSession;
            _sessionFactory = _getSessionFactory != null ? _getSessionFactory() : _configurator.CreateSessionFactory();
        }

        public NHibernateModule(DatabaseType databaseType, bool inDevelopment = false, bool cacheEnabled = true, IOwinContext context = null)
        {
            _context = context;
            _configurator = new NHibernateConfigurator
            {
                CacheEnabled = cacheEnabled,
                DatabaseType = databaseType,
                InDevelopment = inDevelopment
            };
            _sessionFactory = _getSessionFactory != null ? _getSessionFactory() : _configurator.CreateSessionFactory();
        }

        public override void Load()
        {
            Kernel.Bind<ISessionFactory>().ToMethod(context => _sessionFactory).InSingletonScope();

            Kernel.Bind<ISession>()
                .ToMethod(
                    context =>
                    {
                        ISession session = _getSession != null
                            ? _getSession()
                            : context.Kernel.Get<ISessionFactory>().OpenFilteredSession();

                        ISessionImplementor sessionImplementation = session.GetSessionImplementation();
                        Guid sessionId = sessionImplementation.SessionId;


                        return session;
                    })
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using MrCMS.DataAccess;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Services
{
    public class UserService : IUserService
    {
        private readonly IDbContext _session;
        private readonly SiteSettings _siteSettings;

        public UserService(IDbContext session, SiteSettings siteSettings)
        {
            _session = session;
            _siteSettings = siteSettings;
        }

        public void AddUser(User user)
        {
            _session.Transact(session =>
                                  {
                                      session.Add(user);
                                  });
        }

        public void SaveUser(User user)
        {
            _session.Transact(session => session.Update(user));
        }

        public User GetUser(int id)
        {
            return _session.Get<User>(id);
        }

        public IPagedList<User> GetAllUsersPaged(int page)
        {
            return _session.Query<User>().Paged(page, _siteSettings.DefaultPageSize);
        }

        public User GetUserByEmail(string email)
        {
            string trim = email.Trim();
            return _session.Query<User>().FirstOrDefault(user => user.Email == trim);
        }

        public User GetUserByResetGuid(Guid resetGuid)
        {
            return
                _session.Query<User>()
                        .FirstOrDefault(
                            user =>
                            user.ResetPasswordGuid == resetGuid && user.ResetPasswordExpiry >= CurrentRequestData.Now);
        }

        public User GetCurrentUser(HttpContextBase context)
        {
            return context.User != null ? GetUserByEmail(context.User.Identity.Name) : null;
        }

        public void DeleteUser(User user)
        {
            _session.Transact(session => session.Delete(user));
        }

        /// <summary>
        /// Checks to see if the supplied email address is unique
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id">The id of user to exlcude from check. Has to be string because of AdditionFields on Remote property</param>
        /// <returns></returns>
        public bool IsUniqueEmail(string email, int? id = null)
        {
            if (id.HasValue)
            {
                return !_session.Query<User>().Any(u => u.Email == email && u.Id != id.Value);
            }
            return !_session.Query<User>().Any(u => u.Email == email);
        }

        /// <summary>
        /// Gets a count of active users
        /// </summary>
        /// <returns></returns>
        public int ActiveUsers()
        {
            return _session.Query<User>().Count(x => x.IsActive);
        }

        /// <summary>
        /// Gets a count of none active users
        /// </summary>
        /// <returns></returns>
        public int NonActiveUsers()
        {
            return _session.Query<User>().Count(x => !x.IsActive);
        }

        public T Get<T>(User user) where T : SystemEntity, IBelongToUser
        {
            return _session.Query<T>().FirstOrDefault(arg => arg.User == user);
        }

        public IList<T> GetAll<T>(User user) where T : SystemEntity, IBelongToUser
        {
            return _session.Query<T>().Where(arg => arg.User == user).ToList();
        }

        public IPagedList<T> GetPaged<T>(User user, Expression<Func<T, bool>> query = null, int page = 1) where T : SystemEntity, IBelongToUser
        {
            query = query ?? (arg => arg.User == user);
            return _session.Query<T>().Where(query).Paged(page);
        }
    }
}
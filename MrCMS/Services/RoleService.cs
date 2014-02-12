using System;
using System.Collections.Generic;
using MrCMS.DataAccess;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using System.Linq;

namespace MrCMS.Services
{
    public class RoleService : IRoleService
    {
        private readonly IDbContext _dbContext;

        public RoleService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SaveRole(UserRole role)
        {
            _dbContext.Transact(session => session.AddOrUpdate(role));
        }

        public IEnumerable<UserRole> GetAllRoles()
        {
            return _dbContext.Query<UserRole>();
        }

        public UserRole GetRoleByName(string name)
        {
            return _dbContext.Query<UserRole>().FirstOrDefault(role => role.Name == name);
        }

        public void DeleteRole(UserRole role)
        {
            if (!role.IsAdmin)
                _dbContext.Transact(session => session.Delete(role));
        }

        public bool IsOnlyAdmin(User user)
        {
            var adminRole = GetRoleByName(UserRole.Administrator);

            var users = adminRole.Users.Where(user1 => user1.IsActive).Distinct().ToList();
            return users.Count() == 1 && users.First() == user;
        }

        public IEnumerable<AutoCompleteResult> Search(string term)
        {
            IQueryable<UserRole> queryable = _dbContext.Query<UserRole>();

            if (!string.IsNullOrWhiteSpace(term))
                queryable = queryable.Where(x => x.Name.StartsWith(term, StringComparison.OrdinalIgnoreCase));

            var userRoles = queryable .ToList();
            return
                userRoles.Select(
                    tag =>
                    new AutoCompleteResult
                        {
                            id = tag.Id,
                            label = tag.Name,
                            value = tag.Name
                        });
        }

        public UserRole GetRole(int id)
        {
            return _dbContext.Get<UserRole>(id);
        }
    }
}
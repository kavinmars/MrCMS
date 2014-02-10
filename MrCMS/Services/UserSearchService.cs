using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;

namespace MrCMS.Services
{
    public class UserSearchService : IUserSearchService
    {
        private readonly IDbContext _dbContext;

        public UserSearchService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<SelectListItem> GetAllRoleOptions()
        {
            var roles = _dbContext.Set<UserRole>().OrderBy(role => role.Name).ToList();

            return roles.BuildSelectItemList(role => role.Name, role => role.Id.ToString(), emptyItemText: "Any role");
        }


        public IPagedList<User> GetUsersPaged(UserSearchQuery searchQuery)
        {
            IQueryable<User> query = _dbContext.Set<User>();

            if (!string.IsNullOrWhiteSpace(searchQuery.Query))
                query =
                    query.Where(
                        user =>
                        user.Email.Contains(searchQuery.Query) ||
                        user.LastName.Contains(searchQuery.Query) ||
                        user.FirstName.Contains(searchQuery.Query));
            if (searchQuery.UserRoleId != null)
            {
                UserRole role = _dbContext.Get<UserRole>(searchQuery.UserRoleId.Value);
                query = query.Where(user => user.Roles.Contains(role));
            }

            return query.Paged(searchQuery.Page);
        }
    }
}
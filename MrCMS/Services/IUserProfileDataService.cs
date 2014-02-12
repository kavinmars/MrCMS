using MrCMS.DataAccess;
using MrCMS.Entities.People;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public interface IUserProfileDataService
    {
        void Add<T>(T data) where T : UserProfileData;
        void Update<T>(T data) where T : UserProfileData;
        void Delete<T>(T data) where T : UserProfileData;
    }

    public class UserProfileDataService : IUserProfileDataService
    {
        private readonly IDbContext _dbContext;

        public UserProfileDataService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add<T>(T data) where T : UserProfileData
        {
            var user = data.User;
            if (user != null) user.UserProfileData.Add(data);
            _dbContext.Transact(session => session.Add(data));
        }

        public void Update<T>(T data) where T : UserProfileData
        {
            _dbContext.Transact(session => session.Update(data));
        }

        public void Delete<T>(T data) where T : UserProfileData
        {
            var user = data.User;
            if (user != null) user.UserProfileData.Remove(data);
            _dbContext.Transact(session => session.Delete(data));
        }
    }
}
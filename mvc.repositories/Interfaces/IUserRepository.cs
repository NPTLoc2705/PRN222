using mvc.dataaccess.Entities;
using mvc.dataaccess.ViewModels;
using mvc.dataaccess.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.repositories.Interfaces
{
    public interface IUserRepository
    {
        public User GetUserByEmail(string email);
        
        public User GetUserById(string userId);
        public User CreateUser(User user);
        Task<List<User>> GetAllUsers();
        UserProfileDTO GetUserProfile(Guid userId);
        bool DeleteUser(Guid userId);
        bool banUser(Guid userId);
        bool unBanUser(Guid userId);
        void UpdateUserProfile(UpdateUserViewModel user);
    }
}

using mvc.dataaccess.Entities;
using mvc.dataaccess.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        UserProfileDTO GetUserProfile(Guid userId);
        bool DeleteUser(Guid userId);
        bool BanUser(Guid userId);
        bool UnBanUser(Guid userId);
        void UpdateUserProfile(UpdateUserViewModel user);
    }
}

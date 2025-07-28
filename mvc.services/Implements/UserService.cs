using mvc.dataaccess.Entities;
using mvc.dataaccess.ViewModels.User;
using mvc.repositories.Implements;
using mvc.repositories.Interfaces;
using mvc.services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public Task<List<User>> GetAllUsers()
        {
            try
            {
                return _userRepository.GetAllUsers();
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all users", ex);
            }
        }

        public UserProfileDTO GetUserProfile(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return null;
                return _userRepository.GetUserProfile(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting user profile for ID: {userId}", ex);
            }
        }
        public bool DeleteUser(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return false;

                var user = _userRepository.GetUserById(userId.ToString());
                if (user == null)
                {
                    return false;
                }
                return _userRepository.DeleteUser(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting user ID: {userId}", ex);
            }
        }
        public bool BanUser(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return false;

                var user = _userRepository.GetUserById(userId.ToString());
                if (user == null)
                {
                    return false;

                }

                return _userRepository.banUser(userId);

            }
            catch (Exception ex)
            {
                throw new Exception($"Error banning user ID: {userId}", ex);

            }
        }

        public bool UnBanUser(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return false;

                var user = _userRepository.GetUserById(userId.ToString());
                if (user == null || user.IsActive)
                {
                    return false;

                }
                return _userRepository.unBanUser(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error unbanning user ID: {userId}", ex);
            }
        }

        public void UpdateUserProfile(UpdateUserViewModel user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            _userRepository.UpdateUserProfile(user);
        }
    
}
}

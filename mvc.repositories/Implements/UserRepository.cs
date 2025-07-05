using Microsoft.EntityFrameworkCore;
using mvc.dataaccess.Entities;
using mvc.dataaccess.ViewModels.User;
using mvc.repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.repositories.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public User GetUserByEmail(string email)
        { 
            try
            {
                return _context.Users.FirstOrDefault(u => u.Email.Equals(email));
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("An error occurred while retrieving the user by email.", ex);
            }
        }

        public User CreateUser(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "User cannot be null");
                }

                _context.Users.Add(user);
                _context.SaveChanges();
                return user;
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("An error occurred while creating the user.", ex);
            }
        }

        public User GetUserById(string userId)
        {
            try
            {
                if (userId != null)
                {
                    var id = Guid.Parse(userId); // Ensure the userId is a valid Guid
                    return _context.Users.FirstOrDefault(c => c.Id.Equals(id));

                }
                return null; // Return null if userId is null or invalid
            }
            catch (Exception ex)
            {

                throw new Exception("An error occurred while retrieving the user by ID.", ex);
            }

        }
        public Task<List<User>> GetAllUsers()
        {
            try
            {
                return _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving all users.", ex);
            }
        }
        public User GetUserProfile(Guid userId)
        {
            try
            {
                return _context.Users.FirstOrDefault(u => u.Id == userId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving user profile.", ex);
            }
        }

        public bool DeleteUser(Guid userId)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return false;

                }
                _context.Users.Remove(user);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the user.", ex);
            }
        }
        public bool banUser(Guid userId)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return false;
                }
                user.IsActive = false;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while banning the user.", ex);

            }
        }

        public bool unBanUser(Guid userId)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return false;
                }
                user.IsActive = true;
                _context.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while unbanning the user.", ex);
            }



        }

        public void UpdateUserProfile(UpdateUserViewModel user)
        {
            try
            {
                var userR = _context.Users.FirstOrDefault(u => u.Id == user.Id);
                if (userR == null)
                {
                    throw new Exception("User not found.");
                }

                userR.FullName = user.FullName;
                userR.PhoneNumber = user.PhoneNumber;
                userR.Address = user.Address;

                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    userR.Password = user.Password;
                }

                _context.Users.Update(userR);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the user profile.", ex);
            }
        }
    }
}

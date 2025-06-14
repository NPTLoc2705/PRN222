using Microsoft.Extensions.Configuration;
using mvc.dataaccess.Entities;
using mvc.repositories.Interfaces;
using mvc.services.Infrastructure;
using mvc.services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly TokenProvider _tokenProvider;

        public AuthService(IUserRepository userRepository, IConfiguration config, TokenProvider tokenProvider)
        {
            _userRepository = userRepository;
            _config = config;
            _tokenProvider = tokenProvider;
        }

        public User AuthenticateUser(string email, string password)
        {
            var adminEmail = _config["Admin:Email"];
            var adminPassword = _config["Admin:Password"];
            if (email.Equals(adminEmail) &&
                password.Equals(adminPassword))
            {
                var adminRoleString = _config["Admin:Role"];
                if (!Enum.TryParse<SystemRole>(adminRoleString, out var adminRole))
                {
                    throw new Exception($"Invalid role value: {adminRoleString}");
                }

                return new User
                {
                    FullName = _config["Admin:FullName"],
                    Role = adminRole,
                    Email = adminEmail,
                    PhoneNumber = _config["Admin:PhoneNumber"],
                    Address = _config["Admin:Address"],
                    Password = adminPassword,
                    IsActive = _config["Admin:IsActive"]?.ToLower() == "true"
                };
            }

            // Check regular user
            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            if (password.Equals(user.Password))
            {
                return user;
            }

            // Authentication failed
            return null;
        }

        public string GenerateToken(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            return _tokenProvider.Create(user);
        }

        public User GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            var user = _userRepository.GetUserByEmail(email);
            return user;
        }

        public User CreateUser(User user)
        {
            return CreateUserInternal(user);
        }

        public User RegisterUser(User user) 
        {
            // RegisterUser no longer needs to set defaults as they're already 
            // handled in the User class definition
            return CreateUserInternal(user);
        }

        private User CreateUserInternal(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null");

            var existingUser = _userRepository.GetUserByEmail(user.Email);
            if (existingUser != null)
                throw new Exception("User with this email already exists.");

            return _userRepository.CreateUser(user);
        }
    }
}

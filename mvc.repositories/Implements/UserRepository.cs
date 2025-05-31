using mvc.dataaccess.Entities;
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
    }
}

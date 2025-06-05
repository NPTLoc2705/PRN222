using mvc.dataaccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvc.services.Interfaces
{
    public interface IAuthService
    {
        public User AuthenticateUser(string email, string password);
        
        public User GetUserByEmail(string email);
        
        public User CreateUser(User user);
        
        public User RegisterUser(User user);
    }
}

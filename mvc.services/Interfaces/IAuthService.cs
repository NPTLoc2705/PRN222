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
        public string GenerateToken(User user);
    }
}

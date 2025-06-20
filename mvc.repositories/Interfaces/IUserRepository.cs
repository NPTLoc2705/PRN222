﻿using mvc.dataaccess.Entities;
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
        
        public User CreateUser(User user);
    }
}

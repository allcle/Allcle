using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public interface IUserRepository
    {
        List<User> GetUsers();
        void PostUsers(User _user);
        bool GetUserId(string _id);
        bool LoginUser(User _user);
    }
}

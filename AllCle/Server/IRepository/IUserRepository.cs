using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;

namespace Server.IRepository
{
    public interface IUserRepository
    {
        List<User> GetUsers();
        void PostUsers(User _user);
        void UpdateUsers(User _user);
        bool GetUserId(string _id);
        User LoginUser(string _id);
    }
}

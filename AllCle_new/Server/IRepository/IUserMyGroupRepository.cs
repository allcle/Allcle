using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;

namespace Server.IRepository
{
    public interface IUserMyGroupRepository
    {
        List<UserMyGroup> GetUserMyGroups(string _userId);
        void PostMyGroup(UserMyGroup _userMyGroup);
        void UpdateMyGroup(UserMyGroup _userMyGroup);
        List<UserMyGroup> CheckSaveMyGroupName(string _id, string MyGroupName);
        void DeleteMyGroup(IdMyGroup idMyGroup);
    }
}

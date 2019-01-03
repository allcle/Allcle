using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;

namespace Server.IRepository
{
    public interface IUserTimeTableRepository
    {
        List<UserTimeTable> GetUserTimeTables(string _userId);
        void PostTimeTalbe(UserTimeTable _userTimeTable);
    }
}

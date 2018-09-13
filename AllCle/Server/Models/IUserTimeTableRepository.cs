using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public interface IUserTimeTableRepository
    {
        List<UserTimeTable> GetUserTimeTables(string _userId);
        void PostTimeTalbe(UserTimeTable _userTimeTable);
    }
}

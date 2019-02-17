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
        void PostTimeTable(UserTimeTable _userTimeTable);
        //void UpdateTimeTable(string EditTime, string ID, string TimeTableName);
        void UpdateTimeTable(UserTimeTable _userTimeTable);
        void UpdateTimeTableName(UpdateTImeTableName updateTImeTableName);
        List<UserTimeTable> CheckSaveTimeTableName(string _id, string TimeTableName);
        void DeleteTimeTable(Del del);
    }
}

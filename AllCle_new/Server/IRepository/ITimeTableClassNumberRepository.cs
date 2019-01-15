using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;

namespace Server.IRepository
{
    public interface ITimeTableClassNumberRepository
    {
        List<string> GetTimeTableClassNumbers(string _id, string _no);
        void PostTimeTable(TimeTableClassNumber _timeTableClassNumber);
        //void UpdateTimeTableClassNumber(UpdateUserIdTimeTable _timeTableClassNumber);
    }
}

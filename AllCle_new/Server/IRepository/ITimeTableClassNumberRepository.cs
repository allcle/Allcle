using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;

namespace Server.IRepository
{
    public interface ITimeTableClassNumberRepository
    {
        List<TimeTableClassNumber> GetTimeTableClassNumbers(string _id, string _timeTableName);
        void PostTimeTable(TimeTableClassNumber _timeTableClassNumber);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public interface ITimeTableClassNumberRepository
    {
        List<TimeTableClassNumber> GetTimeTableClassNumbers(string _timeTableName);
        void PostTimeTable(TimeTableClassNumber _timeTableClassNumber);
    }
}

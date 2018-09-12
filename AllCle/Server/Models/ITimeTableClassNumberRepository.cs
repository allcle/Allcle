using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public interface ITimeTableClassNumberRepository
    {
        void PostTimeTalbe(TimeTableClassNumber _timeTableClassNumber);
    }
}

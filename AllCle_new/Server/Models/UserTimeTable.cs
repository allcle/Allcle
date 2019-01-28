using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class UserTimeTable
    {
        public string ID { get; set; }
        public string TimeTableName { get; set; }
        public string SaveTime { get; set; }
        public string EditTime { get; set; }
    }
}

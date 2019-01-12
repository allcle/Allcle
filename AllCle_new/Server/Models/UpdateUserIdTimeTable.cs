using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Server.Models
{
    public class UpdateUserIdTimeTable
    {
        public string ID { get; set; }
        public string TimeTableName { get; set; }
        public string OldTimeTableName { get; set; }
    }
}
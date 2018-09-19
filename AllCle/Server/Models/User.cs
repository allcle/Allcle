using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class User
    {
        public int NO { get; set; }
        public string Id { get; set; }
        public string Password { get; set; }
        //public int StudentId { get; set; } //~13: 1 14,15: 2 16~: 3
        //public int Major { get; set; } //공대 1 건축 2 나머지 3
        //public string TimeTable1 { get; set; }
        //public string Mygroup1 { get; set; }
    }
}

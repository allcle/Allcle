using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Models
{
    class DayOfTheWeek
    {
        public string period { get; set; }
        public string mon { get; set; }
        public string tue { get; set; }
        public string wed { get; set; }
        public string thu { get; set; }
        public string fri { get; set; }
        public string sat { get; set; }
        public DayOfTheWeek(string a, string b, string c, string d, string e, string f, string g)       // "|"를 기준으로 분류된 daylist에 해당하는 string 변수
        {
            period = a; mon = b; tue = c; wed = d; thu = e; fri = f; sat = g;
        }
    }
}

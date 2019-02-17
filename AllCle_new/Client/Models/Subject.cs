using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    class Subject
    {
        public int Grade { get; set; }
        public string deparment { get; set; }
        public string ClassNumber { get; set; }
        public string ClassName { get; set; }
        public int CreditCourse { get; set; }
        public string Professor { get; set; }
        public string 강의시간 { get; set; }
        public string Time1 { get; set; }
        public string Time2 { get; set; }
        public string Time3 { get; set; }
        public string Time4 { get; set; }
        public string Time5 { get; set; }
        public string Time6 { get; set; }
        public string Time7 { get; set; }
        public string Time8 { get; set; }
        public string LectureRoom { get; set; }
        private List<string> times = new List<string>();
        public List<string> Times
        {
            get
            {
                if (times.Count == 0)
                {
                    times.Add(Time1);
                    times.Add(Time2);
                    times.Add(Time3);
                    times.Add(Time4);
                    times.Add(Time5);
                    times.Add(Time6);
                    times.Add(Time7);
                    times.Add(Time8);
                }
                return times;
            }
        }
    }
}

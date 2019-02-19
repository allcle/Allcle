using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.IRepository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Server.Controllers
{
    [Route("api/[controller]")]
    public class TimeTableClassNumberController : Controller
    {

        private ITimeTableClassNumberRepository _repo;   // ISubjectRepository class의 객체 _repo 생성.

        // 의존성 주입: ISubjectRepository의 인스턴스를 SubjectRepository의 인스턴스로
        // Startup.cs에서 설정한 DI를 생성자를 이용하여 받는 코드 구현이다.
        public TimeTableClassNumberController(ITimeTableClassNumberRepository repo) // private _repo에 데이터를 넣는 메소드.
        {
            _repo = repo;
        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }     

       

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]TimeTableClassNumber _timeTalbeClassNumber)
        {
            _repo.PostTimeTable(_timeTalbeClassNumber);
        }

        [HttpGet("{ID}/{TimeTableName}/{ClassNumber}")]
        public bool GetClassNumber(string ID, string TimeTableName, string ClassNumber)
        {
            return _repo.GetClassNumber(ID, TimeTableName, ClassNumber);
        }

        // GET api/<controller>/5
        [HttpGet("{ID}/{TimeTableName}")]
        public IEnumerable<string> GetClassNumber (string ID, string TimeTableName)
        {
            return _repo.GetTimeTableClassNumbers(ID, TimeTableName);
        }    

        // PUT api/<controller>/5
        /*[HttpPut("{_id}/timetable/{_timeTableName}")]
        public void Put([FromBody]UpdateUserIdTimeTable _timeTableClassNumber)
        {
            _repo.UpdateTimeTableClassNumber(_timeTableClassNumber);
        }*/

        // DELETE api/<controller>/5
        [HttpDelete]
        public void Delete([FromBody]TimeTableClassNumber timeTableClassNumber)
        {
            _repo.DeleteSubjectInTimeTable(timeTableClassNumber);
        }
    }
}

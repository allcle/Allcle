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
    public class UserTimeTableController : Controller
    {
        private IUserTimeTableRepository _repo;   // ISubjectRepository class의 객체 _repo 생성.


        // 의존성 주입: ISubjectRepository의 인스턴스를 SubjectRepository의 인스턴스로
        // Startup.cs에서 설정한 DI를 생성자를 이용하여 받는 코드 구현이다.
        public UserTimeTableController(IUserTimeTableRepository repo) // private _repo에 데이터를 넣는 메소드.
        {
            _repo = repo;
        }


        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public IEnumerable<UserTimeTable> Get(string id)
        {
            return _repo.GetUserTimeTables(id);
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]UserTimeTable _userTimeTable)
        {
            _repo.PostTimeTable(_userTimeTable);
        }
        /*
        // GET api/<controller>/5
        [HttpGet("{ID}/update_edittime/{TimeTableName}/EditTime/{EditTime}")]
        public void UpdateTimeTable(string EditTime, string ID, string TimeTableName)
        {
            _repo.UpdateTimeTable(EditTime, ID, TimeTableName);
        }
        */
        // Update api/<controller>
        [HttpPut]
        public void UpdateTimeTable([FromBody]UserTimeTable _userTimeTable)
        {
            _repo.UpdateTimeTable(_userTimeTable);
        }



        // PUT api/<controller>/5
        [HttpPut("name")]
        public void Put([FromBody]UpdateTImeTableName updateTImeTableName)
        {
            _repo.UpdateTimeTableName(updateTImeTableName);
        }

        // GET api/<controller>/5
        [HttpGet("{_id}/TimeTableName/{TimeTableName}")]
        public IEnumerable<UserTimeTable> GetTimeTableClassNumbers(string _id, string TimeTableName)
        {
            return _repo.CheckSaveTimeTableName(_id, TimeTableName);
        }

        // DELETE api/<controller>/5
        [HttpDelete]
        public void Delete([FromBody]Del del)
        {
            _repo.DeleteTimeTable(del);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.IRepository;
using Server.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Server.Controllers
{
    [Route("api/[controller]")]
    public class UserMyGroupController : Controller
    {

        private IUserMyGroupRepository _repo;   // ISubjectRepository class의 객체 _repo 생성.


        // 의존성 주입: ISubjectRepository의 인스턴스를 SubjectRepository의 인스턴스로
        // Startup.cs에서 설정한 DI를 생성자를 이용하여 받는 코드 구현이다.
        public UserMyGroupController(IUserMyGroupRepository repo) // private _repo에 데이터를 넣는 메소드.
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
        public IEnumerable<UserMyGroup> Get(string id)
        {
            return _repo.GetUserMyGroups(id);
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]UserMyGroup _userMyGroup)
        {
            _repo.PostMyGroup(_userMyGroup);
        }

        // PUT api/<controller>/5
        [HttpPut]
        public void UpdateTimeTable([FromBody]UserMyGroup _userMyGroup)
        {
            _repo.UpdateMyGroup(_userMyGroup);
        }

        // DELETE api/<controller>/5
        [HttpDelete]
        public void Delete([FromBody]Del delGrp)
        {
            _repo.DeleteMyGroup(delGrp);
        }
    }
}

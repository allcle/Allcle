﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Server.Models;                       // models에 있는 ISubjectRepository.cs, Subject.cs, SubjectRepository.cs 즉, 입출력 관련 코드 구현한 파일 import
using Newtonsoft.Json;                      // C#의 JSON document를 다루기 위한 가장 기본적인 라이브러리
using System.IO;                            // C#의 파일 입출력을 다루는 라이브러리
using System.Runtime.Serialization.Json;    // 객체를 JSON으로 변환하는 라이브러리
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Server.Controllers
{
    [Route("api/Users")]
    public class UsersController : Controller
    {
        private IUserRepository _repo;   // ISubjectRepository class의 객체 _repo 생성.


        // 의존성 주입: ISubjectRepository의 인스턴스를 SubjectRepository의 인스턴스로
        // Startup.cs에서 설정한 DI를 생성자를 이용하여 받는 코드 구현이다.
        public UsersController(IUserRepository repo) // private _repo에 데이터를 넣는 메소드.
        {
            _repo = repo;
        }
        
        // GET api/<controller>
        [HttpGet]
        public IEnumerable<User> GeUserst()
        {
            return _repo.GetUsers();
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]User _user)
        {
            //string password =  _user.Password;
            // 1234qwer

            // encyption
            // 1234qwer -> jifle8f3jufl3j8flj3js3

            // decyption
            // jifle8f3jufl3j8flj3js3 -> 1234qwer
            _repo.PostUsers(_user);
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

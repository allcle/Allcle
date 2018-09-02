using System;
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
    [Route("api/[controller]")]
    public class AllCleSubjects1Controller : Controller
    {
        private ISubjectRepository1 _repo;   // ISubjectRepository class의 객체 _repo 생성.
        

        // 의존성 주입: ISubjectRepository의 인스턴스를 SubjectRepository의 인스턴스로
        // Startup.cs에서 설정한 DI를 생성자를 이용하여 받는 코드 구현이다.
        public AllCleSubjects1Controller(ISubjectRepository1 repo) // private _repo에 데이터를 넣는 메소드.
        {
            _repo = repo;
        }
        [Route("{time}/{subject}")]
        [HttpGet]
        public IEnumerable<Subject> GetTimeOnSubjectOnSearchOff(string time, string subject)  //남은시간에서만, 담은과목 제외
        {
            return _repo.GetTimeOnSubjectOnSearchOff(time, subject);
        }
        [Route("{time}/subjectfilteroff")]
        [HttpGet]
        public IEnumerable<Subject> GetTimeOnSubjectOffSearchOff(string time)               //담은 과목만 제외
        {
            return _repo.GetTimeOnSubjectOffSearchOff(time);
        }
        [Route("timefilteroff/{subject}")]
        [HttpGet]
        public IEnumerable<Subject> GetTimeOffSubjectOnSearchOff(string subject)                  //남은 시간만에서만
        {
            return _repo.GetTimeOffSubjectOnSearchOff(subject);
        }
        [Route("timefilteroff/subjectfilteroff")]
        [HttpGet]
        public IEnumerable<Subject> GetTimeOffSubjectOffSearchOff()                             //전체 모든 과목 보기
        {
            return _repo.GetTimeOffSubjectOffSearchOff();
        }
        [Route("{time}/{subject}/{search}")]
        [HttpGet]
        public IEnumerable<Subject> GetTimeOnSubjectOnSearchOn(string time, string subject, string search) //남은시간에서만, 담은과목제외, 검색
        {
            return _repo.GetTimeOnSubjectOnSearchOn(time, subject, search);
        }
        [Route("timefilteroff/{subject}/{search}")]
        [HttpGet]
        public IEnumerable<Subject> GetTimeOffSubjectOnSearchOn(string subject, string search)              //담은 과목 제외하고 검색
        {
            return _repo.GetTimeOffSubjectOnSearchOn(subject, search);
        }
        [Route("{time}/subjectfilteroff/{search}")]
        [HttpGet]
        public IEnumerable<Subject> GetTimeOnSubjectOffSearchOn(string time, string search)                 //남은 시간에서만 검색
        {
            return _repo.GetTimeOnSubjectOffSearchOn(time, search);
        }
        [Route("timefilteroff/subjectfilteroff/{search}")]
        [HttpGet]
        public IEnumerable<Subject> GetTimeOffSubjectOffSearchOn(string search)                              //그냥 과목검색
        {
            return _repo.GetTimeOffSubjectOffSearchOn(search);
        }


        /*[HttpPost]
        public Subject PostSubject([FromBody] Subject Subject)              // ID와 PW를 server에 전송하는 메소드
        {
            _repo.AddSubject(Subject);                                      // 로그인할 때 사용하는 메소드
            return Subject;
        }*/



    }
}

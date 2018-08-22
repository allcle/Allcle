using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ApiTest.Models;                       // models에 있는 ISubjectRepository.cs, Subject.cs, SubjectRepository.cs 즉, 입출력 관련 코드 구현한 파일 import
using Newtonsoft.Json;                      // C#의 JSON document를 다루기 위한 가장 기본적인 라이브러리
using System.IO;                            // C#의 파일 입출력을 다루는 라이브러리
using System.Runtime.Serialization.Json;    // 객체를 JSON으로 변환하는 라이브러리


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiTest.Controllers
{
    [Route("api/[controller]")]
    public class AllCleSubjects2Controller : Controller
    {
        private ISubjectRepository2 _repo;   // ISubjectRepository class의 객체 _repo 생성.

        // 의존성 주입: ISubjectRepository의 인스턴스를 SubjectRepository의 인스턴스로
        // Startup.cs에서 설정한 DI를 생성자를 이용하여 받는 코드 구현이다.
        public AllCleSubjects2Controller(ISubjectRepository2 repo) // private _repo에 데이터를 넣는 메소드.
        {
            _repo = repo;
        }

        [HttpGet]
        public IEnumerable<Subject> Get()   // 실행 시 기본으로 출력되는 url. 기본적인 localhost 주소에 의해 출력되는 코드이다.
        {
            return _repo.GetSubjects();     // ISubjectRepository 클래스의 GetSubjects 메소드를 기본으로 url에 출력한다.
                                            // 모든 과목의 모든 데이터를 출력한다.
        }

        [HttpGet("{daylist}")]
        // _daylist= 과목시간1~8을 "|"를 기준으로 merge한 string. (ex. 월5, 화67 과목의 경우 "월5|화6|화7")
        public IEnumerable<Subject> Get(string daylist) // url 뒤에 daylist를 추가한 경우
                                                        // Client UI의 "전체"탭에서 임의의 과목을 선택할 때, "전체"탭에서 그 과목을 제외하고 과목을 출력하도록 기능 구현.
                                                        // 내 생각에는 이건 tap을 따로 두는게 좋을 것 같아. 전체 tap에서는 말그대로 전체를 보여주는게 유저 입장에서 직관적으로 좋을 것 같아. - 인수
        {
            return _repo.GetSubjects(daylist);          // ISubjectRepository 클래스의 String을 매개변수로 받는 GetSubjects 메소드를 출력한다.
        }

        [Route("subject/{subjectName}")]                // url 뒤에 과목명을 추가한 경우
        [HttpGet]
        public IEnumerable<Subject> GetOnlySubjects(string subjectName) // 과목 명을 검색했을 때, 그 과목 명에 해당하는 모든 과목을 출력하도록 한다.
                                                                        // 해당 메소드는 Client UI의 과목 목록에서 "과목 검색"기능에서 사용된다.
        {
            return _repo.GetOnlySubjects(subjectName);                  //과목명만 출력하는 메소드
        }

        [Route("{daylist}/{subjectName}")]                                      // 위의 [HttpGet("{daylist}")] method에 의해 시간표에 저장된 과목을 제외한 과목이 "전체 과목 tap"에서 출력 될 때, 과목을 검색할 때 사용된다.
                                                                                // 예를 들어, "컴퓨터구조"(학수번호1111-1~5)가 개설되었다고 하자.
                                                                                // 클라이언트가 "컴퓨터구조"(학수번호11111-1)를 선택한 경우, 이 메소드를 사용해서 "컴퓨터 구조" 검색하면 "컴퓨터구조"(학수번호1111-2~5)가 출력된다.
        [HttpGet]
        public IEnumerable<Subject> Get(string daylist, string subjectName)     // 그래서 매개변수로 daylist, subjectName를 모두 받는다.
        {
            return _repo.GetSubjects(daylist, subjectName);
        }

        [HttpPost]
        public Subject PostSubject([FromBody] Subject Subject)              // ID와 PW를 server에 전송하는 메소드
        {
            _repo.AddSubject(Subject);                                      // 로그인할 때 사용하는 메소드
            return Subject;
        }
    }
}
